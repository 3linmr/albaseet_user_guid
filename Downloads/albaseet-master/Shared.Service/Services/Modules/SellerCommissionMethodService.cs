using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class SellerCommissionMethodService : BaseService<SellerCommissionMethod>, ISellerCommissionMethodService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<SellerCommissionMethodService> _localizer;
		private readonly ISellerCommissionTypeService _sellerCommissionTypeService;

		public SellerCommissionMethodService(IRepository<SellerCommissionMethod> repository, IHttpContextAccessor httpContextAccessor, IStringLocalizer<SellerCommissionMethodService> localizer, ISellerCommissionTypeService sellerCommissionTypeService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_sellerCommissionTypeService = sellerCommissionTypeService;
		}

		public IQueryable<SellerCommissionMethodDto> GetAllSellerCommissionMethods()
		{
			return
				from sellerCommission in _repository.GetAll()
				from commissionType in _sellerCommissionTypeService.GetSellerCommissionTypes().Where(x=>x.SellerCommissionTypeId == sellerCommission.SellerCommissionTypeId)
				select new SellerCommissionMethodDto()
				{
					SellerCommissionMethodId = sellerCommission.SellerCommissionMethodId,
					SellerCommissionMethodCode = sellerCommission.SellerCommissionMethodCode,
					SellerCommissionMethodNameAr = sellerCommission.SellerCommissionMethodNameAr,
					SellerCommissionMethodNameEn = sellerCommission.SellerCommissionMethodNameEn,
					SellerCommissionTypeId = sellerCommission.SellerCommissionTypeId,
					SellerCommissionTypeName = commissionType.SellerCommissionTypeName,
					IsActive = sellerCommission.IsActive,
					InActiveReasons = sellerCommission.InActiveReasons,
					CompanyId = sellerCommission.CompanyId
                };
		}

        public IQueryable<SellerCommissionMethodDto> GetCompanySellerCommissionMethods()
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return GetAllSellerCommissionMethods().Where(x => x.CompanyId == companyId);
        }

        public IQueryable<SellerCommissionMethodDropDownDto> GetAllSellerCommissionMethodsDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetCompanySellerCommissionMethods().Select(x => new SellerCommissionMethodDropDownDto()
			{
				SellerCommissionMethodId = x.SellerCommissionMethodId,
				SellerCommissionTypeId = x.SellerCommissionTypeId,
				SellerCommissionMethodName = language == LanguageCode.Arabic ? x.SellerCommissionMethodNameAr : x.SellerCommissionMethodNameEn,
				IsActive = x.IsActive
			}).OrderBy(x => x.SellerCommissionMethodName);
		}

		public IQueryable<SellerCommissionMethodDropDownDto> GetActiveSellerCommissionMethodsDropDown()
		{
			return GetAllSellerCommissionMethodsDropDown().Where(x => x.IsActive);
		}

		public Task<SellerCommissionMethodDto?> GetSellerCommissionMethodById(int id)
		{
			return GetAllSellerCommissionMethods().FirstOrDefaultAsync(x => x.SellerCommissionMethodId == id);
		}

		public async Task<ResponseDto> SaveSellerCommissionMethod(SellerCommissionMethodDto sellerCommissionMethod)
		{
			var sellerCommissionMethodExist = await IsSellerCommissionMethodExist(sellerCommissionMethod.SellerCommissionMethodId, sellerCommissionMethod.SellerCommissionMethodNameAr, sellerCommissionMethod.SellerCommissionMethodNameEn);
			if (sellerCommissionMethodExist.Success)
			{
				return new ResponseDto() { Id = sellerCommissionMethodExist.Id, Success = false, Message = _localizer["SellerCommissionMethodAlreadyExist"] };
			}
			else
			{
				if (sellerCommissionMethod.SellerCommissionMethodId == 0)
				{
					return await CreateSellerCommissionMethod(sellerCommissionMethod);
				}
				else
				{
					return await UpdateSellerCommissionMethod(sellerCommissionMethod);
				}
			}
		}

		public async Task<ResponseDto> IsSellerCommissionMethodExist(int id, string? nameAr, string? nameEn)
		{
            var companyId = _httpContextAccessor.GetCurrentUserCompany(); 
            var sellerCommissionMethod = await _repository.GetAll().FirstOrDefaultAsync(x => (x.SellerCommissionMethodNameAr == nameAr || x.SellerCommissionMethodNameEn == nameEn || x.SellerCommissionMethodNameAr == nameEn || x.SellerCommissionMethodNameEn == nameAr) && x.SellerCommissionMethodId != id && x.CompanyId == companyId);
			if (sellerCommissionMethod != null)
			{
				return new ResponseDto() { Id = sellerCommissionMethod.SellerCommissionMethodId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<short> GetNextId()
		{
			var id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.SellerCommissionMethodId) + 1; } catch { id = 1; }
			return (short)id;
		}

		public async Task<short> GetNextCode(int companyId)
		{
			var id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.SellerCommissionMethodCode) + 1; } catch { id = 1; }
			return (short)id;
		}

		public async Task<ResponseDto> CreateSellerCommissionMethod(SellerCommissionMethodDto sellerCommissionMethod)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var newSellerCommissionMethod = new SellerCommissionMethod()
			{
				SellerCommissionMethodId = await GetNextId(),
				SellerCommissionMethodCode = await GetNextCode(companyId),
				SellerCommissionMethodNameAr = sellerCommissionMethod?.SellerCommissionMethodNameAr?.Trim(),
				SellerCommissionMethodNameEn = sellerCommissionMethod?.SellerCommissionMethodNameEn?.Trim(),
				SellerCommissionTypeId = sellerCommissionMethod!.SellerCommissionTypeId,
				IsActive = sellerCommissionMethod!.IsActive,
				InActiveReasons = sellerCommissionMethod.InActiveReasons,
				CompanyId = companyId,
				Hide = false,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
			};

			var sellerCommissionMethodValidator = await new SellerCommissionMethodValidator(_localizer).ValidateAsync(newSellerCommissionMethod);
			var validationResult = sellerCommissionMethodValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newSellerCommissionMethod);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newSellerCommissionMethod.SellerCommissionMethodId, Success = true, Message = _localizer["NewSellerCommissionMethodSuccessMessage", ((language == LanguageCode.Arabic ? newSellerCommissionMethod.SellerCommissionMethodNameAr : newSellerCommissionMethod.SellerCommissionMethodNameEn) ?? ""), newSellerCommissionMethod.SellerCommissionMethodCode] };
			}
			else
			{
				return new ResponseDto() { Id = newSellerCommissionMethod.SellerCommissionMethodId, Success = false, Message = sellerCommissionMethodValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateSellerCommissionMethod(SellerCommissionMethodDto sellerCommissionMethod)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var sellerCommissionMethodDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.SellerCommissionMethodId == sellerCommissionMethod.SellerCommissionMethodId);
			if (sellerCommissionMethodDb != null)
			{
				sellerCommissionMethodDb.SellerCommissionMethodNameAr = sellerCommissionMethod.SellerCommissionMethodNameAr?.Trim();
				sellerCommissionMethodDb.SellerCommissionMethodNameEn = sellerCommissionMethod.SellerCommissionMethodNameEn?.Trim();
				sellerCommissionMethodDb.SellerCommissionTypeId = sellerCommissionMethod!.SellerCommissionTypeId;
				sellerCommissionMethodDb.IsActive = sellerCommissionMethod!.IsActive;
				sellerCommissionMethodDb.InActiveReasons = sellerCommissionMethod.InActiveReasons;
				sellerCommissionMethodDb.ModifiedAt = DateHelper.GetDateTimeNow();
				sellerCommissionMethodDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				sellerCommissionMethodDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var sellerCommissionMethodValidator = await new SellerCommissionMethodValidator(_localizer).ValidateAsync(sellerCommissionMethodDb);
				var validationResult = sellerCommissionMethodValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(sellerCommissionMethodDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = sellerCommissionMethodDb.SellerCommissionMethodId, Success = true, Message = _localizer["UpdateSellerCommissionMethodSuccessMessage", ((language == LanguageCode.Arabic ? sellerCommissionMethodDb.SellerCommissionMethodNameAr : sellerCommissionMethodDb.SellerCommissionMethodNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = sellerCommissionMethodValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoSellerCommissionMethodFound"] };
		}

		public async Task<ResponseDto> DeleteSellerCommissionMethod(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var sellerCommissionMethodDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.SellerCommissionMethodId == id);
			if (sellerCommissionMethodDb != null)
			{
				_repository.Delete(sellerCommissionMethodDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteSellerCommissionMethodSuccessMessage", ((language == LanguageCode.Arabic ? sellerCommissionMethodDb.SellerCommissionMethodNameAr : sellerCommissionMethodDb.SellerCommissionMethodNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoSellerCommissionMethodFound"] };
		}
	}
}
