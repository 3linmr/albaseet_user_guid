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
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Modules
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SellerCommissionMethodsController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exceptionLocalizer;
		private readonly ISellerCommissionMethodService _sellerCommissionMethodService;
		private readonly ISellerCommissionTypeService _sellerCommissionTypeService;
		private readonly ISellerService _sellerService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public SellerCommissionMethodsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exceptionLocalizer, ISellerCommissionMethodService sellerCommissionMethodService,ISellerCommissionTypeService sellerCommissionTypeService,ISellerService sellerService, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_exceptionLocalizer = exceptionLocalizer;
			_sellerCommissionMethodService = sellerCommissionMethodService;
			_sellerCommissionTypeService = sellerCommissionTypeService;
			_sellerService = sellerService;
			_httpContextAccessor = httpContextAccessor;
		}



		[HttpGet]
		[Route("ReadSellerCommissionMethods")]
		public IActionResult ReadSellerCommissionMethods(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_sellerCommissionMethodService.GetCompanySellerCommissionMethods(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllSellerCommissionMethods")]
		public async Task<ActionResult<IEnumerable<SellerCommissionMethodDto>>> GetAllSellerCommissionMethods()
		{
			var allSellerCommissionMethods = await _sellerCommissionMethodService.GetAllSellerCommissionMethods().ToListAsync();
			return Ok(allSellerCommissionMethods);
		}

		[HttpGet]
		[Route("GetAllSellerCommissionMethodsDropDown")]
		public async Task<ActionResult<IEnumerable<SellerCommissionMethodDropDownDto>>> GetAllSellerCommissionMethodsDropDown()
		{
			var allSellerCommissionMethods = await _sellerCommissionMethodService.GetAllSellerCommissionMethodsDropDown().ToListAsync();
			return Ok(allSellerCommissionMethods);
		}
		
		[HttpGet]
		[Route("GetActiveSellerCommissionMethodsDropDown")]
		public async Task<ActionResult<IEnumerable<SellerCommissionMethodDropDownDto>>> GetActiveSellerCommissionMethodsDropDown()
		{
			var allSellerCommissionMethods = await _sellerCommissionMethodService.GetActiveSellerCommissionMethodsDropDown().ToListAsync();
			return Ok(allSellerCommissionMethods);
		}

		[HttpGet]
		[Route("GetSellerCommissionTypes")]
		public async Task<ActionResult<IEnumerable<SellerCommissionTypeDto>>> GetSellerCommissionTypes()
		{
			var allSellerCommissionMethods = await _sellerCommissionTypeService.GetSellerCommissionTypes().ToListAsync();
			return Ok(allSellerCommissionMethods);
		}

		[HttpGet]
		[Route("GetSellerCommissionMethodChangeData")]
		public async Task<ActionResult<SellerCommissionMethodChangeDto>> GetSellerCommissionMethodChangeData(int commissionMethodId)
		{
			var data = await _sellerService.GetSellerCommissionMethodChangeData(commissionMethodId);
			return Ok(data);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetSellerCommissionMethodById(int id)
		{
			var stateDb = await _sellerCommissionMethodService.GetSellerCommissionMethodById(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(stateDb?.CompanyId ?? 0, 0);
			return Ok(userCanLook ? stateDb : new SellerCommissionMethodDto());
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] SellerCommissionMethodDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _sellerCommissionMethodService.SaveSellerCommissionMethod(model);
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

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteSellerCommissionMethod(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _sellerCommissionMethodService.DeleteSellerCommissionMethod(id);
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
