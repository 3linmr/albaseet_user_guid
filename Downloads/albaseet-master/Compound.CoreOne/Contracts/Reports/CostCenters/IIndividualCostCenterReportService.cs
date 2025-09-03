using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Dtos.Reports.CostCenters;

namespace Compound.CoreOne.Contracts.Reports.CostCenters
{
    /// <summary>
    /// Provides methods to generate individual cost center reports.
    /// </summary>
    public interface IIndividualCostCenterReportService
    {
        /// <summary>
        /// Gets the individual cost center report for the specified cost center and date range.
        /// </summary>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="debitOnly">Indicates whether to include only debit transactions.</param>
        /// <param name="includeItems">Indicates whether to include items in the report.</param>
        /// <returns>A queryable collection of individual cost center report DTOs.</returns>
        public IQueryable<IndividualCostCenterReportDto> GetIndividualCostCenterReport(int companyId, DateTime? fromDate, DateTime? toDate, bool debitOnly, bool includeItems);
    }
}
