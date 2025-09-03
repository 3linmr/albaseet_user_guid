using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Menus;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Menus
{
	public class MenuNoteIdentifierService : BaseService<MenuNoteIdentifier>, IMenuNoteIdentifierService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<MenuNoteIdentifierService> _localizer;
		private readonly IColumnIdentifierService _columnIdentifierService;
		private readonly IMenuService _menuService;

		public MenuNoteIdentifierService(IRepository<MenuNoteIdentifier> repository, IHttpContextAccessor httpContextAccessor, IStringLocalizer<MenuNoteIdentifierService> localizer, IColumnIdentifierService columnIdentifierService,IMenuService menuService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_columnIdentifierService = columnIdentifierService;
			_menuService = menuService;
		}

		public IQueryable<MenuNoteIdentifierDto> GetAllMenuNoteIdentifiers()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				from menuNoteIdentifier in _repository.GetAll()
				from columnIdentifier in _columnIdentifierService.GetColumnIdentifiersDropDown().Where(x => x.ColumnIdentifierId == menuNoteIdentifier.ColumnIdentifierId)
				from menu in _menuService.GetAll().Where(x=>x.MenuCode == menuNoteIdentifier.MenuCode).DefaultIfEmpty() 
				select new MenuNoteIdentifierDto()
				{
					MenuNoteIdentifierId = menuNoteIdentifier.MenuNoteIdentifierId,
					MenuNoteIdentifierCode = menuNoteIdentifier.MenuNoteIdentifierCode,
					MenuNoteIdentifierNameAr = menuNoteIdentifier.MenuNoteIdentifierNameAr,
					MenuNoteIdentifierNameEn = menuNoteIdentifier.MenuNoteIdentifierNameEn,
					ColumnIdentifierId = menuNoteIdentifier.ColumnIdentifierId,
					ColumnIdentifierName = columnIdentifier.ColumnIdentifierName,
					CompanyId = menuNoteIdentifier.CompanyId,
					MenuCode = (short?)(menuNoteIdentifier.MenuCode == null ? 0 : menu.MenuCode),
					MenuCodeName = menu != null ? language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn : _localizer["All"]
				};
			return data;
		}

        public IQueryable<MenuNoteIdentifierDto> GetCompanyMenuNoteIdentifiers()
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return GetAllMenuNoteIdentifiers().Where(x => x.CompanyId == companyId);
        }

        public IQueryable<MenuNoteIdentifierDropDownDto> GetAllMenuNoteIdentifiersDropDown(int menuCode)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var data = GetCompanyMenuNoteIdentifiers().Where(x=>x.MenuCode == menuCode || x.MenuCode == 0).Select(x => new MenuNoteIdentifierDropDownDto()
			{
				MenuNoteIdentifierId = x.MenuNoteIdentifierId,
				ColumnIdentifierId = x.ColumnIdentifierId,
				MenuCode = x.MenuCode,
				MenuNoteIdentifierName = language == LanguageCode.Arabic ? x.MenuNoteIdentifierNameAr : x.MenuNoteIdentifierNameEn
			}).OrderBy(x => x.MenuNoteIdentifierName);
			return data;
		}

		public Task<MenuNoteIdentifierDto?> GetMenuNoteIdentifierById(int id)
		{
			return GetAllMenuNoteIdentifiers().FirstOrDefaultAsync(x => x.MenuNoteIdentifierId == id);
		}

		public async Task<ResponseDto> SaveMenuNoteIdentifier(MenuNoteIdentifierDto menuNoteIdentifier)
		{
			var menuNoteIdentifierExist = await IsMenuNoteIdentifierExist(menuNoteIdentifier.MenuNoteIdentifierId, menuNoteIdentifier.MenuNoteIdentifierNameAr, menuNoteIdentifier.MenuNoteIdentifierNameEn,menuNoteIdentifier.MenuCode);
			if (menuNoteIdentifierExist.Success)
			{
				return new ResponseDto() { Id = menuNoteIdentifierExist.Id, Success = false, Message = _localizer["MenuNoteIdentifierAlreadyExist"] };
			}
			else
			{
				if (menuNoteIdentifier.MenuNoteIdentifierId == 0)
				{
					return await CreateMenuNoteIdentifier(menuNoteIdentifier);
				}
				else
				{
					return await UpdateMenuNoteIdentifier(menuNoteIdentifier);
				}
			}
		}

		public async Task<ResponseDto> IsMenuNoteIdentifierExist(int id, string? nameAr, string? nameEn,int? menuCode)
		{
            var companyId = _httpContextAccessor.GetCurrentUserCompany(); 
            var menuNoteIdentifier = await _repository.GetAll().FirstOrDefaultAsync(x => (x.MenuNoteIdentifierNameAr == nameAr || x.MenuNoteIdentifierNameEn == nameEn || x.MenuNoteIdentifierNameAr == nameEn || x.MenuNoteIdentifierNameEn == nameAr) && x.MenuNoteIdentifierId != id && x.CompanyId == companyId && x.MenuCode == menuCode);
			if (menuNoteIdentifier != null)
			{
				return new ResponseDto() { Id = menuNoteIdentifier.MenuNoteIdentifierId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.MenuNoteIdentifierId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.MenuNoteIdentifierCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateMenuNoteIdentifier(MenuNoteIdentifierDto menuNoteIdentifier)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();

			var newMenuNoteIdentifier = new MenuNoteIdentifier()
			{
				MenuNoteIdentifierId = await GetNextId(),
				MenuNoteIdentifierCode = await GetNextCode(companyId),
				ColumnIdentifierId = menuNoteIdentifier!.ColumnIdentifierId,
				MenuNoteIdentifierNameAr = menuNoteIdentifier?.MenuNoteIdentifierNameAr?.Trim(),
				MenuNoteIdentifierNameEn = menuNoteIdentifier?.MenuNoteIdentifierNameEn?.Trim(),
				MenuCode = menuNoteIdentifier?.MenuCode == 0 ? null: menuNoteIdentifier?.MenuCode,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
				CompanyId = companyId
            };

			var menuNoteIdentifierValidator = await new MenuNoteIdentifierValidator(_localizer).ValidateAsync(newMenuNoteIdentifier);
			var validationResult = menuNoteIdentifierValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newMenuNoteIdentifier);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newMenuNoteIdentifier.MenuNoteIdentifierId, Success = true, Message = _localizer["NewMenuNoteIdentifierSuccessMessage", ((language == LanguageCode.Arabic ? newMenuNoteIdentifier.MenuNoteIdentifierNameAr : newMenuNoteIdentifier.MenuNoteIdentifierNameEn) ?? ""), newMenuNoteIdentifier.MenuNoteIdentifierCode] };
			}
			else
			{
				return new ResponseDto() { Id = newMenuNoteIdentifier.MenuNoteIdentifierId, Success = false, Message = menuNoteIdentifierValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateMenuNoteIdentifier(MenuNoteIdentifierDto menuNoteIdentifier)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var menuNoteIdentifierDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.MenuNoteIdentifierId == menuNoteIdentifier.MenuNoteIdentifierId);
			if (menuNoteIdentifierDb != null)
			{
				menuNoteIdentifierDb.ColumnIdentifierId = menuNoteIdentifier!.ColumnIdentifierId;
				menuNoteIdentifierDb.MenuNoteIdentifierNameAr = menuNoteIdentifier.MenuNoteIdentifierNameAr?.Trim();
				menuNoteIdentifierDb.MenuNoteIdentifierNameEn = menuNoteIdentifier.MenuNoteIdentifierNameEn?.Trim();
				menuNoteIdentifierDb.MenuCode = menuNoteIdentifier?.MenuCode == 0 ? null : menuNoteIdentifier?.MenuCode;
				menuNoteIdentifierDb.CompanyId = companyId;
				menuNoteIdentifierDb.ModifiedAt = DateHelper.GetDateTimeNow();
				menuNoteIdentifierDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				menuNoteIdentifierDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var menuNoteIdentifierValidator = await new MenuNoteIdentifierValidator(_localizer).ValidateAsync(menuNoteIdentifierDb);
				var validationResult = menuNoteIdentifierValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(menuNoteIdentifierDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = menuNoteIdentifierDb.MenuNoteIdentifierId, Success = true, Message = _localizer["UpdateMenuNoteIdentifierSuccessMessage", ((language == LanguageCode.Arabic ? menuNoteIdentifierDb.MenuNoteIdentifierNameAr : menuNoteIdentifierDb.MenuNoteIdentifierNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = menuNoteIdentifierValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoMenuNoteIdentifierFound"] };
		}

		public async Task<ResponseDto> DeleteMenuNoteIdentifier(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var menuNoteIdentifierDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.MenuNoteIdentifierId == id);
			if (menuNoteIdentifierDb != null)
			{
				_repository.Delete(menuNoteIdentifierDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteMenuNoteIdentifierSuccessMessage", ((language == LanguageCode.Arabic ? menuNoteIdentifierDb.MenuNoteIdentifierNameAr : menuNoteIdentifierDb.MenuNoteIdentifierNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoMenuNoteIdentifierFound"] };
		}
	}
}
