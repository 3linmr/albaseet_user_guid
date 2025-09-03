using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.CoreOne.Models.Domain.Items;

namespace Sales.CoreOne.Models.Domain
{
    public class ClientPriceRequestDetail : BaseObject
    {
        [Column(Order = 1)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ClientPriceRequestDetailId { get; set; }

        [Column(Order = 2)]
        public int ClientPriceRequestHeaderId { get; set; }

        [Column(Order = 3)]
        public int ItemId { get; set; }

        [Column(Order = 4)]
        public int ItemPackageId { get; set; }

        [Column(Order = 5)]
        public bool IsItemVatInclusive { get; set; }

		[Column(Order = 6)]
        public int? CostCenterId { get; set; }

        [Column(Order = 7)]
        [StringLength(200)]
        public string? BarCode { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 8)]
        public decimal Packing { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 9)]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 10)]
        public decimal ConsumerPrice { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 11)]
        public decimal ConsumerValue { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 12)]
        public decimal CostPrice { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 13)]
        public decimal CostPackage { get; set; }

        [Column(TypeName = "decimal(30,15)", Order = 14)]
        public decimal CostValue { get; set; }

        [StringLength(2000)]
        [Column(Order = 15)]
        public string? Notes { get; set; }

        [StringLength(2000)]
        [Column(Order = 16)]
        public string? ItemNote { get; set; }



		[ForeignKey(nameof(ClientPriceRequestHeaderId))]
        public ClientPriceRequestHeader? ClientPriceRequestHeader { get; set; }

        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }

        [ForeignKey(nameof(ItemPackageId))]
        public ItemPackage? ItemPackage { get; set; }

        [ForeignKey(nameof(CostCenterId))]
        public CostCenter? CostCenter { get; set; }
    }
}
