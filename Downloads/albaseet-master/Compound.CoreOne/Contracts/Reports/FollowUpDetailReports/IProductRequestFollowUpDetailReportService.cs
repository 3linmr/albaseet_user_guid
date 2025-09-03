using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.CoreOne.Contracts.Reports.FollowUpDetailReports
{
    /// <summary>
    /// Provides methods to generate detailed follow-up reports for product requests.
    /// </summary>
    public interface IProductRequestFollowUpDetailReportService
    {
        /// <summary>
        /// Gets the detailed follow-up report for product requests.
        /// </summary>
        /// <param name="storeIds">A list of store identifiers.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>An <see cref="IQueryable{ProductRequestFollowUpDetailReportDto}"/> representing the report data.</returns>
        Task<IQueryable<ProductRequestFollowUpDetailReportDto>> GetProductRequestFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate);
    }
}