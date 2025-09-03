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
    /// Provides methods to generate reports for unsettled or settled sales invoices.
    /// </summary>
    public interface IUnSettledOrSettledSalesInvoicesReportService
    {
        /// <summary>
        /// Gets the unsettled or settled sales invoices report for the specified store and date range.
        /// </summary>
        /// <param name="storeIds">The identifiers of the stores.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="sellerId">The identifier of the seller.</param>
        /// <param name="getSettled">Indicates whether to get settled or unsettled invoices.</param>
        /// <returns>A queryable collection of unsettled or settled sales invoices report DTOs.</returns>
        IQueryable<UnSettledOrSettledSalesInvoicesReportDto> GetUnSettledOrSettledSalesInvoicesReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? clientId, int? sellerId, bool getSettled);
    }
}
