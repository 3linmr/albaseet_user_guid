using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Purchases.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Menus
{
	public class GenericMessageService : IGenericMessageService
	{
		private readonly IStringLocalizer<GenericMessageService> _localization;
		private readonly IMenuService _menuService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public GenericMessageService(IStringLocalizer<GenericMessageService> localization, IMenuService menuService, IHttpContextAccessor contextAccessor)
		{
			_localization = localization;
			_menuService = menuService;
			_httpContextAccessor = contextAccessor;
		}

		public async Task<string> GetMessage(int menuCode, GenericMessageData message, params string[] messageParams)
		{
			return await GetMessageVariadic([menuCode], message, messageParams);
		}

		public async Task<string> GetMessage(int menuCode1, int menuCode2, GenericMessageData message, params string[] messageParams)
		{
			return await GetMessageVariadic([menuCode1, menuCode2], message, messageParams);
		}

		public async Task<string> GetMessage(int menuCode1, int menuCode2, int menuCode3, GenericMessageData message, params string[] messageParams)
		{
			return await GetMessageVariadic([menuCode1, menuCode2, menuCode3], message, messageParams);
		}

		private async Task<string> GetMessageVariadic(List<int> menuCodeList, GenericMessageData message, string[] messageParams)
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
