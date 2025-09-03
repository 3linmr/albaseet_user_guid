using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.FixedAssets;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.CostCenters;
using Shared.CoreOne.Models.Dtos.ViewModels.FixedAssets;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.FixedAssets
{
    public interface IFixedAssetService : IBaseService<FixedAsset>
    {
        Task<ResponseDto> SaveFixedAsset(FixedAssetDto fixedAsset);
        Task<ResponseDto> DeleteFixedAsset(int id);
        Task<ResponseDto> DeleteFixedAssetInFull(int id);
        IQueryable<FixedAssetDto> GetAllFixedAssets();
        Task<FixedAssetDto> GetFixedAssetByFixedAssetCode(int companyId, string fixedAssetCode);
        Task<bool> IsFixedAssetHasChildren(int fixedAssetId);
        IQueryable<FixedAssetTreeDto> GetFixedAssetsTree(int companyId);
        Task<string> GetNextFixedAssetCode(int companyId, int mainFixedAssetId, bool isMainFixedAsset);
        Task<List<FixedAssetAutoCompleteDto>> GetMainFixedAssetsByFixedAssetId(int companyId, int fixedAssetId);
        Task<List<FixedAssetAutoCompleteDto>> GetMainFixedAssetsByFixedAssetCode(int companyId, string fixedAssetCode);
        Task<List<FixedAssetAutoCompleteDto>> GetMainFixedAssetsByFixedAssetName(int companyId, string fixedAssetName);
        Task<FixedAssetDto> GetFixedAssetByFixedAssetId(int fixedAssetId);
        Task<List<FixedAssetDropDownDto>> GetFixedAssetsDropDownByCompanyId(int companyId);
        List<RequestChangesDto> GetFixedAssetRequestChanges(FixedAssetDto oldItem, FixedAssetDto newItem);
        Task<string?> GetFixedAssetTreeName(int fixedAssetId);
        Task<List<FixedAssetAutoCompleteDto>> GetTreeList(int fixedAssetId);
    }
}
