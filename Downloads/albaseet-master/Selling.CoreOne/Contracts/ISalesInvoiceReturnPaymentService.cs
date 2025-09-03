using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
	public interface ISalesInvoiceReturnPaymentService: IBaseService<SalesInvoiceReturnPayment>
	{
		Task<List<SalesInvoiceReturnPaymentDto>> GetSalesInvoiceReturnPayments(int salesInvoiceReturnHeaderId, int storeId);
		Task<List<SalesInvoiceReturnPaymentDto>> SaveSalesInvoiceReturnPayments(int salesInvoiceReturnHeaderId, List<SalesInvoiceReturnPaymentDto> salesInvoiceReturnPayments);
		Task<bool> DeleteSalesInvoiceReturnPayments(int salesInvoiceReturnHeaderId);
	}
}
