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
	public class ItemsTradingMovementPurchaseReportController : Controller //تقرير الحركة التجارية للأصناف شراء
	{
		private readonly IItemsTradingMovementReportService _itemTradingMovmentReportService;
		private readonly IMenuService _menuService;

		public ItemsTradingMovementPurchaseReportController(IItemsTradingMovementReportService itemTradingMovmentReportService, IMenuService menuService)
		{
			_itemTradingMovmentReportService = itemTradingMovmentReportService; 
			_menuService = menuService;
		}

		[HttpGet]
		[Route("GetMenusDropdown")]
		public async Task<IActionResult> GetMenusDropdown()
		{
			var data = await _menuService.GetMenusItemTradingMovementDocumentsDropDown();
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadItemsTradingMovementPurchaseReport")]
		public async Task<IActionResult> ReadItemsTradingMovementReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, bool isGrouped, string? menuCodes)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
			var menuCodesList = JsonSerializer.Deserialize<List<int>>(menuCodes ?? "[]");
			var data = await _itemTradingMovmentReportService.GetItemsTradingMovementReport(storeIdsList!, fromDate, toDate, isGrouped, menuCodesList!);
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
