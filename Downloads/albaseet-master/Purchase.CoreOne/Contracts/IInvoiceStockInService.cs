using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
	public interface IInvoiceStockInService : IBaseService<InvoiceStockIn>
	{
		IQueryable<int> GetStockInsLinkedToPurchaseInvoice(int purchaseInvoiceHeaderId);
		Task LinkStockInsToPurchaseInvoice(int purchaseInvoiceHeaderId, List<int> stockInHeaderIds);
		Task UnlinkStockInsFromPurchaseInvoice(int purchaseInvoiceHeaderId);
	}
}
