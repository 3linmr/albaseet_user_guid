using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Items
{
	public class ItemVm
	{
		public ItemDto Item { get; set; } = new ItemDto();
		public List<ItemBarCodeDto>? BarCodes { get; set; } = new List<ItemBarCodeDto>();
		public List<ItemBarCodeDetailDto>? BarCodesDetails { get; set; } = new List<ItemBarCodeDetailDto>();
		public List<ItemAttributeDto>? Attributes { get; set; } = new List<ItemAttributeDto>();
		public List<ItemTaxDto> ItemTaxes { get; set; } = new List<ItemTaxDto>();
		public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();
	}

	public class ItemDto
	{
		public int ItemId { get; set; }

		public string? ItemCode { get; set; }

		public string? ItemName { get; set; }
		public string? ItemNameAr { get; set; }

		public string? ItemNameEn { get; set; }

		public int CompanyId { get; set; }
		public string? CompanyName { get; set; }

		public int? ItemCategoryId { get; set; }
		public string? ItemCategoryName { get; set; }

		public int? ItemSubCategoryId { get; set; }
		public string? ItemSubCategoryName { get; set; }

		public int? ItemSectionId { get; set; }
		public string? ItemSectionName { get; set; }

		public int? ItemSubSectionId { get; set; }
		public string? ItemSubSectionName { get; set; }

		public int? MainItemId { get; set; }
		public string? MainItemName { get; set; }

		public int? VendorId { get; set; }
		public string? VendorName { get; set; }

		public byte ItemTypeId { get; set; }
		public string? ItemTypeName { get; set; }

		public byte TaxTypeId { get; set; }
		public string? TaxTypeName { get; set; }

		public string? TaxesList { get; set; }


		public int SingularPackageId { get; set; }
		public string? SingularPackageName { get; set; }


		public decimal PurchasingPrice { get; set; }  //سعر الشراء

		public decimal ConsumerPrice { get; set; }  //سعر المستهلك

		public decimal InternalPrice { get; set; }  //السعر الداخلي - Transfer Price

		public decimal MaxDiscountPercent { get; set; }

		public int? SalesAccountId { get; set; }  //Sales

		public int? PurchaseAccountId { get; set; }  //Purchases

		public decimal MinBuyQuantity { get; set; }

		public decimal MinSellQuantity { get; set; }

		public decimal MaxBuyQuantity { get; set; }

		public decimal MaxSellQuantity { get; set; }

		public decimal ReorderPointQuantity { get; set; } //حد الأمان

		public decimal CoverageQuantity { get; set; } // تغطية المخزون

		public bool IsActive { get; set; }

		public string? InActiveReasons { get; set; }

		public bool NoReplenishment { get; set; }

		public bool IsUnderSelling { get; set; }

		public bool IsNoStock { get; set; }

		public bool IsUntradeable { get; set; }

		public bool IsDeficit { get; set; }

		public bool IsPos { get; set; }

		public bool IsOnline { get; set; }

		public bool IsPoints { get; set; }

		public bool IsGifts { get; set; }

		public bool IsPromoted { get; set; }

		public bool IsExpired { get; set; }

		public bool IsBatched { get; set; }

		public string? ItemLocation { get; set; }

		public bool? IsApproved { get; set; }

		public int? CurrentStepId { get; set; }
		public string? CurrentStepName { get; set; }

		public int? CurrentStatusId { get; set; }
		public string? CurrentStatusName { get; set; }

		public int? LastStepId { get; set; }
		public string? LastStepName { get; set; }

		public int? LastStatusId { get; set; }
		public string? LastStatusName { get; set; }

		public int? ArchiveHeaderId { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? ModifiedAt { get; set; }

		public string? UserNameCreated { get; set; }

		public string? UserNameModified { get; set; }
	}

	public class ItemAutoCompleteVm
	{
		public int ItemId { get; set; }
		public string? ItemCode { get; set; }
		public string? ItemName { get; set; }
	}

	public class ItemTaxDto
	{
		public int ItemTaxId { get; set; }
		public int ItemId { get; set; }
		public string? ItemName { get; set; }
		public int TaxId { get; set; }
		public string? TaxName { get; set; }
	}

	public class ItemTaxDataDto
	{
		public int ItemId { get; set; }
		public int TaxId { get; set; }
		public byte TaxTypeId { get; set; }
		public decimal TaxPercent { get; set; }
		public int? DebitAccountId { get; set; }
		public int? CreditAccountId { get; set; }
		public bool TaxAfterVatInclusive { get; set; }
	}

	public struct IncompleteItemDto
	{
		public required int ItemId { get; set; }
		public required string? ItemName { get; set; }
		public required int ItemPackageId { get; set; }
		public required string? ItemPackageName { get; set; }
		public required bool Partial { get; set; }
	}
}
