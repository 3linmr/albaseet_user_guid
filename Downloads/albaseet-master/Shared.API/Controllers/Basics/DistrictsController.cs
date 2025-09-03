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
using Shared.Service.Services.Basics;

namespace Shared.API.Controllers.Basics
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DistrictsController : ControllerBase
    {
        private readonly IDistrictService _districtService;
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exceptionLocalizer;

        public DistrictsController(IDistrictService districtService,IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exceptionLocalizer)
        {
            _districtService = districtService;
            _databaseTransaction = databaseTransaction;
            _exceptionLocalizer = exceptionLocalizer;
        }

        [HttpGet]
        [Route("ReadDistricts")]
        public IActionResult ReadDistricts(DataSourceLoadOptions loadOptions)
        {
            var data = DataSourceLoader.Load(_districtService.GetAllDistricts(), loadOptions);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetAllDistricts")]
        public async Task<ActionResult<IEnumerable<DistrictDto>>> GetAllDistricts()
        {
            var allDistricts = await _districtService.GetAllDistricts().ToListAsync();
            return Ok(allDistricts);
        }

        [HttpGet]
        [Route("GetAllDistrictsByCityId")]
        public async Task<ActionResult<IEnumerable<DistrictDto>>> GetAllDistrictsByCityId(int cityId)
        {
            var allDistricts = await _districtService.GetAllDistrictsByCityId(cityId).ToListAsync();
            return Ok(allDistricts);
        }
        [HttpGet]
        [Route("GetDistrictsByCityIdIdDropDown")]
        public async Task<ActionResult<IEnumerable<DistrictDropDownDto>>> GetDistrictsByCityIdIdDropDown(int cityId)
        {
            var allDistricts = await _districtService.GetDistrictsByCityIdIdDropDown(cityId).ToListAsync();
            return Ok(allDistricts);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDistrictById(int id)
        {
            var stateDb = await _districtService.GetDistrictById(id);
            return Ok(stateDb);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] DistrictDto model)
        {
            ResponseDto response;
            try
            {
				await _databaseTransaction.BeginTransaction();
                response = await _districtService.SaveDistrict(model);
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
        public async Task<IActionResult> DeleteDistrict(int id)
        {
            ResponseDto response;
            try
            {
				await _databaseTransaction.BeginTransaction();
                response = await _districtService.DeleteDistrict(id);
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
