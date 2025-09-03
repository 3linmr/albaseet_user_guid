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

    public class AgeingSalesInvoiceReportController : Controller
    {
        private readonly IAgeingSalesInvoiceReportService _ageingSalesInvoiceReportService;

        public AgeingSalesInvoiceReportController(IAgeingSalesInvoiceReportService ageingSalesInvoiceReportService)
        {
            _ageingSalesInvoiceReportService = ageingSalesInvoiceReportService; 
        }

        [HttpGet]
        [Route("ReadAgeingSalesInvoiceReport")]
        public async Task<IActionResult> ReadAgeingSalesInvoiceReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? toDate, int? clientId, int? sellerId)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
            var data = _ageingSalesInvoiceReportService.GetAgeingSalesInvoiceReport(storeIdsList, toDate, clientId, sellerId);
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
