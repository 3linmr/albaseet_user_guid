using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;

namespace Shared.API.Controllers.Approval
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class RequestsController : ControllerBase
	{
		private readonly IApproveRequestService _approveRequestService;
		private readonly IDatabaseTransaction _databaseTransaction;
		private readonly IHandleApprovalRequestService _handleApprovalRequestService;
		private readonly IStringLocalizer<HandelException> _exLocalizer;

		public RequestsController(IApproveRequestService approveRequestService,IDatabaseTransaction databaseTransaction,IHandleApprovalRequestService handleApprovalRequestService, IStringLocalizer<HandelException> exLocalizer)
		{
			_approveRequestService = approveRequestService;
			_databaseTransaction = databaseTransaction;
			_handleApprovalRequestService = handleApprovalRequestService;
			_exLocalizer = exLocalizer;
		}

		[HttpGet]
		[Route("ReadPendingUserRequests")]
		public async Task<IActionResult> ReadPendingUserRequests(DataSourceLoadOptions loadOptions)
		{
			return Ok(await DataSourceLoader.LoadAsync(await _approveRequestService.ReadPendingUserRequests(), loadOptions));
		}

		[HttpGet]
		[Route("ReadUserRequestsHistory")]
		public async Task<IActionResult> ReadUserRequestsHistory(DataSourceLoadOptions loadOptions)
		{
			return Ok(await DataSourceLoader.LoadAsync(await _approveRequestService.ReadUserRequestsHistory(), loadOptions));
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteRequest(int id)
		{
			ResponseDto response;
			try
			{
				await _databaseTransaction.BeginTransaction();
				response = await _handleApprovalRequestService.DeleteRequest(id);
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
