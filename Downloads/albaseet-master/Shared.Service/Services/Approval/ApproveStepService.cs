using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;

namespace Shared.Service.Services.Approval
{
    public class ApproveStepService : BaseService<ApproveStep>, IApproveStepService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IApproveService _approveService;
        private readonly IStringLocalizer<ApproveStepService> _localizer;

        public ApproveStepService(IRepository<ApproveStep> repository, IHttpContextAccessor httpContextAccessor, IApproveService approveService, IStringLocalizer<ApproveStepService> localizer) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _approveService = approveService;
            _localizer = localizer;
        }

        public IQueryable<ApproveStepDto> GetAllApproveSteps()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var data =
            (
                from approveStep in _repository.GetAll()
                from approve in _approveService.GetAll().Where(x => x.ApproveId == approveStep.ApproveId)
                select new ApproveStepDto()
                {
                    ApproveStepId = approveStep.ApproveStepId,
                    StepName = language == LanguageData.LanguageCode.Arabic ? approveStep.StepNameAr : approveStep.StepNameEn,
					StepNameAr = approveStep.StepNameAr,
                    StepNameEn = approveStep.StepNameEn,
                    ApproveId = approve.ApproveId,
                    ApproveName = language == LanguageData.LanguageCode.Arabic ? approve.ApproveNameAr : approve.ApproveNameEn,
                    IsDeleted = approveStep.IsDeleted,
                    IsLastStep = approveStep.IsLastStep,
                    AllCountShouldApprove = approveStep.AllCountShouldApprove,
                    ApproveCount = approveStep.ApproveCount,
                    ApproveOrder = approveStep.ApproveOrder
                });
            return data;
        }

        public IQueryable<ApproveStepDto> GetApproveSteps(int approveId)
        {
            return GetAllApproveSteps().Where(x => x.ApproveId == approveId);
        }

        public async Task<ApproveStepDto> GetApproveById(int id)
        {
            return await GetAllApproveSteps().FirstOrDefaultAsync(x => x.ApproveStepId == id) ?? new ApproveStepDto();
        }

        public async Task<int> GetApproveId(int stepId)
        {
            return await _repository.GetAll().Where(x => x.ApproveStepId == stepId).Select(s => s.ApproveId).FirstOrDefaultAsync();
        }

        public async Task<ResponseDto> SaveApproveSteps(int approveId, List<ApproveStepDto> approves)
        {
            var newApproves = new List<ApproveStepDto>();
            var updatedApproves = new List<ApproveStepDto>();
            var loopCount = 1;

            if (approves.Any())
            {
                var lastStep = approves.LastOrDefault(x => !x.IsDeleted);
                if (lastStep != null)
                {
                    lastStep.IsLastStep = true;
                    var others = approves.Where(x =>
                        x.StepNameAr != lastStep.StepNameAr && x.StepNameEn != lastStep.StepNameEn).ToList();
                    others.ForEach(x => x.IsLastStep = false);

                }

                foreach (var approve in approves)
                {
                    var exist = await IsApproveStepExist(approveId, approve.ApproveStepId, approve.StepNameAr, approve.StepNameEn);
                    if (!exist.Success)
                    {
                        approve.ApproveOrder = (byte)loopCount;

                        if (approve.ApproveStepId == 0)
                        {
                            newApproves.Add(approve);
                        }
                        else
                        {
                            updatedApproves.Add(approve);
                        }
                        loopCount++;
                    }
                    else
                    {
                        return new ResponseDto() { Id =exist.Id, Success = false, Message = _localizer["ApproveStepAlreadyExist"] };
                    }
                }

                if (newApproves.Count > 0 && updatedApproves.Count == 0)
                {
                    return await CreateApproveSteps(approveId, newApproves);
                }
                if (newApproves.Count == 0 && updatedApproves.Count > 0)
                {
                    return await UpdateApproveSteps(updatedApproves);
                }
                if (newApproves.Count > 0 && updatedApproves.Count > 0)
                {
                    await UpdateApproveSteps(updatedApproves);
                    return await CreateApproveSteps(approveId, newApproves);
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoApproveStepFound"] };
        }

        public async Task<ResponseDto> CreateApproveSteps(int approveId, List<ApproveStepDto> approves)
        {
            var approveList = new List<ApproveStep>();
            var stepsList = new List<int>();
            var approveStepId = await GetNextId();
            foreach (var approve in approves)
            {
                var approveModel = new ApproveStep()
                {
                    ApproveStepId = approveStepId,
                    ApproveId = approveId,
                    StepNameAr = approve.StepNameAr?.Trim(),
                    StepNameEn = approve.StepNameEn?.Trim(),
                    IsDeleted = approve.IsDeleted,
                    IsLastStep = approve.IsLastStep,
                    AllCountShouldApprove = approve.AllCountShouldApprove,
                    ApproveCount = approve.ApproveCount,
                    ApproveOrder = approve.ApproveOrder,
                    CreatedAt = DateHelper.GetDateTimeNow(),
                    UserNameCreated = await _httpContextAccessor!.GetUserName(),
                    IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                    Hide = false,
                };

                var approveStepValidator = await new ApproveStepValidator(_localizer).ValidateAsync(approveModel);
                var validationResult = approveStepValidator.IsValid;
                if (validationResult)
                {
                    approveList.Add(approveModel);
                    stepsList.Add(approveModel.ApproveStepId);
                    approveStepId++;
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = approveStepValidator.ToString("~") };
                }
            }

            if (approveList.Any())
            {
                await _repository.InsertRange(approveList);
                await _repository.SaveChanges();
                return new ResponseDto() { IdList = stepsList, Success = true, Message = _localizer["NewApproveStepSuccessMessage"] };
            }
            return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoApproveStepFound"] };
        }
        public async Task<ResponseDto> UpdateApproveSteps(List<ApproveStepDto> approves)
        {
            var approveList = new List<ApproveStep>();
            var stepsList = new List<int>();

            foreach (var approve in approves)
            {
                var approveModel = new ApproveStep()
                {
                    ApproveStepId = approve.ApproveStepId,
                    ApproveId = approve.ApproveId,
                    StepNameAr = approve.StepNameAr?.Trim(),
                    StepNameEn = approve.StepNameEn?.Trim(),
                    IsDeleted = approve.IsDeleted,
                    AllCountShouldApprove = approve.AllCountShouldApprove,
                    ApproveCount = approve.ApproveCount,
                    ApproveOrder = approve.ApproveOrder,
                    IsLastStep = approve.IsLastStep,
                    ModifiedAt = DateHelper.GetDateTimeNow(),
                    UserNameModified = await _httpContextAccessor!.GetUserName(),
                    IpAddressModified = _httpContextAccessor?.GetIpAddress(),
                    Hide = false
                };

                var approveStepValidator = await new ApproveStepValidator(_localizer).ValidateAsync(approveModel);
                var validationResult = approveStepValidator.IsValid;
                if (validationResult)
                {
                    approveList.Add(approveModel);
                    stepsList.Add(approveModel.ApproveStepId);
                }
            }

            if (approveList.Any())
            {
                _repository.UpdateRange(approveList);
                await _repository.SaveChanges();
                return new ResponseDto() { IdList = stepsList, Success = true, Message = _localizer["UpdateApproveStepSuccessMessage"] };
            }
            return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoApproveStepFound"] };
        }


        public async Task<ResponseDto> IsApproveStepExist(int approveId, int stepId, string? nameAr, string? nameEn)
        {
            var approveStep = await _repository.GetAll().FirstOrDefaultAsync(x => (x.StepNameAr == nameAr || x.StepNameEn == nameEn || x.StepNameAr == nameEn || x.StepNameEn == nameAr) && x.ApproveStepId == stepId && x.ApproveId != approveId);
            if (approveStep != null)
            {
                return new ResponseDto() { Id = approveStep.ApproveStepId, Success = true, Message = _localizer["ApproveStepAlreadyExist"] };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }
        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.ApproveStepId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> DeleteApproveSteps(int approveId)
        {
            var steps = _repository.GetAll().Where(x => x.ApproveId == approveId).ToList();
            _repository.RemoveRange(steps);
            await _repository.SaveChanges();
            return new ResponseDto() { Id = 0, Success = true, Message = _localizer["NoApproveStepFound"] };
        }
    }
}
