using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Logic;

namespace Purchases.CoreOne.Models.Dtos.ViewModels
{
    public class PurchaseInvoiceReturnDto
    {
        public PurchaseInvoiceReturnHeaderDto? PurchaseInvoiceReturnHeader { get; set; }
        public List<PurchaseInvoiceReturnDetailDto> PurchaseInvoiceReturnDetails { get; set; } = new List<PurchaseInvoiceReturnDetailDto>();
        public JournalDto? Journal { get; set; }
        public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();

    }

    public class PurchaseInvoiceReturnHeaderDto
    {
        public int PurchaseInvoiceReturnHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int DocumentCode { get; set; }
        public string? Suffix { get; set; }
        public string? DocumentFullCode { get; set; }
        public int PurchaseInvoiceHeaderId { get; set; }
        public string? PurchaseInvoiceFullCode { get; set; }
        public string? PurchaseInvoiceDocumentReference { get; set; }
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
        public int? ArchiveHeaderId { get; set; }
		public short? MenuCode { get; set; }
		public string? MenuName { get; set; }

		public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
        public string? UserNameCreated { get; set; }
        public string? IpAddressCreated { get; set; }

        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
        public string? IpAddressModified { get; set; }
    }

    public class PurchaseInvoiceReturnDetailTaxDto
    {
        public int PurchaseInvoiceReturnDetailTaxId { get; set; }
        public int PurchaseInvoiceReturnDetailId { get; set; }
        public int TaxId { get; set; }
        public byte TaxTypeId { get; set; }
        public int CreditAccountId { get; set; }
        public bool TaxAfterVatInclusive { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxValue { get; set; }
    }

    public class PurchaseInvoiceReturnDetailDto
    {
        public int PurchaseInvoiceReturnDetailId { get; set; }
        public int PurchaseInvoiceReturnHeaderId { get; set; }
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
        public string? BatchNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal BonusQuantity { get; set; }
        [System.Text.Json.Serialization.JsonIgnore] [Newtonsoft.Json.JsonIgnore]
        public decimal KeyLevelQuantity { get; set; }
        [System.Text.Json.Serialization.JsonIgnore] [Newtonsoft.Json.JsonIgnore]
        public decimal KeyLevelBonusQuantity { get; set; }
        public decimal PurchaseInvoiceQuantity { get; set; }
        public decimal PurchaseInvoiceBonusQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal AvailableBonusQuantity { get; set; }
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
        public List<PurchaseInvoiceReturnDetailTaxDto> PurchaseInvoiceReturnDetailTaxes { get; set; } = new List<PurchaseInvoiceReturnDetailTaxDto>();
    }
}
