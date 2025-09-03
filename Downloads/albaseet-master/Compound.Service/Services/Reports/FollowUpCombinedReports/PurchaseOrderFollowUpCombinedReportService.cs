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
	public class PurchaseOrderFollowUpCombinedReportService: IPurchaseOrderFollowUpCombinedReportService
	{
		private readonly IPurchaseOrderFollowUpReportService _purchaseOrderFollowUpReportService;
		private readonly IPurchaseOrderFollowUpDetailReportService _purchaseOrderFollowUpDetailReportService;

		public PurchaseOrderFollowUpCombinedReportService(IPurchaseOrderFollowUpReportService purchaseOrderFollowUpReportService, IPurchaseOrderFollowUpDetailReportService purchaseOrderFollowUpDetailReportService)
		{
			_purchaseOrderFollowUpReportService = purchaseOrderFollowUpReportService;
			_purchaseOrderFollowUpDetailReportService = purchaseOrderFollowUpDetailReportService;
		}

		public async Task<IQueryable<PurchaseOrderFollowUpReportVm>> GetPurchaseOrderFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
		{
			return isDetail ? (await _purchaseOrderFollowUpDetailReportService.GetPurchaseOrderFollowUpDetailReport(storeIds, fromDate, toDate)).Select(x => new PurchaseOrderFollowUpReportVm { PurchaseOrderFollowUpDetailReportDto = x }) :
							  _purchaseOrderFollowUpReportService.GetPurchaseOrderFollowUpReport(storeIds, fromDate, toDate).Select(x => new PurchaseOrderFollowUpReportVm { PurchaseOrderFollowUpReportDto = x });
		}
	}
}
