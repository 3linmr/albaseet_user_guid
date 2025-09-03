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
    public class TaxesTotalReportController : ControllerBase
    {
        private readonly ITaxesTotalReportService _taxesTotalReportService;

        public TaxesTotalReportController(ITaxesTotalReportService taxesTotalReportService)
        {
            _taxesTotalReportService = taxesTotalReportService;
        }

        [HttpGet]        
        [Route("ReadTaxesTotalReport")]
        public async Task<IActionResult> GetTaxesTotalReport(DataSourceLoadOptions loadOptions, int companyId, DateTime? fromDate, DateTime? toDate)
        {
			var data = await _taxesTotalReportService.GetTaxesTotalReport(companyId, fromDate, toDate);
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
    }
}
