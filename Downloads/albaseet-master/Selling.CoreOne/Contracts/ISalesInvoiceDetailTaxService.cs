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
    public interface ISalesInvoiceDetailTaxService : IBaseService<SalesInvoiceDetailTax>
    {
        IQueryable<SalesInvoiceDetailTaxDto> GetSalesInvoiceDetailTaxes(int salesInvoiceHeaderId);
        Task<bool> SaveSalesInvoiceDetailTaxes(int salesInvoiceHeaderId, List<SalesInvoiceDetailTaxDto> salesInvoiceDetailTaxes);
        Task<bool> DeleteSalesInvoiceDetailTaxes(int salesInvoiceHeaderId);
    }
}
