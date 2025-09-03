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
    public class StockOutReturnFromSalesInvoiceFollowUpCombinedReportService : IStockOutReturnFromSalesInvoiceFollowUpCombinedReportService
    {
        private readonly IStockOutReturnFromSalesInvoiceFollowUpReportService _stockOutReturnFromSalesInvoiceFollowUpReportService;
        private readonly IStockOutReturnFromSalesInvoiceFollowUpDetailReportService _stockOutReturnFromSalesInvoiceFollowUpDetailReportService;

        public StockOutReturnFromSalesInvoiceFollowUpCombinedReportService(IStockOutReturnFromSalesInvoiceFollowUpReportService stockOutReturnFromSalesInvoiceFollowUpReportService, IStockOutReturnFromSalesInvoiceFollowUpDetailReportService stockOutReturnFromSalesInvoiceFollowUpDetailReportService)
        {
            _stockOutReturnFromSalesInvoiceFollowUpReportService = stockOutReturnFromSalesInvoiceFollowUpReportService;
            _stockOutReturnFromSalesInvoiceFollowUpDetailReportService = stockOutReturnFromSalesInvoiceFollowUpDetailReportService;
        }

        public async Task<IQueryable<StockOutReturnFromSalesInvoiceFollowUpReportVm>> GetStockOutReturnFromSalesInvoiceFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            return isDetail
                ? (await _stockOutReturnFromSalesInvoiceFollowUpDetailReportService.GetStockOutReturnFromSalesInvoiceFollowUpDetailReport(storeIds, fromDate, toDate))
                    .Select(x => new StockOutReturnFromSalesInvoiceFollowUpReportVm { StockOutReturnFromSalesInvoiceFollowUpDetailReportDto = x })
                : _stockOutReturnFromSalesInvoiceFollowUpReportService.GetStockOutReturnFromSalesInvoiceFollowUpReport(storeIds, fromDate, toDate)
                    .Select(x => new StockOutReturnFromSalesInvoiceFollowUpReportVm { StockOutReturnFromSalesInvoiceFollowUpReportDto = x });
        }
    }
}