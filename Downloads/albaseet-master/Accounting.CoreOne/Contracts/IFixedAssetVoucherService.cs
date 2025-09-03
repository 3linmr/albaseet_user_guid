using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.FixedAssets;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.CoreOne.Contracts
{
    public interface IFixedAssetVoucherService
    { 
		List<RequestChangesDto> GetFixedAssetVoucherRequestChanges(FixedAssetVoucherDto oldItem, FixedAssetVoucherDto newItem);
        Task<FixedAssetVoucherDto> GetFixedAssetVoucher(int fixedAssetVoucherHeaderId);
        Task<List<FixedAssetVoucherDetailDto>> GetFixedAssetDepreciation(int fixedAssetId, DateTime? fromDate, DateTime? toDate);
        Task<List<FixedAssetVoucherDetailDto>> GetAllFixedAssetsDepreciation(DateTime? fromDate, DateTime? toDate);
        Task<ResponseDto> SaveFixedAssetVoucher(FixedAssetVoucherDto fixedAssetVoucher, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> SaveFixedAssetAdditionVoucher(
            FixedAssetAdditionDto fixedAssetAddition,
            bool hasApprove,
            bool approved,
            int? requestId);
        Task<ResponseDto> SaveAllFixedAssetDepreciationVoucher(
            DateTime depreciationDate,
            int? storeId,
            bool hasApprove,
            bool approved,
            int? requestId);
        Task<ResponseDto> SaveFixedAssetDepreciationVoucher(
            FixedAssetDto fixedAsset,
            DateTime depreciationDate,
            int? storeId,
            bool hasApprove,
            bool approved,
            int? requestId);
        Task<ResponseDto> SaveFixedAssetExclusionVoucher(
            FixedAssetExclusionDto fixedAssetExclusion,
            bool hasApprove,
            bool approved,
            int? requestId);
        Task<ResponseDto> DeleteFixedAssetVoucher(int fixedAssetVoucherHeaderId);
    }
}
