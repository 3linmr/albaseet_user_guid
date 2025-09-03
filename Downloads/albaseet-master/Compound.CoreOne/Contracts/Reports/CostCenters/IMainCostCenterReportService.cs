using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Dtos.Reports.CostCenters;

namespace Compound.CoreOne.Contracts.Reports.CostCenters
{
    /// <summary>
    /// Provides methods to generate main cost center reports.
    /// </summary>
    public interface IMainCostCenterReportService
    {
        /// <summary>
        /// Gets the main cost center report for the specified company and date range.
        /// </summary>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="mainCostCenterId">The identifier of the main cost center (optional).</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="debitOnly">Indicates whether to include only debit transactions.</param>
        /// <param name="includeItems">Indicates whether to include items in the report.</param>
        /// <returns>A list of main cost center report DTOs.</returns>
        Task<List<MainCostCenterReportDto>> GetMainCostCenterReport(int companyId, int? mainCostCenterId, DateTime? fromDate, DateTime? toDate, bool debitOnly, bool includeItems);
    }
}
