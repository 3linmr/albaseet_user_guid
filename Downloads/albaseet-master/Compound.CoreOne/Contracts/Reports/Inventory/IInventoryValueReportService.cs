using Compound.CoreOne.Models.Dtos.Reports.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reports.Inventory
{
	public interface IInventoryValueReportService
	{
		Task<IQueryable<InventoryValueReportDto>> GetInventoryValueReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? itemCategoryId, int? itemSubCategoryId, int? itemSectionId, int? itemSubSectionId, int? mainItemId, DateTime? expireBefore);
	}
}
