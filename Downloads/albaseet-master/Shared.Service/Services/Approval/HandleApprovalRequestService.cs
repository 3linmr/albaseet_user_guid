using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Models.Dtos;

namespace Shared.Service.Services.Approval
{
	public class HandleApprovalRequestService : IHandleApprovalRequestService
	{
		private readonly IApprovalSystemService _approvalSystemService;
		private readonly IApproveRequestService _approveRequestService;
		private readonly IApproveRequestUserService _approveRequestUserService;
		private readonly IStringLocalizer<HandleApprovalRequestService> _localizer;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IApproveRequestDetailService _approveRequestDetailService;

		public HandleApprovalRequestService(IApprovalSystemService approvalSystemService, IApproveRequestService approveRequestService, IApproveRequestUserService approveRequestUserService, IStringLocalizer<HandleApprovalRequestService> localizer, IHttpContextAccessor httpContextAccessor, IApproveRequestDetailService approveRequestDetailService)
		{
			_approvalSystemService = approvalSystemService;
			_approveRequestService = approveRequestService;
			_approveRequestUserService = approveRequestUserService;
			_localizer = localizer;
			_httpContextAccessor = httpContextAccessor;
			_approveRequestDetailService = approveRequestDetailService;
		}

		public async Task<object?> GetRequestData(int requestId)
		{
			var request = await _approveRequestService.GetAll().FirstOrDefaultAsync(x => x.RequestId == requestId);
			var requestTypeId = request?.ApproveRequestTypeId;
			if (requestTypeId == ApproveRequestTypeData.Delete)
			{
				return await _approveRequestDetailService.GetRequestOldData(requestId);
			}
			else
			{
				return await _approveRequestDetailService.GetRequestNewData(requestId);
			}
		}

		public async Task<ResponseDto> SaveRequest(NewApproveRequestDto request)
		{
			var isExist = await _approveRequestService.IsDuplicateRequest(request.MenuCode, request.ReferenceId,request.RequestId);
			if (!isExist)
			{
				if (request.RequestId == 0)
				{
					return await CreateNewRequest(request);
				}
				else
				{
					return await UpdateRequest(request);
				}
			}
			else
			{
				return new ResponseDto() { Success = false, Id = request.RequestId, Message = _localizer["RequestAlreadyExistOnDocument"] };
			}
		}

		public async Task<ResponseDto> CreateNewRequest(NewApproveRequestDto request)
		{
			var approveData = await _approvalSystemService.GetFirstStepStatus(request.MenuCode);
			var approveRequest = new ApproveRequestDto()
			{
				MenuCode = request.MenuCode,
				ApproveRequestTypeId = request.ApproveRequestTypeId,
				ApproveId = approveData.ApproveId,
				CurrentStepId = approveData.CurrentStepId,
				CurrentStatusId = approveData.CurrentStatusId,
				LastStatusId = approveData.LastStatusId,
				LastStepId = approveData.LastStepId,
				CurrentStepCount = approveData.CurrentStepCount,
				IsApproved = approveData.IsApproved,
				FromUserName = await _httpContextAccessor.GetUserName(),
				RequestDate = DateTime.Today,
				ReferenceId = request.ReferenceId,
				ReferenceCode = request.ReferenceCode,
				RequestId = request.RequestId
			};

			//result.IdList[0] contains the request code
			var result = await _approveRequestService.InsertNewRequest(approveRequest);

			var requestDetail = new ApproveRequestDetailDto()
			{
				ApproveRequestId = result.Id,
				ApproveRequestDetailId = 0,
				OldValue = request.OldValue != null ? JsonConvert.SerializeObject(request.OldValue) : null,
				NewValue = request.NewValue != null ? JsonConvert.SerializeObject(request.NewValue) : null,
				Changes = request.Changes != null ? JsonConvert.SerializeObject(request.Changes) : null
			};

			await _approveRequestDetailService.SaveApproveRequestDetail(requestDetail);
			return new ResponseDto() { Success = true, Id = result.Id, Message = _localizer["NewRequest", result.IdList[0], (approveData.CurrentStepName ?? "")] };
		}

		public async Task<ResponseDto> UpdateRequest(NewApproveRequestDto request)
		{
			var requestCode = (await _approveRequestService.GetUserApproveRequestById(request.RequestId))?.RequestCode ?? 0;

			var requestDetail = new ApproveRequestDetailDto()
			{
				ApproveRequestId = request.RequestId,
				ApproveRequestDetailId = request.RequestId,
				OldValue = request.OldValue != null ? JsonConvert.SerializeObject(request.OldValue) : null,
				NewValue = request.NewValue != null ? JsonConvert.SerializeObject(request.NewValue) : null,
				Changes = request.Changes != null ? JsonConvert.SerializeObject(request.Changes) : null,
			};
			await _approveRequestDetailService.SaveApproveRequestDetail(requestDetail);

			return new ResponseDto() { Success = true, Id = request.RequestId, Message = _localizer["UpdateRequest", requestCode] };
		}

		public async Task<ResponseDto> DeleteRequest(int requestId)
		{
			var requestCode = (await _approveRequestService.GetUserApproveRequestById(requestId))?.RequestCode ?? 0;

			await _approveRequestDetailService.DeleteApproveRequestDetail(requestId);
			await _approveRequestService.DeleteApproveRequest(requestId);
			return new ResponseDto() { Success = true, Id = requestId, Message = _localizer["DeleteRequest", requestCode] };
		}

		public async Task<ApproveResponseDto> HandleApprovalRequest(HandleApproveRequestDto request)
		{
			var requestDb = await _approveRequestService.GetUserApproveRequestById(request.RequestId);
			if (requestDb != null)
			{
				return await MoveRequestToNext(requestDb, request.Approved, request.Remarks);
			}
			return new ApproveResponseDto() { RequestId = request.RequestId, Success = false, Message = _localizer["SomethingWentWrong"] };
		}

		public async Task<ApproveResponseDto> MoveRequestToNext(ApproveRequestDto request, bool userApproved, string? remarks)
		{
			var nextStepStatus = await _approvalSystemService.GetNextStepStatus(request.MenuCode, request.CurrentStepId, userApproved, !userApproved, request.CurrentStepCount);
			var requestData = request.ApproveRequestTypeId == ApproveRequestTypeData.Delete ? await _approveRequestDetailService.GetRequestOldData(request.RequestId) : await _approveRequestDetailService.GetRequestNewData(request.RequestId);
			if (nextStepStatus.IsApproved == true)
			{
				await _approveRequestService.UpdateApproveRequest(nextStepStatus, request.RequestId);
				await _approveRequestUserService.SaveApproveRequestUser(request.RequestId, request.CurrentStepId, nextStepStatus.CurrentUserStatusId, remarks);
				return new ApproveResponseDto() { RequestId = request.RequestId, Approved = nextStepStatus.IsApproved, UserRequest = requestData, Message = _localizer["RequestFinallyApproved"], MenuCode = request.MenuCode, ApproveRequestTypeId = request.ApproveRequestTypeId, Success = true };
			}
			else
			{
				await _approveRequestService.UpdateApproveRequest(nextStepStatus, request.RequestId);
				await _approveRequestUserService.SaveApproveRequestUser(request.RequestId, request.CurrentStepId, nextStepStatus.CurrentUserStatusId, remarks);
				return new ApproveResponseDto() { RequestId = request.RequestId, Approved = nextStepStatus.IsApproved, UserRequest = requestData, Message = nextStepStatus.IsApproved == false ? _localizer["RequestFinallyDeclined"] : _localizer["RequestToNextStep"], MenuCode = request.MenuCode, ApproveRequestTypeId = request.ApproveRequestTypeId, Success = true };
			}
		}
	}
}
