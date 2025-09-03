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
using Shared.Service.Services.Modules;

namespace Shared.API.Controllers.Modules
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SellerCommissionsController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exceptionLocalizer;
		private readonly ISellerCommissionService _sellerCommissionService;
		private readonly IMenuNoteService _menuNoteService;

		public SellerCommissionsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exceptionLocalizer, ISellerCommissionService sellerCommissionService, IMenuNoteService menuNoteService)
		{
			_databaseTransaction = databaseTransaction;
			_exceptionLocalizer = exceptionLocalizer;
			_sellerCommissionService = sellerCommissionService;
			_menuNoteService = menuNoteService;
		}

		[HttpGet]
		[Route("ReadSellerCommissions")]
		public IActionResult ReadSellerCommissions(DataSourceLoadOptions loadOptions, int sellerCommissionMethodId)
		{
			var data = DataSourceLoader.Load(_sellerCommissionService.GetSellerCommissionsByType(sellerCommissionMethodId), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllSellerCommissions")]
		public async Task<ActionResult<IEnumerable<SellerCommissionDto>>> GetAllSellerCommissions()
		{
			var allSellerCommissions = await _sellerCommissionService.GetAllSellerCommissions().ToListAsync();
			return Ok(allSellerCommissions);
		}

		[HttpGet]
		[Route("GetSellerCommissionsByType")]
		public async Task<ActionResult<IEnumerable<SellerCommissionDto>>> GetSellerCommissionsByType(int sellerCommissionMethodId)
		{
			var sellerCommissions = await _sellerCommissionService.GetSellerCommissionsByType(sellerCommissionMethodId).ToListAsync();
			return Ok(sellerCommissions);
		}	
		
		[HttpGet]
		[Route("GetSellerCommissionsByCommissionMethodId")]
		public async Task<ActionResult<List<SellerCommissionDto>>> GetSellerCommissionsByCommissionMethodId(int sellerCommissionMethodId)
		{
			var sellerCommissions = await _sellerCommissionService.GetSellerCommissionsByCommissionMethodId(sellerCommissionMethodId);
			return Ok(sellerCommissions);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetSellerCommissionById(int id)
		{
			var stateDb = await _sellerCommissionService.GetSellerCommissionById(id);
			return Ok(stateDb);
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] SellerCommissionDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _sellerCommissionService.SaveSellerCommission(model);
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
		public async Task<IActionResult> DeleteSellerCommission(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _sellerCommissionService.DeleteSellerCommission(id);
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
