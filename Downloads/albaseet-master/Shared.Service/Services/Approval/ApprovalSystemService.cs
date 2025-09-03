using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Models.Domain.Approval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Models.StaticData;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;

namespace Shared.Service.Services.Approval
{
	public class ApprovalSystemService : IApprovalSystemService
	{
		private readonly IApproveService _approveService;
		private readonly IApproveStepService _approveStepService;
		private readonly IApproveStatusService _approveStatusService;
		private readonly IStringLocalizer<ApprovalSystemService> _localizer;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ICompanyService _companyService;

		public ApprovalSystemService(IApproveService approveService, IApproveStepService approveStepService, IApproveStatusService approveStatusService, IStringLocalizer<ApprovalSystemService> localizer, IHttpContextAccessor httpContextAccessor,ICompanyService companyService)
		{
			_approveService = approveService;
			_approveStepService = approveStepService;
			_approveStatusService = approveStatusService;
			_localizer = localizer;
			_httpContextAccessor = httpContextAccessor;
			_companyService = companyService;
		}

		public async Task<MenuApproveTypeDto> IsMenuHasApprove(int menuCode)
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            var isCompanyExist = await _companyService.IsCompanyExist(companyId);
            if (isCompanyExist)
            {
				var approve = await _approveService.GetAll().Where(x => x.MenuCode == menuCode && !x.IsStopped && x.CompanyId == companyId).Select(x => new MenuApproveTypeDto
				{
					HasApprove = true,
					MenuCode = x.MenuCode,
					OnAdd = x.OnAdd,
					OnEdit = x.OnEdit,
					OnDelete = x.OnDelete
				}).FirstOrDefaultAsync();
				return approve ?? new MenuApproveTypeDto();
			}
            else
            {
	            throw new Exception(_localizer["NoCompanyFound"]);
            }
		}

		public async Task<int> GetApproveIdByMenuCode(int menuCode)
		{
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return await _approveService.FindBy(x => x.MenuCode == menuCode && x.CompanyId == companyId).Select(s => s.ApproveId).FirstOrDefaultAsync();
		}

		public async Task<bool> IsApproveHasMultiSteps(int approveId)
		{
			return await _approveStepService.GetAll().AsQueryable().CountAsync(x => x.IsDeleted == false && x.ApproveId == approveId) > 1;
		}

		public async Task<bool> IsApproveStepHasMultiCount(int approveStepId)
		{
			var step = await _approveStepService.GetAll().FirstOrDefaultAsync(x => x.ApproveStepId == approveStepId);
			if (step != null)
			{
				return step.ApproveCount > 1;
			}
			return false;
		}

		public async Task<int> GetApproveStepCount(int approveStepId)
		{
			var step = await _approveStepService.GetAll().FirstOrDefaultAsync(x => x.ApproveStepId == approveStepId);
			return step?.ApproveCount ?? 0;
		}

		public async Task<ApprovalDto> GetNextStepStatus(int menuCode, int currentStep, bool approved, bool declined, byte stepCount)
		{
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var approveId = await _approveService.GetAll().Where(x => x.CompanyId == companyId && x.MenuCode == menuCode && !x.IsStopped).Select(s => s.ApproveId).FirstOrDefaultAsync();
			if (approveId > 0)
			{
				var model = new ApprovalDto();
				var steps = await _approveStepService.GetAll().Where(x => x.ApproveId == approveId && x.IsDeleted == false).OrderBy(o => o.ApproveOrder).ToListAsync();
				var statuses = await (
					from approveStep in _approveStepService.GetAll().AsQueryable().Where(x => x.ApproveId == approveId)
					from approveStatus in _approveStatusService.GetAll().AsQueryable()
						.Where(x => x.ApproveStepId == approveStep.ApproveStepId)
					select approveStatus).ToListAsync();
				var currentStepIndex = steps.FindIndex(x => x.ApproveStepId == currentStep);
				var isLastStep = steps.Where(x => x.ApproveStepId == currentStep).Select(s => s.IsLastStep).FirstOrDefault();
				var lastStepStatus = GetLastStepStatus(approveId, currentStep, steps, statuses);
				var stepHasMultiCount = false;

				var currentStepMultiCount = steps.Where(x => x.ApproveStepId == currentStep).Select(s => s.ApproveCount).FirstOrDefault();

				if (currentStepMultiCount > 1)
				{
					stepHasMultiCount = true;
					if (currentStepMultiCount > stepCount)
					{
						if (currentStepMultiCount - 1 > stepCount)
						{
							stepCount++;
							return KeepUnChanged(approveId, currentStep, stepCount, stepHasMultiCount, statuses);
						}
						else
						{
							stepCount++;
						}
					}
				}

				if (!approved && !declined)
				{
					model = KeepUnChanged(approveId, currentStep, stepCount, stepHasMultiCount, statuses);
				}

				if (approved)
				{
					if (isLastStep)
					{
						model = MoveToRealApprove(approveId, currentStep, lastStepStatus.StepId, lastStepStatus.StatusId, statuses, stepCount);
					}
					else
					{
						model = MoveToNextStep(approveId, currentStep, currentStepIndex, steps, statuses, stepCount);
					}
				}

				if (declined)
				{
					if (isLastStep)
					{
						model = MoveToRealDecline(approveId, currentStep, lastStepStatus.StepId, lastStepStatus.StatusId, statuses, stepCount);
					}
					else
					{
						model = DeclineCurrentStep(approveId, currentStep, lastStepStatus.StepId, lastStepStatus.StatusId, statuses, stepCount);
					}
				}
				return model;
			}

			throw new Exception("Approve Not Found");
		}

		public async Task<ApprovalDto> GetFirstStepStatus(int menuCode)
		{
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var isMenuHasApprove = await IsMenuHasApprove(menuCode);
			if (isMenuHasApprove.HasApprove)
			{
				var approveId = _approveService.GetAll().AsQueryable().Where(x => x.MenuCode == menuCode && x.CompanyId == companyId).Select(s => s.ApproveId).FirstOrDefault();
				var steps = _approveStepService.GetAll().AsQueryable().Where(x => x.ApproveId == approveId && x.IsDeleted == false).OrderBy(o => o.ApproveOrder).ToList();
				var statuses = (
					from approveStep in _approveStepService.GetAll().AsQueryable().Where(x => x.ApproveId == approveId)
					from approveStatus in _approveStatusService.GetAll().AsQueryable()
						.Where(x => x.ApproveStepId == approveStep.ApproveStepId)
					select approveStatus).ToList();

				if (steps.Any())
				{
					var currentStepCount = steps.Select(x => x.ApproveCount).FirstOrDefault();
					var stepId = steps.Select(x => x.ApproveStepId).FirstOrDefault();
					var stepName = language == LanguageCode.Arabic ? steps.FirstOrDefault(x => x.ApproveStepId == stepId)?.StepNameAr : steps.FirstOrDefault(x => x.ApproveStepId == stepId)?.StepNameEn;
					var statusId = statuses.Where(x => x.ApproveStepId == stepId && x.Pending).Select(x => x.ApproveStatusId).FirstOrDefault();
					return new ApprovalDto
					{
						ApproveId = approveId,
						CurrentStepId = stepId,
						CurrentStepName = stepName,
						CurrentStatusId = statusId,
						LastStepId = stepId,
						LastStatusId = statusId,
						IsApproved = null,
						CurrentStepCount = 0,
						//UsersApprove = ComposeUserApproves(stepId, statusId, stepId, statusId, null)
					};
				}
			}
			return new ApprovalDto() { IsApproved = true };
		}
		public LastStepStatus GetLastStepStatus(int approveId, int currentStep, List<ApproveStep> steps, List<ApproveStatus> statuses)
		{
			var stepsOrdered = steps.Where(x => x.ApproveId == approveId).OrderBy(x => x.ApproveOrder).ToList();
			var currentIndex = stepsOrdered.FindIndex(x => x.ApproveStepId == currentStep);
			if (currentIndex <= 0)
			{
				return new LastStepStatus()
				{
					StepId = currentStep,
					StatusId = statuses.Where(x => x.ApproveStepId == currentStep && x.Pending).Select(s => s.ApproveStatusId).FirstOrDefault()
				};
			}
			else
			{
				var previousStep = steps[stepsOrdered.FindIndex(x => x.ApproveStepId == currentStep) - 1].ApproveStepId;

				return new LastStepStatus()
				{
					StepId = previousStep,
					StatusId = statuses.Where(x => x.ApproveStepId == previousStep && x.Approved).Select(s => s.ApproveStatusId).FirstOrDefault()
				};
			}
		}

		public async Task<LastStepStatus> GetApprovedLastStepStatusByApproveId(int approveId)
		{
			var data =
				await (from approve in  _approveService.GetAll().Where(x=> x.ApproveId == approveId && !x.IsStopped)
				from approveStep in _approveStepService.GetAll().Where(x=>x.ApproveId == approve.ApproveId && x.IsLastStep && !x.IsDeleted)
				from status in _approveStatusService.GetAll().Where(x=>x.ApproveStepId == approveStep.ApproveStepId && x.Approved)
				select new LastStepStatus()
				{
					StepId = status.ApproveStepId,
					StatusId = status.ApproveStatusId,
				}).FirstOrDefaultAsync();
			return data ?? new LastStepStatus();
		}

		public async Task<LastStepStatus> GetDeclinedLastStepStatusByApproveId(int approveId)
		{
			var data =
				await(from approve in _approveService.GetAll().Where(x => x.ApproveId == approveId && !x.IsStopped)
					from approveStep in _approveStepService.GetAll().Where(x => x.ApproveId == approve.ApproveId && x.IsLastStep && !x.IsDeleted)
					from status in _approveStatusService.GetAll().Where(x => x.ApproveStepId == approveStep.ApproveStepId && x.Rejected)
					select new LastStepStatus()
					{
						StepId = status.ApproveStepId,
						StatusId = status.ApproveStatusId,
					}).FirstOrDefaultAsync();
			return data ?? new LastStepStatus();
		}

		public async Task<bool> IsLastStep(int approveId, int stepId)
		{
			var steps = await _approveStepService.GetAll().AsQueryable().Where(x => x.ApproveId == approveId && x.IsDeleted == false).OrderBy(o => o.ApproveOrder).ToListAsync();
			var isLastStep = steps.Where(x => x.ApproveStepId == stepId).Select(s => s.IsLastStep).FirstOrDefault();
			return isLastStep;
		}

		public async Task<List<ApproveStep>> GetNextSteps(int approveId, int stepId)
		{
			var stepOrder = await _approveStepService.FindBy(x => x.ApproveStepId == stepId).Select(s => s.ApproveOrder).FirstOrDefaultAsync();
			var data = await _approveStepService.GetAll().AsQueryable().Where(x => x.ApproveId == approveId && x.ApproveStepId != stepId && x.ApproveOrder > stepOrder && !x.IsDeleted).OrderBy(x => x.ApproveOrder).ToListAsync();
			return data;
		}

		public async Task<int> GetPendingStatusByStepId(int stepId)
		{
			return await _approveStatusService.GetAll().AsQueryable().Where(x => x.ApproveStepId == stepId && x.Pending).Select(s => s.ApproveStatusId).FirstOrDefaultAsync();
		}


		public List<UserStepDto> GetUserApprove(string usersApprove)
		{
			return JsonSerializer.Deserialize<List<UserStepDto>>(usersApprove) ?? new List<UserStepDto>();
		}

		private ApprovalDto KeepUnChanged(int approveId,int currentStep, byte stepCount, bool stepHasMultiCount, List<ApproveStatus> statuses)
		{
			var lastStatus = statuses.Where(x => x.ApproveStepId == currentStep && x.Pending).Select(s => s.ApproveStatusId).FirstOrDefault();
			var currentStatus = statuses.Where(x => x.ApproveStepId == currentStep && x.Pending).Select(s => s.ApproveStatusId).FirstOrDefault();
			var approvedStatus = statuses.Where(x => x.ApproveStepId == currentStep && x.Approved).Select(s => s.ApproveStatusId).FirstOrDefault();
			var currentApproveStatus = statuses.Where(x => x.ApproveStepId == currentStep && x.Approved).Select(s => s.ApproveStatusId).FirstOrDefault();
			var model = new ApprovalDto
			{
				ApproveId = approveId,
				LastStepId = currentStep,
				LastStatusId = lastStatus,
				CurrentStepId = currentStep,
				CurrentStatusId = currentStatus,
				Message = stepHasMultiCount ? _localizer["CounterMessage"] : _localizer["PendingMessage"],
				Success = false,
				CurrentStepCount = stepCount,
				//UsersApprove = ComposeUserApproves(currentStep, currentApproveStatus, currentStep, lastStatus, currentUsers),
				IsApproved = null,
				CurrentUserStatusId = approvedStatus
			};
			return model;
		}

		private ApprovalDto MoveToRealApprove(int approveId, int currentStep, int lastStep, int lastStatus, List<ApproveStatus> statuses, byte stepCount)
		{
			var currentStatus = statuses.Where(x => x.ApproveStepId == currentStep && x.Approved).Select(s => s.ApproveStatusId).FirstOrDefault();
			var model = new ApprovalDto
			{
				ApproveId = approveId,
				LastStepId = lastStep,
				LastStatusId = lastStatus,
				CurrentStepId = currentStep,
				CurrentStatusId = currentStatus,
				Message = _localizer["LastStepApproved"],
				Success = true,
				CurrentStepCount = stepCount,
				//UsersApprove = ComposeUserApproves(currentStep, currentStatus, lastStep, lastStatus, currentUsers),
				IsApproved = true,
				CurrentUserStatusId = currentStatus
			};
			return model;
		}
		private ApprovalDto MoveToNextStep(int approveId, int currentStep, int currentStepIndex, List<ApproveStep> steps, List<ApproveStatus> statuses, byte stepCount)
		{
			var nextStep = steps[currentStepIndex + 1].ApproveStepId;
			var statusAr = steps.Where(s => s.ApproveStepId == nextStep).Select(s => s.StepNameAr).FirstOrDefault();
			var statusEn = steps.Where(s => s.ApproveStepId == nextStep).Select(s => s.StepNameEn).FirstOrDefault();
			var status = ((_httpContextAccessor.GetSelectedLanguageId() == (int)LanguageData.Languages.Arabic) ? statusAr : statusEn) ?? "";
			var currentStatus =  statuses.Where(x => x.ApproveStepId == nextStep && x.Pending).Select(s => s.ApproveStatusId).FirstOrDefault();
			var lastStatus = statuses.Where(x => x.ApproveStepId == currentStep && x.Approved).Select(s => s.ApproveStatusId).FirstOrDefault();

			var model = new ApprovalDto
			{
				ApproveId = approveId,
				LastStepId = currentStep,
				LastStatusId = lastStatus,
				CurrentStepId = nextStep,
				CurrentStatusId = currentStatus,
				Message = _localizer["CurrentApprove", status],
				Success = true,
				//CurrentStepCount = stepCount,
				CurrentStepCount = 0,
				//UsersApprove = ComposeUserApproves(nextStep, currentStatus, currentStep, lastStatus, currentUsers),
				IsApproved = null,
				CurrentUserStatusId = lastStatus
			};
			return model;
		}

		private ApprovalDto MoveToRealDecline(int approveId, int currentStep, int lastStep, int lastStatus, List<ApproveStatus> statuses, byte stepCount)
		{
			var currentStatus = statuses.Where(x => x.ApproveStepId == currentStep && x.Rejected).Select(s => s.ApproveStatusId).FirstOrDefault();
			var model = new ApprovalDto
			{
				ApproveId = approveId,
				LastStepId = lastStep,
				LastStatusId = lastStatus,
				CurrentStepId = currentStep,
				CurrentStatusId = currentStatus,
				Message = _localizer["LastStepDeclined"],
				Success = true,
				CurrentStepCount = stepCount,
				//UsersApprove = ComposeUserApproves(currentStep, currentStatus, lastStep, lastStatus, currentUsers),
				IsApproved = false,
				CurrentUserStatusId = currentStatus
			};
			return model;
		}
		private ApprovalDto DeclineCurrentStep(int approveId, int currentStep, int lastStep, int lastStatus, List<ApproveStatus> statuses, byte stepCount)
		{
			var currentStatus = statuses.Where(x => x.ApproveStepId == currentStep && x.Rejected).Select(s => s.ApproveStatusId).FirstOrDefault();
			var model = new ApprovalDto
			{
				ApproveId = approveId,
				LastStepId = lastStep,
				LastStatusId = lastStatus,
				CurrentStepId = currentStep,
				CurrentStatusId = currentStatus,
				Message = _localizer["RequestDeclined"],
				Success = true,
				CurrentStepCount = stepCount,
				//UsersApprove = ComposeUserApproves(currentStep, currentStatus, lastStep,lastStatus,currentUsers),
				IsApproved = false,
				CurrentUserStatusId = currentStatus
			};
			return model;
		}

		private async Task<string> ComposeUserApproves(int currentStep,int currentStatus, int lastStep, int lastStatus, string? currentUsers)
		{
			if (currentUsers == null)
			{
				var modelDb = new List<UserStepDto>();
				var userName = await _httpContextAccessor.GetUserName();
				var userStepDto = new UserStepDto()
				{
					UserName = userName,
					StepId = currentStep,
					StatusId = currentStatus
				};
				modelDb.Add(userStepDto);
				return JsonSerializer.Serialize(modelDb);
			}
			else
			{
				var modelDb = GetUserApprove(currentUsers);
				var userName = await _httpContextAccessor.GetUserName();

				if (currentStep != lastStep)
				{
					var lastStepModel = new UserStepDto()
					{
						UserName = userName,
						StepId = lastStep,
						StatusId = lastStatus
					};
					modelDb.Add(lastStepModel);
				}

				var currentStepModel = new UserStepDto()
				{
					UserName = userName,
					StepId = currentStep,
					StatusId = currentStatus
				};
				modelDb.Add(currentStepModel);
				var userSteps = modelDb.DistinctBy(x => new { x.StepId,x.StatusId,x.UserName }).ToList();
				return JsonSerializer.Serialize(userSteps);
			}


		}
	}
}
