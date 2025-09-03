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
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
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
	public class ClientsController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IClientService _clientService;
		private readonly IMenuNoteService _menuNoteService;
        private readonly IAccountEntityService _accountEntityService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IClientService clientService,IMenuNoteService menuNoteService,IAccountEntityService accountEntityService, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_clientService = clientService;
			_menuNoteService = menuNoteService;
            _accountEntityService = accountEntityService;
			_httpContextAccessor = httpContextAccessor;
        }


		[HttpGet]
		[Route("ReadClients")]
		public IActionResult ReadClients(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_clientService.GetUserClients(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadClientsByCompanyId")]
		public IActionResult ReadClientsByCompanyId(DataSourceLoadOptions loadOptions,int companyId)
		{
			var data = DataSourceLoader.Load(_clientService.GetClientsByCompanyId(companyId), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadClientsDropDown")]
		public async Task<IActionResult> ReadClientsDropDown(DataSourceLoadOptions loadOptions)
		{
			try
			{
				var data = DataSourceLoader.Load(_clientService.GetClientsDropDown(), loadOptions);
				return Ok(data);
			}
			catch (Exception e)
			{
				var data = DataSourceLoader.Load(await _clientService.GetClientsDropDown().ToListAsync(), loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("GetAllClients")]
		public async Task<ActionResult<IEnumerable<ClientDto>>> GetAllClients()
		{
			var allClients = await _clientService.GetAllClients().ToListAsync();
			return Ok(allClients);
		}

		[HttpGet]
		[Route("GetClientsByCompanyId")]
		public async Task<ActionResult<IEnumerable<ClientDto>>> GetClientsByCompanyId(int companyId)
		{
			var allClients = await _clientService.GetAllClients().ToListAsync();
			return Ok(allClients);
		}

		[HttpGet]
		[Route("GetUserClientDropDown")]
		public async Task<ActionResult<IEnumerable<ClientDropDownDto>>> GetUserClientDropDown()
		{
			var allCountries = await _clientService.GetClientsDropDown().ToListAsync();
			return Ok(allCountries);
		}

		[HttpGet]
		[Route("GetClientsByCompanyIdDropDown")]
		public async Task<ActionResult<IEnumerable<ClientDropDownDto>>> GetClientsByCompanyIdDropDown(int companyId)
		{
			var allCountries = await _clientService.GetClientsByCompanyIdDropDown(companyId).ToListAsync();
			return Ok(allCountries);
		}

		[HttpGet]
		[Route("GetClientsAutoComplete")]
		public async Task<ActionResult<IEnumerable<ClientAutoCompleteDto>>> GetClientsAutoComplete(string term)
		{
			var data = await _clientService.GetClientsAutoComplete(term);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetClientsAutoCompleteByStoreIds")]
		public async Task<ActionResult<IEnumerable<ClientAutoCompleteDto>>> GetClientsAutoCompleteByStoreIds(string term, string? storeIds)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]")!;
			var data = await _clientService.GetClientsAutoCompleteByStoreIds(term, storeIdsList);
			return Ok(data);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetClientById(int id)
		{
			var ClientDb = await _clientService.GetClientById(id) ?? new ClientDto();
			var userCanLook = await _httpContextAccessor.UserCanLook(ClientDb.CompanyId, 0);
			return Ok(userCanLook ? ClientDb : new ClientDto());
		}

		[HttpGet]
		[Route("GetClientByAccountId")]
		public async Task<ActionResult<ClientDto>> GetClientByAccountId(int accountId)
		{
			var clients = await _clientService.GetClientByAccountId(accountId);
			return Ok(clients);
		}

		[HttpGet("GetClientFullAddress")]
		public async Task<ActionResult<string>> GetClientFullAddress(int id)
		{
			var fullAddress = await _clientService.GetClientFullAddress(id);
			return Ok(fullAddress);
		}

        [HttpGet("GetClientFullResponsibleName")]
        public async Task<ActionResult<string>> GetClientFullResponsibleName(int id)
        {
            var allResponsibleName = await _clientService.GetClientFullResponsibleName(id);
            return Ok(allResponsibleName);
        }

        [HttpGet("GetClientFullResponsiblePhone")]
        public async Task<ActionResult<string>> GetClientFullResponsiblePhone(int id)
        {
            var allResponsiblePhone = await _clientService.GetClientFullResponsiblePhone(id);
            return Ok(allResponsiblePhone);
        }

        [HttpPost]
		public async Task<IActionResult> Save([FromBody] ClientDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _accountEntityService.SaveClient(model);
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
		public async Task<IActionResult> DeleteClient(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _accountEntityService.DeleteClient(id);
				if (response.Success)
				{
					await _menuNoteService.DeleteMenuNotes(MenuCodeData.Client, id);
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
