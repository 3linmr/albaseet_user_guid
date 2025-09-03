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
	public interface IItemDisassembleHeaderService : IBaseService<ItemDisassembleHeader>
	{
		IQueryable<ItemDisassembleHeaderDto> GetItemDisassembleHeaders();
		IQueryable<ItemDisassembleHeaderDto> GetItemDisassembleHeadersByStoreId(int storeId);
		Task<ItemDisassembleHeaderDto> GetItemDisassembleHeader(int itemDisassembleHeaderId);
		Task<ResponseDto> SaveItemDisassembleHeader(ItemDisassembleHeaderDto model);
		Task<ResponseDto> DeleteItemDisassembleHeader(int itemDisassembleHeaderId);
	}
}
