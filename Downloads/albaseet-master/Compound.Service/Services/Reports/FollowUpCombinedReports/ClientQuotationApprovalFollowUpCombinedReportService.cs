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
    public class ClientQuotationApprovalFollowUpCombinedReportService : IClientQuotationApprovalFollowUpCombinedReportService
    {
        private readonly IClientQuotationApprovalFollowUpReportService _clientQuotationApprovalFollowUpReportService;
        private readonly IClientQuotationApprovalFollowUpDetailReportService _clientQuotationApprovalFollowUpDetailReportService;

        public ClientQuotationApprovalFollowUpCombinedReportService(IClientQuotationApprovalFollowUpReportService clientQuotationApprovalFollowUpReportService, IClientQuotationApprovalFollowUpDetailReportService clientQuotationApprovalFollowUpDetailReportService)
        {
            _clientQuotationApprovalFollowUpReportService = clientQuotationApprovalFollowUpReportService;
            _clientQuotationApprovalFollowUpDetailReportService = clientQuotationApprovalFollowUpDetailReportService;
        }

        public async Task<IQueryable<ClientQuotationApprovalFollowUpReportVm>> GetClientQuotationApprovalFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            return isDetail
                ? (await _clientQuotationApprovalFollowUpDetailReportService.GetClientQuotationApprovalFollowUpDetailReport(storeIds, fromDate, toDate))
                    .Select(x => new ClientQuotationApprovalFollowUpReportVm { ClientQuotationApprovalFollowUpDetailReportDto = x })
                : _clientQuotationApprovalFollowUpReportService.GetClientQuotationApprovalFollowUpReport(storeIds, fromDate, toDate)
                    .Select(x => new ClientQuotationApprovalFollowUpReportVm { ClientQuotationApprovalFollowUpReportDto = x });
        }
    }
}