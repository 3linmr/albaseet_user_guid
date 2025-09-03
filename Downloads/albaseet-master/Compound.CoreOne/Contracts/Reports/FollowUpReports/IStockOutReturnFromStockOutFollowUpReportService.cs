using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.CoreOne.Contracts.Reports.FollowUpReports
{
    /// <summary>
    /// Provides methods to generate follow-up reports for stock-out returns from stock-out.
    /// </summary>
    public interface IStockOutReturnFromStockOutFollowUpReportService
    {
        /// <summary>
        /// Gets the follow-up report for stock-out returns from stock-out.
        /// </summary>
        /// <param name="storeIds">A list of store identifiers.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>An <see cref="IQueryable{StockOutReturnFromStockOutFollowUpReportDto}"/> representing the report data.</returns>
        IQueryable<StockOutReturnFromStockOutFollowUpReportDto> GetStockOutReturnFromStockOutFollowUpReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate);
    }
}