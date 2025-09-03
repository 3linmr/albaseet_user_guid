using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Accounting
{
    /// <summary>
    /// Provides methods to generate reports for missing document codes.
    /// </summary>
    public interface IMissingDocumentCodesReportService
    {
        /// <summary>
        /// Gets the report of missing document codes for the specified store and document type.
        /// </summary>
        /// <param name="storeIds">The identifiers of the stores.</param>
        /// <param name="menuCodes">The menu codes to check for missing document codes.</param>
        /// <returns>A list of missing document codes report DTOs.</returns>
        Task<IEnumerable<MissingDocumentCodesReportDto>> GetMissingDocumentCodesReport(List<int> storeIds, List<int> menuCodes);
    }
}
