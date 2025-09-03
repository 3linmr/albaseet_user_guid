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
    public class ProformaInvoiceFollowUpCombinedReportService : IProformaInvoiceFollowUpCombinedReportService
    {
        private readonly IProformaInvoiceFollowUpReportService _proformaInvoiceFollowUpReportService;
        private readonly IProformaInvoiceFollowUpDetailReportService _proformaInvoiceFollowUpDetailReportService;

        public ProformaInvoiceFollowUpCombinedReportService(IProformaInvoiceFollowUpReportService proformaInvoiceFollowUpReportService, IProformaInvoiceFollowUpDetailReportService proformaInvoiceFollowUpDetailReportService)
        {
            _proformaInvoiceFollowUpReportService = proformaInvoiceFollowUpReportService;
            _proformaInvoiceFollowUpDetailReportService = proformaInvoiceFollowUpDetailReportService;
        }

        public async Task<IQueryable<ProformaInvoiceFollowUpReportVm>> GetProformaInvoiceFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            return isDetail
                ? (await _proformaInvoiceFollowUpDetailReportService.GetProformaInvoiceFollowUpDetailReport(storeIds, fromDate, toDate))
                    .Select(x => new ProformaInvoiceFollowUpReportVm { ProformaInvoiceFollowUpDetailReportDto = x })
                : _proformaInvoiceFollowUpReportService.GetProformaInvoiceFollowUpReport(storeIds, fromDate, toDate)
                    .Select(x => new ProformaInvoiceFollowUpReportVm { ProformaInvoiceFollowUpReportDto = x });
        }
    }
}