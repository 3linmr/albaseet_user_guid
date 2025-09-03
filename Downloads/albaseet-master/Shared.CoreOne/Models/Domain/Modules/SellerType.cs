using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Modules
{
    public class SellerType : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte SellerTypeId { get; set; }

        [Column(Order = 2)]
        [StringLength(20)]
        public string? SellerTypeNameAr { get; set; }

        [Column(Order = 3)]
        [StringLength(20)]
        public string? SellerTypeNameEn { get; set; }
    }
}
