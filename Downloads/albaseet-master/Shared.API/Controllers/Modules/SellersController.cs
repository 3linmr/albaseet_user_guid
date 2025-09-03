using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Service.Services.Modules;
using Shared.Helper.Identity;
using System.Text.Json;

namespace Shared.API.Controllers.Modules
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SellersController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exceptionLocalizer;
		private readonly ISellerService _sellerService;
		private readonly IMenuNoteService _menuNoteService;
		private readonly ISellerTypeService _sellerTypeService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public SellersController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exceptionLocalizer, ISellerService sellerService, IMenuNoteService menuNoteService,ISellerTypeService sellerTypeService, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_exceptionLocalizer = exceptionLocalizer;
			_sellerService = sellerService;
			_menuNoteService = menuNoteService;
			_sellerTypeService = sellerTypeService;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadSellers")]
		public IActionResult ReadSellers(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_sellerService.GetUserSellers(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllSellers")]
		public async Task<ActionResult<IEnumerable<SellerDto>>> GetAllSellers()
		{
			var allSellers = await _sellerService.GetAllSellers().ToListAsync();
			return Ok(allSellers);
		}
		
		[HttpGet]
		[Route("GetSellersDropDownByCompanyId")]
		public async Task<ActionResult<IEnumerable<SellerDropDownDto>>> GetSellersDropDownByCompanyId(int companyId)
		{
			var sellers = await _sellerService.GetSellersDropDownByCompanyId(companyId).ToListAsync();
			return Ok(sellers);
		}
        
        [HttpGet]
		[Route("GetSellersDropDownByStoreId")]
		public async Task<ActionResult<IEnumerable<SellerDropDownDto>>> GetSellersDropDownByStoreId(int storeId)
		{
			var sellers = await _sellerService.GetSellersDropDownByStoreId(storeId).ToListAsync();
			return Ok(sellers);
		}
        
        [HttpGet]
		[Route("GetSellersDropDown")]
		public async Task<ActionResult<IEnumerable<SellerDropDownDto>>> GetSellersDropDown(int storeId)
		{
			var sellers = await _sellerService.GetSellersDropDownByStoreId(storeId).ToListAsync();
			return Ok(sellers);
		}

		[HttpGet]
		[Route("GetSellersAutoComplete")]
		public async Task<ActionResult<IEnumerable<SellerAutoCompleteDto>>> GetSellersAutoComplete(string term)
		{
			var data = await _sellerService.GetSellersAutoComplete(term);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetSellersAutoCompleteByStoreIds")]
		public async Task<ActionResult<IEnumerable<SellerAutoCompleteDto>>> GetSellersAutoCompleteByStoreIds(string term, string? storeIds)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]")!;
			var data = await _sellerService.GetSellersAutoCompleteByStoreIds(term, storeIdsList);
			return Ok(data);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetSellerById(int id)
		{
			var stateDb = await _sellerService.GetSellerById(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(stateDb?.CompanyId ?? 0, 0);
			return Ok(userCanLook ? stateDb : new SellerDto());
		}

		[HttpGet]
		[Route("GetSellerTypes")]
		public async Task<ActionResult<IEnumerable<SellerTypeDto>>> GetSellerTypes()
		{
			var sellerTypes = await _sellerTypeService.GetSellerTypes().ToListAsync();
			return Ok(sellerTypes);
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] SellerDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _sellerService.SaveSeller(model);
				if (response.Success)
				{
					if (model.MenuNotes != null)
					{
						await _menuNoteService.SaveMenuNotes(model.MenuNotes, response.Id);
					}

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
				var handleException = new HandelException(_exceptionLocalizer);
				return Ok(handleException.Handle(e));
			}
			return Ok(response);
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteSeller(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _sellerService.DeleteSeller(id);
				await _menuNoteService.DeleteMenuNotes(MenuCodeData.Seller, response.Id);

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
				var handleException = new HandelException(_exceptionLocalizer);
				return Ok(handleException.Handle(e));
			}
			return Ok(response);
		}
	}
}
