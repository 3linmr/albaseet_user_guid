using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Approval
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ApprovesController : ControllerBase
	{
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IStringLocalizer<HandelException> _exceptionLocalizer;
		private readonly IApproveService _approveService;
		private readonly IApproveDefinitionService _approveDefinitionService;
		private readonly IApproveRequestService _approveRequestService;
		private readonly IStringLocalizer<ApprovesController> _localizer;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ApprovesController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exceptionLocalizer, IApproveService approveService, IApproveDefinitionService approveDefinitionService, IApproveRequestService approveRequestService, IStringLocalizer<ApprovesController> localizer, IHttpContextAccessor httpContextAccessor)
		{
			_databaseTransaction = databaseTransaction;
			_exceptionLocalizer = exceptionLocalizer;
			_approveService = approveService;
			_approveDefinitionService = approveDefinitionService;
			_approveRequestService = approveRequestService;
			_localizer = localizer;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("ReadApproves")]
		public IActionResult ReadApproves(DataSourceLoadOptions loadOptions)
		{
			var data = DataSourceLoader.Load(_approveService.GetCompanyApproves(), loadOptions);
			return Ok(data);
		}

		[HttpGet]
		[Route("GetAllApproves")]
		public IActionResult GetAllApproves()
		{
			var data = _approveService.GetAllApproves();
			return Ok(data);
		}

		[HttpGet]
		[Route("GetCompanyApproves")]
		public IActionResult GetCompanyApproves()
		{
			var data = _approveService.GetCompanyApproves();
			return Ok(data);
		}
		
		[HttpGet]
		[Route("GetApproveTree")]
		public async Task<IActionResult> GetApproveTree(int companyId)
		{
			var data = await _approveDefinitionService.GetApproveTree(companyId);
			return Ok(data);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetApprove(int id)
		{
			var stateDb = await _approveDefinitionService.GetApprove(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(stateDb?.Approve?.CompanyId ?? 0, 0);
			return Ok(userCanLook ? stateDb : new ApproveDto());
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody] ApproveDefinitionDto approve)
		{
			ResponseDto response;
			try
			{
				if (approve!.Approve!.ApproveId > 0)
				{
					var isAnyPendingRequest = await _approveRequestService.IsAnyPendingRequest(approve.Approve.ApproveId);
					if (isAnyPendingRequest)
					{
						return Ok(new ResponseDto() { Success = false, Message = _localizer["HasPendingRequests"] });
					}
				}
				await _databaseTransaction.BeginTransaction();
				response = await _approveDefinitionService.SaveApprove(approve);
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
		public async Task<IActionResult> DeleteApprove(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _approveDefinitionService.DeleteApprove(id);
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
