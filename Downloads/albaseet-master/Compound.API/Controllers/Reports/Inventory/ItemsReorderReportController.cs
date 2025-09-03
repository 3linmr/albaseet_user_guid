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
	public class ItemsReorderReportController : Controller //تقرير الاصناف التي وصلت حد الطلب
	{
		private readonly IItemsReorderReportService _itemsReorderReportService;

		public ItemsReorderReportController(IItemsReorderReportService itemReorderReportService)
		{
			_itemsReorderReportService = itemReorderReportService; 
		}

		[HttpGet]
		[Route("ReadItemsReorderReport")]
		public async Task<IActionResult> ReadItemsReorderReport(DataSourceLoadOptions loadOptions, string? storeIds)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
			var data = await _itemsReorderReportService.GetItemsReorderReport(storeIdsList);
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
