using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.CoreOne.Contracts.Journal;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Accounts;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using Compound.CoreOne.Contracts.Reports.Accounting;
using System.Text.Json;
using Shared.CoreOne.Contracts.Menus;

namespace Compound.API.Controllers.Reports.Accounting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StoreIncomeReportController : ControllerBase //تقرير إجمالي الدخل للموقع
    {
        private readonly IStoreIncomeReportService _storeIncomeReportService;
		private readonly IMenuService _menuService;

        public StoreIncomeReportController(IStoreIncomeReportService storeIncomeReportService, IMenuService menuService)
        {
            _storeIncomeReportService = storeIncomeReportService;
			_menuService = menuService;
        }

		[HttpGet]
		[Route("GetMenusDropdown")]
		public async Task<IActionResult> GetMenusDropdown()
		{
			var data = await _menuService.GetMenusStoreCashIncomeDocumentsDropDown();
			return Ok(data);
		}

        [HttpGet]
        [Route("ReadStoreIncomeReport")]
        public async Task<IActionResult> ReadStoreIncomeReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, string? menuCodes)
        {
            var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
            var menuCodesList = JsonSerializer.Deserialize<List<short>?>(menuCodes ?? "[]")!;
            var data = _storeIncomeReportService.GetStoreIncomeReport(storeIdsList, fromDate, toDate, menuCodesList);
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
