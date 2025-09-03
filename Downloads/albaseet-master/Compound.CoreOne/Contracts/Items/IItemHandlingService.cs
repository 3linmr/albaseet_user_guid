using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;

namespace Compound.CoreOne.Contracts.Items
{
	public interface IItemHandlingService
	{
		Task<IQueryable<ItemBalanceDto>> GetItemBalances(int storeId, bool isGrouped);
		Task<IQueryable<ItemBalanceDto>> GetItemCard(int storeId);
		Task<IQueryable<ItemAutoCompleteDto>> GetItemsByStoreId(int storeId);
		Task<List<ItemPricesDto>> GetItemPrices(int storeId,int itemId,int packageId);
		Task<ItemAutoCompleteDto?> GetItemByBarCode(string? barCode, int storeId, DateTime? currentDate = null);
		Task<ItemAutoCompleteDto?> GetItemSearchByItemId(int itemId, int storeId, DateTime? currentDate = null);
		Task<ItemAutoCompleteDto?> GetItemSearchByItemCode(string? itemCode, int storeId, DateTime? currentDate = null);
		Task<ItemAutoCompleteDto?> GetItemDataOnPackageChange(int itemId,int fromPackageId, int storeId,bool isGrouped, DateTime? currentDate = null,DateTime? expireDate = null,string? batchNumber = null);
		Task<ItemPricesAutoCompleteDto?> GetItemPricesOnPackageChange(int itemId,int fromPackageId, int storeId);
		Task<ItemCostPriceValueDto?> GetItemCostPriceValue(int itemId,int itemPackageId, int storeId);
	}
}
