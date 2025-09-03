using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Taxes;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Taxes;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Taxes
{
	public class TaxService : BaseService<Tax>, ITaxService
	{
		private readonly ITaxTypeService _taxTypeService;
		private readonly ICompanyService _companyService;
		private readonly IAccountService _accountService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<TaxService> _localizer;
		private readonly IBranchService _branchService;
		private readonly IStoreService _storeService;

		public TaxService(IRepository<Tax> repository,ITaxTypeService taxTypeService,ICompanyService companyService,IAccountService accountService,IHttpContextAccessor httpContextAccessor,IStringLocalizer<TaxService> localizer,IBranchService branchService,IStoreService storeService) : base(repository)
		{
			_taxTypeService = taxTypeService;
			_companyService = companyService;
			_accountService = accountService;
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_branchService = branchService;
			_storeService = storeService;
		}

		public IQueryable<TaxDto> GetAllTaxes()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from tax in _repository.GetAll()
				from taxType in _taxTypeService.GetAll().Where(x => x.TaxTypeId == tax.TaxTypeId)
				from company in _companyService.GetAll().Where(x=>x.CompanyId == tax.CompanyId)
				from drAccount in _accountService.GetAll().Where(x=>x.AccountId == tax.DrAccount).DefaultIfEmpty()
				from crAccount in _accountService.GetAll().Where(x=>x.AccountId == tax.CrAccount).DefaultIfEmpty()
				select new TaxDto()
				{
					TaxId = tax.TaxId,
					TaxCode = tax.TaxCode,
					TaxTypeId = tax.TaxTypeId,
					CompanyId = tax.CompanyId,
					CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
					TaxNameAr = tax.TaxNameAr,
					TaxNameEn = tax.TaxNameEn,
					TaxTypeName = language == LanguageCode.Arabic ? taxType.TaxTypeNameAr : taxType.TaxTypeNameEn,
					DrAccount = tax.DrAccount,
					DrAccountCode = drAccount != null ? drAccount.AccountCode : null,
					DrAccountName = drAccount != null ? (language == LanguageCode.Arabic ? drAccount.AccountNameAr : drAccount.AccountNameEn) : null,
					CrAccount = tax.CrAccount,
					CrAccountCode = crAccount != null ? crAccount.AccountCode : null,
					CrAccountName = crAccount != null ? (language == LanguageCode.Arabic ? crAccount.AccountNameAr : crAccount.AccountNameEn) : null,
					TaxAfterVatInclusive = tax.TaxAfterVatInclusive,
					IsVatTax = tax.IsVatTax
				};
			return data;
		}

		public IQueryable<TaxDto> GetCompanyTaxes(int companyId)
		{
			return GetAllTaxes().Where(x => x.CompanyId == companyId);
		}


		public IQueryable<TaxDto> GetAllStoreTaxes()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var taxes = GetAllTaxes();

			var data =
				from tax in taxes
				from company in _companyService.GetAll().Where(x => x.CompanyId == tax.CompanyId)
				from branch in _branchService.GetAll().Where(x => x.CompanyId == company.CompanyId)
				from store in _storeService.GetAll().Where(x => x.BranchId == branch.BranchId)
				from drAccount in _accountService.GetAll().Where(x => x.AccountId == tax.DrAccount).DefaultIfEmpty()
				from crAccount in _accountService.GetAll().Where(x => x.AccountId == tax.CrAccount).DefaultIfEmpty()
				select new TaxDto()
				{
					TaxId = tax.TaxId,
					TaxCode = tax.TaxCode,
					TaxTypeId = tax.TaxTypeId,
					CompanyId = tax.CompanyId,
					StoreId = store.StoreId,
					CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					TaxNameAr = tax.TaxNameAr,
					TaxNameEn = tax.TaxNameEn,
					TaxName = language == LanguageCode.Arabic ? tax.TaxNameAr : tax.TaxNameEn,
					TaxTypeName = tax.TaxTypeName,
					DrAccount = tax.DrAccount,
					DrAccountCode = drAccount != null ? drAccount.AccountCode : null,
					DrAccountName = drAccount != null ? (language == LanguageCode.Arabic ? drAccount.AccountNameAr : drAccount.AccountNameEn) : null,
					CrAccount = tax.CrAccount,
					CrAccountCode = crAccount != null ? crAccount.AccountCode : null,
					CrAccountName = crAccount != null ? (language == LanguageCode.Arabic ? crAccount.AccountNameAr : crAccount.AccountNameEn) : null,
					TaxAfterVatInclusive = tax.TaxAfterVatInclusive,
					IsVatTax = tax.IsVatTax
				};
			return data;
		}

		public IQueryable<TaxDto> GetStoreTaxes(int storeId)
		{
			return GetAllStoreTaxes().Where(x => x.StoreId == storeId);
		}

		public IQueryable<TaxDto> GetAllStoreVatTaxes()
		{
			return GetAllStoreTaxes().Where(x => x.IsVatTax);
		}

        public IQueryable<TaxDto> GetUserTaxes()
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return GetAllTaxes().Where(x => x.CompanyId == companyId);
		}


        public IQueryable<TaxDropDownDto> GetAllTaxesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetAllTaxes().Select(x => new TaxDropDownDto()
			{
				TaxId = x.TaxId,
				TaxName = language == LanguageCode.Arabic ? x.TaxNameAr : x.TaxNameEn,
				IsVatTax = x.IsVatTax
			}).OrderBy(x => x.TaxName);
		}

		public IQueryable<TaxDropDownDto> GetCompanyTaxesDropDown(int companyId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetCompanyTaxes(companyId).Select(x => new TaxDropDownDto()
			{
				TaxId = x.TaxId,
				TaxName = language == LanguageCode.Arabic ? x.TaxNameAr : x.TaxNameEn,
				IsVatTax = x.IsVatTax
			}).OrderBy(x => x.TaxName);
		}

		public IQueryable<TaxDropDownDto> GetCompanyOtherTaxesDropDown(int companyId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetCompanyTaxes(companyId).Where(x=>!x.IsVatTax).Select(x => new TaxDropDownDto()
			{
				TaxId = x.TaxId,
				TaxName = language == LanguageCode.Arabic ? x.TaxNameAr : x.TaxNameEn,
			}).OrderBy(x => x.TaxName);
		}

		public IQueryable<TaxDropDownDto> GetStoreTaxesDropDown(int storeId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetStoreTaxes(storeId).Select(x => new TaxDropDownDto()
			{
				TaxId = x.TaxId,
				TaxName = language == LanguageCode.Arabic ? x.TaxNameAr : x.TaxNameEn,
			}).OrderBy(x => x.TaxName);
		}

		public IQueryable<TaxDropDownDto> GeVatTaxDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return GetAllTaxes().Where(x=>x.IsVatTax && x.CompanyId == companyId).Select(x => new TaxDropDownDto()
			{
				TaxId = x.TaxId,
				TaxName = language == LanguageCode.Arabic ? x.TaxNameAr : x.TaxNameEn,
			}).OrderBy(x => x.TaxName);
		}

		public IQueryable<TaxDropDownDto> GetStoreOtherTaxesDropDown(int storeId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetStoreTaxes(storeId).Where(x => !x.IsVatTax).Select(x => new TaxDropDownDto()
			{
				TaxId = x.TaxId,
				TaxName = language == LanguageCode.Arabic ? x.TaxNameAr : x.TaxNameEn,
			}).OrderBy(x => x.TaxName);
		}

		public IQueryable<TaxDto> GetAllTaxesByTypeId(int taxTypeId)
		{
			return GetAllTaxes().Where(x => x.TaxTypeId == taxTypeId);
		}

		public async Task<TaxDto?> GetTaxById(int id)
		{
			return await GetAllTaxes().FirstOrDefaultAsync(x => x.TaxId == id);
		}

		public async Task<ResponseDto> IsVatTaxExist()
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var exist = await GetAllTaxes().AnyAsync(x => x.IsVatTax && x.CompanyId == companyId);
			return new ResponseDto() { Success = exist, Message = "VAT exist" };
		}

		public async Task<ResponseDto> SaveTax(TaxDto tax)
		{
			var isTaxExist = await IsTaxExist(tax.CompanyId, tax.TaxId, tax.TaxNameAr,tax.TaxNameEn);
			if (isTaxExist.Success)
			{
				return new ResponseDto() { Id = isTaxExist.Id, Success = false, Message = _localizer["TaxAlreadyExist"] };
			}

			if (tax.IsVatTax)
			{
				var isVatTaxExist = await IsVatTaxExist(tax.CompanyId, tax.TaxId);
				if (isVatTaxExist)
				{
					return new ResponseDto() { Id = 0, Success = false, Message = _localizer["VatTaxAlreadyExist"] };
				}
			}

			if (tax.TaxId == 0)
			{
				return await CreateTax(tax);
			}
			else
			{
				return await UpdateTax(tax);
			}
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.TaxId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.TaxCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateTax(TaxDto tax)
		{
			var newTax = new Tax()
			{
				TaxId = await GetNextId(),
				TaxCode = await GetNextCode(tax.CompanyId),
				TaxNameAr = tax.TaxNameAr?.Trim(),
				TaxNameEn = tax.TaxNameEn?.Trim(),
				CompanyId = tax.CompanyId,
				TaxTypeId = tax.TaxTypeId,
				DrAccount = tax.DrAccount,
				CrAccount = tax.CrAccount,
				IsVatTax = tax.IsVatTax,
				TaxAfterVatInclusive = tax.TaxAfterVatInclusive,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false
			};

			var taxValidator = await new TaxValidator(_localizer).ValidateAsync(newTax);
			var validationResult = taxValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newTax);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newTax.TaxId, Success = true, Message = _localizer["NewTaxSuccessMessage"] };
			}
			else
			{
				return new ResponseDto() { Id = newTax.TaxId, Success = false, Message = taxValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateTax(TaxDto tax)
		{
			var taxDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.TaxId == tax.TaxId);
			if (taxDb != null)
			{
				taxDb.TaxNameAr = tax.TaxNameAr?.Trim();
				taxDb.TaxNameEn = tax.TaxNameEn?.Trim();
				taxDb.CompanyId = tax.CompanyId;
				taxDb.TaxTypeId = tax.TaxTypeId;
				taxDb.DrAccount = tax.DrAccount;
				taxDb.CrAccount = tax.CrAccount;
				taxDb.IsVatTax = tax.IsVatTax;
				taxDb.TaxAfterVatInclusive = tax.TaxAfterVatInclusive;
				taxDb.ModifiedAt = DateHelper.GetDateTimeNow();
				taxDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				taxDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var taxValidator = await new TaxValidator(_localizer).ValidateAsync(taxDb);
				var validationResult = taxValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(taxDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = taxDb.TaxId, Success = true, Message = _localizer["UpdateTaxSuccessMessage"] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = taxValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoTaxFound"] };
		}

		public async Task<bool> IsVatTaxExist(int companyId,int taxId)
		{
			return await _repository.GetAll().AnyAsync(x => x.TaxId != taxId && x.CompanyId == companyId && x.IsVatTax );
		}

		public async Task<ResponseDto> IsTaxExist(int companyId, int id, string? nameAr, string? nameEn)
		{
			var tax = await _repository.GetAll().FirstOrDefaultAsync(x => (x.TaxNameAr == nameAr || x.TaxNameEn == nameEn || x.TaxNameAr == nameEn || x.TaxNameEn == nameAr) && x.TaxId != id && x.CompanyId == companyId);
			if (tax != null)
			{
				return new ResponseDto() { Id = tax.TaxId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}

		public async Task<ResponseDto> DeleteTax(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var taxDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.TaxId == id);
			if (taxDb != null)
			{
				_repository.Delete(taxDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteTaxSuccessMessage"] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoTaxFound"] };
		}
	}
}
