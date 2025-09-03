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
    public interface IPurchaseOrderHeaderService : IBaseService<PurchaseOrderHeader>
    {
        IQueryable<PurchaseOrderHeaderDto> GetPurchaseOrderHeaders();
        IQueryable<PurchaseOrderHeaderDto> GetPurchaseOrderHeadersByStoreId(int storeId, int? supplierId, int purchaseOrderHeaderId);
        IQueryable<PurchaseOrderHeaderDto> GetPurchaseOrderHeadersByStoreIdAndMenuCode(int storeId, int? supplierId, int menuCode, int purchaseOrderHeaderId);
        Task<PurchaseOrderHeaderDto?> GetPurchaseOrderHeaderById(int id);
        IQueryable<PurchaseOrderHeaderDto> GetUserPurchaseOrderHeaders();
        Task<DocumentCodeDto> GetPurchaseOrderCode(int storeId, DateTime documentDate);
        Task<bool> UpdateBlocked(int? purchaseOrderHeaderId, bool isBlocked);
        Task<bool> UpdateClosed(int? purchaseOrderHeaderId, bool isClosed);
        Task<bool> UpdateEnded(int? purchaseOrderHeaderId, bool isEnded);
        Task<bool> UpdateEndedAndClosed(int? purchaseOrderHeaderId, bool isEnded, bool isClosed);
        Task<bool> UpdateDocumentStatus(int purchaseOrderHeaderId, int documentStatusId);
        Task<ResponseDto> SavePurchaseOrderHeader(PurchaseOrderHeaderDto purchaseOrder, bool hasApprove, bool approved, int? requestId, bool shouldValidate, string? documentReference, int documentStatus, bool shouldInitializeFlags);
        Task<ResponseDto> DeletePurchaseOrderHeader(int id);
    }
}
