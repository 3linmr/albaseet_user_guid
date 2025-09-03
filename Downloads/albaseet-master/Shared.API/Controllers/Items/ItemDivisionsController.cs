using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.CoreOne.Contracts.Items;

namespace Shared.API.Controllers.Items
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ItemDivisionsController : ControllerBase
	{
		private readonly IItemDivisionService _itemDivisionService;

		public ItemDivisionsController(IItemDivisionService itemDivisionService)
		{
			_itemDivisionService = itemDivisionService;
		}

		[HttpGet]
		[Route("GetItemDivisions")]
		public async Task<IActionResult> GetItemDivisions()
		{
			var data = await _itemDivisionService.GetItemDivisions();
			return Ok(data);
		}
	}
}
