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
    public interface ISupplierQuotationHeaderService : IBaseService<SupplierQuotationHeader>
    {
        IQueryable<SupplierQuotationHeaderDto> GetSupplierQuotationHeaders();
        IQueryable<SupplierQuotationHeaderDto> GetSupplierQuotationHeadersByStoreId(int storeId, int? supplierId, int supplierQuotationHeaderId);
        Task<SupplierQuotationHeaderDto?> GetSupplierQuotationHeaderById(int id);
        IQueryable<SupplierQuotationHeaderDto> GetUserSupplierQuotationHeaders();
        Task<DocumentCodeDto> GetSupplierQuotationCode(int storeId, DateTime documentDate);
        Task<ResponseDto> UpdateClosed(int? supplierQuotationHeaderId, bool isClosed);
        Task<ResponseDto> SaveSupplierQuotationHeader(SupplierQuotationHeaderDto supplierQuotation, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteSupplierQuotationHeader(int id);
    }
}
