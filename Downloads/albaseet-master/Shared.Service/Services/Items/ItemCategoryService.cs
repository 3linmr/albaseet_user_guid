using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Items
{
    public class ItemCategoryService : BaseService<ItemCategory>, IItemCategoryService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<ItemCategoryService> _localizer;

		public ItemCategoryService(IRepository<ItemCategory> repository,IHttpContextAccessor httpContextAccessor,IStringLocalizer<ItemCategoryService> localizer) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
		}

		public IQueryable<ItemCategoryDto> GetItemCategories()
		{
			return _repository.GetAll().Select(x => new ItemCategoryDto()
			{
				CategoryNameAr = x.CategoryNameAr,
				CategoryNameEn = x.CategoryNameEn,
				ItemCategoryId = x.ItemCategoryId,
				ItemCategoryCode = x.ItemCategoryCode,
				CompanyId = x.CompanyId,
				CreatedAt = x.CreatedAt,
				ModifiedAt = x.ModifiedAt,
				UserNameCreated = x.UserNameCreated,
				UserNameModified = x.UserNameModified,
			});
		}

        public IQueryable<ItemCategoryDto> GetCompanyItemCategories()
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return GetItemCategories().Where(x => x.CompanyId == companyId);
        }

        public IQueryable<ItemCategoryDropDownDto> GetItemCategoriesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetCompanyItemCategories().Select(x => new ItemCategoryDropDownDto()
			{
				ItemCategoryId = x.ItemCategoryId,
				CategoryName = language == LanguageCode.Arabic ? x.CategoryNameAr : x.CategoryNameEn
			}).OrderBy(x => x.CategoryName);
		}

		public async Task<ItemCategoryDto?> GetItemCategoryById(int id)
		{
			return await GetItemCategories().FirstOrDefaultAsync(x => x.ItemCategoryId == id);
		}

		public async Task<ResponseDto> SaveItemCategory(ItemCategoryDto itemCategory)
		{
			var itemCategoryExist = await IsItemCategoryExist(itemCategory.ItemCategoryId, itemCategory.CategoryNameAr, itemCategory.CategoryNameEn);
			if (itemCategoryExist.Success)
			{
				return new ResponseDto() { Id = itemCategoryExist.Id, Success = false, Message = _localizer["ItemCategoryAlreadyExist"] };
			}
			else
			{
				if (itemCategory.ItemCategoryId == 0)
				{
					return await CreateItemCategory(itemCategory);
				}
				else
				{
					return await UpdateItemCategory(itemCategory);
				}
			}
		}

		public async Task<ResponseDto> IsItemCategoryExist(int id, string? nameAr, string? nameEn)
		{
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            var itemCategory = await _repository.GetAll().FirstOrDefaultAsync(x => (x.CategoryNameAr == nameAr || x.CategoryNameEn == nameEn || x.CategoryNameAr == nameEn || x.CategoryNameEn == nameAr) && x.ItemCategoryId != id && x.CompanyId == companyId);
			if (itemCategory != null)
			{
				return new ResponseDto() { Id = itemCategory.ItemCategoryId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemCategoryId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.ItemCategoryCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateItemCategory(ItemCategoryDto itemCategory)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var newItemCategory = new ItemCategory()
			{
				ItemCategoryId = await GetNextId(),
				ItemCategoryCode = await GetNextCode(companyId),
				CategoryNameAr = itemCategory?.CategoryNameAr?.Trim(),
				CategoryNameEn = itemCategory?.CategoryNameEn?.Trim(),
				CompanyId = companyId,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false
			};

			var itemCategoryValidator = await new ItemCategoryValidator(_localizer).ValidateAsync(newItemCategory);
			var validationResult = itemCategoryValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newItemCategory);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newItemCategory.ItemCategoryId, Success = true, Message = _localizer["NewItemCategorySuccessMessage", ((language == LanguageCode.Arabic ? newItemCategory.CategoryNameAr : newItemCategory.CategoryNameEn) ?? ""), newItemCategory.ItemCategoryCode] };
			}
			else
			{
				return new ResponseDto() { Id = newItemCategory.ItemCategoryId, Success = false, Message = itemCategoryValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateItemCategory(ItemCategoryDto itemCategory)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemCategoryDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemCategoryId == itemCategory.ItemCategoryId);
			if (itemCategoryDb != null)
			{
				itemCategoryDb.CategoryNameAr = itemCategory.CategoryNameAr?.Trim();
				itemCategoryDb.CategoryNameEn = itemCategory.CategoryNameEn?.Trim();
				itemCategoryDb.ModifiedAt = DateHelper.GetDateTimeNow();
				itemCategoryDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				itemCategoryDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var itemCategoryValidator = await new ItemCategoryValidator(_localizer).ValidateAsync(itemCategoryDb);
				var validationResult = itemCategoryValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(itemCategoryDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = itemCategoryDb.ItemCategoryId, Success = true, Message = _localizer["UpdateItemCategorySuccessMessage", ((language == LanguageCode.Arabic ? itemCategoryDb.CategoryNameAr : itemCategoryDb.CategoryNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = itemCategoryValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoItemCategoryFound"] };
		}

		public async Task<ResponseDto> DeleteItemCategory(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemCategoryDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemCategoryId == id);
			if (itemCategoryDb != null)
			{
				_repository.Delete(itemCategoryDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteItemCategorySuccessMessage", ((language == LanguageCode.Arabic ? itemCategoryDb.CategoryNameAr : itemCategoryDb.CategoryNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoItemCategoryFound"] };
		}
	}
}
