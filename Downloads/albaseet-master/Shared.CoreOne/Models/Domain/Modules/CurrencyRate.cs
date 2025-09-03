using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;

namespace Shared.CoreOne.Models.Domain.Modules

{
    public class CurrencyRate : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CurrencyRateId { get; set; }

        [Column(Order = 2)]
        public short FromCurrencyId { get; set; }

        [Column(Order = 3)]
        public short ToCurrencyId { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 4)]
        public decimal CurrencyRateValue { get; set; }


        [ForeignKey(nameof(FromCurrencyId))]
        public Currency? FromCurrency { get; set; } 
        
        [ForeignKey(nameof(ToCurrencyId))]
        public Currency? ToCurrency { get; set; }
    }
}
