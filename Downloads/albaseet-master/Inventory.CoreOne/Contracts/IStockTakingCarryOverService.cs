using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;

namespace Inventory.CoreOne.Contracts
{
	public interface IStockTakingCarryOverService
	{
		List<RequestChangesDto> GetCarryOverRequestChanges(StockTakingCarryOverDto oldItem, StockTakingCarryOverDto newItem);
		Task<StockTakingCarryOverDto> GetCarryOver(int stockTakingCarryOverHeaderId);
        Task<ResponseDto> SaveStockTaking(StockTakingCarryOverDto stockTaking, bool hasApprove, bool approved, int? requestId);
		Task<ResponseDto> DeleteStockTaking(int stockTakingCarryOverHeaderId);
		Task<bool> DoCarryOver(int storeId, bool isOpenBalance, DateTime openBalanceCarryOverDate, List<StockTakingCarryOverEffectDetail> effects);
		Task<bool> UnDoCarryOver(int storeId, bool isOpenBalance, DateTime openBalanceCarryOverDate, List<StockTakingCarryOverEffectDetail> effects);
	}
}
