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
    public interface IStockInReturnDetailService : IBaseService<StockInReturnDetail>
    {
        Task<List<StockInReturnDetailDto>> GetStockInReturnDetails(int stockInReturnHeaderId);
        IQueryable<StockInReturnDetailDto> GetStockInReturnDetailsAsQueryable(int stockInReturnHeaderId);
        List<StockInReturnDetailDto> GroupStockInReturnDetailsWithAllKeys(List<StockInReturnDetailDto> details);
        List<StockInReturnDetailDto> GroupStockInReturnDetails(List<StockInReturnDetailDto> details);
		Task<List<StockInReturnDetailDto>> SaveStockInReturnDetails(int stockInReturnHeaderId, List<StockInReturnDetailDto> stockInReturnDetails);
        Task<bool> DeleteStockInReturnDetailList(List<StockInReturnDetailDto> stockInReturnDetails, int stockInReturnHeaderId);
        Task<bool> DeleteStockInReturnDetails(int stockInReturnHeaderId);
    }
}
