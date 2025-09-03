using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
    public interface ISupplierQuotationDetailService : IBaseService<SupplierQuotationDetail>
    {
        Task<List<SupplierQuotationDetailDto>> GetSupplierQuotationDetails(int supplierQuotationHeaderId);
        Task<List<SupplierQuotationDetailDto>> SaveSupplierQuotationDetails(int supplierQuotationHeaderId, List<SupplierQuotationDetailDto> supplierQuotationDetails);
        Task<bool> DeleteSupplierQuotationDetails(int supplierQuotationHeaderId);
        Task<bool> DeleteSupplierQuotationDetailList(List<SupplierQuotationDetailDto> details, int headerId);
    }
}
