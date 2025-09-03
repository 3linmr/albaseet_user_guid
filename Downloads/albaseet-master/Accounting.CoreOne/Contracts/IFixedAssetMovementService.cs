using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.CoreOne.Contracts
{
    public interface IFixedAssetMovementService
    {
        Task<FixedAssetMovementDto> GetFixedAssetMovement(int fixedAssetMovementHeaderId);
        Task<ResponseDto> SaveFixedAssetMovement(FixedAssetMovementDto fixedAssetMovement, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteFixedAssetMovement(int fixedAssetMovementHeaderId);
        List<RequestChangesDto> GetFixedAssetMovementRequestChanges(FixedAssetMovementDto oldItem, FixedAssetMovementDto newItem);
    }
}
