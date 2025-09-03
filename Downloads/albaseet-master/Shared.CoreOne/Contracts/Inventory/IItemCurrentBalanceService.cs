using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Inventory
{
	public interface IItemCurrentBalanceService : IBaseService<ItemCurrentBalance>
	{
		IQueryable<ItemCurrentBalanceDto> GetItemCurrentBalances();
		IQueryable<ItemCurrentBalanceDto> GetStoreItemCurrentBalances(int storeId);
		IQueryable<ItemCurrentBalanceDto> GetStoreItemCurrentBalancesWithBarCodes(int storeId);
		Task<List<ItemCurrentBalanceDto>> GetItemCurrentBalanceByItemId(int storeId,int itemId);
		Task<ItemCurrentBalanceDto> GetItemCurrentBalanceInfoByItemAndPackageId(int storeId, int itemId, int itemPackageId, bool isGrouped);
		Task<ItemCurrentBalanceDto> GetItemCurrentBalanceInfo(int storeId, int itemId, int itemPackageId, DateTime? expireDate,string? batchNumber);
		Task<List<ItemCurrentBalanceDto>> GetItemCurrentBalanceByItemCode(int storeId, string itemCode);
		Task<List<ItemCurrentBalanceDto>> GetItemCurrentBalanceByBarCode(int storeId, string barCode);
        IQueryable<ItemCurrentBalanceDto> GetItemCurrentBalanceInfo(int storeId, bool isSinglePackage, bool isGrouped);
        Task<ItemCurrentBalanceDto> GetItemCurrentBalanceInfoByItemId(int storeId,int itemId, bool isSinglePackage, bool isGrouped);
        Task<ItemCurrentBalanceDto?> GetItemCurrentBalance(int balanceId);
		Task<bool> InventoryIn(int storeId,List<ItemCurrentBalanceDto> balances);
		Task<bool> InventoryOut(int storeId, List<ItemCurrentBalanceDto> balances);
		Task<bool> InventoryInOut(int storeId,List<ItemCurrentBalanceDto> oldBalances, List<ItemCurrentBalanceDto> newBalances);
		Task<List<int>> ReOrderInventory(int storeId,DateTime openBalanceCarryOverDate, List<ItemCurrentBalanceDto> balances);
		Task<bool> ReOrderInventory(int storeId, List<ItemCurrentBalanceDto> balances);
	}
}
