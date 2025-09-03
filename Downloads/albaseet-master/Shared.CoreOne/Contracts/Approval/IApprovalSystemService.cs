using Shared.CoreOne.Models.Domain.Approval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;

namespace Shared.CoreOne.Contracts.Approval
{
    public interface IApprovalSystemService
    {
        Task<MenuApproveTypeDto> IsMenuHasApprove(int menuCode);
        Task<int> GetApproveIdByMenuCode(int menuCode);
        Task<bool> IsApproveHasMultiSteps(int approveId);
        Task<bool> IsApproveStepHasMultiCount(int approveStepId);
        Task<int> GetApproveStepCount(int approveStepId);
        Task<ApprovalDto> GetNextStepStatus(int menuCode, int currentStep, bool approved, bool declined, byte stepCount);
        Task<ApprovalDto> GetFirstStepStatus(int menuCode);
        LastStepStatus GetLastStepStatus(int approveId,int currentStep, List<ApproveStep> steps,List<ApproveStatus> statuses);
        Task<LastStepStatus> GetApprovedLastStepStatusByApproveId(int approveId);
        Task<LastStepStatus> GetDeclinedLastStepStatusByApproveId(int approveId);
        Task<bool> IsLastStep(int approveId, int stepId);
        Task<List<ApproveStep>> GetNextSteps(int approveId, int stepId);
        Task<int> GetPendingStatusByStepId(int stepId);
		List<UserStepDto> GetUserApprove(string currentStepCount);
    }
}
