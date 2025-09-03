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
	public class StagnantItemsReportController : Controller //تقرير أصناف راكدة لم تباع منذ مدة
	{
		private readonly IStagnantItemsReportService _stagnantItemsReportService;

		public StagnantItemsReportController(IStagnantItemsReportService stagnantItemsReportService)
		{
			_stagnantItemsReportService = stagnantItemsReportService; 
		}

		[HttpGet]
		[Route("ReadStagnantItemsSinceReport")]
		public async Task<IActionResult> ReadStagnantItemsWithinReport(DataSourceLoadOptions loadOptions, string? storeIds, int daysSinceLastSold)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]");
			var data = await _stagnantItemsReportService.GetStagnantItemsReport(storeIdsList!, daysSinceLastSold);
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
