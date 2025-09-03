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
    public interface IProductRequestDetailService : IBaseService<ProductRequestDetail>
    {
        Task<List<ProductRequestDetailDto>> GetProductRequestDetails(int productRequestHeaderId);
        Task<bool> SaveProductRequestDetails(int productRequestHeaderId, List<ProductRequestDetailDto> productRequestDetails);
        Task<bool> DeleteProductRequestDetails(int productRequestHeaderId);
    }
}
