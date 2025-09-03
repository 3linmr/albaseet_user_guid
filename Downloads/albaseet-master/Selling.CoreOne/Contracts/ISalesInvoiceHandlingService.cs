using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Domain.Modules;

namespace Sales.CoreOne.Contracts
{
    public interface ISalesInvoiceHandlingService
    {
        Task<SalesInvoiceDto> GetSalesInvoiceFromProformaInvoice(int proformaInvoiceHeaderId);
        Task<SalesInvoiceWithResponseDto> GetSalesInvoiceFromClientQuotationApproval(int clientQuotationApprovalHeaderId, bool isOnTheWay, bool isCreditPayment);
        IQueryable<SalesInvoiceHeaderDto> GetSalesInvoiceHeadersByStoreIdAndMenuCode(int storeId, int? clientId, int menuCode, int salesInvoiceHeaderId);
        Task<ExpirationDaysAndDateDto> GetDefaultValidReturnDate(int storeId);
        Task<ResponseDto> CheckSalesInvoiceIsValidForSave(SalesInvoiceDto salesInvoice, int menuCode);
		Task<ResponseDto> SaveSalesInvoice(SalesInvoiceDto salesInvoice, bool hasApprove, bool approved, int? requestId, string? documentRefernce = null);
        Task<ResponseDto> CheckSalesInvoiceIsValidForDelete(bool isEnded, bool isBlocked, bool hasSettlement, int menuCode);
		Task<ResponseDto> DeleteSalesInvoice(int salesInvoiceHeaderId, int menuCode);
    }
}
