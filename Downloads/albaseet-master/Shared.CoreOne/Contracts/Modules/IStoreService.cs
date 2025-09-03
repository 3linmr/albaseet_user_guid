using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Modules
{
	public interface IStoreService : IBaseService<Store>
    {
		IQueryable<StoreDto> GetAllStores();
		Task<List<StoreDto>> GetUserStores();
		Task<List<StoreDropDownDto>> GetAllUserStoresDropDown();
		Task<List<StoreDropDownDto>> GetUserStoresDropDown();
		Task<List<StoreDropDownDto>> GetAllStoresDropDown();
		Task<List<StoreDropDownDto>> GetStoresDropDown();
		Task<List<StoreDropDownDto>> GetCompanyStoresDropDown();
		Task<List<StoreDropDownVm>> GetAllStoresFullNameDropDown(int companyId);
		Task<List<NameDto>> GetStoresDropDownForAdmin(int branchId);
		Task<StoreDto?> GetStoreById(int id);
		Task<int> GetReservedStoreByParentStoreId(int parentStoreId);
		Task<int> GetStoreRounding(int storeId);
		Task<int> GetStoreHeaderRounding(int storeId);
		Task<int> GetCompanyIdByStoreId(int storeId);
		Task<ResponseDto> IsStoreLimitReached();
		Task<TaxDetailDto> GetStoreTaxDetails(int storeId);
		Task<ResponseDto> CreateStoreFromBranch(CompanyDto company, int branchId, bool fromSelfCreation);
		Task<ResponseDto> SaveStore(StoreDto store, bool fromSelfCreation);
		Task<ResponseDto> DeleteStore(int id);
	}
}
