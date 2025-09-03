using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class SellerDto
	{
		public int SellerId { get; set; }
		public int SellerCode { get; set; }

		public byte SellerTypeId { get; set; }
		public string? SellerTypeName { get; set; }

		public short? SellerCommissionMethodId { get; set; }
		public string? SellerCommissionMethodName{ get; set; }

		public string? SellerNameAr { get; set; }

		public string? SellerNameEn { get; set; }

		public DateTime ContractDate { get; set; }

		public int CompanyId { get; set; }
		public string? CompanyName { get; set; }

		public int? EmployeeId { get; set; }

		public string? OutSourcingCompany { get; set; }

		public string? Phone { get; set; }

		public string? WhatsApp { get; set; }

		public string? Address { get; set; }

		public string? Email { get; set; }

		public decimal ClientsDebitLimit { get; set; } // الحد الائتماني لمديونية عملاؤه

		public bool IsActive { get; set; }

		public string? IsActiveName { get; set; }

		public string? InActiveReasons { get; set; }

		public int? ArchiveHeaderId { get; set; }


		public DateTime? CreatedAt { get; set; }

		public DateTime? ModifiedAt { get; set; }

		public string? UserNameCreated { get; set; }

		public string? UserNameModified { get; set; }

		public List<MenuNoteDto>? MenuNotes { get; set; }
	}

	public class SellerDropDownDto
	{
		public int SellerId { get; set; }
		public int SellerCode { get; set; }
		public string? SellerName { get; set; }
	}

	public class SellerAutoCompleteDto
	{
		public int SellerId { get; set; }
		public int SellerCode { get; set; }
		public string? SellerName { get; set; }
		public string? SellerNameAr { get; set; }
		public string? SellerNameEn { get; set; }
	}
}
