using Compound.CoreOne.Contracts.Reports.Accounting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sales.CoreOne.Contracts;
using DevExtreme.AspNet.Data;
using Compound.API.Models;
using System.Text.Json;
using Shared.CoreOne.Contracts.Menus;

namespace Compound.API.Controllers.Reports.Accounting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MissingDocumentCodesReportController: ControllerBase //تقرير ارقام المستندات المفقودة
    {
        private readonly IMissingDocumentCodesReportService _missingDocumentCodesReportService;
		private readonly IMenuService _menuService;

        public MissingDocumentCodesReportController(IMissingDocumentCodesReportService missingDocumentCodesReportService, IMenuService menuService)
        {
            _missingDocumentCodesReportService = missingDocumentCodesReportService;
			_menuService = menuService;
        }

		[HttpGet]
		[Route("GetMenusDropdown")]
		public async Task<IActionResult> GetMenusDropdown()
		{
			var data = await _menuService.GetMenusMissingNumberDocumentsDropDown();
			return Ok(data);
		}

        [HttpGet]
        [Route("ReadMissingDocumentNumbersReport")]
        public async Task<IActionResult> ReadMissingDocumentNumbersReport(DataSourceLoadOptions loadOptions, string? storeIds, string? menuCodes)
        {
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]")!;
			var menuCodesList = JsonSerializer.Deserialize<List<int>>(menuCodes ?? "[]")!;
            var data = await _missingDocumentCodesReportService.GetMissingDocumentCodesReport(storeIdsList, menuCodesList);
			return Ok(DataSourceLoader.Load(data, loadOptions));
        }
    }
}
