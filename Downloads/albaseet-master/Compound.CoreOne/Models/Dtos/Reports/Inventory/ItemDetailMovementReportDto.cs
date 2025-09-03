using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Inventory
{
	public class ItemDetailMovementReportDto
	{
		public int Serial { get; set; } //line number
		public int? HeaderId { get; set; }
		public short? MenuCode { get; set; }
		public string? MenuName { get; set; }
		public string? DocumentFullCode { get; set; }
		public int? ClientId { get; set; }
		public int? ClientCode { get; set; }
		public string? ClientName { get; set; }
		public int? SellerId { get; set; }
		public int? SellerCode { get; set; }
		public string? SellerName { get; set; }
		public int? CostCenterId { get; set; }
		public string? CostCenterCode { get; set; }
		public string? CostCenterName { get; set; }
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
		public int ItemId { get; set; }
		public string? ItemCode { get; set; }
		public string? ItemName { get; set; }
		public int ItemTypeId { get; set; }
		public string? ItemTypeName { get; set; }
		public int ItemPackageId { get; set; }
		public int ItemPackageCode { get; set; }
		public string? ItemPackageName { get; set; }
		public DateTime? ExpireDate { get; set; }
		public string? BatchNumber { get; set; }
		public DateTime? DocumentDate { get; set; }
		public DateTime? EntryDate { get; set; }
		public string? BarCode { get; set; }
		public decimal? CostValue { get; set; } //سعر التكلفة اللحظي
		public decimal? Price { get; set; }
		public string? Reference { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public string? Notes { get; set; }
		public decimal? InQuantity { get; set; }
		public decimal? OutQuantity { get; set; }
		public decimal? PendingOutQuantity { get; set; }
		public decimal? ReservedQuantity { get; set; }
		public decimal CurrentBalance { get; set; }
		public decimal AvailableBalance { get; set; }

		public decimal ReorderPointQuantity { get; set; }
  
		public string? TaxTypeName { get; set; }
		public decimal TaxValue { get; set; }

		public int? VendorId { get; set; }
		public int? VendorCode { get; set; }
		public string? VendorName { get; set; }

		public decimal PurchasingPrice { get; set; }
		public decimal ConsumerPrice { get; set; }
		public decimal InternalPrice { get; set; }
		public decimal MaxDiscountPercent { get; set; }
		public int? SalesAccountId { get; set; }
		public string? SalesAccountName { get; set; }
		public int? PurchaseAccountId { get; set; }
		public string? PurchaseAccountName { get; set; }
		public decimal MinBuyQuantity { get; set; }
		public decimal MinSellQuantity { get; set; }
		public decimal MaxBuyQuantity { get; set; }
		public decimal MaxSellQuantity { get; set; }
		public decimal CoverageQuantity { get; set; }
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
		public bool IsPromoted { get; set; }
		public bool IsExpired { get; set; }
		public bool IsBatched { get; set; }
		public string? ItemLocation { get; set; }

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

		public string? OtherTaxes { get; set; }
		public string? ItemAttributes { get; set; }
		public string? ItemMenuNotes { get; set; }

		public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
	}
}
