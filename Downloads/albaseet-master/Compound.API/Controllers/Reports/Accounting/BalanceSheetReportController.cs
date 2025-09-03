using Compound.API.Models;
using Compound.CoreOne.Contracts.Reports.Accounting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;

namespace Compound.API.Controllers.Reports.Accounting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BalanceSheetReportController : ControllerBase
    {
        private readonly IBalanceSheetReportService _balanceSheetReportService;

        public BalanceSheetReportController(IBalanceSheetReportService balanceSheetReportService)
        {
            _balanceSheetReportService = balanceSheetReportService;
        }

        [HttpGet("ReadBalanceSheetReport")]
        public async Task<IActionResult> ReadBalanceSheetReport(DataSourceLoadOptions loadOptions, int companyId, DateTime? fromDate, DateTime? toDate, int? level, int? parentSerial)
        {
            var data = await _balanceSheetReportService.GetBalanceSheetReport(companyId, fromDate, toDate, level, parentSerial);
			return Ok(DataSourceLoader.Load(data, loadOptions));
        }
    }
}