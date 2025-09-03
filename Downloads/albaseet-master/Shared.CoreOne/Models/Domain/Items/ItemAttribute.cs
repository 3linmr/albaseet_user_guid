using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Items
{
    public class ItemAttribute : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemAttributeId { get; set; }

        [Column(Order = 2)]
        public int ItemAttributeTypeId { get; set; }

        [Column(Order = 3)]
        public int ItemId { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 4)]
        public string? ItemAttributeNameAr { get; set; }

        [Required, StringLength(100)]
        [Column(Order = 5)]
        public string? ItemAttributeNameEn { get; set; }


        [ForeignKey(nameof(ItemAttributeTypeId))]
        public ItemAttributeType? ItemAttributeType { get; set; }

        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }
    }
}
