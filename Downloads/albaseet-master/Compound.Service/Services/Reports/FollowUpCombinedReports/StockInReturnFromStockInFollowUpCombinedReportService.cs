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
	public class StockInReturnFromStockInFollowUpCombinedReportService: IStockInReturnFromStockInFollowUpCombinedReportService
	{
		private readonly IStockInReturnFromStockInFollowUpReportService _stockInReturnFromStockInFollowUpReportService;
		private readonly IStockInReturnFromStockInFollowUpDetailReportService _stockInReturnFromStockInFollowUpDetailReportService;

		public StockInReturnFromStockInFollowUpCombinedReportService(IStockInReturnFromStockInFollowUpReportService stockInReturnFromStockInFollowUpReportService, IStockInReturnFromStockInFollowUpDetailReportService stockInReturnFromStockInFollowUpDetailReportService)
		{
			_stockInReturnFromStockInFollowUpReportService = stockInReturnFromStockInFollowUpReportService;
			_stockInReturnFromStockInFollowUpDetailReportService = stockInReturnFromStockInFollowUpDetailReportService;
		}

		public async Task<IQueryable<StockInReturnFromStockInFollowUpReportVm>> GetStockInReturnFromStockInFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
		{
			return isDetail ? (await _stockInReturnFromStockInFollowUpDetailReportService.GetStockInReturnFromStockInFollowUpDetailReport(storeIds, fromDate, toDate)).Select(x => new StockInReturnFromStockInFollowUpReportVm { StockInReturnFromStockInFollowUpDetailReportDto = x }) :
							  _stockInReturnFromStockInFollowUpReportService.GetStockInReturnFromStockInFollowUpReport(storeIds, fromDate, toDate).Select(x => new StockInReturnFromStockInFollowUpReportVm { StockInReturnFromStockInFollowUpReportDto = x });
		}
	}
}
