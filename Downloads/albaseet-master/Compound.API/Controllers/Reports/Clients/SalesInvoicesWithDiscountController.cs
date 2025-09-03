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
    public class SalesInvoicesWithDiscountController : ControllerBase
    {
        private readonly ISalesInvoicesWithDiscountReportService _salesInvoicesWithDiscountGreaterThanReportService;

        public SalesInvoicesWithDiscountController(ISalesInvoicesWithDiscountReportService salesInvoicesWithDiscountGreaterThanReportService)
        {
            _salesInvoicesWithDiscountGreaterThanReportService = salesInvoicesWithDiscountGreaterThanReportService;
        }

        [HttpGet]
        [Route("ReadSalesInvoicesWithDiscountReport")]
        public async Task<IActionResult> ReadSalesInvoicesWithDiscountReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, int? clientId, int? sellerId, decimal discountThreshold)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
            var data = _salesInvoicesWithDiscountGreaterThanReportService.GetSalesInvoicesWithDiscountGreaterThanReport(storeIdsList, fromDate, toDate, clientId, sellerId, discountThreshold);
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
