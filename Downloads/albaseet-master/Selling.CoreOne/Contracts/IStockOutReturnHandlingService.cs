using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
    public interface IStockOutReturnHandlingService
    {
        Task<StockOutReturnDto> GetStockOutReturnFromStockOut(int stockOutHeaderId);
        Task<StockOutReturnDto> GetStockOutReturnFromSalesInvoice(int salesInvoiceHeaderId);
        Task<int> GetParentMenuCode(int? stockOutHeaderId, int? salesInvoiceHeaderId);
        Task<int?> GetGrandParentMenuCode(int? stockOutHeaderId);
		Task<ResponseDto> CheckStockOutReturnIsValidForSave(StockOutReturnDto stockOutReturn, int menuCode, int parentMenuCode, int? grandParentMenuCode);
		Task<ResponseDto> SaveStockOutReturn(StockOutReturnDto stockOutReturn, bool hasApprove, bool approved, int? requestId, bool affectBalances = true, string? documentReference = null, bool shouldInitializeFlags = false);
        Task<ResponseDto> CheckStockOutReturnIsValidForDelete(int stockOutReturnHeaderId, int storeId, int? stockOutHeaderId, bool isEnded, bool isBlocked, int menuCode, int parentMenuCode, int? grandParentMenuCode);
		Task<ResponseDto> DeleteStockOutReturn(int stockOutReturnHeaderId, int menuCode, bool affectBalances = true);
        Task<List<StockOutReturnDetailDto>> GetStockOutReturnDetailsCalculated(int stockOutReturnHeaderId);
        Task<List<StockOutReturnDetailDto>> ModifyStockOutReturnDetailsWithLiveAvailableQuantity(int stockOutReturnHeaderId, int? stockOutHeaderId, int? salesInvoiceHeaderId, List<StockOutReturnDetailDto> stockOutReturnDetails);
		Task<StockOutReturnDto> GetStockOutReturn(int stockOutReturnHeaderId);
    }
}
