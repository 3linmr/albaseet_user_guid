using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Approval;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Approval
{
	public class ApproveRequestService : BaseService<ApproveRequest>,IApproveRequestService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IApproveService _approveService;
		private readonly IApproveStepService _approveStepService;
		private readonly IApproveStatusService _approveStatusService;
		private readonly IMenuService _menuService;
		private readonly IApproveRequestUserService _approveRequestUserService;
		private readonly IApproveRequestTypeService _approveRequestTypeService;
		private readonly IApprovalSystemService _approvalSystemService;

		public ApproveRequestService(IRepository<ApproveRequest> repository,IHttpContextAccessor httpContextAccessor,IApproveService approveService,IApproveStepService approveStepService,IApproveStatusService approveStatusService,IMenuService menuService,IApproveRequestUserService approveRequestUserService,IApproveRequestTypeService approveRequestTypeService,IApprovalSystemService approvalSystemService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_approveService = approveService;
			_approveStepService = approveStepService;
			_approveStatusService = approveStatusService;
			_menuService = menuService;
			_approveRequestUserService = approveRequestUserService;
			_approveRequestTypeService = approveRequestTypeService;
			_approvalSystemService = approvalSystemService;
		}
	
		public IQueryable<ApproveRequestDto> GetAllApproveRequests()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from request in _repository.GetAll()
				from approveRequestType in _approveRequestTypeService.GetAll().Where(x=>x.ApproveRequestTypeId == request.ApproveRequestTypeId)
				from approve in _approveService.GetAll().Where(x=>x.ApproveId == request.ApproveId)
				from currentStep in _approveStepService.GetAll().Where(x=>x.ApproveId == approve.ApproveId && x.ApproveStepId == request.CurrentStepId)
				from currentStatus in _approveStatusService.GetAll().Where(x=>x.ApproveStepId == currentStep.ApproveStepId && x.ApproveStatusId == request.CurrentStatusId)
				from lastStep in _approveStepService.GetAll().Where(x => x.ApproveId == approve.ApproveId && x.ApproveStepId == request.LastStepId)
				from lastStatus in _approveStatusService.GetAll().Where(x => x.ApproveStepId == lastStep.ApproveStepId && x.ApproveStatusId == request.LastStatusId)
				from menu in _menuService.GetAll().Where(x=>x.MenuCode == request.MenuCode && x.HasApprove)
				select new ApproveRequestDto
				{
					RequestId = request.RequestId,
					RequestCode = request.RequestCode,
					RequestDate = request.RequestDate,
					MenuCode = request.MenuCode,
					FromUserName = request.FromUserName,
					ReferenceId = request.ReferenceId,
					ReferenceCode = request.ReferenceCode,
					ApproveId = request.ApproveId,
					ApproveName = language == LanguageCode.Arabic ? approve.ApproveNameAr : approve.ApproveNameEn,
					CurrentStepId = request.CurrentStepId,
					CurrentStepName = language == LanguageCode.Arabic ? currentStep.StepNameAr : currentStep.StepNameEn,
					CurrentStatusId = request.CurrentStatusId,
					CurrentStatusName = language == LanguageCode.Arabic ? currentStatus.StatusNameAr : currentStatus.StatusNameEn,
					LastStepId = request.LastStepId,
					LastStepName = language == LanguageCode.Arabic ? lastStep.StepNameAr : lastStep.StepNameEn ,
					LastStatusId = request.LastStatusId,
					LastStatusName = language == LanguageCode.Arabic ? lastStatus.StatusNameAr : lastStatus.StatusNameEn,
					IsApproved = request.IsApproved,
					CurrentStepCount = request.CurrentStepCount,
					MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
					MenuUrl = menu.MenuUrl,
					CompanyId = request.CompanyId,
					BranchId = request.BranchId,
					StoreId = request.StoreId,
                    ApproveRequestTypeId = request.ApproveRequestTypeId,
					ApproveRequestTypeName = language == LanguageCode.Arabic ? approveRequestType.ApproveRequestTypeNameAr : approveRequestType.ApproveRequestTypeNameEn,
					UserCanEdit = (request.CurrentStepId == request.LastStepId && request.CurrentStatusId == request.LastStatusId && request.ApproveRequestTypeId != ApproveRequestTypeData.Delete) && (request.MenuCode != MenuCodeData.InternalReceiveItems),
					UserCanDelete = (request.CurrentStepId == request.LastStepId && request.CurrentStatusId == request.LastStatusId) && (request.MenuCode != MenuCodeData.InternalReceiveItems)
				};
			return data;
		}
		public async Task<IQueryable<ApproveRequestDto>> GetUserApproveRequests()
        {
            var userCompany = _httpContextAccessor.GetCurrentUserCompany();
			var userName = await _httpContextAccessor.GetUserName();
			var userSteps = await _httpContextAccessor.GetUserSteps();
			var data =
				from request in GetAllApproveRequests().Where(x => userSteps.Contains(x.CurrentStepId) && x.IsApproved == null && x.CompanyId == userCompany)
				from requestUser in _approveRequestUserService.GetAll().Where(x => x.StepId == request.CurrentStepId && x.UserName == userName && x.StatusId == request.CurrentStatusId && x.RequestId == request.RequestId).DefaultIfEmpty()
				where requestUser == null
				select request;
			return data;
		}

		public async Task<IQueryable<ApproveRequestDto>> GetApproveHistory()
		{
			var userCompany = _httpContextAccessor.GetCurrentUserCompany();
			var userName = await _httpContextAccessor.GetUserName();
			var userSteps = await _httpContextAccessor.GetUserSteps();
			var data =
				from request in GetAllApproveRequests().Where(x => userSteps.Contains(x.CurrentStepId) && x.CompanyId == userCompany)
				from requestUser in _approveRequestUserService.GetAll().Where(x => x.UserName == userName && x.RequestId == request.RequestId).DefaultIfEmpty()
				where requestUser != null
				select request;
			return data;
		}

		public async Task<RequestHistoryDto> GetCurrentUserApproveRemarks(int requestId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var userCompany = _httpContextAccessor.GetCurrentUserCompany();
			var userName = await _httpContextAccessor.GetUserName();
			var data =
				(from request in GetAllApproveRequests().Where(x => x.CompanyId == userCompany && x.RequestId == requestId)
				from requestUser in _approveRequestUserService.GetAll().Where(x => x.UserName == userName && x.RequestId == request.RequestId).DefaultIfEmpty()
				from approveStatus in _approveStatusService.GetAll().Where(x=>x.ApproveStatusId == requestUser.StatusId).DefaultIfEmpty()
				 where requestUser != null
				select new RequestHistoryDto
				{
					Remarks = requestUser.Remarks,
					ApproveStatus = language == LanguageCode.Arabic ? approveStatus.StatusNameAr : approveStatus.StatusNameEn,
				} ).FirstOrDefault();
			return data ?? new RequestHistoryDto();
		}

		public async Task<IQueryable<ApproveRequestDto>> ReadPendingUserRequests()
		{
            var userName = await _httpContextAccessor.GetUserName();
            var userCompany = _httpContextAccessor.GetCurrentUserCompany();
            var userStore = _httpContextAccessor.GetCurrentUserStore();
			var menus = new List<int>(new int[] { MenuCodeData.Item, MenuCodeData.Account, MenuCodeData.CostCenter });
            return
                from request in GetAllApproveRequests().Where(x => x.FromUserName == userName && x.IsApproved == null)
                where ((menus.Contains(request.MenuCode)) && request.CompanyId == userCompany)
                      || ((!menus.Contains(request.MenuCode)) && request.StoreId == userStore)
                select request;
        }

		public async Task<IQueryable<ApproveRequestDto>> ReadUserRequestsHistory()
		{
			var userName = await _httpContextAccessor.GetUserName();
			var userCompany = _httpContextAccessor.GetCurrentUserCompany();
			var userStore = _httpContextAccessor.GetCurrentUserStore();
			var menus = new List<int>(new int[] { MenuCodeData.Item, MenuCodeData.Account, MenuCodeData.CostCenter });
			return
				from request in GetAllApproveRequests().Where(x => x.FromUserName == userName && x.IsApproved != null)
				where ((menus.Contains(request.MenuCode)) && request.CompanyId == userCompany)
				      || ((!menus.Contains(request.MenuCode)) && request.StoreId == userStore)
				select request;
		}

		public Task<ApproveRequestDto?> GetUserApproveRequestById(int id)
		{
			return GetAllApproveRequests().FirstOrDefaultAsync(x => x.RequestId == id);
		}

		public async Task<ResponseDto> InsertNewRequest(ApproveRequestDto request)
		{
			var newRequest = new ApproveRequest()
			{
				RequestId = await GetNextId(),
				RequestCode = await GetNextCode(_httpContextAccessor!.GetCurrentUserCompany()),
				ApproveId = request.ApproveId,
				ApproveRequestTypeId = request.ApproveRequestTypeId,
				CurrentStepId = request.CurrentStepId,
				CurrentStatusId = request.CurrentStatusId,
				LastStepId = request.LastStepId,
				LastStatusId = request.LastStatusId,
				IsApproved = request.IsApproved,
				FromUserName = request.FromUserName,
				MenuCode = request.MenuCode,
				ReferenceCode = request.ReferenceCode,
				ReferenceId = request.ReferenceId,
				RequestDate = request.RequestDate,
				CurrentStepCount = request.CurrentStepCount,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				CompanyId = _httpContextAccessor!.GetCurrentUserCompany(),
                BranchId = _httpContextAccessor!.GetCurrentUserBranch(),
                StoreId = _httpContextAccessor!.GetCurrentUserStore(),
                Hide = false
			};
			await _repository.Insert(newRequest);
			await _repository.SaveChanges();
			return new ResponseDto() { Id = newRequest.RequestId, IdList = [newRequest.RequestCode], Success = true };
		}

		public async Task<bool> UpdateApproveRequest(ApprovalDto model, int requestId)
		{
			var requestDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.RequestId == requestId);
			if (requestDb != null)
			{
				requestDb.CurrentStepId = model.CurrentStepId;
				requestDb.CurrentStatusId = model.CurrentStatusId;
				requestDb.LastStepId = model.LastStepId;
				requestDb.LastStatusId = model.LastStatusId;
				requestDb.IsApproved = model.IsApproved;
				requestDb.CurrentStepCount = model.CurrentStepCount;
				return true;
			}
			return false;
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.RequestId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.RequestCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<bool> IsDuplicateRequest(short menuCode, int referenceId, int requestId)
		{
			return await _repository.GetAll().AnyAsync(x => x.MenuCode == menuCode && x.ReferenceId == referenceId && x.IsApproved == null && x.RequestId != requestId && x.ApproveRequestTypeId != ApproveRequestTypeData.Add);
		}

		public async Task<bool> IsAnyPendingRequest(int approveId)
		{
			//var approved = await _approvalSystemService.GetApprovedLastStepStatusByApproveId(approveId);
			//var declined = await _approvalSystemService.GetDeclinedLastStepStatusByApproveId(approveId);
			//return await _repository.GetAll().AnyAsync(x =>
			//	x.ApproveId == approveId && x.CurrentStepId != approved.StepId &&
			//	x.CurrentStatusId != approved.StatusId && x.CurrentStatusId != declined.StatusId);

			return await _repository.GetAll().AnyAsync(x => x.ApproveId == approveId &&  x.IsApproved == null);
		}

		public async Task<bool> DeleteApproveRequest(int requestId)
		{
			var data = await _repository.GetAll().FirstOrDefaultAsync(x => x.RequestId == requestId);
			if (data != null)
			{
				_repository.Delete(data);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}
	}
}
