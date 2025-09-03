using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Modules
{
	public interface IMenuNoteService : IBaseService<MenuNote>
	{
		IQueryable<MenuNoteDto> GetMenuNotes(int menuCode, int referenceId);
		Task<ResponseDto> SaveMenuNotes(List<MenuNoteDto> notes,int referenceId);
		Task<ResponseDto> DeleteMenuNotes(int menuCode,int referenceId);
	}
}
