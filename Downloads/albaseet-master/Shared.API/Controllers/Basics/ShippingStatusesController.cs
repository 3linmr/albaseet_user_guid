using System.Data.SqlClient;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.API.Models;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Database;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Basics
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShippingStatusesController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IShippingStatusService _shippingStatusService;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public ShippingStatusesController(IDatabaseTransaction databaseTransaction, IShippingStatusService shippingStatusService, IStringLocalizer<HandelException> exLocalizer, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _shippingStatusService = shippingStatusService;
            _exLocalizer = exLocalizer;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadShippingStatuses")]
        public IActionResult ReadShippingStatuses(DataSourceLoadOptions loadOptions)
        {
            var data = DataSourceLoader.Load(_shippingStatusService.GetCompanyShippingStatuses(), loadOptions);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetAllShippingStatuses")]
        public async Task<ActionResult<IEnumerable<ShippingStatusDto>>> GetAllShippingStatuses()
        {
            var allShippingStatuses = await _shippingStatusService.GetAllShippingStatuses().ToListAsync();
            return Ok(allShippingStatuses);
        }

        [HttpGet]
        [Route("GetAllShippingStatusesDropDown")]
        public async Task<ActionResult<IEnumerable<ShippingStatusDropDownDto>>> GetAllShippingStatusesDropDown(int menuCode)
        {
            var allShippingStatuses = await _shippingStatusService.GetAllShippingStatusesDropDown(menuCode).ToListAsync();
            return Ok(allShippingStatuses);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetShippingStatusById(int id)
        {
            var shippingStatusDb = await _shippingStatusService.GetShippingStatusById(id) ?? new ShippingStatusDto();
			var userCanLook = await _httpContextAccessor.UserCanLook(shippingStatusDb.CompanyId, 0);
            return Ok(userCanLook ? shippingStatusDb : new ShippingStatusDto());
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] ShippingStatusDto model)
        {
            ResponseDto response;
            try
            {
                await _databaseTransaction.BeginTransaction();
                response = await _shippingStatusService.SaveShippingStatus(model);
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
        public async Task<IActionResult> DeleteShippingStatus(int id)
        {
            ResponseDto response;
            try
            {
                await _databaseTransaction.BeginTransaction();
                response = await _shippingStatusService.DeleteShippingStatus(id);
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
