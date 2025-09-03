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
    public interface IInventoryInDetailService : IBaseService<InventoryInDetail>
    {
        Task<List<InventoryInDetailDto>> GetInventoryInDetails(int inventoryInHeaderId);
        Task<List<InventoryInDetailDto>> SaveInventoryInDetails(int inventoryInHeaderId, List<InventoryInDetailDto> inventoryInDetails);
        Task<bool> DeleteInventoryInDetails(int inventoryInHeaderId);
    }
}
