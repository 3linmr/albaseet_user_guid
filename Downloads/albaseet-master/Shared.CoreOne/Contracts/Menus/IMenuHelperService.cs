using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Menus;

namespace Shared.CoreOne.Contracts.Menus
{
	public interface IMenuHelperService
	{
		Task<List<MenuEncodingDto>> GetMenuEncodingsByStoreId(int storeId);
	}
}
