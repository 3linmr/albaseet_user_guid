using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.CoreOne.Contracts
{
    public interface IInternalTransferReceiveService
    {
        Task<InternalTransferReceiveDto> GetInternalTransferReceive(int internalTransferReceiveHeaderId);
		Task<int> GetInternalTransferReceiveHeaderIdFromInternalTransferHeaderId(int internalTransferHeaderId);
		Task<ResponseDto> SaveInternalTransferReceive(InternalTransferReceiveDto internalTransferReceive, bool hasApprove, bool approved, int? requestId);
		Task<ResponseDto> SaveInternalTransferReceiveWithoutUpdatingBalances(InternalTransferReceiveDto internalTransferReceive, bool hasApprove, bool approved, int? requestId, string? documentReference);
        Task<ResponseDto> DeleteInternalTransferReceiveWithoutUpdatingBalances(int internalTransferReceiveHeaderId);
		Task<InternalTransferReceiveDto> GetTransferReceiveFromInternalTransfer(int internalTransferId);

	}
}
