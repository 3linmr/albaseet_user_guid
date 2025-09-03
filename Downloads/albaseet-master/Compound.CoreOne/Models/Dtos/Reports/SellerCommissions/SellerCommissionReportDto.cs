using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.SellerCommissions
{
    public class SellerCommissionReportDto
    {
        public int HeaderId { get; set; }
        public short? MenuCode { get; set; }
        public string? MenuName { get; set; }
        public string? DocumentFullCode { get; set; }
        public int? SalesInvoiceHeaderId { get; set; }
        public short? SalesInvoiceMenuCode { get; set; }
        public string? SalesInvoiceMenuName { get; set; }
        public string? SalesInvoiceFullCode { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime EntryDate { get; set; }

        public int SellerId { get; set; }
        public int SellerCode { get; set; }
        public string? SellerName { get; set; }

	    public int SellerCommissionMethodId { get; set; }
	    public int SellerCommissionMethodCode { get; set; }
	    public string? SellerCommissionMethodName { get; set; }

        public decimal CollectedValue { get; set; } //Used to calculate the commissionValues

		public decimal TotalCashIncome { get; set; }
		public int? AgeOfDebt { get; set; }
		public int? SellerCommissionId { get; set; }
		public string? CommissionRange { get; set; }
		public string? CommissionPercent { get; set; }
		public decimal? CommissionValue { get; set; }

        public decimal? InvoiceValue { get; set; }
        public decimal? InvoiceSettledValue { get; set; }
        public decimal? SettleValue { get; set; }

        public int ClientId { get; set; }
        public int ClientCode { get; set; }
        public string? ClientName { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public string? Reference { get; set; }
        public byte? InvoiceTypeId { get; set; }
        public string? InvoiceTypeName { get; set; }
        public DateTime? DueDate { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }

		public string? SellerTypeName { get; set; }
		public string? SellerNameAr { get; set; }
		public string? SellerNameEn { get; set; }
		public DateTime ContractDate { get; set; }
		public string? OutSourcingCompany { get; set; }
		public string? Phone { get; set; }
		public string? WhatsApp { get; set; }
		public string? Address { get; set; }
		public string? Email { get; set; }
		public decimal ClientsDebitLimit { get; set; }
		public bool IsActive { get; set; }
		public string? IsActiveName { get; set; }
		public string? InActiveReasons { get; set; }

		public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
    }

    public class SellerCashIncomeDto
    {
        public int SellerId { get; set; }
        public decimal CashIncome { get; set; }
    }
}
