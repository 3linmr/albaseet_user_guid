using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Settings
{
    public class ApplicationFlagDetailSelect : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ApplicationFlagDetailSelectId { get; set; }

        [Column(Order = 2)]
        public int ApplicationFlagDetailId { get; set; }

        [Column(Order = 3)]
        public short SelectId { get; set; }

        [Column(Order = 4)]
        [StringLength(200)]
        public string? SelectNameAr { get; set; }

        [Column(Order = 5)]
        [StringLength(200)]
        public string? SelectNameEn { get; set; }

        [Column(Order = 6)]
        public short Order { get; set; }


        [ForeignKey(nameof(ApplicationFlagDetailId))]
        public ApplicationFlagDetail? ApplicationFlagDetail { get; set; }
    }
}
