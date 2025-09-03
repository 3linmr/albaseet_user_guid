using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Database;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Accounts
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class AccountsController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exLocalizer;
		private readonly IAccountService _accountService;
		private readonly IHandleApprovalRequestService _handleApprovalRequestService;
		private readonly IApprovalSystemService _approvalSystemService;
		private readonly IAccountEntityService _accountEntityService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AccountsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exLocalizer, IAccountService accountService, IHandleApprovalRequestService handleApprovalRequestService, IApprovalSystemService approvalSystemService, IAccountEntityService accountEntityService, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_exLocalizer = exLocalizer;
			_accountService = accountService;
			_handleApprovalRequestService = handleApprovalRequestService;
			_approvalSystemService = approvalSystemService;
			_accountEntityService = accountEntityService;
			_httpContextAccessor = httpContextAccessor;
		}


		[HttpGet]
		[Route("ReadAccountsTree")]
		public IActionResult ReadAccountsTree(DataSourceLoadOptions loadOptions, int companyId)
		{
			var data = DataSourceLoader.Load(_accountService.GetAccountsTree(companyId), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadAccountsSimpleTreeByCompanyId")]
		public async Task<IActionResult> ReadAccountsSimpleTreeByCompanyId(DataSourceLoadOptions loadOptions, int companyId, int? mainAccountId)
		{
			var data = DataSourceLoader.Load(await _accountService.GetAccountsSimpleTreeByCompanyId(companyId, mainAccountId), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetMainAccountIdByAccountType")]
		public async Task<IActionResult> GetMainAccountIdByAccountType(int accountTypeId)
		{
			var data = await _accountService.GetMainAccountIdByAccountType(accountTypeId);
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadMainAccountsByAccountTypeId")]
		public async Task<IActionResult> ReadMainAccountsByAccountTypeId(DataSourceLoadOptions loadOptions, int accountTypeId)
		{
			try
			{
				var data = DataSourceLoader.Load(_accountService.GetMainAccountsByAccountTypeId(accountTypeId),
					loadOptions);
				return Ok(data);
			}
			catch
			{
				var data = DataSourceLoader.Load(await _accountService.GetMainAccountsByAccountTypeId(accountTypeId).ToListAsync(),
					loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("GetMainAccountsByAccountTypeId")]
		public IActionResult GetMainAccountsByAccountTypeId(int accountTypeId)
		{
			var data = _accountService.GetMainAccountsByAccountTypeId(accountTypeId);
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadAccountsSimpleTreeByStoreId")]
		public async Task<IActionResult> ReadAccountsSimpleTreeByStoreId(DataSourceLoadOptions loadOptions, int storeId, int? mainAccountId)
		{
			var data = DataSourceLoader.Load(await _accountService.GetAccountsSimpleTreeByStoreId(storeId, mainAccountId), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("ReadIndividualAccountsByCompanyId")]
		public async Task<IActionResult> ReadIndividualAccountsByCompanyId(DataSourceLoadOptions loadOptions, int companyId)
		{
			try
			{
				var data = DataSourceLoader.Load(_accountService.GetIndividualAccountsByCompanyId(companyId), loadOptions);
				return Ok(data);
			}
			catch
			{
				var data = DataSourceLoader.Load(await _accountService.GetIndividualAccountsByCompanyId(companyId).ToListAsync(), loadOptions);
				return Ok(data);
			}
		}
		
		[HttpGet]
		[Route("ReadIndividualAccounts")]
		public async Task<IActionResult> ReadIndividualAccounts(DataSourceLoadOptions loadOptions, int storeId)
		{
			try
			{
				var data = DataSourceLoader.Load(_accountService.GetIndividualAccounts(storeId), loadOptions);
				return Ok(data);
			}
			catch
			{
				var data = DataSourceLoader.Load(await _accountService.GetIndividualAccounts(storeId).ToListAsync(), loadOptions);
				return Ok(data);
			}
		}

		[HttpGet]
		[Route("GetIndividualAccountsByCompanyId")]
		public async Task<IActionResult> GetIndividualAccountsByCompanyId(int companyId)
		{
			var data = await _accountService.GetIndividualAccountsByCompanyId(companyId).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetIndividualAccounts")]
		public async Task<IActionResult> GetIndividualAccounts(int storeId)
		{
			var data = await _accountService.GetIndividualAccounts(storeId).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetFractionalApproximationDifferenceAccountByCompanyId")]
		public async Task<IActionResult> GetFractionalApproximationDifferenceAccountByCompanyId(int companyId)
		{
			var data = await _accountService.GetFractionalApproximationDifferenceAccount(companyId);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetFractionalApproximationDifferenceAccountByStoreId")]
		public async Task<IActionResult> GetFractionalApproximationDifferenceAccountByStoreId(int storeId)
		{
			var data = await _accountService.GetFractionalApproximationDifferenceAccountByStoreId(storeId);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllowedDiscountAccountByCompanyId")]
		public async Task<IActionResult> GetAllowedDiscountAccountByCompanyId(int companyId)
		{
			var data = await _accountService.GetAllowedDiscountAccount(companyId);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllowedDiscountAccountByStoreId")]
		public async Task<IActionResult> GetAllowedDiscountAccountByStoreId(int storeId)
		{
			var data = await _accountService.GetAllowedDiscountAccountByStoreId(storeId);
			return Ok(data);
		}

		//[HttpGet]
		//[Route("ReadIndividualAccounts")]
		//public IActionResult ReadAccounts(DataSourceLoadOptions loadOptions, int storeId)
		//{
		//	var data = DataSourceLoader.Load(_accountService.GetIndividualAccounts(storeId), loadOptions);
		//	return Ok(data);
		//}

		[HttpGet]
		[Route("ReadEntities")]
		public IActionResult ReadEntities(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_accountEntityService.GetEntities(), loadOptions);
			return Ok(data);
		}


		[HttpGet]
		[Route("GetAccountsTree")]
		public async Task<ActionResult<IEnumerable<AccountTreeDto>>> GetAccountsTree(int companyId)
		{
			var data = await _accountService.GetAccountsTree(companyId).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetMainAccountsByAccountCodeAutoComplete")]
		public async Task<ActionResult<IEnumerable<AccountAutoCompleteDto>>> GetMainAccountsByAccountCodeAutoComplete(int companyId, string accountCode)
		{
			var data = await _accountService.GetMainAccountsByAccountCode(companyId, accountCode);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetMainAccountsByAccountNameAutoComplete")]
		public async Task<ActionResult<IEnumerable<AccountAutoCompleteDto>>> GetMainAccountsByAccountNameAutoComplete(int companyId, string accountName)
		{
			var data = await _accountService.GetMainAccountsByAccountName(companyId, accountName);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetIndividualAccountsByAccountCodeAutoComplete")]
		public async Task<ActionResult<IEnumerable<AccountAutoCompleteDto>>> GetIndividualAccountsByAccountCodeAutoComplete(int storeId, string accountCode)
		{
			var data = await _accountService.GetIndividualAccountsByAccountCode(storeId, accountCode);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetIndividualAccountsByAccountNameAutoComplete")]
		public async Task<ActionResult<IEnumerable<AccountAutoCompleteDto>>> GetIndividualAccountsByAccountNameAutoComplete(int storeId, string accountName)
		{
			var data = await _accountService.GetIndividualAccountsByAccountName(storeId, accountName);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetNextAccountCode")]
		public async Task<ActionResult> GetNextAccountCode(int companyId, int mainAccountId, bool isMainAccount)
		{
			var data = await _accountService.GetNextAccountCode(companyId, mainAccountId, isMainAccount);
			return Ok(new { data = data });
		}

		[HttpGet]
		[Route("GetAllAccounts")]
		public async Task<ActionResult> GetAllAccounts()
		{
			var data = await _accountService.GetAllAccounts().ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetCompanyAccounts")]
		public async Task<ActionResult> GetCompanyAccounts(int companyId)
		{
			var data = await _accountService.GetCompanyAccounts(companyId).ToListAsync();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAccountEntityByAccountId")]
		public async Task<ActionResult> GetAccountEntityByAccountId(int accountId)
		{
			var data = await _accountEntityService.GetAccountEntityByAccountId(accountId);
			return Ok(data);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetAccountByAccountId(int id)
		{
			var countryDb = await _accountService.GetAccountByAccountId(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(countryDb.CompanyId, 0);
			return Ok(userCanLook ? countryDb : new AccountDto());
		}

		[HttpGet]
		[Route("GetAccountByAccountCode")]
		public async Task<ActionResult> GetAccountByAccountCode(int companyId, string accountCode)
		{
			var data = await _accountService.GetAccountByAccountCode(companyId, accountCode);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAccountTreeName")]
		public async Task<IActionResult> GetAccountTreeName(int accountId)
		{
			var data = await _accountService.GetAccountTreeName(accountId);
			return Ok(new { data = data });
		}

		[HttpGet]
		[Route("GetRequestData")]
		public async Task<IActionResult> GetRequestData(int requestId)
		{
			var data = await _handleApprovalRequestService.GetRequestData(requestId);
			if (data != null)
			{
				if (data.CanBeConverted<AccountDto>())
				{
					var convertedData = data.ConvertToType<AccountDto>();
					var userCanLook = await _httpContextAccessor.UserCanLook(convertedData?.CompanyId ?? 0, 0);
					return Ok(userCanLook ? convertedData : new AccountDto());
				}
			}
			return Ok(new AccountDto());
		}

		[HttpPost]
		[Route("Save")]
		public async Task<IActionResult> Save([FromBody] AccountDto model, int requestId)
		{
			ResponseDto response;
			try
			{
				var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.Account);
				if (hasApprove.HasApprove)
				{
					if (hasApprove.OnAdd && model.AccountId == 0)
					{
						response = await SendApproveRequest(model, null, requestId, 0, false);
					}
					else if (hasApprove.OnEdit && model.AccountId > 0)
					{
						var oldValue = await _accountService.GetAccountByAccountId(model.AccountId);
						response = await SendApproveRequest(model, oldValue, requestId, model.AccountId, false);
					}
					else
					{
						response = await SaveAccount(model);
					}
				}
				else
				{
					response = await SaveAccount(model);
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

		[HttpPost]
		[Route("SaveAccount")]
		public async Task<ResponseDto> SaveAccount(AccountDto model)
		{
			await _databaseTransaction.BeginTransaction();
			var response = await _accountEntityService.SaveAccount(model);
			if (response.Success)
			{
				await _databaseTransaction.Commit();
			}
			else
			{
				await _databaseTransaction.Rollback();
			}
			return response;
		}

		[HttpDelete]
		[Route("DeleteAccount")]
		public async Task<IActionResult> DeleteAccount(int accountId, int requestId)
		{
			ResponseDto response;
			try
			{
				var hasApprove = await _approvalSystemService.IsMenuHasApprove(MenuCodeData.Account);
				if (hasApprove.HasApprove && hasApprove.OnDelete)
				{
					var model = await _accountService.GetAccountByAccountId(accountId);
					response = await SendApproveRequest(null, model, requestId, accountId, true);
				}
				else
				{
					await _databaseTransaction.BeginTransaction();
					response = await _accountEntityService.DeleteAccount(accountId);
					if (response.Success)
					{
						await _databaseTransaction.Commit();
					}
					else
					{
						await _databaseTransaction.Rollback();
					}
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

		[HttpPost]
		[Route("SendApproveRequest")]
		public async Task<ResponseDto> SendApproveRequest([FromQuery] AccountDto? newModel, [FromQuery] AccountDto? oldModel, int requestId, int itemId, bool isDelete)
		{
			var changes = new List<RequestChangesDto>();
			var requestTypeId = isDelete ? ApproveRequestTypeData.Delete : (itemId == 0 ? ApproveRequestTypeData.Add : ApproveRequestTypeData.Edit);
			if (requestTypeId == ApproveRequestTypeData.Edit)
			{
				changes = _accountService.GetAccountRequestChanges(oldModel!, newModel!);
			}

			var request = new NewApproveRequestDto()
			{
				RequestId = requestId,
				MenuCode = MenuCodeData.Account,
				ApproveRequestTypeId = requestTypeId,
				ReferenceId = itemId,
				ReferenceCode = itemId.ToString(),
				OldValue = oldModel,
				NewValue = newModel,
				Changes = changes
			};
			await _databaseTransaction.BeginTransaction();

			var response = await _handleApprovalRequestService.SaveRequest(request);
			if (response.Success)
			{
				await _databaseTransaction.Commit();
			}
			else
			{
				await _databaseTransaction.Rollback();
			}
			return response;
		}

		[HttpPost]
		[Route("TEST")]
		public async Task<IActionResult> SaveFixedAssetAccount([FromBody] FixedAssetAccountDto model)
		{
			FixedAssetAccountReturnDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _accountService.SaveFixedAssetAccount(model);
				if (response.Result.Success)
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
