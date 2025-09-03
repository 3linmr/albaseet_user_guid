using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Shared
{
    public class GeneralInvoiceDto
    {
        public int InvoiceId { get; set; }
        public int StoreId { get; set; }
        public int? JournalHeaderId { get; set; }
        public short? MenuCode { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime? EntryDate { get; set; }
        public string? InvoicePrefix { get; set; }
        public int InvoiceCode { get; set; }
        public string? InvoiceSuffix { get; set; }
        public string? FullInvoiceCode { get; set; }
        public string? Reference { get; set; }
        public byte? InvoiceTypeId { get; set; }
        public byte? TaxTypeId { get; set; }
        public decimal? NetValue { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
        public decimal? GrossValue { get; set; }
    }
}
