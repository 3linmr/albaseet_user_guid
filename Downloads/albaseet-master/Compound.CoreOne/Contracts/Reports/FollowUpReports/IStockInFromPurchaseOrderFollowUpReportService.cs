using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compound.CoreOne.Contracts.Reports.FollowUpReports
{
    /// <summary>
    /// Provides methods to generate follow-up reports for stock-in from purchase orders.
    /// </summary>
    public interface IStockInFromPurchaseOrderFollowUpReportService
    {
        /// <summary>
        /// Gets the follow-up report for stock-in from purchase orders.
        /// </summary>
        /// <param name="storeIds">A list of store identifiers.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>An <see cref="IQueryable{StockInFromPurchaseOrderFollowUpReportDto}"/> representing the report data.</returns>
        IQueryable<StockInFromPurchaseOrderFollowUpReportDto> GetStockInFromPurchaseOrderFollowUpReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate);
    }
}
