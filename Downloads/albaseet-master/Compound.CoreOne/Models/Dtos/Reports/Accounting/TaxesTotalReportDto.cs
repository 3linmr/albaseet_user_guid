using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Accounting
{
    public class TaxesTotalReportDto
    {
        public int Serial { get; set; }
        public int? TaxTypeId { get; set; }
        public string? TaxTypeName { get; set; }

        public decimal TotalValue { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TotalTaxValue { get; set; }

        public decimal TotalPurchaseOrSalesAmount { get; set; }
        public decimal TotalPurchaseOrSaleReturnAmount { get; set; }
        public decimal TotalJournalEntryAmount { get; set; }
    }
}
