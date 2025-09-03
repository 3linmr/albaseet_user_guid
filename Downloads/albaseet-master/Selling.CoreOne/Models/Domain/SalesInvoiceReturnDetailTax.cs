using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Taxes;

namespace Sales.CoreOne.Models.Domain
{
	public class SalesInvoiceReturnDetailTax : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int SalesInvoiceReturnDetailTaxId { get; set; }

		[Column(Order = 2)]
		public int SalesInvoiceReturnDetailId { get; set; }

		[Column(Order = 3)]
		public byte TaxTypeId { get; set; }

		[Column(Order = 4)]
		public int TaxId { get; set; }

		[Column(Order = 5)]
		public bool TaxAfterVatInclusive { get; set; } //احتساب الضريبة علي المبلغ شامل الضرائب (Value + VAT)

		[Column(Order = 6)]
		public int DebitAccountId { get; set; }

		[Column(Order = 7, TypeName = "decimal(30,15)")]
		public decimal TaxPercent { get; set; }

		[Column(Order = 8, TypeName = "decimal(30,15)")]
		public decimal TaxValue { get; set; } //DebitValue


		[ForeignKey(nameof(SalesInvoiceReturnDetailId))]
		public SalesInvoiceReturnDetail? SalesInvoiceReturnDetail { get; set; }

		[ForeignKey(nameof(TaxTypeId))]
		public TaxType? TaxType { get; set; }

		[ForeignKey(nameof(TaxId))]
		public Tax? Tax { get; set; }

		[ForeignKey(nameof(DebitAccountId))]
		public Account? DebitAccount { get; set; }
	}
}