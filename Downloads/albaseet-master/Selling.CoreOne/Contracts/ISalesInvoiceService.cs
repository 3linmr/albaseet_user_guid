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
    public interface ISalesInvoiceService
    {
        List<RequestChangesDto> GetSalesInvoiceRequestChanges(SalesInvoiceDto oldItem, SalesInvoiceDto newItem);
		Task ModifySalesInvoiceDetailsWithStoreIdAndAvaialbleBalance(int salesInvoiceHeaderId, int storeId, List<SalesInvoiceDetailDto> details, bool isRequestData, bool isCreate);
        Task ModifySalesInvoiceCreditLimits(SalesInvoiceHeaderDto salesInvoiceHeader);
		Task<SalesInvoiceDto> GetSalesInvoice(int salesInvoiceHeaderId);
        Task<List<SalesInvoiceCollectionDto>> AddNonincludedPaymentMethods(List<SalesInvoiceCollectionDto> invoiceCollections, int storeId);
		Task<ResponseDto> SaveSalesInvoice(SalesInvoiceDto salesInvoice, bool hasApprove, bool approved, int? requestId, string? documentReference = null);
        Task<ResponseDto> DeleteSalesInvoice(int salesInvoiceHeaderId, int menuCode);
        Task<decimal> GetLastSalesPrice(int itemId, int itemPackageId);
        Task<List<LastSalesPriceDto>> GetMultipleLastSalesPrices(List<int> itemIds);
    }
}
