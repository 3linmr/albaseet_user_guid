using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.CoreOne.Contracts.Reports.FollowUpDetailReports
{
    /// <summary>
    /// Provides methods to generate detailed follow-up reports for purchase invoices on the way.
    /// </summary>
    public interface IPurchaseInvoiceOnTheWayFollowUpDetailReportService
    {
        /// <summary>
        /// Gets the detailed follow-up report for purchase invoices on the way.
        /// </summary>
        /// <param name="storeIds">A list of store identifiers.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>An <see cref="IQueryable{PurchaseInvoiceOnTheWayFollowUpDetailReportDto}"/> representing the report data.</returns>
        Task<IQueryable<PurchaseInvoiceOnTheWayFollowUpDetailReportDto>> GetPurchaseInvoiceOnTheWayFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate);
    }
}