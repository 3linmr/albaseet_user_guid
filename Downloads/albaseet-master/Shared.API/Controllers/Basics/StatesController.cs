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
    public class StatesController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStateService _stateService;
        private readonly IStringLocalizer<HandelException> _exceptionLocalizer;

        public StatesController(IDatabaseTransaction databaseTransaction, IStateService stateService, IStringLocalizer<HandelException> exceptionLocalizer)
        {
            _databaseTransaction = databaseTransaction;
            _stateService = stateService;
            _exceptionLocalizer = exceptionLocalizer;
        }

        [HttpGet]
        [Route("ReadStates")]
        public IActionResult ReadStates(DataSourceLoadOptions loadOptions)
        {
            var data = DataSourceLoader.Load(_stateService.GetAllStates(), loadOptions);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetAllStates")]
        public async Task<ActionResult<IEnumerable<StateDto>>> GetAllStates()
        {
            var allStates = await _stateService.GetAllStates().ToListAsync();
            return Ok(allStates);
        }
        
        [HttpGet]
        [Route("GetAllStatesByCountryId")]
        public async Task<ActionResult<IEnumerable<StateDto>>> GetAllStatesByCountryId(int countryId)
        {
            var allStates = await _stateService.GetAllStatesByCountryId(countryId).ToListAsync();
            return Ok(allStates);
        }
        [HttpGet]
        [Route("GetStatesByCountryIdDropDown")]
        public async Task<ActionResult<IEnumerable<StateDto>>> GetStatesByCountryIdDropDown(int countryId)
        {
            var allStates = await _stateService.GetStatesByCountryIdDropDown(countryId).ToListAsync();
            return Ok(allStates);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStateById(int id)
        {
            var stateDb = await _stateService.GetStateById(id) ?? new StateDto();
            return Ok(stateDb);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] StateDto model)
        {
            ResponseDto response;
            try
            {
				await _databaseTransaction.BeginTransaction();
                response = await _stateService.SaveState(model);
                if (response.Success)
                {
                  await  _databaseTransaction.Commit();
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
        public async Task<IActionResult> DeleteState(int id)
        {
            ResponseDto response;
            try
            {
				await _databaseTransaction.BeginTransaction();
                response = await _stateService.DeleteState(id);
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
