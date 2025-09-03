using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Logic;

namespace Accounting.CoreOne.Models.Dtos.ViewModels
{
	public class ReceiptVoucherDto
	{
		public ReceiptVoucherHeaderDto? ReceiptVoucherHeader { get; set; }
		public List<ReceiptVoucherDetailDto> ReceiptVoucherDetails { get; set; } = new List<ReceiptVoucherDetailDto>();
		public List<CostCenterJournalDetailDto> CostCenterJournalDetails { get; set; } = new List<CostCenterJournalDetailDto>();
		public List<SalesInvoiceSettlementDto> SalesInvoiceSettlements { get; set; } = new List<SalesInvoiceSettlementDto> { };
		public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();
	}

	public class ReceiptVoucherHeaderDto
	{
		public int ReceiptVoucherHeaderId { get; set; }
		public string? Prefix { get; set; }
		public int ReceiptVoucherCode { get; set; }
		public string? Suffix { get; set; }
		public string? ReceiptVoucherCodeFull { get; set; }
		public string? DocumentReference { get; set; }
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
		public int? ClientId { get; set; }
		public int? ClientCode { get; set; }
		public string? ClientName { get; set; }
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
	public class ReceiptVoucherDetailDto
	{
		public int ReceiptVoucherDetailId { get; set; }
		public int ReceiptVoucherHeaderId { get; set; }
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

	public class ReceiptVoucherInvoiceDto
	{
		public int ReceiptVoucherInvoiceId { get; set; }
		public int ReceiptVoucherHeaderId { get; set; }
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

	public class SalesInvoiceSettlementDto
	{
		public int SalesInvoiceSettlementId { get; set; }
		public int SalesInvoiceHeaderId { get; set; }
		public int ReceiptVoucherHeaderId { get; set; }
		public string? Prefix { get; set; }
		public int DocumentCode { get; set; }
		public string? Suffix { get; set; }
		public string? DocumentFullCode { get; set; }
		public string? DocumentReference { get; set; }
		public int ClientId { get; set; }
		public int ClientCode { get; set; }
		public string? ClientName { get; set; }
		public int? SellerId { get; set; }
		public int? SellerCode { get; set; }
		public string? SellerName { get; set; }
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

    public class SalesInvoiceSettledValueDto
    {
        public int SalesInvoiceHeaderId { get; set; }
        public decimal? SettledValue { get; set; }
    }

    public class ReceiptVoucherSettledValueDto
    {
        public int ReceiptVoucherHeaderId { get; set; }
        public decimal? SettledValue { get; set; }
    }
}
