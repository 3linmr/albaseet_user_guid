using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.CoreOne.Contracts.Reports.FollowUpReports
{
    /// <summary>
    /// Provides methods to generate follow-up reports for sales invoice interims.
    /// </summary>
    public interface ISalesInvoiceInterimFollowUpReportService
    {
        /// <summary>
        /// Gets the follow-up report for sales invoice interims.
        /// </summary>
        /// <param name="storeIds">A list of store identifiers.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>An <see cref="IQueryable{SalesInvoiceInterimFollowUpReportDto}"/> representing the report data.</returns>
        IQueryable<SalesInvoiceInterimFollowUpReportDto> GetSalesInvoiceInterimFollowUpReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate);
    }
}