using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
	public enum PurchaseMessageData
	{
		AlreadyHasDocument,
		AlreadyInvoiced,
		BonusQuantityExceeding,
		BonusQuantityNotMatchingReceived,
		BonusQuantityNotMatchingRemaining,
		BonusQuantityNotMatchingReturned,
		CannotDecreaseValueOfMemo,
		CannotDeleteBecauseReturnedAfterInvoice,
		CanOnlyCreateOnCreditInvoice,
		DeleteCauseBonusQuantityExceed,
		DeleteCauseInvoiceValueNegative,
		DeleteCauseQuantityExceed,
		PurchaseInvoiceAlreadyReturned,
		QuantityExceeding,
		QuantityNotMatchingReceived,
		QuantityNotMatchingRemaining,
		QuantityNotMatchingReturned,
		SaveCauseBonusQuantityExceed,
		SaveCauseQuantityExceed,
		StockInReturnFromPurchaseInvoiceCreatedBeforeInvoiceReturn,
		ValueExceeding,
		ValueNotMatchingJournalCredit,
		ValueNotMatchingJournalDebit,
	}

	public interface IPurchaseMessageService
	{
		Task<string> GetMessage(int menuCode, PurchaseMessageData message, params string[] messageParams);
		Task<string> GetMessage(int menuCode1, int menuCode2, PurchaseMessageData message, params string[] messageParams);
		Task<string> GetMessage(int menuCode1, int menuCode2, int menuCode3, PurchaseMessageData message, params string[] messageParams);
	}
}
