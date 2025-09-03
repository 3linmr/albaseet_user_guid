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
    public interface IInternalTransferReceiveHeaderService : IBaseService<InternalTransferReceiveHeader>
    {
        IQueryable<InternalTransferReceiveHeaderDto> GetInternalTransferReceiveHeaders();
        IQueryable<InternalTransferReceiveHeaderDto> GetUserInternalTransferReceiveHeaders();
        IQueryable<InternalTransferReceiveHeaderDto> GetInternalTransferReceiveHeadersByStoreId(int storeId);
        Task<InternalTransferReceiveHeaderDto?> GetInternalTransferReceiveHeaderById(int id);
        Task<DocumentCodeDto> GetInternalTransferReceiveCode(int storeId, DateTime documentDate);
        Task<ResponseDto> SaveInternalTransferReceiveHeader(InternalTransferReceiveHeaderDto internalTransferReceive, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags);
        Task<ResponseDto> DeleteInternalTransferReceiveHeader(int id);
    }
}
