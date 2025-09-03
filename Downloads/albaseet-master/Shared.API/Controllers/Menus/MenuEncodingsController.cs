using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Menus;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Menus
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class MenuEncodingsController : ControllerBase
	{
		private readonly IMenuEncodingService _menuEncodingService;
		private readonly IMenuHelperService _menuHelperService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public MenuEncodingsController(IMenuEncodingService menuEncodingService,IMenuHelperService menuHelperService,IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IHttpContextAccessor httpContextAccessor)
		{
			_menuEncodingService = menuEncodingService;
			_menuHelperService = menuHelperService;
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("GetMenuEncodingsByStoreId")]
		public async Task<IActionResult> GetMenuEncodingsByStoreId(DataSourceLoadOptions loadOptions,int storeId)
		{
			var data = DataSourceLoader.Load(await _menuHelperService.GetMenuEncodingsByStoreId(storeId), loadOptions);
			return Ok(data);
		}
		
		[HttpGet]
		[Route("GetMenuEncoding")]
		public async Task<IActionResult> GetMenuEncoding(int storeId,int menuCode)
		{
			var data = await _menuEncodingService.GetMenuEncoding(storeId,menuCode);
			var userCanLook = await _httpContextAccessor.UserCanLook(0, storeId);
			return Ok(userCanLook ? data : new MenuEncodingVm());
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] List<MenuEncodingDto> model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _menuEncodingService.SaveMenuEncoding(model);
				if (response.Success)
				{
					await _databaseTransaction.Commit();
				}
				else
				{
					response.Success = false;
					response.Message = response.Message;
				}
			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				var handleException = new HandelException(_exLocalizer);
				return Ok(handleException.Handle(e));
			}
			return Ok(response);
		}
	}
}
