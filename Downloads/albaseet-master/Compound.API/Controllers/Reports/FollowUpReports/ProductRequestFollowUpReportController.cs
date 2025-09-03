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
    public class ProductRequestFollowUpReportController : ControllerBase
    {
        private readonly IProductRequestFollowUpReportService _productRequestFollowUpReportService;
		private readonly IProductRequestFollowUpDetailReportService _productRequestFollowUpDetailReportService;
		private readonly IProductRequestFollowUpCombinedReportService _productRequestFollowUpCombinedReportService;

        public ProductRequestFollowUpReportController(IProductRequestFollowUpReportService productRequestFollowUpReportService, IProductRequestFollowUpDetailReportService productRequestFollowUpDetailReportService, IProductRequestFollowUpCombinedReportService productRequestFollowUpCombinedReportService)
        {
            _productRequestFollowUpReportService = productRequestFollowUpReportService;
			_productRequestFollowUpDetailReportService = productRequestFollowUpDetailReportService;
			_productRequestFollowUpCombinedReportService = productRequestFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadProductRequestFollowUpReport")]
        public async Task<IActionResult> ReadProductRequestFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _productRequestFollowUpReportService.GetProductRequestFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadProductRequestFollowUpDetailReport")]
        public async Task<IActionResult> ReadProductRequestFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _productRequestFollowUpDetailReportService.GetProductRequestFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadProductRequestFollowUpCombinedReport")]
        public async Task<IActionResult> ReadProductRequestFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _productRequestFollowUpCombinedReportService.GetProductRequestFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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