using Sales.CoreOne.Models.Dtos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
	public interface ISalesValueService
	{
		Task<decimal> GetOverallValueOfSalesInvoice(int salesInvoiceHeaderId);
		Task<decimal> GetOverallValueOfSalesInvoiceExceptClientCredit(int salesInvoiceHeaderId, int exceptClientCredit);
		Task<decimal> GetOverallValueOfSalesInvoiceExceptClientDebit(int salesInvoiceHeaderId, int exceptClientDebit);
		IQueryable<SalesInvoiceOverallValueDto> GetOverallValueOfSalesInvoices();
		IQueryable<SalesInvoiceOverallValueDto> GetOverallValueOfSalesInvoicesWithDateRange(DateTime? fromDate = null, DateTime? toDate = null);
	}
}
