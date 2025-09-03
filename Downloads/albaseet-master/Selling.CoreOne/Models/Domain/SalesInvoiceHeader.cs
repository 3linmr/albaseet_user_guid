using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Taxes;

namespace Sales.CoreOne.Models.Domain
{
    public class SalesInvoiceHeader : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SalesInvoiceHeaderId { get; set; }

        [StringLength(10)]
        [Column(Order = 2)]
        public string? Prefix { get; set; }

        [Column(Order = 3)]
        public int DocumentCode { get; set; }

        [StringLength(10)]
        [Column(Order = 4)]
        public string? Suffix { get; set; }

        [Column(Order = 5)]
        public int ProformaInvoiceHeaderId { get; set; }

        [StringLength(20)]
        [Column(Order = 6)]
        public string? DocumentReference { get; set; }

        [Column(Order = 7)]
        public int ClientId { get; set; }

        [Column(Order = 8)]
        public int? SellerId { get; set; }

		[Column(Order = 9)]
        public int StoreId { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 10)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DocumentDate { get; set; }

        [Column(Order = 11)]
        public DateTime EntryDate { get; set; }

        [Column(Order = 12)]
        [StringLength(20)]
        public string? Reference { get; set; }

        [Column(Order = 13)]
        public bool IsDirectInvoice { get; set; }

        [Column(Order = 14)]
        public bool CreditPayment { get; set; }

        [Column(Order = 15)]
        public byte TaxTypeId { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 16)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ShippingDate { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 17)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DeliveryDate { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 18)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DueDate { get; set; }

        [Column(Order = 19)]
        public int? ShipmentTypeId { get; set; }

        [Column(Order = 20)]
        [StringLength(1000)]
        public string? ClientName { get; set; }

        [Column(Order = 21)]
        [StringLength(500)]
        public string? ClientPhone { get; set; }

        [Column(Order = 22)]
        [StringLength(2000)]
        public string? ClientAddress { get; set; }

        [Column(Order = 23)]
        [StringLength(50)]
        public string? ClientTaxCode { get; set; }

		[Column(Order = 24)]
        [StringLength(500)]
        public string? DriverName { get; set; }

        [Column(Order = 25)]
        [StringLength(500)]
        public string? DriverPhone { get; set; }

        [Column(Order = 26)]
        [StringLength(500)]
        public string? ClientResponsibleName { get; set; }

        [Column(Order = 27)]
        [StringLength(500)]
        public string? ClientResponsiblePhone { get; set; }

        [Column(Order = 28)]
        [StringLength(2000)]
        public string? ShipTo { get; set; }

        [Column(Order = 29)]
        [StringLength(2000)]
        public string? BillTo { get; set; }

        [Column(Order = 30)]
        [StringLength(2000)]
        public string? ShippingRemarks { get; set; }

        [Column(Order = 31, TypeName = "decimal(30,15)")]
        public decimal TotalValue { get; set; }

        [Column(Order = 32, TypeName = "decimal(30,15)")]
        public decimal DiscountPercent { get; set; }

        [Column(Order = 33, TypeName = "decimal(30,15)")]
        public decimal DiscountValue { get; set; }

        [Column(Order = 34, TypeName = "decimal(30,15)")]
        public decimal TotalItemDiscount { get; set; }

        [Column(Order = 35, TypeName = "decimal(30,15)")]
        public decimal GrossValue { get; set; }

        [Column(Order = 36, TypeName = "decimal(30,15)")]
        public decimal VatValue { get; set; }

        [Column(Order = 37, TypeName = "decimal(30,15)")]
        public decimal SubNetValue { get; set; }

        [Column(Order = 38, TypeName = "decimal(30,15)")]
        public decimal OtherTaxValue { get; set; }

        [Column(Order = 39, TypeName = "decimal(30,15)")]
        public decimal NetValueBeforeAdditionalDiscount { get; set; }

		[Column(Order = 40, TypeName = "decimal(30,15)")]
        public decimal AdditionalDiscountValue { get; set; }

        [Column(Order = 41, TypeName = "decimal(30,15)")]
        public decimal NetValue { get; set; }

        [Column(Order = 42, TypeName = "decimal(30,15)")]
        public decimal TotalCostValue { get; set; }

        [Column(Order = 43)]
        public int? DebitAccountId { get; set; }

        [Column(Order = 44)]
        public int CreditAccountId { get; set; }

        [Column(Order = 45)]
        public int JournalHeaderId { get; set; }

        [Column(Order = 46)]
        [StringLength(2000)]
        public string? RemarksAr { get; set; }

        [Column(Order = 47)]
        [StringLength(2000)]
        public string? RemarksEn { get; set; }

        [Column(Order = 48)]
        public int CanReturnInDays { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 49)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CanReturnUntilDate { get; set; }

		[Column(Order = 50)]
        public bool IsOnTheWay { get; set; }

		[Column(Order = 51)]
        public bool IsClosed { get; set; }

        [Column(Order = 52)]
        public bool IsEnded { get; set; }

        [Column(Order = 53)]
        public bool IsBlocked { get; set; }

        [Column(Order = 54)]
        public bool HasSettlement { get; set; }

		[Column(Order = 55)]
        public bool IsSettlementCompleted { get; set; }

		[Column(Order = 56)]
        public short? MenuCode { get; set; }

        [Column(Order = 57)]
        public byte InvoiceTypeId { get; set; }

		[Column(Order = 58, TypeName = "decimal(30,15)")]
		public decimal ClientBalance { get; set; }

		[Column(Order = 59)]
		public int CreditLimitDays { get; set; }

		[Column(Order = 60, TypeName = "decimal(30,15)")]
		public decimal CreditLimitValues { get; set; }
		
		[Column(Order = 61)]
		public int DebitLimitDays { get; set; }

		[Column(Order = 62)]
        public int? ArchiveHeaderId { get; set; }


		[ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }

        [ForeignKey(nameof(SellerId))]
        public Seller? Seller { get; set; }

		[ForeignKey(nameof(ProformaInvoiceHeaderId))]
        public ProformaInvoiceHeader? ProformaInvoiceHeader { get; set; }

        [ForeignKey(nameof(TaxTypeId))]
        public TaxType? TaxType { get; set; }

        [ForeignKey(nameof(ShipmentTypeId))]
        public ShipmentType? ShipmentType { get; set; }

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

        [ForeignKey(nameof(InvoiceTypeId))]
        public InvoiceType? InvoiceType { get; set; }
	}
}
