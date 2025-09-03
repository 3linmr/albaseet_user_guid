using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Compound.CoreOne.Contracts.Reports.Inventory;
using Microsoft.EntityFrameworkCore;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using System.Text.Json;

namespace Compound.API.Controllers.Reports.Inventory
{
	[Route("api/[controller]")]
	[Authorize]
	[ApiController]
	public class ItemsProfitReportController : Controller //تقرير ارباح الاصناف
	{
		private readonly IItemsProfitReportService _itemsProfitReportService;
		private readonly IItemReportDataService _itemReportDataService;

		public ItemsProfitReportController(IItemsProfitReportService itemsProfitReportService, IItemReportDataService itemReportDataService)
		{
			_itemsProfitReportService = itemsProfitReportService; 
			_itemReportDataService = itemReportDataService;
		}

		[HttpGet]
		[Route("ReadItemsProfitReport")]
		public async Task<IActionResult> ReadItemsProfitReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
			var data = await _itemsProfitReportService.GetItemsProfitReport(storeIdsList, fromDate, toDate);
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
