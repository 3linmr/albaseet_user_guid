using Compound.CoreOne.Models.Dtos.Reports.CostCenters;

namespace Compound.CoreOne.Contracts.Reports.CostCenters
{
    /// <summary>
    /// Provides methods to generate cost center accounts reports.
    /// </summary>
    public interface ICostCenterAccountsReportService
    {
        /// <summary>
        /// Gets the cost center accounts report for the specified cost center and date range.
        /// </summary>
        /// <param name="costCenterId">The identifier of the cost center.</param>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="debitOnly">Indicates whether to include only debit transactions.</param>
        /// <param name="includeItems">Indicates whether to include items in the report.</param>
        /// <returns>A queryable collection of cost center accounts report DTOs.</returns>
        IQueryable<CostCenterAccountsReportDto> GetCostCenterAccountsReport(int? costCenterId, int companyId, DateTime? fromDate, DateTime? toDate, bool debitOnly, bool includeItems);
    }
}
