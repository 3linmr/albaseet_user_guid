using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;

namespace Sales.CoreOne.Contracts
{
    public interface IStockOutDetailService: IBaseService<StockOutDetail>
    {
        Task<List<StockOutDetailDto>> GetStockOutDetails(int stockOutHeaderId);
        IQueryable<StockOutDetailDto> GetStockOutDetailsAsQueryable(int stockOutHeaderId);
        List<StockOutDetailDto> GroupStockOutDetails(List<StockOutDetailDto> details);
        List<StockOutDetailDto> GroupStockOutDetailsWithAllKeys(List<StockOutDetailDto> details);
        Task<List<StockOutDetailDto>> GetStockOutDetailsGrouped(int stockOutHeaderId);
        Task<List<StockOutDetailDto>> GetStockOutDetailsGroupedWithAllKeys(int stockOutHeaderId);
		Task<List<StockOutDetailDto>> SaveStockOutDetails(int stockOutHeaderId, List<StockOutDetailDto> stockOutDetails);
        Task<bool> DeleteStockOutDetails(int stockOutHeaderId);
        Task<bool> DeleteStockOutDetailList(List<StockOutDetailDto> details, int headerId);
    }
}
