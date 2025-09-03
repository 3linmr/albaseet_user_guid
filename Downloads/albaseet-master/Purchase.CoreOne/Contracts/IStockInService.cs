using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
    public interface IStockInService
    {
        List<RequestChangesDto> GetStockInRequestChanges(StockInDto oldItem, StockInDto newItem);
        Task<ResponseDto> SaveStockIn(StockInDto stockIn, bool hasApprove, bool approved, int? requestId, string? documentReference = null, bool shouldInitializeFlags = false);
        Task<ResponseDto> DeleteStockIn(int stockInHeaderId, int menuCode);
    }
}
