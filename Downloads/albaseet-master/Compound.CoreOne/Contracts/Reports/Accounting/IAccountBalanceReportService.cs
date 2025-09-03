using Compound.CoreOne.Models.Dtos.Reports;
using System;
using System.Collections.Generic;

using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Accounting
{
    /// <summary>
    /// Provides methods to generate account balance reports.
    /// </summary>
    public interface IAccountBalanceReportService
    {
        /// <summary>
        /// Gets the account balance report for the specified store and date range.
        /// </summary>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="mainAccountId">If zero, return all accounts</param>
        /// <returns>A list of account balance report DTOs.</returns>
        public Task<List<BalanceReportDto>> GetAccountBalanceReport(int companyId, DateTime? fromDate, DateTime? toDate, int? mainAccountId);

        /// <summary>
        /// Gets the account balance report as an IQueryable for the specified company and date range.
        /// </summary>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="onlyNonMainAccounts">If true, return only non-main accounts; if false, return all accounts.</param>
        /// <returns>An IQueryable of account balance report DTOs.</returns>
        public IQueryable<BalanceReportDto> GetAccountDataQueryable(int companyId, DateTime? fromDate, DateTime? toDate, bool onlyNonMainAccounts);
    }
}
