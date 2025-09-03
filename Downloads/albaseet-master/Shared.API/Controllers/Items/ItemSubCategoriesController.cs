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
	public class ItemSubCategoriesController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IItemSubCategoryService _itemSubCategoryService;

		public ItemSubCategoriesController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer,IItemSubCategoryService itemSubCategoryService)
		{
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_itemSubCategoryService = itemSubCategoryService;
		}

		[HttpGet]
		[Route("ReadItemSubCategories")]
		public IActionResult ReadItemSubCategories(DataSourceLoadOptions loadOptions, int categoryId)
		{
			var data = DataSourceLoader.Load(_itemSubCategoryService.GetItemSubCategoriesByCategoryId(categoryId), loadOptions);
			return Ok(data);
		}

		[Route("GetItemSubCategoryById")]
		[HttpGet]
		public async Task<IActionResult> GetItemSubCategoryById(int id)
		{
			var data = await _itemSubCategoryService.GetItemSubCategoryById(id);
			return Ok(data);
		}

		[Route("GetItemSubCategoriesDropDown")]
		[HttpGet]
		public async Task<IActionResult> GetItemSubCategoriesDropDown(int categoryId)
		{
			var data = await _itemSubCategoryService.GetItemSubCategoriesDropDown(categoryId).ToListAsync();
			return Ok(data);
		}

		[HttpPost]
		[Route("SaveItemSubCategory")]
		public async Task<IActionResult> SaveItemSubCategory([FromBody] ItemSubCategoryDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _itemSubCategoryService.SaveItemSubCategory(model);
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
		[Route("DeleteItemSubCategory")]
		public async Task<IActionResult> DeleteItemSubCategory(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _itemSubCategoryService.DeleteItemSubCategory(id);
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
