using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Modules
{
    public class ShipmentType : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ShipmentTypeId { get; set; }

        [Column(Order = 2)]
        public int ShipmentTypeCode { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 3)]
        public string? ShipmentTypeNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 4)]
        public string? ShipmentTypeNameEn { get; set; }

        [Column(Order = 5)]
		public int? CompanyId { get; set; }


        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
    }
}
