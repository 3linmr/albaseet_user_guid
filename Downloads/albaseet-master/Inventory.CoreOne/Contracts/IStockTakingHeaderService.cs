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
	public interface IStockTakingHeaderService : IBaseService<StockTakingHeader>
	{
		IQueryable<StockTakingHeaderDto> GetStockTakingHeaders();
		IQueryable<StockTakingHeaderDto> GetUserStockTakingHeaders(bool isOpenBalance);
        IQueryable<StockTakingHeaderDto> GetStockTakingHeaders(bool isOpenBalance);
		Task<List<StockTakingDropDownDto>> GetPendingStockTakings(int storeId,bool isOpenBalance);
		IQueryable<StockTakingHeaderDto> GetStockTakingHeadersByStoreId(int storeId);
		Task<StockTakingHeaderDto?> GetStockTakingHeaderById(int id);
		Task<DocumentCodeDto> GetStockTakingCode(int storeId, DateTime stockDate, bool isOpenBalance);
		Task<ResponseDto> SaveStockTakingHeader(StockTakingHeaderDto stockTaking, bool hasApprove, bool approved, int? requestId);
		Task<bool> UpdateStockTakingToBeCarriedOver(List<int> ids);
		Task<bool> UpdateStockTakingToBeUnCarriedOver(List<int> ids);
		Task<ResponseDto> DeleteStockTakingHeader(int id);
	}
}
