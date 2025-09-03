using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Accounting
{
    public class GeneralJournalReportDto 
    {
        public int JournalDetailId { get; set; }
        public int JournalHeaderId { get; set; }
        public int JournalId { get; set; }
        public int TicketId { get; set; }
        public int Serial { get; set; }
        public byte JournalTypeId { get; set; }
        public string? JournalTypeName { get; set; }
        public int? InvoiceId { get; set; }
        public string? InvoiceFullCode { get; set; }
        public string? JournalFullCode { get; set; }
        public DateTime TicketDate { get; set; }
        public DateTime EntryDate { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public int AccountId { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public bool IsPrivate { get; set; }
        public string? InvoiceReference { get; set; }
        public string? PeerReference { get; set; }
        public decimal CreditValue { get; set; }
        public decimal DebitValue { get; set; }
        public decimal Difference { get; set; }
        public short CurrencyId { get; set; }
        public string? CurrencyName { get; set; }
        public decimal CurrencyRate { get; set; }
        public decimal CreditValueAccount { get; set; }
        public decimal DebitValueAccount { get; set; }
        public decimal DifferenceAccount { get; set; }
        public bool IsTax { get; set; }
        public int? TaxId { get; set; }
        public int? TaxParentId { get; set; }
        public decimal? TaxPercent { get; set; }
        public string? TaxName { get; set; }
        public string? TaxCode { get; set; }
        public string? TaxReference { get; set; }
        public DateTime? TaxDate { get; set; }
        public byte? InvoiceTypeId { get; set; }
        public string? InvoiceTypeName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UserNameCreated { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
    }
}
