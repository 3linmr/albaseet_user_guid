using Compound.CoreOne.Models.Dtos.Reports.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Clients
{
    /// <summary>
    /// Provides methods to generate reports for clients exceeding their credit limits.
    /// </summary>
    public interface IClientsExceedCreditLimitReportService
    {
        /// <summary>
        /// Gets the report of clients who have exceeded their credit limits for the specified company.
        /// </summary>
        /// <param name="companyId">The identifier of the company.</param>
        /// <param name="clientId">The identifier of the client (optional).</param>
        /// <param name="isDay">Indicates whether the report is for a specific day.</param>
        /// <returns>A queryable collection of clients exceed credit limit report DTOs.</returns>
        public IQueryable<ClientsExceedCreditLimitDto> GetClientsExceedCreditLimitReport(int companyId, int? clientId, bool isDay);
    }
}
