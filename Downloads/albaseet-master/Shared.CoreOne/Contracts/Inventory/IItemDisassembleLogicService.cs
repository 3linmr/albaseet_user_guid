using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Contracts.Inventory
{
	public interface IItemDisassembleLogicService
	{
		Task<List<ItemPackageTreeDto>> GetItemPackageBalanceAfterDisassembleQuantity(int itemId, int storeId, int fromPackageId, int toPackageId, string? batchNumber, DateTime? expireDate, decimal quantity);
		Task<ItemPackageConversionDto> GetItemPackageConversion(int itemId, int storeId, int fromPackageId, DateTime? fromPackageExpireDate, string? fromPackageBatchNumber, int toPackageId);
		Task<ResponseDto> SetItemPackageConversion(int itemDisassembleHeaderId, ItemConversionDto model);
		Task<ResponseDto> ReverseItemPackageConversion(int itemDisassembleHeaderId);
	}
}
