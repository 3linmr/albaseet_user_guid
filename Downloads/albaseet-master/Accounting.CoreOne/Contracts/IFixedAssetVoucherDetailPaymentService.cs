using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.CoreOne.Contracts
{
    public interface IFixedAssetVoucherDetailPaymentService : IBaseService<FixedAssetVoucherDetailPayment>
    {
        IQueryable<FixedAssetVoucherDetailPaymentDto> GetAllFixedAssetVoucherDetailPayment();
        Task<bool> SaveFixedAssetVoucherDetailPayment(List<FixedAssetVoucherDetailPaymentDto>? payments, int detailId);
        Task<bool> DeleteFixedAssetVoucherDetailPayment(int detailId);
        Task<List<FixedAssetVoucherDetailPaymentDto>> GetFixedAssetVoucherDetailPayment(int detailId);
        Task<List<FixedAssetVoucherDetailPaymentDto>> GetFixedAssetVoucherDetailPaymentByFixedAssetId(int fixedAssetId);
    }
}
