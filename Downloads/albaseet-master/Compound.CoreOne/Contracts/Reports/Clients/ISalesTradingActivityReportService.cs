using Compound.CoreOne.Models.Dtos.Reports.Clients;
using Shared.CoreOne.Models.Domain.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Clients
{
    /// <summary>
    /// Provides methods to generate sales trading activity reports.
    /// </summary>
    public interface ISalesTradingActivityReportService
    {
        /// <summary>
        /// Gets the sales trading activity report for the specified store and date range.
        /// </summary>
        /// <param name="storeIds">The identifiers of the stores.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="sellerId">The identifier of the seller.</param>
        /// <returns>A queryable collection of sales trading activity report DTOs.</returns>
        IQueryable<SalesTradingActivityReportDto> GetSalesTradingActivityReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? clientId, int? sellerId);
    }
}
