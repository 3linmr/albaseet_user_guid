using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Journal;

namespace Accounting.CoreOne.Models.Domain
{
    public class PaymentVoucherHeader : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PaymentVoucherHeaderId { get; set; }

        [StringLength(10)]
        [Column(Order = 2)]
        public string? Prefix { get; set; }

        [Column(Order = 3)]
        public int PaymentVoucherCode { get; set; }

        [StringLength(10)]
        [Column(Order = 4)]
        public string? Suffix { get; set; }

        [StringLength(20)]
        [Column(Order = 5)]
        public string? DocumentReference { get; set; }

		[Column(Order = 6)]
        public int StoreId { get; set; }

        [Column(Order = 7)]
        public int? SupplierId { get; set; }

		[Column(Order = 8)]
        public DateTime EntryDate { get; set; }

		[DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 9)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DocumentDate { get; set; }

        [StringLength(50)]
        [Column(Order = 10)]
        public string? PeerReference { get; set; }

		[Column(Order = 11)]
		[StringLength(50)]
        public string? PaymentReference { get; set; }

        [Column(Order = 12)]
        public int? SellerId { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 13)]
        public decimal TotalDebitValue { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 14)]
        public decimal TotalDebitValueAccount { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 15)]
        public decimal TotalCreditValue { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 16)]
        public decimal TotalCreditValueAccount { get; set; }

        [Column(Order = 17)]
        [StringLength(500)]
        public string? RemarksAr { get; set; }

        [Column(Order = 18)]
        [StringLength(500)]
        public string? RemarksEn { get; set; }

		[Column(Order = 19)]
        public int? JournalHeaderId { get; set; }

		[Column(Order = 20)]
        public int? ArchiveHeaderId { get; set; }


		[ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }

        [ForeignKey(nameof(SupplierId))]
        public Supplier? Supplier { get; set; }

		[ForeignKey(nameof(SellerId))]
        public Seller? Seller { get; set; }

        [ForeignKey(nameof(JournalHeaderId))]
        public JournalHeader? JournalHeader { get; set; }

		[ForeignKey(nameof(ArchiveHeaderId))]
        public ArchiveHeader? ArchiveHeader { get; set; }
    }
}
