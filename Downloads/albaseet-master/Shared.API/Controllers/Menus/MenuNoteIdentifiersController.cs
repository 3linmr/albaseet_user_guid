using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Menus;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Service.Services.Basics;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Menus
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class MenuNoteIdentifiersController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IMenuNoteIdentifierService _menuNoteIdentifierService;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public MenuNoteIdentifiersController(IDatabaseTransaction databaseTransaction, IMenuNoteIdentifierService menuNoteIdentifierService, IStringLocalizer<HandelException> exLocalizer, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_menuNoteIdentifierService = menuNoteIdentifierService;
			_exLocalizer = exLocalizer;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadMenuNoteIdentifiers")]
		public IActionResult ReadMenuNoteIdentifiers(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_menuNoteIdentifierService.GetCompanyMenuNoteIdentifiers(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllMenuNoteIdentifiers")]
		public async Task<ActionResult<IEnumerable<MenuNoteIdentifierDto>>> GetAllMenuNoteIdentifiers()
		{
			var allMenuNoteIdentifiers = await _menuNoteIdentifierService.GetAllMenuNoteIdentifiers().ToListAsync();
			return Ok(allMenuNoteIdentifiers);
		}

		[HttpGet]
		[Route("GetAllMenuNoteIdentifiersDropDown")]
		public async Task<ActionResult<IEnumerable<MenuNoteIdentifierDropDownDto>>> GetAllMenuNoteIdentifiersDropDown(int menuCode)
		{
			var allMenuNoteIdentifiers = await _menuNoteIdentifierService.GetAllMenuNoteIdentifiersDropDown(menuCode).ToListAsync();
			return Ok(allMenuNoteIdentifiers);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetMenuNoteIdentifierById(int id)
		{
			var menuNoteIdentifierDb = await _menuNoteIdentifierService.GetMenuNoteIdentifierById(id) ?? new MenuNoteIdentifierDto();
			var userCanLook = await _httpContextAccessor.UserCanLook(menuNoteIdentifierDb.CompanyId, 0);
			return Ok(userCanLook ? menuNoteIdentifierDb : new MenuNoteIdentifierDto());
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] MenuNoteIdentifierDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _menuNoteIdentifierService.SaveMenuNoteIdentifier(model);
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

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteMenuNoteIdentifier(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _menuNoteIdentifierService.DeleteMenuNoteIdentifier(id);
				if (response.Success)
				{
					await _databaseTransaction.Commit();
				}
				else
				{
					await _databaseTransaction.Rollback();
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
