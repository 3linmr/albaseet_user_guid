using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Dtos.Reports;

namespace Accounting.CoreOne.Contracts.Reports
{
	public interface IJournalLedgerService
	{
		Task<List<LedgerDto>> GetList(DateTime fromDate,DateTime toDate,int storeId);
	}
}
