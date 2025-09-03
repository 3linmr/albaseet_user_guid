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
    public class StockInFromPurchaseOrderFollowUpReportController : ControllerBase
    {
        private readonly IStockInFromPurchaseOrderFollowUpReportService _stockInFromPurchaseOrderFollowUpReportService;
		private readonly IStockInFromPurchaseOrderFollowUpDetailReportService _stockInFromPurchaseOrderFollowUpDetailReportService;
		private readonly IStockInFromPurchaseOrderFollowUpCombinedReportService _stockInFromPurchaseOrderFollowUpCombinedReportService;

        public StockInFromPurchaseOrderFollowUpReportController(IStockInFromPurchaseOrderFollowUpReportService stockInFromPurchaseOrderFollowUpReportService, IStockInFromPurchaseOrderFollowUpDetailReportService stockInFromPurchaseOrderFollowUpDetailReportService, IStockInFromPurchaseOrderFollowUpCombinedReportService stockInFromPurchaseOrderFollowUpCombinedReportService)
        {
            _stockInFromPurchaseOrderFollowUpReportService = stockInFromPurchaseOrderFollowUpReportService;
			_stockInFromPurchaseOrderFollowUpDetailReportService = stockInFromPurchaseOrderFollowUpDetailReportService;
			_stockInFromPurchaseOrderFollowUpCombinedReportService = stockInFromPurchaseOrderFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadStockInFromPurchaseOrderFollowUpReport")]
        public async Task<IActionResult> ReadStockInFromPurchaseOrderFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _stockInFromPurchaseOrderFollowUpReportService.GetStockInFromPurchaseOrderFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadStockInFromPurchaseOrderFollowUpDetailReport")]
        public async Task<IActionResult> ReadStockInFromPurchaseOrderFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _stockInFromPurchaseOrderFollowUpDetailReportService.GetStockInFromPurchaseOrderFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadStockInFromPurchaseOrderFollowUpCombinedReport")]
        public async Task<IActionResult> ReadStockInFromPurchaseOrderFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _stockInFromPurchaseOrderFollowUpCombinedReportService.GetStockInFromPurchaseOrderFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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
