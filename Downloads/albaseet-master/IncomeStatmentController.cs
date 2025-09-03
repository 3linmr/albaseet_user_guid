using Compound.CoreOne.Contracts.Reports.Accounting;
using Microsoft.AspNetCore.Mvc;

namespace Compound.API.Controllers.Reports.Accounting
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncomeStatmentController : ControllerBase
    {
        private readonly IincomeStatmentservice _incomeStatmentService;

        public IncomeStatmentController(IincomeStatmentservice incomeStatmentService)
        {
            _incomeStatmentService = incomeStatmentService;
        }

        [HttpGet("IncomeStatements")]
        public async Task<IActionResult> GetIncomeStatements(DateTime fromDate, DateTime toDate, int storeId)
        {
            var data = await _incomeStatmentService.GetList(fromDate, toDate, storeId);
            return Ok(data);
        }
    }
}
