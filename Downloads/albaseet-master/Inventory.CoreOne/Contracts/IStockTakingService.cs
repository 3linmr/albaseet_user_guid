using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.CoreOne.Models.Dtos.ViewModels;

namespace Inventory.CoreOne.Contracts
{
	public interface IStockTakingService
	{
		List<RequestChangesDto> GetStockTakingRequestChanges(StockTakingDto oldItem, StockTakingDto newItem);
		Task<StockTakingDto> GetStockTaking(int stockTakingHeaderId);
        Task<List<StockTakingCarryOverDetailDto>> GetStockTakingCompareData(bool isOpenBalance ,int storeId, DateTime carryOverDate, List<int> stockTakingIds);
		Task<ResponseDto> SaveStockTaking(StockTakingDto stockTaking, bool hasApprove, bool approved, int? requestId);
		Task<ResponseDto> DeleteStockTaking(int stockTakingHeaderId);
	}
}
