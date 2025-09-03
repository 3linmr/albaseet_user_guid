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

namespace Shared.API.Controllers.Modules
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CurrenciesController : ControllerBase
	{
		private readonly ICurrencyService _currencyService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;

		public CurrenciesController(ICurrencyService currencyService,IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer)
		{
			_currencyService = currencyService;
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
		}


		[HttpGet]
		[Route("ReadCurrencies")]
		public IActionResult ReadCurrencies(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_currencyService.GetAllCurrencies(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadCurrenciesDropDown")]
		public async Task<IActionResult> ReadCurrenciesDropDown(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load( await _currencyService.GetCurrenciesDropDown(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllCurrencies")]
		public async Task<ActionResult<IEnumerable<CurrencyDto>>> GetAllCurrencies()
		{
			var currencies = await _currencyService.GetAllCurrencies().ToListAsync();
			return Ok(currencies);
		}

		[HttpGet]
		[Route("GetCurrenciesDropDown")]
		public async Task<ActionResult<IEnumerable<CurrencyDropDownDto>>> GetCurrenciesDropDown()
		{
			var currencies = await _currencyService.GetCurrenciesDropDown();
			return Ok(currencies);
		}

		[HttpGet]
		[Route("GetCurrencySymbolById")]
		public async Task<ActionResult<string>> GetCurrencySymbolById(int currencyId)
		{
			var symbol = await _currencyService.GetCurrencySymbolById(currencyId);
			return Ok(new { symbol = symbol });
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetCurrencyById(int id)
		{
			var currencyDb = await _currencyService.GetCurrencyById(id) ?? new CurrencyDto();
			return Ok(currencyDb);
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] CurrencyDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _currencyService.SaveCurrency(model);
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
		public async Task<IActionResult> DeleteCurrency(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _currencyService.DeleteCurrency(id);
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
