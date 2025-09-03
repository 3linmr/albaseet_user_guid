using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
	public interface IGetSalesInvoiceSettleValueService
	{
		Task<decimal> GetSalesInvoiceSettleValue(int salesInvoiceHeaderId);
		Task<ResponseDto> CheckSettlementExceedingAndUpdateSettlementCompletedFlag(int salesInvoiceHeaderId, int menuCode, int parentMenuCode, bool isSave);
		Task<decimal> GetSalesInvoiceValueMinusSettledValue(int salesInvoiceHeaderId, int clientCreditMemoId = 0);
	}
}
