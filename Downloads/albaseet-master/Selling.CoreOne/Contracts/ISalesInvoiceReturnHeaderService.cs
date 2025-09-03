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
    public interface ISalesInvoiceReturnHeaderService: IBaseService<SalesInvoiceReturnHeader>
    {
        IQueryable<SalesInvoiceReturnHeaderDto> GetSalesInvoiceReturnHeaders();
        IQueryable<SalesInvoiceReturnHeaderDto> GetSalesInvoiceReturnHeadersByStoreId(int storeId, int? clientId, int salesInvoiceReturnHeaderId);
        Task<SalesInvoiceReturnHeaderDto?> GetSalesInvoiceReturnHeaderById(int id);
        IQueryable<SalesInvoiceReturnHeaderDto> GetUserSalesInvoiceReturnHeaders(int menuCode);
        Task<DocumentCodeDto> GetSalesInvoiceReturnCode(int storeId, DateTime documentDate, bool isOnTheWay, bool isDirectInvoice, bool creditPayment);
        Task<bool> UpdateAllSalesInvoiceReturnsBlockedFromProformaInvoice(int proformaInvoiceHeaderId, bool isBlocked);
		Task<bool> UpdateClosed(int? salesInvoiceReturnHeaderId, bool isClosed);
        Task<bool> UpdateEnded(int? salesInvoiceReturnHeaderId, bool isEnded);
        Task<int> UpdateAllSalesInvoiceReturnEndedLinkedToSalesInvoice(int? salesInvoiceHeaderId, bool isEnded);
		Task<bool> UpdateReservationInvoiceCloseOutEndedLinkedToSalesInvoice(int? salesInvoiceHeaderId, bool isEnded);
		Task<bool> UpdateEndedAndClosed(int? salesInvoiceReturnHeaderId, bool isEnded, bool isClosed);
        Task<ResponseDto> SaveSalesInvoiceReturnHeader(SalesInvoiceReturnHeaderDto salesInvoiceReturn, bool hasApprove, bool approved, int? requestId, string? documentReference);
        Task<ResponseDto> DeleteSalesInvoiceReturnHeader(int id, int menuCode);
        Task<int> GetNextId();

	}
}
