using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Approval
{
	public interface IApproveRequestService : IBaseService<ApproveRequest>
	{
		IQueryable<ApproveRequestDto> GetAllApproveRequests();
		Task<IQueryable<ApproveRequestDto>> GetUserApproveRequests();
		Task<IQueryable<ApproveRequestDto>> GetApproveHistory();
		Task<RequestHistoryDto> GetCurrentUserApproveRemarks(int requestId);
		Task<IQueryable<ApproveRequestDto>> ReadPendingUserRequests();
		Task<IQueryable<ApproveRequestDto>> ReadUserRequestsHistory();
		Task<ApproveRequestDto?> GetUserApproveRequestById(int id);
		Task<ResponseDto> InsertNewRequest(ApproveRequestDto request);
		Task<bool> UpdateApproveRequest(ApprovalDto model,int requestId);
		Task<bool> IsDuplicateRequest(short menuCode, int referenceId,int requestId);
		Task<bool> IsAnyPendingRequest(int approveId);
		Task<bool> DeleteApproveRequest(int requestId);
	}
}
