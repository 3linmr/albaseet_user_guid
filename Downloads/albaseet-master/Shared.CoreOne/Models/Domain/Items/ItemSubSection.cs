using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Items
{
    public class ItemSubSection : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemSubSectionId { get; set; }

        [Column(Order = 2)]
        public int ItemSubSectionCode { get; set; }

        [Column(Order = 3)]
        public int ItemSectionId { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 4)]
        public string? SubSectionNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 5)]
        public string? SubSectionNameEn { get; set; }

        [Column(Order = 6)]
        public int CompanyId { get; set; }


        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
        
        [ForeignKey(nameof(ItemSectionId))]
        public ItemSection? ItemSection { get; set; }
    }
}
