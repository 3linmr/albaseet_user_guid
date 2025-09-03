using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Accounting
{
    public interface IBalanceSheetReportService
    /// <summary>
    /// Provides methods to generate balance sheet reports.
    /// </summary>
    {
        /// <summary>
        /// Gets the balance sheet details for the specified company, date range, and main account.
        /// </summary>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="fromDate">The start date of the balance sheet period. If <c>null</c>, the report includes all data up to <paramref name="toDate"/>.</param>
        /// <param name="toDate">The end date of the balance sheet period. If <c>null</c>, the report includes all data from <paramref name="fromDate"/> onward.</param>
        /// <param name="level">
        /// The nesting level of accounts to show in the report. 
        /// </param>
        /// <returns>A list of balance sheet DTOs representing the report details.</returns>
        Task<List<BalanceSheetReportDto>> GetBalanceSheetReport(int companyId, DateTime? fromDate, DateTime? toDate, int? level, int? parentSerial);
    }
}
