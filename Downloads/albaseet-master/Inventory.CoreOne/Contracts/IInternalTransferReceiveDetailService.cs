using Inventory.CoreOne.Models.Domain;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.CoreOne.Contracts
{
    public interface IInternalTransferReceiveDetailService: IBaseService<InternalTransferReceiveDetail>
    {
        Task<List<InternalTransferReceiveDetailDto>> GetInternalTransferReceiveDetails(int internalTransferReceiveHeaderId);
        Task<bool> SaveInternalTransferReceiveDetails(int internalTransferReceiveHeaderId, List<InternalTransferReceiveDetailDto> internalTransferReceiveDetails);
        Task<bool> DeleteInternalTransferReceiveDetails(int internalTransferReceiveHeaderId);
    }
}
