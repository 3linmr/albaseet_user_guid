using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Domain.Modules;

namespace Inventory.CoreOne.Models.Domain
{
	public class StockTakingCarryOverHeader : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int StockTakingCarryOverHeaderId { get; set; }

		[Column(Order = 2)]
		[StringLength(10000)]
		public string? StockTakingList { get; set; }
	
		[Column(Order = 3)]
		public int StoreId { get; set; }

		[StringLength(10)]
		[Column(Order = 4)]
		public string? Prefix { get; set; }

		[Column(Order = 5)]
		public int StockTakingCarryOverCode { get; set; }

		[StringLength(10)]
		[Column(Order = 6)]
		public string? Suffix { get; set; }

		[StringLength(20)]
		[Column(Order = 7)]
		public string? DocumentReference { get; set; }

		[Required, StringLength(100)]
		[Column(Order = 8)]
		public string? StockTakingCarryOverNameAr { get; set; }

		[Required, StringLength(100)]
		[Column(Order = 9)]
		public string? StockTakingCarryOverNameEn { get; set; }

		[Column(Order = 10)]
		public bool IsOpenBalance { get; set; }

		[Column(Order = 11)]
		public bool IsAllItemsAffected { get; set; }

		[DataType(DataType.Date)]
		[Column(TypeName = "Date", Order = 12)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime DocumentDate { get; set; }

		[Column(Order = 13)]
		public DateTime EntryDate { get; set; }

		[StringLength(50)]
		[Column(Order = 14)]
		public string? Reference { get; set; }

		[Column(Order = 15, TypeName = "decimal(30,15)")]
		public decimal TotalCurrentBalanceConsumerValue { get; set; }

		[Column(Order = 16, TypeName = "decimal(30,15)")]
		public decimal TotalStockTakingConsumerValue { get; set; }

		[Column(Order = 17, TypeName = "decimal(30,15)")]
		public decimal TotalCurrentBalanceCostValue { get; set; }

		[Column(Order = 18, TypeName = "decimal(30,15)")]
		public decimal TotalStockTakingCostValue { get; set; }

		[Column(Order = 19)]
		[StringLength(500)]
		public string? RemarksAr { get; set; }

		[Column(Order = 20)]
		[StringLength(500)]
		public string? RemarksEn { get; set; }


		[ForeignKey(nameof(StoreId))]
		public Store? Store { get; set; }
	}
}
