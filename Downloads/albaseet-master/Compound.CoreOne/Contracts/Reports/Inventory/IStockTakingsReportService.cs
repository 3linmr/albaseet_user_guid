using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Dtos.Reports.Inventory;

namespace Compound.CoreOne.Contracts.Reports.Inventory
{
	public interface IStockTakingsReportService
	{
		Task<IQueryable<StockTakingsReportDto>> GetStockTakingsReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? itemCategoryId, int? itemSubCategoryId, int? itemSectionId, int? itemSubSectionId, int? mainItemId, DateTime? expireBefore);
	}
}
