using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Accounts
{
	public class AccountCategoryDto
	{
		public byte AccountCategoryId { get; set; }
		public byte AccountLedgerId { get; set; }
		public string? AccountLedgerName { get; set; }
		public byte Serial { get; set; }
		public string? AccountCategoryNameAr { get; set; }
		public string? AccountCategoryNameEn { get; set; }
		public byte Level { get; set; }
		public byte? MainAccountCategoryId { get; set; }
	}
	public class AccountCategoryDropDownDto
	{
		public byte AccountCategoryId { get; set; }
		public byte? MainAccountCategoryId { get; set; }
		public byte AccountLedgerId { get; set; }
		public string? AccountCategoryName { get; set; }
	}
}
