using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
    public interface ISupplierQuotationDetailTaxService: IBaseService<SupplierQuotationDetailTax>
    {
        IQueryable<SupplierQuotationDetailTaxDto> GetSupplierQuotationDetailTaxes(int supplierQuotationHeaderId);
        Task<bool> SaveSupplierQuotationDetailTaxes(int supplierQuotationHeaderId, List<SupplierQuotationDetailTaxDto> supplierQuotationDetailTaxes);
        Task<bool> DeleteSupplierQuotationDetailTaxes(int supplierQuotationHeaderId);
    }
}
