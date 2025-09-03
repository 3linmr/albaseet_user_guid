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
    public interface IApproveStepService :IBaseService<ApproveStep>
    {
        IQueryable<ApproveStepDto> GetAllApproveSteps();
        IQueryable<ApproveStepDto> GetApproveSteps(int approveId);
        Task<ApproveStepDto> GetApproveById(int id);
        Task<int> GetApproveId(int stepId);
		Task<ResponseDto> SaveApproveSteps(int approveId, List<ApproveStepDto> approves);
        Task<ResponseDto> DeleteApproveSteps(int approveId);
    }
}
