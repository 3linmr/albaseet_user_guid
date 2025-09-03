using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Inventory
{
	public class ItemCostDto
	{
		public int ItemCostId { get; set; }
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
		public int ItemId { get; set; }
		public string? ItemCode { get; set; }
		public int ItemPackageId { get; set; }
		public decimal Packing { get; set; }
		public string? ItemPackageName { get; set; }
		public string? ItemName { get; set; }
		public decimal CostPrice { get; set; } //Actual Average Price
		public decimal LastPurchasePrice { get; set; }
		public decimal LastCostPrice { get; set; }
		public DateTime? LastUpdateDate { get; set; }
		public string? LastUserNameUpdate { get; set; }
	}

	
	public class ItemCostVm
	{
		public int ItemId { get; set; }
		public decimal ItemPacking { get; set; } = 1;
		public decimal ItemInvoiceQuantity { get; set; }
		public decimal ItemInvoicePrice { get; set; }
		public decimal ItemExpenseValue { get; set; }
	}

	public class CalculateItemCost
	{
		public int StoreId { get; set; }
		public int CurrentPurchaseInvoiceHeaderId { get; set; }
		public List<ItemCostVm> Items { get; set; } = new List<ItemCostVm>();
	}

}
