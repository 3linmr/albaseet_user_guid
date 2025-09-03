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
	public class ClientPriceRequestFollowUpCombinedReportService: IClientPriceRequestFollowUpCombinedReportService
	{
		private readonly IClientPriceRequestFollowUpReportService _clientPriceRequestFollowUpReportService;
		private readonly IClientPriceRequestFollowUpDetailReportService _clientPriceRequestFollowUpDetailReportService;

		public ClientPriceRequestFollowUpCombinedReportService(IClientPriceRequestFollowUpReportService clientPriceRequestFollowUpReportService, IClientPriceRequestFollowUpDetailReportService clientPriceRequestFollowUpDetailReportService)
		{
			_clientPriceRequestFollowUpReportService = clientPriceRequestFollowUpReportService;
			_clientPriceRequestFollowUpDetailReportService = clientPriceRequestFollowUpDetailReportService;
		}

		public async Task<IQueryable<ClientPriceRequestFollowUpReportVm>> GetClientPriceRequestFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
		{
			return isDetail ? (await _clientPriceRequestFollowUpDetailReportService.GetClientPriceRequestFollowUpDetailReport(storeIds, fromDate, toDate)).Select(x => new ClientPriceRequestFollowUpReportVm { ClientPriceRequestFollowUpDetailReportDto = x }) :
							  _clientPriceRequestFollowUpReportService.GetClientPriceRequestFollowUpReport(storeIds, fromDate, toDate).Select(x => new ClientPriceRequestFollowUpReportVm { ClientPriceRequestFollowUpReportDto = x });
		}
	}
}
