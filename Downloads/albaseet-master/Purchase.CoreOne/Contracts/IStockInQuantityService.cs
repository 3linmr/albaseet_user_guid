using Purchases.CoreOne.Models.Dtos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
    public interface IStockInQuantityService
    {
        IQueryable<StockReceivedFromPurchaseOrderDto> GetFinalStocksReceivedFromPurchaseOrder(int purchaseOrderHeaderId);
        IQueryable<StockReceivedFromPurchaseOrderDto> GetFinalStocksReceivedFromPurchaseOrderWithAllKeys(int purchaseOrderHeaderId);
        IQueryable<StockReceivedFromPurchaseOrderDto> GetFinalStocksReceivedFromPurchaseOrderExceptStockInHeaderId(int purchaseOrderHeaderId, int exceptStockInHeaderId);
        IQueryable<StockReceivedFromPurchaseOrderDto> GetFinalStocksReceivedFromPurchaseOrderExceptStockInReturnHeaderId(int purchaseOrderHeaderId, int exceptStockInReturnHeaderId);
        IQueryable<StockReceivedFromPurchaseInvoiceDto> GetFinalStocksReceivedFromPurchaseInvoice(int purchaseInvoiceHeaderId);
        IQueryable<StockReceivedFromPurchaseInvoiceDto> GetFinalStocksReceivedFromPurchaseInvoiceExceptStockInHeaderId(int purchaseInvoiceHeaderId, int exceptStockInHeaderId);
        IQueryable<StockReceivedFromPurchaseInvoiceDto> GetFinalStocksReceivedFromPurchaseInvoiceExceptStockInReturnHeaderId(int purchaseInvoiceHeaderId, int exceptStockInReturnHeaderId);
        IQueryable<StockReturnedDto> GetStocksReturnedFromStockIn(int stockInHeaderId);
        IQueryable<StockReturnedDto> GetStocksReturnedFromStockInExceptStockInReturnHeaderId(int stockInHeaderId, int exceptStockInReturnHeaderId);
        IQueryable<StockReturnedDto> GetStocksReturnedFromPurchaseInvoice(int purchaseInvoiceHeaderId);
        IQueryable<StockReturnedDto> GetStocksReturnedFromPurchaseInvoiceExceptStockInReturnHeaderId(int purchaseInvoiceHeaderId, int? exceptStockInReturnHeaderId);
        IQueryable<ParentQuantityDto> GetOverallQuantityAvailableFromPurchaseInvoices();
        IQueryable<ParentQuantityDto> GetOverallQuantityAvailableReturnFromStockIns();
        IQueryable<ParentQuantityDto> GetOverallQuantityAvailableReturnFromPurchaseInvoices();
        IQueryable<ParentQuantityDto> GetOverallQuantityReturnedFromPurchaseInvoices();
    }
}
