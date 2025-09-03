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
	public interface IItemDisassembleDetailService:IBaseService<ItemDisassembleDetail>
	{
		IQueryable<ItemDisassembleDetailDto> GetItemDisassembleDetails();
		Task<List<ItemDisassembleDetailDto>> GetItemDisassembleDetails(int itemDisassembleHeaderId);
		Task<ResponseDto> SaveItemDisassembleDetails(int itemDisassembleHeaderId,List<ItemDisassembleDetailDto> details);
		Task<ResponseDto> DeleteItemDisassembleDetails(int itemDisassembleHeaderId);
	}
}
