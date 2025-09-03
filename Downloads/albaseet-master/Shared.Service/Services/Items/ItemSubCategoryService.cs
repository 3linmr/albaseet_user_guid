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
    public class ItemSubCategoryService : BaseService<ItemSubCategory>, IItemSubCategoryService
	{
		private readonly IItemCategoryService _itemCategoryService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<ItemSubCategoryService> _localizer;

		public ItemSubCategoryService(IRepository<ItemSubCategory> repository, IItemCategoryService itemCategoryService, IHttpContextAccessor httpContextAccessor,IStringLocalizer<ItemSubCategoryService> localizer) : base(repository)
		{
			_itemCategoryService = itemCategoryService;
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
		}

		public IQueryable<ItemSubCategoryDto> GetItemSubCategories()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return
				from subCategory in _repository.GetAll()
				from category in _itemCategoryService.GetAll().Where(x => x.ItemCategoryId == subCategory.ItemCategoryId)
				select new ItemSubCategoryDto()
				{
					SubCategoryNameAr = subCategory.SubCategoryNameAr,
					SubCategoryNameEn = subCategory.SubCategoryNameEn,
					ItemCategoryId = subCategory.ItemCategoryId,
					CompanyId = subCategory.CompanyId,
					CreatedAt = subCategory.CreatedAt,
					ModifiedAt = subCategory.ModifiedAt,
					UserNameCreated = subCategory.UserNameCreated,
					UserNameModified = subCategory.UserNameModified,
					ItemSubCategoryId = subCategory.ItemSubCategoryId,
					ItemSubCategoryCode = subCategory.ItemSubCategoryCode,
					ItemCategoryName = language == LanguageCode.Arabic ? category.CategoryNameAr : category.CategoryNameEn,
					SubCategoryName = language == LanguageCode.Arabic ? subCategory.SubCategoryNameAr : subCategory.SubCategoryNameEn
				};
		}

        public IQueryable<ItemSubCategoryDto> GetCompanyItemSubCategories()
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return GetItemSubCategories().Where(x => x.CompanyId == companyId);
        }

        public IQueryable<ItemSubCategoryDto> GetItemSubCategoriesByCategoryId(int categoryId)
		{
			return GetCompanyItemSubCategories().Where(x => x.ItemCategoryId == categoryId);
		}

		public IQueryable<ItemSubCategoryDropDownDto> GetItemSubCategoriesDropDown(int categoryId)
		{
			return GetItemSubCategoriesByCategoryId(categoryId).Select(x => new ItemSubCategoryDropDownDto()
			{
				ItemCategoryId = x.ItemCategoryId,
				ItemSubCategoryId = x.ItemSubCategoryId,
				SubCategoryName = x.SubCategoryName
			}).OrderBy(x => x.SubCategoryName);
		}

		public Task<ItemSubCategoryDto?> GetItemSubCategoryById(int id)
		{
			return GetItemSubCategories().FirstOrDefaultAsync(x => x.ItemSubCategoryId == id);
		}

		public async Task<ResponseDto> SaveItemSubCategory(ItemSubCategoryDto itemSubCategory)
		{
			var itemSubCategoryExist = await IsItemSubCategoryExist(itemSubCategory.ItemSubCategoryId,itemSubCategory.ItemCategoryId, itemSubCategory.SubCategoryNameAr, itemSubCategory.SubCategoryNameEn);
			if (itemSubCategoryExist.Success)
			{
				return new ResponseDto() { Id = itemSubCategoryExist.Id, Success = false, Message = _localizer["ItemSubCategoryAlreadyExist"] };
			}
			else
			{
				if (itemSubCategory.ItemSubCategoryId == 0)
				{
					return await CreateItemSubCategory(itemSubCategory);
				}
				else
				{
					return await UpdateItemSubCategory(itemSubCategory);
				}
			}
		}

		public async Task<ResponseDto> IsItemSubCategoryExist(int id,int parentId, string? nameAr, string? nameEn)
		{
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            var itemSubCategory = await _repository.GetAll().FirstOrDefaultAsync(x => (x.SubCategoryNameAr == nameAr || x.SubCategoryNameEn == nameEn || x.SubCategoryNameAr == nameEn || x.SubCategoryNameEn == nameAr) && x.ItemSubCategoryId != id && x.ItemCategoryId == parentId && x.CompanyId == companyId);
			if (itemSubCategory != null)
			{
				return new ResponseDto() { Id = itemSubCategory.ItemSubCategoryId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemSubCategoryId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.ItemSubCategoryCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateItemSubCategory(ItemSubCategoryDto itemSubCategory)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var newItemSubCategory = new ItemSubCategory()
			{
				ItemSubCategoryId = await GetNextId(),
				ItemSubCategoryCode = await GetNextCode(companyId),
				SubCategoryNameAr = itemSubCategory?.SubCategoryNameAr?.Trim(),
				SubCategoryNameEn = itemSubCategory?.SubCategoryNameEn?.Trim(),
				ItemCategoryId = itemSubCategory!.ItemCategoryId,
				CompanyId = companyId,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var itemSubCategoryValidator = await new ItemSubCategoryValidator(_localizer).ValidateAsync(newItemSubCategory);
			var validationResult = itemSubCategoryValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newItemSubCategory);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newItemSubCategory.ItemSubCategoryId, Success = true, Message = _localizer["NewItemSubCategorySuccessMessage", ((language == LanguageCode.Arabic ? newItemSubCategory.SubCategoryNameAr : newItemSubCategory.SubCategoryNameEn) ?? ""), newItemSubCategory.ItemSubCategoryCode] };
			}
			else
			{
				return new ResponseDto() { Id = newItemSubCategory.ItemSubCategoryId, Success = false, Message = itemSubCategoryValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateItemSubCategory(ItemSubCategoryDto itemSubCategory)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemSubCategoryDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemSubCategoryId == itemSubCategory.ItemSubCategoryId);
			if (itemSubCategoryDb != null)
			{
				itemSubCategoryDb.SubCategoryNameAr = itemSubCategory.SubCategoryNameAr?.Trim();
				itemSubCategoryDb.SubCategoryNameEn = itemSubCategory.SubCategoryNameEn?.Trim();
				itemSubCategoryDb.ItemCategoryId = itemSubCategory!.ItemCategoryId;
				itemSubCategoryDb.ModifiedAt = DateHelper.GetDateTimeNow();
				itemSubCategoryDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				itemSubCategoryDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var itemSubCategoryValidator = await new ItemSubCategoryValidator(_localizer).ValidateAsync(itemSubCategoryDb);
				var validationResult = itemSubCategoryValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(itemSubCategoryDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = itemSubCategoryDb.ItemSubCategoryId, Success = true, Message = _localizer["UpdateItemSubCategorySuccessMessage", ((language == LanguageCode.Arabic ? itemSubCategoryDb.SubCategoryNameAr : itemSubCategoryDb.SubCategoryNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = itemSubCategoryValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoItemSubCategoryFound"] };
		}

		public async Task<ResponseDto> DeleteItemSubCategory(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemSubCategoryDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemSubCategoryId == id);
			if (itemSubCategoryDb != null)
			{
				_repository.Delete(itemSubCategoryDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteItemSubCategorySuccessMessage", ((language == LanguageCode.Arabic ? itemSubCategoryDb.SubCategoryNameAr : itemSubCategoryDb.SubCategoryNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoItemSubCategoryFound"] };
		}
	}
}
