using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Clients
{
    public class SalesInvoicesWithDiscountDto
    {
        public int SalesInvoiceHeaderId { get; set; }
        public string? DocumentFullCode { get; set; }
        public short? MenuCode { get; set; }
        public string? MenuName { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime EntryDate { get; set; }
        public int? AccountId { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public int ClientId { get; set; }
        public int ClientCode { get; set; }
        public string? ClientName { get; set; }
        public int sellerId { get; set; }
        public int? SellerCode { get; set; }
        public string? SellerName { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }

        public decimal InvoiceValueBeforeDiscount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal InvoiceValueAfterDiscount { get; set; }
        public decimal CostValue { get; set; }
        public decimal Profit { get; set; }

        public string? Reference { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }

        public DateTime ContractDate { get; set; }
        public int CreditLimitDays { get; set; }
        public int DebitLimitDays { get; set; }
        public decimal CreditLimitValues { get; set; }
        public string? TaxCode { get; set; }
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }
        public int? StateId { get; set; }
        public string? StateName { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
        public int? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public string? PostalCode { get; set; }
        public string? BuildingNumber { get; set; }
        public string? CommercialRegister { get; set; }
        public string? ClientStreet1 { get; set; }
        public string? ClientStreet2 { get; set; }
        public string? ClientAdditionalNumber { get; set; }
        public string? CountryCode { get; set; }
        public string? ClientAddress1 { get; set; }
        public string? ClientAddress2 { get; set; }
        public string? ClientAddress3 { get; set; }
        public string? ClientAddress4 { get; set; }
        public string? FirstResponsibleName { get; set; }
        public string? FirstResponsiblePhone { get; set; }
        public string? FirstResponsibleEmail { get; set; }
        public string? SecondResponsibleName { get; set; }
        public string? SecondResponsiblePhone { get; set; }
        public string? SecondResponsibleEmail { get; set; }
        public bool IsCredit { get; set; }
        public bool IsActive { get; set; }
        public string? InActiveReasons { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? UserNameCreated { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
    }
}
