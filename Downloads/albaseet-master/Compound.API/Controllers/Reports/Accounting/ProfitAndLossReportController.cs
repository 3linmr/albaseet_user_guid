using Compound.API.Models;
using Compound.CoreOne.Contracts.Reports.Accounting;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Compound.API.Controllers.Reports.Accounting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfitAndLossReportController : ControllerBase
    {
        private readonly IProfitAndLossReportService _profitAndLossReportService;

        public ProfitAndLossReportController(IProfitAndLossReportService profitAndLossReportService)
        {
            _profitAndLossReportService = profitAndLossReportService;
        }


        [HttpGet("ReadProfitAndLossReport")]
        public async Task<IActionResult> GetDetailsAsync(DataSourceLoadOptions loadOptions, int companyId, DateTime? fromDate, DateTime? toDate, int? level, int? mainAccountId)
        {
            var data = await _profitAndLossReportService.GetProfitAndLossReport(companyId, fromDate, toDate, level, mainAccountId);
			return Ok(DataSourceLoader.Load(data, loadOptions));
        }
    }
}
