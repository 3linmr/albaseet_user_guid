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
	public interface IStockTakingCarryOverHeaderService : IBaseService<StockTakingCarryOverHeader>
	{
		IQueryable<StockTakingCarryOverHeaderDto> GetStockTakingCarryOverHeaders();
		IQueryable<StockTakingCarryOverHeaderDto> GetUserStockTakingCarryOverHeaders(bool isOpenBalance);
        IQueryable<StockTakingCarryOverHeaderDto> GetStockTakingCarryOverHeaders(bool isOpenBalance);
		IQueryable<StockTakingCarryOverHeaderDto> GetStockTakingCarryOverHeadersByStoreId(int storeId);
		Task<StockTakingCarryOverHeaderDto?> GetStockTakingCarryOverHeaderById(int id);
		Task<DocumentCodeDto> GetStockTakingCarryOverCode(int storeId, DateTime stockDate, bool isOpenBalance);
		Task<ResponseDto> SaveStockTakingCarryOverHeader(StockTakingCarryOverHeaderDto stockTakingHeader, bool hasApprove, bool approved, int? requestId);
		Task<ResponseDto> DeleteStockTakingCarryOverHeader(int id);
	}
}
