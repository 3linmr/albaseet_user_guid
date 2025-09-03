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
	public class ItemSubSectionsController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IItemSubSectionService _itemSubSectionService;

		public ItemSubSectionsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IItemSubSectionService itemSubSectionService)
		{
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_itemSubSectionService = itemSubSectionService;
		}

		[HttpGet]
		[Route("ReadItemSubSections")]
		public IActionResult ReadItemSubSections(DataSourceLoadOptions loadOptions, int sectionId)
		{
			var data = DataSourceLoader.Load(_itemSubSectionService.GetItemSubSectionsBySectionId(sectionId), loadOptions);
			return Ok(data);
		}

		[Route("GetItemSubSectionById")]
		[HttpGet]
		public async Task<IActionResult> GetItemSubSectionById(int id)
		{
			var data = await _itemSubSectionService.GetItemSubSectionById(id);
			return Ok(data);
		}

		[Route("GetItemSubSectionsDropDown")]
		[HttpGet]
		public async Task<IActionResult> GetItemSubSectionsDropDown(int sectionId)
		{
			var data = await _itemSubSectionService.GetItemSubSectionsDropDown(sectionId).ToListAsync();
			return Ok(data);
		}

		[HttpPost]
		[Route("SaveItemSubSection")]
		public async Task<IActionResult> SaveItemSubSection([FromBody] ItemSubSectionDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _itemSubSectionService.SaveItemSubSection(model);
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
		[Route("DeleteItemSubSection")]
		public async Task<IActionResult> DeleteItemSubSection(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _itemSubSectionService.DeleteItemSubSection(id);
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
