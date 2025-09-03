using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.FollowUpCombinedReports
{
    /// <summary>
    /// Provides methods to generate combined follow-up reports for proforma invoices.
    /// </summary>
    public interface IProformaInvoiceFollowUpCombinedReportService
    {
        /// <summary>
        /// Gets the combined follow-up report for proforma invoices.
        /// </summary>
        /// <param name="storeIds">The identifiers of the stores.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="isDetail">Indicates whether the report should include detailed information.</param>
        /// <returns>A queryable collection of proforma invoice follow-up report view models.</returns>
        Task<IQueryable<ProformaInvoiceFollowUpReportVm>> GetProformaInvoiceFollowUpCombinedReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isDetail);
    }
}