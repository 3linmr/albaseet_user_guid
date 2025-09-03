using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Accounting
{
    /// <summary>
    /// Provides methods to generate profit and loss reports.
    /// </summary>
    public interface IProfitAndLossReportService
    {
        /// <summary>
        /// Gets the profit and loss details for the specified store and date range.
        /// </summary>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="mainAccountId">
        /// The identifier of the main account to filter the report. 
        /// If <c>null</c>, the report returns the root accounts (first level).
        /// </param>
        /// <returns>A list collection of profit and loss DTOs.</returns>
		Task<List<ProfitAndLossReportDto>> GetProfitAndLossReport(int companyId, DateTime? fromDate, DateTime? toDate, int? level, int? mainAccountId);
    }
}
