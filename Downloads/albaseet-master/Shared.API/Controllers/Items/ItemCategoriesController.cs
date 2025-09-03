using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;

namespace Shared.API.Controllers.Items
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ItemCategoriesController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IItemCategoryService _itemCategoryService;

		public ItemCategoriesController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer,IItemCategoryService itemCategoryService)
		{
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_itemCategoryService = itemCategoryService;
		}
		
		
		[HttpGet]
		[Route("ReadItemCategories")]
		public IActionResult ReadItemCategories(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_itemCategoryService.GetCompanyItemCategories(), loadOptions);
			return Ok(data);
		}

		[Route("GetItemCategoryById")]
		[HttpGet]
		public async Task<IActionResult> GetItemCategoryById(int id)
		{
			var data = await _itemCategoryService.GetItemCategoryById(id);
			return Ok(data);
		}
		
		[Route("GetItemCategoriesDropDown")]
		[HttpGet]
		public async Task<IActionResult> GetItemCategoriesDropDown()
		{
			var data = await _itemCategoryService.GetItemCategoriesDropDown().ToListAsync();
			return Ok(data);
		}

		[HttpPost]
		[Route("SaveItemCategory")]
		public async Task<IActionResult> SaveItemCategory([FromBody] ItemCategoryDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _itemCategoryService.SaveItemCategory(model);
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
		[Route("DeleteItemCategory")]
		public async Task<IActionResult> DeleteItemCategory(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _itemCategoryService.DeleteItemCategory(id);
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
