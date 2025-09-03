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
    public class StockOutReturnFromStockOutFollowUpReportController : ControllerBase
    {
        private readonly IStockOutReturnFromStockOutFollowUpReportService _stockOutReturnFromStockOutFollowUpReportService;
        private readonly IStockOutReturnFromStockOutFollowUpDetailReportService _stockOutReturnFromStockOutFollowUpDetailReportService;
        private readonly IStockOutReturnFromStockOutFollowUpCombinedReportService _stockOutReturnFromStockOutFollowUpCombinedReportService;

        public StockOutReturnFromStockOutFollowUpReportController(IStockOutReturnFromStockOutFollowUpReportService stockOutReturnFromStockOutFollowUpReportService, IStockOutReturnFromStockOutFollowUpDetailReportService stockOutReturnFromStockOutFollowUpDetailReportService, IStockOutReturnFromStockOutFollowUpCombinedReportService stockOutReturnFromStockOutFollowUpCombinedReportService)
        {
            _stockOutReturnFromStockOutFollowUpReportService = stockOutReturnFromStockOutFollowUpReportService;
            _stockOutReturnFromStockOutFollowUpDetailReportService = stockOutReturnFromStockOutFollowUpDetailReportService;
            _stockOutReturnFromStockOutFollowUpCombinedReportService = stockOutReturnFromStockOutFollowUpCombinedReportService;
        }

        [HttpGet]
        [Route("ReadStockOutReturnFromStockOutFollowUpReport")]
        public async Task<IActionResult> ReadStockOutReturnFromStockOutFollowUpReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = _stockOutReturnFromStockOutFollowUpReportService.GetStockOutReturnFromStockOutFollowUpReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadStockOutReturnFromStockOutFollowUpDetailReport")]
        public async Task<IActionResult> ReadStockOutReturnFromStockOutFollowUpDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _stockOutReturnFromStockOutFollowUpDetailReportService.GetStockOutReturnFromStockOutFollowUpDetailReport(storeIdsList!, fromDate, toDate);

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
        [Route("ReadStockOutReturnFromStockOutFollowUpCombinedReport")]
        public async Task<IActionResult> ReadStockOutReturnFromStockOutFollowUpCombinedReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var data = await _stockOutReturnFromStockOutFollowUpCombinedReportService.GetStockOutReturnFromStockOutFollowUpCombinedReport(storeIdsList!, fromDate, toDate, isDetail);

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
