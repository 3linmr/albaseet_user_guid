using Compound.API.Models;
using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.Service.Services.Reports.Accounting;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Compound.API.Controllers.Reports.Accounting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaxesDetailReportController : ControllerBase
    {
        private readonly ITaxesDetailReportService _taxesDetailReportService;

        public TaxesDetailReportController(ITaxesDetailReportService taxesDetailReportService )
        {
            _taxesDetailReportService = taxesDetailReportService;
        }

        [HttpGet]
        [Route("ReadTaxesDetailReport")]
        public async Task<IActionResult> GetTaxesDetailReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isVatTax)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
			var data = _taxesDetailReportService.GetTaxesDetailReport(storeIdsList, fromDate, toDate, isVatTax);
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
