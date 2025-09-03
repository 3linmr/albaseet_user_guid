using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.API.Models;
using Shared.CoreOne.Contracts.Modules;
using Shared.Service.Services.Modules;

namespace Shared.API.Controllers.Modules
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class MenuNotesController : ControllerBase
	{
		private readonly IMenuNoteService _menuNoteService;

		public MenuNotesController(IMenuNoteService menuNoteService)
		{
			_menuNoteService = menuNoteService;
		}

		[HttpGet]
		[Route("ReadMenuNotes")]
		public IActionResult ReadMenuNotes(DataSourceLoadOptions loadOptions, int menuCode, int referenceId)
		{
			var data = DataSourceLoader.Load(_menuNoteService.GetMenuNotes(menuCode,referenceId), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetMenuNotes")]
		public async Task<IActionResult> GetMenuNotes(int menuCode, int referenceId)
		{
			var menuNotes = await _menuNoteService.GetMenuNotes(menuCode, referenceId).ToListAsync();
			return Ok(menuNotes);
		}
	}
}
