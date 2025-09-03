using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Logic;


namespace Accounting.CoreOne.Models.Dtos.ViewModels
{
	public class PaymentVoucherDto
	{
		public PaymentVoucherHeaderDto? PaymentVoucherHeader { get; set; }
		public List<PaymentVoucherDetailDto> PaymentVoucherDetails { get; set; } = new List<PaymentVoucherDetailDto>();
		public List<CostCenterJournalDetailDto> CostCenterJournalDetails { get; set; } = new List<CostCenterJournalDetailDto>();
		public List<PurchaseInvoiceSettlementDto> PurchaseInvoiceSettlements { get; set; } = new List<PurchaseInvoiceSettlementDto>();
		public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();
	}

	public class PaymentVoucherHeaderDto
	{
		public int PaymentVoucherHeaderId { get; set; }
		public string? Prefix { get; set; }
		public int PaymentVoucherCode { get; set; }
		public string? Suffix { get; set; }
		public string? PaymentVoucherCodeFull { get; set; }
		public string? DocumentReference { get; set; }
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
		public int? SupplierId { get; set; }
		public int? SupplierCode { get; set; }
		public string? SupplierName { get; set; }
		public int StoreCurrencyId { get; set; }
		public DateTime EntryDate { get; set; } = DateHelper.GetDateTimeNow();
		public DateTime DocumentDate { get; set; }
		public string? PeerReference { get; set; }
		public string? PaymentReference { get; set; }
		public int? SellerId { get; set; }
		public int? SellerCode { get; set; }
		public string? SellerName { get; set; }
        public decimal TotalDebitValue { get; set; }
        public decimal TotalDebitValueAccount { get; set; }
        public decimal TotalCreditValue { get; set; }
        public decimal TotalCreditValueAccount { get; set; }
        public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public int? JournalHeaderId { get; set; }
		public int? ArchiveHeaderId { get; set; }
		public DateTime? CreatedAt { get; set; }
		public string? IpAddressCreated { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public string? UserNameModified { get; set; }
	}
	public class PaymentVoucherDetailDto
	{
		public int PaymentVoucherDetailId { get; set; }
		public int PaymentVoucherHeaderId { get; set; }
		public int? PaymentMethodId { get; set; }
		public string? PaymentMethodName { get; set; }
		public int AccountId { get; set; }
		public string? AccountCode { get; set; }
		public string? AccountName { get; set; }
		public short CurrencyId { get; set; }
		public string? CurrencyName { get; set; }
		public decimal CurrencyRate { get; set; }
		public decimal DebitValue { get; set; }
		public decimal CreditValue { get; set; }
		public decimal DebitValueAccount { get; set; }
		public decimal CreditValueAccount { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
		public string? UserNameCreated { get; set; }
		public string? IpAddressCreated { get; set; }
	}

	public class PaymentVoucherInvoiceDto
	{
		public int PaymentVoucherInvoiceId { get; set; }
		public int PaymentVoucherHeaderId { get; set; }
		public int InvoiceId { get; set; }
		public DateTime InvoiceDate { get; set; }
		public decimal InvoiceTotalValue { get; set; }
		public decimal InvoicePaidValue { get; set; }
		public decimal InvoiceDueValue { get; set; }
		public decimal InvoiceInstallmentValue { get; set; }
		public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
		public string? UserNameCreated { get; set; }
		public string? IpAddressCreated { get; set; }
	}

	public class PurchaseInvoiceSettlementDto
	{
		public int PurchaseInvoiceSettlementId { get; set; }
		public int PurchaseInvoiceHeaderId { get; set; }
		public int PaymentVoucherHeaderId { get; set; }
		public string? Prefix { get; set; }
		public int DocumentCode { get; set; }
		public string? Suffix { get; set; }
		public string? DocumentFullCode { get; set; }
		public string? DocumentReference { get; set; }
		public int SupplierId { get; set; }
		public int SupplierCode { get; set; }
		public string? SupplierName { get; set; }
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
		public DateTime DocumentDate { get; set; }
		public DateTime? DueDate { get; set; }
		public DateTime EntryDate { get; set; } = DateHelper.GetDateTimeNow();
		public string? Reference { get; set; }
		public short? MenuCode { get; set; }
		public string? MenuName { get; set; }
		public byte InvoiceTypeId { get; set; }
		public string? InvoiceTypeName { get; set; }
		public decimal InvoiceValue { get; set; }
		public decimal PreviouslySettledValue { get; set; }
		public decimal SettleValue { get; set; }
		public decimal RemainingValue { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }

		public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
		public string? UserNameCreated { get; set; }
		public string? IpAddressCreated { get; set; }
	}

	public class PurchaseInvoiceSettledValueDto
	{
		public int PurchaseInvoiceHeaderId { get; set; }
		public decimal? SettledValue { get; set; }
	}

	public class PaymentVoucherSettledValueDto
	{
		public int PaymentVoucherHeaderId { get; set; }
		public decimal? SettledValue { get; set; }
	}
}
