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
    public interface IStockInReturnHeaderService : IBaseService<StockInReturnHeader>
    {
        IQueryable<StockInReturnHeaderDto> GetStockInReturnHeaders();
        IQueryable<StockInReturnHeaderDto> GetStockInReturnHeadersByStoreId(int storeId, int? supplierId, int stockTypeId, int stockInReturnHeaderId);
        Task<StockInReturnHeaderDto?> GetStockInReturnHeaderById(int id);
        IQueryable<StockInReturnHeaderDto> GetUserStockInReturnHeaders(int stockTypeId);
        Task<DocumentCodeDto> GetStockInReturnCode(int storeId, DateTime documentDate, int stockTypeId);
        Task<bool> UpdateAllStockInReturnsBlockedFromPurchaseOrder(int purchaseOrderHeaderId, bool isBlocked);
        Task<int> UpdateAllStockInReturnsEndedDirectlyFromPurchaseOrder(int purchaseOrderHeaderId, bool isEnded);
        Task<int> UpdateAllStockInReturnsOnTheWayEndedFromPurchaseInvoice(int purchaseInvoiceHeaderId, bool isEnded);
        Task<int> UpdateAllStockInReturnsEndedDirectlyFromPurchaseInvoice(int purchaseInvoiceHeaderId, bool isEnded);
        Task<ResponseDto> SaveStockInReturnHeader(StockInReturnHeaderDto stockInReturn, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags);
        Task<ResponseDto> DeleteStockInReturnHeader(int id, int menuCode);
    }
}
