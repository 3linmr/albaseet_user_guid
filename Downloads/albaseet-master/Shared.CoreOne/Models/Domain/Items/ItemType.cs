using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Items
{
    public class ItemType : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte ItemTypeId { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 2)]
        public string? ItemTypeNameAr { get; set; }

        [Required, StringLength(50)]
        [Column(Order = 3)]
        public string? ItemTypeNameEn { get; set; }
    }
}
