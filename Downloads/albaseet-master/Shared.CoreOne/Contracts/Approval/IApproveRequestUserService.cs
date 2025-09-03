using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Approval
{
	public interface IApproveRequestUserService : IBaseService<ApproveRequestUser>
	{
		Task<List<ApproveRequestUserDto>> GetApproveRequestUser(int requestId);
		Task<ResponseDto> SaveApproveRequestUser(int requestId, int stepId, int statusId, string? remarks);
	}
}
