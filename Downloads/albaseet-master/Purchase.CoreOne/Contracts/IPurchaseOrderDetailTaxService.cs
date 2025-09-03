using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
    public interface IPurchaseOrderDetailTaxService: IBaseService<PurchaseOrderDetailTax>
    {
        IQueryable<PurchaseOrderDetailTaxDto> GetPurchaseOrderDetailTaxes(int purchaseOrderHeaderId);
        Task<bool> SavePurchaseOrderDetailTaxes(int purchaseOrderHeaderId, List<PurchaseOrderDetailTaxDto> purchaseOrderDetailTaxes);
        Task<bool> DeletePurchaseOrderDetailTaxes(int purchaseOrderHeaderId);
    }
}
