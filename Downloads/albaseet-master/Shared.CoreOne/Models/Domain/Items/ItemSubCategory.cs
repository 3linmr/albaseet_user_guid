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
    public class ItemSubCategory : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemSubCategoryId { get; set; }

        [Column(Order = 2)]
        public int ItemSubCategoryCode { get; set; }

        [Column(Order = 3)]
        public int ItemCategoryId { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 4)]
        public string? SubCategoryNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 5)]
        public string? SubCategoryNameEn { get; set; }

        [Column(Order = 6)]
        public int CompanyId { get; set; }


        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
        
        [ForeignKey(nameof(ItemCategoryId))]
        public ItemCategory? ItemCategory { get; set; }
    }
}
