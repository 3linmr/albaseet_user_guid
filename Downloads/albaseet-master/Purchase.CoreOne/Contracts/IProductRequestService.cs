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
    public interface IProductRequestService
    {
        List<RequestChangesDto> GetProductRequestRequestChanges(ProductRequestDto oldItem, ProductRequestDto newItem);
        Task<ProductRequestDto> GetProductRequest(int productRequestHeaderId);
        Task<ResponseDto> SaveProductRequest(ProductRequestDto productRequest, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteProductRequest(int productRequestHeaderId);
    }
}
