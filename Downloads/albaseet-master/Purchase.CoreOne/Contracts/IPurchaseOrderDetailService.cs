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
    public interface IPurchaseOrderDetailService : IBaseService<PurchaseOrderDetail>
    {
        Task<List<PurchaseOrderDetailDto>> GetPurchaseOrderDetails(int purchaseOrderHeaderId);
        IQueryable<PurchaseOrderDetailDto> GetPurchaseOrderDetailsAsQueryable(int purchaseOrderHeaderId);
        Task<List<PurchaseOrderDetailDto>> GetPurchaseOrderDetailsGrouped(int purchaseOrderHeaderId);
        Task<List<PurchaseOrderDetailDto>> GetPurchaseOrderDetailsFullyGrouped(int purchaseOrderHeaderId);
        IQueryable<PurchaseOrderDetailDto> GetPurchaseOrderDetailsGroupedQueryable(int purchaseOrderHeaderId);
		List<PurchaseOrderDetailDto> GroupPurchaseOrderDetails(List<PurchaseOrderDetailDto> details);
		Task<List<PurchaseOrderDetailDto>> SavePurchaseOrderDetails(int purchaseOrderHeaderId, List<PurchaseOrderDetailDto> purchaseOrderDetails);
        Task<bool> DeletePurchaseOrderDetails(int purchaseOrderHeaderId);
        Task<bool> DeletePurchaseOrderDetailList(List<PurchaseOrderDetailDto> details, int headerId);
    }
}
