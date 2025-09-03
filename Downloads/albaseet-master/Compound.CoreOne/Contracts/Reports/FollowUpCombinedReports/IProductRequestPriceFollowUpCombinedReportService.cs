using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.CoreOne.Contracts.Reports.FollowUpCombinedReports
{
    /// <summary>
    /// Provides methods to generate combined follow-up reports for product request prices.
    /// </summary>
    public interface IProductRequestPriceFollowUpCombinedReportService
    {
        /// <summary>
        /// Gets the combined follow-up report for product request prices.
        /// </summary>
        /// <param name="storeIds">The identifiers of the stores.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="isDetail">Indicates whether the report should include detailed information.</param>
        /// <returns>A queryable collection of product request price follow-up report view models.</returns>
        Task<IQueryable<ProductRequestPriceFollowUpReportVm>> GetProductRequestPriceFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail);
    }
}