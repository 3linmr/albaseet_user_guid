using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Models.StaticData;
using Shared.Helper.Models.UserDetail;
using Shared.Service.Validators;
using static System.Formats.Asn1.AsnWriter;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using LanguageData = Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class StoreService : BaseService<Store>,IStoreService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStoreClassificationService _storeClassificationService;
		private readonly ICountryService _countryService;
		private readonly IStateService _stateService;
		private readonly ICityService _cityService;
		private readonly IDistrictService _districtService;
		private readonly ICompanyService _companyService;
		private readonly IBranchService _branchService;
		private readonly ICurrencyService _currencyService;
		private readonly IStringLocalizer<StoreService> _localizer;

		public StoreService(IRepository<Store> repository,IHttpContextAccessor httpContextAccessor,IStoreClassificationService storeClassificationService,ICountryService countryService,IStateService stateService,ICityService cityService,IDistrictService districtService,ICompanyService companyService,IBranchService branchService,ICurrencyService currencyService,IStringLocalizer<StoreService> localizer) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_storeClassificationService = storeClassificationService;
			_countryService = countryService;
			_stateService = stateService;
			_cityService = cityService;
			_districtService = districtService;
			_companyService = companyService;
			_branchService = branchService;
			_currencyService = currencyService;
			_localizer = localizer;
		}

		public IQueryable<StoreDto> GetAllStores()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =  
				from store in _repository.GetAll()
				from storeClassification in _storeClassificationService.GetAll().Where(x=>x.StoreClassificationId == store.StoreClassificationId)
				from branch in _branchService.GetAll().Where(x=>x.BranchId == store.BranchId)
				from company in _companyService.GetAll().Where(x=>x.CompanyId == branch.CompanyId)
				from currency in _currencyService.GetAll().Where(x=>x.CurrencyId == company.CurrencyId)
				from country in _countryService.GetAll().Where(x=>x.CountryId == store.CountryId).DefaultIfEmpty()
				from state in _stateService.GetAll().Where(x=>x.StateId == store.StateId).DefaultIfEmpty()
				from city in _cityService.GetAll().Where(x=>x.CityId == store.CityId).DefaultIfEmpty()
				from district in _districtService.GetAll().Where(x=>x.DistrictId == store.DistrictId).DefaultIfEmpty()
				select new StoreDto()
				{
					StoreId = store.StoreId,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					StoreNameAr = store.StoreNameAr,
					StoreNameEn = store.StoreNameEn,
					BranchId = branch.BranchId,
					CompanyId = company.CompanyId,
					IsActive = store.IsActive,
					InActiveReasons = store.InActiveReasons,
					IsActiveName = store.IsActive ? _localizer["Active"].Value : _localizer["InActive"].Value,
					Address1 = store.Address1,
					Address2 = store.Address2,
					Address3 = store.Address3,
					Address4 = store.Address4,
					AdditionalNumber = store.AdditionalNumber,
					BuildingNumber = store.BuildingNumber,
					CityId = store.CityId,
					CommercialRegister = store.CommercialRegister,
					CountryId = store.CountryId,
					CountryCode = store.CountryCode,
					DistrictId = store.DistrictId,
					GMap = store.GMap,
					Lat = store.Lat,
					Long = store.Long,
					StateId = store.StateId,
					CanAcceptDirectInternalTransfer = store.CanAcceptDirectInternalTransfer,
					ExpenseAccountId = store.ExpenseAccountId,
					StockCreditAccountId = store.StockCreditAccountId,
					StockDebitAccountId = store.StockDebitAccountId,
					StoreClassificationId = store.StoreClassificationId,
					Street1 = store.Street1,
					Street2 = store.Street1,
					NoReplenishment = store.NoReplenishment,
					PostalCode = store.PostalCode,
					CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
					BranchName = language == LanguageCode.Arabic ? branch.BranchNameAr : branch.BranchNameEn,
					StoreClassificationName = language == LanguageCode.Arabic ? storeClassification.ClassificationNameAr : storeClassification.ClassificationNameEn,
					CountryName = country != null ? language == LanguageCode.Arabic ? country.CountryNameAr : country.CountryNameEn: "",
					StateName = state != null ? language == LanguageCode.Arabic ? state.StateNameAr : state.StateNameEn : "",
					CityName = city != null ? language == LanguageCode.Arabic ? city.CityNameAr : city.CityNameEn : "",
					DistrictName = district != null ? language == LanguageCode.Arabic ? district.DistrictNameAr : district.DistrictNameEn : "",
					CurrencyId = company.CurrencyId,
					Rounding = NumberHelper.GetTrailingZerosFromInteger(currency.NumberToBasic),
					CurrencyName = language == LanguageCode.Arabic ? currency.CurrencyNameAr : currency.CurrencyNameEn,
					ReservedParentStoreId = store.ReservedParentStoreId,
					IsReservedStore = store.IsReservedStore
				};
			return data;
		}

		public async Task<List<StoreDto>> GetUserStores()
		{
			var userStores = await _httpContextAccessor.GetUserStores();
			var stores = await GetAllStores().Where(x=>x.IsActive && userStores.Contains(x.StoreId)).ToListAsync();
			return stores;
		}

		public async Task<List<StoreDropDownDto>> GetAllUserStoresDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var stores = await GetUserStores();
			return stores.Where(x=>x.IsActive).Select(x=> new StoreDropDownDto()
			{
				StoreId = x.StoreId,
				StoreName = language == LanguageData.LanguageCode.Arabic ? x.StoreNameAr : x.StoreNameEn,
				CurrencyId = x.CurrencyId,
				Rounding = x.Rounding,
                BranchId = x.BranchId,
                CompanyId = x.CompanyId
            }).OrderBy(x => x.StoreName).ToList();
		}

		public async Task<List<StoreDropDownDto>> GetUserStoresDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var stores = await GetUserStores();
			return stores.Where(x => x.IsActive && !x.IsReservedStore).Select(x => new StoreDropDownDto()
			{
				StoreId = x.StoreId,
				StoreName = language == LanguageData.LanguageCode.Arabic ? x.StoreNameAr : x.StoreNameEn,
				CurrencyId = x.CurrencyId,
				Rounding = x.Rounding,
				BranchId = x.BranchId,
				CompanyId = x.CompanyId
			}).OrderBy(x => x.StoreName).ToList();
		}

		public async Task<List<StoreDropDownDto>> GetAllStoresDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return await GetAllStores().Where(x => x.IsActive ).Select(x => new StoreDropDownDto()
			{
				StoreId = x.StoreId,
				StoreName = language == LanguageData.LanguageCode.Arabic ? x.StoreNameAr : x.StoreNameEn,
				CurrencyId = x.CurrencyId,
				Rounding = x.Rounding,
				BranchId = x.BranchId,
				CompanyId = x.CompanyId,
				IsReservedStore = x.IsReservedStore
			}).OrderBy(x => x.StoreName).ToListAsync();
		}

		public async Task<List<StoreDropDownDto>> GetStoresDropDown()
		{
			var stores = await GetAllStoresDropDown();
			return stores.Where(x => !x.IsReservedStore).ToList();
		}

		public async Task<List<StoreDropDownDto>> GetCompanyStoresDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return await GetAllStores().Where(x => x.IsActive && x.CompanyId == companyId && !x.IsReservedStore).Select(x => new StoreDropDownDto()
			{
				StoreId = x.StoreId,
				StoreName = language == LanguageCode.Arabic ? x.StoreNameAr : x.StoreNameEn,
				CurrencyId = x.CurrencyId,
				Rounding = x.Rounding,
				BranchId = x.BranchId,
				CompanyId = x.CompanyId,
				IsReservedStore = x.IsReservedStore
			}).OrderBy(x => x.StoreName).ToListAsync();
		}

		public async Task<List<StoreDropDownVm>> GetAllStoresFullNameDropDown(int companyId)
		{
			var stores =  await GetAllStores().Where(x => x.IsActive && x.CompanyId == companyId && !x.IsReservedStore).ToListAsync();
			return stores.Select(x => new StoreDropDownVm()
			{
				StoreId = x.StoreId,
				StoreName = $"{x.BranchName} - {x.StoreName}"
			}).OrderBy(x => x.StoreName).ToList();
		}

		public Task<List<NameDto>> GetStoresDropDownForAdmin(int branchId)
		{
			return GetAllStores().Where(x=>x.BranchId == branchId && !x.IsReservedStore && x.IsActive).Select(x => new NameDto()
			{
				Id = x.StoreId,
				Name = x.StoreName
			}).ToListAsync();
		}

		public Task<StoreDto?> GetStoreById(int id)
		{
			return GetAllStores().FirstOrDefaultAsync(x => x.StoreId == id);
		}

		public async Task<int> GetReservedStoreByParentStoreId(int parentStoreId)
		{
			var reservedStoreExist = await _repository.GetAll().AsNoTracking().Where(x=>x.ReservedParentStoreId == parentStoreId).Select(x=>x.StoreId).FirstOrDefaultAsync();
			if (reservedStoreExist > 0)
			{
				return reservedStoreExist;
			}
			else
			{
				var parentStore = await GetStoreById(parentStoreId);
				if (parentStore != null)
				{
					parentStore.CanAcceptDirectInternalTransfer = true;
					await UpdateStore(parentStore);

					var newStore = parentStore;
					newStore.ReservedParentStoreId = parentStore.StoreId;
					newStore.StoreId = 0;
					newStore.StoreNameAr = $"{parentStore.StoreNameAr} - محجوز";
					newStore.StoreNameEn = $"{parentStore.StoreNameEn} - Reserved";
					newStore.IsReservedStore = true;
					var result = await CreateStore(newStore,true);
					return result.Id;

				}
			}
			return 0;
		}

		public async Task<int> GetStoreRounding(int storeId)
		{
			var store =  await GetStoreById(storeId);
			return store?.Rounding ?? 2;
		}

        public async Task<int> GetStoreHeaderRounding(int storeId)
        {
            return await GetStoreRounding(storeId) + 1;
        }

        public async Task<int> GetCompanyIdByStoreId(int storeId)
		{
			var data =
				await (from store in _repository.GetAll().Where(x=>x.StoreId == storeId)
				from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
				from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
				select company.CompanyId).FirstOrDefaultAsync();
			return data;
		}

		public async Task<ResponseDto> IsStoreLimitReached()
		{
			var storeCount = await _repository.GetAll().CountAsync();
			var businessCount = await IdentityHelper.GetSubscriptionBusinessCount();
			if (businessCount.StoresCount <= storeCount)
			{
				return new ResponseDto() { Success = true, Message = _localizer["StoreLimitReached"] };
			}
			return new ResponseDto() { Success = false };
		}

		public async Task<TaxDetailDto> GetStoreTaxDetails(int storeId)
		{
			var data =
				await (from store in _repository.GetAll().Where(x => x.StoreId == storeId)
					from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
					from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
					select new TaxDetailDto
					{
						TaxCode = company.TaxCode,
						CommercialRegister = store.CommercialRegister
					}).FirstOrDefaultAsync();
			return data ?? new TaxDetailDto();
		}

		public async Task<ResponseDto> CreateStoreFromBranch(CompanyDto company,int branchId, bool fromSelfCreation)
		{
			var store = new StoreDto()
			{
				BranchId = branchId,
				CurrencyId = company.CurrencyId,
				StoreNameAr = company.CompanyNameAr,
				StoreNameEn = company.CompanyNameEn,
				StoreClassificationId = StoreClassificationData.Commercial,
				IsActive = true
			};
			return await SaveStore(store, fromSelfCreation);
		}

		public async Task<ResponseDto> SaveStore(StoreDto store, bool fromSelfCreation)
		{
			var storeExist = await IsStoreExist(store.StoreId, store.StoreNameAr, store.StoreNameEn);
			if (storeExist.Success)
			{
				return new ResponseDto() { Id = storeExist.Id, Success = false, Message = _localizer["StoreAlreadyExist"] };
			}
			else
			{
				if (store.StoreId == 0)
				{
					return await CreateStore(store, fromSelfCreation);
				}
				else
				{
					//UpdateParentStore
					var result = await UpdateStore(store);
					if (result.Success)
					{   //UpdateReservedStoreIfExist
						await UpdateReservedStore(store.StoreId);
						return result;
					}
				}

				return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoStoreFound"] };
			}
		}

		public async Task<ResponseDto> IsStoreExist(int id, string? nameAr, string? nameEn)
		{
			var store = await _repository.GetAll().FirstOrDefaultAsync(x => (x.StoreNameAr == nameAr || x.StoreNameEn == nameEn || x.StoreNameAr == nameEn || x.StoreNameEn == nameAr) && x.StoreId != id);
			if (store != null)
			{
				return new ResponseDto() { Id = store.StoreId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.StoreId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateStore(StoreDto store, bool fromSelfCreation)
		{
			var storeCount = await _repository.GetAll().Where(x=>!x.IsReservedStore).CountAsync();
			var businessCount= new SubscriptionBusinessCountDto();
			if (!fromSelfCreation)
			{
				businessCount = await IdentityHelper.GetSubscriptionBusinessCount();
			}
			if (businessCount.StoresCount > storeCount || fromSelfCreation)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();
				var newStore = new Store()
				{
					StoreId = await GetNextId(),
					StoreNameAr = store.StoreNameAr,
					StoreNameEn = store.StoreNameEn,
					BranchId = store.BranchId,
					IsActive = store.IsActive,
					InActiveReasons = store.InActiveReasons,
					Address1 = store.Address1,
					Address2 = store.Address2,
					Address3 = store.Address3,
					Address4 = store.Address4,
					AdditionalNumber = store.AdditionalNumber,
					BuildingNumber = store.BuildingNumber,
					CountryId = store.CountryId > 0 ? store.CountryId : null,
					StateId = store.StateId > 0 ? store.StateId : null,
					CityId = store.CityId > 0 ? store.CityId : null,
					DistrictId = store.DistrictId > 0 ? store.DistrictId : null,
					CommercialRegister = store.CommercialRegister,
					CountryCode = store.CountryCode,
					GMap = store.GMap,
					Lat = store.Lat,
					Long = store.Long,
					CanAcceptDirectInternalTransfer = store.CanAcceptDirectInternalTransfer,
					ExpenseAccountId = store.ExpenseAccountId,
					StockCreditAccountId = store.StockCreditAccountId,
					StockDebitAccountId = store.StockDebitAccountId,
					StoreClassificationId = store.StoreClassificationId,
					Street1 = store.Street1,
					Street2 = store.Street1,
					NoReplenishment = store.NoReplenishment,
					PostalCode = store.PostalCode,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor!.GetUserName(),
					IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
					Hide = false,
					IsReservedStore = store.IsReservedStore,
					ReservedParentStoreId = store.ReservedParentStoreId
				};

				var storeValidator = await new StoreValidator(_localizer).ValidateAsync(newStore);
				var validationResult = storeValidator.IsValid;
				if (validationResult)
				{
					await _repository.Insert(newStore);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = newStore.StoreId, Success = true, Message = _localizer["NewStoreSuccessMessage", ((language == LanguageCode.Arabic ? newStore.StoreNameAr : newStore.StoreNameEn) ?? ""), newStore.StoreId] };
				}
				else
				{
					return new ResponseDto() { Id = newStore.StoreId, Success = false, Message = storeValidator.ToString("~") };
				}
			}
			else
			{
				return new ResponseDto() { Id = 0, Success = false, Message = _localizer["StoreLimitReached"] };
			}
		}

		public async Task<ResponseDto> UpdateStore(StoreDto store)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var storeDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.StoreId == store.StoreId);
			if (storeDb != null)
			{
				storeDb.StoreNameAr = store.StoreNameAr?.Trim();
				storeDb.StoreNameEn = store.StoreNameEn?.Trim();
				storeDb.BranchId = store.BranchId;
				storeDb.IsActive = store.IsActive;
				storeDb.InActiveReasons = store.InActiveReasons;
				storeDb.Address1 = store.Address1;
				storeDb.Address2 = store.Address2;
				storeDb.Address3 = store.Address3;
				storeDb.Address4 = store.Address4;
				storeDb.AdditionalNumber = store.AdditionalNumber;
				storeDb.BuildingNumber = store.BuildingNumber;
				storeDb.CityId = store.CityId;
				storeDb.CommercialRegister = store.CommercialRegister;
				storeDb.CountryId = store.CountryId;
				storeDb.CountryCode = store.CountryCode;
				storeDb.DistrictId = store.DistrictId;
				storeDb.GMap = store.GMap;
				storeDb.Lat = store.Lat;
				storeDb.Long = store.Long;
				storeDb.StateId = store.StateId;
				storeDb.CanAcceptDirectInternalTransfer = store.CanAcceptDirectInternalTransfer;
				storeDb.ExpenseAccountId = store.ExpenseAccountId;
				storeDb.StockCreditAccountId = store.StockCreditAccountId;
				storeDb.StockDebitAccountId = store.StockDebitAccountId;
				storeDb.StoreClassificationId = store.StoreClassificationId;
				storeDb.Street1 = store.Street1;
				storeDb.Street2 = store.Street1;
				storeDb.NoReplenishment = store.NoReplenishment;
				storeDb.PostalCode = store.PostalCode;
				storeDb.ModifiedAt = DateHelper.GetDateTimeNow();
				storeDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				storeDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var storeValidator = await new StoreValidator(_localizer).ValidateAsync(storeDb);
				var validationResult = storeValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(storeDb);
					await _repository.SaveChanges();


					return new ResponseDto() { Id = storeDb.StoreId, Success = true, Message = _localizer["UpdateStoreSuccessMessage", ((language == LanguageCode.Arabic ? storeDb.StoreNameAr : storeDb.StoreNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = storeValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoStoreFound"] };
		}

		public async Task<bool> UpdateReservedStore(int parentStoreId)
		{
			var reservedStoreId = await _repository.GetAll().AsNoTracking().Where(x=>x.ReservedParentStoreId == parentStoreId).Select(x=>x.StoreId).FirstOrDefaultAsync();
			if (reservedStoreId > 0)
			{
				var reservedStore = await GetStoreById(reservedStoreId);
				var parentStore = await GetStoreById(parentStoreId);
				if (reservedStore != null && parentStore != null)
				{
					reservedStore.StoreNameAr = $"{parentStore.StoreNameAr} - محجوز";
					reservedStore.StoreNameEn = $"{parentStore.StoreNameEn} - Reserved";
					reservedStore.BranchId = parentStore.BranchId;
					reservedStore.IsActive = parentStore.IsActive;
					reservedStore.InActiveReasons = parentStore.InActiveReasons;
					reservedStore.Address1 = parentStore.Address1;
					reservedStore.Address2 = parentStore.Address2;
					reservedStore.Address3 = parentStore.Address3;
					reservedStore.Address4 = parentStore.Address4;
					reservedStore.AdditionalNumber = parentStore.AdditionalNumber;
					reservedStore.BuildingNumber = parentStore.BuildingNumber;
					reservedStore.CityId = parentStore.CityId;
					reservedStore.CommercialRegister = parentStore.CommercialRegister;
					reservedStore.CountryId = parentStore.CountryId;
					reservedStore.CountryCode = parentStore.CountryCode;
					reservedStore.DistrictId = parentStore.DistrictId;
					reservedStore.GMap = parentStore.GMap;
					reservedStore.Lat = parentStore.Lat;
					reservedStore.Long = parentStore.Long;
					reservedStore.StateId = parentStore.StateId;
					reservedStore.CanAcceptDirectInternalTransfer = parentStore.CanAcceptDirectInternalTransfer;
					reservedStore.ExpenseAccountId = parentStore.ExpenseAccountId;
					reservedStore.StockCreditAccountId = parentStore.StockCreditAccountId;
					reservedStore.StockDebitAccountId = parentStore.StockDebitAccountId;
					reservedStore.StoreClassificationId = parentStore.StoreClassificationId;
					reservedStore.Street1 = parentStore.Street1;
					reservedStore.Street2 = parentStore.Street1;
					reservedStore.NoReplenishment = parentStore.NoReplenishment;
					reservedStore.PostalCode = parentStore.PostalCode;

					await UpdateStore(reservedStore);
					return true;
				}
			}
			return false;
		}

		public async Task<ResponseDto> DeleteStore(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var store = await _repository.GetAll().FirstOrDefaultAsync(x => x.StoreId == id);
			if (store != null)
			{
				_repository.Delete(store);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteStoreSuccessMessage", ((language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoStoreFound"] };
		}
	}
}
