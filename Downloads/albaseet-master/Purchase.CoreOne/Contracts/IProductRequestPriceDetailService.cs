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
    public interface IProductRequestPriceDetailService : IBaseService<ProductRequestPriceDetail>
    {
        Task<List<ProductRequestPriceDetailDto>> GetProductRequestPriceDetails(int productRequestPriceHeaderId);
        Task<List<ProductRequestPriceDetailDto>> SaveProductRequestPriceDetails(int productRequestPriceHeaderId, List<ProductRequestPriceDetailDto> productRequestPriceDetails);
        Task<bool> DeleteProductRequestPriceDetails(int productRequestPriceHeaderId);
        Task<bool> DeleteProductRequestPriceDetailList(List<ProductRequestPriceDetailDto> details, int headerId);
    }
}
