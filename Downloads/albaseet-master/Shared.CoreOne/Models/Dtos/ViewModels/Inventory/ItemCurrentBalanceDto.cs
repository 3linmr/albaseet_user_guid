using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Inventory;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Inventory
{
	public class ItemCurrentBalanceDto
	{
		public int ItemCurrentBalanceId { get; set; }
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
		public int ItemId { get; set; }
		public string ItemCode { get; set; } = "";
		public string? ItemName { get; set; }
		public byte ItemTypeId { get; set; }
		public DateTime? ExpireDate { get; set; }
		public string? BatchNumber { get; set; }
		public int ItemPackageId { get; set; }
		public string? ItemPackageName { get; set; }
		public DateTime? OpenDate { get; set; }
		public string? BarCode { get; set; }
		public decimal ConsumerPrice { get; set; }
		public decimal OpenQuantity { get; set; }
		public decimal InQuantity { get; set; }
		public decimal OutQuantity { get; set; }
		public decimal PendingInQuantity { get; set; }
		public decimal PendingOutQuantity { get; set; }
		public decimal CurrentBalance { get; set; }
		public decimal AvailableBalance { get; set; }


		//public int SingularPackageId { get; set; }
		//public string? SingularPackageName { get; set; }
	}

	public class ItemBalanceDto
	{
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
		public int ItemId { get; set; }
		public string ItemCode { get; set; } = "";
		public string? ItemName { get; set; }
		public DateTime? ExpireDate { get; set; }
		public string? BatchNumber { get; set; }
		public int ItemPackageId { get; set; }
		public bool IsSingularPackage { get; set; }
		public string? ItemPackageName { get; set; }
		public string? BarCode { get; set; }
		public int ItemTypeId { get; set; }
		public int TaxId { get; set; }
		public int TaxTypeId { get; set; }
		public decimal ConsumerPrice { get; set; }
		public decimal OpenQuantity { get; set; }
		public decimal InQuantity { get; set; }
		public decimal OutQuantity { get; set; }
		public decimal PendingInQuantity { get; set; }
		public decimal PendingOutQuantity { get; set; }
		public decimal CurrentBalance { get; set; }
		public decimal AvailableBalance { get; set; }
		public decimal CostPrice { get; set; }
		public decimal Packing { get; set; }
		public decimal CostPackage { get; set; }
		public decimal LastPurchasePrice { get; set; }
		public decimal LastCostPrice { get; set; }
		public decimal LastSalesPrice { get; set; }
	}

	public class ItemPackageTreeDto
	{
		public int ItemId { get; set; }
		public string? ItemName { get; set; }
		public int ItemPackageId { get; set; }
		public int? MainItemPackageId { get; set; }
		public string? ItemPackageName { get; set; }
		public decimal ItemPackageBalanceBefore { get; set; }
		public decimal ItemPackageBalanceAfter { get; set; }
		public string? ItemPackageNameWithBalanceBefore { get; set; }
		public string? ItemPackageNameWithBalanceAfter { get; set; }
	}
}
