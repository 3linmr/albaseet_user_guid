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
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Items
{
    public class ItemSubSectionService : BaseService<ItemSubSection>, IItemSubSectionService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemCategoryService _itemCategoryService;
		private readonly IItemSubCategoryService _itemSubCategoryService;
		private readonly IItemSectionService _itemSectionService;
		private readonly IStringLocalizer<ItemSubSectionService> _localizer;

		public ItemSubSectionService(IRepository<ItemSubSection> repository, IHttpContextAccessor httpContextAccessor, IItemCategoryService itemCategoryService, IItemSubCategoryService itemSubCategoryService,IItemSectionService itemSectionService, IStringLocalizer<ItemSubSectionService> localizer) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_itemCategoryService = itemCategoryService;
			_itemSubCategoryService = itemSubCategoryService;
			_itemSectionService = itemSectionService;
			_localizer = localizer;
		}

		public IQueryable<ItemSubSectionDto> GetItemSubSections()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return
				from itemSubSection in _repository.GetAll()
				from itemSection in _itemSectionService.GetAll().Where(x=>x.ItemSectionId == itemSubSection.ItemSectionId)
				from subCategory in _itemSubCategoryService.GetAll().Where(x => x.ItemSubCategoryId == itemSection.ItemSubCategoryId)
				from category in _itemCategoryService.GetAll().Where(x => x.ItemCategoryId == subCategory.ItemCategoryId)
				select new ItemSubSectionDto()
				{
					ItemSubSectionId = itemSubSection.ItemSubSectionId,
					ItemSubSectionCode = itemSubSection.ItemSubSectionCode,
					SubSectionNameAr = itemSubSection.SubSectionNameAr,
					SubSectionNameEn = itemSubSection.SubSectionNameEn,
					ItemCategoryId = subCategory.ItemCategoryId,
					ItemSubCategoryId = subCategory.ItemSubCategoryId,
					ItemSectionId = itemSection.ItemSectionId,
					ItemCategoryName = language == LanguageCode.Arabic ? category.CategoryNameAr : category.CategoryNameEn,
					ItemSubCategoryName = language == LanguageCode.Arabic ? subCategory.SubCategoryNameAr : subCategory.SubCategoryNameEn,
					ItemSectionName = language == LanguageCode.Arabic ? itemSection.SectionNameAr : itemSection.SectionNameEn,
					ItemSubSectionName = language == LanguageCode.Arabic ? itemSubSection.SubSectionNameAr : itemSubSection.SubSectionNameEn,
					CompanyId = itemSubSection.CompanyId,
					CreatedAt = subCategory.CreatedAt,
					ModifiedAt = subCategory.ModifiedAt,
					UserNameCreated = subCategory.UserNameCreated,
					UserNameModified = subCategory.UserNameModified
				};
		}

        public IQueryable<ItemSubSectionDto> GetCompanyItemSubSections()
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return GetItemSubSections().Where(x => x.CompanyId == companyId);
        }

        public IQueryable<ItemSubSectionDto> GetItemSubSectionsBySectionId(int sectionId)
		{
			return GetCompanyItemSubSections().Where(x => x.ItemSectionId == sectionId);
		}

		public IQueryable<ItemSubSectionDropDownDto> GetItemSubSectionsDropDown(int sectionId)
		{
			return GetItemSubSectionsBySectionId(sectionId).Select(x => new ItemSubSectionDropDownDto()
			{
				ItemSectionId = x.ItemSectionId,
				ItemSubSectionId = x.ItemSubSectionId,
				ItemSubSectionName = x.ItemSubSectionName
			}).OrderBy(x => x.ItemSubSectionName);
		}

		public Task<ItemSubSectionDto?> GetItemSubSectionById(int id)
		{
			return GetItemSubSections().FirstOrDefaultAsync(x => x.ItemSubSectionId == id);
		}

		public async Task<ResponseDto> SaveItemSubSection(ItemSubSectionDto itemSubSection)
		{
			var itemSubSectionExist = await IsItemSubSectionExist(itemSubSection.ItemSubSectionId, itemSubSection.ItemCategoryId, itemSubSection.SubSectionNameAr, itemSubSection.SubSectionNameEn);
			if (itemSubSectionExist.Success)
			{
				return new ResponseDto() { Id = itemSubSectionExist.Id, Success = false, Message = _localizer["ItemSubSectionAlreadyExist"] };
			}
			else
			{
				if (itemSubSection.ItemSubSectionId == 0)
				{
					return await CreateItemSubSection(itemSubSection);
				}
				else
				{
					return await UpdateItemSubSection(itemSubSection);
				}
			}
		}
		public async Task<ResponseDto> IsItemSubSectionExist(int id, int parentId, string? nameAr, string? nameEn)
		{
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            var itemSubSection = await _repository.GetAll().FirstOrDefaultAsync(x => (x.SubSectionNameAr == nameAr || x.SubSectionNameEn == nameEn || x.SubSectionNameAr == nameEn || x.SubSectionNameEn == nameAr) && x.ItemSubSectionId != id && x.ItemSectionId == parentId && x.CompanyId == companyId);
			if (itemSubSection != null)
			{
				return new ResponseDto() { Id = itemSubSection.ItemSubSectionId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemSubSectionId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.ItemSubSectionCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateItemSubSection(ItemSubSectionDto itemSubSection)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var newItemSubSection = new ItemSubSection()
			{
				ItemSubSectionId = await GetNextId(),
				ItemSubSectionCode = await GetNextCode(companyId),
				SubSectionNameAr = itemSubSection?.SubSectionNameAr?.Trim(),
				SubSectionNameEn = itemSubSection?.SubSectionNameEn?.Trim(),
				ItemSectionId = itemSubSection!.ItemSectionId,
				CompanyId = companyId,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var itemSubSectionValidator = await new ItemSubSectionValidator(_localizer).ValidateAsync(newItemSubSection);
			var validationResult = itemSubSectionValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newItemSubSection);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newItemSubSection.ItemSubSectionId, Success = true, Message = _localizer["NewItemSubSectionSuccessMessage", ((language == LanguageCode.Arabic ? newItemSubSection.SubSectionNameAr : newItemSubSection.SubSectionNameEn) ?? ""), newItemSubSection.ItemSubSectionCode] };
			}
			else
			{
				return new ResponseDto() { Id = newItemSubSection.ItemSubSectionId, Success = false, Message = itemSubSectionValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateItemSubSection(ItemSubSectionDto itemSubSection)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemSubSectionDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemSubSectionId == itemSubSection.ItemSubSectionId);
			if (itemSubSectionDb != null)
			{
				itemSubSectionDb.SubSectionNameAr = itemSubSection.SubSectionNameAr?.Trim();
				itemSubSectionDb.SubSectionNameEn = itemSubSection.SubSectionNameEn?.Trim();
				itemSubSectionDb.ItemSectionId = itemSubSection!.ItemSectionId;
				itemSubSectionDb.ModifiedAt = DateHelper.GetDateTimeNow();
				itemSubSectionDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				itemSubSectionDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var itemSubSectionValidator = await new ItemSubSectionValidator(_localizer).ValidateAsync(itemSubSectionDb);
				var validationResult = itemSubSectionValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(itemSubSectionDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = itemSubSectionDb.ItemSubSectionId, Success = true, Message = _localizer["UpdateItemSubSectionSuccessMessage", ((language == LanguageCode.Arabic ? itemSubSectionDb.SubSectionNameAr : itemSubSectionDb.SubSectionNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = itemSubSectionValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoItemSubSectionFound"] };
		}

		public async Task<ResponseDto> DeleteItemSubSection(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemSubSectionDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemSubSectionId == id);
			if (itemSubSectionDb != null)
			{
				_repository.Delete(itemSubSectionDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteItemSubSectionSuccessMessage", ((language == LanguageCode.Arabic ? itemSubSectionDb.SubSectionNameAr : itemSubSectionDb.SubSectionNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoItemSubSectionFound"] };
		}
	}
}
