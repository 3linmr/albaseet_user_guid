using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class BankDto
	{
		public int BankId { get; set; }

		public int BankCode { get; set; }

		public string? BankNameAr { get; set; }

		public string? BankNameEn { get; set; }

        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public string? AccountNumber { get; set; }

		public string? IBAN { get; set; }

		public string? Phone { get; set; }

		public string? Fax { get; set; }

		public string? Website { get; set; }

		public string? Email { get; set; }

		public string? Address { get; set; }

		public string? ResponsibleName { get; set; }

		public string? ResponsiblePhone { get; set; }

		public string? ResponsibleFax { get; set; }

		public string? ResponsibleEmail { get; set; }
		public string? TaxCode { get; set; }

		public decimal VisaFees { get; set; }

		public int? AccountId { get; set; }

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

	public class BankDropDownDto
	{
		public int BankId { get; set; }
		public int BankCode { get; set; }
		public string? BankName { get; set; }
	}

	public class BankAutoCompleteDto
	{
		public int BankId { get; set; }
		public int BankCode { get; set; }
		public string? BankName { get; set; }
		public string? BankNameAr { get; set; }
		public string? BankNameEn { get; set; }
	}
}
