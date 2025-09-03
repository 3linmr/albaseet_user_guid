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
	public class SupplierQuotationFollowUpCombinedReportService: ISupplierQuotationFollowUpCombinedReportService
	{
		private readonly ISupplierQuotationFollowUpReportService _supplierQuotationFollowUpReportService;
		private readonly ISupplierQuotationFollowUpDetailReportService _supplierQuotationFollowUpDetailReportService;

		public SupplierQuotationFollowUpCombinedReportService(ISupplierQuotationFollowUpReportService supplierQuotationFollowUpReportService, ISupplierQuotationFollowUpDetailReportService supplierQuotationFollowUpDetailReportService)
		{
			_supplierQuotationFollowUpReportService = supplierQuotationFollowUpReportService;
			_supplierQuotationFollowUpDetailReportService = supplierQuotationFollowUpDetailReportService;
		}

		public async Task<IQueryable<SupplierQuotationFollowUpReportVm>> GetSupplierQuotationFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
		{
			return isDetail ? (await _supplierQuotationFollowUpDetailReportService.GetSupplierQuotationFollowUpDetailReport(storeIds, fromDate, toDate)).Select(x => new SupplierQuotationFollowUpReportVm { SupplierQuotationFollowUpDetailReportDto = x }) :
							  _supplierQuotationFollowUpReportService.GetSupplierQuotationFollowUpReport(storeIds, fromDate, toDate).Select(x => new SupplierQuotationFollowUpReportVm { SupplierQuotationFollowUpReportDto = x });
		}
	}
}
