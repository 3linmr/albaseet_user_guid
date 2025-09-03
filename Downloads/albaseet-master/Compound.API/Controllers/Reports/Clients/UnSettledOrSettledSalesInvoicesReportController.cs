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

    public class UnSettledOrSettledSalesInvoicesReportController : Controller
	{
		private readonly IUnSettledOrSettledSalesInvoicesReportService _unSettledOrSettledSalesInvoicesReportService;

		public UnSettledOrSettledSalesInvoicesReportController(IUnSettledOrSettledSalesInvoicesReportService unSettledOrSettledSalesInvoiceReportService)
		{
            _unSettledOrSettledSalesInvoicesReportService = unSettledOrSettledSalesInvoiceReportService; 
		}

		[HttpGet]
		[Route("ReadUnSettledOrSettledSalesInvoicesReport")]
		public async Task<IActionResult> ReadUnSettledOrSettledSalesInvoicesReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, int? clientId, int? sellerId, bool getSettled)
		{
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
			var data = _unSettledOrSettledSalesInvoicesReportService.GetUnSettledOrSettledSalesInvoicesReport(storeIdsList, fromDate, toDate, clientId, sellerId, getSettled);
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
