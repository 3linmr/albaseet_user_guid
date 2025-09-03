using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class CompanyDto
	{
		public int CompanyId { get; set; }

		public string? CompanyNameAr { get; set; }

		public string? CompanyNameEn { get; set; }

		public string? TaxCode { get; set; }

		public short CurrencyId { get; set; }

		public string? CurrencyName { get; set; }

		public string? Phone { get; set; }

		public string? WhatsApp { get; set; }

		public string? Email { get; set; }

		public string? Website { get; set; }

		public string? Address { get; set; }

		public string? LogoUrl { get; set; }

		public string? HeaderUrl { get; set; }

		public string? FooterUrl { get; set; }

		public bool IsActive { get; set; }
		public string? IsActiveName { get; set; }
		public string? InActiveReasons { get; set; }
	}

	public class CompanyDropDownDto
	{
		public int CompanyId { get; set; }
		public string? CompanyName { get; set; }
	}


	public class CompanyIdentityDto
	{
		public int CompanyId { get; set; }
		public string? CompanyNameAr { get; set; }
		public string? CompanyNameEn { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
		public string? VatNumber { get; set; }
		public string? Email { get; set; }
		public short CountryId { get; set; }
	}

	public class SshOptionsDto
	{
		public string? Host { get; set; }
		public int Port { get; set; }
		public string? Username { get; set; }
		public string? Password { get; set; }
	}
}
