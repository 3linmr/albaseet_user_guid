using System.Data.SqlClient;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Extensions;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.API.Models;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Database;

namespace Shared.API.Controllers.Basics
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CountriesController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly ICountryService _countryService;
        private readonly IStringLocalizer<HandelException> _exLocalizer;

        public CountriesController(IDatabaseTransaction databaseTransaction,ICountryService countryService,IStringLocalizer<HandelException> exLocalizer)
        {
            _databaseTransaction = databaseTransaction;
            _countryService = countryService;
            _exLocalizer = exLocalizer;
        }

        [HttpGet]
        [Route("ReadCountries")]
        public IActionResult ReadCountries(DataSourceLoadOptions loadOptions)
        {
            var data = DataSourceLoader.Load(_countryService.GetAllCountries(), loadOptions);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetAllCountries")]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetAllCountries()
        {
            var allCountries = await _countryService.GetAllCountries().ToListAsync();
            return Ok(allCountries);
        }
        
        [HttpGet]
        [Route("GetAllCountriesDropDown")]
        public async Task<ActionResult<IEnumerable<CountryDropDownDto>>> GetAllCountriesDropDown()
        {
            var allCountries = await _countryService.GetAllCountriesDropDown().ToListAsync();
            return Ok(allCountries);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCountryById(int id)
        {
            var countryDb = await _countryService.GetCountryById(id) ?? new CountryDto();
            return Ok(countryDb);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] CountryDto model)
        {
            ResponseDto response;
            try
            {
	           await _databaseTransaction.BeginTransaction();
				response = await _countryService.SaveCountry(model);
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
        public async Task<IActionResult> DeleteCountry(int id)
        {
            ResponseDto response;
            try
            {
				await _databaseTransaction.BeginTransaction();
                response = await _countryService.DeleteCountry(id);
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
