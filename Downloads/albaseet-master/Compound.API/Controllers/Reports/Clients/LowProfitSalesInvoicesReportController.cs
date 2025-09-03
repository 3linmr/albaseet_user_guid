using Compound.CoreOne.Contracts.Reports.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
namespace Compound.API.Controllers.Reports.Clients
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LowProfitSalesInvoicesReportController : ControllerBase
    {
        private readonly ILowProfitSalesInvoicesReportService _lowProfitInvoiceReportService;

        public LowProfitSalesInvoicesReportController( ILowProfitSalesInvoicesReportService lowProfitInvoiceReportService)
        {
            _lowProfitInvoiceReportService = lowProfitInvoiceReportService;
        }

        [HttpGet]
        [Route("ReadLowProfitSalesInvoicesReport")]
        public async Task<IActionResult> ReadLowProfitSalesInvoicesReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, int? clientId,int? sellerId, decimal profitThreshold)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
            var data = _lowProfitInvoiceReportService.GetLowProfitInvoicesReport(storeIdsList, fromDate, toDate,clientId, sellerId, profitThreshold);
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
