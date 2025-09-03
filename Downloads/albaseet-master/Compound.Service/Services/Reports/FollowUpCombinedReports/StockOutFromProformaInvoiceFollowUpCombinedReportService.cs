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
    public class StockOutFromProformaInvoiceFollowUpCombinedReportService : IStockOutFromProformaInvoiceFollowUpCombinedReportService
    {
        private readonly IStockOutFromProformaInvoiceFollowUpReportService _stockOutFromProformaInvoiceFollowUpReportService;
        private readonly IStockOutFromProformaInvoiceFollowUpDetailReportService _stockOutFromProformaInvoiceFollowUpDetailReportService;

        public StockOutFromProformaInvoiceFollowUpCombinedReportService(IStockOutFromProformaInvoiceFollowUpReportService stockOutFromProformaInvoiceFollowUpReportService, IStockOutFromProformaInvoiceFollowUpDetailReportService stockOutFromProformaInvoiceFollowUpDetailReportService)
        {
            _stockOutFromProformaInvoiceFollowUpReportService = stockOutFromProformaInvoiceFollowUpReportService;
            _stockOutFromProformaInvoiceFollowUpDetailReportService = stockOutFromProformaInvoiceFollowUpDetailReportService;
        }

        public async Task<IQueryable<StockOutFromProformaInvoiceFollowUpReportVm>> GetStockOutFromProformaInvoiceFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            return isDetail
                ? (await _stockOutFromProformaInvoiceFollowUpDetailReportService.GetStockOutFromProformaInvoiceFollowUpDetailReport(storeIds, fromDate, toDate))
                    .Select(x => new StockOutFromProformaInvoiceFollowUpReportVm { StockOutFromProformaInvoiceFollowUpDetailReportDto = x })
                : _stockOutFromProformaInvoiceFollowUpReportService.GetStockOutFromProformaInvoiceFollowUpReport(storeIds, fromDate, toDate)
                    .Select(x => new StockOutFromProformaInvoiceFollowUpReportVm { StockOutFromProformaInvoiceFollowUpReportDto = x });
        }
    }
}