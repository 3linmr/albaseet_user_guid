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
	public class PopularItemsReportController : Controller //تقرير الاصناف الأكثر دوران
	{
		private readonly IPopularItemsReportService _popularItemsReportService;

		public PopularItemsReportController(IPopularItemsReportService popularItemsReportService)
		{
			_popularItemsReportService = popularItemsReportService; 
		}

		[HttpGet]
		[Route("ReadPopularItemsReport")]
		public async Task<IActionResult> ReadPopularItemsReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
			var data = await _popularItemsReportService.GetPopularItemsReport(storeIdsList!, fromDate, toDate);
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
