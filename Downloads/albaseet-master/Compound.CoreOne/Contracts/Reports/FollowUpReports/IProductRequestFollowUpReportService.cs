using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.CoreOne.Contracts.Reports.FollowUpReports
{
    /// <summary>
    /// Provides methods to generate follow-up reports for product requests.
    /// </summary>
    public interface IProductRequestFollowUpReportService
    {
        /// <summary>
        /// Gets the follow-up report for product requests.
        /// </summary>
        /// <param name="storeIds">A list of store identifiers.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>An <see cref="IQueryable{ProductRequestFollowUpReportDto}"/> representing the report data.</returns>
        IQueryable<ProductRequestFollowUpReportDto> GetProductRequestFollowUpReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate);
    }
}