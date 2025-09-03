using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
    public interface IPurchaseOrderService
    {
        List<RequestChangesDto> GetPurchaseOrderRequestChanges(PurchaseOrderDto oldItem, PurchaseOrderDto newItem);
        Task<PurchaseOrderDto> GetPurchaseOrder(int purchaseOrderHeaderId);
        Task<PurchaseOrderDto> GetPurchaseOrderFromSupplierQuotation(int supplierQuotationHeaderId);
        Task<ResponseDto> UpdateBlocked(int purchaseOrderHeaderId, bool isBlocked);
        Task<ResponseDto> SavePurchaseOrder(PurchaseOrderDto purchaseOrder, bool hasApprove, bool approved, int? requestId, bool shouldValidate = true, string? documentReference = null, int documentStatusId = DocumentStatusData.PurchaseOrderCreated, bool shouldInitializeFlags = false);
        Task<ResponseDto> DeletePurchaseOrder(int purchaseOrderHeaderId );
    }
}
