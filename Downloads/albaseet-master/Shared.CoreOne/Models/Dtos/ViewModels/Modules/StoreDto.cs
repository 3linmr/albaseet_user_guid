using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class StoreDto
	{
		public int StoreId { get; set; }

		public string? StoreName { get; set; }
		public string? StoreNameAr { get; set; }

		public string? StoreNameEn { get; set; }

		public byte StoreClassificationId { get; set; }
		public string? StoreClassificationName { get; set; }

		public int CompanyId { get; set; }
		public string? CompanyName { get; set; }

		public int BranchId { get; set; }
		public string? BranchName { get; set; }

		public short CurrencyId { get; set; }
		public string? CurrencyName { get; set; }
		public int Rounding { get; set; }

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


		public int? StockDebitAccountId { get; set; } //Dr

		public int? StockCreditAccountId { get; set; } //Cr

		public int? ExpenseAccountId { get; set; } //Manufacturing

		public string? Long { get; set; }

		public string? Lat { get; set; }

		public string? GMap { get; set; }

		public bool NoReplenishment { get; set; }

		public bool IsActive { get; set; }
		public bool IsReservedStore { get; set; }
		public int? ReservedParentStoreId { get; set; }
		public string? IsActiveName { get; set; }
		public string? InActiveReasons { get; set; }

		public bool CanAcceptDirectInternalTransfer { get; set; }
	}

	public class StoreDropDownDto
	{
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public int CurrencyId { get; set; }
		public int Rounding { get; set; }
		public bool IsReservedStore { get; set; }
	}

	public class StoreDropDownVm
	{
		public int StoreId { get; set; }
		public string? StoreName { get; set; }
	}

	public class StoreClassificationDto
	{
		public int StoreClassificationId { get; set; }
		public string? StoreClassificationName { get; set; }
	}

	public class TaxDetailDto
	{
		public string? TaxCode { get; set; }
		public string? CommercialRegister { get; set; }
	}
}
