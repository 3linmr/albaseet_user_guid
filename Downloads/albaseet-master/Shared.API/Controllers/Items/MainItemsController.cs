using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.CoreOne.Models.Dtos;

namespace Shared.API.Controllers.Items
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class MainItemsController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IMainItemService _mainItemService;

		public MainItemsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IMainItemService mainItemService)
		{
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_mainItemService = mainItemService;
		}

		[HttpGet]
		[Route("ReadMainItems")]
		public IActionResult ReadMainItems(DataSourceLoadOptions loadOptions, int subSectionId)
		{
			var data = DataSourceLoader.Load(_mainItemService.GetMainItemsBySubSectionId(subSectionId), loadOptions);
			return Ok(data);
		}

		[Route("GetMainItemById")]
		[HttpGet]
		public async Task<IActionResult> GetMainItemById(int id)
		{
			var data = await _mainItemService.GetMainItemById(id);
			return Ok(data);
		}

		[Route("GetMainItemsDropDown")]
		[HttpGet]
		public async Task<IActionResult> GetMainItemsDropDown(int subSectionId)
		{
			var data = await _mainItemService.GetMainItemsDropDown(subSectionId).ToListAsync();
			return Ok(data);
		}

		[HttpPost]
		[Route("SaveMainItem")]
		public async Task<IActionResult> SaveMainItem([FromBody] MainItemDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _mainItemService.SaveMainItem(model);
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

		[HttpDelete]
		[Route("DeleteMainItem")]
		public async Task<IActionResult> DeleteMainItem(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _mainItemService.DeleteMainItem(id);
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
