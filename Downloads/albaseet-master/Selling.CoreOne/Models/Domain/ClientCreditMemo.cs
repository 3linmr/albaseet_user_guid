using Shared.CoreOne.Models.Domain.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Journal;

namespace Sales.CoreOne.Models.Domain
{
    public class ClientCreditMemo : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ClientCreditMemoId { get; set; }

        [StringLength(10)]
        [Column(Order = 2)]
        public string? Prefix { get; set; }

        [Column(Order = 3)]
        public int DocumentCode { get; set; }

        [StringLength(10)]
        [Column(Order = 4)]
        public string? Suffix { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 5)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DocumentDate { get; set; }

        [Column(Order = 6)]
        public DateTime EntryDate { get; set; }

        [StringLength(20)]
        [Column(Order = 7)]
        public string? DocumentReference { get; set; }

        [Column(Order = 8)]
        public int SalesInvoiceHeaderId { get; set; }

        [Column(Order = 9)]
        public int ClientId { get; set; }

        [Column(Order = 10)]
        public int? SellerId { get; set; }

		[Column(Order = 11)]
        public int StoreId { get; set; }

        [Column(Order = 12)]
        [StringLength(20)]
        public string? Reference { get; set; }

        [Column(Order = 13)]
        public int DebitAccountId { get; set; }

        [Column(Order = 14)]
        public int CreditAccountId { get; set; }

        [Column(Order = 15, TypeName = "decimal(30,15)")]
        public decimal MemoValue { get; set; }

        [Column(Order = 16)]
        public int JournalHeaderId { get; set; }

        [Column(Order = 17)]
        [StringLength(2000)]
        public string? RemarksAr { get; set; }

        [Column(Order = 18)]
        [StringLength(2000)]
        public string? RemarksEn { get; set; }

        [Column(Order = 19)]
        public bool IsClosed { get; set; }

        [Column(Order = 20)]
        public int? ArchiveHeaderId { get; set; }



        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }

        [ForeignKey(nameof(SellerId))]
        public Seller? Seller { get; set; }

		[ForeignKey(nameof(SalesInvoiceHeaderId))]
        public SalesInvoiceHeader? SalesInvoiceHeader { get; set; }

        [ForeignKey(nameof(DebitAccountId))]
        public Account? DebitAccount { get; set; }

        [ForeignKey(nameof(CreditAccountId))]
        public Account? CreditAccount { get; set; }

        [ForeignKey(nameof(JournalHeaderId))]
        public JournalHeader? JournalHeader { get; set; }

        [ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }
    }
}
