using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Accounts
{
	public class AccountLedgerDto
	{
		public byte AccountLedgerId { get; set; }
		public string? AccountLedgerNameAr { get; set; }
		public string? AccountLedgerNameEn { get; set; }
	}
	public class AccountLedgerDropDownDto
	{
		public byte AccountLedgerId { get; set; }
		public string? AccountLedgerName { get; set; }
	}
}
