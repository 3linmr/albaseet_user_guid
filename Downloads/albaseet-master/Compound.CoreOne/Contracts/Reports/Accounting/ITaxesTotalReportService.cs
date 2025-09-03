using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Accounting
{
    /// <summary>
    /// Provides methods to generate total taxes reports.
    /// </summary>
    public interface ITaxesTotalReportService
    {
        /// <summary>
        /// Gets the total taxes report for sales and purchases within the specified date range and store.
        /// </summary>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>A list of taxes total report DTOs.</returns>
        Task<List<TaxesTotalReportDto>> GetTaxesTotalReport(int companyId, DateTime? fromDate, DateTime? toDate);
    }
}
