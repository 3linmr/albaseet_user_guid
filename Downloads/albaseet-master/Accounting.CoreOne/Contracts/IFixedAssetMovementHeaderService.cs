using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Models.Dtos;

namespace Accounting.CoreOne.Contracts
{
    public interface IFixedAssetMovementHeaderService
    {
        Task<ResponseDto> SaveFixedAssetMovementHeader(FixedAssetMovementHeaderDto fixedAssetMovementHeader, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteFixedAssetMovementHeader(int id);
        IQueryable<FixedAssetMovementHeaderDto> GetAllFixedAssetMovementHeaders();
        Task<IQueryable<FixedAssetMovementHeaderDto>> GetUserFixedAssetMovementHeaders();
        Task<FixedAssetMovementHeaderDto> GetFixedAssetMovementHeaderById(int fixedAssetMovementHeaderId);
        Task<string> GetNextFixedAssetMovementHeaderCodeByStoreId(int storeId);
    }
}
