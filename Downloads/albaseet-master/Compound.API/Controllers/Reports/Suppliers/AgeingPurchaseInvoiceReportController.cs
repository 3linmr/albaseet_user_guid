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
	public class AgeingPurchaseInvoiceReportController : Controller
	{
		private readonly IAgeingPurchaseInvoiceReportService _ageingPurchaseInvoiceReportService;

		public AgeingPurchaseInvoiceReportController(IAgeingPurchaseInvoiceReportService ageingPurchaseInvoiceReportService)
		{
			_ageingPurchaseInvoiceReportService = ageingPurchaseInvoiceReportService; 
		}

		[HttpGet]
		[Route("ReadAgeingPurchaseInvoiceReport")]
		public async Task<IActionResult> ReadAgeingPurchaseInvoiceReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? toDate, int? supplierId)
		{
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
			var data = _ageingPurchaseInvoiceReportService.GetAgeingPurchaseInvoiceReport(storeIdsList, toDate, supplierId);
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
