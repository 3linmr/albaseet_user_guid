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
    public interface IItemCategoryService : IBaseService<ItemCategory>
	{
		IQueryable<ItemCategoryDto> GetItemCategories();
		IQueryable<ItemCategoryDto> GetCompanyItemCategories();
		IQueryable<ItemCategoryDropDownDto> GetItemCategoriesDropDown();
		Task<ItemCategoryDto?> GetItemCategoryById(int id);
		Task<ResponseDto> SaveItemCategory(ItemCategoryDto itemCategory);
		Task<ResponseDto> DeleteItemCategory(int id);
	}
}
