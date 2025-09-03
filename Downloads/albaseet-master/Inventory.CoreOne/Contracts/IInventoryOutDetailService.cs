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
    public interface IInventoryOutDetailService : IBaseService<InventoryOutDetail>
    {
        Task<List<InventoryOutDetailDto>> GetInventoryOutDetails(int inventoryOutHeaderId);
        Task<List<InventoryOutDetailDto>> SaveInventoryOutDetails(int inventoryOutHeaderId, List<InventoryOutDetailDto> inventoryOutDetails);
        Task<bool> DeleteInventoryOutDetails(int inventoryOutHeaderId);
    }
}
