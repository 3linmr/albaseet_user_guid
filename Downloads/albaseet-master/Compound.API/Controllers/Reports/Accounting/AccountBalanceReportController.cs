using Compound.API.Models;
using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.Service.Services.Reports.Accounting;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.Domain.Accounts;

namespace Compound.API.Controllers.Reports.Accounting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountBalanceReportController : ControllerBase
    {
        private readonly IAccountBalanceReportService _accountBalanceReportService;

        public AccountBalanceReportController(IAccountBalanceReportService accountBalanceReportService)
        {
            _accountBalanceReportService = accountBalanceReportService;
        }

		/// <summary>
		/// Retrieves the account balance report for the specified company and date range.
		/// </summary>
		/// <param name="loadOptions">Options for data shaping and paging.</param>
		/// <param name="companyId">The unique identifier of the company.</param>
		/// <param name="fromDate">The start date for the report (optional).</param>
		/// <param name="toDate">The end date for the report (optional).</param>
		/// <param name="mainAccountId">
		/// Filters the report by main account:
		/// - If <c>null</c>, returns only the top-level accounts.
		/// - If <c>0</c>, returns all accounts regardless of their level.
		/// - Otherwise, returns accounts under the specified main account.
		/// </param>
		/// <returns>The account balance report data.</returns>
		[HttpGet]
		[Route("ReadAccountBalanceReport")]
		public async Task<IActionResult> ReadAccountBalanceReportAsync(DataSourceLoadOptions loadOptions, int companyId, DateTime? fromDate, DateTime? toDate, int? mainAccountId)
		{
			var data = await _accountBalanceReportService.GetAccountBalanceReport(companyId, fromDate, toDate, mainAccountId);
			return Ok(DataSourceLoader.Load(data, loadOptions));
		}
    }
}
