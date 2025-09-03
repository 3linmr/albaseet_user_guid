using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Compound.CoreOne.Contracts.Reports.Inventory;
using Microsoft.EntityFrameworkCore;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using System.Text.Json;
using Shared.CoreOne.Contracts.Menus;

namespace Compound.API.Controllers.Reports.Inventory
{
	[Route("api/[controller]")]
	[Authorize]
	[ApiController]
	public class ItemsMovementReportController : Controller //تقرير حركة الأصناف
	{
		private readonly IItemsMovementReportService _itemsMovementReportService;
		private readonly IMenuService _menuService;

		public ItemsMovementReportController(IItemsMovementReportService itemsMovementReportService, IMenuService menuService)
		{
			_itemsMovementReportService = itemsMovementReportService; 
			_menuService = menuService;
		}

		[HttpGet]
		[Route("GetMenusDropdown")]
		public async Task<IActionResult> GetMenusDropdown()
		{
			var data = await _menuService.GetMenusInventoryDocumentsDropDown();
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadItemsMovementReport")]
		public async Task<IActionResult> ReadItemsMovementReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, string? menuCodes)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
            var menuCodesList = JsonSerializer.Deserialize<List<int>>(menuCodes ?? "[]")!;
			var data = await _itemsMovementReportService.GetItemsMovementReport(storeIdsList!, fromDate, toDate, menuCodesList);
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
