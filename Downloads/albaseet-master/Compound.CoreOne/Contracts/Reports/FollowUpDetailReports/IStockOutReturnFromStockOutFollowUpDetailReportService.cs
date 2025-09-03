using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.CoreOne.Contracts.Reports.FollowUpDetailReports
{
    /// <summary>
    /// Provides methods to generate detailed follow-up reports for stock-out returns from stock-out.
    /// </summary>
    public interface IStockOutReturnFromStockOutFollowUpDetailReportService
    {
        /// <summary>
        /// Gets the detailed follow-up report for stock-out returns from stock-out.
        /// </summary>
        /// <param name="storeIds">A list of store identifiers.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>An <see cref="IQueryable{StockOutReturnFromStockOutFollowUpDetailReportDto}"/> representing the report data.</returns>
        Task<IQueryable<StockOutReturnFromStockOutFollowUpDetailReportDto>> GetStockOutReturnFromStockOutFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate);
    }
}