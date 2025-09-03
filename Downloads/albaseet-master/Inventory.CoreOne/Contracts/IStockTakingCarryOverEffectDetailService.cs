using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.CoreOne.Models.Domain;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;

namespace Inventory.CoreOne.Contracts
{
	public interface IStockTakingCarryOverEffectDetailService : IBaseService<StockTakingCarryOverEffectDetail>
	{
		Task<List<StockTakingCarryOverEffectDetail>> InsertCarryOverEffect(int stockTakingCarryOverHeaderId,int storeId, bool isOpenBalance, bool isAllItemsAffected, List<StockTakingCarryOverDetailDto> carryOverDetails);
		Task<ResponseDto> DeleteCarryOverEffect(int headerId);
	}
}
