using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
    public interface ISalesInvoiceReturnDetailService: IBaseService<SalesInvoiceReturnDetail>
    {
        IQueryable<SalesInvoiceReturnDetailDto> GetSalesInvoiceReturnDetailsAsQueryable(int salesInvoiceReturnHeaderId);
		Task<List<SalesInvoiceReturnDetailDto>> GetSalesInvoiceReturnDetails(int salesInvoiceReturnHeaderId);
        List<SalesInvoiceReturnDetailDto> GroupSalesInvoiceReturnDetails(List<SalesInvoiceReturnDetailDto> details);
		Task<List<SalesInvoiceReturnDetailDto>> SaveSalesInvoiceReturnDetails(int salesInvoiceReturnHeaderId, List<SalesInvoiceReturnDetailDto> salesInvoiceReturnDetails);
        Task<bool> DeleteSalesInvoiceReturnDetails(int salesInvoiceReturnHeaderId);
        Task<bool> DeleteSalesInvoiceReturnDetailList(List<SalesInvoiceReturnDetailDto> details, int headerId);
    }
}
