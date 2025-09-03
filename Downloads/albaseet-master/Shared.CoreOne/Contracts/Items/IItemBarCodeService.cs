using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Items
{
	public interface IItemBarCodeService : IBaseService<ItemBarCode>
	{
		IQueryable<ItemBarCodeDto> GetAllItemBarCodes();
		Task<List<ItemBarCodeDto>> GetItemBarCodesByItemId(int itemId);
		Task<string?> GetDefaultItemBarCode(int itemId);
		Task<ItemBarCodeDto?> GetItemBarCodeById(int id);
		Task<decimal> GetPacking(int itemId, int fromPackageId, int toPackageId);
		Task<decimal> GetSingularItemPacking(int itemId,int fromPackageId);
		public Task<List<PackageTreeDto>> ItemPackagesTree(int itemId);
		public List<PackageTreeDto> ItemPackagesTreeFromModel(List<ItemBarCodeDto> barCodeDtos);
		Task<List<int>> GetItemPackages(int itemId);
		Task<List<ItemPackageLevelDto>> GetItemPackagesLevel(int itemId, int fromPackage,int toPackage);
		Task<List<ItemPackageDropDownDto>> GetItemPackagesDropDown(int itemId);
		Task<List<ItemPackageDropDownDto>> GetItemPackagesWithoutSingularDropDown(int itemId);
		Task<List<ItemPackagesDto>> GetItemsPackages(List<int> itemId);
		Task<List<ItemBarCodeDetailDto>> SaveItemBarCodes(List<ItemBarCodeDto> attributes, int itemId);
		Task<ResponseDto> DeleteItemBarCodesByItemId(int itemId);
	}
}
