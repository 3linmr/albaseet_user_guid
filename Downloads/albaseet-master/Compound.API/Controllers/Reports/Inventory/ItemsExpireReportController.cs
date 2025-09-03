using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Compound.CoreOne.Contracts.Reports.Inventory;
using Microsoft.EntityFrameworkCore;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using Shared.Helper.Logic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Compound.API.Controllers.Reports.Inventory
{
	[Route("api/[controller]")]
	[Authorize]
	[ApiController]
	public class ItemsExpireReportController : Controller //تقرير الأصناف منتهية الصلاحية
	{
		private readonly IItemsExpireReportService _itemsExpireReportService;

		public ItemsExpireReportController(IItemsExpireReportService itemsExpireReportService)
		{
			_itemsExpireReportService = itemsExpireReportService; 
		}

		[HttpGet]
		[Route("ReadItemsExpireReport")]
		public async Task<IActionResult> ReadItemsExpireReport(DataSourceLoadOptions loadOptions, string? storeIds)
		{
			return await ReadItemsExpireWithinReport(loadOptions, storeIds, 0);
		}

		[HttpGet]
		[Route("ReadItemsExpireWithinReport")]
		public async Task<IActionResult> ReadItemsExpireWithinReport(DataSourceLoadOptions loadOptions, string? storeIds, [Required]int expireWithin)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
			var expireBefore = DateHelper.GetDateTimeNow().Date.AddDays(expireWithin);
			var data = await _itemsExpireReportService.GetItemsExpireReport(storeIdsList!, expireBefore);
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
