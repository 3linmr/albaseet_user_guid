using Compound.CoreOne.Contracts.Reports.Accounting;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Compound.API.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
namespace Compound.API.Controllers.Reports.Accounting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GeneralAccountStatementReportController : ControllerBase
    {
        private readonly IAccountStatementReportService _accountStatementReportService;
        public GeneralAccountStatementReportController(IAccountStatementReportService accountStatementReportService)
        {
            _accountStatementReportService = accountStatementReportService;
        }

        [HttpGet]
        [Route("ReadGeneralAccountStatementReport")]
        public async Task<IActionResult> ReadGeneralAccountStatementReport(DataSourceLoadOptions loadOptions, string accountIds, int companyId, DateTime? fromDate, DateTime? toDate)
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

			var accountIdsList = JsonSerializer.Deserialize<List<int>?>(accountIds ?? "[]")!;
			var data = _accountStatementReportService.GetGeneralAccountsStatementReport(accountIdsList, companyId, fromDate, toDate);
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
