namespace Compound.CoreOne.Models.Dtos.Reports.Suppliers
{
	public class AgeingPurchaseInvoiceReportDto
	{
		public int PurchaseInvoiceHeaderId { get; set; }
		public string? DocumentFullCode { get; set; }
		public short? MenuCode { get; set; }
		public string? MenuName { get; set; }
		public DateTime DocumentDate { get; set; }
		public DateTime EntryDate { get; set; }
		public int SupplierId { get; set; }
		public int SupplierCode { get; set; }
		public string? SupplierName { get; set; }
		public int AccountId { get; set; }
		public string? AccountCode { get; set; }
		public string? AccountName { get; set; }
		public decimal RemainingValue { get; set; }
		public int InvoiceDuration { get; set; }
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
		public int BranchId { get; set; }
		public string? BranchName { get; set; }
		public int CompanyId { get; set; }
		public string? CompanyName { get; set; }
		public decimal InvoiceValue { get; set; }
		public DateTime? DueDate { get; set; }
		public int Age { get; set; }
		public int RemainingDays { get; set; }
		public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
	}
}
