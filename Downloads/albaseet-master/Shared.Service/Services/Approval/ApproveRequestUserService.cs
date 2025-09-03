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
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Models.StaticData;
using LanguageData = Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Approval
{
	public class ApproveRequestUserService : BaseService<ApproveRequestUser>, IApproveRequestUserService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<ApproveRequestUserService> _localizer;
		private readonly IApproveStepService _approveStepService;
		private readonly IApproveStatusService _approveStatusService;

		public ApproveRequestUserService(IRepository<ApproveRequestUser> repository,IHttpContextAccessor httpContextAccessor,IStringLocalizer<ApproveRequestUserService>localizer,IApproveStepService approveStepService,IApproveStatusService approveStatusService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_approveStepService = approveStepService;
			_approveStatusService = approveStatusService;
		}

		public Task<List<ApproveRequestUserDto>> GetApproveRequestUser(int requestId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return 
				(from approveRequestUser in _repository.GetAll().Where(x=>x.RequestId == requestId)
				from approveStep in _approveStepService.GetAll().Where(x=>x.ApproveStepId == approveRequestUser.StepId)
				from approveStatus in _approveStatusService.GetAll().Where(x=>x.ApproveStatusId == approveRequestUser.StatusId)
					select new ApproveRequestUserDto
					{
						ApproveRequestUserId = approveRequestUser.ApproveRequestUserId,
						RequestId = approveRequestUser.RequestId,
						StepId = approveRequestUser.StepId,
						StepName = language == LanguageData.LanguageCode.Arabic ? approveStep.StepNameAr : approveStep.StepNameEn,
						UserName = approveRequestUser.UserName,
						StatusName = language == LanguageData.LanguageCode.Arabic ? approveStatus.StatusNameAr : approveStatus.StatusNameEn,
						Remarks = approveRequestUser.Remarks,
						EntryDate = approveRequestUser.CreatedAt.GetValueOrDefault().ToString("dd/MM/yyyy hh:mm:ss tt")
					}).ToListAsync();
		}

		public async Task<ResponseDto> SaveApproveRequestUser(int requestId, int stepId, int statusId, string? remarks)
		{
			var requestDb = new ApproveRequestUser()
			{
				ApproveRequestUserId = await GetNextId(),
				RequestId = requestId,
				StepId = stepId,
				StatusId = statusId,
				UserName = await _httpContextAccessor.GetUserName(),
				Remarks = remarks,
				UserNameCreated = await _httpContextAccessor.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				CreatedAt = DateHelper.GetDateTimeNow(),
				Hide = false
			};
			await _repository.Insert(requestDb);
			await _repository.SaveChanges();
			return new ResponseDto() { Id = requestId, Success = true };
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ApproveRequestUserId) + 1; } catch { id = 1; }
			return id;
		}

	}
}
