using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Models.Dtos;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Service.Validators;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Logic;
using static Shared.Helper.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Basics
{
    public class ShippingStatusService: BaseService<ShippingStatus>, IShippingStatusService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<ShippingStatusService> _localizer;
        private readonly IMenuService _menuService;

        public ShippingStatusService(IRepository<ShippingStatus> repository, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ShippingStatusService> localizer, IMenuService menuService) : base(repository) 
        {
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
            _menuService = menuService;
        }

        public IQueryable<ShippingStatusDto> GetAllShippingStatuses()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var data = from shippingStatus in _repository.GetAll()
                       from menu in _menuService.GetAll().Where(x => x.MenuCode == shippingStatus.MenuCode)
                       select new ShippingStatusDto
                       {
                           ShippingStatusId = shippingStatus.ShippingStatusId,
                           ShippingStatusCode = shippingStatus.ShippingStatusCode,
                           ShippingStatusNameAr = shippingStatus.ShippingStatusNameAr,
                           ShippingStatusNameEn = shippingStatus.ShippingStatusNameEn,
                           CompanyId = shippingStatus.CompanyId,
                           MenuCode = shippingStatus.MenuCode,
                           MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
                           StatusOrder = shippingStatus.StatusOrder,
                           IsActive = shippingStatus.IsActive,
                           InActiveReasons = shippingStatus.InActiveReasons
					   };

            return data;
        }

        public IQueryable<ShippingStatusDto> GetCompanyShippingStatuses()
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return GetAllShippingStatuses().Where(x => x.CompanyId == companyId);
        }

        public IQueryable<ShippingStatusDropDownDto> GetAllShippingStatusesDropDown(int menuCode)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data = GetCompanyShippingStatuses().Where(x => x.MenuCode == menuCode && x.IsActive).OrderBy(x=>x.StatusOrder).Select(x => new ShippingStatusDropDownDto
            {
                ShippingStatusId = x.ShippingStatusId,
                ShippingStatusName = language == LanguageCode.Arabic ? x.ShippingStatusNameAr : x.ShippingStatusNameEn
            });

            return data;
        }

        public async Task<ShippingStatusDto?> GetShippingStatusById(int id)
        {
            return await GetAllShippingStatuses().FirstOrDefaultAsync(x => x.ShippingStatusId == id);
        }

        public async Task<ResponseDto> SaveShippingStatus(ShippingStatusDto shippingStatus)
        {
            var shippingStatusExists = await IsShippingStatusExists(shippingStatus.ShippingStatusId, shippingStatus.ShippingStatusNameAr, shippingStatus.ShippingStatusNameEn,shippingStatus.MenuCode);

            var isOrderExist = await IsOrderExist(shippingStatus.ShippingStatusId,shippingStatus.MenuCode, shippingStatus.StatusOrder);

			if (shippingStatusExists.Success)
            {
                return new ResponseDto { Id = shippingStatus.ShippingStatusId, Success = false, Message = _localizer["ShippingStatusAlreadyExist"] };
            }
            else
            {
	            if (isOrderExist.Success)
	            {
		            return new ResponseDto { Id = shippingStatus.ShippingStatusId, Success = false, Message = _localizer["ShippingStatusOrderAlreadyExist"] };
				}
	            else
	            {
					if (shippingStatus.ShippingStatusId == 0)
					{
						return await CreateShippingStatus(shippingStatus);
					}
					else
					{
						return await UpdateShippingStatus(shippingStatus);
					}
				}
            }
        }

        private async Task<ResponseDto> CreateShippingStatus(ShippingStatusDto shippingStatus)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var newShippingStatus = new ShippingStatus
            {
                ShippingStatusId = await GetNextId(),
                ShippingStatusCode = await GetNextCode(companyId),
                ShippingStatusNameAr = shippingStatus.ShippingStatusNameAr,
                ShippingStatusNameEn = shippingStatus.ShippingStatusNameEn,
                CompanyId = companyId,
                MenuCode = shippingStatus.MenuCode,
                StatusOrder = shippingStatus.StatusOrder,
                IsActive = shippingStatus.IsActive,
                InActiveReasons = shippingStatus.InActiveReasons,
				IpAddressCreated = _httpContextAccessor.GetIpAddress(),
                UserNameCreated = await _httpContextAccessor.GetUserName(),
                CreatedAt = DateHelper.GetDateTimeNow(),
                Hide = false
            };

            var validationResult = await new ShippingStatusValidator(_localizer).ValidateAsync(newShippingStatus);
            if (validationResult.IsValid)
            {
                await _repository.Insert(newShippingStatus);
                await _repository.SaveChanges();
                return new ResponseDto { Id = shippingStatus.ShippingStatusId, Success = true, Message = _localizer["NewShippingStatusSuccessMessage", (language == LanguageCode.Arabic ? shippingStatus.ShippingStatusNameAr : shippingStatus.ShippingStatusNameEn) ?? "", newShippingStatus.ShippingStatusCode] };
            }
            else
            {
                return new ResponseDto { Id = shippingStatus.ShippingStatusId, Success = false, Message = validationResult.ToString("~") };
            }
        }

        private async Task<ResponseDto> UpdateShippingStatus(ShippingStatusDto shippingStatus)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var shippingStatusDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ShippingStatusId == shippingStatus.ShippingStatusId);

            if(shippingStatusDb != null)
            {
                shippingStatusDb.ShippingStatusNameAr = shippingStatus.ShippingStatusNameAr;
                shippingStatusDb.ShippingStatusNameEn = shippingStatus.ShippingStatusNameEn;
                shippingStatusDb.MenuCode = shippingStatus.MenuCode;
                shippingStatusDb.StatusOrder = shippingStatus.StatusOrder;
                shippingStatusDb.IsActive = shippingStatus.IsActive;
                shippingStatusDb.InActiveReasons = shippingStatus.InActiveReasons;

                shippingStatusDb.IpAddressModified = _httpContextAccessor.GetIpAddress();
                shippingStatusDb.UserNameModified = await _httpContextAccessor.GetUserName();
                shippingStatusDb.ModifiedAt = DateHelper.GetDateTimeNow();

                var validationResult = await new ShippingStatusValidator(_localizer).ValidateAsync(shippingStatusDb);
                if (validationResult.IsValid)
                {
                    _repository.Update(shippingStatusDb);
                    await _repository.SaveChanges();
                    return new ResponseDto { Id = shippingStatus.ShippingStatusId, Success = true, Message = _localizer["UpdateShippingStatusSuccessMessage", (language == LanguageCode.Arabic ? shippingStatus.ShippingStatusNameAr : shippingStatus.ShippingStatusNameEn) ?? ""] };
                }
                else
                {
                    return new ResponseDto { Id = shippingStatus.ShippingStatusId, Success = false, Message = validationResult.ToString("~") };
                }
            }
            return new ResponseDto { Id = shippingStatus.ShippingStatusId, Success = false, Message = _localizer["NoShippingStatusFound"] };
        } 

        public async Task<ResponseDto> DeleteShippingStatus(int id)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var shippingStatusDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ShippingStatusId == id);

            if (shippingStatusDb != null)
            {
                _repository.Delete(shippingStatusDb);
                await _repository.SaveChanges();
                return new ResponseDto { Id = id, Success = true, Message = _localizer["DeleteShippingStatusSuccessMessage", (language == LanguageCode.Arabic ? shippingStatusDb.ShippingStatusNameAr : shippingStatusDb.ShippingStatusNameEn) ?? ""] };
            }
            else
            {
                return new ResponseDto { Id = id, Success = false, Message = _localizer["NoShippingStatusFound"] };
            }
        }

        private async Task<ResponseDto> IsShippingStatusExists(int id, string? nameAr, string? nameEn,int menuCode)
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var shippingStatus = await _repository.GetAll().FirstOrDefaultAsync(x => x.ShippingStatusId != id && x.CompanyId == companyId && x.MenuCode == menuCode && (x.ShippingStatusNameAr == nameAr || x.ShippingStatusNameEn == nameEn || x.ShippingStatusNameAr == nameEn || x.ShippingStatusNameEn == nameAr));

            if (shippingStatus == null)
            {
                return new ResponseDto { Id = 0, Success = false };
            }

            return new ResponseDto { Id = shippingStatus.ShippingStatusId, Success = true };
        }
        
        private async Task<ResponseDto> IsOrderExist(int id,int menuCode,int order)
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var shippingStatus = await _repository.GetAll().FirstOrDefaultAsync(x => x.ShippingStatusId != id && x.CompanyId == companyId && x.StatusOrder == order && x.MenuCode == menuCode);

            if (shippingStatus == null)
            {
                return new ResponseDto { Id = 0, Success = false };
            }

            return new ResponseDto { Id = shippingStatus.ShippingStatusId, Success = true };
        }

        private async Task<int> GetNextId()
        {
            int id = 1;
            try
            {
                id = await _repository.GetAll().MaxAsync(x => x.ShippingStatusId) + 1;
            }
            catch
            {
                id = 1;
            }

            return id;
        }

		private async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try
			{
				id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(x => x.ShippingStatusCode) + 1;
			}
			catch
			{
				id = 1;
			}

			return id;
		}
	}
}
