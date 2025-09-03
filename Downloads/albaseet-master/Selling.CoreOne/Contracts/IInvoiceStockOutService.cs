using Sales.CoreOne.Models.Domain;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
	public interface IInvoiceStockOutService : IBaseService<InvoiceStockOut>
	{
		IQueryable<int> GetStockOutsLinkedToSalesInvoice(int salesInvoiceHeaderId);
		Task LinkStockOutsToSalesInvoice(int salesInvoiceHeaderId, List<int> stockOutHeaderIds);
		Task UnlinkStockOutsFromSalesInvoice(int salesInvoiceHeaderId);
	}
}
