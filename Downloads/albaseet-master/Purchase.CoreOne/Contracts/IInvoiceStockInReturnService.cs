using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
	public interface IInvoiceStockInReturnService : IBaseService<InvoiceStockInReturn>
	{
		IQueryable<int> GetStockInReturnsLinkedToPurchaseInvoiceReturn(int purchaseInvoiceReturnHeaderId);
		Task LinkStockInReturnsToPurchaseInvoiceReturn(int purchaseInvoiceReturnHeaderId, List<int> stockInReturnHeaderIds);
		Task UnlinkStockInReturnsFromPurchaseInvoiceReturn(int purchaseInvoiceReturnHeaderId);
	}
}
