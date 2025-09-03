using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Shared
{
    public class GeneralInventoryDocumentDto
    {
		public int StoreId { get; set; }
		public int ItemId { get; set; }
		public int ItemPackageId { get; set; }
		public DateTime? ExpireDate { get; set; }
		public string? BatchNumber { get; set; }
		public DateTime? DocumentDate { get; set; }
		public string? BarCode { get; set; }
		public string? ItemNote { get; set; }
		public decimal OpenQuantity { get; set; }
		public decimal InQuantity { get; set; }
		public decimal OutQuantity { get; set; }
		public decimal PendingInQuantity { get; set; }
		public decimal PendingOutQuantity { get; set; }
		public decimal ReservedQuantity { get; set; }
    }

    public class GeneralInventoryDocumentWithMenuCodeDto
    {
		public int HeaderId { get; set; }
		public short MenuCode { get; set; }
		public string? DocumentFullCode { get; set; }
		public int StoreId { get; set; }
		public int? ClientId { get; set; }
		public int? SupplierId { get; set; }
		public int? SellerId { get; set; }
		public int ItemId { get; set; }
		public int ItemPackageId { get; set; }
		public DateTime? ExpireDate { get; set; }
		public string? BatchNumber { get; set; }
		public DateTime DocumentDate { get; set; }
		public DateTime? EntryDate { get; set; }
		public string? BarCode { get; set; }
		public int? CostCenterId { get; set; }
		public string? ItemNote { get; set; }
		public decimal StoredQuantity { get; set; }
		public decimal OpenQuantity { get; set; }
		public decimal InQuantity { get; set; }
		public decimal OutQuantity { get; set; }
		public decimal PendingInQuantity { get; set; }
		public decimal PendingOutQuantity { get; set; }
		public decimal ReservedQuantity { get; set; }
		public decimal CostPrice { get; set; }
		public decimal? CostPackage { get; set; }
		public decimal? Price { get; set; }
		public string? Reference { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public string? Notes { get; set; }
		public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }

	   	public decimal PurchaseInvoicesQuantity { get; set; }
	   	public decimal SalesInvoiceReturnQuantity { get; set; }
	   	public decimal StockInQuantity { get; set; }
		public decimal StockOutReturnQuantity { get; set; }
	   	public decimal InternalTransferInQuantity { get; set; }
	   	public decimal InventoryInQuantity { get; set; }
	   	public decimal CarryOverInQuantity { get; set; }
		public decimal DisassembleInQuantity { get; set; }

	   	public decimal SalesInvoicesQuantity { get; set; }
	   	public decimal PurchaseInvoiceReturnQuantity { get; set; }
	   	public decimal StockOutQuantity { get; set; }
		public decimal StockInReturnQuantity { get; set; }
		public decimal InternalTransferOutQuantity { get; set; }
	   	public decimal InventoryOutQuantity { get; set; }
	   	public decimal CarryOverOutQuantity { get; set; }
		public decimal DisassembleOutQuantity { get; set; }
    }
}
