using Compound.CoreOne.Models.Dtos.Reports.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Clients
{
    public interface ITopSellingClientsWithAmountsReportService
    {
        IQueryable<TopSellingClientsWithAmountsReportDto> GetTopSellingClientsWithAmountsReport(int companyId, DateTime? fromDate, DateTime? toDate);
    }
}
