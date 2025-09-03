using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Extensions;

namespace Sales.CoreOne.Models.Dtos.ViewModels
{
	public class SalesInvoiceWithResponseDto
	{
		public required SalesInvoiceDto SalesInvoice { get; set; }
		public required ResponseDto Result { get; set; }
	}

	public class SalesInvoiceDetailsWithResponseDto
	{
		public required List<SalesInvoiceDetailDto> SalesInvoiceDetails { get; set; }
		public required ResponseDto Result { get; set; }
	}

	public struct SalesInvoiceDetailsAndListofIncompleteItems
	{
		public required List<SalesInvoiceDetailDto> SalesInvoiceDetails;
		public required List<IncompleteItemDto> incompleteItemIds;
	}

	public class SalesInvoiceDto
    {
        public SalesInvoiceHeaderDto? SalesInvoiceHeader { get; set; }
        public List<SalesInvoiceDetailDto> SalesInvoiceDetails { get; set; } = new List<SalesInvoiceDetailDto>();
        public List<SalesInvoiceCollectionDto> SalesInvoiceCollections {  get; set; } = new List<SalesInvoiceCollectionDto>();
		public JournalDto? Journal { get; set; }
        public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();
    }

    public class SalesInvoiceHeaderDto
    {
        public int SalesInvoiceHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int DocumentCode { get; set; }
        public string? Suffix { get; set; }
        public string? DocumentFullCode { get; set; }
        public int ProformaInvoiceHeaderId { get; set; }
        public string? ProformaInvoiceFullCode { get; set; }
        public string? ProformaInvoiceDocumentReference { get; set; }
        public int? ClientQuotationApprovalHeaderId { get; set; }
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
        public DateTime EntryDate { get; set; } = DateHelper.GetDateTimeNow();
        public string? Reference { get; set; }
        public bool IsDirectInvoice { get; set; }
        public bool CreditPayment { get; set; }
        public byte TaxTypeId { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int? ShipmentTypeId { get; set; }
        public string? CashClientName { get; set; }
        public string? ClientPhone { get; set; }
        public string? ClientAddress { get; set; }
        public string? ClientTaxCode { get; set; }
        public string? ShipmentTypeName { get; set; }
        public string? DriverName { get; set; }
        public string? DriverPhone { get; set; }
        public string? ClientResponsibleName { get; set; }
        public string? ClientResponsiblePhone { get; set; }
        public string? ShipTo { get; set; }
        public string? BillTo { get; set; }
        public string? ShippingRemarks { get; set; }
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
        public int? DebitAccountId { get; set; }
        public int CreditAccountId { get; set; }
        public int JournalHeaderId { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
        public int CanReturnInDays { get; set; }
        public DateTime CanReturnUntilDate { get; set; }
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
		public decimal ClientBalance { get; set; }
		public int CreditLimitDays { get; set; }
		public decimal CreditLimitValues { get; set; }
		public int DebitLimitDays { get; set; }
		public int? ArchiveHeaderId { get; set; }

		public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
        public string? UserNameCreated { get; set; }
        public string? IpAddressCreated { get; set; }

        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
        public string? IpAddressModified { get; set; }
    }

    public class SalesInvoiceDetailTaxDto
    {
        public int SalesInvoiceDetailTaxId { get; set; }
        public int SalesInvoiceDetailId { get; set; }
        public int TaxId { get; set; }
        public byte TaxTypeId { get; set; }
        public bool TaxAfterVatInclusive { get; set; } 
        public int CreditAccountId { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxValue { get; set; }  
    }

    public class SalesInvoiceDetailDto
    {
        public int SalesInvoiceDetailId { get; set; }
        public int SalesInvoiceHeaderId { get; set; }
        public int StoreId { get; set; }
        public decimal AvailableBalance { get; set; }
        public int ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? ItemNameReport { get; set; }
        public byte? ItemTypeId { get; set; }
        public byte? TaxTypeId { get; set; }
        public int ItemPackageId { get; set; }
        public string? ItemPackageName { get; set; }
        public bool IsItemVatInclusive { get; set; }
        public int? CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
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
        public decimal SellingPrice { get; set; }
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
        [Trim]
        public string? ItemNote { get; set; }
        public byte VatTaxTypeId { get; set; }
        public int VatTaxId { get; set; }
        public decimal ConsumerPrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal CostPackage { get; set; }
        public decimal CostValue { get; set; }
        public decimal LastSalesPrice { get; set; }

        public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
        public string? UserNameCreated { get; set; }
        public string? IpAddressCreated { get; set; }

        public string? Packages { get; set; }
        public string? Taxes { get; set; }

        public List<ItemTaxDataDto> ItemTaxData { get; set; } = new List<ItemTaxDataDto>();
        public List<SalesInvoiceDetailTaxDto> SalesInvoiceDetailTaxes { get; set; } = new List<SalesInvoiceDetailTaxDto>();
    }

    public class SalesInvoiceCollectionDto
    {
		public int SalesInvoiceCollectionId { get; set; }
		public int SalesInvoiceHeaderId { get; set; }
		public int PaymentMethodId { get; set; }
        public string? PaymentMethodName { get; set; }
		public int AccountId { get; set; }
		public short CurrencyId { get; set; }
        public string? CurrencyName { get; set; }
		public decimal CurrencyRate { get; set; }
		public decimal CollectedValue { get; set; }
		public decimal CollectedValueAccount { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }

		public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
		public string? UserNameCreated { get; set; }
		public string? IpAddressCreated { get; set; }
	}

	public class LastSalesPriceDto
    {
        public int ItemId { get; set; }
        public int ItemPackageId { get; set; }
        public decimal SellingPrice { get; set; }
    }

	public class SalesInvoiceSaveResult
	{
		public ResponseDto Result = new ResponseDto();
		public int? SalesInvoiceHeaderId;
		public int? ProformaInvoiceHeaderId;
		public int? StockOutHeaderId;
	}

    public class SalesInvoiceFromParentDto
    {
        public int ProformaInvoiceHeaderId {get;set;}
        public string? ProformaInvoiceFullCode {get;set;}
        public string? ProformaInvoiceDocumentReference {get;set;}
        public int? ClientQuotationApprovalHeaderId {get;set;}
        public int ClientId {get;set;}
        public int ClientCode { get; set; }
        public string? ClientName {get;set;}
        public int? SellerId { get; set; }
        public int? SellerCode { get; set; }
        public string? SellerName { get; set; }
        public int StoreId {get;set;}
        public string? StoreName {get;set;}
        public DateTime DocumentDate {get;set;}
        public string? Reference {get;set;}
        public bool IsDirectInvoice {get;set;}
        public bool CreditPayment {get;set;}
        public byte TaxTypeId {get;set;}
        public DateTime? ShippingDate {get;set;}
        public DateTime? DeliveryDate {get;set;}
        public DateTime? DueDate {get;set;}
        public int? ShipmentTypeId {get;set;}
        public string? ShipmentTypeName { get;set; }
        public string? CashClientName {get;set;}
        public string? ClientPhone {get;set;}
        public string? ClientAddress {get;set;}
        public string? ClientTaxCode {get;set;}
        public string? DriverName {get;set;}
        public string? DriverPhone {get;set;}
        public string? ClientResponsibleName {get;set;}
        public string? ClientResponsiblePhone {get;set;}
        public string? ShipTo {get;set;}
        public string? BillTo {get;set;}
        public string? ShippingRemarks {get;set;}
        public decimal DiscountPercent {get;set;}
		public decimal AdditionalDiscountValue { get; set; }
        public string? RemarksAr {get;set;}
        public string? RemarksEn {get;set;}
        public bool IsOnTheWay {get;set;}
        public int? ArchiveHeaderId { get; set; }
	}

	public class SalesInvoiceOverallValueDto
	{
		public int SalesInvoiceHeaderId { get; set; }
		public decimal OverallNetValue { get; set; }
        public decimal OverallTotalValue { get; set; }
        public decimal OverallGrossValue { get; set; }
        public decimal OverallCostValue { get; set; }
        public decimal OverallProfit { get; set; } 
        public decimal OverallProfitPercent { get; set; } 
        public decimal OverallDiscount { get; set; }
        public decimal OverallDiscountPercent { get; set; }
        public decimal OverallTaxValue { get; set; }
    }
}
