using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
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
	public class BanksController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exceptionLocalizer;
		private readonly IBankService _bankService;
		private readonly IMenuNoteService _menuNoteService;
        private readonly IAccountEntityService _accountEntityService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public BanksController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exceptionLocalizer,IBankService bankService,IMenuNoteService menuNoteService,IAccountEntityService accountEntityService, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_exceptionLocalizer = exceptionLocalizer;
			_bankService = bankService;
			_menuNoteService = menuNoteService;
            _accountEntityService = accountEntityService;
			_httpContextAccessor = httpContextAccessor;
        }

		[HttpGet]
		[Route("ReadBanks")]
		public IActionResult ReadBanks(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_bankService.GetUserBanks(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllBanks")]
		public async Task<ActionResult<IEnumerable<BankDto>>> GetAllBanks()
		{
			var allBanks = await _bankService.GetAllBanks().ToListAsync();
			return Ok(allBanks);
		}

		[HttpGet]
		[Route("GetBanksDropDown")]
		public async Task<ActionResult<IEnumerable<BankDropDownDto>>> GetBanksDropDown()
		{
			var allBanks = await _bankService.GetBanksDropDown().ToListAsync();
			return Ok(allBanks);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetBankById(int id)
		{
			var stateDb = await _bankService.GetBankById(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(stateDb?.CompanyId ?? 0, 0);
			return Ok(userCanLook ? stateDb : new BankDto());
		}

		[HttpGet]
		[Route("GetBanksAutoComplete")]
		public async Task<ActionResult<IEnumerable<BankAutoCompleteDto>>> GetBanksAutoComplete(string term)
		{
			var data = await _bankService.GetBanksAutoComplete(term);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetBanksAutoCompleteByStoreIds")]
		public async Task<ActionResult<IEnumerable<BankAutoCompleteDto>>> GetBanksAutoCompleteByStoreIds(string term, string? storeIds)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]")!;
			var data = await _bankService.GetBanksAutoCompleteByStoreIds(term, storeIdsList);
			return Ok(data);
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] BankDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _accountEntityService.SaveBank(model);
				if (response.Success)
				{
					if (response.Success)
					{
						if (model.MenuNotes != null)
						{
							await _menuNoteService.SaveMenuNotes(model.MenuNotes, response.Id);
						}

						if (response.Success)
						{
							await _databaseTransaction.Commit();
						}
						else
						{
							await _databaseTransaction.Rollback();
						}
					}
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
				var handleException = new HandelException(_exceptionLocalizer);
				return Ok(handleException.Handle(e));
			}
			return Ok(response);
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteBank(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _accountEntityService.DeleteBank(id);
				await _menuNoteService.DeleteMenuNotes(MenuCodeData.Bank, response.Id);
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
