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
    public interface IInventoryInHeaderService : IBaseService<InventoryInHeader>
    {
        IQueryable<InventoryInHeaderDto> GetInventoryInHeaders();
        IQueryable<InventoryInHeaderDto> GetUserInventoryInHeaders();
        IQueryable<InventoryInHeaderDto> GetInventoryInHeadersByStoreId(int storeId);
        Task<InventoryInHeaderDto?> GetInventoryInHeaderById(int id);
        Task<DocumentCodeDto> GetInventoryInCode(int storeId, DateTime documentDate);
        Task<ResponseDto> SaveInventoryInHeader(InventoryInHeaderDto inventoryIn, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteInventoryInHeader(int id);
    }
}
