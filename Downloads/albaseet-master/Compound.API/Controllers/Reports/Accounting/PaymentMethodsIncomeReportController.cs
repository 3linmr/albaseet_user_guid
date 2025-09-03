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
	public class PaymentMethodsIncomeReportController : ControllerBase //تقرير طرق التحصيلات النقدية
	{
		private readonly IPaymentMethodsIncomeReportService _paymentMethodsIncomeReportService;
		private readonly IMenuService _menuService;

		public PaymentMethodsIncomeReportController(IPaymentMethodsIncomeReportService paymentMethodsIncomeReportService, IMenuService menuService)
		{
			_paymentMethodsIncomeReportService = paymentMethodsIncomeReportService;
			_menuService = menuService;
		}

		[HttpGet]
		[Route("GetMenusDropdown")]
		public async Task<IActionResult> GetMenusDropdown()
		{
			var data = await _menuService.GetMenusPaymentMethodsIncomeDocumentsDropDown();
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadPaymentMethodsIncomeReport")]
		public async Task<IActionResult> ReadPaymentMethodsIncomeReport(DataSourceLoadOptions loadOptions, string? storeIds, DateTime? fromDate, DateTime? toDate, string? menuCodes)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>?>(storeIds ?? "[]")!;
			var menuCodesList = JsonSerializer.Deserialize<List<short>?>(menuCodes ?? "[]")!;
			var data = await _paymentMethodsIncomeReportService.GetPaymentMethodsIncomeReport(storeIdsList, fromDate, toDate, menuCodesList);
			return Ok(DataSourceLoader.Load(data, loadOptions));
		}
	}
}
