using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Accounting
{
    public class AccountStatementDto
    {
        public int AccountId { get; set; }
        public string? AccountCode{ get; set; }
        public string? AccountName { get; set; }
        public string? StoreName { get; set; }
        public string? JournalTypeName { get; set; }
        public string? DocumentTypeName { get; set; }
        public string? InvoiceTypeName { get; set; }
        public string? DocumentFullCode { get; set; }
        public int? JournalDetailId { get; set; }
        public string? JournalFullCode { get; set; }// مسلسل القيد
        public DateTime? TicketDate { get; set; }        
        public DateTime? EntryDate { get; set; }
        public decimal CreditValue { get; set; }
        public decimal DebitValue { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceAccount { get; set; }//رصيد حالي عملة
        public decimal DebitValueAccount { get; set; } 
        public decimal CreditValueAccount { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
        public string? CostCenterName { get; set; } // مركز تكلفة
        public string? PeerReference { get; set; } // كود مرجعي للقيد
        public string? DocumentReference { get; set; } //كود مرجعي للسند
        public int DocumentEntrySerial { get; set; } // مسلسل قيد السند
        public decimal OpeningBalanceAccount { get; set; } // اول المدة عملة
        public decimal ExchangeRate { get; set; } // سعر الصرف

        public DateTime? CreatedAt { get; set; }
        public string? UserNameCreated { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
    }
}
