using Sales.CoreOne.Models.Dtos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
	public interface IDirectSalesInvoiceFromClientQuotationApprovalService
	{
		Task<SalesInvoiceDetailsWithResponseDto> GetSalesInvoiceDetailsFromClientQuotationApproval(int clientQuotationApprovalHeaderId, int storeId, decimal headerDiscountPercent, bool isOnTheWay, bool isCreditPayment);
		Task<SalesInvoiceDetailsWithResponseDto> DistributeQuantityOnAvailabileBatches(List<SalesInvoiceDetailDto> details, int storeId);
		List<SalesInvoiceDetailDto> RecalculateSalesInvoiceDetailValues(List<SalesInvoiceDetailDto> salesInvoiceDetails, decimal headerDiscountPercent);
	}
}
