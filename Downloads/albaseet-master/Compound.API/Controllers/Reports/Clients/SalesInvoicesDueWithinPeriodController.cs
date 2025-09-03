using Compound.CoreOne.Contracts.Reports.Clients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
namespace Compound.API.Controllers.Reports.Clients
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesInvoicesDueWithinPeriodController : ControllerBase
    {
        private readonly ISalesInvoicesDueWithinPeriodReportService _salesInvoicesDueWithinPeriodReportService;

        public SalesInvoicesDueWithinPeriodController(ISalesInvoicesDueWithinPeriodReportService salesInvoicesDueWithinPeriodReportService)
        {
            _salesInvoicesDueWithinPeriodReportService = salesInvoicesDueWithinPeriodReportService;
        }

        [HttpGet]
        [Route("ReadSalesInvoicesDueWithinPeriodReport")]
        public async Task<IActionResult> ReadSalesInvoicesDueWithinPeriodReport(DataSourceLoadOptions loadOptions, string? storeIds, int periodDays, int? clientId, int? sellerId)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
            var data = _salesInvoicesDueWithinPeriodReportService.GetSalesInvoicesDueWithinPeriodReport(storeIdsList, periodDays, clientId, sellerId);
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
