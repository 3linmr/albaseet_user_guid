using Sales.CoreOne.Models.Domain;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
	public interface IInvoiceStockOutReturnService: IBaseService<InvoiceStockOutReturn>
	{
		IQueryable<int> GetStockOutReturnsLinkedToSalesInvoiceReturn(int salesInvoiceReturnHeaderId);
		Task LinkStockOutReturnsToSalesInvoiceReturn(int salesInvoiceReturnHeaderId, List<int> stockOutReturnHeaderIds);
		Task UnlinkStockOutReturnsFromSalesInvoiceReturn(int salesInvoiceReturnHeaderId);
	}
}
