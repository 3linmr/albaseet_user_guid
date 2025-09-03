using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
    public interface IPurchaseInvoiceHandlingService
    {
        Task<PurchaseInvoiceDto> GetPurchaseInvoiceFromPurchaseOrder(int purchaseOrderHeaderId);
		Task<PurchaseInvoiceDto> GetPurchaseInvoiceFromSupplierQuotation(int supplierQuotationHeaderId);
		IQueryable<PurchaseInvoiceHeaderDto> GetPurchaseInvoiceHeadersByStoreIdAndMenuCode(int storeId, int? supplierId, int menuCode, int purchaseInvoiceHeaderId);
        Task<ResponseDto> SavePurchaseInvoice(PurchaseInvoiceDto purchaseInvoice, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeletePurchaseInvoice(int purchaseInvoiceHeaderId, int menuCode);
    }
}
