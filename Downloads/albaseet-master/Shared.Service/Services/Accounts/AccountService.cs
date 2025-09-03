using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using Shared.Service.Logic.Tree;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.CoreOne.Models.StaticData.StaticData;
using static Shared.Helper.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Accounts
{
	public class AccountService : BaseService<Account>, IAccountService
	{
		private readonly IAccountCategoryService _accountCategoryService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IApplicationSettingService _applicationSettingService;
		private readonly IStringLocalizer<AccountService> _localizer;
		private readonly ICurrencyService _currencyService;
		private readonly ICostCenterService _costCenterService;
		private readonly ICompanyService _companyService;
		private readonly IBranchService _branchService;
		private readonly IStoreService _storeService;
		private List<int> _deletedAccounts = new();
		private List<AccountAutoCompleteDto> _treeName = new();
		private List<Account> _accounts = new();

		public AccountService(IRepository<Account> repository, IAccountCategoryService accountCategoryService, IHttpContextAccessor httpContextAccessor, IApplicationSettingService applicationSettingService, IStringLocalizer<AccountService> localizer, ICurrencyService currencyService, ICostCenterService costCenterService, ICompanyService companyService, IBranchService branchService, IStoreService storeService) : base(repository)
		{
			_accountCategoryService = accountCategoryService;
			_httpContextAccessor = httpContextAccessor;
			_applicationSettingService = applicationSettingService;
			_localizer = localizer;
			_currencyService = currencyService;
			_costCenterService = costCenterService;
			_companyService = companyService;
			_branchService = branchService;
			_storeService = storeService;
		}

		public IQueryable<AccountTreeDto> GetAccountsTree(int companyId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				(from account in _repository.GetAll().Where(x => x.CompanyId == companyId)
				from mainAccount in _repository.GetAll().Where(x => x.AccountId == account.MainAccountId).DefaultIfEmpty()
				from accountCategory in _accountCategoryService.GetAll().Where(x => x.AccountCategoryId == account.AccountCategoryId)
				select new AccountTreeDto
				{
					AccountId = account.AccountId,
					AccountCode = account.AccountCode,
					AccountCategoryId = account.AccountCategoryId,
					AccountTypeId = account.AccountTypeId,
					AccountName = language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn,
					AccountCategoryName = language == LanguageCode.Arabic ? accountCategory.AccountCategoryNameAr : accountCategory.AccountCategoryNameEn,
					AccountNameAr = account.AccountNameAr,
					AccountNameEn = account.AccountNameEn,
					IsMainAccount = account.IsMainAccount,
					MainAccountId = account.MainAccountId ?? 0,
					MainAccountCode = mainAccount != null ? mainAccount.AccountCode : "",
					AccountLevel = account.AccountLevel,
					IsLastLevel = account.IsLastLevel,
					Order = account.Order,
					CurrencyId = account.CurrencyId,
					IsCreatedAutomatically = account.IsCreatedAutomatically
				}).OrderBy(x=>x.AccountCode);
			return data;
		}

		public async Task<List<AccountTreeDto>> GetAccountsSimpleTreeByCompanyId(int companyId, int? mainAccountId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var accounts = await GetAll().Where(x => x.CompanyId == companyId && x.IsActive).Select(x => new AccountTreeDto()
			{
				AccountId = x.AccountId,
				AccountName = (language == LanguageCode.Arabic ? x.AccountNameAr : x.AccountNameEn) + $" ({x.AccountCode})",
				AccountCode = x.AccountCode,
				MainAccountId = x.MainAccountId ?? 0,
				IsMainAccount = x.IsMainAccount,
				AccountTypeId = x.AccountTypeId
			}).ToListAsync();

			if (mainAccountId > 0)
			{
				var currentAccount = accounts.FirstOrDefault(x => x.AccountId == mainAccountId);
				accounts = accounts.Where(x => x.MainAccountId == mainAccountId).ToList();
				if (currentAccount != null)
				{
					accounts.Add(currentAccount);
					var mainAccount = await GetMainAccount(currentAccount.MainAccountId);
					accounts.Add(mainAccount);
					if (mainAccount.MainAccountId > 0)
					{
						var mainAccount2 = await GetMainAccount(mainAccount.MainAccountId);
						accounts.Add(mainAccount2);
						if (mainAccount2.MainAccountId > 0)
						{
							var mainAccount3 = await GetMainAccount(mainAccount2.MainAccountId);
							accounts.Add(mainAccount3);
						}
					}
				}
			}
			return accounts.Where(x=> x.AccountTypeId != AccountTypeData.FractionalApproximationDifference).ToList();
		}

		public async Task<List<AccountTreeDto>> GetAccountsSimpleTreeByStoreId(int storeId, int? mainAccountId)
		{
			var companyId = await _storeService.GetCompanyIdByStoreId(storeId);
			return await GetAccountsSimpleTreeByCompanyId(companyId, mainAccountId);
		}

		public IQueryable<AccountNameDto> GetMainAccountsByAccountTypeId(int accountTypeId)
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =   _repository.GetAll().Where(x => x.CompanyId == companyId && x.AccountTypeId == accountTypeId && x.IsActive && x.IsMainAccount).Select(x => new AccountNameDto
			{
				AccountId = x.AccountId,
				AccountCode = x.AccountCode,
				AccountName = (language == LanguageCode.Arabic ? x.AccountNameAr : x.AccountNameEn) + $" ({x.AccountCode})",
				CurrencyId = x.CurrencyId
			});
			return data;
		}

		public async Task<AccountTreeDto> GetMainAccount(int accountId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return await _repository.GetAll().Where(x => x.AccountId == accountId).Select(x => new AccountTreeDto()
			{
				AccountId = x.AccountId,
				AccountName = (language == LanguageCode.Arabic ? x.AccountNameAr : x.AccountNameEn) + $" ({x.AccountCode})",
				AccountCode = x.AccountCode,
				MainAccountId = x.MainAccountId ?? 0,
				IsMainAccount = x.IsMainAccount
			}).FirstOrDefaultAsync() ?? new AccountTreeDto();
		}

		public async Task<AccountNameDto> GetMainAccountIdByAccountType(int accountTypeId)
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.AccountTypeId == accountTypeId && x.IsActive && x.IsMainAccount).Select(x => new AccountNameDto
			{
				AccountId = x.AccountId,
				AccountCode = x.AccountCode,
				AccountName = (language == LanguageCode.Arabic ? x.AccountNameAr : x.AccountNameEn) + $" ({x.AccountCode})",
				CurrencyId = x.CurrencyId
			}).FirstOrDefaultAsync() ?? new AccountNameDto();
		}

		public async Task<string> GetNextAccountCode(int companyId, int mainAccountId, bool isMainAccount)
		{
			var mainAccount = await _repository.GetAll().FirstOrDefaultAsync(x => x.AccountId == mainAccountId);
			var nextCode = await _repository.GetAll().CountAsync(x => x.MainAccountId == mainAccountId && x.IsMainAccount == isMainAccount && x.CompanyId == companyId) + 1;
			if (mainAccount != null)
			{
				var mainAccountCode = mainAccount.AccountCode ?? "";

				var mainAccountLength = await _applicationSettingService.GetApplicationSettingValueByCompanyId(companyId, ApplicationSettingDetailData.MainAccount) ?? "";
				var individualAccountLength = await _applicationSettingService.GetApplicationSettingValueByCompanyId(companyId, ApplicationSettingDetailData.IndividualAccount) ?? "";

				var accountCodes = await _repository.GetAll().Where(x => x.CompanyId == companyId).Select(x => x.AccountCode).ToListAsync();

				return TreeLogic.GenerateNextCode(accountCodes, mainAccountCode, isMainAccount, mainAccountLength, individualAccountLength, nextCode);
			}
			return "";
		}

		public async Task<List<AccountAutoCompleteDto>> GetMainAccountsByAccountCode(int companyId, string accountCode)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.IsMainAccount && x.AccountCode!.ToLower().Contains(accountCode.Trim().ToLower()) && x.IsActive).Select(s => new AccountAutoCompleteDto
			{
				AccountCode = s.AccountCode,
				AccountId = s.AccountId,
				AccountName = language == LanguageCode.Arabic ? s.AccountNameAr : s.AccountNameEn,
				AccountTypeId = s.AccountTypeId
			}).Take(10).ToListAsync();
		}

		public async Task<List<AccountAutoCompleteDto>> GetMainAccountsByAccountName(int companyId, string accountName)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			if (language == LanguageCode.Arabic)
			{
				return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.IsMainAccount && x.AccountNameAr!.ToLower().Contains(accountName.Trim().ToLower()) && x.IsActive).Select(s => new AccountAutoCompleteDto
				{
					AccountCode = s.AccountCode,
					AccountId = s.AccountId,
					AccountName = s.AccountNameAr,
					AccountTypeId = s.AccountTypeId
				}).Take(10).ToListAsync();
			}
			else
			{
				return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.IsMainAccount && x.AccountNameEn!.ToLower().Contains(accountName.Trim().ToLower()) && x.IsActive).Select(s => new AccountAutoCompleteDto
				{
					AccountCode = s.AccountCode,
					AccountId = s.AccountId,
					AccountName = s.AccountNameEn
				}).Take(10).ToListAsync();
			}
		}

		public Task<List<AccountAutoCompleteDto>> GetIndividualAccountsByAccountCode(int storeId, string accountCode)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				(from account in _repository.GetAll().Where(x => !x.IsMainAccount && x.AccountCode!.ToLower().Contains(accountCode.Trim().ToLower()) && x.IsActive)
				 from company in _companyService.GetAll().Where(x => x.CompanyId == account.CompanyId && x.IsActive)
				 from branch in _branchService.GetAll().Where(x => x.CompanyId == company.CompanyId && x.IsActive)
				 from store in _storeService.GetAll().Where(x => x.BranchId == branch.BranchId && x.StoreId == storeId && x.IsActive)
				 select new AccountAutoCompleteDto
				 {
					 AccountCode = account.AccountCode,
					 AccountId = account.AccountId,
					 AccountName = language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn,
					 AccountTypeId = account.AccountTypeId
				 }).Take(10).ToListAsync();
			return data;
		}

		public Task<List<AccountAutoCompleteDto>> GetIndividualAccountsByAccountName(int storeId, string accountName)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			if (language == LanguageCode.Arabic)
			{
				var data =
					(from account in _repository.GetAll().Where(x => !x.IsMainAccount && x.AccountNameAr!.ToLower().Contains(accountName.Trim().ToLower()) && x.IsActive)
					 from company in _companyService.GetAll().Where(x => x.CompanyId == account.CompanyId && x.IsActive)
					 from branch in _branchService.GetAll().Where(x => x.CompanyId == company.CompanyId && x.IsActive)
					 from store in _storeService.GetAll().Where(x => x.BranchId == branch.BranchId && x.StoreId == storeId && x.IsActive)
					 select new AccountAutoCompleteDto
					 {
						 AccountCode = account.AccountCode,
						 AccountId = account.AccountId,
						 AccountName = account.AccountNameAr,
						 AccountTypeId = account.AccountTypeId
					 }).Take(10).ToListAsync();
				return data;
			}
			else
			{
				var data =
					(from account in _repository.GetAll().Where(x => !x.IsMainAccount && x.AccountNameEn!.ToLower().Contains(accountName.Trim().ToLower()) && x.IsActive)
					 from company in _companyService.GetAll().Where(x => x.CompanyId == account.CompanyId && x.IsActive)
					 from branch in _branchService.GetAll().Where(x => x.CompanyId == company.CompanyId && x.IsActive)
					 from store in _storeService.GetAll().Where(x => x.BranchId == branch.BranchId && x.StoreId == storeId && x.IsActive)
					 select new AccountAutoCompleteDto
					 {
						 AccountCode = account.AccountCode,
						 AccountId = account.AccountId,
						 AccountName = account.AccountNameEn,
						 AccountTypeId = account.AccountTypeId
					 }).Take(10).ToListAsync();
				return data;
			}
		}

		public IQueryable<AccountNameDto> GetIndividualAccountsByCompanyId(int companyId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
			(
				from account in _repository.GetAll().Where(x => !x.IsMainAccount && x.IsActive && x.CompanyId == companyId)
				select new AccountNameDto
				{
					AccountCode = account.AccountCode,
					AccountId = account.AccountId,
					AccountName = (language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn) + $" ({account.AccountCode})",
					CurrencyId = account.CurrencyId
				});
			return data;
		}

		public IQueryable<AccountNameDto> GetIndividualAccounts(int storeId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				(
				 //from account in _repository.GetAll().Where(x => !x.IsMainAccount && x.IsActive && x.AccountTypeId != AccountTypeData.FractionalApproximationDifference)
				 from account in _repository.GetAll().Where(x => !x.IsMainAccount && x.IsActive)
				 from company in _companyService.GetAll().Where(x => x.CompanyId == account.CompanyId && x.IsActive)
				 from branch in _branchService.GetAll().Where(x => x.CompanyId == company.CompanyId && x.IsActive)
				 from store in _storeService.GetAll().Where(x => x.BranchId == branch.BranchId && x.StoreId == storeId && x.IsActive)
				 select new AccountNameDto
				 {
					 AccountCode = account.AccountCode,
					 AccountId = account.AccountId,
					 AccountName = (language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn) + $" ({account.AccountCode})",
					 CurrencyId = account.CurrencyId
				 });
			return data;
		}

		public async Task<ResponseDto> CreateMainAccounts(int companyId, short currencyId)
		{
			var mainAccount = await _applicationSettingService.GetApplicationSettingValueByCompanyId(companyId, ApplicationSettingDetailData.MainAccount) ?? "";
			var individualAccount = await _applicationSettingService.GetApplicationSettingValueByCompanyId(companyId, ApplicationSettingDetailData.IndividualAccount) ?? "";
			var nextId = await GetNextId();
			var accountList = new List<Account>()
			{
				new()
				{
					AccountId = nextId,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Assets,
					AccountNameAr = "الاصول",
					AccountNameEn = "Assets",
					AccountCode = 1.ToString($"D{mainAccount}"),
					AccountLevel = 1,
					IsMainAccount = true,
					MainAccountId = null,
					Order = 1,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+1,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Liabilities,
					AccountNameAr = "الخصوم",
					AccountNameEn = "Liabilities",
					AccountCode = 2.ToString($"D{mainAccount}"),
					AccountLevel = 1,
					IsMainAccount = true,
					MainAccountId = null,
					Order = 2,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+2,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Expenses,
					AccountNameAr = "المصروفات",
					AccountNameEn = "Expenses",
					AccountCode = 3.ToString($"D{mainAccount}"),
					AccountLevel = 1,
					IsMainAccount = true,
					MainAccountId = null,
					Order = 3,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+3,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Revenues,
					AccountNameAr = "الايرادات",
					AccountNameEn = "Revenues",
					AccountCode = 4.ToString($"D{mainAccount}"),
					AccountLevel = 1,
					IsMainAccount = true,
					MainAccountId = null,
					Order = 4,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+4,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Assets,
					AccountNameAr = "الاصول المتداولة",
					AccountNameEn = "Current Assets",
					AccountCode = 1.ToString($"D{mainAccount}") + 1.ToString($"D{mainAccount}") ,
					AccountLevel = 2,
					IsMainAccount = true,
					MainAccountId = nextId,
					Order = 1,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+5,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Assets,
					AccountTypeId = AccountTypeData.FixedAssets,
					AccountNameAr = "الاصول الثابتة",
					AccountNameEn = "Fixed Assets",
					AccountCode = 1.ToString($"D{mainAccount}")+ 2.ToString($"D{mainAccount}"),
					AccountLevel = 2,
					IsMainAccount = true,
					MainAccountId = nextId,
					Order = 2,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+6,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Assets,
					AccountNameAr = "اخري",
					AccountNameEn = "Other",
					AccountCode = 1.ToString($"D{mainAccount}") + 3.ToString($"D{mainAccount}"),
					AccountLevel = 2,
					IsMainAccount = true,
					MainAccountId = nextId,
					Order = 3,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+7,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Liabilities,
					AccountNameAr = "الخصوم المتداولة",
					AccountNameEn = "Current Liabilities",
					AccountCode =2.ToString($"D{mainAccount}") +  1.ToString($"D{mainAccount}"),
					AccountLevel = 2,
					IsMainAccount = true,
					MainAccountId = nextId+1,
					Order = 1,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+8,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Liabilities,
					AccountTypeId = AccountTypeData.OwnershipEquity,
					AccountNameAr = "حقوق الملكية",
					AccountNameEn = "Ownership Equity",
					AccountCode = 2.ToString($"D{mainAccount}") + 2.ToString($"D{mainAccount}"),
					AccountLevel = 2,
					IsMainAccount = true,
					MainAccountId = nextId+1,
					Order = 2,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+9,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Liabilities,
					AccountTypeId = AccountTypeData.AccumulatedDepreciation,
					AccountNameAr = "مجمع الاهلاك",
					AccountNameEn = "Accumulated Depreciation",
					AccountCode =  2.ToString($"D{mainAccount}") + 3.ToString($"D{mainAccount}"),
					AccountLevel = 2,
					IsMainAccount = true,
					MainAccountId = nextId+1,
					Order = 3,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+10,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Expenses,
					AccountTypeId = AccountTypeData.Purchases,
					AccountNameAr = "المشتريات",
					AccountNameEn = "Purchases",
					AccountCode = 3.ToString($"D{mainAccount}") + 1.ToString($"D{mainAccount}"),
					AccountLevel = 2,
					IsMainAccount = true,
					MainAccountId = nextId+2,
					Order = 1,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+11,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Expenses,
					AccountTypeId = AccountTypeData.MiscellaneousExpenses,
					AccountNameAr = "مصروفات عمومية وادارية",
					AccountNameEn = "Miscellaneous Expenses",
					AccountCode = 3.ToString($"D{mainAccount}") + 2.ToString($"D{mainAccount}"),
					AccountLevel = 2,
					IsMainAccount = true,
					MainAccountId = nextId+2,
					Order = 2,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+12,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Expenses,
					AccountTypeId = AccountTypeData.Depreciation,
					AccountNameAr = "الاهلاكات",
					AccountNameEn = "Depreciation",
					AccountCode = 3.ToString($"D{mainAccount}") + 3.ToString($"D{mainAccount}"),
					AccountLevel = 2,
					IsMainAccount = true,
					MainAccountId = nextId+2,
					Order = 3,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+13,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Expenses,
					AccountTypeId = AccountTypeData.RevenuesCost,
					AccountNameAr = "تكلفة الايرادات",
					AccountNameEn = "Revenues Cost",
					AccountCode = 3.ToString($"D{mainAccount}") + 4.ToString($"D{mainAccount}"),
					AccountLevel = 2,
					IsMainAccount = true,
					MainAccountId = nextId+2,
					Order = 4,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+14,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Revenues,
					AccountTypeId = AccountTypeData.Sales,
					AccountNameAr = "المبيعات",
					AccountNameEn = "Sales",
					AccountCode = 4.ToString($"D{mainAccount}") + 1.ToString($"D{mainAccount}"),
					AccountLevel = 2,
					IsMainAccount = true,
					MainAccountId = nextId+3,
					Order = 1,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+15,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Revenues,
					AccountTypeId = AccountTypeData.MiscellaneousIncome,
					AccountNameAr = "ايرادات متنوعة",
					AccountNameEn = "Miscellaneous Income",
					AccountCode = 4.ToString($"D{mainAccount}") + 2.ToString($"D{mainAccount}"),
					AccountLevel = 2,
					IsMainAccount = true,
					MainAccountId = nextId+3,
					Order = 2,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+16,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Assets,
					AccountTypeId = AccountTypeData.Cash,
					AccountNameAr = "النقدية/الصندوق",
					AccountNameEn = "Cash",
					AccountCode = 1.ToString($"D{mainAccount}") + 1.ToString($"D{mainAccount}") + 1.ToString($"D{mainAccount}"),
					AccountLevel = 3,
					IsMainAccount = true,
					MainAccountId = nextId+4,
					Order = 1,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+17,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Assets,
					AccountTypeId = AccountTypeData.Banks,
					AccountNameAr = "البنوك",
					AccountNameEn = "Banks",
					AccountCode = 1.ToString($"D{mainAccount}") + 1.ToString($"D{mainAccount}") + 2.ToString($"D{mainAccount}"),
					AccountLevel = 3,
					IsMainAccount = true,
					MainAccountId = nextId+4,
					Order = 2,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+18,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Assets,
					AccountTypeId = AccountTypeData.Clients,
					AccountNameAr = "العملاء",
					AccountNameEn = "Clients",
					AccountCode = 1.ToString($"D{mainAccount}") + 1.ToString($"D{mainAccount}") + 3.ToString($"D{mainAccount}"),
					AccountLevel = 3,
					IsMainAccount = true,
					MainAccountId = nextId+4,
					Order = 3,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+19,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Liabilities,
					AccountTypeId = AccountTypeData.Suppliers,
					AccountNameAr = "الموردين",
					AccountNameEn = "Suppliers",
					AccountCode = 2.ToString($"D{mainAccount}") + 1.ToString($"D{mainAccount}") + 1.ToString($"D{mainAccount}"),
					AccountLevel = 3,
					IsMainAccount = true,
					MainAccountId = nextId+7,
					Order = 1,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+20,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Expenses,
					AccountTypeId = AccountTypeData.FractionalApproximationDifference,
					AccountNameAr = "فرق تقريب كسور",
					AccountNameEn = "Fractional Approximation Difference",
					AccountCode = 3.ToString($"D{mainAccount}") + 2.ToString($"D{mainAccount}") + 1.ToString($"D{individualAccount}"),
					AccountLevel = 3,
					IsMainAccount = false,
					MainAccountId = nextId+11,
					Order = 1,
					IsLastLevel = true,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsNonDeletable = true,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+21,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Expenses,
					AccountTypeId = AccountTypeData.AllowedDiscount,
					AccountNameAr = "خصم مسموح به",
					AccountNameEn = "Allowed Discount",
					AccountCode = 3.ToString($"D{mainAccount}") + 2.ToString($"D{mainAccount}") + 2.ToString($"D{individualAccount}"),
					AccountLevel = 3,
					IsMainAccount = false,
					MainAccountId = nextId+11,
					Order = 2,
					IsLastLevel = true,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsNonDeletable = true,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+22,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Assets,
					AccountTypeId = AccountTypeData.Inventory,
					AccountNameAr = "المخزون",
					AccountNameEn = "Inventory",
					AccountCode = 1.ToString($"D{mainAccount}") + 1.ToString($"D{mainAccount}") + 4.ToString($"D{mainAccount}"),
					AccountLevel = 3,
					IsMainAccount = true,
					MainAccountId = nextId+4,
					Order = 4,
					IsLastLevel = false,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+23,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Assets,
					AccountTypeId = AccountTypeData.InventoryAccount,
					AccountNameAr = "حساب المخزون",
					AccountNameEn = "Inventory Account",
					AccountCode = 1.ToString($"D{mainAccount}") + 1.ToString($"D{mainAccount}") + 4.ToString($"D{mainAccount}") + 1.ToString($"D{individualAccount}"),
					AccountLevel = 4,
					IsMainAccount = false,
					MainAccountId = nextId+22,
					Order = 1,
					IsLastLevel = true,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
				new()
				{
					AccountId = nextId+24,
					CompanyId = companyId,
					CurrencyId = currencyId,
					AccountCategoryId = AccountCategoryData.Expenses,
					AccountTypeId = AccountTypeData.RevenuesCostAccount,
					AccountNameAr = "حساب تكلفة المبيعات",
					AccountNameEn = "Revenues Cost Account",
					AccountCode = 3.ToString($"D{mainAccount}") + 4.ToString($"D{mainAccount}") + 1.ToString($"D{individualAccount}"),
					AccountLevel = 3,
					IsMainAccount = false,
					MainAccountId = nextId+13,
					Order = 1,
					IsLastLevel = true,
					HasRemarks = false,
					IsPrivate = false,
					IsNonEditable = false,
					IsActive = true,
					IsCreatedAutomatically = true,
					HasCostCenter = false,
					CostCenterId = null,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
				},
			};
			await _repository.InsertRange(accountList);
			await _repository.SaveChanges();
			return new ResponseDto() { Success = true, Message = "" };
		}

		public IQueryable<AccountDto> GetAllAccounts()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from account in _repository.GetAll()
				from mainAccount in _repository.GetAll().Where(x => x.AccountId == account.MainAccountId).DefaultIfEmpty()
				from currency in _currencyService.GetAll().Where(x => x.CurrencyId == account.CurrencyId).DefaultIfEmpty()
				from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == account.CostCenterId).DefaultIfEmpty()
				select new AccountDto()
				{
					AccountId = account.AccountId,
					AccountCode = account.AccountCode,
					CompanyId = account.CompanyId,
					AccountCategoryId = account.AccountCategoryId,
					AccountTypeId = account.AccountTypeId,
					CurrencyId = account.CurrencyId,
					AccountNameAr = account.AccountNameAr,
					AccountNameEn = account.AccountNameEn,
					IsMainAccount = account.IsMainAccount,
					MainAccountId = account.MainAccountId ?? 0,
					MainAccountCode = mainAccount != null ? mainAccount.AccountCode : null,
					MainAccountName = mainAccount != null ? (language == LanguageCode.Arabic ? mainAccount.AccountNameAr : mainAccount.AccountNameEn) : "",
					CostCenterId = account.CostCenterId ?? 0,
					HasCostCenter = account.HasCostCenter,
					Order = account.Order,
					AccountLevel = account.AccountLevel,
					HasRemarks = account.HasRemarks,
					RemarksAr = account.RemarksAr,
					RemarksEn = account.RemarksEn,
					NotesAr = account.NotesAr,
					NotesEn = account.NotesEn,
					IsPrivate = account.IsPrivate,
					IsNonEditable = account.IsNonEditable,
					IsLastLevel = account.IsLastLevel,
					IsActive = account.IsActive,
					IsCreatedAutomatically = account.IsCreatedAutomatically,
					InActiveReasons = account.InActiveReasons,
					CurrencyName = currency != null ? (language == LanguageCode.Arabic ? currency.CurrencyNameAr : currency.CurrencyNameEn) : "",
					CostCenterName = costCenter != null ? (language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn) : "",
					InternalReferenceAccountId = account.InternalReferenceAccountId,
				};
			return data;
		}

		public IQueryable<AccountDto> GetCompanyAccounts(int companyId)
		{
			return GetAllAccounts().Where(x => x.CompanyId == companyId);
		}

		public async Task<AccountDto> GetAccountByAccountId(int accountId)
		{
			var accountDb = await GetAllAccounts().FirstOrDefaultAsync(x => x.AccountId == accountId) ?? new AccountDto();
			accountDb.HasChildren = await IsAccountHasChildren(accountId);
			return accountDb;
		}

		public async Task<bool> IsAccountHasChildren(int accountId)
		{
			return await _repository.GetAll().AnyAsync(x => x.MainAccountId == accountId);
		}

		public async Task<AccountDto> GetAccountByAccountCode(int companyId, string accountCode)
		{
			return await GetAllAccounts().FirstOrDefaultAsync(x => x.CompanyId == companyId && x.AccountCode!.Trim() == accountCode.Trim()) ?? new AccountDto();
		}

		public List<RequestChangesDto> GetAccountRequestChanges(AccountDto oldItem, AccountDto newItem)
		{
			return CompareLogic.GetDifferences(oldItem, newItem);
		}

		public async Task<AccountDto> GetRootAccountByCatgoryId(int companyId, byte accountCategoryId)
		{
			return await GetAllAccounts().FirstOrDefaultAsync(x => x.CompanyId == companyId && x.AccountCategoryId == accountCategoryId && x.MainAccountId == 0) ?? new AccountDto();
		}

		public async Task<AccountDto> GetFractionalApproximationDifferenceAccount(int companyId)
		{
			return await GetAllAccounts().FirstOrDefaultAsync(x => x.CompanyId == companyId && x.AccountTypeId == AccountTypeData.FractionalApproximationDifference) ?? new AccountDto();
		}

		public async Task<AccountDto> GetAllowedDiscountAccount(int companyId)
		{
			return await GetAllAccounts().FirstOrDefaultAsync(x => x.CompanyId == companyId && x.AccountTypeId == AccountTypeData.AllowedDiscount) ?? new AccountDto();
		}

		public async Task<AccountDto> GetFixedAssetsAccount(int companyId)
		{
			return await GetAllAccounts().FirstOrDefaultAsync(x => x.CompanyId == companyId && x.AccountTypeId == AccountTypeData.FixedAssets && x.AccountLevel == 2) ?? new AccountDto();
		}

		public async Task<AccountDto> GetDepreciationAccount(int companyId)
		{
			return await GetAllAccounts().FirstOrDefaultAsync(x => x.CompanyId == companyId && x.AccountTypeId == AccountTypeData.Depreciation && x.AccountLevel == 2) ?? new AccountDto();
		}

		public async Task<AccountDto> GetAccumulatedDepreciationAccount(int companyId)
		{
			return await GetAllAccounts().FirstOrDefaultAsync(x => x.CompanyId == companyId && x.AccountTypeId == AccountTypeData.AccumulatedDepreciation && x.AccountLevel == 2) ?? new AccountDto();
		}

		public async Task<AccountDto> GetMirrorDepreciationAccount(int accountId)
		{
			return await GetAllAccounts().FirstOrDefaultAsync(x => x.InternalReferenceAccountId == accountId && x.AccountTypeId == AccountTypeData.Depreciation) ?? new AccountDto();
		}

		public async Task<AccountDto> GetMirrorAccumulatedDepreciationAccount(int accountId)
		{
			return await GetAllAccounts().FirstOrDefaultAsync(x => x.InternalReferenceAccountId == accountId && x.AccountTypeId == AccountTypeData.AccumulatedDepreciation) ?? new AccountDto();
		}

		public async Task<AccountDto> GetFractionalApproximationDifferenceAccountByStoreId(int storeId)
		{
			var companyId = await _storeService.GetCompanyIdByStoreId(storeId);
			return await GetFractionalApproximationDifferenceAccount(companyId);
		}

		public async Task<AccountDto> GetAllowedDiscountAccountByStoreId(int storeId)
		{
			var companyId = await _storeService.GetCompanyIdByStoreId(storeId);
			return await GetAllowedDiscountAccount(companyId);
		}

		public async Task<ResponseDto> SaveAccount(AccountDto account)
		{
			var result = await SaveAccountInternal(account);
			if (result.Success && account.AccountTypeId == AccountTypeData.FixedAssets)
			{
				await SaveDepreciationAccount(result.Id, account);
				await SaveAccumulatedDepreciationAccount(result.Id, account);
			}
			return result;
		}

		private async Task<ResponseDto> SaveAccountInternal(AccountDto account)
		{
			var accountExist = await IsAccountExist(account.AccountId, account.AccountNameAr, account.AccountNameEn, account.CompanyId, account.AccountCode!);
			if (accountExist.Success)
			{
				return new ResponseDto() { Id = accountExist.Id, Success = false, Message = _localizer["AccountAlreadyExist"] };
			}
			else
			{
				ResponseDto result;
				if (account.AccountId == 0)
				{
					result = await CreateAccount(account);
				}
				else
				{
					result = await UpdateAccount(account);
				}

				return result;
			}
		}

		public async Task<FixedAssetAccountReturnDto> SaveFixedAssetAccount(FixedAssetAccountDto account)
		{
			var accountDto = await GetAccountByAccountId(account.AccountId);
			if (accountDto.AccountId == 0)
			{
				var mainAccount = account.MainAccountId != null ?
					await GetAccountByAccountId((int)account.MainAccountId) :
					await GetFixedAssetsAccount(account.CompanyId); 

				accountDto.AccountCode = await GetNextAccountCode(account.CompanyId, mainAccount.AccountId, false);
				accountDto.CompanyId = account.CompanyId;
				accountDto.AccountCategoryId = AccountCategoryData.Assets;
				accountDto.AccountTypeId = AccountTypeData.FixedAssets;
				accountDto.IsMainAccount = account.IsMainAccount ?? false;
				accountDto.MainAccountId = mainAccount.AccountId;
				accountDto.AccountLevel = (byte)(mainAccount.AccountLevel + 1);
				accountDto.Order = mainAccount.Order + 1;
				accountDto.IsLastLevel = !(account.IsMainAccount ?? false);
				accountDto.CurrencyId = (await _companyService.GetCompanyById(account.CompanyId))!.CurrencyId;
				accountDto.HasCostCenter = false;
				accountDto.CostCenterId = null;
				accountDto.IsPrivate = false;
				accountDto.IsActive = true;
				accountDto.IsCreatedAutomatically = false;
				accountDto.InActiveReasons = null;
				accountDto.HasRemarks = false;
				accountDto.RemarksAr = null;
				accountDto.RemarksEn = null;
				accountDto.IsNonEditable = false;
				accountDto.NotesAr = null;
				accountDto.NotesEn = null;
				accountDto.ArchiveHeaderId = null;
				accountDto.InternalReferenceAccountId = null;
				accountDto.HasChildren = false;
			}

			accountDto.AccountNameAr = account.FixedAssetNameAr;
			accountDto.AccountNameEn = account.FixedAssetNameEn;

            var result = await IsAccountExist(accountDto.AccountId, accountDto.AccountNameAr, accountDto.AccountNameEn, accountDto.CompanyId, accountDto.AccountCode!);
			int accountId = result.Id;
            if (!result.Success)
            {
                result = await SaveAccountInternal(accountDto);
				accountId = result.Id;
            }
            var depreciationAccountId = await SaveDepreciationAccount(accountId, accountDto);
            var accumulatedDepreciationAcccountId = await SaveAccumulatedDepreciationAccount(accountId, accountDto);

            return new FixedAssetAccountReturnDto()
			{
				AccountId = accountId,
				DepreciationAccountId = depreciationAccountId,
				CumulativeDepreciationAccountId = accumulatedDepreciationAcccountId,
				Result = result,
			};
		}

		public async Task<int> SaveDepreciationAccount(int resultAccountId, AccountDto account)
		{
			AccountDto generatedAccount = await GetMirrorDepreciationAccount(resultAccountId);

			generatedAccount.AccountNameAr = "إهلاك " + account.AccountNameAr;
			generatedAccount.AccountNameEn = account.AccountNameEn + " Depreciation";

			if (generatedAccount.AccountId == 0)
			{
				var mirrorMainAccount = (await GetMirrorDepreciationAccount((int)account.MainAccountId!));
				if (mirrorMainAccount.AccountId == 0)
				{
					mirrorMainAccount = (await GetDepreciationAccount(account.CompanyId));
				}

				generatedAccount.MainAccountId = mirrorMainAccount.AccountId;
				generatedAccount.AccountCode = await GetNextAccountCode(account.CompanyId, (int)mirrorMainAccount.AccountId, account.IsMainAccount);
				generatedAccount.InternalReferenceAccountId = resultAccountId;
			}

			generatedAccount.CompanyId = account.CompanyId;
			generatedAccount.AccountCategoryId = AccountCategoryData.Expenses;
			generatedAccount.AccountTypeId = AccountTypeData.Depreciation;
			generatedAccount.IsMainAccount = account.IsMainAccount;
			generatedAccount.AccountLevel = account.AccountLevel;
			generatedAccount.Order = account.Order;
			generatedAccount.IsLastLevel = account.IsLastLevel;
			generatedAccount.CurrencyId = account.CurrencyId;
			generatedAccount.HasCostCenter = account.HasCostCenter;
			generatedAccount.CostCenterId = account.CostCenterId;
			generatedAccount.IsPrivate = account.IsPrivate;
			generatedAccount.IsActive = account.IsActive;
			generatedAccount.IsCreatedAutomatically = true;
			generatedAccount.InActiveReasons = account.InActiveReasons;
			generatedAccount.HasRemarks = account.HasRemarks;
			generatedAccount.RemarksAr = account.RemarksAr;
			generatedAccount.RemarksEn = account.RemarksEn;
			generatedAccount.IsNonEditable = account.IsNonEditable;
			generatedAccount.NotesAr = account.NotesAr;
			generatedAccount.NotesEn = account.NotesEn;
			generatedAccount.ArchiveHeaderId = null;

			var result = await SaveAccountInternal(generatedAccount);
			return result.Id;
		}

		public async Task<int> SaveAccumulatedDepreciationAccount(int resultAccountId, AccountDto account)
		{
			AccountDto generatedAccount = await GetMirrorAccumulatedDepreciationAccount(resultAccountId);

			generatedAccount.AccountNameAr = "مجمع إهلاك " + account.AccountNameAr;
			generatedAccount.AccountNameEn = account.AccountNameEn + " Accumulated Depreciation";

			if (generatedAccount.AccountId == 0)
			{
				var mirrorMainAccount = (await GetMirrorAccumulatedDepreciationAccount((int)account.MainAccountId!));
				if (mirrorMainAccount.AccountId == 0)
				{
					mirrorMainAccount = (await GetAccumulatedDepreciationAccount(account.CompanyId));
				}

				generatedAccount.MainAccountId = mirrorMainAccount.AccountId;
				generatedAccount.AccountCode = await GetNextAccountCode(account.CompanyId, (int)mirrorMainAccount.AccountId, account.IsMainAccount);
				generatedAccount.InternalReferenceAccountId = resultAccountId;
			}

			generatedAccount.CompanyId = account.CompanyId;
			generatedAccount.AccountCategoryId = AccountCategoryData.Liabilities;
			generatedAccount.AccountTypeId = AccountTypeData.AccumulatedDepreciation;
			generatedAccount.IsMainAccount = account.IsMainAccount;
			generatedAccount.AccountLevel = account.AccountLevel;
			generatedAccount.Order = account.Order;
			generatedAccount.IsLastLevel = account.IsLastLevel;
			generatedAccount.CurrencyId = account.CurrencyId;
			generatedAccount.HasCostCenter = account.HasCostCenter;
			generatedAccount.CostCenterId = account.CostCenterId;
			generatedAccount.IsPrivate = account.IsPrivate;
			generatedAccount.IsActive = account.IsActive;
			generatedAccount.IsCreatedAutomatically = true;
			generatedAccount.InActiveReasons = account.InActiveReasons;
			generatedAccount.HasRemarks = account.HasRemarks;
			generatedAccount.RemarksAr = account.RemarksAr;
			generatedAccount.RemarksEn = account.RemarksEn;
			generatedAccount.IsNonEditable = account.IsNonEditable;
			generatedAccount.NotesAr = account.NotesAr;
			generatedAccount.NotesEn = account.NotesEn;
			generatedAccount.ArchiveHeaderId = null;

			var result = await SaveAccountInternal(generatedAccount);
			return result.Id;
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.AccountId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> IsAccountExist(int id, string? nameAr, string? nameEn, int companyId, string accountCode)
		{
			var account = await _repository.GetAll().FirstOrDefaultAsync(x => (x.AccountNameAr == nameAr || x.AccountNameEn == nameEn || x.AccountNameAr == nameEn || x.AccountNameEn == nameAr || x.AccountCode!.Trim() == accountCode.Trim()) && x.AccountId != id && x.CompanyId == companyId);
			if (account != null)
			{
				return new ResponseDto() { Id = account.AccountId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<ResponseDto> CreateAccount(AccountDto account)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var mainAccountDb = await _repository.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.AccountId == account.MainAccountId);

			var newAccount = new Account()
			{
				AccountId = await GetNextId(),
				AccountNameAr = account?.AccountNameAr?.Trim(),
				AccountNameEn = account?.AccountNameEn?.Trim(),
				IsMainAccount = account!.IsMainAccount,
				MainAccountId = account.MainAccountId != 0 ? account.MainAccountId : null,
				AccountCode = account!.AccountCode?.Trim(),
				CompanyId = account!.CompanyId,
				CurrencyId = account!.CurrencyId,
				CostCenterId = account.CostCenterId != 0 ? account.CostCenterId : null,
				HasCostCenter = account.HasCostCenter,
				Order = account.Order,
				AccountCategoryId = account.AccountCategoryId,
				AccountTypeId = mainAccountDb != null ? mainAccountDb.AccountTypeId : account.AccountTypeId,
				AccountLevel = (byte)(mainAccountDb != null ? mainAccountDb.AccountLevel + 1 : account.AccountLevel),
				HasRemarks = account.HasRemarks,
				IsLastLevel = !account.IsMainAccount,
				IsPrivate = account.IsPrivate,
				RemarksAr = account.RemarksAr?.Trim(),
				RemarksEn = account.RemarksEn?.Trim(),
				IsNonEditable = account.IsNonEditable,
				NotesAr = account.NotesAr?.Trim(),
				NotesEn = account.NotesEn?.Trim(),
				IsActive = account!.IsActive,
				InActiveReasons = account?.InActiveReasons,
				InternalReferenceAccountId = account?.InternalReferenceAccountId,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false
			};

			var accountValidator = await new AccountValidator(_localizer).ValidateAsync(newAccount);
			var validationResult = accountValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newAccount);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newAccount.AccountId, Success = true, Message = _localizer["NewAccountSuccessMessage", ((language == LanguageCode.Arabic ? newAccount.AccountNameAr : newAccount.AccountNameEn) ?? ""), newAccount.AccountCode!] };
			}
			else
			{
				return new ResponseDto() { Id = newAccount.AccountId, Success = false, Message = accountValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateAccount(AccountDto account)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var accountDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.AccountId == account.AccountId);
			if (accountDb != null)
			{
				var mainAccountDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.AccountId == account.MainAccountId);
				accountDb.AccountNameAr = account.AccountNameAr?.Trim();
				accountDb.AccountNameEn = account.AccountNameEn?.Trim();
				accountDb.CurrencyId = account!.CurrencyId;
				accountDb.Hide = false;
				accountDb.IsActive = (bool)account?.IsActive;
				accountDb.InActiveReasons = account?.InActiveReasons;
				accountDb.IsMainAccount = account!.IsMainAccount;
				accountDb.MainAccountId = account.MainAccountId != 0 ? account.MainAccountId : null;
				accountDb.AccountCode = account!.AccountCode?.Trim();
				accountDb.CompanyId = account!.CompanyId;
				accountDb.CurrencyId = account!.CurrencyId;
				accountDb.CostCenterId = account.CostCenterId != 0 ? account.CostCenterId : null;
				accountDb.HasCostCenter = account.HasCostCenter;
				accountDb.Order = account.Order;
				accountDb.AccountCategoryId = account.AccountCategoryId;
				accountDb.AccountTypeId = mainAccountDb != null ? mainAccountDb.AccountTypeId : account.AccountTypeId;
				accountDb.AccountLevel = (byte)(mainAccountDb != null ? mainAccountDb.AccountLevel + 1 : account.AccountLevel);
				accountDb.HasRemarks = account.HasRemarks;
				accountDb.IsLastLevel = !account.IsMainAccount;
				accountDb.IsPrivate = account.IsPrivate;
				accountDb.RemarksAr = account.RemarksAr?.Trim();
				accountDb.RemarksEn = account.RemarksEn?.Trim();
				accountDb.IsNonEditable = account.IsNonEditable;
				accountDb.NotesAr = account.NotesAr?.Trim();
				accountDb.NotesEn = account.NotesEn?.Trim();
				accountDb.ModifiedAt = DateHelper.GetDateTimeNow();
				accountDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				accountDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var accountValidator = await new AccountValidator(_localizer).ValidateAsync(accountDb);
				var validationResult = accountValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(accountDb);
					await _repository.SaveChanges();
					await ReorderAccountsLevels(accountDb.AccountId, accountDb.CompanyId);
					return new ResponseDto() { Id = accountDb.AccountId, Success = true, Message = _localizer["UpdateAccountSuccessMessage", ((language == LanguageCode.Arabic ? accountDb.AccountNameAr : accountDb.AccountNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = accountValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoAccountFound"] };
		}

		public async Task<bool> ReorderAccountsLevels(int accountId, int companyId)
		{
			var nextLevel = 1;
			var accountDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.AccountId == accountId);
			if (accountDb != null)
			{
				var mainAccountId = accountDb.MainAccountId;
				var parent = await _repository.GetAll().FirstOrDefaultAsync(x => x.AccountId == mainAccountId);
				if (parent != null)
				{
					nextLevel = parent.AccountLevel + 1;
				}
				var accountsDb = await _repository.GetAll().Where(x => x.CompanyId == companyId).ToListAsync();

				var tree = GetAccountTreeByAccountId(accountsDb, accountId);
				LoopThroughAccounts(accountsDb, tree, nextLevel);
				if (_accounts.Any())
				{
					_repository.UpdateRange(_accounts);
					await _repository.SaveChanges();
				}
			}
			return true;
		}

		public void LoopThroughAccounts(List<Account> accounts, List<AccountTreeVm> tree, int nextLevel)
		{
			foreach (var item in tree)
			{
				var accountDb = accounts.FirstOrDefault(x => x.AccountId == item.AccountId);
				if (accountDb != null)
				{
					accountDb.AccountLevel = (byte)nextLevel;
					_accounts.Add(accountDb);
				}
			}

			foreach (var item in tree)
			{
				if (item.Children != null)
				{
					if (item.Children.Any())
					{
						nextLevel++;
						LoopThroughAccounts(accounts, item.Children, nextLevel);
					}
				}
			}
		}
		public List<AccountTreeVm> GetAccountTreeByAccountId(List<Account> accounts, int accountId)
		{
			var data = BuildTree(accounts, accountId);
			return data;
		}

		public async Task<ResponseDto> DeleteAccount(Account account)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			if (account.AccountTypeId == AccountTypeData.FixedAssets)
			{
				await DeleteDepreciationAccount(account.AccountId);
				await DeleteAccumulatedDepreciationAccount(account.AccountId);
			}
			_repository.Delete(account);
			await _repository.SaveChanges();
			return new ResponseDto() { Id = account.AccountId, Success = true, Message = _localizer["DeleteAccountSuccessMessage", ((language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn) ?? "")] };
		}

		private async Task DeleteDepreciationAccount(int accountId)
		{
			var depeciationAccount = await GetMirrorDepreciationAccount(accountId);
			var accountDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.AccountId == depeciationAccount.AccountId);

			if (accountDb != null)
			{
				_repository.Delete(accountDb);
				await _repository.SaveChanges();
			}
		}

		private async Task DeleteAccumulatedDepreciationAccount(int accountId)
		{
			var accumulatedDepeciationAccount = await GetMirrorAccumulatedDepreciationAccount(accountId);
			var accountDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.AccountId == accumulatedDepeciationAccount.AccountId);

			if (accountDb != null)
			{
				_repository.Delete(accountDb);
				await _repository.SaveChanges();
			}
		}

		public async Task<List<AccountAutoCompleteDto>> RetrievingMainAccounts(int mainAccountId, int companyId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var allAccounts = await _repository.GetAll().Where(x => x.CompanyId == companyId).ToListAsync();
			var tree = GetTreeMain(allAccounts, mainAccountId);
			foreach (var account in tree)
			{
				var accountDb = allAccounts.FirstOrDefault(x => x.AccountId == account.AccountId);
				if (accountDb != null)
				{
					_treeName.Add(new AccountAutoCompleteDto()
					{
						AccountId = accountDb.AccountId,
						AccountCode = accountDb.AccountCode,
						AccountName = language == LanguageCode.Arabic ? accountDb.AccountNameAr : accountDb.AccountNameEn,
						AccountLevel = accountDb.AccountLevel
					});
				}

				if (account!.List!.Any())
				{
					DeepIntoMainAccounts(account.List);
				}
			}
			return _treeName;
		}
		public async Task<List<int>> RetrievingSubAccounts(int mainAccountId, int companyId)
		{
			var allAccounts = await _repository.GetAll().Where(x => x.CompanyId == companyId).ToListAsync();
			var tree = GetTreeNodes(allAccounts, mainAccountId);

			_deletedAccounts.Add(mainAccountId);

			foreach (var account in tree)
			{
				var accountDb = allAccounts.FirstOrDefault(x => x.AccountId == account.AccountId);
				if (accountDb != null)
				{
					_deletedAccounts.Add(accountDb.AccountId);
				}

				if (account!.List!.Any())
				{
					DeepIntoSubAccounts(account.List);
				}
			}
			return _deletedAccounts;
		}

		public void DeepIntoMainAccounts(List<AccountSimpleTreeDto> list)
		{

			foreach (var account in list)
			{
				var accountDb = list.FirstOrDefault(x => x.MainAccountId == account.MainAccountId);
				if (accountDb != null)
				{
					_treeName.Add(new AccountAutoCompleteDto()
					{
						AccountId = accountDb.AccountId,
						AccountCode = accountDb.AccountCode,
						AccountName = accountDb.AccountName,
						AccountLevel = accountDb.AccountLevel
					});
				}

				if (account!.List!.Any())
				{
					DeepIntoMainAccounts(account.List);
				}
			}
		}

		public void DeepIntoSubAccounts(List<AccountSimpleTreeDto> list)
		{
			foreach (var account in list)
			{
				var accountDb = list.FirstOrDefault(x => x.AccountId == account.AccountId);
				if (accountDb != null)
				{
					_deletedAccounts.Add(accountDb.AccountId);
				}

				if (account!.List!.Any())
				{
					DeepIntoSubAccounts(account.List);
				}
			}
		}

		public List<AccountSimpleTreeDto> GetTreeMain(List<Account> list, int? parent)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return list.Where(x => x.AccountId == parent).Select(x => new AccountSimpleTreeDto
			{
				AccountId = x.AccountId,
				AccountCode = x.AccountCode,
				AccountName = language == LanguageCode.Arabic ? x.AccountNameAr : x.AccountNameEn,
				AccountLevel = x.AccountLevel,
				MainAccountId = x.MainAccountId,
				List = GetTreeMain(list, x.MainAccountId)
			}).ToList();
		}
		public List<AccountSimpleTreeDto> GetTreeNodes(List<Account> list, int parent)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return list.Where(x => x.MainAccountId == parent).Select(x => new AccountSimpleTreeDto
			{
				AccountId = x.AccountId,
				AccountCode = x.AccountCode,
				AccountName = language == LanguageCode.Arabic ? x.AccountNameAr : x.AccountNameEn,
				List = GetTreeNodes(list, x.AccountId)
			}).ToList();
		}

		public async Task<List<AccountAutoCompleteDto>> GetTreeList(int accountId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var accountDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.AccountId == accountId);
			if (accountDb != null)
			{
				_treeName.Add(new AccountAutoCompleteDto()
				{
					AccountId = accountDb.AccountId,
					AccountCode = accountDb.AccountCode,
					AccountName = accountDb.IsLastLevel ? (language == LanguageCode.Arabic ? $"{accountDb.AccountNameAr} ({accountDb.AccountCode} )" : $"{accountDb.AccountNameEn} ({accountDb.AccountCode} )") : (language == LanguageCode.Arabic ? accountDb.AccountNameAr : accountDb.AccountNameEn),
					AccountLevel = accountDb.AccountLevel
				});

				if (accountDb.MainAccountId != null)
				{
					await RetrievingMainAccounts(accountDb.MainAccountId.GetValueOrDefault(), accountDb.CompanyId);
				}
			}
			return _treeName.OrderBy(x => x.AccountLevel).ThenBy(x => x.AccountCode).ToList();
		}

		public async Task<string?> GetAccountTreeName(int accountId)
		{
			var accountTree = "";
			var treeList = await GetTreeList(accountId);
			foreach (var account in treeList)
			{
				accountTree += account.AccountName + " ›› ";
			}
			return accountTree.Substring(0, accountTree.Length - 4);
		}

		private List<AccountTreeVm> BuildTree(List<Account> accounts, int? accountId)
		{
			if (accounts.Any())
			{
				var accountsInOrder = accounts.Where(x => x.AccountId == accountId)
					.Select(x => new AccountTreeVm
					{
						AccountId = x.AccountId,
						AccountNameAr = x.AccountNameEn,
						AccountNameEn = x.AccountNameEn,
						MainAccountId = x.MainAccountId,
						AccountCode = x.AccountCode,
						IsMainAccount = x.IsMainAccount,
						IsLastLevel = x.IsLastLevel,
						AccountLevel = x.AccountLevel,
						Children = GetChildren(accounts, x.AccountId)
					}).ToList();
				return accountsInOrder;
			}
			return new List<AccountTreeVm>();

		}

		private List<AccountTreeVm> GetChildren(List<Account> accounts, int? parentId)
		{
			return accounts.Where(x => x.MainAccountId == parentId)
				.Select(x => new AccountTreeVm
				{
					AccountId = x.AccountId,
					AccountNameAr = x.AccountNameEn,
					AccountNameEn = x.AccountNameEn,
					MainAccountId = x.MainAccountId,
					AccountCode = x.AccountCode,
					IsMainAccount = x.IsMainAccount,
					IsLastLevel = x.IsLastLevel,
					AccountLevel = x.AccountLevel,
					Children = GetChildren(accounts, x.AccountId)
				}).ToList();
		}
	}
}
