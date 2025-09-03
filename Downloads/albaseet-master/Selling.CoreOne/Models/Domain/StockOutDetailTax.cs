using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Taxes;

namespace Sales.CoreOne.Models.Domain
{
    public class StockOutDetailTax : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StockOutDetailTaxId { get; set; }

        [Column(Order = 2)]
        public int StockOutDetailId { get; set; }

        [Column(Order = 3)]
        public int TaxId { get; set; }

        [Column(Order = 4)]
        public bool TaxAfterVatInclusive { get; set; } //احتساب الضريبة علي المبلغ شامل الضرائب (Value + VAT)

		[Column(Order = 5)]
        public int CreditAccountId { get; set; }

        [Column(Order = 6, TypeName = "decimal(30,15)")]
        public decimal TaxPercent { get; set; }

        [Column(Order = 7, TypeName = "decimal(30,15)")]
        public decimal TaxValue { get; set; }  //CreditValue



        [ForeignKey(nameof(StockOutDetailId))]
        public StockOutDetail? StockOutDetail { get; set; }


        [ForeignKey(nameof(TaxId))]
        public Tax? Tax { get; set; }
    }
}
