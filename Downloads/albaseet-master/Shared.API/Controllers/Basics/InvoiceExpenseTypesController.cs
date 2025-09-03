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
    public class InvoiceExpenseTypesController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IInvoiceExpenseTypeService _invoiceExpenseTypeService;
        private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public InvoiceExpenseTypesController(IDatabaseTransaction databaseTransaction,IInvoiceExpenseTypeService invoiceExpenseTypeService,IStringLocalizer<HandelException> exLocalizer, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _invoiceExpenseTypeService = invoiceExpenseTypeService;
            _exLocalizer = exLocalizer;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadInvoiceExpenseTypes")]
        public IActionResult ReadInvoiceExpenseTypes(DataSourceLoadOptions loadOptions)
        {
            var data = DataSourceLoader.Load(_invoiceExpenseTypeService.GetCompanyInvoiceExpenseTypes(), loadOptions);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetAllInvoiceExpenseTypes")]
        public async Task<ActionResult<IEnumerable<InvoiceExpenseTypeDto>>> GetAllInvoiceExpenseTypes()
        {
            var allInvoiceExpenseTypes = await _invoiceExpenseTypeService.GetAllInvoiceExpenseTypes().ToListAsync();
            return Ok(allInvoiceExpenseTypes);
        }
        
        [HttpGet]
        [Route("GetAllInvoiceExpenseTypesDropDown")]
        public async Task<ActionResult<IEnumerable<InvoiceExpenseTypeDropDownDto>>> GetAllInvoiceExpenseTypesDropDown()
        {
            var allInvoiceExpenseTypes = await _invoiceExpenseTypeService.GetAllInvoiceExpenseTypesDropDown().ToListAsync();
            return Ok(allInvoiceExpenseTypes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInvoiceExpenseTypeById(int id)
        {
            var invoiceExpenseTypeDb = await _invoiceExpenseTypeService.GetInvoiceExpenseTypeById(id) ?? new InvoiceExpenseTypeDto();
			var userCanLook = await _httpContextAccessor.UserCanLook(invoiceExpenseTypeDb.CompanyId, 0);
            return Ok(userCanLook ? invoiceExpenseTypeDb : new InvoiceExpenseTypeDto());
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] InvoiceExpenseTypeDto model)
        {
            ResponseDto response;
            try
            {
	           await _databaseTransaction.BeginTransaction();
				response = await _invoiceExpenseTypeService.SaveInvoiceExpenseType(model);
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
        public async Task<IActionResult> DeleteInvoiceExpenseType(int id)
        {
            ResponseDto response;
            try
            {
				await _databaseTransaction.BeginTransaction();
                response = await _invoiceExpenseTypeService.DeleteInvoiceExpenseType(id);
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
