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
    public class Currency : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CurrencyId { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 2)]
        public string? CurrencyNameAr { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 3)]
        public string? CurrencyNameEn { get; set; }

        [Column(Order = 4)]
        [StringLength(10)]
        public string? IsoCode { get; set; }

        [StringLength(10)]
        [Column(Order = 5)]
        public string? Symbol { get; set; }

        [StringLength(50)]
        [Column(Order = 6)]
        public string? FractionalUnitAr { get; set; }

        [StringLength(50)]
        [Column(Order = 7)]
        public string? FractionalUnitEn { get; set; }

        [Column(Order = 8)]
        public short NumberToBasic { get; set; }
    }
}
