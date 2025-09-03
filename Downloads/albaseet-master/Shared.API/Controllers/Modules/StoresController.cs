using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;
using Shared.Service.Services.Modules;

namespace Shared.API.Controllers.Modules
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class StoresController : ControllerBase
	{
		private readonly IStoreService _storeService;
		private readonly IStoreClassificationService _storeClassificationService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public StoresController(IStoreService storeService,IStoreClassificationService storeClassificationService,IDatabaseTransaction databaseTransaction,IStringLocalizer<HandelException>exLocalizer, IHttpContextAccessor httpContextAccessor)
		{
			_storeService = storeService;
			_storeClassificationService = storeClassificationService;
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_httpContextAccessor = httpContextAccessor;
		}


		[HttpGet]
		[Route("ReadStores")]
		public IActionResult ReadStores(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_storeService.GetAllStores().Where(x=>!x.IsReservedStore), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllStores")]
		public async Task<ActionResult<IEnumerable<StoreDto>>> GetAllStores()
		{
			var stores = await _storeService.GetAllStores().ToListAsync();
			return Ok(stores);
		}
		[HttpGet]
		[Route("GetStoresDropDown")]
		public async Task<ActionResult<IEnumerable<StoreDropDownDto>>> GetStoresDropDown()
		{
			var stores = await _storeService.GetStoresDropDown();
			return Ok(stores);
		}
		
		[HttpGet]
		[Route("GetReservedStoreByParentStoreId")]
		public async Task<ActionResult> GetReservedStoreByParentStoreId(int parentStoreId)
		{
			var storeId = await _storeService.GetReservedStoreByParentStoreId(parentStoreId);
			return Ok(storeId);
		}

		[HttpGet]
		[Route("GetAllUserStoresDropDown")]
		public async Task<ActionResult<IEnumerable<StoreDropDownDto>>> GetAllUserStoresDropDown()
		{
			var stores = await _storeService.GetAllUserStoresDropDown();
			return Ok(stores);
		}
		
		[HttpGet]
		[Route("GetUserStoresDropDown")]
		public async Task<ActionResult<IEnumerable<StoreDropDownDto>>> GetUserStoresDropDown()
		{
			var stores = await _storeService.GetUserStoresDropDown();
			return Ok(stores);
		}
		
		[HttpGet]
		[Route("GetCompanyStoresDropDown")]
		public async Task<ActionResult<IEnumerable<StoreDropDownDto>>> GetCompanyStoresDropDown()
		{
			var stores = await _storeService.GetCompanyStoresDropDown();
			return Ok(stores);
		}
		
		[HttpGet]
		[Route("GetAllStoresFullNameDropDown")]
		public async Task<ActionResult<IEnumerable<StoreDropDownVm>>> GetAllStoresFullNameDropDown(int companyId)
		{
			var stores = await _storeService.GetAllStoresFullNameDropDown(companyId);
			return Ok(stores);
		}
		

		[HttpGet]
		[Route("GetStoreClassifications")]
		public async Task<ActionResult<IEnumerable<StoreClassificationDto>>> GetStoreClassifications()
		{
			var storeClassifications = await _storeClassificationService.GetStoreClassifications();
			return Ok(storeClassifications);
		}

		[HttpGet]
		[Route("IsStoreLimitReached")]
		public async Task<ActionResult<ResponseDto>> IsStoreLimitReached()
		{
			var response = await _storeService.IsStoreLimitReached();
			return Ok(response);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetStoreById(int id)
		{
			var storeDb = await _storeService.GetStoreById(id) ?? new StoreDto();
			var userCanLook = await _httpContextAccessor.UserCanLook(storeDb.CompanyId, 0);
			return Ok(userCanLook ? storeDb : new StoreDto());
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] StoreDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _storeService.SaveStore(model,false);
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

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteStore(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _storeService.DeleteStore(id);
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
