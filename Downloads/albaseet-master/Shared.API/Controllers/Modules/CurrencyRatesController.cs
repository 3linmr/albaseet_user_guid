using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
	public class CurrencyRatesController : ControllerBase
	{
		private readonly ICurrencyRateService _currencyRateService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exceptionLocalizer;

		public CurrencyRatesController(ICurrencyRateService currencyRateService,IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exceptionLocalizer)
		{
			_currencyRateService = currencyRateService;
			_databaseTransaction = databaseTransaction;
			_exceptionLocalizer = exceptionLocalizer;
		}

		[HttpGet]
		[Route("ReadCurrencyRates")]
		public IActionResult ReadCurrencyRates(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_currencyRateService.GetCurrencyRates(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetCurrencyRateData")]
		public async Task<IActionResult> GetCurrencyRateData(int fromCurrencyId, int toCurrencyId)
		{
			var data =await _currencyRateService.GetCurrencyRateData(fromCurrencyId, toCurrencyId);
			return Ok(data);
		}
		
		[HttpGet]
		[Route("GetCurrencyRate")]
		public async Task<IActionResult> GetCurrencyRate(int fromCurrencyId, int toCurrencyId)
		{
			var data =await _currencyRateService.GetCurrencyRate(fromCurrencyId, toCurrencyId);
			return Ok(data);
		}


		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetCurrencyRateById(int id)
		{
			var stateDb = await _currencyRateService.GetCurrencyRateById(id);
			return Ok(stateDb);
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] CurrencyRateDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _currencyRateService.SaveCurrencyRate(model);
				if (response.Success)
				{
					if (response.Success)
					{
						await _databaseTransaction.Commit();
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
		public async Task<IActionResult> DeleteCurrencyRate(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _currencyRateService.DeleteCurrencyRate(id);
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
