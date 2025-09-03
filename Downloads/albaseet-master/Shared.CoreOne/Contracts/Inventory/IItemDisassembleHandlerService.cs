using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Inventory
{
	public interface IItemDisassembleHandlerService
	{
		IQueryable<ItemConversionDto> GetItemDisassembleHeaderWithOneDetail(int storeId);
		Task<ItemDisassembleInfoDto> GetItemDisassembleInfo(int itemDisassembleHeaderId);
		Task<ResponseDto> SaveItemDisassembleDocument(ItemConversionDto model);
		Task<ResponseDto> DeleteItemDisassembleDocument(int itemDisassembleHeaderId);
	}
}
