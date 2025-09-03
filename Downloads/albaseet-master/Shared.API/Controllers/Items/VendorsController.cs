using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Service.Services.Modules;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Items
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VendorsController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exceptionLocalizer;
        private readonly IVendorService _vendorService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public VendorsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exceptionLocalizer, IVendorService vendorService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exceptionLocalizer = exceptionLocalizer;
            _vendorService = vendorService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadVendors")]
        public IActionResult ReadVendors(DataSourceLoadOptions loadOptions)
        {
            var data = DataSourceLoader.Load(_vendorService.GetCompanyVendors(), loadOptions);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetAllVendors")]
        public async Task<ActionResult<IEnumerable<VendorDto>>> GetAllVendors()
        {
            var allVendors = await _vendorService.GetAllVendors().ToListAsync();
            return Ok(allVendors);
        }

        [HttpGet]
        [Route("GetVendorsDropDown")]
        public async Task<ActionResult<IEnumerable<VendorDropDownDto>>> GetVendorsDropDown()
        {
            var allVendors = await _vendorService.GetVendorsDropDown().ToListAsync();
            return Ok(allVendors);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetVendorById(int id)
        {
            var stateDb = await _vendorService.GetVendorById(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(stateDb?.CompanyId ?? 0, 0);
            return Ok(userCanLook ? stateDb : new VendorDto());
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] VendorDto model)
        {
            ResponseDto response;
            try
            {
                await _databaseTransaction.BeginTransaction();
                response = await _vendorService.SaveVendor(model);
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
        public async Task<IActionResult> DeleteVendor(int id)
        {
            ResponseDto response;
            try
            {
                await _databaseTransaction.BeginTransaction();
                response = await _vendorService.DeleteVendor(id);
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
