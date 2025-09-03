using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Settings
{
    public class ApplicationFlagType : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte ApplicationFlagTypeId { get; set; }

        [StringLength(200)]
        [Column(Order = 2)]
        public string? ApplicationFlagTypeNameAr { get; set; }

        [StringLength(200)]
        [Column(Order = 3)]
        public string? ApplicationFlagTypeNameEn { get; set; }
    }
}
