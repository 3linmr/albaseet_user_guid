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
	public class SuppliersController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly ISupplierService _supplierService;
		private readonly IMenuNoteService _menuNoteService;
        private readonly IAccountEntityService _accountEntityService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public SuppliersController(IDatabaseTransaction databaseTransaction,IStringLocalizer<HandelException> exLocalizer,ISupplierService supplierService,IMenuNoteService menuNoteService,IAccountEntityService accountEntityService, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_supplierService = supplierService;
			_menuNoteService = menuNoteService;
            _accountEntityService = accountEntityService;
			_httpContextAccessor = httpContextAccessor;
        }


		[HttpGet]
		[Route("ReadSuppliers")]
		public IActionResult ReadSuppliers(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_supplierService.GetUserSuppliers(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadSuppliersDropDown")]
		public async Task<IActionResult> ReadSuppliersDropDown(DataSourceLoadOptions loadOptions)
		{
			try
			{
				var data = DataSourceLoader.Load(_supplierService.GetAllSuppliersDropDown(), loadOptions);
				return Ok(data);
			}
			catch (Exception e)
			{
				var data = DataSourceLoader.Load(await _supplierService.GetAllSuppliersDropDown().ToListAsync(), loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("GetAllSuppliers")]
		public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAllSuppliers()
		{
			var allSuppliers = await _supplierService.GetAllSuppliers().ToListAsync();
			return Ok(allSuppliers);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetSupplierById(int id)
		{
			var SupplierDb = await _supplierService.GetSupplierById(id) ?? new SupplierDto();
			var userCanLook = await _httpContextAccessor.UserCanLook(SupplierDb.CompanyId, 0);
			return Ok(userCanLook ? SupplierDb : new SupplierDto());
		}

		[HttpGet]
		[Route("GetSupplierByAccountId")]
		public async Task<ActionResult<SupplierDto>> GetSupplierByAccountId(int accountId)
		{
			var suppliers = await _supplierService.GetSupplierByAccountId(accountId);
			return Ok(suppliers);
		}

		[HttpGet]
		[Route("GetAllSuppliersDropDown")]
		public async Task<ActionResult<IEnumerable<SupplierDropDownDto>>> GetAllSuppliersDropDown()
		{
			var suppliers = await _supplierService.GetAllSuppliersDropDown().ToListAsync();
			return Ok(suppliers);
		}

        [HttpGet]
        [Route("GetSuppliersDropDownByCompanyId")]
        public async Task<ActionResult<IEnumerable<SupplierDropDownDto>>> GetSuppliersDropDownByCompanyId(int companyId)
        {
            var suppliers = await _supplierService.GetSuppliersDropDownByCompanyId(companyId).ToListAsync();
            return Ok(suppliers);
        }


		[HttpGet]
        [Route("GetSuppliersDropDownByStoreId")]
        public async Task<ActionResult<IEnumerable<SupplierDropDownDto>>> GetSuppliersDropDownByStoreId(int storeId)
        {
            var suppliers = await _supplierService.GetSuppliersDropDownByStoreId(storeId);
            return Ok(await suppliers.ToListAsync());
        }

        [HttpGet]
		[Route("GetSuppliersAutoComplete")]
		public async Task<ActionResult<IEnumerable<SupplierAutoCompleteDto>>> GetSuppliersAutoComplete(int companyId, string term)
		{
			var data = await _supplierService.GetSuppliersAutoComplete(companyId, term);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetSuppliersAutoCompleteByStoreIds")]
		public async Task<ActionResult<IEnumerable<SupplierAutoCompleteDto>>> GetSuppliersAutoCompleteByStoreIds(string term, string? storeIds)
		{
			var storeIdsList = JsonSerializer.Deserialize<List<int>>(storeIds ?? "[]")!;
			var data = await _supplierService.GetSuppliersAutoCompleteByStoreIds(term, storeIdsList);
			return Ok(data);
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] SupplierDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _accountEntityService.SaveSupplier(model);
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
					await _databaseTransaction.Rollback();
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
		public async Task<IActionResult> DeleteSupplier(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _accountEntityService.DeleteSupplier(id);
				if (response.Success)
				{
					await _menuNoteService.DeleteMenuNotes(MenuCodeData.Supplier, response.Id);
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
