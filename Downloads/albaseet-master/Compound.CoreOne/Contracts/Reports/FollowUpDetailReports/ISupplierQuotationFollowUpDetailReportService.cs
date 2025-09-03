using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.CoreOne.Contracts.Reports.FollowUpDetailReports
{
    /// <summary>
    /// Provides methods to generate detailed follow-up reports for supplier quotations.
    /// </summary>
    public interface ISupplierQuotationFollowUpDetailReportService
    {
        /// <summary>
        /// Gets the detailed follow-up report for supplier quotations.
        /// </summary>
        /// <param name="storeIds">A list of store identifiers.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>An <see cref="IQueryable{SupplierQuotationFollowUpDetailReportDto}"/> representing the report data.</returns>
        Task<IQueryable<SupplierQuotationFollowUpDetailReportDto>> GetSupplierQuotationFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate);
    }
}