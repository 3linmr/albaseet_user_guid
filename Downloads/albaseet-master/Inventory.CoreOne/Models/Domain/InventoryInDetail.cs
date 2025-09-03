using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Items;

namespace Inventory.CoreOne.Models.Domain
{
    public class InventoryInDetail : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InventoryInDetailId { get; set; }

        [Column(Order = 2)]
        public int InventoryInHeaderId { get; set; }

        [Column(Order = 3)]
        public int ItemId { get; set; }

        [Column(Order = 4)]
        public int ItemPackageId { get; set; }

        [Column(Order = 5)]
        [StringLength(200)]
		public string? BarCode { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 6)]
        public decimal Packing { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 7)]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 8)]
        public decimal ConsumerPrice { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 9)]
        public decimal ConsumerValue { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 10)]
        public decimal CostPrice { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 11)]
        public decimal CostPackage { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 12)]
        public decimal CostValue { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date", Order = 13)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ExpireDate { get; set; }

        [StringLength(50)]
        [Column(Order = 14)]
        public string? BatchNumber { get; set; }

        [Column(Order = 15)]
        public bool? IsLinkedToCostCenters { get; set; }

        [Column(Order = 16)]
        public bool? IsCostCenterDistributed { get; set; }



		[ForeignKey(nameof(InventoryInHeaderId))]
        public InventoryInHeader? InventoryInHeader { get; set; }

        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }

        [ForeignKey(nameof(ItemPackageId))]
        public ItemPackage? ItemPackage { get; set; }
    }
}
