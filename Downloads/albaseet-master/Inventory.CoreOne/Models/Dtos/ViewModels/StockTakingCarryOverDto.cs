using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Logic;

namespace Inventory.CoreOne.Models.Dtos.ViewModels
{
	public class StockTakingCarryOverDto
	{
		public StockTakingCarryOverHeaderDto StockTakingCarryOverHeader { get; set; } = new StockTakingCarryOverHeaderDto();
		public List<StockTakingCarryOverDetailDto> StockTakingCarryOverDetails{ get; set; } = new List<StockTakingCarryOverDetailDto>();
        public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();
    }

	public class StockTakingCarryOverHeaderDto
	{
		public int StockTakingCarryOverHeaderId { get; set; }
		public string? StockTakingList { get; set; }
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
		public string? Prefix { get; set; }
		public int StockTakingCarryOverCode { get; set; }
		public string? Suffix { get; set; }
		public string? DocumentReference { get; set; }
		public string? StockTakingCarryOverFullCode { get; set; }
		public string? StockTakingCarryOverNameAr { get; set; }
		public string? StockTakingCarryOverNameEn { get; set; }
		public bool IsOpenBalance { get; set; }
		public bool IsAllItemsAffected { get; set; }
		public DateTime DocumentDate { get; set; }
		public DateTime EntryDate { get; set; } = DateHelper.GetDateTimeNow();
		public string? Reference { get; set; }
		public decimal TotalCurrentBalanceConsumerValue { get; set; }
		public decimal TotalStockTakingConsumerValue { get; set; }
		public decimal TotalCurrentBalanceCostValue { get; set; }
		public decimal TotalStockTakingCostValue { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
	}

	public class StockTakingCarryOverDetailDto
	{
		public int StockTakingCarryOverDetailId { get; set; }
		public int StockTakingCarryOverHeaderId { get; set; }
		public int ItemId { get; set; }
		public string? ItemCode { get; set; }
		public string? ItemName { get; set; }
		public byte? ItemTypeId { get; set; }
		public int ItemPackageId { get; set; }
		public string? ItemPackageName { get; set; }
		public string? BarCode { get; set; }
		public decimal Packing { get; set; }
		public decimal StockTakingQuantity { get; set; }
		public decimal CurrentQuantity { get; set; }
		public decimal StockTakingConsumerPrice { get; set; }
		public decimal CurrentConsumerPrice { get; set; }
		public decimal CurrentConsumerValue { get; set; }
		public decimal StockTakingConsumerValue { get; set; }
		public decimal ItemCostPrice { get; set; }
		public decimal StockTakingCostPrice { get; set; }
		public decimal CurrentCostPrice { get; set; }
		public decimal StockTakingCostPackage { get; set; }
		public decimal CurrentCostPackage { get; set; }
		public decimal StockTakingCostValue { get; set; }
		public decimal CurrentCostValue { get; set; }
		public DateTime? ExpireDate { get; set; }
		public string? BatchNumber { get; set; }
		public decimal OpenQuantity { get; set; }
		public decimal OldOpenQuantity { get; set; }
		public decimal InQuantity { get; set; }
		public decimal OutQuantity { get; set; }

		public decimal DifferenceBetweenStockTakingAndCurrent { get; set; }
		public decimal DifferenceBetweenStockTakingConsumerAndCurrent { get; set; }
		public decimal DifferenceBetweenStockTakingCostAndCurrent { get; set; }
	}

	public class StockTakingCarryOverEffectDetailDto
	{
		public int StockTakingCarryOverEffectDetailId { get; set; }
		public int StockTakingCarryOverHeaderId { get; set; }
		public int ItemId { get; set; }
		public int ItemPackageId { get; set; }
		public DateTime? ExpireDate { get; set; }
		public string? BatchNumber { get; set; }
		public decimal OpenQuantity { get; set; }
		public decimal OldOpenQuantity { get; set; }
		public decimal InQuantity { get; set; }
		public decimal OutQuantity { get; set; }
	}
}
