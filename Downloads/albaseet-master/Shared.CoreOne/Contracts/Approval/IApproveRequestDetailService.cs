using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;

namespace Shared.CoreOne.Contracts.Approval
{
	public interface IApproveRequestDetailService : IBaseService<ApproveRequestDetail>
	{
		Task<object?> GetRequestOldData(int requestId);
		Task<object?> GetRequestNewData(int requestId);
		Task<List<RequestChangesDto>?> GetRequestChanges(int requestId);
		Task<bool> SaveApproveRequestDetail(ApproveRequestDetailDto requestDetail);
		Task<bool> DeleteApproveRequestDetail(int requestId);
	}
}
