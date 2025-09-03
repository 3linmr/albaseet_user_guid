namespace Compound.CoreOne.Models.Dtos.Reports.FollowUpReports
{
	public class ProductRequestPriceFollowUpReportVm
	{
		public ProductRequestPriceFollowUpReportDto? ProductRequestPriceFollowUpReportDto { get; set; }
		public ProductRequestPriceFollowUpDetailReportDto? ProductRequestPriceFollowUpDetailReportDto { get; set; }
	}

    public class ProductRequestPriceFollowUpReportDto
    {
        public int ProductRequestPriceHeaderId { get; set; }
        public short? MenuCode { get; set; }
        public string? MenuName { get; set; }
        public string? DocumentFullCode { get; set; }
        public DateTime DocumentDate { get; set; }
        public decimal Price { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public string? SupplierQuotation { get; set; }
        public string? PurchaseOrder { get; set; }
        public string? StockInFromPurchaseOrder { get; set; }
        public string? StockInReturnFromStockIn { get; set; }
        public string? PurchaseInvoiceInterim { get; set; }
        public string? PaymentVoucher { get; set; }
        public string? StockInReturnFromPurchaseInvoice { get; set; }
        public string? PurchaseInvoiceReturn { get; set; }
        public string? SupplierDebitMemo { get; set; }
        public string? SupplierCreditMemo { get; set; }
        public string? Reference { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? UserNameCreated { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
    }

    public class ProductRequestPriceFollowUpDetailReportDto
    {
		public int ProductRequestPriceHeaderId { get; set; }
		public short MenuCode { get; set; }
		public string? MenuName { get; set; }
		public string? DocumentFullCode { get; set; }
		public DateTime? DocumentDate { get; set; }
		public int StoreId { get; set; }
		public string? StoreName { get; set; }

		public string? BarCode { get; set; }
		public int ItemId { get; set; }
		public string? ItemCode { get; set; }
		public string? ItemName { get; set; }
		public int ItemPackageId { get; set; }
		public int ItemPackageCode { get; set; }
		public string? ItemPackageName { get; set; }
		public decimal Packing { get; set; }
		public decimal Quantity { get; set; }
		public int? CostCenterId { get; set; }
		public string? CostCenterCode { get; set; }
		public string? CostCenterName { get; set; }
		public decimal PackagePrice { get; set; }
		public decimal NetValue { get; set; }
		public decimal CostPrice { get; set; }
		public decimal CostPackage { get; set; }
		public decimal CostValue { get; set; }

		public decimal SupplierQuotationQuantity { get; set; }
		public string? SupplierQuotationDocumentFullCodes { get; set; }
		public decimal PurchaseOrderQuantity { get; set; }
		public string? PurchaseOrderDocumentFullCodes { get; set; }
		public decimal StockInFromPurchaseOrderQuantity { get; set; }
		public string? StockInFromPurchaseOrderDocumentFullCodes { get; set; }
		public decimal StockInReturnFromStockInQuantity { get; set; }
		public string? StockInReturnFromStockInDocumentFullCodes { get; set; }
		public decimal PurchaseInvoiceInterimQuantity { get; set; }
		public string? PurchaseInvoiceInterimDocumentFullCodes { get; set; }
		public decimal StockInReturnFromPurchaseInvoiceQuantity { get; set; }
		public string? StockInReturnFromPurchaseInvoiceDocumentFullCodes { get; set; }
		public decimal PurchaseInvoiceReturnQuantity { get; set; }
		public string? PurchaseInvoiceReturnDocumentFullCodes { get; set; }

		public string? Reference { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }

		public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
    }
}
