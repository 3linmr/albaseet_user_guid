using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Modules
{
    public class Vendor : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int VendorId { get; set; }

        [Column(Order = 2)]
        public int VendorCode { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 3)]
        public string? VendorNameAr { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 4)]
        public string? VendorNameEn { get; set; }

        [Column(Order = 5)]
        public int CompanyId { get; set; }


        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
    }
}
