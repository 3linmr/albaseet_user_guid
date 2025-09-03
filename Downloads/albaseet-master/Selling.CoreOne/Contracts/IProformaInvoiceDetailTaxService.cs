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
    public interface IProformaInvoiceDetailTaxService: IBaseService<ProformaInvoiceDetailTax>
    {
        IQueryable<ProformaInvoiceDetailTaxDto> GetProformaInvoiceDetailTaxes(int proformaInvoiceHeaderId);
        Task<bool> SaveProformaInvoiceDetailTaxes(int proformaInvoiceHeaderId, List<ProformaInvoiceDetailTaxDto> proformaInvoiceDetailTaxes);
        Task<bool> DeleteProformaInvoiceDetailTaxes(int proformaInvoiceHeaderId);
    }
}
