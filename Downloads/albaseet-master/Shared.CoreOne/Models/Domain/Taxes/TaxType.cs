using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Taxes
{
    public class TaxType : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte TaxTypeId { get; set; }

        [Column(Order = 2)]
        [Required, StringLength(50)]
        public string? TaxTypeNameAr { get; set; }


        [Column(Order = 3)]
        [Required, StringLength(50)]
        public string? TaxTypeNameEn { get; set; }
    }
}
