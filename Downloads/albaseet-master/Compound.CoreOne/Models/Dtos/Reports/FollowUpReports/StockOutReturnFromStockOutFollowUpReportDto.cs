namespace Compound.CoreOne.Models.Dtos.Reports.FollowUpReports
{
	public class StockOutReturnFromStockOutFollowUpReportVm
	{
		public StockOutReturnFromStockOutFollowUpReportDto? StockOutReturnFromStockOutFollowUpReportDto { get; set; }
		public StockOutReturnFromStockOutFollowUpDetailReportDto? StockOutReturnFromStockOutFollowUpDetailReportDto { get; set; }
	}

    public class StockOutReturnFromStockOutFollowUpReportDto
    {
        public int StockOutReturnHeaderId { get; set; }
        public short? MenuCode { get; set; }
        public string? MenuName { get; set; }
        public string? DocumentFullCode { get; set; }
        public DateTime DocumentDate { get; set; }
        public decimal Price { get; set; }
        public int ClientId { get; set; }
        public string? ClientName { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public string? SalesInvoiceInterim { get; set; }
        public string? ReceiptVoucher { get; set; }
        public string? StockOutReturnFromSalesInvoice { get; set; }
        public string? SalesInvoiceReturn { get; set; }
        public string? ClientDebitMemo { get; set; }
        public string? ClientCreditMemo { get; set; }
        public string? Reference { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? UserNameCreated { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
    }

    public class StockOutReturnFromStockOutFollowUpDetailReportDto
    {
		public int StockOutReturnHeaderId { get; set; }
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

		public decimal SalesInvoiceInterimQuantity { get; set; }
		public string? SalesInvoiceInterimDocumentFullCodes { get; set; }
		public decimal StockOutReturnFromSalesInvoiceQuantity { get; set; }
		public string? StockOutReturnFromSalesInvoiceDocumentFullCodes { get; set; }
		public decimal SalesInvoiceReturnQuantity { get; set; }
		public string? SalesInvoiceReturnDocumentFullCodes { get; set; }

		public string? Reference { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }

		public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
    }
}
