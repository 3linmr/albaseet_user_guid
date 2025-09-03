using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.CostCenters;

namespace Purchases.CoreOne.Models.Domain
{
    public class PurchaseOrderDetail : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int PurchaseOrderDetailId { get; set; }

        [Column(Order = 2)]
        public int PurchaseOrderHeaderId { get; set; }

        [Column(Order = 3)]
        public int? CostCenterId { get; set; }

        [Column(Order = 4)]
        public int ItemId { get; set; }
        
        [Column(Order = 5)]
        public int ItemPackageId { get; set; }

        [Column(Order = 6)]
        public bool IsItemVatInclusive { get; set; }

		[Column(Order = 7)]
        [StringLength(200)]
        public string? BarCode { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 8)]
        public decimal Packing { get; set; }

		[Column(Order = 9, TypeName = "decimal(30,15)")]
        public decimal Quantity { get; set; }
        
        [Column(Order = 10, TypeName = "decimal(30,15)")]
        public decimal BonusQuantity { get; set; }
        
        [Column(Order = 11, TypeName = "decimal(30,15)")]
        public decimal PurchasePrice { get; set; }

		[Column(Order = 12, TypeName = "decimal(30,15)")]
		public decimal TotalValue { get; set; }

		[Column(Order = 13, TypeName = "decimal(30,15)")]
		public decimal ItemDiscountPercent { get; set; }

		[Column(Order = 14, TypeName = "decimal(30,15)")]
		public decimal ItemDiscountValue { get; set; }

		[Column(Order = 15, TypeName = "decimal(30,15)")]
		public decimal TotalValueAfterDiscount { get; set; }

		[Column(Order = 16, TypeName = "decimal(30,15)")]
		public decimal HeaderDiscountValue { get; set; }

		[Column(Order = 17, TypeName = "decimal(30,15)")]
		public decimal GrossValue { get; set; }

		[Column(Order = 18, TypeName = "decimal(30,15)")]
		public decimal VatPercent { get; set; }

		[Column(Order = 19, TypeName = "decimal(30,15)")]
		public decimal VatValue { get; set; }

		[Column(Order = 20, TypeName = "decimal(30,15)")]
		public decimal SubNetValue { get; set; }

		[Column(Order = 21, TypeName = "decimal(30,15)")]
		public decimal OtherTaxValue { get; set; }

		[Column(Order = 22, TypeName = "decimal(30,15)")]
		public decimal NetValue { get; set; }

		[Column(Order = 23)]
        [StringLength(2000)]
        public string? Notes { get; set; }

        [Column(Order = 24, TypeName = "decimal(30,15)")]
        public decimal ConsumerPrice { get; set; }

        [Column(Order = 25, TypeName = "decimal(30,15)")]
        public decimal CostPrice { get; set; }

        [Column(Order = 26, TypeName = "decimal(30,15)")]
        public decimal CostPackage { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 27)]
        public decimal CostValue { get; set; }

        [Column(Order = 28, TypeName = "decimal(30,15)")]
        public decimal LastPurchasePrice { get; set; }

        [StringLength(2000)]
        [Column(Order = 29)]
        public string? ItemNote { get; set; }


		[ForeignKey(nameof(PurchaseOrderHeaderId))]
        public PurchaseOrderHeader? PurchaseOrderHeader { get; set; }

        [ForeignKey(nameof(CostCenterId))]
        public CostCenter? CostCenter { get; set; }

        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }

        [ForeignKey(nameof(ItemPackageId))]
        public ItemPackage? ItemPackage { get; set; }
    }
}
