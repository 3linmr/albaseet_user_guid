using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.CoreOne.Contracts
{
	public interface IDirectInternalTransferService
	{
		Task<int> GetInternalTransferIdFromReferenceAndMenuCode(int referenceId, int menuCode);
		Task<ResponseDto> SaveDirectInternalTransfer(InternalTransferDto internalTransfer, bool hasApprove, bool approved, int? requestId, string? documentReference);
		Task<ResponseDto> SaveDirectInternalTransferWithoutUpdatingBalances(InternalTransferDto internalTransfer, bool hasApprove, bool approved, int? requestId, string? documentReference);
		Task<ResponseDto> DeleteDirectInternalTransfer(int internalTransferHeaderId);
		Task<ResponseDto> DeleteDirectInternalTransferWithoutUpdatingBalances(int internalTransferHeaderId);
	}
}
