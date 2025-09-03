using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Modules
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class PaymentMethodsController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exceptionLocalizer;
		private readonly IPaymentMethodService _paymentMethodService;
		private readonly IPaymentTypeService _paymentTypeService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public PaymentMethodsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exceptionLocalizer, IPaymentMethodService paymentMethodService,IPaymentTypeService paymentTypeService, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_exceptionLocalizer = exceptionLocalizer;
			_paymentMethodService = paymentMethodService;
			_paymentTypeService = paymentTypeService;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadPaymentMethods")]
		public IActionResult ReadPaymentMethods(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_paymentMethodService.GetUserPaymentMethods(), loadOptions);
			return Ok(data);
		}
		
		[HttpGet]
		[Route("ReadCompanyPaymentMethods")]
		public IActionResult ReadCompanyPaymentMethods(DataSourceLoadOptions loadOptions,int companyId)
		{
			var data = DataSourceLoader.Load(_paymentMethodService.GetCompanyPaymentMethods(companyId), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadPaymentMethodsAccounts")]
		public IActionResult ReadPaymentMethodsAccounts(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_paymentMethodService.GetPaymentMethodsAccounts(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadReceivingMethodsAccounts")]
		public IActionResult ReadReceivingMethodsAccounts(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_paymentMethodService.GetReceivingMethodsAccounts(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetPaymentMethodsAccounts")]
		public IActionResult GetPaymentMethodsAccounts()
		{
			var data = _paymentMethodService.GetPaymentMethodsAccounts();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetReceivingMethodsAccounts")]
		public IActionResult GetReceivingMethodsAccounts()
		{
			var data = _paymentMethodService.GetReceivingMethodsAccounts();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllPaymentMethods")]
		public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetAllPaymentMethods()
		{
			var allPaymentMethods = await _paymentMethodService.GetAllPaymentMethods().ToListAsync();
			return Ok(allPaymentMethods);
		}

		[HttpGet]
		[Route("GetCompanyPaymentMethods")]
		public async Task<IActionResult> GetCompanyPaymentMethods(int companyId)
		{
			var data = await _paymentMethodService.GetCompanyPaymentMethods(companyId).ToListAsync();
			return Ok(data);
		}
		
		[HttpGet]
		[Route("GetReceiptVoucherPaymentMethods")]
		public async Task<IActionResult> GetReceiptVoucherPaymentMethods(int storeId, bool isCashOnly)
		{
			var data = await _paymentMethodService.GetVoucherPaymentMethods(storeId,false, isCashOnly);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetPaymentVoucherPaymentMethods")]
		public async Task<IActionResult> GetPaymentVoucherPaymentMethods(int storeId, bool isCashOnly)
		{
			var data = await _paymentMethodService.GetVoucherPaymentMethods(storeId, true, isCashOnly);
			return Ok(data);
		}
		
		[HttpGet]
		[Route("GetPaymentMethodEntries")]
		public async Task<JournalDetailCalculationVm> GetPaymentMethodEntries(int storeId, int paymentMethodId, decimal debitValue, decimal creditValue)
		{
			var data = await _paymentMethodService.GetPaymentMethodEntries( storeId,paymentMethodId,debitValue,creditValue);
			return data;
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetPaymentMethodById(int id)
		{
			var paymentMethods = await _paymentMethodService.GetPaymentMethodById(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(paymentMethods.CompanyId, 0);
			return Ok(userCanLook ? paymentMethods : new PaymentMethodDto());
		}

		[HttpGet]
		[Route("GetPaymentTypes")]
		public async Task<ActionResult<IEnumerable<PaymentTypeDto>>> GetPaymentTypes()
		{
			var paymentMethodTypes = await _paymentTypeService.GetPaymentTypesDropDown();
			return Ok(paymentMethodTypes);
		}

		[HttpGet]
		[Route("GetDefaultCashMethod")]
		public async Task<ActionResult<PaymentMethodDto>> GetDefaultCashMethod()
		{
			var paymentMethod = await _paymentMethodService.GetDefaultCashMethod();
			return Ok(paymentMethod);
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] PaymentMethodDto model)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _paymentMethodService.SavePaymentMethod(model);
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

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeletePaymentMethod(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _paymentMethodService.DeletePaymentMethod(id);

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
