using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Logic;

namespace Inventory.CoreOne.Models.Dtos.ViewModels
{
	public class StockTakingDto
	{
		public StockTakingHeaderDto? StockTakingHeader { get; set; }
		public List<StockTakingDetailDto> StockTakingDetails { get; set; } = new List<StockTakingDetailDto>();
        public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();
    }

	public class StockTakingHeaderDto
	{
		public int StockTakingHeaderId { get; set; }
		public string? DocumentReference { get; set; }
		public string? Prefix { get; set; }
		public int StockTakingCode { get; set; }
		public string? Suffix { get; set; }
		public string? StockTakingFullCode { get; set; }
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
		public string? StockTakingNameAr { get; set; }
		public string? StockTakingNameEn { get; set; }
		public bool IsOpenBalance { get; set; }
		public DateTime StockDate { get; set; }
		public DateTime EntryDate { get; set; } = DateHelper.GetDateTimeNow();
		public decimal TotalConsumerValue { get; set; }
		public decimal TotalCostValue { get; set; }
		public string? Reference { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public bool IsClosed { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsCarriedOver { get; set; }
	}

	public class StockTakingDetailDto
	{
		public int StockTakingDetailId { get; set; }
		public int StockTakingHeaderId { get; set; }
		public int ItemPackageId { get; set; }
		public string? ItemPackageName { get; set; }
		public int StoreId { get; set; }
		public int ItemId { get; set; }
		public string? ItemCode { get; set; }
		public string? ItemName { get; set; }
		public byte? ItemTypeId { get; set; }
		public decimal Packing { get; set; }
		public decimal Quantity { get; set; }
		public decimal ConsumerPrice { get; set; }
		public decimal ConsumerValue { get; set; }
		public decimal CostPrice { get; set; }
		public decimal CostPackage { get; set; }
		public decimal CostValue { get; set; }
		public string? BarCode { get; set; }
		public DateTime? ExpireDate { get; set; }
		public string? BatchNumber { get; set; }

		public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
		public string? UserNameCreated { get; set; }
		public string? IpAddressCreated { get; set; }

		public string? Packages { get; set; }
	}

	public class StockTakingDropDownDto
	{
		public int StockTakingHeaderId { get; set; }
		public string? StockTakingName { get; set; }
	}
}
