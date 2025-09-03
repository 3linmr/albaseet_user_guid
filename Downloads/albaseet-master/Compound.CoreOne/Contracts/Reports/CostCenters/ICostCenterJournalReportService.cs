using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Dtos.Reports.CostCenters;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;

namespace Compound.CoreOne.Contracts.Reports.CostCenters
{
    /// <summary>
    /// Provides methods to generate cost center journal reports.
    /// </summary>
    public interface ICostCenterJournalReportService
    {
        /// <summary>
        /// Gets the cost center journal report for the specified cost center and date range.
        /// </summary>
        /// <param name="costCenterId">The identifier of the cost center.</param>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="debitOnly">Indicates whether to include only debit transactions.</param>
        /// <param name="includeItems">Indicates whether to include items in the report.</param>
        /// <returns>A queryable collection of cost center journal report DTOs.</returns>
        IQueryable<CostCenterJournalReportDto> GetCostCenterJournalReport(int? costCenterId, int companyId, DateTime? fromDate, DateTime? toDate, bool debitOnly, bool includeItems);
    }
}
