using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;

namespace Shared.CoreOne.Contracts.Inventory
{
	public interface IItemCostService : IBaseService<ItemCost>
	{
		Task<int> GetNextId();
		IQueryable<ItemCostDto> GetItemCosts();
		IQueryable<ItemCostDto> GetItemCostsByStoreId();
		IQueryable<ItemCostDto> GetStoreItemCost(int storeId);
		Task<ItemCostDto?> GetItemCostById(int costId);
		Task<ItemCostDto?> GetItemCostByItemId(int itemId, int storeId);
		Task<decimal> GetItemCostPriceByItemId(int itemId, int storeId);
		Task<bool> SaveCosts(int storeId, List<ItemCostDto> costs);
	}
}
