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
    public interface IPurchaseInvoiceDetailService : IBaseService<PurchaseInvoiceDetail>
    {
        Task<List<PurchaseInvoiceDetailDto>> GetPurchaseInvoiceDetails(int purchaseInvoiceHeaderId);
        IQueryable<PurchaseInvoiceDetailDto> GetPurchaseInvoiceDetailsAsQueryable(int purchaseInvoiceHeaderId);
		Task<List<PurchaseInvoiceDetailDto>> GetPurchaseInvoiceDetailsGrouped(int purchaseInvoiceHeaderId);
        IQueryable<PurchaseInvoiceDetailDto> GetPurchaseInvoiceDetailsGroupedQueryable(int purchaseInvoiceHeaderId);
		List<PurchaseInvoiceDetailDto> GroupPurchaseInvoiceDetails(List<PurchaseInvoiceDetailDto> details);
        List<PurchaseInvoiceDetailDto> GroupPurchaseInvoiceDetailsWithoutExpireAndBatch(List<PurchaseInvoiceDetailDto> details);
		Task<List<PurchaseInvoiceDetailDto>> SavePurchaseInvoiceDetails(int purchaseInvoiceHeaderId, List<PurchaseInvoiceDetailDto> purchaseInvoiceDetails);
        Task<bool> DeletePurchaseInvoiceDetailList(List<PurchaseInvoiceDetailDto> purchaseInvoiceDetails, int purchaseInvoiceHeaderId);
        Task<bool> DeletePurchaseInvoiceDetails(int purchaseInvoiceHeaderId);
    }
}
