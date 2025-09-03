using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Dtos.Reports.Clients;

namespace Compound.CoreOne.Contracts.Reports.Clients
{
    /// <summary>
    /// Provides methods to generate clients profit reports.
    /// </summary>
    public interface IClientsProfitReportService
    {
        /// <summary>
        /// Gets the clients profit report for the specified company and date range.
        /// </summary>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>A list of clients profit report DTOs.</returns>
        Task<IQueryable<ClientsProfitReportDto>> GetClientsProfitReport(int companyId, DateTime? fromDate, DateTime? toDate);
    }
}
