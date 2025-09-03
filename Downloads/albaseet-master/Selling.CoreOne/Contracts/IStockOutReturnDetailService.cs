using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
    public interface IStockOutReturnDetailService: IBaseService<StockOutReturnDetail>
    {
        IQueryable<StockOutReturnDetailDto> GetStockOutReturnDetailsAsQueryable(int stockOutReturnHeaderId);
		Task<List<StockOutReturnDetailDto>> GetStockOutReturnDetails(int stockOutReturnHeaderId);
        List<StockOutReturnDetailDto> GroupStockOutReturnDetailsWithAllKeys(List<StockOutReturnDetailDto> details);
        List<StockOutReturnDetailDto> GroupStockOutReturnDetails(List<StockOutReturnDetailDto> details);
		Task<List<StockOutReturnDetailDto>> SaveStockOutReturnDetails(int stockOutReturnHeaderId, List<StockOutReturnDetailDto> stockOutReturnDetails);
        Task<bool> DeleteStockOutReturnDetails(int stockOutReturnHeaderId);
        Task<bool> DeleteStockOutReturnDetailList(List<StockOutReturnDetailDto> details, int headerId);
    }
}
