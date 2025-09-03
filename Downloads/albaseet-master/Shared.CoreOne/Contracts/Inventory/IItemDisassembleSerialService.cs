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
	public interface IItemDisassembleSerialService:IBaseService<ItemDisassembleSerial>
	{
		IQueryable<ItemDisassembleSerialDto> GetItemDisassembleSerial();
		Task<List<ItemDisassembleSerialDto>> GetItemDisassembleSerial(int itemDisassembleHeaderId);
		Task<ResponseDto> SaveItemDisassembleSerial(int itemDisassembleHeaderId, List<ItemDisassembleSerialDto> details);
		Task<ResponseDto> DeleteItemDisassembleSerial(int itemDisassembleHeaderId);
	}
}
