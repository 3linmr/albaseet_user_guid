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
    public class CostCenterAccountsReportController : ControllerBase // ﬁ—Ì— Õ”«»«  „—«ﬂ“ «· ﬂ·›…
    {
        private readonly ICostCenterAccountsReportService _costCenterAccountsReportService;

        public CostCenterAccountsReportController(ICostCenterAccountsReportService costCenterAccountsReportService)
        {
            _costCenterAccountsReportService = costCenterAccountsReportService;
        }

        [HttpGet]
        [Route("ReadCostCenterAccountsReport")]
        public async Task<IActionResult> ReadCostCenterAccountsReport(DataSourceLoadOptions loadOptions, int? costCenterId, int companyId, DateTime? fromDate, DateTime? toDate, bool debitOnly, bool includeItems)
        {
            var data = _costCenterAccountsReportService.GetCostCenterAccountsReport(costCenterId, companyId, fromDate, toDate, debitOnly, includeItems);
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
