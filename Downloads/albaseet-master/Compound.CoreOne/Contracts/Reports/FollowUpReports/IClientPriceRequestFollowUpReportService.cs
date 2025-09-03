using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.CoreOne.Contracts.Reports.FollowUpReports
{
    /// <summary>
    /// Provides methods to generate follow-up reports for client price requests.
    /// </summary>
    public interface IClientPriceRequestFollowUpReportService
    {
        /// <summary>
        /// Gets the follow-up report for client price requests.
        /// </summary>
        /// <param name="storeIds">A list of store identifiers.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>An <see cref="IQueryable{ClientPriceRequestFollowUpReportDto}"/> representing the report data.</returns>
        IQueryable<ClientPriceRequestFollowUpReportDto> GetClientPriceRequestFollowUpReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate);
    }
}