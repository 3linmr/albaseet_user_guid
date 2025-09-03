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
	public class InventoryValueReportController : Controller // تقرير قيمة البضاعة
	{
		private readonly IInventoryValueReportService _inventoryValueReportService;

		public InventoryValueReportController(IInventoryValueReportService inventoryValueReportService)
		{
			_inventoryValueReportService = inventoryValueReportService; 
		}

		[HttpGet]
		[Route("ReadInventoryValueReport")]
		public async Task<IActionResult> ReadInventoryValueReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, int? itemCategoryId, int? itemSubCategoryId, int? itemSectionId, int? itemSubSectionId, int? mainItemId, DateTime? expireBefore)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
			var data = await _inventoryValueReportService.GetInventoryValueReport(storeIdsList!, fromDate, toDate, itemCategoryId, itemSubCategoryId, itemSectionId, itemSubSectionId, mainItemId, expireBefore);
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
