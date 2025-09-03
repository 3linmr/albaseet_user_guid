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
    public class InvoiceStockOut : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InvoiceStockOutId { get; set; }

        [Column(Order = 2)]
        public int SalesInvoiceHeaderId { get; set; }

        [Column(Order = 3)]
        public int StockOutHeaderId { get; set; }



        [ForeignKey(nameof(SalesInvoiceHeaderId))]
        public SalesInvoiceHeader? SalesInvoiceHeader { get; set; }

        [ForeignKey(nameof(StockOutHeaderId))]
        public StockOutHeader? StockOutHeader { get; set; }
    }
}
