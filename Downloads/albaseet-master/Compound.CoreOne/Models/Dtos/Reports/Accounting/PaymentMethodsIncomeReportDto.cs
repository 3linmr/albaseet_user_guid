using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Accounting
{
    public class PaymentMethodsIncomeReportDto 
    {
        public short MenuCode { get; set; }
        public string? MenuName { get; set; }
        public int HeaderId { get; set; }
        public string? DocumentFullCode { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime EntryDate { get; set; }
        public decimal NetValue { get; set; } //مبلغ السند بالكامل
        public decimal PaidValue { get; set; } //مبالغ محصلة
        public decimal ReceivedValue { get; set; } //مبالغ مدفوعة
        public int? ClientId { get; set; }
        public string? ClientName { get; set; }
        public int? SellerId { get; set; }
        public string? SellerName { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }

        public decimal BankAccountPaymentValue { get; set; }
        public decimal BankCardPaymentValue { get; set; }
        public decimal InstallmentPaymentValue { get; set; }
        public decimal CreditTransferPaymentValue { get; set; }
        public decimal CashPaymentValue { get; set; }

        public decimal PaidBankCommissionValue { get; set; }

        public decimal BankAccountPaymentValueAfterCommission { get; set; }
        public decimal BankCardPaymentValueAfterCommission { get; set; }
        public decimal InstallmentPaymentValueAfterCommission { get; set; }
        public decimal CreditTransferPaymentValueAfterCommission { get; set; }
        public decimal CashPaymentValueAfterCommission { get; set; }

        public required string AllPaymentMethodsConcat { get; set; }
        public required string AllPaymentMethodsAfterCommissionConcat { get; set; }
        public decimal NetValueAfterCommission { get; set; }

		public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
    }

    public class PaymentMethodsIncomeReportDataDto
    {
		public int HeaderId { get; set; }
		public short MenuCode { get; set; }
		public string? DocumentFullCode { get; set; }
		public DateTime DocumentDate { get; set; }
		public DateTime EntryDate { get; set; }
		public decimal NetValue { get; set; }
		public int? ClientId { get; set; }
		public int? SellerId { get; set; }
		public int StoreId { get; set; }

        public int PaymentMethodId { get; set; }
        public decimal PaymentValue { get; set; }
        public decimal BankCommissionPaid { get; set; }

        public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
        public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
    }
}
