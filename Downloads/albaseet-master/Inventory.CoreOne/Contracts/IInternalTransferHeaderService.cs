using Inventory.CoreOne.Models.Domain;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.CoreOne.Contracts
{
    public interface IInternalTransferHeaderService : IBaseService<InternalTransferHeader>
    {
        IQueryable<InternalTransferHeaderDto> GetInternalTransferHeaders();
        IQueryable<InternalTransferHeaderDto> GetUserInternalTransferHeaders();
        IQueryable<InternalTransferHeaderDto> GetPendingInternalTransfers();
        IQueryable<InternalTransferHeaderDto> GetClosedInternalTransfers();
		IQueryable<InternalTransferHeaderDto> GetInternalTransferHeadersByStoreId(int storeId);
        Task<InternalTransferHeaderDto?> GetInternalTransferHeaderById(int id);
        Task<DocumentCodeDto> GetInternalTransferCode(int storeId, DateTime documentDate);
        Task<ResponseDto> SaveInternalTransferHeader(InternalTransferHeaderDto internalTransfer, bool hasApprove, bool approved, int? requestId, string? documentRefernce, bool shouldValidate, bool shouldInitializeFlags);
        Task<bool> UpdateReturned(int internalTransferId, bool isReturned, string? returnedReason);
        Task<bool> UpdateClosed(int internalTransferId);
        Task<ResponseDto> DeleteInternalTransferHeader(int id);
    }
}
