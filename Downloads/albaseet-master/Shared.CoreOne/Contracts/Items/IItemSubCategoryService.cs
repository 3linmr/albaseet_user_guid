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
    public interface IItemSubCategoryService : IBaseService<ItemSubCategory>
	{
		IQueryable<ItemSubCategoryDto> GetItemSubCategories();
		IQueryable<ItemSubCategoryDto> GetCompanyItemSubCategories();
		IQueryable<ItemSubCategoryDto> GetItemSubCategoriesByCategoryId(int categoryId);
		IQueryable<ItemSubCategoryDropDownDto> GetItemSubCategoriesDropDown(int categoryId);
		Task<ItemSubCategoryDto?> GetItemSubCategoryById(int id);
		Task<ResponseDto> SaveItemSubCategory(ItemSubCategoryDto itemSubCategory);
		Task<ResponseDto> DeleteItemSubCategory(int id);
	}
}
