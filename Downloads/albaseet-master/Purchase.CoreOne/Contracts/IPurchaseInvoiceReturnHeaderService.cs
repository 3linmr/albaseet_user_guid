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
    public interface IPurchaseInvoiceReturnHeaderService : IBaseService<PurchaseInvoiceReturnHeader>
    {
        IQueryable<PurchaseInvoiceReturnHeaderDto> GetPurchaseInvoiceReturnHeadersByStoreId(int storeId, int? supplierId, int purchaseInvoiceReturnHeaderId);
        Task<PurchaseInvoiceReturnHeaderDto?> GetPurchaseInvoiceReturnHeaderById(int id);
        IQueryable<PurchaseInvoiceReturnHeaderDto> GetUserPurchaseInvoiceReturnHeaders(int menuCode);
        Task<DocumentCodeDto> GetPurchaseInvoiceReturnCode(int storeId, DateTime documentDate, bool isOnTheWay, bool isDirectInvoice, bool creditPayment);
        Task<bool> UpdateClosed(int? purchaseInvoiceReturnHeaderId, bool isClosed);
        Task<bool> UpdateEnded(int? purchaseInvoiceReturnHeaderId, bool isEnded);
        Task<bool> UpdatePurchaseInvoiceReturnOnTheWayEndedLinkedToPurchaseInvoice(int? purchaseInvoiceHeaderId, bool isEnded);
        Task<bool> UpdatePurchaseInvoiceReturnNotOnTheWayEndedLinkedToPurchaseInvoice(int? purchaseInvoiceHeaderId, bool isEnded);
        Task<bool> UpdateAllPurchaseInvoiceReturnsBlockedFromPurchaseOrder(int purchaseOrderHeaderId, bool isBlocked);
        Task<ResponseDto> SavePurchaseInvoiceReturnHeader(PurchaseInvoiceReturnHeaderDto purchaseInvoiceReturn, bool hasApprove, bool approved, int? requestId, string? documentReference);
        Task<int> GetNextId();
        Task<ResponseDto> DeletePurchaseInvoiceReturnHeader(int id, int menuCode);
    }
}
