using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Approval
{
	public interface IHandleApprovalRequestService
	{
		Task<object?> GetRequestData(int requestId);
		Task<ResponseDto> SaveRequest(NewApproveRequestDto request);
		Task<ResponseDto> DeleteRequest(int requestId);
		Task<ApproveResponseDto> HandleApprovalRequest(HandleApproveRequestDto request);
	}
}
