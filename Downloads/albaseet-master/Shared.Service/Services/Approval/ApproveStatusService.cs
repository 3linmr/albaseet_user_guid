using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Approval
{
    public class ApproveStatusService : BaseService<ApproveStatus>, IApproveStatusService
    {
	    private readonly IHttpContextAccessor _httpContextAccessor;
	    private readonly IApproveStepService _approveStepService;

	    public ApproveStatusService(IRepository<ApproveStatus> repository,IHttpContextAccessor httpContextAccessor,IApproveStepService approveStepService) : base(repository)
	    {
		    _httpContextAccessor = httpContextAccessor;
		    _approveStepService = approveStepService;
	    }

	    public async Task<ApproveStatusDto> GetApproveStatus(int id)
	    {
		    var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return _repository.GetAll().Where(x => x.ApproveStatusId == id).Select(x => new ApproveStatusDto()
		    {
			    StatusId = x.ApproveStatusId,
				StepId = x.ApproveStepId,
			    StatusName = language == LanguageCode.Arabic ? x.StatusNameAr : x.StatusNameEn,
			}).FirstOrDefault() ?? new ApproveStatusDto();
	    }

	    public async Task<bool> SaveApproveStatuses(List<int> approveSteps)
        {
	        var nextStatusId = await GetNextId();
	        var statusList = new List<ApproveStatus>();

			foreach (var approveStep in approveSteps)
	        {
				var pending = new ApproveStatus()
				{
					ApproveStatusId = nextStatusId,
					ApproveStepId = approveStep,
					Pending = true,
					Rejected = false,
					Approved = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					StatusNameAr = "معلق",
					StatusNameEn = "Pending",
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					Hide = false
				};

				statusList.Add(pending);
				nextStatusId++;

				var approved = new ApproveStatus()
				{
					ApproveStatusId = nextStatusId,
					ApproveStepId = approveStep,
					Pending = false,
					Rejected = false,
					Approved = true,
					CreatedAt = DateHelper.GetDateTimeNow(),
					StatusNameAr = "موافقة",
					StatusNameEn = "Approved",
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					Hide = false
				};

				statusList.Add(approved);
				nextStatusId++;

				var rejected = new ApproveStatus()
				{
					ApproveStatusId = nextStatusId,
					ApproveStepId = approveStep,
					Pending = false,
					Rejected = true,
					Approved = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					StatusNameAr = "رفض",
					StatusNameEn = "Rejected",
					IpAddressCreated = _httpContextAccessor.GetIpAddress(),
					UserNameCreated = await _httpContextAccessor.GetUserName(),
					Hide = false
				};
				statusList.Add(rejected);
				nextStatusId++;
			}

			await _repository.InsertRange(statusList);
            await _repository.SaveChanges();
            return true;
        }


        public async Task<int> GetNextId()
        {
	        int id = 1;
	        try { id = await _repository.GetAll().MaxAsync(a => a.ApproveStatusId) + 1; } catch { id = 1; }
	        return id;
        }

		public async Task<bool> DeleteApproveStatuses(int approveId)
		{
			var statusList =
				(from approveStatus in _repository.GetAll()
				from approveStep in _approveStepService.GetAll().Where(x=>x.ApproveStepId == approveStatus.ApproveStepId && x.ApproveId == approveId)
				select approveStatus).ToList();
			_repository.RemoveRange(statusList);
			await _repository.SaveChanges();
			return true;
		}
    }
}
