using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Compound.API.Controllers.Reports.Accounting
{
    [Route("api/reports/accounting/income-statement")]
    [ApiController]
    public class IncomeStatementController : ControllerBase
    {
        private readonly IIncomeStatementService _incomeStatementService;

        public IncomeStatementController(IIncomeStatementService incomeStatementService)
        {
            _incomeStatementService = incomeStatementService;
        }

        [HttpGet]
        public IActionResult GetIncomeStatement(int storeId, DateTime? fromDate, DateTime? toDate)
        {
            var result = _incomeStatementService.GetIncomeStatement(storeId, fromDate, toDate);
            return Ok(result);
        }
    }
}
