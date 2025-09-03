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
    public interface IStockInReturnService
    {
        List<RequestChangesDto> GetStockInReturnRequestChanges(StockInReturnDto oldItem, StockInReturnDto newItem);
        Task<ResponseDto> SaveStockInReturn(StockInReturnDto stockInReturn, bool hasApprove, bool approved, int? requestId, string? documentReference = null, bool shouldInitializeFlags = false);
        Task<ResponseDto> DeleteStockInReturn(int stockInReturnHeaderId, int menuCode);
    }
}
