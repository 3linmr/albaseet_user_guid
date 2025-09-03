using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
    public interface IStockOutReturnHeaderService: IBaseService<StockOutReturnHeader>
    {
        IQueryable<StockOutReturnHeaderDto> GetStockOutReturnHeaders();
        IQueryable<StockOutReturnHeaderDto> GetStockOutReturnHeadersByStoreId(int storeId, int? clientId, int stockTypeId, int stockOutReturnByStoreId);
        Task<StockOutReturnHeaderDto?> GetStockOutReturnHeaderById(int id);
        IQueryable<StockOutReturnHeaderDto> GetUserStockOutReturnHeaders(int stockTypeId);
        Task<DocumentCodeDto> GetStockOutReturnCode(int storeId, DateTime documentDate, int stockTypeId);
        Task<bool> UpdateAllStockOutReturnsBlockedFromProformaInvoice(int proformaInvoiceHeaderId, bool isBlocked);
        Task<int> UpdateAllStockOutReturnReservationFromSalesInvoice(int salesInvoiceHeaderId, bool isEnded);
        Task<int> UpdateAllStockOutReturnsEndedDirectlyFromSalesInvoice(int salesInvoiceHeaderId, bool isEnded);
		Task<bool> UpdateStockOutReturnsEnded(List<int> stockOutReturnHeaderIds, bool isEnded);
		Task<ResponseDto> SaveStockOutReturnHeader(StockOutReturnHeaderDto stockOutReturn, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags);
        Task<ResponseDto> DeleteStockOutReturnHeader(int id, int menuCode);
        Task<int> GetNextId();

	}
}
