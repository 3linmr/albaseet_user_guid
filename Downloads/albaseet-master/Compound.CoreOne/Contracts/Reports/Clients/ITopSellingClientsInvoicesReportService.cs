using Compound.CoreOne.Models.Dtos.Reports.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Clients
{
    /// <summary>
    /// Provides methods to generate reports for top selling clients' invoices.
    /// </summary>
    public interface ITopSellingClientsInvoicesReportService
    {
        /// <summary>
        /// Gets the top selling clients invoices report for the specified company and date range.
        /// </summary>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>A list of top selling clients invoices report DTOs.</returns>
        Task<IQueryable<TopSellingClientsInvoicesReportDto>> GetTopSellingClientsInvoicesReport(int companyId, DateTime? fromDate, DateTime? toDate);
    }
}
