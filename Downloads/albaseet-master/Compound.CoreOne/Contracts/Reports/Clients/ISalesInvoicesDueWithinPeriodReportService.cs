using Compound.CoreOne.Models.Dtos.Reports.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Clients
{
    /// <summary>
    /// Provides methods to generate reports for sales invoices due within a specific period.
    /// </summary>
    public interface ISalesInvoicesDueWithinPeriodReportService
    {
        /// <summary>
        /// Gets the sales invoices due within the specified period for the given stores.
        /// </summary>
        /// <param name="storeIds">The identifiers of the stores.</param>
        /// <param name="periodDays">The number of days in the period.</param>
        /// <param name="clientId">The identifier of the client (optional).</param>
        /// <param name="sellerId">The identifier of the seller (optional).</param>
        /// <returns>A queryable collection of sales invoices due within period report DTOs.</returns>
        IQueryable<SalesInvoicesDueWithinPeriodReportDto> GetSalesInvoicesDueWithinPeriodReport(List<int> storeIds, int periodDays, int? clientId, int? sellerId);
    }
}
