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
    public class ItemSection : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemSectionId { get; set; }

        [Column(Order = 2)]
        public int ItemSectionCode { get; set; }

        [Column(Order = 3)]
        public int ItemSubCategoryId { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 4)]
        public string? SectionNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 5)]
        public string? SectionNameEn { get; set; }

        [Column(Order = 6)]
        public int CompanyId { get; set; }


        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
        
        [ForeignKey(nameof(ItemSubCategoryId))]
        public ItemSubCategory? ItemSubCategory { get; set; }
    }
}
