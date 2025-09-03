using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;

namespace Shared.CoreOne.Contracts.Items
{
	public interface IItemBarCodeDetailService : IBaseService<ItemBarCodeDetail>
	{
		Task<List<ItemBarCodeDetailDto>> GetItemBarCodeDetails(int itemBarCodeId);
		Task<bool> SaveItemBarCodeDetails(List<ItemBarCodeDetailDto> barCodes, string? itemCode, int itemCompanyId, Func<List<ItemBarCodeDetail>, int, Task<bool>> isBarCodeExistFunc);
	}
}
