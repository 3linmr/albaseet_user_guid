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
    public interface IInternalTransferService
    {
        List<RequestChangesDto> GetInternalTransferRequestChanges(InternalTransferDto oldItem, InternalTransferDto newItem);
        Task<InternalTransferDto> GetInternalTransfer(int internalTransferHeaderId);
        Task ModifyInternalTransferDetailsWithStoreIdAndAvaialbleBalance(int internalTransferHeaderId, int storeId, List<InternalTransferDetailDto> details, bool isRequestData);

		Task<int> GetInternalTransferHeaderIdFromReferenceAndMenuCode(int referenceId, int menuCode);
		Task<bool> UpdateReturned(int internalTransferId, bool isReturned, string? returnedReason);
        Task<bool> UpdateClosed(int internalTransferId);
        Task<ResponseDto> SaveInternalTransfer(InternalTransferDto internalTransfer, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> CheckInventoryZeroStock(int storeId, List<InternalTransferDetailDto> newDetails, List<InternalTransferDetailDto> oldDetails);
		Task<ResponseDto> SaveInternalTransferWithoutUpdatingBalances(InternalTransferDto internalTransfer, bool hasApprove, bool approved, int? requestId, string? documentReference);
        Task<ResponseDto> DeleteInternalTransfer(int internalTransferHeaderId);
        Task<ResponseDto> DeleteInternalTransferWithoutUpdatingBalances(int internalTransferHeaderId);
        Task<bool> GetInternalTransferPendingItems(int storeId,bool isAllItems, IEnumerable<int> itemIds);
    }
}
