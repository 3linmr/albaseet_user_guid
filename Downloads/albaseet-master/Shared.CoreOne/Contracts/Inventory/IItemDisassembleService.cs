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
	public interface IItemDisassembleService : IBaseService<ItemDisassemble>
	{
		IQueryable<ItemDisassembleDto> GetItemDisassembles();
		IQueryable<ItemDisassembleDto> GetItemDisassembles(int storeId);
		Task<List<ItemDisassembleDto>> GetItemDisassembleByHeaderId(int itemDisassembleHeaderId);
		Task<ResponseDto> SaveItemDisassemble(int storeId, int itemDisassembleHeaderId, List<ItemDisassembleDto> data);
		Task<ResponseDto> DeleteItemDisassemble(int itemDisassembleHeaderId);
	}
}
