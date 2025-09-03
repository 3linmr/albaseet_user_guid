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
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Menus;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Models.StaticData;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.Helper.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Approval
{
    public class ApproveService : BaseService<Approve>, IApproveService
    {
	    private readonly IHttpContextAccessor _httpContextAccessor;
	    private readonly IStringLocalizer<ApproveService> _localizer;
	    private readonly IMenuService _menuService;

	    public ApproveService(IRepository<Approve> repository,IHttpContextAccessor httpContextAccessor,IStringLocalizer<ApproveService> localizer,IMenuService menuService) : base(repository)
	    {
		    _httpContextAccessor = httpContextAccessor;
		    _localizer = localizer;
		    _menuService = menuService;
	    }

        public IQueryable<ApproveDto> GetAllApproves()
        {
	        var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
		        (
			        from approve in _repository.GetAll()
			        from menu in _menuService.GetAll().Where(x => x.MenuCode == approve.MenuCode)
			        select new ApproveDto()
			        {
				        ApproveId = approve.ApproveId,
						ApproveCode = approve.ApproveCode,
				        ApplicationId = approve.ApplicationId,
                        CompanyId = approve.CompanyId,
				        MenuCode = approve.MenuCode,
				        ApproveName = language == LanguageCode.Arabic ? approve.ApproveNameAr : approve.ApproveNameEn,
						ApproveNameAr = approve.ApproveNameAr,
				        ApproveNameEn = approve.ApproveNameEn,
						MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
						OnAdd = approve.OnAdd,
						OnEdit = approve.OnEdit,
						OnDelete = approve.OnDelete,
						IsStopped = approve.IsStopped,
						IsStoppedName = approve.IsStopped ? _localizer["IsStoppedNameTrue"].Value : _localizer["IsStoppedNameFalse"].Value,
						OnAddName = approve.OnAdd ? _localizer["True"].Value : _localizer["False"].Value,
						OnEditName = approve.OnEdit ? _localizer["True"].Value : _localizer["False"].Value,
						OnDeleteName = approve.OnDelete ? _localizer["True"].Value : _localizer["False"].Value,
					});
			return data;
        }

        public IQueryable<ApproveDto> GetCompanyApproves()
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return GetAllApproves().Where(x => x.CompanyId == companyId);
        }

        public async Task<ApproveDto> GetApproveById(int id)
        {
	        return await GetAllApproves().FirstOrDefaultAsync(x => x.ApproveId == id) ?? new ApproveDto();
        }

        public async Task<List<MenuCodeDropDownDto>> GetMenusForApprovesDropDown()
        {
	        var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var allMenus = await _menuService.GetAllMenus();
			var approves = await _repository.GetAll().Where(x => x.CompanyId == companyId).ToListAsync();
			var menus =
				(from menu in allMenus.Where(x => x.HasApprove)
				from approve in approves.Where(x => x.MenuCode == menu.MenuCode).DefaultIfEmpty()
				where approve == null
				select new MenuCodeDropDownDto
				{
					MenuCode = menu.MenuCode,
					MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn
				}).ToList();
			return menus;
        }

        public async Task<ResponseDto> SaveApprove(ApproveDto approve)
		{
			var approveExist = await IsApproveExist(approve.ApproveId,approve.CompanyId, approve.MenuCode,approve.ApproveNameAr, approve.ApproveNameEn);
			if (approveExist.Success)
			{
				return new ResponseDto() { Id = approveExist.Id, Success = false, Message = _localizer["ApproveAlreadyExist"] };
			}
			else
			{
				if (approve.ApproveId == 0)
				{
					return await CreateApprove(approve);
				}
				else
				{
					return await UpdateApprove(approve);
				}
			}
		}

		public ResponseDto ValidateApproveStatus(ApproveDefinitionDto approve)
		{
			if (approve.Approve != null)
			{
				if (!approve.Approve.OnAdd && !approve.Approve.OnEdit && !approve.Approve.OnDelete)
				{
					return new ResponseDto() { Id = approve.Approve.ApproveId, Success = false, Message = _localizer["OneShouldCheck"] };
				}

				if (approve.ApproveSteps != null)
				{
					if (approve.ApproveSteps.All(x=>x.IsDeleted))
					{
						return new ResponseDto() { Id = approve.Approve.ApproveId, Success = false, Message = _localizer["OneShouldWork"] };
					}
				}
			}
			return new ResponseDto(){Success = true};
;		}


		public async Task<ResponseDto> IsApproveExist(int id,int companyId,int menuCode, string? nameAr, string? nameEn)
		{
			var menuCodeExist = await _repository.GetAll().FirstOrDefaultAsync(x => x.MenuCode == menuCode && x.CompanyId == companyId && x.ApproveId != id);
			if (menuCodeExist != null)
			{
				return new ResponseDto() { Id = menuCodeExist.ApproveId, Success = true };
			}
			var approve = await _repository.GetAll().FirstOrDefaultAsync(x => (x.ApproveNameAr == nameAr || x.ApproveNameEn == nameEn || x.ApproveNameAr == nameEn || x.ApproveNameEn == nameAr) && x.ApproveId != id && x.CompanyId == companyId);
			if (approve != null)
			{
				return new ResponseDto() { Id = approve.ApproveId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ApproveId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.ApproveCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateApprove(ApproveDto approve)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var newApprove = new Approve()
			{
				ApproveId = await GetNextId(),
				ApproveCode = await GetNextCode(approve!.CompanyId),
				ApproveNameAr = approve?.ApproveNameAr?.Trim(),
				ApproveNameEn = approve?.ApproveNameEn?.Trim(),
				CompanyId = approve!.CompanyId,
				MenuCode = approve!.MenuCode,
				ApplicationId = ApplicationData.ApplicationId,
				OnAdd = approve.OnAdd,
				OnEdit = approve.OnEdit,
				OnDelete = approve.OnDelete,
				IsStopped = approve.IsStopped,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var approveValidator = await new ApproveValidator(_localizer).ValidateAsync(newApprove);
			var validationResult = approveValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newApprove);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newApprove.ApproveId, Success = true, Message = _localizer["NewApproveSuccessMessage", ((language == LanguageCode.Arabic ? newApprove.ApproveNameAr : newApprove.ApproveNameEn) ?? ""), newApprove.ApproveCode] };
			}
			else
			{
				return new ResponseDto() { Id = newApprove.ApproveId, Success = false, Message = approveValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateApprove(ApproveDto approve)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var approveDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ApproveId == approve.ApproveId);
			if (approveDb != null)
			{
				approveDb.ApproveNameAr = approve.ApproveNameAr?.Trim();
				approveDb.ApproveNameEn = approve.ApproveNameEn?.Trim();
				approveDb.CompanyId = approve!.CompanyId;
				approveDb.MenuCode = approve!.MenuCode;
				approveDb.ApplicationId = ApplicationData.ApplicationId;
				approveDb.OnAdd = approve.OnAdd;
				approveDb.OnEdit = approve.OnEdit;
				approveDb.OnDelete = approve.OnDelete;
				approveDb.IsStopped = approve.IsStopped;
				approveDb.ModifiedAt = DateHelper.GetDateTimeNow();
				approveDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				approveDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var approveValidator = await new ApproveValidator(_localizer).ValidateAsync(approveDb);
				var validationResult = approveValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(approveDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = approveDb.ApproveId, Success = true, Message = _localizer["UpdateApproveSuccessMessage", ((language == LanguageCode.Arabic ? approveDb.ApproveNameAr : approveDb.ApproveNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = approveValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoApproveFound"] };
		}
		public async Task<ResponseDto> DeleteApprove(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var approveDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ApproveId == id);
			if (approveDb != null)
			{
				_repository.Delete(approveDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteApproveSuccessMessage", ((language == LanguageCode.Arabic ? approveDb.ApproveNameAr : approveDb.ApproveNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoApproveFound"] };
		}
	}
}
