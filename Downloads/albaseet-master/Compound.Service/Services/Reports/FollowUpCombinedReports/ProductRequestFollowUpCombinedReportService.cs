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
	public class ProductRequestFollowUpCombinedReportService: IProductRequestFollowUpCombinedReportService
	{
		private readonly IProductRequestFollowUpReportService _productRequestFollowUpReportService;
		private readonly IProductRequestFollowUpDetailReportService _productRequestFollowUpDetailReportService;

		public ProductRequestFollowUpCombinedReportService(IProductRequestFollowUpReportService productRequestFollowUpReportService, IProductRequestFollowUpDetailReportService productRequestFollowUpDetailReportService)
		{
			_productRequestFollowUpReportService = productRequestFollowUpReportService;
			_productRequestFollowUpDetailReportService = productRequestFollowUpDetailReportService;
		}

		public async Task<IQueryable<ProductRequestFollowUpReportVm>> GetProductRequestFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
		{
			return isDetail ? (await _productRequestFollowUpDetailReportService.GetProductRequestFollowUpDetailReport(storeIds, fromDate, toDate)).Select(x => new ProductRequestFollowUpReportVm { ProductRequestFollowUpDetailReportDto = x }) :
							  _productRequestFollowUpReportService.GetProductRequestFollowUpReport(storeIds, fromDate, toDate).Select(x => new ProductRequestFollowUpReportVm { ProductRequestFollowUpReportDto = x });
		}
	}
}
