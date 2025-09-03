using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Items
{
	public interface IItemPackingService: IBaseService<ItemPacking>
	{
		IQueryable<ItemPackingVm> GetItemPacking(int itemId);
		Task<ResponseDto> UpdateItemPackings(int itemId, List<ItemBarCodeDto> itemBarCodes);
		Task<decimal> GetItemPacking(int itemId, int fromPackageId, int toPackageId);
		Task<List<ItemPackageVm>> GetItemSiblingPackages(int itemId, int packageId);
		Task<List<ItemPackageDropDownDto>> GetItemSiblingPackagesDropDown(int itemId, int packageId);
		Task<bool> DeleteItemPackings(int itemId);
	}
}
