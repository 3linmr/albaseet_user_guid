using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Logic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Purchases.CoreOne.Models.Dtos.ViewModels
{
    public class PurchaseInvoiceDto
    {
        public PurchaseInvoiceHeaderDto? PurchaseInvoiceHeader { get; set; }
        public List<PurchaseInvoiceDetailDto> PurchaseInvoiceDetails { get; set; } = new List<PurchaseInvoiceDetailDto>();
        public List<PurchaseInvoiceExpenseDto> PurchaseInvoiceExpenses { get; set; } = new List<PurchaseInvoiceExpenseDto>();
        public JournalDto? Journal { get; set; }
        public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();

    }

    public class PurchaseInvoiceHeaderDto
    {
        public int PurchaseInvoiceHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int DocumentCode { get; set; }
        public string? Suffix { get; set; }
        public string? DocumentFullCode { get; set; }
        public int PurchaseOrderHeaderId { get; set; }
        public string? PurchaseOrderFullCode { get; set; }
        public string? PurchaseOrderDocumentReference { get; set; }
        public int? SupplierQuotationHeaderId { get; set; }
        public string? DocumentReference { get; set; }
        public int SupplierId { get; set; }
        public int SupplierCode { get; set; }
        public string? SupplierName { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime EntryDate { get; set; } = DateHelper.GetDateTimeNow();
        public string? Reference { get; set; }
        public bool IsDirectInvoice { get; set; }
        public bool CreditPayment { get; set; }
        public byte TaxTypeId { get; set; }
        public decimal TotalValue { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal TotalItemDiscount { get; set; }
        public decimal GrossValue { get; set; }
        public decimal VatValue { get; set; }
        public decimal SubNetValue { get; set; }
        public decimal OtherTaxValue { get; set; }
		public decimal NetValueBeforeAdditionalDiscount { get; set; }
		public decimal AdditionalDiscountValue { get; set; }
        public decimal NetValue { get; set; }
        public decimal TotalInvoiceExpense { get; set; }
        public decimal TotalCostValue { get; set; }
        public int DebitAccountId { get; set; }
        public int CreditAccountId { get; set; }
        public int JournalHeaderId { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
        public bool IsOnTheWay { get; set; }
        public bool IsClosed { get; set; }
        public bool IsEnded { get; set; }
        public bool IsBlocked { get; set; }
        public bool HasSettlement { get; set; }
		public bool IsSettlementCompleted { get; set; }
        public short? MenuCode { get; set; }
        public string? MenuName { get; set; }
		public byte InvoiceTypeId { get; set; }
        public string? InvoiceTypeName { get; set; }
		public decimal SupplierBalance { get; set; }
		public int CreditLimitDays { get; set; }
		public decimal CreditLimitValues { get; set; }
		public int DebitLimitDays { get; set; }
		public DateTime? DueDate { get; set; }
		public int? ArchiveHeaderId { get; set; }

		public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
        public string? UserNameCreated { get; set; }
        public string? IpAddressCreated { get; set; }

        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
        public string? IpAddressModified { get; set; }
    }

    public class PurchaseInvoiceDetailTaxDto
    {
        public int PurchaseInvoiceDetailTaxId { get; set; }
        public int PurchaseInvoiceDetailId { get; set; }
        public int TaxId { get; set; }
        public byte TaxTypeId { get; set; }
        public int DebitAccountId { get; set; }
        public bool TaxAfterVatInclusive { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxValue { get; set; }
    }

    public class PurchaseInvoiceDetailDto
    {
        private string? _batchNumber;

        public int PurchaseInvoiceDetailId { get; set; }
        public int PurchaseInvoiceHeaderId { get; set; }
        public int? CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
        public int ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? ItemNameReport { get; set; }
        public byte TaxTypeId { get; set; }
		public byte? ItemTypeId { get; set; }
		public int ItemPackageId { get; set; }
        public string? ItemPackageName { get; set; }
        public bool IsItemVatInclusive { get; set; }
        public string? BarCode { get; set; }
        public decimal Packing { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? BatchNumber
        {
            get => string.IsNullOrWhiteSpace(_batchNumber) ? null : _batchNumber;
            set => _batchNumber = value;
        }
        public decimal Quantity { get; set; }
        public decimal BonusQuantity { get; set; }
        [System.Text.Json.Serialization.JsonIgnore] [Newtonsoft.Json.JsonIgnore]
        public decimal KeyLevelQuantity { get; set; }
        [System.Text.Json.Serialization.JsonIgnore] [Newtonsoft.Json.JsonIgnore]
        public decimal KeyLevelBonusQuantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal TotalValue { get; set; }
        public decimal ItemDiscountPercent { get; set; }
        public decimal ItemDiscountValue { get; set; }
        public decimal TotalValueAfterDiscount { get; set; }
        public decimal HeaderDiscountValue { get; set; }
        public decimal GrossValue { get; set; }
        public decimal VatPercent { get; set; }
        public decimal VatValue { get; set; }
        public decimal SubNetValue { get; set; }
        public decimal OtherTaxValue { get; set; }
        public decimal NetValue { get; set; }
        public decimal ItemExpensePercent { get; set; }
        public decimal ItemExpenseValue { get; set; }
        public string? Notes { get; set; }
        public string? ItemNote { get; set; }
        public byte VatTaxTypeId { get; set; }
        public int VatTaxId { get; set; }
        public decimal ConsumerPrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal CostPackage { get; set; }
        public decimal CostValue { get; set; }
        public decimal LastPurchasePrice { get; set; }

        public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
        public string? UserNameCreated { get; set; }
        public string? IpAddressCreated { get; set; }

        public string? Packages { get; set; }
        public string? Taxes { get; set; }
        public List<ItemTaxDataDto> ItemTaxData { get; set; } = new List<ItemTaxDataDto>();
        public List<PurchaseInvoiceDetailTaxDto> PurchaseInvoiceDetailTaxes { get; set; } = new List<PurchaseInvoiceDetailTaxDto>();
    }

    public class PurchaseInvoiceExpenseDto
    {
        public int PurchaseInvoiceExpenseId { get; set; }
        public int PurchaseInvoiceHeaderId { get; set; }
        public int InvoiceExpenseTypeId { get; set; }
        public string? InvoiceExpenseTypeName { get; set; }
        public decimal ExpenseValue { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
    }

    public class LastPurchasePriceDto
    {
        public int ItemId { get; set; }
        public int ItemPackageId { get; set; }
        public decimal PurchasePrice { get; set; }
    }

    public class PurchaseInvoiceOverallValueDto
    {
        public int PurchaseInvoiceHeaderId { get; set; }
        public decimal OverallValue { get; set; }
    }
}
