using Sales.CoreOne.Models.Dtos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
    public interface IStockOutQuantityService
    {
        IQueryable<StockDisbursedFromProformaInvoiceDto> GetUnInvoicedDisbursedQuantityFromProformaInvoiceWithAllKeys(int proformaInvoiceHeaderId);
		IQueryable<StockDisbursedFromProformaInvoiceDto> GetFinalStocksDisbursedFromProformaInvoice(int proformaInvoiceHeaderId);
        IQueryable<StockDisbursedFromProformaInvoiceDto> GetFinalStocksDisbursedFromProformaInvoiceExceptStockOutHeaderId(int proformaInvoiceHeaderId, int exceptStockOutHeaderId);
        IQueryable<StockDisbursedFromProformaInvoiceDto> GetFinalStocksDisbursedFromProformaInvoiceExceptStockOutReturnHeaderId(int proformaInvoiceHeaderId, int exceptStockOutHeaderId);
        IQueryable<StockDisbursedFromSalesInvoiceDto> GetFinalStocksDisbursedFromSalesInvoice(int proformaInvoiceHeaderId);
        IQueryable<StockDisbursedFromSalesInvoiceDto> GetFinalStocksDisbursedFromSalesInvoiceExceptStockOutHeaderId(int proformaInvoiceHeaderId, int exceptStockOutHeaderId);
        IQueryable<StockDisbursedFromSalesInvoiceDto> GetFinalStocksDisbursedFromSalesInvoiceExceptStockOutReturnHeaderId(int proformaInvoiceHeaderId, int exceptStockOutHeaderId);
        IQueryable<StockDisbursedReturnedDto> GetStocksDisbursedReturnedFromStockOut(int stockOutHeaderId);
        IQueryable<StockDisbursedReturnedDto> GetStocksDisbursedReturnedFromStockOutExceptStockOutReturnHeaderId(int stockOutHeaderId, int exceptStockOutReturnHeaderId);
        IQueryable<StockDisbursedReturnedDto> GetStocksDisbursedReturnedFromSalesInvoice(int salesInvoiceHeaderId);
        IQueryable<StockDisbursedReturnedDto> GetStocksDisbursedReturnedFromSalesInvoiceExceptStockOutReturnHeaderId(int salesInvoiceHeaderId, int exceptStockOutReturnHeaderId);
        IQueryable<StockDisbursedReturnedDto> GetUnInvoicedStocksDisbursedReturnedFromSalesInvoice(int salesInvoiceHeaderId);
		IQueryable<ParentQuantityDto> GetOverallQuantityAvailableFromSalesInvoices();
		IQueryable<ParentQuantityDto> GetOverallQuantityAvailableReturnFromStockOuts();
		IQueryable<ParentQuantityDto> GetOverallQuantityAvailableReturnFromSalesInvoices();
        IQueryable<ParentQuantityDto> GetOverallQuantityAvailableFromProformaInvoices();
		IQueryable<ParentQuantityDto> GetOverallUnInvoicedQuantityFromProformaInvoices();
        IQueryable<ParentQuantityDto> GetOverallQuantityReturnedFromSalesInvoices();
        IQueryable<ParentQuantityDto> GetOverallUnInvoicedQuantityReturnedFromSalesInvoices();
	}
}
