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
    public interface IInventoryOutService
    {
        List<RequestChangesDto> GetInventoryOutRequestChanges(InventoryOutDto oldItem, InventoryOutDto newItem);
        Task<InventoryOutDto> GetInventoryOut(int inventoryOutHeaderId);
        Task ModifyInventoryOutDetailsWithStoreIdAndAvaialbleBalance(int inventoryOutHeaderId, int storeId, List<InventoryOutDetailDto> details, bool isRequestData);
		Task<ResponseDto> SaveInventoryOut(InventoryOutDto inventoryOut, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> CheckInventoryZeroStock(int storeId, List<InventoryOutDetailDto> newDetails, List<InventoryOutDetailDto> oldDetails);
		Task<ResponseDto> DeleteInventoryOut(int inventoryOutHeaderId);
    }
}
