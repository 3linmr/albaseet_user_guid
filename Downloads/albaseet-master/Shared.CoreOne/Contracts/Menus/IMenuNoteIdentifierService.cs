using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Menus;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Menus
{
	public interface IMenuNoteIdentifierService : IBaseService<MenuNoteIdentifier>
	{
		IQueryable<MenuNoteIdentifierDto> GetAllMenuNoteIdentifiers();
		IQueryable<MenuNoteIdentifierDto> GetCompanyMenuNoteIdentifiers();
		IQueryable<MenuNoteIdentifierDropDownDto> GetAllMenuNoteIdentifiersDropDown(int menuCode);
		Task<MenuNoteIdentifierDto?> GetMenuNoteIdentifierById(int id);
		Task<ResponseDto> SaveMenuNoteIdentifier(MenuNoteIdentifierDto menuNoteIdentifier);
		Task<ResponseDto> DeleteMenuNoteIdentifier(int id);
	}
}
