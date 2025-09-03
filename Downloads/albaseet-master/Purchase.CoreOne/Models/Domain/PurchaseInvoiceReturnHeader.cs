using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Taxes;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Menus;

namespace Purchases.CoreOne.Models.Domain
{
	public class PurchaseInvoiceReturnHeader : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int PurchaseInvoiceReturnHeaderId { get; set; }

		[StringLength(10)]
		[Column(Order = 2)]
		public string? Prefix { get; set; }

		[Column(Order = 3)]
		public int DocumentCode { get; set; }

		[StringLength(10)]
		[Column(Order = 4)]
		public string? Suffix { get; set; }

		[Column(Order = 5)]
		public int PurchaseInvoiceHeaderId { get; set; }
		
		[StringLength(20)]
		[Column(Order = 6)]
		public string? DocumentReference { get; set; }

		[Column(Order = 7)]
		public int SupplierId { get; set; }

		[Column(Order = 8)]
		public int StoreId { get; set; }

		[DataType(DataType.Date)]
		[Column(TypeName = "Date", Order = 9)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime DocumentDate { get; set; }

		[Column(Order = 10)]
		public DateTime EntryDate { get; set; }

		[Column(Order = 11)]
		[StringLength(20)]
		public string? Reference { get; set; }

		[Column(Order = 12)]
		public bool IsDirectInvoice { get; set; }

        [Column(Order = 13)]
        public bool CreditPayment { get; set; }

        [Column(Order = 14)]
		public byte TaxTypeId { get; set; }

		[Column(Order = 15, TypeName = "decimal(30,15)")]
		public decimal TotalValue { get; set; }

		[Column(Order = 16, TypeName = "decimal(30,15)")]
		public decimal DiscountPercent { get; set; }

		[Column(Order = 17, TypeName = "decimal(30,15)")]
		public decimal DiscountValue { get; set; }

		[Column(Order = 18, TypeName = "decimal(30,15)")]
		public decimal TotalItemDiscount { get; set; }

		[Column(Order = 19, TypeName = "decimal(30,15)")]
		public decimal GrossValue { get; set; }

		[Column(Order = 20, TypeName = "decimal(30,15)")]
		public decimal VatValue { get; set; }

		[Column(Order = 21, TypeName = "decimal(30,15)")]
		public decimal SubNetValue { get; set; }

		[Column(Order = 22, TypeName = "decimal(30,15)")]
		public decimal OtherTaxValue { get; set; }

		[Column(Order = 23, TypeName = "decimal(30,15)")]
		public decimal NetValueBeforeAdditionalDiscount { get; set; }

		[Column(Order = 24, TypeName = "decimal(30,15)")]
		public decimal AdditionalDiscountValue { get; set; }

		[Column(Order = 25, TypeName = "decimal(30,15)")]
		public decimal NetValue { get; set; }

        [Column(Order = 26, TypeName = "decimal(30,15)")]
        public decimal TotalCostValue { get; set; }

        [Column(Order = 27)]
		public int DebitAccountId { get; set; }

		[Column(Order = 28)]
		public int CreditAccountId { get; set; }

		[Column(Order = 29)]
		public int JournalHeaderId { get; set; }

		[Column(Order = 30)]
		[StringLength(2000)]
		public string? RemarksAr { get; set; }

		[Column(Order = 31)]
		[StringLength(2000)]
		public string? RemarksEn { get; set; }

        [Column(Order = 32)]
		public bool IsOnTheWay { get; set; }

		[Column(Order = 33)]
		public bool IsClosed { get; set; }

        [Column(Order = 34)]
        public bool IsEnded { get; set; }

        [Column(Order = 35)]
        public bool IsBlocked { get; set; }

        [Column(Order = 36)]
		public short? MenuCode { get; set; }

		[Column(Order = 37)]
		public int? ArchiveHeaderId { get; set; }



		[ForeignKey(nameof(StoreId))]
		public Store? Store { get; set; }

		[ForeignKey(nameof(SupplierId))]
		public Supplier? Supplier { get; set; }

		[ForeignKey(nameof(PurchaseInvoiceHeaderId))]
		public PurchaseInvoiceHeader? PurchaseInvoiceHeader { get; set; }

		[ForeignKey(nameof(TaxTypeId))]
		public TaxType? TaxType { get; set; }

        [ForeignKey(nameof(DebitAccountId))]
        public Account? DebitAccount { get; set; }

        [ForeignKey(nameof(CreditAccountId))]
        public Account? CreditAccount { get; set; }

        [ForeignKey(nameof(JournalHeaderId))]
		public JournalHeader? JournalHeader { get; set; }

		[ForeignKey(nameof(ArchiveHeaderId))]
		public ArchiveHeader? ArchiveHeader { get; set; }

		[ForeignKey(nameof(MenuCode))]
		public Menu? Menu { get; set; }
	}
}
