using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Compound.CoreOne.Contracts.Reports.CostCenters;
using Compound.CoreOne.Models.Dtos.Reports.CostCenters;
using Compound.API.Models;
using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;

namespace Compound.API.Controllers.Reports.CostCenters
{
	[Route("api/[controller]")]
	[Authorize]
	[ApiController]
	public class IndividualCostCenterReportController : ControllerBase
	{
        private readonly IIndividualCostCenterReportService _individualCostCenterReportService;

        public IndividualCostCenterReportController(IIndividualCostCenterReportService individualCostCenterReportService)
        {
            _individualCostCenterReportService = individualCostCenterReportService;
        }

        [HttpGet]
        [Route("ReadIndividualCostCenterReport")]
        public async Task<IActionResult> ReadIndividualCostCenterReport(DataSourceLoadOptions loadOptions, int companyId, DateTime? fromDate, DateTime? toDate, bool debitOnly, bool includeItems)
        {
            var data = _individualCostCenterReportService.GetIndividualCostCenterReport(companyId, fromDate, toDate, debitOnly, includeItems);
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
