using Compound.CoreOne.Models.Dtos.Reports.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Clients
{
    /// <summary>
    /// Provides methods to generate low profit sales invoices reports.
    /// </summary>
    public interface ILowProfitSalesInvoicesReportService
    {
        /// <summary>
        /// Gets the low profit sales invoices report for the specified store and date range.
        /// </summary>
        /// <param name="storeIds">The identifiers of the stores.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="sellerId">The identifier of the seller.</param>
        /// <param name="profitThreshold">The profit threshold for the report.</param>
        /// <returns>A queryable collection of low profit sales invoices report DTOs.</returns>
        public IQueryable<LowProfitSalesInvoicesReportDto> GetLowProfitInvoicesReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? clientId, int? sellerId, decimal profitThreshold);
    }
}
