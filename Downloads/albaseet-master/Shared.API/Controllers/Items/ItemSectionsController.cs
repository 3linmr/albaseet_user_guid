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

namespace Shared.API.Controllers.Items
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ItemSectionsController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IItemSectionService _itemSectionService;

		public ItemSectionsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IItemSectionService itemSectionService)
		{
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_itemSectionService = itemSectionService;
		}

		[HttpGet]
		[Route("ReadItemSections")]
		public IActionResult ReadItemSections(DataSourceLoadOptions loadOptions, int subCategoryId)
		{
			var data = DataSourceLoader.Load(_itemSectionService.GetItemSectionsBySubCategoryId(subCategoryId), loadOptions);
			return Ok(data);
		}

		[Route("GetItemSectionById")]
		[HttpGet]
		public async Task<IActionResult> GetItemSectionById(int id)
		{
			var data = await _itemSectionService.GetItemSectionById(id);
			return Ok(data);
		}

		[Route("GetItemSectionsDropDown")]
		[HttpGet]
		public async Task<IActionResult> GetItemSectionsDropDown(int subCategoryId)
		{
			var data = await _itemSectionService.GetItemSectionsDropDown(subCategoryId).ToListAsync();
			return Ok(data);
		}

		[HttpPost]
		[Route("SaveItemSection")]
		public async Task<IActionResult> SaveItemSection([FromBody] ItemSectionDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _itemSectionService.SaveItemSection(model);
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
		[Route("DeleteItemSection")]
		public async Task<IActionResult> DeleteItemSection(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _itemSectionService.DeleteItemSection(id);
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
