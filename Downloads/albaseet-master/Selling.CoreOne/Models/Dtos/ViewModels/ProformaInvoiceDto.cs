using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Logic;
using Shared.Helper.Extensions;

namespace Sales.CoreOne.Models.Dtos.ViewModels
{
    public class ProformaInvoiceDto
    {
        public ProformaInvoiceHeaderDto? ProformaInvoiceHeader { get; set; }
        public List<ProformaInvoiceDetailDto> ProformaInvoiceDetails { get; set; } = new List<ProformaInvoiceDetailDto>();
        public List<MenuNoteDto> MenuNotes { get; set; } = new List<MenuNoteDto>();
    }

    public class ProformaInvoiceHeaderDto
    {
        public int ProformaInvoiceHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int DocumentCode { get; set; }
        public string? Suffix { get; set; }
        public string? DocumentFullCode { get; set; }
        public int? ClientQuotationApprovalHeaderId { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime EntryDate { get; set; } = DateHelper.GetDateTimeNow();
        public string? DocumentReference { get; set; }
        public int ClientId { get; set; }
        public int ClientCode { get; set; }
        public string? ClientName { get; set; }
		public int? SellerId { get; set; }
        public int? SellerCode { get; set; }
		public string? SellerName { get; set; }
		public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public string? Reference { get; set; }
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
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
        public bool IsClosed { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsEnded { get; set; }
        public bool IsBlocked { get; set; }
        public int DocumentStatusId { get; set; }
        public string? DocumentStatusName { get; set; }
        public int? ShippingStatusId { get; set; }
        public string? ShippingStatusName { get; set; }
        public int? ArchiveHeaderId { get; set; }

        public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
        public string? UserNameCreated { get; set; }
        public string? IpAddressCreated { get; set; }

        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
        public string? IpAddressModified { get; set; }
    }

    public class ProformaInvoiceDetailTaxDto
    {
        public int ProformaInvoiceDetailTaxId { get; set; }
        public int ProformaInvoiceDetailId { get; set; }
        public int TaxId { get; set; }
        public int CreditAccountId { get; set; }
        public bool TaxAfterVatInclusive { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxValue { get; set; }  //CreditValue
    }

    public class ProformaInvoiceDetailDto
    {
        public int ProformaInvoiceDetailId { get; set; }
        public int ProformaInvoiceHeaderId { get; set; }
        public int StoreId { get; set; }
        public decimal AvailableBalance { get; set; }
        public int? CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
        public int ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? ItemNameReport { get; set; }
        public byte? ItemTypeId { get; set; }
        public byte? TaxTypeId { get; set; }
        public int ItemPackageId { get; set; }
        public string? ItemPackageName { get; set; }
        public bool IsItemVatInclusive { get; set; }
        public string? BarCode { get; set; }
        public decimal Packing { get; set; }
        public decimal Quantity { get; set; }
        public decimal BonusQuantity { get; set; }
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
        public List<ProformaInvoiceDetailTaxDto> ProformaInvoiceDetailTaxes { get; set; } = new List<ProformaInvoiceDetailTaxDto>();
    }

    public class DescendantDto
    {
        public int MenuCode { get; set; }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
