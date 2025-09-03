using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
    public interface ISalesInvoiceHeaderService: IBaseService<SalesInvoiceHeader>
    {
        IQueryable<SalesInvoiceHeaderDto> GetSalesInvoiceHeaders();
        IQueryable<SalesInvoiceHeaderDto> GetSalesInvoiceHeadersByStoreId(int storeId, int? clientId, int salesInvoiceHeaderId, bool? isOnTheWay = null);
        Task<SalesInvoiceHeaderDto?> GetSalesInvoiceHeaderById(int id);
        IQueryable<SalesInvoiceHeaderDto> GetUserSalesInvoiceHeaders(int menuCode);
        Task<DocumentCodeDto> GetSalesInvoiceCode(int storeId, DateTime documentDate, bool isOnTheWay, bool isDirectInvoice, bool creditPayment, int? sellerId);
        Task<bool> UpdateBlocked(int? salesInvoiceHeaderId, bool isBlocked);
        Task<bool> UpdateClosed(int? salesInvoiceHeaderId, bool isClosed);
        Task<bool> UpdateEnded(int? salesInvoiceHeaderId, bool isEnded);
        Task<bool> UpdateEndedAndClosed(int? salesInvoiceHeaderId, bool isEnded, bool isClosed);
        Task UpdateHasSettlementFlag(List<int> salesInvoiceHeaderIds, bool hasSettlement);
        Task UpdateIsSettlementCompletedFlags(List<int> salesInvoiceHeaderIds, bool isSettlementCompleted);
		Task UpdateAllSalesInvoicesBlockedFromProformaInvoice(int proformaInvoiceHeaderId, bool isBlocked);
        Task<ResponseDto> SaveSalesInvoiceHeader(SalesInvoiceHeaderDto salesInvoice, bool hasApprove, bool approved, int? requestId, string? documentReference);
        Task<ResponseDto> DeleteSalesInvoiceHeader(int id, int menuCode);
        Task<int> GetNextId();
    }
}
