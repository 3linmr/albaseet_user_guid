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
    public interface IStockInHeaderService : IBaseService<StockInHeader>
    {
        IQueryable<StockInHeaderDto> GetStockInHeaders();
        Task<StockInHeaderDto?> GetStockInHeaderById(int id);
        Task<bool> UpdateClosed(int? stockInHeaderId, bool isClosed);
        Task<bool> UpdateAllStockInsBlockedFromPurchaseOrder(int purchaseOrderHeaderId, bool isBlocked);
        Task<int> UpdateAllStockInsEndedDirectlyFromPurchaseOrder(int purchaseOrderHeaderId, bool isEnded);
        Task<int> UpdateAllStockInsEndedDirectlyFromPurchaseInvoice(int purchaseInvoiceHeaderId, bool isEnded);
		IQueryable<StockInHeaderDto> GetUserStockInHeaders(int stockTypeId);
        Task<DocumentCodeDto> GetStockInCode(int storeId, DateTime documentDate, int stockTypeId);
        Task<ResponseDto> SaveStockInHeader(StockInHeaderDto stockIn, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags);
        Task<ResponseDto> DeleteStockInHeader(int id, int menuCode);
    }
}
