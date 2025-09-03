using Compound.CoreOne.Models.Dtos.Reports.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Clients
{
    /// <summary>
    /// Provides methods to generate reports for sales invoices with discounts.
    /// </summary>
    public interface ISalesInvoicesWithDiscountReportService
    {
        /// <summary>
        /// Gets the sales invoices with discount greater than the specified threshold for the given store IDs and date range.
        /// </summary>
        /// <param name="storeIds">The list of store identifiers.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="sellerId">The identifier of the seller.</param>
        /// <param name="discountThreshold">The discount threshold value.</param>
        /// <returns>A queryable collection of sales invoices with discount DTOs.</returns>
        IQueryable<SalesInvoicesWithDiscountDto> GetSalesInvoicesWithDiscountGreaterThanReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? clientId, int? sellerId, decimal discountThreshold);
    }
}
