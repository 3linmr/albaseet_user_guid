using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Inventory
{
    public class ItemCostUpdateDetail : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemCostUpdateDetailId { get; set; }

        [Column(Order = 2)]
        public int ItemCostUpdateHeaderId { get; set; }

        [Column(Order = 3)]
        public int ItemId { get; set; }

        [Column(Order = 4)]
        public int ItemPackageId { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 5)]
        public decimal OldCostPrice { get; set; } //Actual Average Price

        [Column(TypeName = "decimal(30,15)", Order = 6)]
        public decimal NewCostPrice { get; set; } //Actual Average Price

        [Column(TypeName = "decimal(30,15)", Order = 7)]
        public decimal OldLastPurchasePrice { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 8)]
        public decimal NewLastPurchasePrice { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 9)]
        public decimal OldLastCostPrice { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 10)]
        public decimal NewLastCostPrice { get; set; }


        [ForeignKey(nameof(ItemCostUpdateHeaderId))]
        public ItemCostUpdateHeader? ItemCostUpdateHeader { get; set; }

        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }

        [ForeignKey(nameof(ItemPackageId))]
        public ItemPackage? ItemPackage { get; set; }
    }
}
