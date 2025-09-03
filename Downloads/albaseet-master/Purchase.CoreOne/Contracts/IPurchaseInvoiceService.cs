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
    public interface IPurchaseInvoiceService
    {
        List<RequestChangesDto> GetPurchaseInvoiceRequestChanges(PurchaseInvoiceDto oldItem, PurchaseInvoiceDto newItem);
        Task<PurchaseInvoiceDto> GetPurchaseInvoice(int purchaseInvoiceHeaderId);
        Task<ResponseDto> SavePurchaseInvoice(PurchaseInvoiceDto purchaseInvoice, bool hasApprove, bool approved, int? requestId, string? documentReference = null);
        Task<ResponseDto> DeletePurchaseInvoice(int purchaseInvoiceHeaderId, int menuCode);
        Task<decimal> GetLastPurchasePrice(int itemId, int itemPackageId);
        Task<List<LastPurchasePriceDto>> GetMultipleLastPurchasePrices(List<int> itemIds);
        Task<List<QuantityPriceDto>> GetLatestInvoicesBasedOnQuantity(int currentPurchaseInvoiceHeaderId, int itemId, decimal itemStockQuantity);
    }
}
