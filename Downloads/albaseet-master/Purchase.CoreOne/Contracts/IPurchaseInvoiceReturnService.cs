using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;

namespace Purchases.CoreOne.Contracts
{
    public interface IPurchaseInvoiceReturnService
    {
        List<RequestChangesDto> GetPurchaseInvoiceReturnRequestChanges(PurchaseInvoiceReturnDto oldItem, PurchaseInvoiceReturnDto newItem);
        Task<PurchaseInvoiceReturnDto> GetPurchaseInvoiceReturn(int purchaseInvoiceReturnHeaderId);
        Task<List<PurchaseInvoiceReturnDetailDto>> GetPurchaseInvoiceReturnDetailsCalculated(int purchaseInvoiceReturnHeaderId, PurchaseInvoiceReturnHeaderDto? purchaseInvoiceReturnHeader = null, List<PurchaseInvoiceReturnDetailDto>? purchaseInvoiceReturnDetails = null);
		Task<ResponseDto> SavePurchaseInvoiceReturn(PurchaseInvoiceReturnDto purchaseInvoiceReturn, bool hasApprove, bool approved, int? requestId, string? documentReference = null);
        Task<ResponseDto> DeletePurchaseInvoiceReturn(int purchaseInvoiceReturnHeaderId, int menuCode);
    }
}
