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
    public interface IItemSectionService : IBaseService<ItemSection>
	{
		IQueryable<ItemSectionDto> GetItemSections();
		IQueryable<ItemSectionDto> GetCompanyItemSections();
		IQueryable<ItemSectionDto> GetItemSectionsBySubCategoryId(int subCategoryId);
		IQueryable<ItemSectionDropDownDto> GetItemSectionsDropDown(int subCategoryId);
		Task<ItemSectionDto?> GetItemSectionById(int id);
		Task<ResponseDto> SaveItemSection(ItemSectionDto itemSection);
		Task<ResponseDto> DeleteItemSection(int id);
	}
}
