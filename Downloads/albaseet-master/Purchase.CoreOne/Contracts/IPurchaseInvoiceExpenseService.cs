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
    public interface IPurchaseInvoiceExpenseService : IBaseService<PurchaseInvoiceExpense>
    {
        Task<List<PurchaseInvoiceExpenseDto>> GetPurchaseInvoiceExpenses(int purchaseInvoiceHeaderId);
        Task<bool> SavePurchaseInvoiceExpenses(int purchaseInvoiceHeaderId, List<PurchaseInvoiceExpenseDto> purchaseInvoiceExpenses);
        Task<bool> DeletePurchaseInvoiceExpenses(int purchaseInvoiceHeaderId);
    }
}
