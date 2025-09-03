using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
    public interface IStockInHandlingService
    {
        IQueryable<StockInHeaderDto> GetStockInHeadersByStoreId(int storeId, int? supplierId, int stockTypeId, int stockInHeaderId);
        Task<StockInDto> GetStockInFromPurchaseOrder(int purchaseOrderHeaderId);
        Task<StockInDto> GetStockInFromPurchaseInvoice(int purchaseInvoiceHeaderId);
        Task<ResponseDto> SaveStockIn(StockInDto stockIn, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteStockIn(int stockInHeaderId, int menuCode);
        Task<List<StockInDetailDto>> GetStockInDetailsCalculated(int stockInHeaderId, int? purchaseOrderHeaderId = null, int? purchaseInvoiceHeaderId = null, List<StockInDetailDto>? stockInDetailDtos = null);
        Task<StockInDto> GetStockIn(int stockInHeaderId);
    }
}
