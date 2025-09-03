using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;

namespace Shared.API.Controllers.Basics
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CitiesController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exceptionLocalizer;
        private readonly ICityService _cityService;

        public CitiesController(IDatabaseTransaction databaseTransaction,IStringLocalizer<HandelException> exceptionLocalizer,ICityService cityService)
        {
            _databaseTransaction = databaseTransaction;
            _exceptionLocalizer = exceptionLocalizer;
            _cityService = cityService;
        }

        [HttpGet]
        [Route("ReadCities")]
        public IActionResult ReadCities(DataSourceLoadOptions loadOptions)
        {
            var data = DataSourceLoader.Load(_cityService.GetAllCities(), loadOptions);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetAllCities")]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetAllCities()
        {
            var allCities = await _cityService.GetAllCities().ToListAsync();
            return Ok(allCities);
        }

        [HttpGet]
        [Route("GetAllCitiesByStateId")]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetAllCitiesByStateId(int stateId)
        {
            var allCities = await _cityService.GetAllCitiesByStateId(stateId).ToListAsync();
            return Ok(allCities);
        }
        [HttpGet]
        [Route("GetCitiesByStateIdDropDown")]
        public async Task<ActionResult<IEnumerable<CityDropDownDto>>> GetCitiesByStateIdDropDown(int stateId)
        {
            var allCities = await _cityService.GetCitiesByStateIdDropDown(stateId).ToListAsync();
            return Ok(allCities);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCityById(int id)
        {
            var stateDb = await _cityService.GetCityById(id);
            return Ok(stateDb);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] CityDto model)
        {
            ResponseDto response;
            try
            {
				await _databaseTransaction.BeginTransaction();
                response = await _cityService.SaveCity(model);
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
                var handleException = new HandelException(_exceptionLocalizer);
                return Ok(handleException.Handle(e));
            }
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            ResponseDto response;
            try
            {
				await _databaseTransaction.BeginTransaction();
                response = await _cityService.DeleteCity(id);
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
