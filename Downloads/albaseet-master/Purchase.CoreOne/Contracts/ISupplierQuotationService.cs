using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
    public interface ISupplierQuotationService
    {
        List<RequestChangesDto> GetSupplierQuotationRequestChanges(SupplierQuotationDto oldItem, SupplierQuotationDto newItem);
        Task<SupplierQuotationDto> GetSupplierQuotation(int supplierQuotationHeaderId);
        Task<SupplierQuotationDto> GetSupplierQuotationFromProductRequestPrice(int productRequestPriceHeaderId);
        Task<ResponseDto> SaveSupplierQuotation(SupplierQuotationDto supplierQuotation, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteSupplierQuotation(int supplierQuotationHeaderId);
    }
}
