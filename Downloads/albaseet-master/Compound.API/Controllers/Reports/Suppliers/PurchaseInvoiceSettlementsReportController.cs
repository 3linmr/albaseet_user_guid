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
	public class PurchaseInvoiceSettlementsReportController : Controller
	{
		private readonly IPurchaseInvoiceSettlementsReportService _purchaseInvoiceSettlementsReportService;

		public PurchaseInvoiceSettlementsReportController(IPurchaseInvoiceSettlementsReportService purchaseInvoiceSettlementsReportService)
		{
			_purchaseInvoiceSettlementsReportService = purchaseInvoiceSettlementsReportService; 
		}

		[HttpGet]
		[Route("ReadPurchaseInvoicesSettlementsReport")]
		public async Task<IActionResult> ReadPurchaseInvoicesSettlementsReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, int? supplierId, int? sellerId)
		{
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
			var data = _purchaseInvoiceSettlementsReportService.GetPurchaseInvoiceSettlementsReport(storeIdsList, fromDate, toDate, supplierId, sellerId);
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
