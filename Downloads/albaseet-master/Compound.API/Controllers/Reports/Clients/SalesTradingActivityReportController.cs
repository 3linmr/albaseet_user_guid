using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Compound.CoreOne.Contracts.Reports.Suppliers;
using Microsoft.EntityFrameworkCore;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using Compound.CoreOne.Contracts.Reports.Clients;
using System.Text.Json;

namespace Compound.API.Controllers.Reports.Clients
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class SalesTradingActivityReportController : ControllerBase
    {

        private readonly ISalesTradingActivityReportService _salesTradingActivityReportService;

        public SalesTradingActivityReportController(ISalesTradingActivityReportService salesTradingActivityReportService)
        {
            _salesTradingActivityReportService = salesTradingActivityReportService;
        }

        [HttpGet]
        [Route("ReadSalesTradingActivityReport")]
        public async Task<IActionResult> ReadSalesTradingActivityReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, int? clientId, int? sellerId)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
            var data = _salesTradingActivityReportService.GetSalesTradingActivityReport(storeIdsList, fromDate, toDate, clientId, sellerId);
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
