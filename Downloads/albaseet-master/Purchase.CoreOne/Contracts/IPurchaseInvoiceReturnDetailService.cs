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
    public interface IPurchaseInvoiceReturnDetailService : IBaseService<PurchaseInvoiceReturnDetail>
    {
        IQueryable<PurchaseInvoiceReturnDetailDto> GetPurchaseInvoiceReturnDetailsAsQueryable(int purchaseInvoiceReturnHeaderId);
		Task<List<PurchaseInvoiceReturnDetailDto>> GetPurchaseInvoiceReturnDetails(int purchaseInvoiceReturnHeaderId);
        List<PurchaseInvoiceReturnDetailDto> GroupPurchaseInvoiceReturnDetails(List<PurchaseInvoiceReturnDetailDto> details);
		Task<List<PurchaseInvoiceReturnDetailDto>> SavePurchaseInvoiceReturnDetails(int purchaseInvoiceReturnHeaderId, List<PurchaseInvoiceReturnDetailDto> purchaseInvoiceReturnDetails);
        Task<bool> DeletePurchaseInvoiceReturnDetailList(List<PurchaseInvoiceReturnDetailDto> purchaseInvoiceReturnDetails, int purchaseInvoiceReturnHeaderId);
        Task<bool> DeletePurchaseInvoiceReturnDetails(int purchaseInvoiceReturnHeaderId);
    }
}
