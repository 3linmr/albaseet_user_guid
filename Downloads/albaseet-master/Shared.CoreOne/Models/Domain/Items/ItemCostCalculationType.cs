using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Items
{
    public class ItemCostCalculationType : BaseObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public byte ItemCostCalculationTypeId { get; set; }

        [Required, StringLength(50)]
        public string? ItemCostCalculationTypeNameAr { get; set; }

        [Required, StringLength(50)]
        public string? ItemCostCalculationTypeNameEn { get; set; }
    }
}
