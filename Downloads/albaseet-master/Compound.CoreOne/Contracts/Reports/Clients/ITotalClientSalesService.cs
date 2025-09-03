using Compound.CoreOne.Models.Dtos.Reports.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Clients
{
    /// <summary>
    /// Provides methods to generate total client sales reports.
    /// </summary>
    public interface ITotalClientSalesService
    {
        /// <summary>
        /// Gets the total client sales report for the specified store and date range.
        /// </summary>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="includeStores">Indicates whether to include store details in the report.</param>
        /// <returns>A queryable collection of top-selling clients report values DTOs.</returns>
        IQueryable<TopSellingClientsReportValuesDto> GetTotalClientSales(DateTime? fromDate, DateTime? toDate, bool includeStores);
    }
}
