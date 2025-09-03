using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.CoreOne.Contracts.Reports.FollowUpCombinedReports
{
    /// <summary>
    /// Provides methods to generate combined follow-up reports for purchase invoice interims.
    /// </summary>
    public interface IPurchaseInvoiceInterimFollowUpCombinedReportService
    {
        /// <summary>
        /// Gets the combined follow-up report for purchase invoice interims.
        /// </summary>
        /// <param name="storeIds">The list of store identifiers.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="isDetail">Indicates whether the report should include detailed information.</param>
        /// <returns>A queryable collection of purchase invoice interim follow-up report view models.</returns>
        Task<IQueryable<PurchaseInvoiceInterimFollowUpReportVm>> GetPurchaseInvoiceInterimFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail);
    }
}