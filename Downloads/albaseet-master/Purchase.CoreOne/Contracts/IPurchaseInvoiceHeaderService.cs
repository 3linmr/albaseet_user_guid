using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
    public interface IPurchaseInvoiceHeaderService : IBaseService<PurchaseInvoiceHeader>
    {
        IQueryable<PurchaseInvoiceHeaderDto> GetPurchaseInvoiceHeaders();
        IQueryable<PurchaseInvoiceHeaderDto> GetPurchaseInvoiceHeadersByStoreId(int storeId, int? supplierId, int purchaseInvoiceHeaderId, bool? isOnTheWay = null);
        Task<PurchaseInvoiceHeaderDto?> GetPurchaseInvoiceHeaderById(int id);
        IQueryable<PurchaseInvoiceHeaderDto> GetUserPurchaseInvoiceHeaders(int menuCode);
        Task<DocumentCodeDto> GetPurchaseInvoiceCode(int storeId, DateTime documentDate, bool isOnTheWay, bool isDirectInvoice, bool creditPayment);
        Task<bool> GetIsOnTheWay(int purchaseInvoiceHeaderId);
        Task UpdateHasSettlementFlag(List<int> purchaseInvoiceHeaderIds, bool hasSettlement);
        Task UpdateIsSettlementCompletedFlags(List<int> salesInvoiceHeaderIds, bool isSettlementCompleted);
		Task<bool> UpdateClosed(int? purchaseInvoiceHeaderId, bool isClosed);
        Task<bool> UpdateEnded(int? purchaseInvoiceHeaderId, bool isEnded);
        Task<bool> UpdateEndedAndClosed(int? purchaseInvoiceHeaderId, bool isEnded, bool isClosed);
        Task<bool> UpdateAllPurchaseInvoicesBlockedFromPurchaseOrder(int purchaseOrderHeaderId, bool isBlocked);
        Task<ResponseDto> SavePurchaseInvoiceHeader(PurchaseInvoiceHeaderDto purchaseInvoice, bool hasApprove, bool approved, int? requestId, string? documentReference);
        Task<int> GetNextId();
        Task<ResponseDto> DeletePurchaseInvoiceHeader(int id, int menuCode);
    }
}
