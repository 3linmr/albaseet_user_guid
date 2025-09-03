using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.CoreOne.Models.Dtos.Reports
{
	public class LedgerDto
	{
		public int Id { get; set; }
		public string? AccountNameAr { get; set; }
		public string? AccountName { get; set; }
	}
}
