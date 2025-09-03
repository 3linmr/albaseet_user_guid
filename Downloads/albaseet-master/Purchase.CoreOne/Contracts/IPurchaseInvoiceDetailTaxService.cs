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
    public interface IPurchaseInvoiceDetailTaxService: IBaseService<PurchaseInvoiceDetailTax>
    {
        IQueryable<PurchaseInvoiceDetailTaxDto> GetPurchaseInvoiceDetailTaxes(int purchaseInvoiceHeaderId);
        Task<bool> SavePurchaseInvoiceDetailTaxes(int purchaseInvoiceHeaderId, List<PurchaseInvoiceDetailTaxDto> purchaseInvoiceDetailTaxes);
        Task<bool> DeletePurchaseInvoiceDetailTaxes(int purchaseInvoiceHeaderId);
    }
}
