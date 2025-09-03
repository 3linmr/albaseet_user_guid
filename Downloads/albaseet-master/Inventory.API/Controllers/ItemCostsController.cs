using DevExtreme.AspNet.Data;
using Inventory.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Inventory;
using Shared.Service.Services.Inventory;

namespace Inventory.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ItemCostsController : ControllerBase
	{
		private readonly IItemCostService _itemCostService;

		public ItemCostsController(IItemCostService itemCostService)
		{
			_itemCostService = itemCostService;
		}

		[HttpGet]
		[Route("ReadItemCostsByStoreId")]
		public async Task<IActionResult> ReadItemCostsByStoreId(DataSourceLoadOptions loadOptions)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_itemCostService.GetItemCostsByStoreId(), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = _itemCostService.GetItemCostsByStoreId();
				var data = DataSourceLoader.Load(model.ToList(), loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("GetItemCostsByStoreId")]
		public async Task<IActionResult> GetItemCostsByStoreId()
		{
			var data = await _itemCostService.GetItemCostsByStoreId().ToListAsync();
			return Ok(data);
		}
	}
}
