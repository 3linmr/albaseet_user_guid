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
    public class OtherTaxesTotalReportController : ControllerBase
    {
        private readonly IOtherTaxesTotalReportService _otherTaxesTotalReportService;

        public OtherTaxesTotalReportController(IOtherTaxesTotalReportService otherTaxesTotalReportService)
        {
            _otherTaxesTotalReportService = otherTaxesTotalReportService;
        }

        [HttpGet]        
        [Route("ReadOtherTaxesTotalReport")]
        public async Task<IActionResult> GetOtherTaxesTotalReport(DataSourceLoadOptions loadOptions, int companyId, DateTime? fromDate, DateTime? toDate)
        {
			var data = await _otherTaxesTotalReportService.GetOtherTaxesTotalReport(companyId, fromDate, toDate);
            return Ok(DataSourceLoader.Load(data, loadOptions));
        }
    }
}
