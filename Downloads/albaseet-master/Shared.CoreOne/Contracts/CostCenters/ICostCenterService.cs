using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.CoreOne.Models.Dtos.ViewModels.CostCenters;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.CostCenters;
using Shared.Helper.Models.Dtos;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;

namespace Shared.CoreOne.Contracts.CostCenters
{
	public interface ICostCenterService : IBaseService<CostCenter>
	{
		IQueryable<CostCenterDropDownDto> GetCostCentersDropDown();
		IQueryable<CostCenterTreeDto> GetCostCentersTree(int companyId);
		IQueryable<CostCenterDropDownDto> GetMainCostCentersByCompanyId(int companyId);
		IQueryable<CostCenterDropDownDto> GetMainCostCentersByStoreId(int storeId);
		IQueryable<CostCenterDropDownDto> GetIndividualCostCentersByCompanyId(int companyId);
		IQueryable<CostCenterDropDownDto> GetIndividualCostCentersByStoreId(int storeId);
		Task<string> GetNextCostCenterCode(int companyId, int mainCostCenterId, bool isMainCostCenter);
		Task<List<CostCenterAutoCompleteDto>> GetMainCostCentersByCostCenterCode(int companyId, string costCenterCode);
		Task<List<CostCenterAutoCompleteDto>> GetMainCostCentersByCostCenterName(int companyId, string costCenterName);
		IQueryable<CostCenterDto> GetAllCostCenters();
		IQueryable<CostCenterDto> GetCompanyCostCenters(int companyId);
		Task<CostCenterDto> GetCostCenterByCostCenterId(int costCenterId);
		Task<CostCenterDto> GetCostCenterByCostCenterCode(int companyId, string costCenterCode);

		List<RequestChangesDto> GetCostCenterRequestChanges(CostCenterDto oldItem, CostCenterDto newItem);
		Task<ResponseDto> SaveCostCenter(CostCenterDto costCenter);
		Task<ResponseDto> SaveCostCenterInFull(CostCenterDto costCenter);
		Task<ResponseDto> DeleteCostCenter(int id);
		Task<ResponseDto> DeleteCostCenterInFull(int id);
		Task<List<CostCenterAutoCompleteDto>> GetTreeList(int costCenterId);
		Task<string?> GetCostCenterTreeName(int costCenterId);
	}
}
