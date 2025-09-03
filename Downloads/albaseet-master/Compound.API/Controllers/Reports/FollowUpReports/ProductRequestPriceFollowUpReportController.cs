using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Compound.CoreOne.Contracts.Reports.FollowUpReports;
using DevExtreme.AspNet.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Compound.API.Models;
using Compound.CoreOne.Contracts.Reports.FollowUpDetailReports;
using Microsoft.EntityFrameworkCore;
using Compound.CoreOne.Contracts.Reports.FollowUpCombinedReports;

namespace Compound.API.Controllers.Reports.FollowUpReports
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductRequestPriceFollowUpReportController : ControllerBase
    {
        private readonly IProductRequestPriceFollowUpReportService _productRequestPriceFollowUpReportService;
		private readonly IProductRequestPriceFollowUpDetailReportService _productRequestPriceFollowUpDetailReportService;
		private readonly IProductRequestPriceFollowUpCombinedReportService _productRequestPriceFollowUpCombinedReportService;

        public ProductRequestPriceFollowUpReportController(IProductRequestPriceFollowUpReportService productRequestPriceFollowUpReportService, IProductRequestPriceFollowUpDetailReportService productRequestPriceFollowUpDetailReportService, IProductRequestPriceFollowUpCombinedReportService productRequestPriceFollowUpCombinedReportService)
        {
            _productRequestPriceFollowUpReportService = productRequestPriceFollowUpReportService;
			_productRequestPriceFollowUpDetailReportService = productRequestPriceFollowUpDetailReportService;
			_productRequestPriceFollowUpCombinedReportService = productRequestPriceFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadProductRequestPriceFollowUpReport")]
        public async Task<IActionResult> ReadProductRequestPriceFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _productRequestPriceFollowUpReportService.GetProductRequestPriceFollowUpReport(storeIdsList!, fromDate, toDate);

            try
            {
                return Ok(await DataSourceLoader.LoadAsync(data, loadOptions));
            }
            catch
            {
                return Ok(DataSourceLoader.Load(await data.ToListAsync(), loadOptions));
            }
        }

        [HttpGet]
        [Route("ReadProductRequestPriceFollowUpDetailReport")]
        public async Task<IActionResult> ReadProductRequestPriceFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _productRequestPriceFollowUpDetailReportService.GetProductRequestPriceFollowUpDetailReport(storeIdsList!, fromDate, toDate);

            try
            {
                return Ok(await DataSourceLoader.LoadAsync(data, loadOptions));
            }
            catch
            {
                return Ok(DataSourceLoader.Load(await data.ToListAsync(), loadOptions));
            }
        }

        [HttpGet]
        [Route("ReadProductRequestPriceFollowUpCombinedReport")]
        public async Task<IActionResult> ReadProductRequestPriceFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _productRequestPriceFollowUpCombinedReportService.GetProductRequestPriceFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

            try
            {
                return Ok(await DataSourceLoader.LoadAsync(data, loadOptions));
            }
            catch
            {
                return Ok(DataSourceLoader.Load(await data.ToListAsync(), loadOptions));
            }
        }
    }
}
