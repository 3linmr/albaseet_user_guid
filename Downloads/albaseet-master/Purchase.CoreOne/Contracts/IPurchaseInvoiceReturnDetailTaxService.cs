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
    public interface IPurchaseInvoiceReturnDetailTaxService: IBaseService<PurchaseInvoiceReturnDetailTax>
    {
        IQueryable<PurchaseInvoiceReturnDetailTaxDto> GetPurchaseInvoiceReturnDetailTaxes(int purchaseInvoiceReturnHeaderId);
        Task<bool> SavePurchaseInvoiceReturnDetailTaxes(int purchaseInvoiceReturnHeaderId, List<PurchaseInvoiceReturnDetailTaxDto> purchaseInvoiceReturnDetailTaxes);
        Task<bool> DeletePurchaseInvoiceReturnDetailTaxes(int purchaseInvoiceReturnHeaderId);
    }
}
