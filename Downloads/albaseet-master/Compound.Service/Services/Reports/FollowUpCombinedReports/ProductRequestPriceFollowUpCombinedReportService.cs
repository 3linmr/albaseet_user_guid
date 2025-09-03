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
	public class ProductRequestPriceFollowUpCombinedReportService: IProductRequestPriceFollowUpCombinedReportService
	{
		private readonly IProductRequestPriceFollowUpReportService _productRequestPriceFollowUpReportService;
		private readonly IProductRequestPriceFollowUpDetailReportService _productRequestPriceFollowUpDetailReportService;

		public ProductRequestPriceFollowUpCombinedReportService(IProductRequestPriceFollowUpReportService productRequestPriceFollowUpReportService, IProductRequestPriceFollowUpDetailReportService productRequestPriceFollowUpDetailReportService)
		{
			_productRequestPriceFollowUpReportService = productRequestPriceFollowUpReportService;
			_productRequestPriceFollowUpDetailReportService = productRequestPriceFollowUpDetailReportService;
		}

		public async Task<IQueryable<ProductRequestPriceFollowUpReportVm>> GetProductRequestPriceFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
		{
			return isDetail ? (await _productRequestPriceFollowUpDetailReportService.GetProductRequestPriceFollowUpDetailReport(storeIds, fromDate, toDate)).Select(x => new ProductRequestPriceFollowUpReportVm { ProductRequestPriceFollowUpDetailReportDto = x }) :
							  _productRequestPriceFollowUpReportService.GetProductRequestPriceFollowUpReport(storeIds, fromDate, toDate).Select(x => new ProductRequestPriceFollowUpReportVm { ProductRequestPriceFollowUpReportDto = x });
		}
	}
}
