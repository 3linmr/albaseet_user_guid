using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.Helper.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
    public class SupplierService : BaseService<Supplier>, ISupplierService
	{
		private readonly IShipmentTypeService _shipmentTypeService;
		private readonly ICountryService _countryService;
		private readonly IStateService _stateService;
		private readonly ICityService _cityService;
		private readonly IDistrictService _districtService;
		private readonly ICompanyService _companyService;
        private readonly IStoreService _storeService;
        private readonly IBranchService _branchService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _accountService;
        private readonly IStringLocalizer<SupplierService> _localizer;

		public SupplierService(IRepository<Supplier> repository,IShipmentTypeService shipmentTypeService, ICountryService countryService, IStateService stateService, ICityService cityService, IDistrictService districtService,ICompanyService companyService,IStoreService storeService,IBranchService branchService,IHttpContextAccessor httpContextAccessor,IAccountService accountService,IStringLocalizer<SupplierService>localizer) : base(repository)
		{
			_shipmentTypeService = shipmentTypeService;
			_countryService = countryService;
			_stateService = stateService;
			_cityService = cityService;
			_districtService = districtService;
			_companyService = companyService;
            _storeService = storeService;
            _branchService = branchService;
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
            _localizer = localizer;
		}

		public IQueryable<SupplierDto> GetAllSuppliers()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from supplier in _repository.GetAll()
				from company in _companyService.GetAll().Where(x=>x.CompanyId == supplier.CompanyId)
				from country in _countryService.GetAll().Where(x => x.CountryId == supplier.CountryId).DefaultIfEmpty()
				from state in _stateService.GetAll().Where(x => x.StateId == supplier.StateId).DefaultIfEmpty()
				from city in _cityService.GetAll().Where(x => x.CityId == supplier.CityId).DefaultIfEmpty()
				from district in _districtService.GetAll().Where(x => x.DistrictId == supplier.DistrictId).DefaultIfEmpty()
				from shipmentType in _shipmentTypeService.GetAll().Where(x=>x.ShipmentTypeId == supplier.ShipmentTypeId).DefaultIfEmpty()
				from account in _accountService.GetAll().Where(x => x.AccountId == supplier.AccountId).DefaultIfEmpty()
				select new SupplierDto()
				{
					SupplierId = supplier.SupplierId,
					SupplierCode = supplier.SupplierCode,
					SupplierNameAr = supplier.SupplierNameAr,
					SupplierNameEn = supplier.SupplierNameEn,
					AccountId = supplier.AccountId,
					AccountCode = account != null ? account.AccountCode : null,
					AccountName = account != null ? (language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn) + " (" + account.AccountCode +")" : null,
					CompanyId = supplier.CompanyId,
					CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
					Address1 = supplier.Address1,
					Address2 = supplier.Address2,
					Address3 = supplier.Address3,
					Address4 = supplier.Address4,
					AdditionalNumber = supplier.AdditionalNumber,
					BuildingNumber = supplier.BuildingNumber,
					CityId = supplier.CityId,
					CommercialRegister = supplier.CommercialRegister,
					CountryId = supplier.CountryId,
					CountryCode = supplier.CountryCode,
					DistrictId = supplier.DistrictId,
					StateId = supplier.StateId,
					Street1 = supplier.Street1,
					Street2 = supplier.Street2,
					PostalCode = supplier.PostalCode,
					CountryName = country != null ? (language == LanguageCode.Arabic ? country.CountryNameAr : country.CountryNameEn) : "",
					StateName = state != null ? (language == LanguageCode.Arabic ? state.StateNameAr : state.StateNameEn) : "",
					CityName = city != null ? (language == LanguageCode.Arabic ? city.CityNameAr : city.CityNameEn) : "",
					DistrictName = district != null ? (language == LanguageCode.Arabic ? district.DistrictNameAr : district.DistrictNameEn) : "",
					ShipmentTypeId = supplier.ShipmentTypeId,
					ContractDate = supplier.ContractDate,
					IsCredit = supplier.IsCredit,
					IsActive = supplier.IsActive,
					InActiveReasons = supplier.InActiveReasons,
					IsActiveName = (bool)supplier.IsActive ? _localizer["Active"].Value : _localizer["InActive"].Value,
					TaxCode = supplier.TaxCode,
					CreditLimitDays = supplier.CreditLimitDays,
					CreditLimitValues = supplier.CreditLimitValues,
					DebitLimitDays = supplier.DebitLimitDays,
					ShipmentTypeName = shipmentType != null ? (language == LanguageCode.Arabic ? shipmentType.ShipmentTypeNameAr : shipmentType.ShipmentTypeNameEn) : "",
					ModifiedAt = supplier.ModifiedAt,
					CreatedAt = supplier.CreatedAt,
					UserNameCreated = supplier.UserNameCreated,
					UserNameModified = supplier.UserNameModified,
					ArchiveHeaderId = supplier.ArchiveHeaderId,
					FirstResponsibleName = supplier.FirstResponsibleName,
					FirstResponsiblePhone = supplier.FirstResponsiblePhone,
					FirstResponsibleEmail = supplier.FirstResponsibleEmail,
					SecondResponsibleName = supplier.SecondResponsibleName,
					SecondResponsiblePhone = supplier.SecondResponsiblePhone,
					SecondResponsibleEmail = supplier.SecondResponsibleEmail
				};
			return data;
		}

        public IQueryable<SupplierDto> GetUserSuppliers()
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return GetAllSuppliers().Where(x => x.CompanyId == companyId);
		}

        public IQueryable<SupplierDropDownDto> GetAllSuppliersDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return GetAllSuppliers().Where(x=>x.IsActive && x.CompanyId == companyId).Select(x => new SupplierDropDownDto()
			{
				SupplierId = x.SupplierId,
				SupplierCode = x.SupplierCode,
				SupplierName = language == LanguageCode.Arabic ? x.SupplierNameAr : x.SupplierNameEn,
				MobileNumber = $"{x.FirstResponsiblePhone} - {x.SecondResponsiblePhone}",
				AccountId = x.AccountId,
				AccountCode = x.AccountCode,
				AccountName = x.AccountName,
			}).OrderBy(x => x.SupplierName);
		}

        public IQueryable<SupplierDropDownDto> GetSuppliersDropDownByCompanyId(int companyId)
        {
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
            return GetAllSuppliers().Where(x=>x.CompanyId == companyId && x.IsActive).Select(x => new SupplierDropDownDto()
            {
                SupplierId = x.SupplierId,
				SupplierCode = x.SupplierCode,
                SupplierName = language == LanguageCode.Arabic ? x.SupplierNameAr : x.SupplierNameEn,
            }).OrderBy(x => x.SupplierName);
        }

        public async Task<IQueryable<SupplierDropDownDto>> GetSuppliersDropDownByStoreId(int storeId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var companyId = await _storeService.GetCompanyIdByStoreId(storeId);
            return GetAllSuppliers().Where(x => x.CompanyId == companyId && x.IsActive).Select(x => new SupplierDropDownDto()
            {
                SupplierId = x.SupplierId,
				SupplierCode = x.SupplierCode,
                SupplierName = language == LanguageCode.Arabic ? x.SupplierNameAr : x.SupplierNameEn,
            }).OrderBy(x => x.SupplierName);
        }

        public Task<SupplierDto?> GetSupplierById(int id)
		{
			return GetAllSuppliers().FirstOrDefaultAsync(x => x.SupplierId == id);
		}

		public async Task<SupplierDto?> GetSupplierByAccountId(int id)
		{
			return await GetAllSuppliers().FirstOrDefaultAsync(x => x.AccountId == id);
		}

		public async Task<List<SupplierAutoCompleteDto>> GetSuppliersAutoComplete(int companyId, string term)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			if (language == LanguageCode.Arabic)
			{
				return await _repository.GetAll().Where(x => x.CompanyId == companyId && (x.SupplierNameAr!.ToLower().Contains(term.Trim().ToLower()) || x.SupplierCode.ToString().Contains(term.Trim().ToLower())) && x.IsActive).Select(s => new SupplierAutoCompleteDto
				{
					SupplierId = s.SupplierId,
					SupplierCode = s.SupplierCode,
					SupplierName = $"{s.SupplierCode} - {s.SupplierNameAr}",
					SupplierNameAr = s.SupplierNameAr,
					SupplierNameEn = s.SupplierNameEn
				}).Take(10).ToListAsync();
			}
			else
			{
				return await _repository.GetAll().Where(x => x.CompanyId == companyId && (x.SupplierNameEn!.ToLower().Contains(term.Trim().ToLower()) || x.SupplierCode.ToString().Contains(term.Trim().ToLower())) && x.IsActive).Select(s => new SupplierAutoCompleteDto
				{
					SupplierId = s.SupplierId,
					SupplierCode = s.SupplierCode,
					SupplierName = $"{s.SupplierCode} - {s.SupplierNameEn}",
					SupplierNameAr = s.SupplierNameAr,
					SupplierNameEn = s.SupplierNameEn
				}).Take(10).ToListAsync();
			}
		}

		public async Task<List<SupplierAutoCompleteDto>> GetSuppliersAutoCompleteByStoreIds(string term, List<int> storeIds)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyIds = _storeService.GetAllStores().Where(x => storeIds.Contains(x.StoreId)).Select(x => x.CompanyId);

			if (language == LanguageCode.Arabic)
			{
				return await (from supplier in _repository.GetAll()
							  where companyIds.Contains(supplier.CompanyId) && supplier.IsActive && (supplier.SupplierNameAr!.ToLower().Contains(term.Trim().ToLower()) || supplier.SupplierCode.ToString().Contains(term.Trim().ToLower()))
							  select new SupplierAutoCompleteDto
							  {
								  SupplierId = supplier.SupplierId,
								  SupplierCode = supplier.SupplierCode,
								  SupplierName = $"{supplier.SupplierCode} - {supplier.SupplierNameAr}",
								  SupplierNameAr = supplier.SupplierNameAr,
								  SupplierNameEn = supplier.SupplierNameEn
							  }).Take(10).ToListAsync();
			}
			else
			{
				return await (from supplier in _repository.GetAll()
							  where companyIds.Contains(supplier.CompanyId) && supplier.IsActive && (supplier.SupplierNameEn!.ToLower().Contains(term.Trim().ToLower()) || supplier.SupplierCode.ToString().Contains(term.Trim().ToLower()))
							  select new SupplierAutoCompleteDto
							  {
								  SupplierId = supplier.SupplierId,
								  SupplierCode = supplier.SupplierCode,
								  SupplierName = $"{supplier.SupplierCode} - {supplier.SupplierNameEn}",
								  SupplierNameAr = supplier.SupplierNameAr,
								  SupplierNameEn = supplier.SupplierNameEn
							  }).Take(10).ToListAsync();
			}
		}

		public async Task<ResponseDto> LinkWithSupplierAccount(AccountDto account, bool update)
		{
			if (update)
			{
				var supplierDb = await GetSupplierById(account.SupplierId.GetValueOrDefault());
				if (supplierDb != null)
				{
					supplierDb.SupplierNameAr = account.AccountNameAr;
					supplierDb.SupplierNameEn = account.AccountNameEn;
					return await SaveSupplier(supplierDb);
				}
			}
			else
			{
				if (account.CreateNewSupplier)
				{
					var supplier = new SupplierDto() { AccountId = account.AccountId, SupplierNameAr = account.AccountNameAr, SupplierNameEn = account.AccountNameEn, ContractDate = DateTime.Today, CompanyId = account.CompanyId, IsActive = true };
					return await SaveSupplier(supplier);
				}
				else
				{
					if (account.SupplierId != null)
					{
						var supplierDb = await GetSupplierById(account.SupplierId.GetValueOrDefault());
						if (supplierDb != null)
						{
							var isSupplierNotLinkedBefore = await IsSupplierNotLinkedBefore(account.AccountId);
							if (isSupplierNotLinkedBefore.Success)
							{
								supplierDb.AccountId = account.AccountId;
								return await SaveSupplier(supplierDb);
							}
							else
							{
								return isSupplierNotLinkedBefore;
							}
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["NoSupplierFound"] };
		}

		public async Task<bool> UnLinkWithSupplierAccount(int accountId)
		{
			var supplier = await _repository.GetAll().FirstOrDefaultAsync(x => x.AccountId == accountId);
			if (supplier != null)
			{
				supplier.AccountId = null;
				_repository.Update(supplier);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}

		public async Task<ResponseDto> IsSupplierNotLinkedBefore(int? accountId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var supplier = await _repository.GetAll().FirstOrDefaultAsync(x => x.AccountId == accountId);
			if (supplier == null)
			{
				return new ResponseDto() { Success = true };
			}
			else
			{
				return new ResponseDto() { Success = false, Id = supplier.SupplierId, Message = _localizer["SupplierAlreadyLinked", ((language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn) ?? ""), supplier.SupplierId] };
			}
		}

		public async Task<ResponseDto> SaveSupplier(SupplierDto supplier)
		{
			var supplierExist = await IsSupplierExist(supplier.SupplierId, supplier.SupplierNameAr, supplier.SupplierNameEn);
			if (supplierExist.Success)
			{
				return new ResponseDto() { Id = supplierExist.Id, Success = false, Message = _localizer["SupplierAlreadyExist"] };
			}
			else
			{
				if (supplier.SupplierId == 0)
				{
					return await CreateSupplier(supplier);
				}
				else
				{
					return await UpdateSupplier(supplier);
				}
			}
		}

		public async Task<ResponseDto> IsSupplierExist(int id, string? nameAr, string? nameEn)
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var supplier = await _repository.GetAll().FirstOrDefaultAsync(x => (x.SupplierNameAr == nameAr || x.SupplierNameEn == nameEn || x.SupplierNameAr == nameEn || x.SupplierNameEn == nameAr) && x.SupplierId != id && x.CompanyId == companyId);
			if (supplier != null)
			{
				return new ResponseDto() { Id = supplier.SupplierId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.SupplierId) + 1; } catch { id = 1; }
			return id;
		}


		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.SupplierCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateSupplier(SupplierDto supplier)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var newSupplier = new Supplier()
			{
				SupplierId = await GetNextId(),
				SupplierCode = await GetNextCode(supplier.CompanyId),
				SupplierNameAr = supplier.SupplierNameAr,
				SupplierNameEn = supplier.SupplierNameEn,
				IsActive = supplier.IsActive,
				AccountId = supplier.AccountId,
				CompanyId = supplier.CompanyId,
				InActiveReasons = supplier.InActiveReasons,
				IsCredit = supplier.IsCredit,
				CountryId = supplier.CountryId,
				StateId = supplier.StateId,
				CityId = supplier.CityId,
				DistrictId = supplier.DistrictId,
				Address1 = supplier.Address1,
				Address2 = supplier.Address2,
				Address3 = supplier.Address3,
				Address4 = supplier.Address4,
				AdditionalNumber = supplier.AdditionalNumber,
				BuildingNumber = supplier.BuildingNumber,
				CommercialRegister = supplier.CommercialRegister,
				CountryCode = supplier.CountryCode,
				Street1 = supplier.Street1,
				Street2 = supplier.Street2,
				ContractDate = supplier.ContractDate,
				CreditLimitDays = supplier.CreditLimitDays,
				DebitLimitDays = supplier.DebitLimitDays,
				CreditLimitValues = supplier.CreditLimitValues,
				ShipmentTypeId = supplier.ShipmentTypeId,
				TaxCode = supplier.TaxCode,
				PostalCode = supplier.PostalCode,
				FirstResponsibleName = supplier.FirstResponsibleName,
				FirstResponsiblePhone = supplier.FirstResponsiblePhone,
				FirstResponsibleEmail = supplier.FirstResponsibleEmail,
				SecondResponsibleName = supplier.SecondResponsibleName,
				SecondResponsiblePhone = supplier.SecondResponsiblePhone,
				SecondResponsibleEmail = supplier.SecondResponsibleEmail,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false
			};

			var supplierValidator = await new SupplierValidator(_localizer).ValidateAsync(newSupplier);
			var validationResult = supplierValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newSupplier);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newSupplier.SupplierId, Success = true, Message = _localizer["NewSupplierSuccessMessage", ((language == LanguageCode.Arabic ? newSupplier.SupplierNameAr : newSupplier.SupplierNameEn) ?? ""), newSupplier.SupplierCode] };
			}
			else
			{
				return new ResponseDto() { Id = newSupplier.SupplierId, Success = false, Message = supplierValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateSupplier(SupplierDto supplier)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var supplierDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.SupplierId == supplier.SupplierId);
			if (supplierDb != null)
			{
				supplierDb.SupplierNameAr = supplier.SupplierNameAr ?? supplierDb.SupplierNameAr;
				supplierDb.SupplierNameEn = supplier.SupplierNameEn ?? supplierDb.SupplierNameEn;
				supplierDb.AccountId = supplier.AccountId ?? supplierDb.AccountId;
				supplierDb.CompanyId = supplier.CompanyId;
				supplierDb.IsActive = supplier.IsActive;
				supplierDb.InActiveReasons = supplier.InActiveReasons;
				supplierDb.IsCredit = supplier.IsCredit;
				supplierDb.CountryId = supplier.CountryId;
				supplierDb.StateId = supplier.StateId;
				supplierDb.CityId = supplier.CityId;
				supplierDb.DistrictId = supplier.DistrictId;
				supplierDb.Address1 = supplier.Address1;
				supplierDb.Address2 = supplier.Address2;
				supplierDb.Address3 = supplier.Address3;
				supplierDb.Address4 = supplier.Address4;
				supplierDb.AdditionalNumber = supplier.AdditionalNumber;
				supplierDb.BuildingNumber = supplier.BuildingNumber;
				supplierDb.CommercialRegister = supplier.CommercialRegister;
				supplierDb.CountryCode = supplier.CountryCode;
				supplierDb.Street1 = supplier.Street1;
				supplierDb.Street2 = supplier.Street2;
				supplierDb.ContractDate = supplier.ContractDate;
				supplierDb.CreditLimitDays = supplier.CreditLimitDays;
				supplierDb.CreditLimitValues = supplier.CreditLimitValues;
				supplierDb.DebitLimitDays = supplier.DebitLimitDays;
				supplierDb.ShipmentTypeId = supplier.ShipmentTypeId;
				supplierDb.TaxCode = supplier.TaxCode;
				supplierDb.PostalCode = supplier.PostalCode;
				supplierDb.FirstResponsibleName = supplier.FirstResponsibleName;
				supplierDb.FirstResponsiblePhone = supplier.FirstResponsiblePhone;
				supplierDb.FirstResponsibleEmail = supplier.FirstResponsibleEmail;
				supplierDb.SecondResponsibleName = supplier.SecondResponsibleName;
				supplierDb.SecondResponsiblePhone = supplier.SecondResponsiblePhone;
				supplierDb.SecondResponsibleEmail = supplier.SecondResponsibleEmail;
				supplierDb.ModifiedAt = DateHelper.GetDateTimeNow();
				supplierDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				supplierDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var supplierValidator = await new SupplierValidator(_localizer).ValidateAsync(supplierDb);
				var validationResult = supplierValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(supplierDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = supplierDb.SupplierId, Success = true, Message = _localizer["UpdateSupplierSuccessMessage", ((language == LanguageCode.Arabic ? supplierDb.SupplierNameAr : supplierDb.SupplierNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = supplierValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoSupplierFound"] };
		}

		public async Task<ResponseDto> DeleteSupplier(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var supplier = await _repository.GetAll().FirstOrDefaultAsync(x => x.SupplierId == id);
			if (supplier != null)
			{
				var isAccountExist = supplier.AccountId != null;
				if (isAccountExist)
				{
					return new ResponseDto() { Id = id, Success = false, Message = _localizer["SupplierIsLinkedWithAccount"] };
				}
				else
				{
					_repository.Delete(supplier);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteSupplierSuccessMessage", ((language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn) ?? "")] };
				}
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoSupplierFound"] };
		}
	}
}
