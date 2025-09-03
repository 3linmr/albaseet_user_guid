using Compound.CoreOne.Contracts.Reports.FollowUpCombinedReports;
using Compound.CoreOne.Contracts.Reports.FollowUpDetailReports;
using Compound.CoreOne.Contracts.Reports.FollowUpReports;
using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compound.Service.Services.Reports.FollowUpCombinedReports
{
    public class StockOutReturnFromStockOutFollowUpCombinedReportService : IStockOutReturnFromStockOutFollowUpCombinedReportService
    {
        private readonly IStockOutReturnFromStockOutFollowUpReportService _stockOutReturnFromStockOutFollowUpReportService;
        private readonly IStockOutReturnFromStockOutFollowUpDetailReportService _stockOutReturnFromStockOutFollowUpDetailReportService;

        public StockOutReturnFromStockOutFollowUpCombinedReportService(IStockOutReturnFromStockOutFollowUpReportService stockOutReturnFromStockOutFollowUpReportService, IStockOutReturnFromStockOutFollowUpDetailReportService stockOutReturnFromStockOutFollowUpDetailReportService)
        {
            _stockOutReturnFromStockOutFollowUpReportService = stockOutReturnFromStockOutFollowUpReportService;
            _stockOutReturnFromStockOutFollowUpDetailReportService = stockOutReturnFromStockOutFollowUpDetailReportService;
        }

        public async Task<IQueryable<StockOutReturnFromStockOutFollowUpReportVm>> GetStockOutReturnFromStockOutFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            return isDetail
                ? (await _stockOutReturnFromStockOutFollowUpDetailReportService.GetStockOutReturnFromStockOutFollowUpDetailReport(storeIds, fromDate, toDate))
                    .Select(x => new StockOutReturnFromStockOutFollowUpReportVm { StockOutReturnFromStockOutFollowUpDetailReportDto = x })
                : _stockOutReturnFromStockOutFollowUpReportService.GetStockOutReturnFromStockOutFollowUpReport(storeIds, fromDate, toDate)
                    .Select(x => new StockOutReturnFromStockOutFollowUpReportVm { StockOutReturnFromStockOutFollowUpReportDto = x });
        }
    }
}