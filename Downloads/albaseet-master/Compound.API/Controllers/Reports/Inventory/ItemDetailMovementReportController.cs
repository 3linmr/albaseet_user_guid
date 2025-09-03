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
	public class ItemDetailMovementReportController : Controller //تقرير حركة تفصيلية لصنف
	{
		private readonly IItemDetailMovementReportService _itemDetailMovmentReportService;

		public ItemDetailMovementReportController(IItemDetailMovementReportService itemDetailMovmentReportService)
		{
			_itemDetailMovmentReportService = itemDetailMovmentReportService; 
		}

		[HttpGet]
		[Route("ReadItemDetailMovementReport")]
		public async Task<IActionResult> ReadItemDetailMovementReport(DataSourceLoadOptions loadOptions, int itemId, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isGrouped)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
			var data = await _itemDetailMovmentReportService.GetItemDetailMovementReport(itemId, storeIdsList!, fromDate, toDate, isGrouped);
			var result = DataSourceLoader.Load(data, loadOptions);

			return Ok(result);
		}
	}
}
