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
	public interface IStockTakingCarryOverDetailService : IBaseService<StockTakingCarryOverDetail>
	{
		IQueryable<StockTakingCarryOverDetailDto> GetCarryOversDetailsById(int id);
		Task<bool> SaveCarryOverDetails(int headerId,List<StockTakingCarryOverDetailDto> details);
		Task<bool> DeleteCarryOverDetails(int headerId);
	}
}
