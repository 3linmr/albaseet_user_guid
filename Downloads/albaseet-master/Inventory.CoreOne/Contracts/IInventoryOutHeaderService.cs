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
    public interface IInventoryOutHeaderService : IBaseService<InventoryOutHeader>
    {
        IQueryable<InventoryOutHeaderDto> GetInventoryOutHeaders();
        IQueryable<InventoryOutHeaderDto> GetUserInventoryOutHeaders();
        IQueryable<InventoryOutHeaderDto> GetInventoryOutHeadersByStoreId(int storeId);
        Task<InventoryOutHeaderDto?> GetInventoryOutHeaderById(int id);
        Task<DocumentCodeDto> GetInventoryOutCode(int storeId, DateTime documentDate);
        Task<ResponseDto> SaveInventoryOutHeader(InventoryOutHeaderDto inventoryOut, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteInventoryOutHeader(int id);
    }
}
