using Compound.CoreOne.Models.Dtos.Reports.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Inventory
{
	public interface IStagnantItemsReportService
	{
		Task<IQueryable<StagnantItemsReportDto>> GetStagnantItemsReport(List<int> storeIds, int daysSinceLastSold);
	}
}
