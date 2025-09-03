using Purchases.CoreOne.Models.Dtos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
	public interface IPurchaseValueService
	{
		Task<decimal> GetOverallValueOfPurchaseInvoice(int purchaseInvoiceHeaderId);
		Task<decimal> GetOverallValueOfPurchaseInvoiceExceptSupplierCredit(int purchaseInvoiceHeaderId, int exceptSupplierCredit);
		Task<decimal> GetOverallValueOfPurchaseInvoiceExceptSupplierDebit(int purchaseInvoiceHeaderId, int exceptSupplierDebit);
		IQueryable<PurchaseInvoiceOverallValueDto> GetOverallValueOfPurchaseInvoices();
	}
}
