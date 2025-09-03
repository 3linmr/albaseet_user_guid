using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static System.Collections.Specialized.BitVector32;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Items
{
    public class MainItemService : BaseService<MainItem>, IMainItemService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemCategoryService _itemCategoryService;
		private readonly IItemSubCategoryService _itemSubCategoryService;
		private readonly IItemSectionService _itemSectionService;
		private readonly IItemSubSectionService _itemSubSectionService;
		private readonly IStringLocalizer<MainItemService> _localizer;

		public MainItemService(IRepository<MainItem> repository, IHttpContextAccessor httpContextAccessor, IItemCategoryService itemCategoryService, IItemSubCategoryService itemSubCategoryService, IItemSectionService itemSectionService,IItemSubSectionService itemSubSectionService, IStringLocalizer<MainItemService> localizer) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_itemCategoryService = itemCategoryService;
			_itemSubCategoryService = itemSubCategoryService;
			_itemSectionService = itemSectionService;
			_itemSubSectionService = itemSubSectionService;
			_localizer = localizer;
		}

		public IQueryable<MainItemDto> GetMainItems()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return
				from mainItem in _repository.GetAll()
				from itemSubSection in _itemSubSectionService.GetAll().Where(x=>x.ItemSubSectionId == mainItem.ItemSubSectionId)
				from itemSection in _itemSectionService.GetAll().Where(x => x.ItemSectionId == itemSubSection.ItemSectionId)
				from subCategory in _itemSubCategoryService.GetAll().Where(x => x.ItemSubCategoryId == itemSection.ItemSubCategoryId)
				from category in _itemCategoryService.GetAll().Where(x => x.ItemCategoryId == subCategory.ItemCategoryId)
				select new MainItemDto()
				{
					MainItemId = mainItem.MainItemId,
					MainItemCode = mainItem.MainItemCode,
					MainItemNameAr = mainItem.MainItemNameAr,
					MainItemNameEn = mainItem.MainItemNameEn,
					ItemSubSectionId = itemSubSection.ItemSubSectionId,
					ItemCategoryId = subCategory.ItemCategoryId,
					ItemSubCategoryId = subCategory.ItemSubCategoryId,
					ItemSectionId = itemSection.ItemSectionId,
					ItemCategoryName = language == LanguageCode.Arabic ? category.CategoryNameAr : category.CategoryNameEn,
					ItemSubCategoryName = language == LanguageCode.Arabic ? subCategory.SubCategoryNameAr : subCategory.SubCategoryNameEn,
					ItemSectionName = language == LanguageCode.Arabic ? itemSection.SectionNameAr : itemSection.SectionNameEn,
					ItemSubSectionName = language == LanguageCode.Arabic ? itemSubSection.SubSectionNameAr : itemSubSection.SubSectionNameEn,
					MainItemName = language == LanguageCode.Arabic ? mainItem.MainItemNameAr : mainItem.MainItemNameEn,
					CompanyId = mainItem.CompanyId,
					CreatedAt = subCategory.CreatedAt,
					ModifiedAt = subCategory.ModifiedAt,
					UserNameCreated = subCategory.UserNameCreated,
					UserNameModified = subCategory.UserNameModified
				};
		}

        public IQueryable<MainItemDto> GetCompanyMainItems()
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return GetMainItems().Where(x => x.CompanyId == companyId);
        }

        public IQueryable<MainItemDto> GetMainItemsBySubSectionId(int subSectionId)
		{
			return GetCompanyMainItems().Where(x => x.ItemSubSectionId == subSectionId);
		}

		public IQueryable<MainItemDropDownDto> GetMainItemsDropDown(int subSectionId)
		{
			return GetMainItemsBySubSectionId(subSectionId).Select(x => new MainItemDropDownDto()
			{
				MainItemId = x.MainItemId,
				MainItemName = x.MainItemName,
				ItemSubSectionId = x.ItemSubSectionId
			}).OrderBy(x => x.MainItemName);
		}

		public Task<MainItemDto?> GetMainItemById(int id)
		{
			return GetMainItems().FirstOrDefaultAsync(x => x.MainItemId == id);
		}

		public async Task<ResponseDto> SaveMainItem(MainItemDto mainItem)
		{
			var mainItemExist = await IsMainItemExist(mainItem.MainItemId, mainItem.ItemCategoryId, mainItem.MainItemNameAr, mainItem.MainItemNameEn);
			if (mainItemExist.Success)
			{
				return new ResponseDto() { Id = mainItemExist.Id, Success = false, Message = _localizer["MainItemAlreadyExist"] };
			}
			else
			{
				if (mainItem.MainItemId == 0)
				{
					return await CreateMainItem(mainItem);
				}
				else
				{
					return await UpdateMainItem(mainItem);
				}
			}
		}
		public async Task<ResponseDto> IsMainItemExist(int id, int parentId, string? nameAr, string? nameEn)
		{
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            var mainItem = await _repository.GetAll().FirstOrDefaultAsync(x => (x.MainItemNameAr == nameAr || x.MainItemNameEn == nameEn || x.MainItemNameAr == nameEn || x.MainItemNameEn == nameAr) && x.MainItemId != id && x.ItemSubSectionId == parentId && x.CompanyId == companyId);
			if (mainItem != null)
			{
				return new ResponseDto() { Id = mainItem.MainItemId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.MainItemId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.MainItemCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateMainItem(MainItemDto mainItem)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var newMainItem = new MainItem()
			{
				MainItemId = await GetNextId(),
				MainItemCode = await GetNextCode(companyId),
				MainItemNameAr = mainItem?.MainItemNameAr?.Trim(),
				MainItemNameEn = mainItem?.MainItemNameEn?.Trim(),
				ItemSubSectionId = mainItem!.ItemSubSectionId,
				CompanyId = companyId,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var mainItemValidator = await new MainItemValidator(_localizer).ValidateAsync(newMainItem);
			var validationResult = mainItemValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newMainItem);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newMainItem.MainItemId, Success = true, Message = _localizer["NewMainItemSuccessMessage", ((language == LanguageCode.Arabic ? newMainItem.MainItemNameAr : newMainItem.MainItemNameEn) ?? ""), newMainItem.MainItemCode] };
			}
			else
			{
				return new ResponseDto() { Id = newMainItem.MainItemId, Success = false, Message = mainItemValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateMainItem(MainItemDto mainItem)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var mainItemDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.MainItemId == mainItem.MainItemId);
			if (mainItemDb != null)
			{
				mainItemDb.MainItemNameAr = mainItem.MainItemNameAr?.Trim();
				mainItemDb.MainItemNameEn = mainItem.MainItemNameEn?.Trim();
				mainItemDb.ItemSubSectionId = mainItem!.ItemSubSectionId;
				mainItemDb.ModifiedAt = DateHelper.GetDateTimeNow();
				mainItemDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				mainItemDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var mainItemValidator = await new MainItemValidator(_localizer).ValidateAsync(mainItemDb);
				var validationResult = mainItemValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(mainItemDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = mainItemDb.MainItemId, Success = true, Message = _localizer["UpdateMainItemSuccessMessage", ((language == LanguageCode.Arabic ? mainItemDb.MainItemNameAr : mainItemDb.MainItemNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = mainItemValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoMainItemFound"] };
		}

		public async Task<ResponseDto> DeleteMainItem(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var mainItemDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.MainItemId == id);
			if (mainItemDb != null)
			{
				_repository.Delete(mainItemDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteMainItemSuccessMessage", ((language == LanguageCode.Arabic ? mainItemDb.MainItemNameAr : mainItemDb.MainItemNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoMainItemFound"] };
		}
	}
}
