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
    public interface IPurchaseInvoiceReturnHandlingService
    {
        Task<PurchaseInvoiceReturnDto> GetPurchaseInvoiceReturnFromPurchaseInvoice(int purchaseInvoiceHeaderId, bool isDirectInvoice, bool isOnTheWay);
        Task<ResponseDto> SavePurchaseInvoiceReturn(PurchaseInvoiceReturnDto purchaseInvoiceReturn, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeletePurchaseInvoiceReturn(int purchaseInvoiceReturnHeaderId, int menuCode);
    }
}
