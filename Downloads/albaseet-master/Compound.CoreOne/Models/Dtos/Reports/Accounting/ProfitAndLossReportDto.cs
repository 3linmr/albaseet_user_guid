using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Accounting
{
	public class ProfitAndLossReportDto
	{
		public int Serial { get; set; }
		public int? AccountId { get; set; }
		public int Level { get; set; }
		public string? AccountName { get; set; }
		public string? AccountNameAr { get; set; }
		public string? AccountNameEn { get; set; }
		public decimal? TotalValue { get; set; } //used for base level accounts
		public decimal? Value { get; set; } //used for sub accounts
		public int? MainAccountId { get; set; }

	}

}
