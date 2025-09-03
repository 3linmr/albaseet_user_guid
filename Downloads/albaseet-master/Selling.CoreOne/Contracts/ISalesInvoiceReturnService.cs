using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
    public interface ISalesInvoiceReturnService
    {
        List<RequestChangesDto> GetSalesInvoiceReturnRequestChanges(SalesInvoiceReturnDto oldItem, SalesInvoiceReturnDto newItem);
        Task<SalesInvoiceReturnDto> GetSalesInvoiceReturn(int salesInvoiceReturnHeaderId);
        Task<List<SalesInvoiceReturnDetailDto>> GetSalesInvoiceReturnDetailsCalculated(int salesInvoiceReturnHeaderId, SalesInvoiceReturnHeaderDto? salesInvoiceReturnHeader = null, List<SalesInvoiceReturnDetailDto>? salesInvoiceReturnDetails = null);
		Task<List<SalesInvoiceReturnPaymentDto>> AddNonincludedPaymentMethods(List<SalesInvoiceReturnPaymentDto> invoicePayments, int storeId);
		Task<ResponseDto> SaveSalesInvoiceReturn(SalesInvoiceReturnDto salesInvoiceReturn, bool hasApprove, bool approved, int? requestId, string? documentReference);
        Task<ResponseDto> DeleteSalesInvoiceReturn(int salesInvoiceReturnHeaderId, int menuCode);
    }
}
