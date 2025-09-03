using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
    public interface IStockInDetailService : IBaseService<StockInDetail>
    {
        Task<List<StockInDetailDto>> GetStockInDetails(int stockInHeaderId);
        IQueryable<StockInDetailDto> GetStockInDetailsAsQueryable(int stockInHeaderId);
		Task<List<StockInDetailDto>> GetStockInDetailsGrouped(int stockInHeaderId);
		Task<List<StockInDetailDto>> GetStockInDetailsGroupedWithAllKeys(int stockInHeaderId);
        List<StockInDetailDto> GroupStockInDetails(List<StockInDetailDto> stockInDetails);
        List<StockInDetailDto> GroupStockInDetailsWithAllKeys(List<StockInDetailDto> stockInDetails);
        Task<List<StockInDetailDto>> SaveStockInDetails(int stockInHeaderId, List<StockInDetailDto> stockInDetails);
        Task<bool> DeleteStockInDetailList(List<StockInDetailDto> stockInDetails, int stockInHeaderId);
        Task<bool> DeleteStockInDetails(int stockInHeaderId);
    }
}
