using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Archive;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Taxes;

namespace Purchases.CoreOne.Models.Domain
{
	public class SupplierQuotationHeader : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int SupplierQuotationHeaderId { get; set; }

		[StringLength(10)]
		[Column(Order = 2)]
		public string? Prefix { get; set; }

		[Column(Order = 3)]
		public int DocumentCode { get; set; }

		[StringLength(10)]
		[Column(Order = 4)]
		public string? Suffix { get; set; }

		[Column(Order = 5)]
		public int? ProductRequestPriceHeaderId { get; set; }

		[Column(Order = 6)]
		public int SupplierId { get; set; }

		[StringLength(20)]
		[Column(Order = 7)]
		public string? DocumentReference { get; set; }

		[Column(Order = 8)]
		public int StoreId { get; set; }

		[Column(Order = 9)]
		public byte TaxTypeId { get; set; }

		[DataType(DataType.Date)]
		[Column(TypeName = "Date", Order = 10)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime DocumentDate { get; set; }

		[Column(Order = 11)]
		public DateTime EntryDate { get; set; }

		[Column(Order = 12)]
		[StringLength(20)]
		public string? Reference { get; set; }

		[Column(Order = 13, TypeName = "decimal(30,15)")]
		public decimal TotalValue { get; set; }

		[Column(Order = 14, TypeName = "decimal(30,15)")]
		public decimal DiscountPercent { get; set; }

		[Column(Order = 15, TypeName = "decimal(30,15)")]
		public decimal DiscountValue { get; set; }

		[Column(Order = 16, TypeName = "decimal(30,15)")]
		public decimal TotalItemDiscount { get; set; }

		[Column(Order = 17, TypeName = "decimal(30,15)")]
		public decimal GrossValue { get; set; }

		[Column(Order = 18, TypeName = "decimal(30,15)")]
		public decimal VatValue { get; set; }

		[Column(Order = 19, TypeName = "decimal(30,15)")]
		public decimal SubNetValue { get; set; }

		[Column(Order = 20, TypeName = "decimal(30,15)")]
		public decimal OtherTaxValue { get; set; }

		[Column(Order = 21, TypeName = "decimal(30,15)")]
		public decimal NetValueBeforeAdditionalDiscount { get; set; }

		[Column(Order = 22, TypeName = "decimal(30,15)")]
		public decimal AdditionalDiscountValue { get; set; }

		[Column(Order = 23, TypeName = "decimal(30,15)")]
		public decimal NetValue { get; set; }

        [Column(Order = 24, TypeName = "decimal(30,15)")]
        public decimal TotalCostValue { get; set; }

        [StringLength(2000)]
		[Column(Order = 25)]
		public string? RemarksAr { get; set; }

		[StringLength(2000)]
		[Column(Order = 26)]
		public string? RemarksEn { get; set; }

		[Column(Order = 27)]
		public bool IsClosed { get; set; }

		[Column(Order = 28)]
		public int? ArchiveHeaderId { get; set; }


		[ForeignKey(nameof(ProductRequestPriceHeaderId))]
		public ProductRequestPriceHeader? ProductRequestPriceHeader { get; set; }

		[ForeignKey(nameof(SupplierId))]
		public Supplier? Supplier { get; set; }

		[ForeignKey(nameof(StoreId))]
		public Store? Store { get; set; }

		[ForeignKey(nameof(TaxTypeId))]
		public TaxType? TaxType { get; set; }

		[ForeignKey(nameof(ArchiveHeaderId))]
		public ArchiveHeader? ArchiveHeader { get; set; }
	}
}
