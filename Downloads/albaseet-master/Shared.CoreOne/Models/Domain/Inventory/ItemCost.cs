using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Inventory
{
    public class ItemCost : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int ItemCostId { get; set; }

        [Column(Order = 2)]
        public int StoreId { get; set; }

        [Column(Order = 3)]
        public int ItemId { get; set; }

        [Column(Order = 4)]
        public int ItemPackageId { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 5)]
        public decimal CostPrice { get; set; } //Actual Average Price

        [Column(TypeName = "decimal(30,15)", Order = 6)]
        public decimal LastPurchasePrice { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 7)]
        public decimal LastCostPrice { get; set; }



		[ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }

        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }

		[ForeignKey(nameof(ItemPackageId))]
		public ItemPackage? ItemPackage { get; set; }
	}
}
