using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;

namespace Compound.CoreOne.Contracts.Reports.Accounting
{
    /// <summary>
    /// Provides methods to generate general journal reports.
    /// </summary>
    public interface IGeneralJournalReportService
    {
        /// <summary>
        /// Gets the general journal report for the specified stores and date range.
        /// </summary>
        /// <param name="storeIds">The identifiers of the stores.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="journalTypes">The types of journals to include in the report.</param>
        /// <returns>A queryable collection of general journal report DTOs.</returns>
        public IQueryable<GeneralJournalReportDto> GetGeneralJournalReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, List<byte> journalTypes);

        /// <summary>
        /// Gets the general journal report for the user stores.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a queryable collection of general journal report DTOs.</returns>
        public Task<IQueryable<GeneralJournalReportDto>> GetGeneralJournalReportUserStores();
    }
}
