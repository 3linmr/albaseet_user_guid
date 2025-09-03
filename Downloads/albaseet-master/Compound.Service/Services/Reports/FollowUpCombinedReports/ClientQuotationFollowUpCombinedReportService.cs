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
	public class ClientQuotationFollowUpCombinedReportService: IClientQuotationFollowUpCombinedReportService
	{
		private readonly IClientQuotationFollowUpReportService _clientQuotationFollowUpReportService;
		private readonly IClientQuotationFollowUpDetailReportService _clientQuotationFollowUpDetailReportService;

		public ClientQuotationFollowUpCombinedReportService(IClientQuotationFollowUpReportService clientQuotationFollowUpReportService, IClientQuotationFollowUpDetailReportService clientQuotationFollowUpDetailReportService)
		{
			_clientQuotationFollowUpReportService = clientQuotationFollowUpReportService;
			_clientQuotationFollowUpDetailReportService = clientQuotationFollowUpDetailReportService;
		}

		public async Task<IQueryable<ClientQuotationFollowUpReportVm>> GetClientQuotationFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
		{
			return isDetail ? (await _clientQuotationFollowUpDetailReportService.GetClientQuotationFollowUpDetailReport(storeIds, fromDate, toDate)).Select(x => new ClientQuotationFollowUpReportVm { ClientQuotationFollowUpDetailReportDto = x }) :
							  _clientQuotationFollowUpReportService.GetClientQuotationFollowUpReport(storeIds, fromDate, toDate).Select(x => new ClientQuotationFollowUpReportVm { ClientQuotationFollowUpReportDto = x });
		}
	}
}
