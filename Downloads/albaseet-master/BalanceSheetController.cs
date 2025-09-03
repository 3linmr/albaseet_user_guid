using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Compound.Service.Controllers.Reports.Accounting
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceSheetController : ControllerBase
    {
        private readonly IBalanceSheetServices balanceSheetServices;

        public BalanceSheetController(IBalanceSheetServices balanceSheetServices)
        {
            this.balanceSheetServices = balanceSheetServices;
        }

        /// <summary>
        /// Generates a balance sheet report for a specific company and date.
        /// </summary>
        /// <param name="companyId">The ID of the company for which the balance sheet is generated.</param>
        /// <param name="asOfDate">The date for which the balance sheet is generated.</param>
        /// <returns>A balance sheet report containing assets, liabilities, and equity.</returns>
        [HttpGet("GenerateBalanceSheet")]
        public async Task<IActionResult> GenerateBalanceSheet([FromQuery] int companyId, [FromQuery] DateTime asOfDate)
        {
            if (companyId <= 0)
            {
                return BadRequest("Invalid company ID.");
            }

            try
            {
                var balanceSheet = await balanceSheetServices.GenerateBalanceSheetAsync(companyId, asOfDate);
                return Ok(balanceSheet);
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is configured)
                return StatusCode(500, $"An error occurred while generating the balance sheet: {ex.Message}");
            }
        }
    }
}
