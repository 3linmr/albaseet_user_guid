using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Accounting
{
    /// <summary>
    /// Provides methods to generate detailed taxes reports.
    /// </summary>
    public interface ITaxesDetailReportService
    {
        /// <summary>
        /// Gets the detailed taxes report for sales and purchases within the specified date range and store.
        /// </summary>
        /// <param name="storeIds">The identifiers of the stores.</param>
        /// <param name="fromDate">The start date for the report (inclusive).</param>
        /// <param name="toDate">The end date for the report (inclusive).</param>
        /// <param name="isVatTax">Indicates whether to include only VAT taxes.</param>
        /// <returns>A queryable list of taxes detail report DTOs.</returns>
        public IQueryable<TaxesDetailReportDto> GetTaxesDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, bool isVatTax);
    }
}
