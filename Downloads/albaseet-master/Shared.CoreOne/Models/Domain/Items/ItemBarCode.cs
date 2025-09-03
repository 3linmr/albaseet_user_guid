using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Items
{
    public class ItemBarCode : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemBarCodeId { get; set; }

        [Column(Order = 2)]
        public int FromPackageId { get; set; }

        [Column(Order = 3)]
        public int ToPackageId { get; set; }

        [Column(Order = 4)]
        public int ItemId { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 5)]
        public decimal Packing { get; set; }

        [Column(Order = 6)]
        public bool IsSingularPackage { get; set; }

        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }

        [ForeignKey(nameof(FromPackageId))]
        public ItemPackage? FromPackage { get; set; }

        [ForeignKey(nameof(ToPackageId))]
        public ItemPackage? ToPackage { get; set; }
	}
}
