using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
	public enum SalesMessageData
	{
		AlreadyHasDocument,
		BonusQuantityExceeding,
		BonusQuantityNotMatchingRemaining,
		BonusQuantityNotMatchingUnInvoiced,
		CannotCompletelyFulfillItemFromStore,
		CannotDecreaseValueOfMemo,
		CannotDeleteBecauseReturnedAfterInvoice,
		CannotPartiallyFulfillItemFromStore,
		CanOnlyCreateOnCreditInvoice,
		CollectedValueNotMatchTotalValue,
		DeleteCauseBonusQuantityExceed,
		DeleteCauseInvoiceValueNegative,
		DeleteCauseQuantityExceed,
		MustHavePaymentMethod,
		NoCollectionMethodWithCreditInvoices,
		NoLongerCanReturn,
		NoLongerValid,
		NoPaymentMethodWithCreditInvoices,
		PaidValueNotMatchTotalValue,
		QuantityCannotBeChanged,
		QuantityExceeding,
		QuantityNotMatchingRemaining,
		QuantityNotMatchingUnInvoiced,
		SaveCauseBonusQuantityExceed,
		SaveCauseQuantityExceed,
		StockOutReturnFromReservationInvoiceCreatedBeforeCloseOut,
		ValueExceeding,
		ValueNotMatchingJournalCredit,
		ValueNotMatchingJournalDebit,
	}

	public interface ISalesMessageService
	{
		string GetMessage(SalesMessageData message, params string[] messageParams);
		Task<string> GetMessage(int menuCode, SalesMessageData message, params string[] messageParams);
		Task<string> GetMessage(int menuCode1, int menuCode2, SalesMessageData message, params string[] messageParams);
		Task<string> GetMessage(int menuCode1, int menuCode2, int menuCode3, SalesMessageData message, params string[] messageParams);
	}
}
