using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.CoreOne.Models.Domain;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Helper.Models.Dtos;

namespace Inventory.CoreOne.Contracts
{
	public interface IStockTakingDetailService : IBaseService<StockTakingDetail>
	{
		Task<List<StockTakingDetailDto>> GetStockTakingDetails(int stockTakingHeaderId, int storeId);
		Task<bool> SaveStockTakingDetails(int stockTakingHeaderId,List<StockTakingDetailDto> stockTakingDetails);
		Task<bool> DeleteStockTakingDetails(int stockTakingHeaderId);
	}
}
