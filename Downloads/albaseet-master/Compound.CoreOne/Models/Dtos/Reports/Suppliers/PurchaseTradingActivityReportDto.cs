using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Suppliers
{
	public class PurchaseTradingActivityReportDto
	{
		public int HeaderId { get; set; }
		public string? DocumentFullCode { get; set; }
		public int SupplierId { get; set; }
		public int SupplierCode { get; set; }
		public string? SupplierName { get; set; }
		public int? AccountId { get; set; }
		public string? AccountCode { get; set; }
		public string? AccountName { get; set; }
		public string? Reference { get; set; }
		public string? RemarksAr { get; set; }
		public string? RemarksEn { get; set; }
		public short? MenuCode { get; set; }
		public string? MenuName { get; set; }
		public DateTime DocumentDate { get; set; }
		public DateTime EntryDate { get; set; }
		public short? InvoiceTypeId { get; set; }
		public string? InvoiceTypeName { get; set; }
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
		public decimal TotalValue { get; set; }
		public decimal DiscountPercent { get; set; }
		public decimal DiscountValue { get; set; }
		public decimal TotalItemDiscount { get; set; }
		public decimal GrossValue { get; set; }
		public decimal VatValue { get; set; }
		public decimal SubNetValue { get; set; }
		public decimal OtherTaxValue { get; set; }
		public decimal NetValue { get; set; }
		public decimal TotalInvoiceExpense { get; set; }
		public decimal CostValue { get; set; }
		public int CompanyId { get; set; }
		public string? CompanyName { get; set; }
		public int BranchId { get; set; }
		public string? BranchName { get; set; }

        public DateTime ContractDate { get; set; }
        public string? TaxCode { get; set; }
        public int? ShipmentTypeId { get; set; }
        public string? ShipmentTypeName { get; set; }
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
        public string? Street1 { get; set; }
        public string? Street2 { get; set; }
        public string? AdditionalNumber { get; set; }
        public string? CountryCode { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? Address4 { get; set; }
        public string? FirstResponsibleName { get; set; }
        public string? FirstResponsiblePhone { get; set; }
        public string? FirstResponsibleEmail { get; set; }
        public string? SecondResponsibleName { get; set; }
        public string? SecondResponsiblePhone { get; set; }
        public string? SecondResponsibleEmail { get; set; }
        public bool IsCredit { get; set; }
        public bool IsActive { get; set; }
        public string? IsActiveName { get; set; }
        public string? InActiveReasons { get; set; }

		public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
	}
}
