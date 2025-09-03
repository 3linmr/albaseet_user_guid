using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Contracts.Menus
{
	public enum GenericMessageData
	{
		CannotDeleteBecauseSettlementStarted,
		CannotDeleteBecauseLessThanSettlment,
		CannotPerformOperationBecauseInvoiced,
		CannotPerformOperationBecauseStoppedDealingOn,
		CannotUpdateBecauseClosed,
		CannotUpdateBecauseSettlementStarted,
		CannotSaveBecauseLessThanSettlement,
		CannotDeleteBecauseNegativeBalance,
		CannotSaveBecauseNegativeBalance,
		CannotSaveBecauseSettled,
		ClosedSuccessfully,
		CodeAlreadyExist,
		CreatedSuccessWithCode,
		DeleteSuccessWithCode,
		DetailIsEmpty,
		HasDocument,
		NotFound,
		OpenedSuccessfully,
		OpenedToDealingOnSuccessfully,
		StoppedDealingOnSuccessfully,
		UpdatedSuccessWithCode,
	}

	public interface IGenericMessageService
	{
		Task<string> GetMessage(int menuCode, GenericMessageData message, params string[] messageParams);
		Task<string> GetMessage(int menuCode1, int menuCode2, GenericMessageData message, params string[] messageParams);
		Task<string> GetMessage(int menuCode1, int menuCode2, int menuCode3, GenericMessageData message, params string[] messageParams);
	}
}
