using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.CoreOne.Models.Dtos.ViewModels;

namespace Sales.CoreOne.Contracts
{
    public interface IStockOutHandlingService
    {
        IQueryable<StockOutHeaderDto> GetStockOutHeadersByStoreId(int storeId, int? clientId, int stockTypeId, int stockOutHeaderId);
		Task<StockOutWithResponseDto> GetStockOutFromProformaInvoice(int proformaInvoiceHeaderId);
        Task<StockOutWithResponseDto> GetStockOutFromSalesInvoice(int salesInvoiceHeaderId);
        Task<int> GetParentMenuCode(int? proformaInvoiceHeaderId, int? salesInvoiceHeaderId);
		Task<ResponseDto> SaveStockOut(StockOutDto stockOut, bool hasApprove, bool approved, int? requestId, bool affectBalances = true, string? documentReference = null, bool shouldInitializeFlags = false);
        Task<ResponseDto> CheckStockOutIsValidForSave(StockOutDto stockOut, int menuCode, int parentMenuCode);
		Task<ResponseDto> DeleteStockOut(int stockOutHeaderId, int menuCode, bool affectBalances = true);
        Task<ResponseDto> CheckStockOutIsValidForDelete(int stockOutHeaderId, int? proformaInvoiceHeaderId, int? salesInvoiceHeaderId, bool isBlocked, bool isEnded, int menuCode);
		Task<List<StockOutDetailDto>> GetStockOutDetailsCalculated(int stockOutHeaderId);
        Task<List<StockOutDetailDto>> ModifyStockOutDetailsWithLiveAvailableQuantity(int stockOutHeaderId, int? proformaInvoiceHeaderId, int? salesInvoiceHeaderId, List<StockOutDetailDto> stockOutDetails);
        Task ModifyStockOutDetailsWithStoreIdAndAvailableBalance(int stockOutHeaderId, int storeId, List<StockOutDetailDto> details, bool isRequestData, bool isCreate);
		Task<StockOutDto> GetStockOut(int stockOutHeaderId);
    }
}
