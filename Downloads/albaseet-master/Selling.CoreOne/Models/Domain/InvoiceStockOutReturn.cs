using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;

namespace Sales.CoreOne.Models.Domain
{
    public class InvoiceStockOutReturn : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InvoiceStockOutReturnId { get; set; }

        [Column(Order = 2)]
        public int StockOutReturnHeaderId { get; set; }

		[Column(Order = 3)]
        public int SalesInvoiceReturnHeaderId { get; set; }



        [ForeignKey(nameof(SalesInvoiceReturnHeaderId))]
        public SalesInvoiceReturnHeader? SalesInvoiceReturnHeader { get; set; }

        [ForeignKey(nameof(StockOutReturnHeaderId))]
        public StockOutReturnHeader? StockOutReturnHeader { get; set; }
    }
}
