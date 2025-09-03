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
	public class PurchaseInvoiceInterimFollowUpCombinedReportService: IPurchaseInvoiceInterimFollowUpCombinedReportService
	{
		private readonly IPurchaseInvoiceInterimFollowUpReportService _purchaseInvoiceInterimFollowUpReportService;
		private readonly IPurchaseInvoiceInterimFollowUpDetailReportService _purchaseInvoiceInterimFollowUpDetailReportService;

		public PurchaseInvoiceInterimFollowUpCombinedReportService(IPurchaseInvoiceInterimFollowUpReportService purchaseInvoiceInterimFollowUpReportService, IPurchaseInvoiceInterimFollowUpDetailReportService purchaseInvoiceInterimFollowUpDetailReportService)
		{
			_purchaseInvoiceInterimFollowUpReportService = purchaseInvoiceInterimFollowUpReportService;
			_purchaseInvoiceInterimFollowUpDetailReportService = purchaseInvoiceInterimFollowUpDetailReportService;
		}

		public async Task<IQueryable<PurchaseInvoiceInterimFollowUpReportVm>> GetPurchaseInvoiceInterimFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
		{
			return isDetail ? (await _purchaseInvoiceInterimFollowUpDetailReportService.GetPurchaseInvoiceInterimFollowUpDetailReport(storeIds, fromDate, toDate)).Select(x => new PurchaseInvoiceInterimFollowUpReportVm { PurchaseInvoiceInterimFollowUpDetailReportDto = x }) :
							  _purchaseInvoiceInterimFollowUpReportService.GetPurchaseInvoiceInterimFollowUpReport(storeIds, fromDate, toDate).Select(x => new PurchaseInvoiceInterimFollowUpReportVm { PurchaseInvoiceInterimFollowUpReportDto = x });
		}
	}
}
