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
    public class ItemAttributeTypeService : BaseService<ItemAttributeType>, IItemAttributeTypeService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<ItemAttributeTypeService> _localizer;

		public ItemAttributeTypeService(IRepository<ItemAttributeType> repository,IHttpContextAccessor httpContextAccessor,IStringLocalizer<ItemAttributeTypeService> localizer) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
		}

		public IQueryable<ItemAttributeTypeDto> GetItemAttributeTypes()
		{
			return _repository.GetAll().Select(x => new ItemAttributeTypeDto()
			{
				ItemAttributeTypeId = x.ItemAttributeTypeId,
				ItemAttributeTypeCode = x.ItemAttributeTypeCode,
				ItemAttributeTypeNameAr = x.ItemAttributeTypeNameAr,
				ItemAttributeTypeNameEn = x.ItemAttributeTypeNameEn,
				CompanyId = x.CompanyId,
				CreatedAt = x.CreatedAt,
				ModifiedAt = x.ModifiedAt,
				UserNameCreated = x.UserNameCreated,
				UserNameModified = x.UserNameModified
			});
		}

        public IQueryable<ItemAttributeTypeDto> GetCompanyItemAttributeTypes()
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return GetItemAttributeTypes().Where(x => x.CompanyId == companyId);
        }

        public IQueryable<ItemAttributeTypeDropDownDto> GetItemAttributeTypesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return GetCompanyItemAttributeTypes().Select(x => new ItemAttributeTypeDropDownDto()
			{
				ItemAttributeTypeId = x.ItemAttributeTypeId,
				ItemAttributeTypeName = language == LanguageCode.Arabic ? x.ItemAttributeTypeNameAr : x.ItemAttributeTypeNameEn,
			}).OrderBy(x => x.ItemAttributeTypeName);
		}

		public Task<ItemAttributeTypeDto?> GetItemAttributeTypeById(int id)
		{
			return GetItemAttributeTypes().FirstOrDefaultAsync(x => x.ItemAttributeTypeId == id);
		}

		public async Task<ResponseDto> SaveItemAttributeType(ItemAttributeTypeDto itemAttributeType)
		{
			var itemAttributeTypeExist = await IsItemAttributeTypeExist(itemAttributeType.ItemAttributeTypeId, itemAttributeType.ItemAttributeTypeNameAr, itemAttributeType.ItemAttributeTypeNameEn);
			if (itemAttributeTypeExist.Success)
			{
				return new ResponseDto() { Id = itemAttributeTypeExist.Id, Success = false, Message = _localizer["ItemAttributeTypeAlreadyExist"] };
			}
			else
			{
				if (itemAttributeType.ItemAttributeTypeId == 0)
				{
					return await CreateItemAttributeType(itemAttributeType);
				}
				else
				{
					return await UpdateItemAttributeType(itemAttributeType);
				}
			}
		}
		public async Task<ResponseDto> IsItemAttributeTypeExist(int id, string? nameAr, string? nameEn)
		{
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var itemAttributeType = await _repository.GetAll().FirstOrDefaultAsync(x => (x.ItemAttributeTypeNameAr == nameAr || x.ItemAttributeTypeNameEn == nameEn || x.ItemAttributeTypeNameAr == nameEn || x.ItemAttributeTypeNameEn == nameAr) && x.ItemAttributeTypeId != id && x.CompanyId == companyId);
			if (itemAttributeType != null)
			{
				return new ResponseDto() { Id = itemAttributeType.ItemAttributeTypeId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemAttributeTypeId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.ItemAttributeTypeCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateItemAttributeType(ItemAttributeTypeDto itemAttributeType)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var newItemAttributeType = new ItemAttributeType()
			{
				ItemAttributeTypeId = await GetNextId(),
				ItemAttributeTypeCode = await GetNextCode(companyId),
				ItemAttributeTypeNameAr = itemAttributeType?.ItemAttributeTypeNameAr?.Trim(),
				ItemAttributeTypeNameEn = itemAttributeType?.ItemAttributeTypeNameEn?.Trim(),
                CompanyId = companyId,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var itemAttributeTypeValidator = await new ItemAttributeTypeValidator(_localizer).ValidateAsync(newItemAttributeType);
			var validationResult = itemAttributeTypeValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newItemAttributeType);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newItemAttributeType.ItemAttributeTypeId, Success = true, Message = _localizer["NewItemAttributeTypeSuccessMessage", ((language == LanguageCode.Arabic ? newItemAttributeType.ItemAttributeTypeNameAr : newItemAttributeType.ItemAttributeTypeNameEn) ?? ""), newItemAttributeType.ItemAttributeTypeCode] };
			}
			else
			{
				return new ResponseDto() { Id = newItemAttributeType.ItemAttributeTypeId, Success = false, Message = itemAttributeTypeValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateItemAttributeType(ItemAttributeTypeDto itemAttributeType)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemAttributeTypeDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemAttributeTypeId == itemAttributeType.ItemAttributeTypeId);
			if (itemAttributeTypeDb != null)
			{
				itemAttributeTypeDb.ItemAttributeTypeNameAr = itemAttributeType.ItemAttributeTypeNameAr?.Trim();
				itemAttributeTypeDb.ItemAttributeTypeNameEn = itemAttributeType.ItemAttributeTypeNameEn?.Trim();
				itemAttributeTypeDb.ModifiedAt = DateHelper.GetDateTimeNow();
				itemAttributeTypeDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				itemAttributeTypeDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var itemAttributeTypeValidator = await new ItemAttributeTypeValidator(_localizer).ValidateAsync(itemAttributeTypeDb);
				var validationResult = itemAttributeTypeValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(itemAttributeTypeDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = itemAttributeTypeDb.ItemAttributeTypeId, Success = true, Message = _localizer["UpdateItemAttributeTypeSuccessMessage", ((language == LanguageCode.Arabic ? itemAttributeTypeDb.ItemAttributeTypeNameAr : itemAttributeTypeDb.ItemAttributeTypeNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = itemAttributeTypeValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoItemAttributeTypeFound"] };
		}

		public async Task<ResponseDto> DeleteItemAttributeType(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemAttributeTypeDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemAttributeTypeId == id);
			if (itemAttributeTypeDb != null)
			{
				_repository.Delete(itemAttributeTypeDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteItemAttributeTypeSuccessMessage", ((language == LanguageCode.Arabic ? itemAttributeTypeDb.ItemAttributeTypeNameAr : itemAttributeTypeDb.ItemAttributeTypeNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoItemAttributeTypeFound"] };
		}
	}
}
