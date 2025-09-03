using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;

namespace Compound.CoreOne.Contracts.Reports.Accounting
{
    /// <summary>
    /// Provides methods to generate store income reports.
    /// </summary>
    public interface IStoreIncomeReportService
    {
        /// <summary>
        /// Gets the store income report for the specified store and date range.
        /// </summary>
        /// <param name="storeIds">The identifiers of the stores.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="menuCodes">The menu codes to filter the report.</param>
        /// <returns>A queryable collection of store income report DTOs.</returns>
        IQueryable<StoreIncomeReportDto> GetStoreIncomeReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, List<short> menuCodes);
    }
}
