using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.CoreOne.Models.Dtos.ViewModels;

namespace Sales.CoreOne.Contracts
{
    public interface ISalesInvoiceReturnHandlingService
    {
        Task<SalesInvoiceReturnDto> GetSalesInvoiceReturnFromSalesInvoice(int salesInvoiceHeaderId, bool isDirectInvoice, bool isOnTheWay);
        Task<int> GetParentMenuCode(int salesInvoiceHeaderId);
		Task<ResponseDto> CheckSalesInvoiceReturnIsValidForSave(SalesInvoiceReturnDto salesInvoiceReturn, int menuCode, int parentMenuCode);
		Task<ResponseDto> SaveSalesInvoiceReturn(SalesInvoiceReturnDto salesInvoiceReturn, bool hasApprove, bool approved, int? requestId, string? documentReference = null);
        Task<ResponseDto> CheckSalesInvoiceReturnIsValidForDelete(int salesInvoiceReturnHeaderId, int salesInvoiceHeaderId, int storeId, bool isBlocked, bool isOnTheWay, int menuCode, int parentMenuCode);
		Task<ResponseDto> DeleteSalesInvoiceReturn(int salesInvoiceReturnHeaderId, int menuCode);
    }
}
