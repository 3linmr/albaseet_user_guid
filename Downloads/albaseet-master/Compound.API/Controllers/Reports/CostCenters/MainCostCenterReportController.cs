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
	public class MainCostCenterReportController : ControllerBase
	{
        private readonly IMainCostCenterReportService _mainCostCenterReportService;

        public MainCostCenterReportController(IMainCostCenterReportService mainCostCenterReportService)
        {
            _mainCostCenterReportService = mainCostCenterReportService;
        }

        [HttpGet]
        [Route("ReadMainCostCenterReport")]
        public async Task<IActionResult> ReadMainCostCenterReport(DataSourceLoadOptions loadOptions, int companyId, int? mainCostCenterId, DateTime? fromDate, DateTime? toDate, bool debitOnly, bool includeItems)
        {
            var data = await _mainCostCenterReportService.GetMainCostCenterReport(companyId, mainCostCenterId, fromDate, toDate, debitOnly, includeItems);
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
	}
}
