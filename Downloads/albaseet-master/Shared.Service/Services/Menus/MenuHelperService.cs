using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Menus;

namespace Shared.Service.Services.Menus
{
	public class MenuHelperService : IMenuHelperService
	{
		private readonly IMenuService _menuService;
		private readonly IMenuEncodingService _menuEncodingService;
		private readonly IStoreService _storeService;

		public MenuHelperService(IMenuService menuService,IMenuEncodingService menuEncodingService,IStoreService storeService)
		{
			_menuService = menuService;
			_menuEncodingService = menuEncodingService;
			_storeService = storeService;
		}

		public async Task<List<MenuEncodingDto>> GetMenuEncodingsByStoreId(int storeId)
		{
			var allMenus = await _menuService.GetAllMenus();
			var userStores = await _storeService.GetUserStores();
			var menuEncodings = await _menuEncodingService.GetAllMenuEncodingByStoreId(storeId).ToListAsync();

			var data =
				(from menu in allMenus.Where(x=>x.HasEncoding)
				from store in userStores.Where(x=> x.StoreId == storeId)
				from menuEncoding in menuEncodings.Where(x=>x.MenuCode == menu.MenuCode).DefaultIfEmpty()
				select new MenuEncodingDto()
				{
					MenuEncodingId = menuEncoding != null ? menuEncoding.MenuEncodingId : 0,
					MenuCode = menu.MenuCode,
					MenuNameAr = menu.MenuNameAr,
					MenuNameEn = menu.MenuNameEn,
					StoreNameAr = store.StoreNameAr,
					StoreNameEn = store.StoreNameEn,
					StoreId = store.StoreId,
					Prefix = menuEncoding != null ? menuEncoding.Prefix :"",
					Suffix = menuEncoding != null ? menuEncoding.Suffix : ""
				}).ToList();
			return data;
		}
	}
}
