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
    public class SalesInvoiceInterimFollowUpCombinedReportService : ISalesInvoiceInterimFollowUpCombinedReportService
    {
        private readonly ISalesInvoiceInterimFollowUpReportService _salesInvoiceInterimFollowUpReportService;
        private readonly ISalesInvoiceInterimFollowUpDetailReportService _salesInvoiceInterimFollowUpDetailReportService;

        public SalesInvoiceInterimFollowUpCombinedReportService(ISalesInvoiceInterimFollowUpReportService salesInvoiceInterimFollowUpReportService, ISalesInvoiceInterimFollowUpDetailReportService salesInvoiceInterimFollowUpDetailReportService)
        {
            _salesInvoiceInterimFollowUpReportService = salesInvoiceInterimFollowUpReportService;
            _salesInvoiceInterimFollowUpDetailReportService = salesInvoiceInterimFollowUpDetailReportService;
        }

        public async Task<IQueryable<SalesInvoiceInterimFollowUpReportVm>> GetSalesInvoiceInterimFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            return isDetail
                ? (await _salesInvoiceInterimFollowUpDetailReportService.GetSalesInvoiceInterimFollowUpDetailReport(storeIds, fromDate, toDate))
                    .Select(x => new SalesInvoiceInterimFollowUpReportVm { SalesInvoiceInterimFollowUpDetailReportDto = x })
                : _salesInvoiceInterimFollowUpReportService.GetSalesInvoiceInterimFollowUpReport(storeIds, fromDate, toDate)
                    .Select(x => new SalesInvoiceInterimFollowUpReportVm { SalesInvoiceInterimFollowUpReportDto = x });
        }
    }
}