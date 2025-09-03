using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Approval
{
	public interface IApproveDefinitionService
	{
		Task<ApproveDefinitionDto> GetApprove(int approveId);
		Task<List<ApproveTreeDto>> GetApproveTree(int companyId);
		Task<ResponseDto> SaveApprove(ApproveDefinitionDto approve);
		Task<ResponseDto> DeleteApprove(int approveId);
	}
}
