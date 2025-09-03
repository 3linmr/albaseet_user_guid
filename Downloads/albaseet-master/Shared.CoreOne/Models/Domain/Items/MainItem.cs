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
    public class MainItem : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MainItemId { get; set; }

        [Column(Order = 2)]
        public int MainItemCode { get; set; }

        [Column(Order = 3)]
        public int ItemSubSectionId { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 4)]
        public string? MainItemNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 5)]
        public string? MainItemNameEn { get; set; }

        [Column(Order = 6)]
        public int CompanyId { get; set; }


        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [ForeignKey(nameof(ItemSubSectionId))]
        public ItemSubSection? ItemSubSection { get; set; }
    }
}
