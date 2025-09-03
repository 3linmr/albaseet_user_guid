using Compound.CoreOne.Models.Dtos.Reports.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Suppliers
{
    public interface ISuppliersExceedCreditLimitReportService
    {
        public IQueryable<SuppliersExceedCreditLimitDto> GetSuppliersExceedCreditLimitReport(int companyId,int? supplierId, bool isDay);

    }
}
