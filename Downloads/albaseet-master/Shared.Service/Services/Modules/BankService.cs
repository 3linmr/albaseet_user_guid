using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class BankService : BaseService<Bank>,IBankService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<BankService> _localizer;
        private readonly ICompanyService _companyService;
        private readonly IStoreService _storeService;
        private readonly IBranchService _branchService;

        public BankService(IRepository<Bank> repository,IHttpContextAccessor httpContextAccessor,IStringLocalizer<BankService> localizer,ICompanyService companyService, IStoreService storeService, IBranchService branchService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
            _companyService = companyService;
			_storeService = storeService;
			_branchService = branchService;
        }

		public IQueryable<BankDto> GetAllBanks()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				from bank in _repository.GetAll()
                from company in _companyService.GetAll().Where(x=>x.CompanyId == bank.CompanyId)
                select new BankDto()
				{
					BankId = bank.BankId,
					BankCode = bank.BankCode,
					BankNameAr = bank.BankNameAr,
					BankNameEn = bank.BankNameEn,
					Email = bank.Email,
					TaxCode = bank.TaxCode,
					AccountId = bank.AccountId,
					IsActive = bank.IsActive,
					IsActiveName = (bool)bank.IsActive ? _localizer["Active"].Value : _localizer["InActive"].Value,
					ResponsiblePhone = bank.ResponsiblePhone,
					ResponsibleEmail = bank.ResponsibleEmail,
					AccountNumber = bank.AccountNumber,
					Address = bank.Address,
					Fax = bank.Fax,
					IBAN = bank.IBAN,
					InActiveReasons = bank.InActiveReasons,
					Phone = bank.Phone,
					ResponsibleFax = bank.ResponsibleFax,
					ResponsibleName = bank.ResponsibleName,
					VisaFees = bank.VisaFees,
					Website = bank.Website,
					ModifiedAt = bank.ModifiedAt,
					UserNameModified = bank.UserNameModified,
					CreatedAt = bank.CreatedAt,
					UserNameCreated = bank.UserNameCreated,
					ArchiveHeaderId = bank.ArchiveHeaderId,
					CompanyId = bank.CompanyId,
					CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn
                };
			return data;
		}

        public IQueryable<BankDto> GetUserBanks()
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return GetAllBanks().Where(x => x.CompanyId == companyId);
		}


        public IQueryable<BankDropDownDto> GetBanksDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = GetUserBanks().Where(x=>x.IsActive).Select(x => new BankDropDownDto()
			{
				BankId = x.BankId,
				BankCode = x.BankCode,
				BankName = language == LanguageCode.Arabic ? x.BankNameAr : x.BankNameEn
			}).OrderBy(x => x.BankName);
			return data;
		}

		public Task<BankDto?> GetBankById(int id)
		{
			return GetAllBanks().FirstOrDefaultAsync(x => x.BankId == id);
		}
		public async Task<List<BankAutoCompleteDto>> GetBanksAutoComplete(string term)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			if (language == LanguageCode.Arabic)
			{
				return await _repository.GetAll().Where(x => x.CompanyId == companyId && (x.BankNameAr!.ToLower().Contains(term.Trim().ToLower()) || x.BankCode.ToString().Contains(term.Trim().ToLower())) && x.IsActive).Select(s => new BankAutoCompleteDto
				{
					BankId = s.BankId,
					BankCode = s.BankCode,
					BankName = $"{s.BankCode} - {s.BankNameAr}",
					BankNameAr = s.BankNameAr,
					BankNameEn = s.BankNameEn
				}).Take(10).ToListAsync();
			}
			else
			{
				return await _repository.GetAll().Where(x => x.CompanyId == companyId && (x.BankNameEn!.ToLower().Contains(term.Trim().ToLower()) || x.BankCode.ToString().Contains(term.Trim().ToLower())) && x.IsActive).Select(s => new BankAutoCompleteDto
				{
					BankId = s.BankId,
					BankCode = s.BankCode,
					BankName = $"{s.BankCode} - {s.BankNameEn}",
					BankNameAr = s.BankNameAr,
					BankNameEn = s.BankNameEn
				}).Take(10).ToListAsync();
			}
		}

		public async Task<List<BankAutoCompleteDto>> GetBanksAutoCompleteByStoreIds(string term, List<int> storeIds)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyIds = _storeService.GetAllStores().Where(x => storeIds.Contains(x.StoreId)).Select(x => x.CompanyId);

			if (language == LanguageCode.Arabic)
			{
				return await (from bank in _repository.GetAll()
							  where companyIds.Contains(bank.CompanyId) && bank.IsActive && (bank.BankNameAr!.ToLower().Contains(term.Trim().ToLower()) || bank.BankCode.ToString().Contains(term.Trim().ToLower()))
							  select new BankAutoCompleteDto
							  {
								  BankId = bank.BankId,
								  BankCode = bank.BankCode,
								  BankName = $"{bank.BankCode} - {bank.BankNameAr}",
								  BankNameAr = bank.BankNameAr,
								  BankNameEn = bank.BankNameEn
							  }).Take(10).ToListAsync();
			}
			else
			{
				return await (from bank in _repository.GetAll()
							  where companyIds.Contains(bank.CompanyId) && bank.IsActive && (bank.BankNameEn!.ToLower().Contains(term.Trim().ToLower()) || bank.BankCode.ToString().Contains(term.Trim().ToLower()))
							  select new BankAutoCompleteDto
							  {
								  BankId = bank.BankId,
								  BankCode = bank.BankCode,
								  BankName = $"{bank.BankCode} - {bank.BankNameEn}",
								  BankNameAr = bank.BankNameAr,
								  BankNameEn = bank.BankNameEn
							  }).Take(10).ToListAsync();
			}
		}

		public async Task<ResponseDto> LinkWithBankAccount(AccountDto account, bool update)
		{
			if (update)
			{
				var bankDb = await GetBankById(account.BankId.GetValueOrDefault());
				if (bankDb != null)
				{
					bankDb.BankNameAr = account.AccountNameAr;
					bankDb.BankNameEn = account.AccountNameEn;
					return await SaveBank(bankDb);
				}
			}
			else
			{
				if (account.CreateNewBank)
				{
					var bank = new BankDto() { AccountId = account.AccountId, BankNameAr = account.AccountNameAr, BankNameEn = account.AccountNameEn, IsActive = true };
					return await SaveBank(bank);
				}
				else
				{
					if (account.BankId != null)
					{
						var bankDb = await GetBankById(account.BankId.GetValueOrDefault());
						if (bankDb != null)
						{
							var isBankNotLinkedBefore = await IsBankNotLinkedBefore(account.AccountId);
							if (isBankNotLinkedBefore.Success)
							{
								bankDb.AccountId = account.AccountId;
								return await SaveBank(bankDb);
							}
							else
							{
								return isBankNotLinkedBefore;
							}
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["NoBankFound"] };
		}

		public async Task<bool> UnLinkWithBankAccount(int accountId)
		{
			var bank = await _repository.GetAll().FirstOrDefaultAsync(x => x.AccountId == accountId);
			if (bank != null)
			{
				bank.AccountId = null;
				_repository.Update(bank);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}

		public async Task<ResponseDto> IsBankNotLinkedBefore(int? accountId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var bank = await _repository.GetAll().FirstOrDefaultAsync(x => x.AccountId == accountId);
			if (bank == null)
			{
				return new ResponseDto() { Success = true };
			}
			else
			{
				return new ResponseDto() { Success = false, Id = bank.BankId, Message = _localizer["BankAlreadyLinked", ((language == LanguageCode.Arabic ? bank.BankNameAr : bank.BankNameEn) ?? ""), bank.BankId] };
			}
		}

		public async Task<ResponseDto> SaveBank(BankDto bank)
		{
			var bankExist = await IsBankExist(bank.BankId, bank.BankNameAr, bank.BankNameEn);
			if (bankExist.Success)
			{
				return new ResponseDto() { Id = bankExist.Id, Success = false, Message = _localizer["BankAlreadyExist"] };
			}
			else
			{
				if (bank.BankId == 0)
				{
					return await CreateBank(bank);
				}
				else
				{
					return await UpdateBank(bank);
				}
			}
		}

		public async Task<ResponseDto> IsBankExist(int id, string? nameAr, string? nameEn)
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var bank = await _repository.GetAll().FirstOrDefaultAsync(x => (x.BankNameAr == nameAr || x.BankNameEn == nameEn || x.BankNameAr == nameEn || x.BankNameEn == nameAr) && x.BankId != id && x.CompanyId == companyId);
			if (bank != null)
			{
				return new ResponseDto() { Id = bank.BankId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.BankId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.BankCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateBank(BankDto bank)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();

			var newBank = new Bank()
			{
				BankId = await GetNextId(),
				BankCode = await GetNextCode(companyId),
				BankNameAr = bank?.BankNameAr?.Trim(),
				BankNameEn = bank?.BankNameEn?.Trim(),
                CompanyId = companyId,
                Email = bank?.Email,
				TaxCode = bank?.TaxCode,
				IsActive = bank!.IsActive,
				ResponsiblePhone = bank?.ResponsiblePhone,
				ResponsibleEmail = bank?.ResponsibleEmail,
				AccountNumber = bank?.AccountNumber,
				Address = bank?.Address,
				Fax = bank?.Fax,
				IBAN = bank?.IBAN,
				AccountId = bank?.AccountId,
				InActiveReasons = bank?.InActiveReasons,
				Phone = bank?.Phone,
				ResponsibleFax = bank?.ResponsibleFax,
				ResponsibleName = bank?.ResponsibleName,
				VisaFees = bank!.VisaFees,
				Website = bank?.Website,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var bankValidator = await new BankValidator(_localizer).ValidateAsync(newBank);
			var validationResult = bankValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newBank);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newBank.BankId, Success = true, Message = _localizer["NewBankSuccessMessage", ((language == LanguageCode.Arabic ? newBank.BankNameAr : newBank.BankNameEn) ?? ""), newBank.BankCode] };
			}
			else
			{
				return new ResponseDto() { Id = newBank.BankId, Success = false, Message = bankValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateBank(BankDto bank)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();

			var bankDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.BankId == bank.BankId);
			if (bankDb != null)
			{
				bankDb.BankNameAr = bank.BankNameAr?.Trim() ?? bankDb.BankNameAr;
				bankDb.BankNameEn = bank.BankNameEn?.Trim() ?? bankDb.BankNameEn;
                bankDb.CompanyId = companyId;
				bankDb.Email = bank?.Email;
				bankDb.TaxCode = bank?.TaxCode;
				bankDb.IsActive = bank!.IsActive;
				bankDb.ResponsiblePhone = bank?.ResponsiblePhone;
				bankDb.ResponsibleEmail = bank?.ResponsibleEmail;
				bankDb.AccountNumber = bank?.AccountNumber;
				bankDb.Address = bank?.Address;
				bankDb.AccountId = bank?.AccountId ?? bankDb.AccountId;
				bankDb.Fax = bank?.Fax;
				bankDb.IBAN = bank?.IBAN;
				bankDb.InActiveReasons = bank?.InActiveReasons;
				bankDb.Phone = bank?.Phone;
				bankDb.ResponsibleFax = bank?.ResponsibleFax;
				bankDb.ResponsibleName = bank?.ResponsibleName;
				bankDb.VisaFees = bank!.VisaFees;
				bankDb.Website = bank?.Website;
				bankDb.ModifiedAt = DateHelper.GetDateTimeNow();
				bankDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				bankDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var bankValidator = await new BankValidator(_localizer).ValidateAsync(bankDb);
				var validationResult = bankValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(bankDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = bankDb.BankId, Success = true, Message = _localizer["UpdateBankSuccessMessage", ((language == LanguageCode.Arabic ? bankDb.BankNameAr : bankDb.BankNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = bankValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoBankFound"] };
		}

		public async Task<ResponseDto> DeleteBank(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var bankDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.BankId == id);
			if (bankDb != null)
			{
				var isAccountExist = bankDb.AccountId != null;
				if (isAccountExist)
				{
					return new ResponseDto() { Id = id, Success = false, Message = _localizer["BankIsLinkedWithAccount"] };
				}
				else
				{
					_repository.Delete(bankDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteBankSuccessMessage", ((language == LanguageCode.Arabic ? bankDb.BankNameAr : bankDb.BankNameEn) ?? "")] };
				}
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoBankFound"] };
		}
	}
}
