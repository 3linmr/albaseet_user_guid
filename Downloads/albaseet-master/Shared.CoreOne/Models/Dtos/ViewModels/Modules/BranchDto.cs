using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class BranchDto 
	{
		public int BranchId { get; set; }
		public string? BranchNameAr { get; set; }
		public string? BranchNameEn { get; set; }
		public int CompanyId { get; set; }
		public string? CompanyName { get; set; }
		public short CurrencyId { get; set; }
		public string? CurrencyName { get; set; }
		public string? BranchPhone { get; set; }
		public string? BranchWhatsApp { get; set; }
		public string? BranchEmail { get; set; }
		public string? BranchWebsite { get; set; }
		public string? BranchAddress { get; set; }
		public string? ResponsibleNameAr { get; set; }
		public string? ResponsibleNameEn { get; set; }
		public string? ResponsiblePhone { get; set; }
		public string? ResponsibleWhatsApp { get; set; }
		public string? ResponsibleEmail { get; set; }
		public string? ResponsibleAddress { get; set; }
		public bool IsActive { get; set; }
		public string? IsActiveName { get; set; }
		public string? InActiveReasons { get; set; }
	}

	public class BranchDropDownDto
	{
		public int BranchId { get; set; }
		public int CompanyId { get; set; }
		public string? BranchName { get; set; }
	}
}
