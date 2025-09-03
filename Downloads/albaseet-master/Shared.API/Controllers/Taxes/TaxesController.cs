using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.Dtos.ViewModels.Taxes;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Taxes
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class TaxesController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly ITaxTypeService _taxTypeService;
		private readonly ITaxService _taxService;
		private readonly ITaxPercentService _taxPercentService;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public TaxesController(IDatabaseTransaction databaseTransaction,ITaxTypeService taxTypeService,ITaxService taxService,ITaxPercentService taxPercentService, IStringLocalizer<HandelException> exLocalizer, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_taxTypeService = taxTypeService;
			_taxService = taxService;
			_taxPercentService = taxPercentService;
			_exLocalizer = exLocalizer;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadTaxes")]
		public IActionResult ReadTaxes(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_taxService.GetUserTaxes(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadVatTaxDropDown")]
		public IActionResult ReadVatTaxDropDown(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load( _taxService.GeVatTaxDropDown(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllTaxes")]
		public async Task<IActionResult> GetAllTaxes()
		{
			return Ok(await _taxService.GetAllTaxes().ToListAsync());
		}

		[HttpGet]
		[Route("GetCompanyTaxes")]
		public async Task<IActionResult> GetCompanyTaxes(int companyId)
		{
			return Ok(await _taxService.GetCompanyTaxes(companyId).ToListAsync());
		}

		[HttpGet]
		[Route("GetStoreTaxes")]
		public async Task<IActionResult> GetStoreTaxes(int storeId)
		{
			return Ok(await _taxService.GetStoreTaxes(storeId).ToListAsync());
		}

		[HttpGet]
		[Route("ReadTaxPercents")]
		public IActionResult ReadTaxPercents(DataSourceLoadOptions loadOptions,int taxId)
		{
			var data = DataSourceLoader.Load(_taxPercentService.GetTaxPercentsByTaxId(taxId), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllTaxTypesDropDown")]
		public async Task<ActionResult<IEnumerable<TaxTypeDropDownDto>>> GetAllTaxTypesDropDown()
		{
			var taxes = await _taxTypeService.GetAllTaxTypesDropDown().ToListAsync();
			return Ok(taxes);
		}

		[HttpGet]
		[Route("GetLimitedTaxTypesDropDown")]
		public async Task<ActionResult<IEnumerable<TaxTypeDropDownDto>>> GetLimitedTaxTypesDropDown()
		{
			var taxes = await _taxTypeService.GetLimitedTaxTypesDropDown().ToListAsync();
			return Ok(taxes);
		}

		[HttpGet]
		[Route("GetAllTaxesDropDown")]
		public async Task<ActionResult<IEnumerable<TaxDropDownDto>>> GetAllTaxesDropDown()
		{
			var taxes = await _taxService.GetAllTaxesDropDown().ToListAsync();
			return Ok(taxes);
		}

		[HttpGet]
		[Route("GetCompanyTaxesDropDown")]
		public async Task<ActionResult<IEnumerable<TaxDropDownDto>>> GetCompanyTaxesDropDown(int companyId)
		{
			var taxes = await _taxService.GetCompanyTaxesDropDown(companyId).ToListAsync();
			return Ok(taxes);
		}
		
		[HttpGet]
		[Route("GetCompanyOtherTaxesDropDown")]
		public async Task<ActionResult<IEnumerable<TaxDropDownDto>>> GetCompanyOtherTaxesDropDown(int companyId)
		{
			var taxes = await _taxService.GetCompanyOtherTaxesDropDown(companyId).ToListAsync();
			return Ok(taxes);
		}

		[HttpGet]
		[Route("GetStoreTaxesDropDown")]
		public async Task<ActionResult<IEnumerable<TaxDropDownDto>>> GetStoreTaxesDropDown(int storeId)
		{
			var taxes = await _taxService.GetStoreTaxesDropDown(storeId).ToListAsync();
			return Ok(taxes);
		}
		
		[HttpGet]
		[Route("GeVatTaxDropDown")]
		public async Task<ActionResult<IEnumerable<TaxDropDownDto>>> GeVatTaxDropDown()
		{
			var taxes = await _taxService.GeVatTaxDropDown().ToListAsync();
			return Ok(taxes);
		}

		[HttpGet]
		[Route("GetStoreOtherTaxesDropDown")]
		public async Task<ActionResult<IEnumerable<TaxDropDownDto>>> GetStoreOtherTaxesDropDown(int storeId)
		{
			var taxes = await _taxService.GetStoreOtherTaxesDropDown(storeId).ToListAsync();
			return Ok(taxes);
		}

		[HttpGet]
		[Route("GetCurrentTaxPercent")]
		public async Task<ActionResult<decimal>> GetCurrentTaxPercent(int taxId, DateTime currentDate)
		{
			var tax = await _taxPercentService.GetCurrentTaxPercent(taxId, currentDate);
			return Ok(tax);
		}

		[HttpGet]
		[Route("GetVatTaxByCompanyId")]
		public async Task<ActionResult<decimal>> GetVatTaxByCompanyId(int companyId, DateTime documentDate)
		{
			var tax = await _taxPercentService.GetVatTaxByCompanyId(companyId, documentDate);
			return Ok(tax);
		}

		[HttpGet]
		[Route("GetVatTaxByStoreId")]
		public async Task<ActionResult<decimal>> GetVatTaxByStoreId(int storeId, DateTime documentDate)
		{
			var tax = await _taxPercentService.GetVatTaxByStoreId(storeId,documentDate);
			return Ok(tax);
		}
		
		[HttpGet]
		[Route("GetVatByCompanyId")]
		public async Task<ActionResult<TaxDto>> GetVatByCompanyId(int companyId, DateTime documentDate)
		{
			var tax = await _taxPercentService.GetVatByCompanyId(companyId, documentDate);
			return Ok(tax);
		}
		
		[HttpGet]
		[Route("GetVatByStoreId")]
		public async Task<ActionResult<TaxDto>> GetVatByStoreId(int storeId, DateTime documentDate)
		{
			var tax = await _taxPercentService.GetVatByStoreId(storeId,documentDate);
			return Ok(tax);
		}
		
		[HttpGet]
		[Route("IsVatTaxExist")]
		public async Task<ActionResult<TaxDto>> IsVatTaxExist()
		{
			var tax = await _taxService.IsVatTaxExist();
			return Ok(tax);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetTaxById(int id)
		{
			var taxes = await _taxService.GetTaxById(id) ?? new TaxDto();
			var userCanLook = await _httpContextAccessor.UserCanLook(taxes.CompanyId, 0);
			if (!userCanLook) return Ok(new TaxDto());

			var taxPercents = await _taxPercentService.GetTaxPercentsByTaxId(id).ToListAsync();
			var model = new TaxVm() { Taxes = taxes, TaxPercents = taxPercents };
			return Ok(model);
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] TaxVm model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _taxService.SaveTax(model.Taxes!);
				if (response.Success)
				{
					await _taxPercentService.SaveTaxPercents(model.TaxPercents!, response.Id);
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
		public async Task<IActionResult> DeleteTax(int id)
		{
			var response = new ResponseDto();
			try
			{
				await _databaseTransaction.BeginTransaction();
				var result = await _taxPercentService.DeleteTaxByTaxId(id);
				if (result.Success)
				{
					response = await _taxService.DeleteTax(id);
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
