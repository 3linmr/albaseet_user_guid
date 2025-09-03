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
	public class InvoiceStockInReturn : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int InvoiceStockInReturnId { get; set; }

		[Column(Order = 2)]
		public int StockInReturnHeaderId { get; set; }

		[Column(Order = 3)]
		public int PurchaseInvoiceReturnHeaderId { get; set; }



		[ForeignKey(nameof(PurchaseInvoiceReturnHeaderId))]
		public PurchaseInvoiceReturnHeader? PurchaseInvoiceReturnHeader { get; set; }

		[ForeignKey(nameof(StockInReturnHeaderId))]
		public StockInReturnHeader? StockInReturnHeader { get; set; }
	}
}
