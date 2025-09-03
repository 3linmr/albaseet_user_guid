using Compound.CoreOne.Contracts.Reports.FollowUpCombinedReports;
using Compound.CoreOne.Contracts.Reports.FollowUpDetailReports;
using Compound.CoreOne.Contracts.Reports.FollowUpReports;
using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.Service.Services.Reports.FollowUpCombinedReports
{
	public class StockInReturnFromPurchaseInvoiceFollowUpCombinedReportService: IStockInReturnFromPurchaseInvoiceFollowUpCombinedReportService
	{
		private readonly IStockInReturnFromPurchaseInvoiceFollowUpReportService _stockInReturnFromPurchaseInvoiceFollowUpReportService;
		private readonly IStockInReturnFromPurchaseInvoiceFollowUpDetailReportService _stockInReturnFromPurchaseInvoiceFollowUpDetailReportService;

		public StockInReturnFromPurchaseInvoiceFollowUpCombinedReportService(IStockInReturnFromPurchaseInvoiceFollowUpReportService stockInReturnFromPurchaseInvoiceFollowUpReportService, IStockInReturnFromPurchaseInvoiceFollowUpDetailReportService stockInReturnFromPurchaseInvoiceFollowUpDetailReportService)
		{
			_stockInReturnFromPurchaseInvoiceFollowUpReportService = stockInReturnFromPurchaseInvoiceFollowUpReportService;
			_stockInReturnFromPurchaseInvoiceFollowUpDetailReportService = stockInReturnFromPurchaseInvoiceFollowUpDetailReportService;
		}

		public async Task<IQueryable<StockInReturnFromPurchaseInvoiceFollowUpReportVm>> GetStockInReturnFromPurchaseInvoiceFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
		{
			return isDetail ? (await _stockInReturnFromPurchaseInvoiceFollowUpDetailReportService.GetStockInReturnFromPurchaseInvoiceFollowUpDetailReport(storeIds, fromDate, toDate)).Select(x => new StockInReturnFromPurchaseInvoiceFollowUpReportVm { StockInReturnFromPurchaseInvoiceFollowUpDetailReportDto = x }) :
							  _stockInReturnFromPurchaseInvoiceFollowUpReportService.GetStockInReturnFromPurchaseInvoiceFollowUpReport(storeIds, fromDate, toDate).Select(x => new StockInReturnFromPurchaseInvoiceFollowUpReportVm { StockInReturnFromPurchaseInvoiceFollowUpReportDto = x });
		}
	}
}
