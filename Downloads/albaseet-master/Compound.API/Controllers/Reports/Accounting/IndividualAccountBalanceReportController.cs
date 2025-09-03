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
    public class IndividualAccountBalanceReportController : ControllerBase
    {
        private readonly IAccountBalanceReportService _accountBalanceReportService;

        public IndividualAccountBalanceReportController(IAccountBalanceReportService accountBalanceReportService)
        {
            _accountBalanceReportService = accountBalanceReportService;
        }

		[HttpGet]
		[Route("ReadIndividualAccountBalanceReport")]
		public async Task<IActionResult> ReadIndividualAccountBalanceReportAsync(DataSourceLoadOptions loadOptions, int companyId, DateTime? fromDate, DateTime? toDate)
		{
			var data = _accountBalanceReportService.GetAccountDataQueryable(companyId, fromDate, toDate, true);
			return Ok(await DataSourceLoader.LoadAsync(data, loadOptions));
		}
    }
}
