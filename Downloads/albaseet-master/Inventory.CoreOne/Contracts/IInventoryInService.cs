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
    public interface IInventoryInService
    {
        List<RequestChangesDto> GetInventoryInRequestChanges(InventoryInDto oldItem, InventoryInDto newItem);
        Task<InventoryInDto> GetInventoryIn(int inventoryInHeaderId);
        Task<ResponseDto> SaveInventoryIn(InventoryInDto inventoryIn, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteInventoryIn(int inventoryInHeaderId);
    }
}
