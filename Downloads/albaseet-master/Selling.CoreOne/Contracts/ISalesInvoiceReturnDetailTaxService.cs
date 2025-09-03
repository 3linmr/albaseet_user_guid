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
    public interface ISalesInvoiceReturnDetailTaxService : IBaseService<SalesInvoiceReturnDetailTax>
    {
        IQueryable<SalesInvoiceReturnDetailTaxDto> GetSalesInvoiceReturnDetailTaxes(int salesInvoiceReturnHeaderId);
        Task<bool> SaveSalesInvoiceReturnDetailTaxes(int salesInvoiceReturnHeaderId, List<SalesInvoiceReturnDetailTaxDto> salesInvoiceReturnDetailTaxes);
        Task<bool> DeleteSalesInvoiceReturnDetailTaxes(int salesInvoiceReturnHeaderId);
    }
}
