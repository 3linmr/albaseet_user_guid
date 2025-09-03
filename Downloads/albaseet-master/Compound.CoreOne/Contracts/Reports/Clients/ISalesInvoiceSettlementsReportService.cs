using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Dtos.Reports.Clients;
using Compound.CoreOne.Models.Dtos.Reports.Suppliers;

namespace Compound.CoreOne.Contracts.Reports.Clients
{
    /// <summary>
    /// Provides methods to generate sales invoice settlements reports.
    /// </summary>
    public interface ISalesInvoiceSettlementsReportService
    {
        /// <summary>
        /// Gets the sales invoice settlements report for the specified store and date range.
        /// </summary>
        /// <param name="storeIds">The identifiers of the stores.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="sellerId">The identifier of the seller.</param>
        /// <returns>A queryable collection of sales invoice settlements report DTOs.</returns>
        IQueryable<SalesInvoiceSettlementsReportDto> GetSalesInvoiceSettlementsReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? clientId, int? sellerId);
    }
}
