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
    public interface ISalesInvoiceDetailService: IBaseService<SalesInvoiceDetail>
    {
        IQueryable<SalesInvoiceDetailDto> GetSalesInvoiceDetailsAsQueryable(int salesInvoiceHeaderId);
		Task<List<SalesInvoiceDetailDto>> GetSalesInvoiceDetails(int salesInvoiceHeaderId);
        Task<List<SalesInvoiceDetailDto>> GetSalesInvoiceDetailsGrouped(int salesInvoiceHeaderId);
        IQueryable<SalesInvoiceDetailDto> GetSalesInvoiceDetailsGroupedQueryable(int salesInvoiceHeaderId);
		List<SalesInvoiceDetailDto> GroupSalesInvoiceDetails(List<SalesInvoiceDetailDto> details);
        List<SalesInvoiceDetailDto> GroupSalesInvoiceDetailsWithoutExpireAndBatch(List<SalesInvoiceDetailDto> details);
		Task<List<SalesInvoiceDetailDto>> SaveSalesInvoiceDetails(int salesInvoiceHeaderId, List<SalesInvoiceDetailDto> salesInvoiceDetails);
        Task<bool> DeleteSalesInvoiceDetails(int salesInvoiceHeaderId);
        Task<bool> DeleteSalesInvoiceDetailList(List<SalesInvoiceDetailDto> details, int headerId);
    }
}
