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
    public interface IStockOutHeaderService: IBaseService<StockOutHeader>
    {
        IQueryable<StockOutHeaderDto> GetStockOutHeaders();
        Task<StockOutHeaderDto?> GetStockOutHeaderById(int id);
        IQueryable<StockOutHeaderDto> GetUserStockOutHeaders(int stockTypeId);
        Task<DocumentCodeDto> GetStockOutCode(int storeId, DateTime documentDate, int stockTypeId);
        Task<bool> UpdateClosed(int? stockOutHeaderId, bool isClosed);
        Task<bool> UpdateAllStockOutsBlockedFromProformaInvoice(int proformaInvoiceHeaderId, bool isBlocked);
        Task<int> UpdateAllStockOutsEndedDirectlyFromSalesInvoice(int salesInvoiceHeaderId, bool isEnded);
		Task<bool> UpdateStockOutsEnded(List<int> stockOutHeaderIds, bool isEnded);
		Task<ResponseDto> SaveStockOutHeader(StockOutHeaderDto stockOut, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags);
        Task<int> GetNextId();
		Task<ResponseDto> DeleteStockOutHeader(int id, int menuCode);
    }
}
