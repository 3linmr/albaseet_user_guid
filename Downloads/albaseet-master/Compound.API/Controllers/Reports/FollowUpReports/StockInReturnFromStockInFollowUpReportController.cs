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
    public class StockInReturnFromStockInFollowUpReportController : ControllerBase
    {
        private readonly IStockInReturnFromStockInFollowUpReportService _stockInReturnFromStockInFollowUpReportService;
		private readonly IStockInReturnFromStockInFollowUpDetailReportService _stockInReturnFromStockInFollowUpDetailReportService;
		private readonly IStockInReturnFromStockInFollowUpCombinedReportService _stockInReturnFromStockInFollowUpCombinedReportService;

        public StockInReturnFromStockInFollowUpReportController(IStockInReturnFromStockInFollowUpReportService stockInReturnFromStockInFollowUpReportService, IStockInReturnFromStockInFollowUpDetailReportService stockInReturnFromStockInFollowUpDetailReportService, IStockInReturnFromStockInFollowUpCombinedReportService stockInReturnFromStockInFollowUpCombinedReportService)
        {
            _stockInReturnFromStockInFollowUpReportService = stockInReturnFromStockInFollowUpReportService;
			_stockInReturnFromStockInFollowUpDetailReportService = stockInReturnFromStockInFollowUpDetailReportService;
			_stockInReturnFromStockInFollowUpCombinedReportService = stockInReturnFromStockInFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadStockInReturnFromStockInFollowUpReport")]
        public async Task<IActionResult> ReadStockInReturnFromStockInFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _stockInReturnFromStockInFollowUpReportService.GetStockInReturnFromStockInFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadStockInReturnFromStockInFollowUpDetailReport")]
        public async Task<IActionResult> ReadStockInReturnFromStockInFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _stockInReturnFromStockInFollowUpDetailReportService.GetStockInReturnFromStockInFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadStockInReturnFromStockInFollowUpCombinedReport")]
        public async Task<IActionResult> ReadStockInReturnFromStockInFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _stockInReturnFromStockInFollowUpCombinedReportService.GetStockInReturnFromStockInFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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
