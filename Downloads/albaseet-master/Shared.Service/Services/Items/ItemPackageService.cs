using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using Shared.Helper.Models.UserDetail;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.Helper.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Items
{
	public class ItemPackageService : BaseService<ItemPackage>, IItemPackageService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<ItemPackageService> _localizer;

		public ItemPackageService(IRepository<ItemPackage> repository, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ItemPackageService> localizer) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
		}

		public IQueryable<ItemPackageDto> GetAllItemPackages()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return
				from itemPackage in _repository.GetAll()
				select new ItemPackageDto()
				{
					ItemPackageId = itemPackage.ItemPackageId,
					ItemPackageCode = itemPackage.ItemPackageCode,
					PackageNameAr = itemPackage.PackageNameAr,
					PackageNameEn = itemPackage.PackageNameEn,
					CompanyId = itemPackage.CompanyId,
					PackageCode = itemPackage.PackageCode,
					CreatedAt = itemPackage.CreatedAt,
					ModifiedAt = itemPackage.ModifiedAt,
					UserNameModified = itemPackage.UserNameModified,
					UserNameCreated = itemPackage.UserNameCreated
				};
		}

        public IQueryable<ItemPackageDto> GetCompanyItemPackages()
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return GetAllItemPackages().Where(x => x.CompanyId == companyId || x.ItemPackageId == ItemPackageData.Each);
        }

        public IQueryable<ItemPackageDropDownDto> GetItemPackagesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
            return GetCompanyItemPackages().Select(x => new ItemPackageDropDownDto()
			{
				ItemPackageId = x.ItemPackageId,
				ItemPackageName = language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn
			}).OrderBy(x => x.ItemPackageName);
		}

		public Task<ItemPackageDto?> GetItemPackageById(int id)
		{
			return GetAllItemPackages().FirstOrDefaultAsync(x => x.ItemPackageId == id);
		}

		public async Task<ResponseDto> SaveItemPackage(ItemPackageDto itemPackage)
		{
			var itemPackageExist = await IsItemPackageExist(itemPackage.ItemPackageId, itemPackage.PackageNameAr, itemPackage.PackageNameEn);
			if (itemPackageExist.Success)
			{
				return new ResponseDto() { Id = itemPackageExist.Id, Success = false, Message = _localizer["ItemPackageAlreadyExist"] };
			}
			else
			{
				if (itemPackage.ItemPackageId == 0)
				{
					return await CreateItemPackage(itemPackage);
				}
				else
				{
					return await UpdateItemPackage(itemPackage);
				}
			}
		}

		public async Task<ResponseDto> IsItemPackageExist(int id, string? nameAr, string? nameEn)
		{
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var itemPackage = await _repository.GetAll().FirstOrDefaultAsync(x => (x.PackageNameAr == nameAr || x.PackageNameEn == nameEn || x.PackageNameAr == nameEn || x.PackageNameEn == nameAr) && x.ItemPackageId != id && x.CompanyId == companyId);
			if (itemPackage != null)
			{
				return new ResponseDto() { Id = itemPackage.ItemPackageId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemPackageId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int code = 1;
			try { code = await _repository.GetAll().Where(x => x.CompanyId == companyId || x.CompanyId == null).MaxAsync(a => a.ItemPackageCode) + 1; } catch { code = 1; }
			return code;
		}

		public async Task<ResponseDto> CreateItemPackage(ItemPackageDto itemPackage)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var newItemPackage = new ItemPackage()
			{
				ItemPackageId = await GetNextId(),
				ItemPackageCode = await GetNextCode(companyId),
				PackageNameAr = itemPackage?.PackageNameAr?.Trim(),
				PackageNameEn = itemPackage?.PackageNameEn?.Trim(),
				PackageCode = itemPackage?.PackageCode?.Trim(),
                CompanyId = companyId,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false
			};

			var itemPackageValidator = await new ItemPackageValidator(_localizer).ValidateAsync(newItemPackage);
			var validationResult = itemPackageValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newItemPackage);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newItemPackage.ItemPackageId, Success = true, Message = _localizer["NewItemPackageSuccessMessage", (language == LanguageCode.Arabic ? newItemPackage.PackageNameAr : newItemPackage.PackageNameEn) ?? "", newItemPackage.ItemPackageCode] };
			}
			else
			{
				return new ResponseDto() { Id = newItemPackage.ItemPackageId, Success = false, Message = itemPackageValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateItemPackage(ItemPackageDto itemPackage)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemPackageDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemPackageId == itemPackage.ItemPackageId);
			if (itemPackageDb != null)
			{
				itemPackageDb.PackageNameAr = itemPackage.PackageNameAr?.Trim();
				itemPackageDb.PackageNameEn = itemPackage.PackageNameEn?.Trim();
				itemPackageDb.PackageCode = itemPackage.PackageCode?.Trim();
				itemPackageDb.ModifiedAt = DateHelper.GetDateTimeNow();
				itemPackageDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				itemPackageDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var itemPackageValidator = await new ItemPackageValidator(_localizer).ValidateAsync(itemPackageDb);
				var validationResult = itemPackageValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(itemPackageDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = itemPackageDb.ItemPackageId, Success = true, Message = _localizer["UpdateItemPackageSuccessMessage", (language == LanguageCode.Arabic ? itemPackageDb.PackageNameAr : itemPackageDb.PackageNameEn) ?? ""] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = itemPackageValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoItemPackageFound"] };
		}

		public async Task<ResponseDto> DeleteItemPackage(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var itemPackageDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ItemPackageId == id);
			if (itemPackageDb != null)
			{
				_repository.Delete(itemPackageDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteItemPackageSuccessMessage", (language == LanguageCode.Arabic ? itemPackageDb.PackageNameAr : itemPackageDb.PackageNameEn) ?? ""] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoItemPackageFound"] };
		}
	}
}
