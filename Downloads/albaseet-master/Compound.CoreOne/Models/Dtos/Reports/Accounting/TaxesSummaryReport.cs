using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Accounting
{
    public class TaxesSummaryReportDto
    {
        public string TaxName { get; set; } // Tax name (e.g., VAT, Sales Tax)
        public string TaxTypeName { get; set; } // Tax type name (e.g., Output Tax, Input Tax)
        public decimal TotalDebit { get; set; } // Total debit amount
        public decimal TotalCredit { get; set; } // Total credit amount
        public decimal NetTax { get; set; } // Net tax (TotalDebit - TotalCredit)
    }
}
