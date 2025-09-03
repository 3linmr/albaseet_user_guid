using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.CoreOne.Contracts.Reports.FollowUpCombinedReports
{
    /// <summary>
    /// Provides methods to generate combined follow-up reports for supplier quotations.
    /// </summary>
    public interface ISupplierQuotationFollowUpCombinedReportService
    {
        /// <summary>
        /// Gets the combined follow-up report for supplier quotations.
        /// </summary>
        /// <param name="storeIds">The identifiers of the stores.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="isDetail">Indicates whether the report should include detailed information.</param>
        /// <returns>A queryable collection of supplier quotation follow-up report view models.</returns>
        Task<IQueryable<SupplierQuotationFollowUpReportVm>> GetSupplierQuotationFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail);
    }
}