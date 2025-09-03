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

    public class SalesInvoiceSettlementsReportController : Controller
	{
		private readonly ISalesInvoiceSettlementsReportService _SalesInvoiceSettlementsReportService;

		public SalesInvoiceSettlementsReportController(ISalesInvoiceSettlementsReportService SalesInvoiceSettlementsReportService)
		{
            _SalesInvoiceSettlementsReportService = SalesInvoiceSettlementsReportService; 
		}

		[HttpGet]
		[Route("ReadSalesInvoicesSettlementsReport")]
		public async Task<IActionResult> ReadSalesInvoicesSettlementsReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, int? clientId, int? sellerId)
		{
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
			var data = _SalesInvoiceSettlementsReportService.GetSalesInvoiceSettlementsReport(storeIdsList, fromDate, toDate, clientId, sellerId);
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
