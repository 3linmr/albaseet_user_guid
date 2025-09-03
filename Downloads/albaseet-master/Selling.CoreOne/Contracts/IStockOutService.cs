using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
    public interface IStockOutService
    {
        List<RequestChangesDto> GetStockOutRequestChanges(StockOutDto oldItem, StockOutDto newItem);
        Task<StockOutDto> GetStockOut(int stockOutHeaderId);
        Task<ResponseDto> SaveStockOut(StockOutDto stockOut, bool hasApprove, bool approved, int? requestId, string? documentReference = null, bool shouldInitializeFlags = false);
        Task<ResponseDto> SaveStockOutWithoutUpdatingBalances(StockOutDto stockOut, bool hasApprove, bool approved, int? requestId, string? documentReference = null, bool shouldInitializeFlags = false);
        Task<ResponseDto> DeleteStockOut(int stockOutHeaderId, int menuCode);
        Task<ResponseDto> DeleteStockOutWithoutUpdatingBalances(int stockOutHeaderId, int menuCode);
    }
}
