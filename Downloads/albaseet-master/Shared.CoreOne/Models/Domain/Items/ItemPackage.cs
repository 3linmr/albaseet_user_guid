using Shared.CoreOne.Models.Domain.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Items
{
    public class ItemPackage : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemPackageId { get; set; }

        [Column(Order = 2)]
        public int? CompanyId { get; set; }

		[Column(Order = 3)]
        public int ItemPackageCode { get; set; }

        [Required, Column(Order = 4)]
        [StringLength(100)]
        public string? PackageNameAr { get; set; }

        [Required, Column(Order = 5)]
        [StringLength(100)]
        public string? PackageNameEn { get; set; }

        [Column(Order = 6)]
        [StringLength(10)]
        public string? PackageCode { get; set; }


        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
    }
}
