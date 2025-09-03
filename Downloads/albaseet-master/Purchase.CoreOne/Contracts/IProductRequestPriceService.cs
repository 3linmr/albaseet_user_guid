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
    public interface IProductRequestPriceService
    {
        List<RequestChangesDto> GetProductRequestPriceRequestChanges(ProductRequestPriceDto oldItem, ProductRequestPriceDto newItem);
        Task<ProductRequestPriceDto> GetProductRequestPrice(int productRequestPriceHeaderId);
        Task<ProductRequestPriceDto> GetProductRequestPriceFromProductRequest(int productRequestHeaderId, DateTime? currentDate = null);
        Task<ResponseDto> SaveProductRequestPrice(ProductRequestPriceDto productRequestPrice, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteProductRequestPrice(int productRequestPriceHeaderId);
    }
}
