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
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Items
{
    public class ItemSectionService : BaseService<ItemSection>, IItemSectionService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemCategoryService _itemCategoryService;
		private readonly IItemSubCategoryService _itemSubCategoryService;
		private readonly IStringLocalizer<ItemSectionService> _localizer;

		public ItemSectionService(IRepository<ItemSection> repository,IHttpContextAccessor httpContextAccessor,IItemCategoryService itemCategoryService,IItemSubCategoryService itemSubCategoryService,IStringLocalizer<ItemSectionService> localizer) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_itemCategoryService = itemCategoryService;
			_itemSubCategoryService = itemSubCategoryService;
			_localizer = localizer;
		}

		public IQueryable<ItemSectionDto> GetItemSections()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return
				from itemSection in _repository.GetAll()
				from subCategory in _itemSubCategoryService.GetAll().Where(x=>x.ItemSubCategoryId == itemSection.ItemSubCategoryId)
				from category in _itemCategoryService.GetAll().Where(x => x.ItemCategoryId == subCategory.ItemCategoryId)
				select new ItemSectionDto()
				{
					ItemSectionId = itemSection.ItemSectionId,
					ItemSectionCode = itemSection.ItemSectionCode,
					SectionNameAr = itemSection.SectionNameAr,
					SectionNameEn = itemSection.SectionNameEn,
					ItemCategoryId = subCategory.ItemCategoryId,
					ItemSubCategoryId = subCategory.ItemSubCategoryId,
					SectionName = language == LanguageCode.Arabic ? itemSection.SectionNameAr : itemSection.SectionNameEn,
					ItemCategoryName = language == LanguageCode.Arabic ? category.CategoryNameAr : category.CategoryNameEn,
					ItemSubCategoryName = language == LanguageCode.Arabic ? subCategory.SubCategoryNameAr : subCategory.SubCategoryNameEn,
					CompanyId = itemSection.CompanyId,
					CreatedAt = subCategory.CreatedAt,
					ModifiedAt = subCategory.ModifiedAt,
					UserNameCreated = subCategory.UserNameCreated,
					UserNameModified = subCategory.UserNameModified
				};
		}

        public IQueryable<ItemSectionDto> GetCompanyItemSections()
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return GetItemSections().Where(x => x.CompanyId == companyId);
        }

        public IQueryable<ItemSectionDto> GetItemSectionsBySubCategoryId(int subCategoryId)
		{
			return GetCompanyItemSections().Where(x => x.ItemSubCategoryId == subCategoryId);
		}

		public IQueryable<ItemSectionDropDownDto> GetItemSectionsDropDown(int subCategoryId)
		{
			return GetItemSectionsBySubCategoryId(subCategoryId).Select(x => new ItemSectionDropDownDto()
			{
				ItemSectionId = x.ItemSectionId,
				ItemSubCategoryId = x.ItemSubCategoryId,
				ItemSectionName = x.SectionName
			}).OrderBy(x => x.ItemSectionName);
		}

		public Task<ItemSectionDto?> GetItemSectionById(int id)
		{
			return GetCompanyItemSections().FirstOrDefaultAsync(x => x.ItemSectionId == id);
		}

		public async Task<ResponseDto> SaveItemSection(ItemSectionDto itemSection)
		{
			var itemSectionExist = await IsItemSectionExist(itemSection.ItemSectionId, itemSection.ItemCategoryId, itemSection.SectionNameAr, itemSection.SectionNameEn);
			if (itemSectionExist.Success)
			{
				return new ResponseDto() { Id = itemSectionExist.Id, Success = false, Message = _localizer["ItemSectionAlreadyExist"] };
			}
			else
			{
				if (itemSection.ItemSectionId == 0)
				{
					return await CreateItemSection(itemSection);
				}
				else
				{
					return await UpdateItemSection(itemSection);
				}
			}
		}

		public async Task<ResponseDto> IsItemSectionExist(int id, int parentId, string? nameAr, string? nameEn)
		{
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            var itemSection = await _repository.GetAll().FirstOrDefaultAsync(x => (x.SectionNameAr == nameAr || x.SectionNameEn == nameEn || x.SectionNameAr == nameEn || x.SectionNameEn == nameAr) && x.ItemSectionId != id && x.ItemSubCategoryId == parentId && x.CompanyId == companyId);
			if (itemSection != null)
			{
				return new ResponseDto() { Id = itemSection.ItemSectionId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemSectionId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.ItemSectionCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateItemSection(ItemSectionDto itemSection)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            var newItemSection = new ItemSection()
			{
				ItemSectionId = await GetNextId(),
				ItemSectionCode = await GetNextCode(companyId),
				SectionNameAr = itemSection?.SectionNameAr?.Trim(),
				SectionNameEn = itemSection?.SectionNameEn?.Trim(),
				ItemSubCategoryId = itemSection!.ItemSubCategoryId,
				CompanyId = companyId,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var itemSectionValidator = await new ItemSectionValidator(_localizer).ValidateAsync(newItemSection);
			var validationResult = itemSectionValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newItemSection);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newItemSection.ItemSectionId, Success = true, Message = _localizer["NewItemSectionSuccessMessage", ((language == LanguageCode.Arabic ? newItemSection.SectionNameAr : newItemSection.SectionNameEn) ?? ""), newItemSection.ItemSectionCode] };
			}
			else
			{
				return new ResponseDto() { Id = newItemSection.ItemSectionId, Success = false, Message = itemSectionValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateItemSection(ItemSectionDto itemSection)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemSectionDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemSectionId == itemSection.ItemSectionId);
			if (itemSectionDb != null)
			{
				itemSectionDb.SectionNameAr = itemSection.SectionNameAr?.Trim();
				itemSectionDb.SectionNameEn = itemSection.SectionNameEn?.Trim();
				itemSectionDb.ItemSubCategoryId = itemSection!.ItemSubCategoryId;
				itemSectionDb.ModifiedAt = DateHelper.GetDateTimeNow();
				itemSectionDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				itemSectionDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var itemSectionValidator = await new ItemSectionValidator(_localizer).ValidateAsync(itemSectionDb);
				var validationResult = itemSectionValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(itemSectionDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = itemSectionDb.ItemSectionId, Success = true, Message = _localizer["UpdateItemSectionSuccessMessage", ((language == LanguageCode.Arabic ? itemSectionDb.SectionNameAr : itemSectionDb.SectionNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = itemSectionValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoItemSectionFound"] };
		}

		public async Task<ResponseDto> DeleteItemSection(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemSectionDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemSectionId == id);
			if (itemSectionDb != null)
			{
				_repository.Delete(itemSectionDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteItemSectionSuccessMessage", ((language == LanguageCode.Arabic ? itemSectionDb.SectionNameAr : itemSectionDb.SectionNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoItemSectionFound"] };
		}
	}
}
