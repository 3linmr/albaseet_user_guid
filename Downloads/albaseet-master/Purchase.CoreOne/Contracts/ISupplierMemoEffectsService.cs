using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
	public interface ISupplierMemoEffectsService
	{
		Task MarkAllDocumentsLinkedToPurchaseInvoiceAsEnded(int purchaseInvoiceHeaderId);
		Task ReopenAllDocumentsRelatedToPurchaseInvoice(int purchaseInvoiceHeaderId);
	}
}
