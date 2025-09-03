using Shared.CoreOne.Contracts.Approval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Identity;
using Shared.Helper.Models.Dtos;
using Shared.CoreOne.Models.Domain.Approval;

namespace Shared.Service.Services.Approval
{
	public class ApproveDefinitionService : IApproveDefinitionService
	{
		private readonly IApproveService _approveService;
		private readonly IApproveStepService _approveStepService;
		private readonly IApproveStatusService _approveStatusService;
		private readonly IStringLocalizer<ApproveDefinitionService> _localizer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApproveDefinitionService(IApproveService approveService, IApproveStepService approveStepService,
			IApproveStatusService approveStatusService, IStringLocalizer<ApproveDefinitionService> localizer,IHttpContextAccessor httpContextAccessor)
		{
			_approveService = approveService;
			_approveStepService = approveStepService;
			_approveStatusService = approveStatusService;
			_localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
        }
		public async Task<ApproveDefinitionDto> GetApprove(int approveId)
		{
			var approve = await _approveService.GetApproveById(approveId);
			var approveSteps = await _approveStepService.GetApproveSteps(approveId).OrderBy(s=>s.ApproveOrder).ToListAsync();
			return new ApproveDefinitionDto()
			{
				Approve = approve,
				ApproveSteps = approveSteps
			};
		}

		public async Task<List<ApproveTreeDto>> GetApproveTree(int companyId)
		{
			var approves = await 
				(from approve in _approveService.GetAllApproves().Where(x => x.CompanyId == companyId && !x.IsStopped)
				select new ApproveTreeDto()
				{
					Id = approve.ApproveId * -1,
					Text = approve.ApproveName,
					Order = approve.ApproveId,
					IsMain = true,
					IsStep = false,
					MainId = null,
					Selected = false
				}).ToListAsync();

			var approveSteps = await
				(from approve in _approveService.GetAllApproves().Where(x => x.CompanyId == companyId && !x.IsStopped)
				from approveStep in _approveStepService.GetAllApproveSteps().Where(x => x.ApproveId == approve.ApproveId && !x.IsDeleted)
				select new ApproveTreeDto()
				{
					Id = approveStep.ApproveStepId,
					Text = approveStep.StepName,
					Order = approveStep.ApproveOrder,
					IsMain = false,
					IsStep = true,
					MainId = approve.ApproveId *-1,
					Selected = false
				}).ToListAsync();

            var finalData = approves.Concat(approveSteps).ToList().OrderBy(x=>x.Text).ToList();
			return finalData;
		}

		public async Task<ResponseDto> SaveApprove(ApproveDefinitionDto approve)
		{
            if (approve.Approve != null)
            {
                approve.Approve.CompanyId = _httpContextAccessor.GetCurrentUserCompany();

                if (approve.Approve != null)
                {
                    var valid = _approveService.ValidateApproveStatus(approve);
                    if (!valid.Success)
                    {
                        return valid;
                    }
                    else
                    {
                        var approveResult = await _approveService.SaveApprove(approve.Approve);
                        if (approveResult.Success)
                        {
                            if (approve.ApproveSteps != null)
                            {
                                var stepsResult =
                                    await _approveStepService.SaveApproveSteps(approveResult.Id, approve.ApproveSteps);
                                if (stepsResult.Success)
                                {
                                    if (stepsResult.IdList != null)
                                    {
                                        var statusResult =
                                            await _approveStatusService.SaveApproveStatuses(stepsResult.IdList);
                                        if (statusResult)
                                        {
                                            return approveResult;
                                        }
                                        else
                                        {
                                            return new ResponseDto()
                                                { Id = 0, Success = false, Message = _localizer["SomethingWentWrong"] };
                                        }
                                    }
                                    else
                                    {
                                        return new ResponseDto()
                                            { Id = 0, Success = false, Message = _localizer["SomethingWentWrong"] };
                                    }
                                }
                                else
                                {
                                    return stepsResult;
                                }
                            }
                            else
                            {
                                return new ResponseDto()
                                    { Id = 0, Success = false, Message = _localizer["NothingThere"] };
                            }
                        }
                        else
                        {
                            return approveResult;
                        }
                    }
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NothingThere"] };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NothingThere"] };
        }

        public async Task<ResponseDto> DeleteApprove(int approveId)
		{
			await _approveStatusService.DeleteApproveStatuses(approveId);
			await _approveStepService.DeleteApproveSteps(approveId);
			return await _approveService.DeleteApprove(approveId);
		}
	}
}
