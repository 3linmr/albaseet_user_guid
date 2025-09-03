using Accounting.CoreOne.Models.Dtos.Reports;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Accounting
{
    /// <summary>
    /// Provides methods to generate account statement reports.
    /// </summary>
    public interface IAccountStatementReportService
    {
        /// <summary>
        /// Gets the account statement report for the specified account and date range.
        /// </summary>
        /// <param name="accountId">The identifier of the account.</param>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>A queryable list of account statement DTOs.</returns>
        public IQueryable<AccountStatementDto> GetAccountStatementReport(int accountId, int companyId, DateTime? fromDate, DateTime? toDate);

        /// <summary>
        /// Gets the general accounts statement report for the specified accounts and date range.
        /// </summary>
        /// <param name="accountIds">The list of account identifiers.</param>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <returns>A queryable list of account statement DTOs.</returns>
        public IQueryable<AccountStatementDto> GetGeneralAccountsStatementReport(List<int> accountIds, int companyId, DateTime? fromDate, DateTime? toDate);
    }
}
