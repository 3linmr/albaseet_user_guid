using Compound.API.Models;
using Compound.CoreOne.Contracts.Approval;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;

namespace Compound.API.Controllers.Approval
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApproveRequestsController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exceptionLocalizer;
        private readonly IApproveRequestService _approveRequestService;
        private readonly IHandleApprovalRequestService _handleApprovalRequestService;
        private readonly IApproveRequestUserService _approveRequestUserService;
        private readonly IRedirectApproveRequestService _redirectApproveRequestService;
        private readonly IStringLocalizer<ApproveRequestsController> _requestLocalizer;
        private readonly IApproveRequestDetailService _approveRequestDetailService;
        private readonly IRedirectDeclinedRequestService _redirectDeclinedRequestService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public ApproveRequestsController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exceptionLocalizer, IApproveRequestService approveRequestService, IHandleApprovalRequestService handleApprovalRequestService, IApproveRequestUserService approveRequestUserService, IRedirectApproveRequestService redirectApproveRequestService, IStringLocalizer<ApproveRequestsController> requestLocalizer, IApproveRequestDetailService approveRequestDetailService, IRedirectDeclinedRequestService redirectDeclinedRequestService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exceptionLocalizer = exceptionLocalizer;
            _approveRequestService = approveRequestService;
            _handleApprovalRequestService = handleApprovalRequestService;
            _approveRequestUserService = approveRequestUserService;
            _redirectApproveRequestService = redirectApproveRequestService;
            _requestLocalizer = requestLocalizer;
            _approveRequestDetailService = approveRequestDetailService;
            _redirectDeclinedRequestService = redirectDeclinedRequestService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadUserApproveRequests")]
        public async Task<IActionResult> ReadUserApproveRequests(DataSourceLoadOptions loadOptions)
        {
            return Ok(await DataSourceLoader.LoadAsync(await _approveRequestService.GetUserApproveRequests(), loadOptions));
        }

        [HttpGet]
        [Route("ReadApproveHistory")]
        public async Task<IActionResult> ReadApproveHistory(DataSourceLoadOptions loadOptions)
        {
	        return Ok(await DataSourceLoader.LoadAsync(await _approveRequestService.GetApproveHistory(), loadOptions));
        }

        [HttpGet]
        [Route("GetCurrentUserApproveRemarks")]
        public async Task<IActionResult> GetCurrentUserApproveRemarks(int requestId)
        {
	        var remarks = await _approveRequestService.GetCurrentUserApproveRemarks(requestId);
	        return Ok(remarks);
        }

		[HttpGet("{id:int}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            var data = await _approveRequestService.GetUserApproveRequestById(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(data?.CompanyId ?? 0, 0);
            return Ok(userCanLook ? data : new ApproveRequestDto());
        }

        [HttpGet]
        [Route("GetUsersApproveFromRequest")]
        public async Task<IActionResult> GetUsersApproveFromRequest(int requestId)
        {
            var usersApprove = await _approveRequestUserService.GetApproveRequestUser(requestId);
            return Ok(usersApprove);
        }

        [HttpGet]
        [Route("GetRequestChanges")]
        public async Task<IActionResult> GetRequestChanges(int requestId)
        {
            var data = await _approveRequestDetailService.GetRequestChanges(requestId);
            return Ok(data);
        }

        [HttpPost("HandleApprovalRequest")]
        public async Task<IActionResult> HandleApprovalRequest([FromBody] HandleApproveRequestDto request)
        {
            ResponseDto response;
            try
            {
                await _databaseTransaction.BeginTransaction();
                var result = await _handleApprovalRequestService.HandleApprovalRequest(request);
                if (result.Approved == true)
                {
                    response = await _redirectApproveRequestService.RedirectApproveRequest(result);
                }
                else if (result.Approved == false)
                {
                    response = await _redirectDeclinedRequestService.RedirectDeclinedRequest(result);
                }
                else
                {
                    response = new ResponseDto() { Success = result.Success, Message = result.Message };
                }

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

        [HttpPost("HandleApprovalRequests")]
        public async Task<IActionResult> HandleApprovalRequests([FromBody] HandleApproveRequestsDto requests)
        {
            var response = new ResponseDto();
            try
            {
                await _databaseTransaction.BeginTransaction();
                if (requests.Requests != null)
                {
                    foreach (var request in requests.Requests)
                    {
                        var result = await _handleApprovalRequestService.HandleApprovalRequest(new HandleApproveRequestDto() { RequestId = request, Approved = requests.Approved, Remarks = requests.Remarks });
                        if (result.Approved == true)
                        {
                            var approveResult = await _redirectApproveRequestService.RedirectApproveRequest(result);
                            if (!approveResult.Success)
                            {
                                await _databaseTransaction.Rollback();
                                return Ok(approveResult);
                            }
                        }
                    }
                    await _databaseTransaction.Commit();
                    response.Success = true;
                    response.Message = requests.Approved ? _requestLocalizer["AllRequestsApproved"] : _requestLocalizer["AllRequestsDeclined"];
                }
                else
                {
                    response.Success = false;
                    response.Message = _requestLocalizer["NothingSelected"];
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
