using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.FixedAssets;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.CoreOne.Contracts
{
    public interface IFixedAssetMovementDetailService : IBaseService<FixedAssetMovementDetail>
    {
        Task<FixedAssetMovementDetailResponseDto> SaveFixedAssetMovementDetail(List<FixedAssetMovementDetailDto> details, int headerId);
        Task<ResponseDto> DeleteFixedAssetMovementDetail(int id);
        IQueryable<FixedAssetMovementDetailDto> GetAllFixedAssetMovementDetails();
        Task<List<FixedAssetMovementDetailDto>> GetFixedAssetMovementDetail(int headerId);
        Task<FixedAssetMovementDetailDto> GetFixedAssetMovementDetailById(int fixedAssetMovementId);
        Task<List<FixedAssetMovementDetailDto>> GetFixedAssetMovementDetailByFixedAssetId(int fixedAssetId);
        List<RequestChangesDto> GetFixedAssetMovementDetailRequestChanges(FixedAssetMovementDetailDto oldItem, FixedAssetMovementDetailDto newItem);
    }
}
