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
    public interface IMainItemService : IBaseService<MainItem>
	{
		IQueryable<MainItemDto> GetMainItems();
		IQueryable<MainItemDto> GetCompanyMainItems();
		IQueryable<MainItemDto> GetMainItemsBySubSectionId(int subSectionId);
		IQueryable<MainItemDropDownDto> GetMainItemsDropDown(int subSectionId);
		Task<MainItemDto?> GetMainItemById(int id);
		Task<ResponseDto> SaveMainItem(MainItemDto mainItem);
		Task<ResponseDto> DeleteMainItem(int id);
	}
}
