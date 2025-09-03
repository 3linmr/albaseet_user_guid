using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Sales.Service.Services
{
	public class SalesMessageService : ISalesMessageService
	{
		private IStringLocalizer<SalesMessageService> _localization;
		private IMenuService _menuService;
		private IHttpContextAccessor _httpContextAccessor;

		public SalesMessageService(IStringLocalizer<SalesMessageService> localization, IMenuService menuService, IHttpContextAccessor contextAccessor)
		{
			_localization = localization;
			_menuService = menuService;
			_httpContextAccessor = contextAccessor;
		}

		public string GetMessage(SalesMessageData message, params string[] messageParams)
		{
			return _localization[message.ToString(), messageParams];
		}

		public async Task<string> GetMessage(int menuCode, SalesMessageData message, params string[] messageParams)
		{
			return await GetMessageVariadic([menuCode], message, messageParams);
		}

		public async Task<string> GetMessage(int menuCode1, int menuCode2, SalesMessageData message, params string[] messageParams)
		{
			return await GetMessageVariadic([menuCode1, menuCode2], message, messageParams);
		}

		public async Task<string> GetMessage(int menuCode1, int menuCode2, int menuCode3, SalesMessageData message, params string[] messageParams)
		{
			return await GetMessageVariadic([menuCode1, menuCode2, menuCode3], message, messageParams);
		}

		private async Task<string> GetMessageVariadic(List<int> menuCodeList, SalesMessageData message, string[] messageParams)
		{
			var langage = _httpContextAccessor.GetProgramCurrentLanguage();

			var menus = await _menuService.GetAll().Where(x => menuCodeList.Contains(x.MenuCode)).ToListAsync();

			var menuNames = from menuCode in menuCodeList
							from menu in menus.Where(x => x.MenuCode == menuCode)
							select langage == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn;

			var messageParamters = menuNames.Concat(messageParams).ToArray();
			return _localization[message.ToString(), messageParamters];
		}
	}
}
