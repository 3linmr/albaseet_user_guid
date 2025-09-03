using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
	public interface IGetPurchaseInvoiceSettleValueService
	{
		Task<decimal> GetPurchaseInvoiceSettleValue(int purchaseInvoiceHeaderId);
		Task<ResponseDto> CheckSettlementExceedingAndUpdateSettlementCompletedFlag(int purchaseInvoiceHeaderId, int menuCode, int parentMenuCode, bool isSave);
		Task<decimal> GetPurchaseInvoiceValueMinusSettledValue(int purchaseInvoiceHeaderId, int supplierDebitMemoId = 0);
	}
}
