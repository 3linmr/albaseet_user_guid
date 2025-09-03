using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Taxes
{
    public class TaxPercent : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TaxPercentId { get; set; }

        [Column(Order = 2)]
        public int TaxId { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 3)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FromDate { get; set; }

        [Column(Order = 4, TypeName = "decimal(30,15)")]
        public decimal Percent { get; set; }


        [ForeignKey(nameof(TaxId))]
        public Tax? Tax { get; set; }
    }
}
