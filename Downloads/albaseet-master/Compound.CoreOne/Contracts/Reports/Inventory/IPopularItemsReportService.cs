using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Dtos.Reports.Inventory;

namespace Compound.CoreOne.Contracts.Reports.Inventory
{
    public interface IPopularItemsReportService
	{
		Task<IQueryable<PopularItemsReportDto>> GetPopularItemsReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate);
	}
}
