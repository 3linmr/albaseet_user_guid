using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.API.Models;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Menus;
using Shared.Service.Services.Menus;

namespace Shared.API.Controllers.Menus
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class MenusController : ControllerBase
	{
		private readonly IMenuService _menuService;
		private readonly IApproveService _approveService;

		public MenusController(IMenuService menuService,IApproveService approveService)
		{
			_menuService = menuService;
			_approveService = approveService;
		}

		[HttpGet]
		[Route("GetMenusHasApprovesDropDown")]
		public async Task<IActionResult> GetMenusHasApprovesDropDown()
		{
			var data = await _menuService.GetMenusHasApprovesDropDown();
			return Ok(data);
		}
		
		[HttpGet]
		[Route("GetMenusForApprovesDropDown")]
		public async Task<IActionResult> GetMenusForApprovesDropDown()
		{
			var data = await _approveService.GetMenusForApprovesDropDown();
			return Ok(data);
		}
		
		[HttpGet]
		[Route("GetMenusHasNotesDropDown")]
		public async Task<IActionResult> GetMenusHasNotesDropDown()
		{
			var data = await _menuService.GetMenusHasNotesDropDown();
			return Ok(data);
		}

        [HttpGet]
        [Route("GetMenusShippingStatusDropDown")]
        public async Task<IActionResult> GetMenusShippingStatusDropDown()
        {
            var data = await _menuService.GetMenusShippingStatusDropDown();
            return Ok(data);
        } 
        
        [HttpGet]
        [Route("GetDocuments")]
        public async Task<IActionResult> GetDocuments()
        {
            var data = await _menuService.GetDocuments();
            return Ok(data);
        }

        [HttpGet("{id:int}")]
		public async Task<ActionResult<MenuCodeDto>> GetMenuByMenuCode(int id)
		{
			var data = await _menuService.GetMenuByMenuCode(id);
			return Ok(data);
		}
	}
}
