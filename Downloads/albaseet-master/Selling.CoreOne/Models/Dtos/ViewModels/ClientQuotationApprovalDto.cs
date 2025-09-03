using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Taxes;
using Shared.CoreOne.Models;
using Shared.Helper.Logic;
using Shared.Helper.Extensions;

namespace Sales.CoreOne.Models.Dtos.ViewModels
{
    public class ClientQuotationApprovalDto
    {
        public ClientQuotationApprovalHeaderDto? ClientQuotationApprovalHeader { get; set; }
        public List<ClientQuotationApprovalDetailDto> ClientQuotationApprovalDetails { get; set; } = new List<ClientQuotationApprovalDetailDto>();
        public List<MenuNoteDto> MenuNotes { get; set; } = new List<MenuNoteDto>();
    }

    public class ClientQuotationApprovalHeaderDto
    {
        public int ClientQuotationApprovalHeaderId { get; set; }
        public string? Prefix { get; set; }
        public int DocumentCode { get; set; }
        public string? Suffix { get; set; }
        public string? DocumentFullCode { get; set; }
        public int? ClientQuotationHeaderId { get; set; }
        public int ClientId { get; set; }
        public int ClientCode { get; set; }
        public string? ClientName { get; set; }
		public int? SellerId { get; set; }
        public int? SellerCode { get; set; }
		public string? SellerName { get; set; }
		public string? DocumentReference { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
		public byte TaxTypeId { get; set; }
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
        public int? ArchiveHeaderId { get; set; }

        public DateTime? CreatedAt { get; set; } = DateHelper.GetDateTimeNow();
        public string? UserNameCreated { get; set; }
        public string? IpAddressCreated { get; set; }

        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
        public string? IpAddressModified { get; set; }
    }

    public class ClientQuotationApprovalDetailTaxDto
    {
        public int ClientQuotationApprovalDetailTaxId { get; set; }
        public int ClientQuotationApprovalDetailId { get; set; }
        public int TaxId { get; set; }
        public int CreditAccountId { get; set; }
        public bool TaxAfterVatInclusive { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxValue { get; set; }  //CreditValue
    }

    public class ClientQuotationApprovalDetailDto
    {
        public int ClientQuotationApprovalDetailId { get; set; }
        public int ClientQuotationApprovalHeaderId { get; set; }
        public int ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? ItemNameReport { get; set; }
        public byte TaxTypeId { get; set; }
        public byte? ItemTypeId { get; set; }
        public int ItemPackageId { get; set; }
        public string? ItemPackageName { get; set; }
        public bool IsItemVatInclusive { get; set; }
        public int? CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
        public string? BarCode { get; set; }
        public decimal Packing { get; set; }
        public decimal Quantity { get; set; }
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
        public List<ClientQuotationApprovalDetailTaxDto> ClientQuotationApprovalDetailTaxes { get; set; } = new List<ClientQuotationApprovalDetailTaxDto>();
    }
}
