using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
	public interface IClientMemoEffectsService
	{
		Task MarkAllDocumentsLinkedToSalesInvoiceAsEnded(int salesInvoiceHeaderId); 
		Task ReopenAllDocumentsRelatedToSalesInvoice(int salesInvoiceHeaderId);
	}
}
