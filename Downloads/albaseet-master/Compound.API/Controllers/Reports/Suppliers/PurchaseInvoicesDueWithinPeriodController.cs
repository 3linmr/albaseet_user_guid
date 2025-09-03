using Compound.CoreOne.Contracts.Reports.Suppliers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
namespace Compound.API.Controllers.Reports.Suppliers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseInvoicesDueWithinPeriodController : ControllerBase
    {
        private readonly IPurchaseInvoicesDueWithinPeriodReportService _purchaseInvoicesDueWithinPeriodReportService;

        public PurchaseInvoicesDueWithinPeriodController(IPurchaseInvoicesDueWithinPeriodReportService purchaseInvoicesDueWithinPeriodReportService)
        {
            _purchaseInvoicesDueWithinPeriodReportService = purchaseInvoicesDueWithinPeriodReportService;
        }

        [HttpGet]
        [Route("ReadPurchaseInvoicesDueWithinPeriodReport")]
        public async Task<IActionResult> ReadPurchaseInvoicesDueWithinPeriodReport(DataSourceLoadOptions loadOptions, string? storeIds, int periodDays, int? supplierId)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
            var data = _purchaseInvoicesDueWithinPeriodReportService.GetPurchaseInvoicesDueWithinPeriodReport(storeIdsList, periodDays, supplierId);
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
