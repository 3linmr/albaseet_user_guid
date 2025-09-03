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
	public class PurchaseTradingActivityReportController : Controller
	{
		private readonly IPurchaseTradingActivityReportService _purchaseTradingActivityReportService;

		public PurchaseTradingActivityReportController(IPurchaseTradingActivityReportService purchaseTradingActivityReportService)
		{
			_purchaseTradingActivityReportService = purchaseTradingActivityReportService; 
		}

		[HttpGet]
		[Route("ReadPurchaseTradingActivityReport")]
		public async Task<IActionResult> ReadPurchaseTradingActivityReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, int? supplierId)
		{
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
			var data = _purchaseTradingActivityReportService.GetPurchaseTradingActivityReport(storeIdsList, fromDate, toDate, supplierId);
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
