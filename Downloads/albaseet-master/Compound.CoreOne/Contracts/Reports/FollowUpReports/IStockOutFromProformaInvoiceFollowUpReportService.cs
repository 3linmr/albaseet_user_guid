using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.CoreOne.Contracts.Reports.FollowUpReports
{
    /// <summary>
    /// Provides methods to generate follow-up reports for stock-out from proforma invoices.
    /// </summary>
    public interface IStockOutFromProformaInvoiceFollowUpReportService
    {
        /// <summary>
        /// Gets the follow-up report for stock-out from proforma invoices.
        /// </summary>
        /// <param name="storeIds">A list of store identifiers.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>An <see cref="IQueryable{StockOutFromProformaInvoiceFollowUpReportDto}"/> representing the report data.</returns>
		IQueryable<StockOutFromProformaInvoiceFollowUpReportDto> GetStockOutFromProformaInvoiceFollowUpReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate);
    }
}