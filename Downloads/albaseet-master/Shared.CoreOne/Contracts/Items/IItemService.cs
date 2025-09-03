using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Items
{
	public interface IItemService : IBaseService<Item>
	{
		IQueryable<ItemDto> GetAllItems();
		IQueryable<ItemDto> GetUserItems();
		Task<List<ItemAutoCompleteVm>> GetItemsAutoComplete(string itemName);
		Task<List<ItemAutoCompleteVm>> GetItemsAutoCompleteByStoreIds(string term, List<int> storeIds);
        Task<ItemDto?> GetItemById(int id);
		Task<List<ItemBarCodeDto>> GetItemBarCodeByItemId(int itemId);
		Task<int> GetItemItemIdByBarCode(string barCode);
		Task<ItemVm> GetItem(int id);
		Task<bool> IsItemsVatInclusive(int storeId);
		Task<ResponseDto> SaveItem(ItemDto item);
		Task<ResponseDto> SaveItemInFull(ItemVm item);
		List<RequestChangesDto> GetItemRequestChanges(ItemVm oldItem,ItemVm newItem);
		Task<ResponseDto> DeleteItem(int id);
		Task<ResponseDto> DeleteItemInFull(int id);
	}
}
