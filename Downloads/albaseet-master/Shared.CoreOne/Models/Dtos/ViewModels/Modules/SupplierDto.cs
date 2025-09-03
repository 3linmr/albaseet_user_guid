using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class SupplierDto
	{
		public int SupplierId { get; set; }

		public int SupplierCode { get; set; }

		public string? SupplierNameAr { get; set; }

		public string? SupplierNameEn { get; set; }

		[DataType(DataType.Date)]
		[Column(TypeName = "Date")]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime ContractDate { get; set; }
		public int? AccountId { get; set; }
		public string? AccountCode { get; set; }
		public string? AccountName { get; set; }

		public int CreditLimitDays { get; set; }

		public decimal CreditLimitValues { get; set; }
		public int DebitLimitDays { get; set; }


		public int CompanyId { get; set; }

		public string? CompanyName { get; set; }

		public string? TaxCode { get; set; } //الرقم الضريبي

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

		public string? CommercialRegister { get; set; } //السجل التجاري

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

		public bool IsCredit { get; set; }   //تعامل آجل

		public bool IsActive { get; set; }
		public string? IsActiveName { get; set; }
		public string? InActiveReasons { get; set; }


		public DateTime? CreatedAt { get; set; }

		public DateTime? ModifiedAt { get; set; }

		public string? UserNameCreated { get; set; }

		public string? UserNameModified { get; set; }

		public int? ArchiveHeaderId { get; set; }

        public bool CreateNewAccount { get; set; }
        public int? MainAccountId { get; set; }

        public List<MenuNoteDto>? MenuNotes { get; set; }

	
	}

	public class SupplierDropDownDto
	{
		public int SupplierId { get; set; }
		public int SupplierCode { get; set; }
		public string? SupplierName { get; set; }
		public string? MobileNumber { get; set; }
		public int? AccountId { get; set; }
		public string? AccountCode { get; set; }
		public string? AccountName { get; set; }
	}

	public class SupplierAutoCompleteDto
	{
		public int SupplierId { get; set; }
		public int SupplierCode { get; set; }
		public string? SupplierName { get; set; }
		public string? SupplierNameAr { get; set; }
		public string? SupplierNameEn { get; set; }
	}
}
