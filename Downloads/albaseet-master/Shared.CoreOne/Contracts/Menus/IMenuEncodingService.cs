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
	public interface IMenuEncodingService : IBaseService<MenuEncoding>
	{
		IQueryable<MenuEncodingDto> GetAllMenuEncoding();
		IQueryable<MenuEncodingDto> GetAllMenuEncodingByStoreId(int storeId);
		Task<MenuEncodingDto?> GetMenuEncodingById(int id);
		Task<MenuEncodingVm> GetMenuEncoding(int storeId, int menuCode);
		Task<ResponseDto> SaveMenuEncoding(List<MenuEncodingDto> menuEncodings);
	}
}
