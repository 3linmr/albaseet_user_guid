
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports
{
	public class BalanceReportDto
	{
		public int AccountId { get; set; }
		public string? AccountCode { get; set; }
		public int CompanyId { get; set; }
		public string? CompanyName { get; set; }
		public string? AccountName { get; set; }
		public string? AccountNameAr { get; set; }
		public string? AccountNameEn { get; set; }
		public string? AccountTypeName { get; set; }
		public int AccountCategoryId { get; set; }
		public string? AccountCategoryName { get; set; }
		public int? MainAccountId { get; set; }
		public int AccountLevel { get; set; }

		public decimal OpenBalance { get; set; }
		public decimal DebitValue { get; set; }
		public decimal CreditValue { get; set; }
		public decimal Difference { get; set; }
		public decimal CurrentBalance { get; set; }

		public decimal? OpenDebitBalance { get; set; }
		public decimal? OpenCreditBalance { get; set; }
		public decimal? CurrentDebitBalance { get; set; }
		public decimal? CurrentCreditBalance { get; set; }
	}
}
