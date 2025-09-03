using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.CoreOne.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.CoreOne.Contracts
{
    public interface IFixedAssetVoucherDetailService : IBaseService<FixedAssetVoucherDetail>
    {
        IQueryable<FixedAssetVoucherDetailDto> GetAllFixedAssetVoucherDetail();
        Task<FixedAssetVoucherDetailResponseDto> SaveFixedAssetVoucherDetail(List<FixedAssetVoucherDetailDto>? details, int headerId);
        Task<bool> DeleteFixedAssetVoucherDetail(int headerId);
        Task<List<FixedAssetVoucherDetailDto>> GetFixedAssetVoucherDetail(int headerId);
        Task<List<FixedAssetVoucherDetailDto>> GetFixedAssetVoucherByFixedAssetId(int fixedAssetId);
    }
}
