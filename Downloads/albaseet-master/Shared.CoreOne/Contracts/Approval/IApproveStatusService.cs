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
    public interface IApproveStatusService : IBaseService<ApproveStatus>
    {
        Task<ApproveStatusDto> GetApproveStatus(int id);
	    Task<bool> SaveApproveStatuses(List<int> approveSteps);
	    Task<bool> DeleteApproveStatuses(int approveId);
    }
}
