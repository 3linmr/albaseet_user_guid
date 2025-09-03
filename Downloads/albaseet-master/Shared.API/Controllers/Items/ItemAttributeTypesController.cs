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
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Items
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ItemAttributeTypesController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IItemAttributeTypeService _itemAttributeTypeService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ItemAttributeTypesController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IItemAttributeTypeService itemAttributeTypeService, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_itemAttributeTypeService = itemAttributeTypeService;
			_httpContextAccessor = httpContextAccessor;
		}
		[HttpGet]
		[Route("ReadItemAttributeTypes")]
		public IActionResult ReadItemAttributeTypes(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_itemAttributeTypeService.GetCompanyItemAttributeTypes(), loadOptions);
			return Ok(data);
		}
		[HttpGet]
		[Route("GetItemAttributeTypes")]
		public async Task<IActionResult> GetItemAttributeTypes()
		{
			return Ok(await _itemAttributeTypeService.GetItemAttributeTypes().ToListAsync());
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetItemAttributeTypeById(int id)
		{
			var data = await _itemAttributeTypeService.GetItemAttributeTypeById(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(data?.CompanyId ?? 0, 0);
			return Ok(userCanLook ? data : new ItemAttributeTypeDto());
		}

		[Route("GetItemAttributeTypesDropDown")]
		[HttpGet]
		public async Task<IActionResult> GetItemAttributeTypesDropDown()
		{
			var data = await _itemAttributeTypeService.GetItemAttributeTypesDropDown().ToListAsync();
			return Ok(data);
		}

		[HttpPost]
		public async Task<IActionResult> SaveItemAttribute([FromBody] ItemAttributeTypeDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _itemAttributeTypeService.SaveItemAttributeType(model);
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

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteItemAttribute(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _itemAttributeTypeService.DeleteItemAttributeType(id);
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
