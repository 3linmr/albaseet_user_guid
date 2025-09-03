using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.CoreOne.Contracts.Reports.FollowUpDetailReports
{
    /// <summary>
    /// Provides methods to generate detailed follow-up reports for client price requests.
    /// </summary>
    public interface IClientPriceRequestFollowUpDetailReportService
    {
        /// <summary>
        /// Gets the detailed follow-up report for client price requests.
        /// </summary>
        /// <param name="storeIds">A list of store identifiers.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>An <see cref="IQueryable{ClientPriceRequestFollowUpDetailReportDto}"/> representing the report data.</returns>
        Task<IQueryable<ClientPriceRequestFollowUpDetailReportDto>> GetClientPriceRequestFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate);
    }
}