using Shared.CoreOne;
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
    public class InternalTransferReceiveDetail : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InternalTransferReceiveDetailId { get; set; }

        [Column(Order = 2)]
        public int InternalTransferReceiveHeaderId { get; set; }

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
        public DateTime? ExpireDate { get; set; }

        [Column(Order = 14)]
        [StringLength(50)]
        public string? BatchNumber { get; set; }


		[ForeignKey(nameof(InternalTransferReceiveHeaderId))]
        public InternalTransferReceiveHeader? InternalTransferReceiveHeader { get; set; }

        [ForeignKey(nameof(ItemPackageId))]
        public ItemPackage? Package { get; set; }

        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }
    }
}
