using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;

namespace Purchases.CoreOne.Models.Domain
{
	public class InvoiceStockIn : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int InvoiceStockInId { get; set; }

		[Column(Order = 2)]
		public int PurchaseInvoiceHeaderId { get; set; }

		[Column(Order = 3)]
		public int StockInHeaderId { get; set; }



		[ForeignKey(nameof(PurchaseInvoiceHeaderId))]
		public PurchaseInvoiceHeader? PurchaseInvoiceHeader { get; set; }

		[ForeignKey(nameof(StockInHeaderId))]
		public StockInHeader? StockInHeader { get; set; }
	}
}
