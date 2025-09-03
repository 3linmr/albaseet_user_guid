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
    /// Provides methods to generate ageing sales invoice reports.
    /// </summary>
    public interface IAgeingSalesInvoiceReportService
    {
        /// <summary>
        /// Gets the ageing sales invoice report for the specified stores and date.
        /// </summary>
        /// <param name="storeIds">The identifiers of the stores.</param>
        /// <param name="toDate">The date for the report.</param>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="sellerId">The identifier of the seller.</param>
        /// <returns>A queryable collection of ageing sales invoice report DTOs.</returns>
        IQueryable<AgeingSalesInvoiceReportDto> GetAgeingSalesInvoiceReport(List<int> storeIds, DateTime? toDate, int? clientId, int? sellerId);
    }
}
