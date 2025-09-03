using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Menus;

namespace Shared.CoreOne.Models.Domain.Archive
{
    public class ArchiveHeader : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int ArchiveHeaderId { get; set; }

        [Column(Order = 2)]
        public short MenuCode { get; set; }

        [Required,StringLength(100)]
        [Column(Order = 3)]
        public string? HeaderNameAr { get; set; }
        
        [Required, StringLength(100)]
        [Column(Order = 4)]
        public string? HeaderNameEn { get; set; }



        [ForeignKey(nameof(MenuCode))]
        public Menu? Menu { get; set; }
    }
}
