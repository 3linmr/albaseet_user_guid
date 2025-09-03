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
	public class StockTakingCarryOverDetail : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int StockTakingCarryOverDetailId { get; set; }

		[Column(Order = 2)]
		public int StockTakingCarryOverHeaderId { get; set; }

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
		public decimal StockTakingQuantity { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 8)]
		public decimal CurrentQuantity { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 9)]
		public decimal StockTakingConsumerPrice { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 10)]
		public decimal CurrentConsumerPrice { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 11)]
		public decimal StockTakingConsumerValue { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 12)]
		public decimal CurrentConsumerValue { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 13)]
		public decimal StockTakingCostPrice { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 14)]
		public decimal CurrentCostPrice { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 15)]
		public decimal StockTakingCostPackage { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 16)]
		public decimal CurrentCostPackage { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 17)]
		public decimal StockTakingCostValue { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 18)]
		public decimal CurrentCostValue { get; set; }

		[DataType(DataType.Date)]
		[Column(TypeName = "Date", Order = 19)]
		public DateTime? ExpireDate { get; set; }

		[Column(Order = 20)]
		[StringLength(50)]
		public string? BatchNumber { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 21)]
		public decimal OpenQuantity { get; set; }		
		
		[Column(TypeName = "decimal(30,15)", Order = 22)]
		public decimal OldOpenQuantity { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 23)]
		public decimal InQuantity { get; set; }

		[Column(TypeName = "decimal(30,15)", Order = 24)]
		public decimal OutQuantity { get; set; }


		[ForeignKey(nameof(StockTakingCarryOverHeaderId))]
		public StockTakingCarryOverHeader? StockTakingCarryOverHeader { get; set; }

		[ForeignKey(nameof(ItemId))]
		public Item? Item { get; set; }

		[ForeignKey(nameof(ItemPackageId))]
		public ItemPackage? Package { get; set; }
	}
}
