using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;

namespace Shared.API.Controllers.Inventory
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ItemCurrentBalancesController : ControllerBase
	{
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IItemDisassembleService _itemDisassembleService;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IItemDisassembleLogicService _disassembleLogicService;

		public ItemCurrentBalancesController(IItemCurrentBalanceService itemCurrentBalanceService,IDatabaseTransaction databaseTransaction, IItemDisassembleService itemDisassembleService,IStringLocalizer<HandelException> exLocalizer,IItemDisassembleLogicService disassembleLogicService)
		{
			_itemCurrentBalanceService = itemCurrentBalanceService;
			_databaseTransaction = databaseTransaction;
			_itemDisassembleService = itemDisassembleService;
			_exLocalizer = exLocalizer;
			_disassembleLogicService = disassembleLogicService;
		}

		[HttpGet]
		[Route("ReadItemCurrentBalances")]
		public IActionResult ReadItemCurrentBalances(DataSourceLoadOptions loadOptions, int storeId)
		{
			try
			{
				var data = DataSourceLoader.Load(_itemCurrentBalanceService.GetStoreItemCurrentBalances(storeId), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = _itemCurrentBalanceService.GetStoreItemCurrentBalances(storeId).ToList();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
		}

        [HttpGet]
        [Route("ReadItemCurrentBalancesWithBarCodes")]
        public IActionResult ReadItemCurrentBalancesWithBarCodes(DataSourceLoadOptions loadOptions, int storeId)
        {
            try
            {
                var data = DataSourceLoader.Load(_itemCurrentBalanceService.GetStoreItemCurrentBalancesWithBarCodes(storeId), loadOptions);
                return Ok(data);
            }
            catch
            {
                var model = _itemCurrentBalanceService.GetStoreItemCurrentBalancesWithBarCodes(storeId).ToList();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
        }

        [HttpGet]
		[Route("ReadItemCurrentBalanceInfo")]
		public async Task<IActionResult> ReadItemCurrentBalanceInfo(DataSourceLoadOptions loadOptions, int storeId, bool toSingularPackage, bool grouped)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_itemCurrentBalanceService.GetItemCurrentBalanceInfo(storeId, toSingularPackage, grouped), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = _itemCurrentBalanceService.GetItemCurrentBalanceInfo(storeId, toSingularPackage, grouped).ToList();
                var data = DataSourceLoader.Load(model.ToList(), loadOptions);
                return Ok(data);
            }
		}

		[HttpGet]
		[Route("GetItemCurrentBalanceByItemId")]
		public async Task<ActionResult<IEnumerable<ItemCurrentBalanceDto>>> GetItemCurrentBalanceByItemId(int storeId,int itemId)
		{
			var data = await _itemCurrentBalanceService.GetItemCurrentBalanceByItemId(storeId,itemId);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetItemCurrentBalanceByItemCode")]
		public async Task<ActionResult<IEnumerable<ItemCurrentBalanceDto>>> GetItemCurrentBalanceByItemCode(int storeId, string? itemCode)
		{
			if (itemCode != null)
			{
				var data = await _itemCurrentBalanceService.GetItemCurrentBalanceByItemCode(storeId, itemCode);
				return Ok(data);
			}
			else
			{
				return Ok(new List<ItemCurrentBalanceDto>());
			}
		}

		[HttpGet]
		[Route("GetItemCurrentBalanceByBarCode")]
		public async Task<ActionResult<IEnumerable<ItemCurrentBalanceDto>>> GetItemCurrentBalanceByBarCode(int storeId, string? barCode)
		{
			if (barCode != null)
			{
				var data = await _itemCurrentBalanceService.GetItemCurrentBalanceByBarCode(storeId, barCode);
				return Ok(data);
			}
			else
			{
				return Ok(new List<ItemCurrentBalanceDto>());
			}
		}
	}
}
