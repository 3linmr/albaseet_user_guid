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
    public class StockInDto
    {
        public StockInHeaderDto? StockInHeader { get; set; }
        public List<StockInDetailDto> StockInDetails { get; set; } = new List<StockInDetailDto>();
        public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();

    }

    public class StockInHeaderDto
    {
        public int StockInHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int DocumentCode { get; set; }
        public string? Suffix { get; set; }
        public string? DocumentFullCode { get; set; }
        public string? DocumentReference { get; set; }
        public byte StockTypeId { get; set; }
        public int? PurchaseOrderHeaderId { get; set; }
        public string? PurchaseOrderFullCode { get; set; }
        public string? PurchaseOrderDocumentReference { get; set; }
        public int? PurchaseInvoiceHeaderId { get; set; }
        public string? PurchaseInvoiceFullCode { get; set; }
        public string? PurchaseInvoiceDocumentReference { get; set; }
        public int SupplierId { get; set; }
        public int SupplierCode { get; set; }
        public string? SupplierName { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime EntryDate { get; set; } = DateHelper.GetDateTimeNow();
        public string? Reference { get; set; }
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
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
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

    public class StockInDetailTaxDto
    {
        public int StockInDetailTaxId { get; set; }
        public int StockInDetailId { get; set; }
        public int TaxId { get; set; }
        public int DebitAccountId { get; set; }
        public bool TaxAfterVatInclusive { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxValue { get; set; }
    }

    public class StockInDetailDto
    {
        public int StockInDetailId { get; set; }
        public int StockInHeaderId { get; set; }
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
        public decimal PurchaseOrderQuantity { get; set; }
        public decimal PurchaseOrderBonusQuantity { get; set; }
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
        public List<StockInDetailTaxDto> StockInDetailTaxes { get; set; } = new List<StockInDetailTaxDto>();
    }

    public class StockReceivedFromPurchaseOrderDto
    {
        public int ItemId { get; set; }
        public int ItemPackageId { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int? CostCenterId { get; set; }
        public string? BarCode { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal ItemDiscountPercent { get; set; }
        public decimal QuantityReceived { get; set; }
        public decimal BonusQuantityReceived { get; set; }
    }

    public class StockReceivedFromPurchaseInvoiceDto
    {
        public int ItemId { get; set; }
        public int ItemPackageId { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int? CostCenterId { get; set; }
        public string? BarCode { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal ItemDiscountPercent { get; set; }
        public decimal QuantityReceived { get; set; }
        public decimal BonusQuantityReceived { get; set; }
    }

    public class ParentQuantityDto
    {
        public int ParentId { get; set; } //Id of Parent document, whatever type it is
        public decimal Quantity { get; set; }
    }
}
