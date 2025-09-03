using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne.Models;

namespace Compound.CoreOne.Models.Domain.InvoiceSettlement
{
	public class SalesInvoiceSettlement : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int SalesInvoiceSettlementId { get; set; }

		[Column(Order = 2)]
		public int SalesInvoiceHeaderId { get; set; }

		[Column(Order = 3)]
		public int ReceiptVoucherHeaderId { get; set; }

		[Column(Order = 4, TypeName = "decimal(30,15)")]
		public decimal SettleValue { get; set; }

		[Column(Order = 5)]
		[StringLength(500)]
		public string? RemarksAr { get; set; }

		[Column(Order = 6)]
		[StringLength(500)]
		public string? RemarksEn { get; set; }


		[ForeignKey(nameof(SalesInvoiceHeaderId))]
		public SalesInvoiceHeader? SalesInvoiceHeader { get; set; }

		[ForeignKey(nameof(ReceiptVoucherHeaderId))]
		public ReceiptVoucherHeader? ReceiptVoucherHeader { get; set; }
	}
}