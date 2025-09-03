using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.ViewModels
{
    public class ItemAutoCompleteDto
    {
        public int? ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemNameAr { get; set; }
        public string? ItemNameEn { get; set; }
        public string? ItemName { get; set; }
        public string? BarCode { get; set; }
        public int? ItemPackageId { get; set; }
        public string? ItemPackageName { get; set; }
        public decimal ConsumerPrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal CostPackage { get; set; }
        public decimal LastPurchasePrice { get; set; }
        public decimal LastSalesPrice { get; set; }
        public decimal Packing { get; set; }
        public int TaxId { get; set; }
        public byte TaxTypeId { get; set; }
		public byte ItemTypeId { get; set; }
		public bool IsItemVatInclusive { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public int FromWhere { get; set; }
        public List<int>? Packages { get; set; }
        public List<ItemPricesDto>? ItemPrices { get; set; }
        public List<ItemTaxDataDto>? ItemTaxData { get; set; }
    }

    public class ItemPricesDto
    {
        public string? BarCode { get; set; }
        public decimal ConsumerPrice { get; set; }
    }


    public class ItemPricesAutoCompleteDto
    {
        public decimal CostPrice { get; set; }
        public decimal CostPackage { get; set; }
        public decimal LastPurchasePrice { get; set; }
        public decimal Packing { get; set; }
        public List<ItemPricesDto> ItemPrices { get; set; } = new List<ItemPricesDto>();
    }

    public class ItemCostPriceValueDto
    {
        public int ItemId { get; set; }
        public int ItemPackageId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal CostPackage { get; set; }
    }

    public class ItemCardDto
    {
	    public int StoreId { get; set; }
	    public string? StoreName { get; set; }
	    public int? ItemId { get; set; }
	    public string? ItemCode { get; set; }
	    public string? ItemName { get; set; }
	    public string? BarCode { get; set; }
	    public int? ItemPackageId { get; set; }
	    public string? ItemPackageName { get; set; }
	    public decimal ConsumerPrice { get; set; }
	    public decimal CostPrice { get; set; }
	    public decimal Packing { get; set; }
	    public decimal CostPackage { get; set; }
	    public decimal LastPurchasePrice { get; set; }
	    public decimal LastCostPrice { get; set; }
	}
}
