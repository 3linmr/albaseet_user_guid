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
    public interface IInternalTransferDetailService: IBaseService<InternalTransferDetail>
    {
        Task<List<InternalTransferDetailDto>> GetInternalTransferDetails(int internalTransferHeaderId);
        Task<bool> SaveInternalTransferDetails(int internalTransferHeaderId, List<InternalTransferDetailDto> internalTransferDetails);
        Task<bool> DeleteInternalTransferDetails(int internalTransferHeaderId);
    }
}
