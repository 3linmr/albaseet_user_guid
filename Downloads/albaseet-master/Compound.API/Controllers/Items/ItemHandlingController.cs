using Compound.CoreOne.Contracts.Items;
using Compound.API.Models;
using Compound.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Purchases.CoreOne.Contracts;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;

namespace Compound.API.Controllers.Items
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemHandlingController : Controller
    {
        private readonly IItemHandlingService _itemHandlingService;
        private readonly IItemCostHandlingService _itemCostHandlingService;

        public ItemHandlingController(IItemHandlingService itemHandlingService,IItemCostHandlingService itemCostHandlingService)
        {
	        _itemHandlingService = itemHandlingService;
	        _itemCostHandlingService = itemCostHandlingService;
        }

        [HttpGet]
        [Route("ReadItemsAutoComplete")]
        public async Task<IActionResult> ReadItemsAutoComplete(DataSourceLoadOptions loadOptions, int storeId)
        {
            var data = await DataSourceLoader.LoadAsync(await _itemHandlingService.GetItemsByStoreId(storeId), loadOptions);
            return Ok(data);
        }

        [HttpGet]
        [Route("ReadItemBalances")]
        public async Task<IActionResult> ReadItemBalances(DataSourceLoadOptions loadOptions, int storeId, bool isGrouped)
        {
	        try
	        {
		        var data = await DataSourceLoader.LoadAsync( await _itemHandlingService.GetItemBalances(storeId, isGrouped), loadOptions);
		        return Ok(data);
	        }
	        catch (Exception e)
	        {
		        var itemData = await _itemHandlingService.GetItemBalances(storeId, isGrouped);
				var data = DataSourceLoader.Load(await itemData.ToListAsync(), loadOptions);
				return Ok(data);
			}
		}
        [HttpGet]
        [Route("ReadItemCard")]
        public async Task<IActionResult> ReadItemCard(DataSourceLoadOptions loadOptions, int storeId)
        {
	        try
	        {
		        var data = await DataSourceLoader.LoadAsync(await _itemHandlingService.GetItemCard(storeId), loadOptions);
		        return Ok(data);
	        }
	        catch (Exception e)
	        {
		        var dataDb = await _itemHandlingService.GetItemCard(storeId);
				var data = DataSourceLoader.Load(await dataDb.ToListAsync(), loadOptions);
				return Ok(data);
			}
        }

        [HttpGet]
        [Route("GetItemBalances")]
        public async Task<IQueryable<ItemBalanceDto>> GetItemBalances(int storeId, bool isGrouped)
        {
	        var data = await _itemHandlingService.GetItemBalances(storeId,isGrouped);
	        return data;
        }

        [HttpGet]
        [Route("GetItemCard")]
        public async Task<IQueryable<ItemBalanceDto>> GetItemCard( int storeId)
        {
	        return  await _itemHandlingService.GetItemCard(storeId);
        }

		[HttpGet]
        [Route("GetItemSearchByItemId")]
        public async Task<ActionResult<ItemAutoCompleteDto>> GetItemSearchByItemId(int itemId, int storeId, DateTime? currentDate = null)
        {
            var data = await _itemHandlingService.GetItemSearchByItemId(itemId, storeId, currentDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetItemSearchByItemCode")]
        public async Task<ActionResult<ItemAutoCompleteDto>> GetItemSearchByItemCode(string? itemCode, int storeId, DateTime? currentDate = null)
        {
            var data = await _itemHandlingService.GetItemSearchByItemCode(itemCode, storeId, currentDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetItemByBarCode")]
        public async Task<ActionResult<ItemAutoCompleteDto>> GetItemByBarCode(string? barCode, int storeId, DateTime? currentDate = null)
        {
            var data = await _itemHandlingService.GetItemByBarCode(barCode, storeId, currentDate);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetItemDataOnPackageChange")]
        public async Task<ActionResult<ItemAutoCompleteDto>> GetItemDataOnPackageChange(int itemId, int fromPackageId, int storeId, bool isGrouped, DateTime? currentDate = null, DateTime? expireDate = null, string? batchNumber = null)
        {
            var data = await _itemHandlingService.GetItemDataOnPackageChange(itemId, fromPackageId, storeId,isGrouped, currentDate,expireDate, batchNumber);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetItemPricesOnPackageChange")]
        public async Task<ActionResult<ItemAutoCompleteDto>> GetItemPricesOnPackageChange(int itemId, int fromPackageId, int storeId)
        {
            var data = await _itemHandlingService.GetItemPricesOnPackageChange(itemId, fromPackageId, storeId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetItemPrices")]
        public async Task<ActionResult<ItemAutoCompleteDto>> GetItemPrices(int storeId, int itemId, int packageId)
        {
            var data = await _itemHandlingService.GetItemPrices(storeId, itemId, packageId);
            return Ok(data);
        }

        [HttpPost]
        [Route("CalculateItemCost")]
        public async Task<ActionResult<ItemAutoCompleteDto>> CalculateItemCost(CalculateItemCost model)
        {
	        var data = await _itemCostHandlingService.CalculateItemCost(model);
	        return Ok(data);
        }

	}
}
