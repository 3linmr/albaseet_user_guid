using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne;
using Shared.Helper.Database;
using DevExtreme.AspNet.Data;
using Microsoft.EntityFrameworkCore;
using Shared.API.Models;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;

namespace Shared.API.Controllers.Inventory
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ItemDisassembleController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IItemDisassembleService _itemDisassembleService;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IItemDisassembleLogicService _disassembleLogicService;
		private readonly IItemDisassembleHandlerService _itemDisassembleHandlerService;
		private readonly IItemDisassembleHeaderService _itemDisassembleHeaderService;

		public ItemDisassembleController(IDatabaseTransaction databaseTransaction, IItemDisassembleService itemDisassembleService, IStringLocalizer<HandelException> exLocalizer, IItemDisassembleLogicService disassembleLogicService,IItemDisassembleHandlerService itemDisassembleHandlerService,IItemDisassembleHeaderService itemDisassembleHeaderService)
		{
			_databaseTransaction = databaseTransaction;
			_itemDisassembleService = itemDisassembleService;
			_exLocalizer = exLocalizer;
			_disassembleLogicService = disassembleLogicService;
			_itemDisassembleHandlerService = itemDisassembleHandlerService;
			_itemDisassembleHeaderService = itemDisassembleHeaderService;
		}

		[HttpGet]
		[Route("ReadItemDisassembleHeaders")]
		public async Task<IActionResult> ReadItemDisassembleHeaders(DataSourceLoadOptions loadOptions, int storeId)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_itemDisassembleHandlerService.GetItemDisassembleHeaderWithOneDetail(storeId), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = _itemDisassembleService.GetItemDisassembles(storeId).ToList();
				var data = DataSourceLoader.Load(model.ToList(), loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("ReadItemDisassembles")]
		public async Task<IActionResult> ReadItemDisassembles(DataSourceLoadOptions loadOptions, int storeId)
		{
			try
			{
				var data = await DataSourceLoader.LoadAsync(_itemDisassembleService.GetItemDisassembles(storeId), loadOptions);
				return Ok(data);
			}
			catch
			{
				var model = _itemDisassembleService.GetItemDisassembles(storeId).ToList();
				var data = DataSourceLoader.Load(model.ToList(), loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("GetItemDisassembles")]
		public async Task<ActionResult<IEnumerable<ItemDisassembleDto>>> GetItemDisassembles(int storeId)
		{
			var data = await _itemDisassembleService.GetItemDisassembles(storeId).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetItemDisassembleHeaders")]
		public async Task<ActionResult<IEnumerable<ItemDisassembleHeaderDto>>> GetItemDisassembleHeaders(int storeId)
		{
			var data = await _itemDisassembleHandlerService.GetItemDisassembleHeaderWithOneDetail(storeId).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetItemDisassembleInfo")]
		public async Task<ActionResult<ItemDisassembleInfoDto>> GetItemDisassembleInfo(int itemDisassembleHeaderId)
		{
			var data = await _itemDisassembleHandlerService.GetItemDisassembleInfo(itemDisassembleHeaderId);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetItemPackageConversion")]
		public async Task<ActionResult<ItemPackageConversionDto>> GetItemPackageConversion(int itemId, int storeId, int fromPackageId, DateTime? fromPackageExpireDate, string? fromPackageBatchNumber, int toPackageId)
		{
			var data = await _disassembleLogicService.GetItemPackageConversion(itemId, storeId, fromPackageId, fromPackageExpireDate, fromPackageBatchNumber, toPackageId);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetItemPackageBalanceAfterDisassembleQuantity")]
		public async Task<ActionResult<List<ItemPackageTreeDto>>> GetItemPackageBalanceAfterDisassembleQuantity(int itemId, int storeId, int fromPackageId, int toPackageId, string? batchNumber, DateTime? expireDate, decimal quantity)
		{
			var data = await _disassembleLogicService.GetItemPackageBalanceAfterDisassembleQuantity(itemId, storeId, fromPackageId, toPackageId, batchNumber, expireDate, quantity);
			return Ok(data);
		}

		[HttpPost]
		[Route("SetItemPackageConversion")]
		public async Task<IActionResult> SetItemPackageConversion(ItemConversionDto model)
		{
			try
			{
				await _databaseTransaction.BeginTransaction();
				var response = await _itemDisassembleHandlerService.SaveItemDisassembleDocument(model);
				if (response.Success)
				{
					await _databaseTransaction.Commit();
				}
				else
				{
					await _databaseTransaction.Rollback();
				}
				return Ok(response);
			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				var handleException = new HandelException(_exLocalizer);
				return Ok(handleException.Handle(e));
			}
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteItemDisassemble(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _itemDisassembleHandlerService.DeleteItemDisassembleDocument(id);
				if (response.Success)
				{
					await _databaseTransaction.Commit();
				}
				else
				{
					await _databaseTransaction.Rollback();
				}
				return Ok(response);
			}
			catch (Exception e)
			{
				await _databaseTransaction.Rollback();
				var handleException = new HandelException(_exLocalizer);
				return Ok(handleException.Handle(e));
			}
		}
	}
}
