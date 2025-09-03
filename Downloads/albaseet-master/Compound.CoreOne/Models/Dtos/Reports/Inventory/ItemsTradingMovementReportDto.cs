using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Inventory
{
	public class ItemsTradingMovementReportDto
	{
		public int HeaderId { get; set; }
		public string? DocumentFullCode { get; set; }
		public short? MenuCode { get; set; }
		public string? MenuName { get; set; }
		public DateTime DocumentDate { get; set; }
		public DateTime EntryDate { get; set; }
		public int InvoiceTypeId { get; set; }
		public int? ClientId { get; set; }
		public int? ClientCode { get; set; }
		public string? ClientName { get; set; }
		public int? SellerId { get; set; }
		public int? SellerCode { get; set; }
		public string? SellerName { get; set; }
		public string? Reference { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public int? CostCenterId { get; set; }
		public string? CostCenterCode { get; set; }
		public string? CostCenterName { get; set; }
		public int? VatTaxId { get; set; }
		public int? VatTaxCode { get; set; }
		public string? VatTaxName { get; set; }
		public int? VatTaxAccountId { get; set; }
		public string? VatTaxAccountCode { get; set; }
		public string? VatTaxAccountName { get; set; }
		public string? OtherTaxAccounts { get; set; }
		public string? InvoiceTypeName { get; set; }
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
		public int ItemId { get; set; }
		public string? ItemCode { get; set; }
		public string? ItemName { get; set; }
		public string? ItemNameAr { get; set; }
		public string? ItemNameEn { get; set; }
		public int ItemTypeId { get; set; }
		public string? ItemTypeName { get; set; }
		public int ItemPackageId { get; set; }
		public int ItemPackageCode { get; set; }
		public string? ItemPackageName { get; set; }
		public DateTime? ExpireDate { get; set; }
		public string? BatchNumber { get; set; }
		public decimal Price { get; set; }
		public decimal Quantity { get; set; }
		public decimal BonusQuantity { get; set; }
		public decimal PurchaseQuantity { get; set; }
		public decimal SellingQuantity { get; set; }
		public decimal TotalValue { get; set; }
		public decimal ItemDiscountPercent { get; set; }
		public decimal ItemDiscountValue { get; set; }
		public decimal TotalValueAfterDiscount { get; set; }
		public decimal HeaderDiscountPercent { get; set; }
		public decimal HeaderDiscountValue { get; set; }
		public decimal GrossValue { get; set; }
		public decimal PurchaseValue { get; set; }
		public decimal SalesValue { get; set; }
		public decimal VatPercent { get; set; }
		public decimal VatValue { get; set; }
		public decimal SubNetValue { get; set; }
		public decimal OtherTaxValue { get; set; }
		public decimal NetValue { get; set; }
		public decimal ItemExpensePercent { get; set; }
		public decimal ItemExpenseValue { get; set; }
		public string? Notes { get; set; }
		public decimal ConsumerPrice { get; set; }
		public decimal CostPrice { get; set; }
		public decimal CostPackage { get; set; }
		public decimal Packing { get; set; }
		public decimal CostValue { get; set; }
		public decimal SalesProfit { get; set; }

		public decimal ReorderPointQuantity { get; set; }

		public int? VendorId { get; set; }
		public int? VendorCode { get; set; }
		public string? VendorName { get; set; }

		public string? TaxTypeName { get; set; }

		public decimal PurchasingPrice { get; set; }
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

		public string? ItemAttributes { get; set; }
		public string? ItemMenuNotes { get; set; }

		public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
	}

	public class ItemsTradingMovementDataDto
	{
		public int HeaderId { get; set; }
		public string? DocumentFullCode { get; set; }
		public short? MenuCode { get; set; }
		public int TableId { get; set; }
		public DateTime DocumentDate { get; set; }
		public DateTime EntryDate { get; set; }
		public int InvoiceTypeId { get; set; }
		public int? ClientId { get; set; }
		public int? SellerId { get; set; }
		public string? Reference { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public int? CostCenterId { get; set; }
		public int? VatTaxId { get; set; }
		public int? VatTaxCode { get; set; }
		public string? VatTaxName { get; set; }
		public int? VatTaxAccountId { get; set; }
		public string? VatTaxAccountCode { get; set; }
		public string? VatTaxAccountName { get; set; }
		public int StoreId { get; set; }
		public int ItemId { get; set; }
		public int ItemTypeId { get; set; }
		public int ItemPackageId { get; set; }
		public string? ItemPackageName { get; set; }
		public DateTime? ExpireDate { get; set; }
		public string? BatchNumber { get; set; }
		public decimal Price { get; set; }
		public decimal Quantity { get; set; }
		public decimal BonusQuantity { get; set; }
		public decimal PurchaseQuantity { get; set; }
		public decimal SellingQuantity { get; set; }
		public decimal TotalValue { get; set; }
		public decimal ItemDiscountPercent { get; set; }
		public decimal ItemDiscountValue { get; set; }
		public decimal TotalValueAfterDiscount { get; set; }
		public decimal HeaderDiscountPercent { get; set; }
		public decimal HeaderDiscountValue { get; set; }
		public decimal GrossValue { get; set; }
		public decimal PurchaseValue { get; set; }
		public decimal SalesValue { get; set; }
		public decimal VatPercent { get; set; }
		public decimal VatValue { get; set; }
		public decimal SubNetValue { get; set; }
		public decimal OtherTaxValue { get; set; }
		public decimal NetValue { get; set; }
		public decimal ItemExpensePercent { get; set; }
		public decimal ItemExpenseValue { get; set; }
		public string? Notes { get; set; }
		public string? ItemNote { get; set; }
		public decimal Packing { get; set; }
		public decimal ConsumerPrice { get; set; }
		public decimal CostPrice { get; set; }
		public decimal CostPackage { get; set; }
		public decimal CostValue { get; set; } //تكلفة المبيعات
		public decimal SalesProfit { get; set; } //ارباح المبيعات
		public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
	}

	//This class has to use value equality and not reference equality
	public class DetailTaxKey : IEquatable<DetailTaxKey>
	{
		public int HeaderId { get; set; }
		public int TableId { get; set; }
		public int ItemId { get; set; }
		public int ItemPackageId { get; set; }
		public DateTime? ExpireDate { get; set; }
		public string? BatchNumber { get; set; }
		public int? CostCenterId { get; set; }
		public decimal? ItemDiscountPercent { get; set; }
		public decimal Price { get; set; }
		public string? ItemNote { get; set; }

		public override bool Equals(object? obj)
		{
			return Equals(obj as DetailTaxKey);
		}

		public bool Equals(DetailTaxKey? other)
		{
			if (other is null) return false;

			return HeaderId == other.HeaderId &&
				   TableId == other.TableId &&
				   ItemId == other.ItemId &&
				   ItemPackageId == other.ItemPackageId &&
				   ExpireDate == other.ExpireDate &&
				   BatchNumber == other.BatchNumber &&
				   CostCenterId == other.CostCenterId &&
				   ItemDiscountPercent == other.ItemDiscountPercent &&
				   Price == other.Price &&
				   ItemNote == other.ItemNote;
		}

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(HeaderId);
            hash.Add(TableId);
            hash.Add(ItemId);
            hash.Add(ItemPackageId);
            hash.Add(ExpireDate);
            hash.Add(BatchNumber);
            hash.Add(CostCenterId);
            hash.Add(ItemDiscountPercent);
            hash.Add(Price);
            hash.Add(ItemNote);
            return hash.ToHashCode();
        }
	}
}
