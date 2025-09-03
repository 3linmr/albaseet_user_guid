using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Helper.Logic;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.CoreOne.Models.Domain.Items;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Inventory
{

	public class ItemDisassembleVm
	{
		public ItemDisassembleHeaderDto? ItemDisassembleHeader { get; set; }
		public List<ItemDisassembleDetailDto> ItemDisassembleDetails { get; set; } = [];
		public List<ItemDisassembleSerialDto> ItemDisassembleSerial { get; set; } = [];
	}

	public class ItemDisassembleInfoDto
	{
		public ItemConversionDto? ItemDisassemble { get; set; }
		public List<ItemPackageTreeDto> ItemDisassembleSerial { get; set; } = [];
	}

	public class ItemDisassembleHeaderDto
	{
		public int ItemDisassembleHeaderId { get; set; }

		public int ItemDisassembleCode { get; set; }

		public int StoreId { get; set; }

		public string? StoreName { get; set; }

		public DateTime DocumentDate { get; set; }

		public DateTime EntryDate { get; set; }

		public bool IsAutomatic { get; set; }

		public short? MenuCode { get; set; }

		public string? MenuCodeName { get; set; }

		public int? ReferenceHeaderCode { get; set; }

		public int? ReferenceDetailCode { get; set; }

		public string? RemarksAr { get; set; }

		public string? RemarksEn { get; set; }

		public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
		public string? UserNameCreated { get; set; }
		public string? IpAddressCreated { get; set; }

		public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
		public string? IpAddressModified { get; set; }
	}

	public class ItemDisassembleDetailDto
	{
		public int ItemDisassembleDetailId { get; set; }

		public int ItemDisassembleHeaderId { get; set; }

		public bool IsSerialConversion { get; set; }

		public int ItemId { get; set; }

		public string? ItemName { get; set; }

		public int FromPackageId { get; set; }
		public string? FromPackageName { get; set; }

		public int ToPackageId { get; set; }
		public string? ToPackageName { get; set; }

		public DateTime? ExpireDate { get; set; }

		public string? BatchNumber { get; set; }

		public decimal Packing { get; set; }

		public decimal Quantity { get; set; }

		public decimal FromPackageQuantityBefore { get; set; }

		public decimal ToPackageQuantityBefore { get; set; }

		public decimal FromPackageQuantityAfter { get; set; }

		public decimal ToPackageQuantityAfter { get; set; }
	}

	public class ItemDisassembleSerialDto
	{
		public int ItemDisassembleSerialId { get; set; }
		public int ItemDisassembleHeaderId { get; set; }

		public int ItemId { get; set; }
		public string? ItemName { get; set; }

		public int ItemPackageId { get; set; }
		public string? ItemPackageName { get; set; }

		public int? MainItemPackageId { get; set; }
		public string? MainItemPackageName { get; set; }

		public decimal ItemPackageBalanceBefore { get; set; }

		public decimal ItemPackageBalanceAfter { get; set; }
	}

	public class ItemDisassembleDto
	{
		public int ItemDisassembleId { get; set; }
		public int ItemDisassembleHeaderId { get; set; }

		public int StoreId { get; set; }
		public string? StoreName { get; set; }

		public int ItemId { get; set; }
		public string? ItemCode { get; set; }
		public string? ItemName { get; set; }

		public DateTime EntryDate { get; set; }

		public int ItemPackageId { get; set; }
		public string? ItemPackageName { get; set; }

		public decimal InQuantity { get; set; }

		public decimal OutQuantity { get; set; }

		public DateTime? ExpireDate { get; set; }

		public string? BatchNumber { get; set; }

		public string? UserNameCreated { get; set; }
	}

	public class ItemConversionDto
	{
		public int ItemDisassembleHeaderId { get; set; }
		public int ItemDisassembleCode { get; set; }
		public int StoreId { get; set; }
		public int ItemId { get; set; }
		public string? ItemName { get; set; }
		public bool IsSerialConversion { get; set; }
		public int FromPackageId { get; set; }
		public string? FromPackageName { get; set; }
		public int ToPackageId { get; set; }
		public string? ToPackageName { get; set; }
		public string? BatchNumber { get; set; }
		public DateTime? ExpireDate { get; set; }
		public decimal Quantity { get; set; }
		public decimal Packing { get; set; }

		public decimal FromPackageQuantityBefore { get; set; }

		public decimal ToPackageQuantityBefore { get; set; }

		public decimal FromPackageQuantityAfter { get; set; }

		public decimal ToPackageQuantityAfter { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }

		public string? StoreName { get; set; }

		public DateTime DocumentDate { get; set; }

		public DateTime EntryDate { get; set; }

		public bool IsAutomatic { get; set; }

		public short? MenuCode { get; set; }

		public string? MenuCodeName { get; set; }

		public int? ReferenceHeaderCode { get; set; }

		public int? ReferenceDetailCode { get; set; }

		public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
		public string? UserNameCreated { get; set; }
		public string? IpAddressCreated { get; set; }
	}

	public class ItemPackageConversionDto
	{
		public int ItemId { get; set; }
		public int FromPackageId { get; set; }
		public string? FromPackageName { get; set; }
		public int ToPackageId { get; set; }
		public string? ToPackageName { get; set; }
		public decimal FromPackageAvailableBalance { get; set; }
		public decimal ToPackageAvailableBalance { get; set; }
		public decimal Packing { get; set; }
	}

	public class ItemSerialConversionDto
	{
		public int ItemCurrentBalanceId { get; set; }
		public int StoreId { get; set; }
		public int ItemId { get; set; }
		public bool IsSerialConversion { get; set; }
		public int ItemPackageId { get; set; }
		public string? BatchNumber { get; set; }
		public DateTime? ExpireDate { get; set; }
		public decimal Quantity { get; set; }
		public decimal Packing { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public bool IsFirstLevel { get; set; }
		public bool IsSecondLevel { get; set; }
		public bool IsLastLevel { get; set; }
		public int Level { get; set; }
		public int LevelCount { get; set; }
	}

	public class ItemConversionVm
	{
		public List<ItemCurrentBalanceDto> OutBalances { get; set; } = [];
		public List<ItemCurrentBalanceDto> InBalances { get; set; } = [];
		public List<ItemDisassembleDto> ItemDisassembles { get; set; } = [];
	}
}
