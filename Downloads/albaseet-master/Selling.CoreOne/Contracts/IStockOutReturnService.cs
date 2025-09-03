using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
    public interface IStockOutReturnService
    {
        List<RequestChangesDto> GetStockOutReturnRequestChanges(StockOutReturnDto oldItem, StockOutReturnDto newItem);
        Task<StockOutReturnDto> GetStockOutReturn(int stockOutReturnHeaderId);
        Task<ResponseDto> SaveStockOutReturn(StockOutReturnDto stockOutReturn, bool hasApprove, bool approved, int? requestId, string? documentReference = null, bool shouldInitializeFlags = false);
        Task<ResponseDto> SaveStockOutReturnWithoutUpdatingBalances(StockOutReturnDto stockOutReturn, bool hasApprove, bool approved, int? requestId, string? documentReference = null, bool shouldInitializeFlags = false);
        Task<ResponseDto> DeleteStockOutReturn(int stockOutReturnHeaderId, int menuCode);
        Task<ResponseDto> DeleteStockOutReturnWithoutUpdatingBalances(int stockOutReturnHeaderId, int menuCode);
    }
}
