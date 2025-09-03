using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Compound.CoreOne.Contracts.Reports.Suppliers;
using Microsoft.EntityFrameworkCore;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using System.Text.Json;

namespace Compound.API.Controllers.Reports.Suppliers
{
	[Route("api/[controller]")]
	[Authorize]
	[ApiController]
	public class UnSettledOrSettledPurchaseInvoicesReportController : Controller
	{
		private readonly IUnSettledOrSettledPurchaseInvoiceReportService _unSettledOrSettledPurchaseInvoicesReportService;

		public UnSettledOrSettledPurchaseInvoicesReportController(IUnSettledOrSettledPurchaseInvoiceReportService unSettledOrSettledPurchaseInvoiceReportService)
		{
			_unSettledOrSettledPurchaseInvoicesReportService = unSettledOrSettledPurchaseInvoiceReportService; 
		}

		[HttpGet]
		[Route("ReadUnSettledOrSettledPurchaseInvoicesReport")]
		public async Task<IActionResult> ReadUnSettledOrSettledPurchaseInvoicesReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, int? supplierId, bool getSettled)
		{
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
			var data = _unSettledOrSettledPurchaseInvoicesReportService.GetUnSettledOrSettledPurchaseInvoicesReport(storeIdsList, fromDate, toDate, supplierId, getSettled);
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
