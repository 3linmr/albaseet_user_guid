using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.Service.Services.Reports.Accounting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
namespace Compound.API.Controllers.Reports.Accounting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountStatementReportController : ControllerBase
    {
        private readonly IAccountStatementReportService _accountStatementReportService;
        public AccountStatementReportController(IAccountStatementReportService accountStatementReportService)
        {
            _accountStatementReportService = accountStatementReportService;
        }

        [HttpGet]
        [Route("ReadAccountStatementReport")]
        public  async Task<IActionResult> ReadAccountStatementReport(DataSourceLoadOptions loadOptions, int accountId, int companyId, DateTime? fromDate, DateTime? toDate)
        {
            if (loadOptions.Sort == null || loadOptions.Sort.Length == 0)
            {
                loadOptions.Sort = new SortingInfo[]
				{
					new SortingInfo
					{
						Selector = "accountId",
						Desc = false
					},
					new SortingInfo
					{
						Selector = "ticketDate",
						Desc = false
					},
					new SortingInfo
					{
						Selector = "journalDetailId",
						Desc = false
					}
				};
            }

			var data = _accountStatementReportService.GetAccountStatementReport(accountId, companyId, fromDate, toDate);
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
