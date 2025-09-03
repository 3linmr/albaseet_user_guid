using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Logic;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.Helper.Models.Dtos;
using System.Text.Json.Serialization;
using Shared.Helper.Extensions;

namespace Sales.CoreOne.Models.Dtos.ViewModels
{
    public class SalesInvoiceReturnDto
    {
        public SalesInvoiceReturnHeaderDto? SalesInvoiceReturnHeader { get; set; }
        public List<SalesInvoiceReturnDetailDto> SalesInvoiceReturnDetails { get; set; } = new List<SalesInvoiceReturnDetailDto>();
        public List<SalesInvoiceReturnPaymentDto> SalesInvoiceReturnPayments { get; set; } = new List<SalesInvoiceReturnPaymentDto>();
        public JournalDto? Journal { get; set; }
        public List<MenuNoteDto>? MenuNotes { get; set; } = new List<MenuNoteDto>();
    }

    public class SalesInvoiceReturnHeaderDto
    {
        public int SalesInvoiceReturnHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int DocumentCode { get; set; }
        public string? Suffix { get; set; }
        public string? DocumentFullCode { get; set; }
        public int SalesInvoiceHeaderId { get; set; }
        public string? SalesInvoiceFullCode { get; set; }
        public string? SalesInvoiceDocumentReference { get; set; }
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
        public string? ShipmentTypeName { get; set; }
        public string? CashClientName { get; set; }
        public string? ClientPhone { get; set; }
        public string? ClientAddress { get; set; }
        public string? ClientTaxCode { get; set; }
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
        public int DebitAccountId { get; set; }
        public int? CreditAccountId { get; set; }
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

    public class SalesInvoiceReturnPaymentDto
    {
		public int SalesInvoiceReturnPaymentId { get; set; }
		public int SalesInvoiceReturnHeaderId { get; set; }
		public int PaymentMethodId { get; set; }
        public string? PaymentMethodName { get; set; }
		public int AccountId { get; set; }
		public short CurrencyId { get; set; }
        public string? CurrencyName { get; set; }
		public decimal CurrencyRate { get; set; }
		public decimal PaidValue { get; set; }
		public decimal PaidValueAccount { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }

		public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
		public string? UserNameCreated { get; set; }
		public string? IpAddressCreated { get; set; }
	}

    public class SalesInvoiceReturnDetailTaxDto
    {
        public int SalesInvoiceReturnDetailTaxId { get; set; }
        public int SalesInvoiceReturnDetailId { get; set; }
        public int TaxId { get; set; }
        public byte TaxTypeId { get; set; }
        public bool TaxAfterVatInclusive { get; set; }
        public int DebitAccountId { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxValue { get; set; }
    }

    public class SalesInvoiceReturnDetailDto
    {
        public int SalesInvoiceReturnDetailId { get; set; }
        public int SalesInvoiceReturnHeaderId { get; set; }
        public int ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? ItemNameReport { get; set; }
        public byte? TaxTypeId { get; set; }
        public byte? ItemTypeId { get; set; }
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
		public decimal SalesInvoiceQuantity { get; set; }
		public decimal SalesInvoiceBonusQuantity { get; set; }
		public decimal AvailableQuantity { get; set; }
		public decimal AvailableBonusQuantity { get; set; }
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
        public List<SalesInvoiceReturnDetailTaxDto> SalesInvoiceReturnDetailTaxes { get; set; } = new List<SalesInvoiceReturnDetailTaxDto>();
    }

    public class SalesInvoiceDetailDataDto
    {
        public int SalesInvoiceHeaderId { get; set; }
        public decimal HeaderDiscountPercent { get; set; }
        public List<SalesInvoiceDetailDto> SalesInvoiceDetails { get; set; } = [];
        public List<int> ItemIds { get; set; } = [];
        public List<Item> Items { get; set; } = [];
        public List<ItemPacking> ItemPackings { get; set; } = [];
        public List<ItemCost> ItemCosts { get; set; } = [];
        public List<LastSalesPriceDto> LastSalesPrices { get; set; } = [];
    }

	public class SalesInvoiceReturnSaveResult
	{
		public ResponseDto Result = new ResponseDto();
		public int? SalesInvoiceReturnHeaderId;
		public int? StockOutReturnHeaderId;
	}
}
