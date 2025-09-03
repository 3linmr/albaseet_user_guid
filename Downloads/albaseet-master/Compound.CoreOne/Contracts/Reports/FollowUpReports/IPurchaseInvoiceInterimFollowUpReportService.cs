using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compound.CoreOne.Contracts.Reports.FollowUpReports
{
    /// <summary>
    /// Provides methods to generate follow-up reports for purchase invoice interims.
    /// </summary>
    public interface IPurchaseInvoiceInterimFollowUpReportService
    {
        /// <summary>
        /// Gets the follow-up report for purchase invoice interims.
        /// </summary>
        /// <param name="storeIds">A list of store identifiers.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>An <see cref="IQueryable{PurchaseInvoiceInterimFollowUpReportDto}"/> representing the report data.</returns>
        IQueryable<PurchaseInvoiceInterimFollowUpReportDto> GetPurchaseInvoiceInterimFollowUpReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate);
    }
}
