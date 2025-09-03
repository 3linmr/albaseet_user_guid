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
	public class PurchaseInvoiceOnTheWayFollowUpCombinedReportService: IPurchaseInvoiceOnTheWayFollowUpCombinedReportService
	{
		private readonly IPurchaseInvoiceOnTheWayFollowUpReportService _purchaseInvoiceOnTheWayFollowUpReportService;
		private readonly IPurchaseInvoiceOnTheWayFollowUpDetailReportService _purchaseInvoiceOnTheWayFollowUpDetailReportService;

		public PurchaseInvoiceOnTheWayFollowUpCombinedReportService(IPurchaseInvoiceOnTheWayFollowUpReportService purchaseInvoiceOnTheWayFollowUpReportService, IPurchaseInvoiceOnTheWayFollowUpDetailReportService purchaseInvoiceOnTheWayFollowUpDetailReportService)
		{
			_purchaseInvoiceOnTheWayFollowUpReportService = purchaseInvoiceOnTheWayFollowUpReportService;
			_purchaseInvoiceOnTheWayFollowUpDetailReportService = purchaseInvoiceOnTheWayFollowUpDetailReportService;
		}

		public async Task<IQueryable<PurchaseInvoiceOnTheWayFollowUpReportVm>> GetPurchaseInvoiceOnTheWayFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
		{
			return isDetail ? (await _purchaseInvoiceOnTheWayFollowUpDetailReportService.GetPurchaseInvoiceOnTheWayFollowUpDetailReport(storeIds, fromDate, toDate)).Select(x => new PurchaseInvoiceOnTheWayFollowUpReportVm { PurchaseInvoiceOnTheWayFollowUpDetailReportDto = x }) :
							  _purchaseInvoiceOnTheWayFollowUpReportService.GetPurchaseInvoiceOnTheWayFollowUpReport(storeIds, fromDate, toDate).Select(x => new PurchaseInvoiceOnTheWayFollowUpReportVm { PurchaseInvoiceOnTheWayFollowUpReportDto = x });
		}
	}
}
