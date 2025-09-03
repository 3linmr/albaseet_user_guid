using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
    public interface IStockInReturnHandlingService
    {
        Task<StockInReturnDto> GetStockInReturnFromStockIn(int stockInHeaderId);
        Task<StockInReturnDto> GetStockInReturnFromPurchaseInvoice(int purchaseInvoiceHeaderId);
        Task<ResponseDto> SaveStockInReturn(StockInReturnDto stockInReturn, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteStockInReturn(int stockInReturnHeaderId, int menuCode);
        Task<List<StockInReturnDetailDto>> GetStockInReturnDetailsCalculated(int stockInReturnHeaderId, int? stockInHeaderId = null, int? purchaseInvoiceHeaderId = null, List<StockInReturnDetailDto>? stockInReturnDetails = null);
        Task<StockInReturnDto> GetStockInReturn(int stockInReturnHeaderId);
    }
}
