using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Compound.CoreOne.Contracts.Reports.CostCenters;
using DevExtreme.AspNet.Data;
using Compound.API.Models;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Compound.API.Controllers.Reports.CostCenters
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CostCenterJournalReportController : ControllerBase // ﬁ—Ì— œ› — ﬁÌÊœ ÌÊ„Ì… ·„—«ﬂ“  ﬂ·›…
    {
        private readonly ICostCenterJournalReportService _costCenterJournalReportService;

        public CostCenterJournalReportController(ICostCenterJournalReportService costCenterJournalReportService)
        {
            _costCenterJournalReportService = costCenterJournalReportService;
        }

        [HttpGet]
        [Route("ReadCostCenterJournalReport")]
        public async Task<IActionResult> ReadCostCenterJournalReport(DataSourceLoadOptions loadOptions, int? costCenterId, int companyId, DateTime? fromDate, DateTime? toDate, bool debitOnly, bool includeItems)
        {
            var data = _costCenterJournalReportService.GetCostCenterJournalReport(costCenterId, companyId, fromDate, toDate, debitOnly, includeItems);
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
