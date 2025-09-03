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
    public interface IItemSubSectionService : IBaseService<ItemSubSection>
	{
		IQueryable<ItemSubSectionDto> GetItemSubSections();
		IQueryable<ItemSubSectionDto> GetCompanyItemSubSections();
		IQueryable<ItemSubSectionDto> GetItemSubSectionsBySectionId(int sectionId);
		IQueryable<ItemSubSectionDropDownDto> GetItemSubSectionsDropDown(int sectionId);
		Task<ItemSubSectionDto?> GetItemSubSectionById(int id);
		Task<ResponseDto> SaveItemSubSection(ItemSubSectionDto itemSubSection);
		Task<ResponseDto> DeleteItemSubSection(int id);
	}
}
