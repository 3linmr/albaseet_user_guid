using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Dtos.Reports.Suppliers;

namespace Compound.CoreOne.Contracts.Reports.Suppliers
{
	public interface IPurchaseInvoiceSettlementsReportService
	{
		IQueryable<PurchaseInvoiceSettlementsReportDto> GetPurchaseInvoiceSettlementsReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? supplierId, int? sellerId);
	}
}
