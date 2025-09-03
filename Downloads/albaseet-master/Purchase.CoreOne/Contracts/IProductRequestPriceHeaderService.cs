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
    public interface IProductRequestPriceHeaderService : IBaseService<ProductRequestPriceHeader>
    {
        IQueryable<ProductRequestPriceHeaderDto> GetProductRequestPriceHeaders();
        IQueryable<ProductRequestPriceHeaderDto> GetProductRequestPriceHeadersByStoreId(int storeId, int? supplierId, int productRequestPriceHeaderId);
        Task<ProductRequestPriceHeaderDto?> GetProductRequestPriceHeaderById(int id);
        IQueryable<ProductRequestPriceHeaderDto> GetUserProductRequestPriceHeaders();
        Task<DocumentCodeDto> GetProductRequestPriceCode(int storeId, DateTime documentDate);
        Task<ResponseDto> UpdateClosed(int? productRequestPriceHeaderId, bool isClosed);
        Task<ResponseDto> SaveProductRequestPriceHeader(ProductRequestPriceHeaderDto productRequestPrice, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteProductRequestPriceHeader(int id);
    }
}
