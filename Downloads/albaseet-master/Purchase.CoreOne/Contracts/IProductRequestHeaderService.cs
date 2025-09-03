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
    public interface IProductRequestHeaderService : IBaseService<ProductRequestHeader>
    {
        IQueryable<ProductRequestHeaderDto> GetProductRequestHeaders();
        IQueryable<ProductRequestHeaderDto> GetUserProductRequestHeaders();
        IQueryable<ProductRequestHeaderDto> GetProductRequestHeadersByStoreId(int storeId, int productRequestHeaderId);
        Task<ProductRequestHeaderDto?> GetProductRequestHeaderById(int id);
        Task<DocumentCodeDto> GetProductRequestCode(int storeId, DateTime documentDate);
        Task<ResponseDto> UpdateClosed(int? productRequestHeaderId, bool isClosed);
        Task<ResponseDto> SaveProductRequestHeader(ProductRequestHeaderDto productRequest, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteProductRequestHeader(int id);
    }
}
