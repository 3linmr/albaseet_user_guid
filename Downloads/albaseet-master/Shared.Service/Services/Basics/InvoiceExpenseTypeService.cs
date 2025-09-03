using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Services.Approval;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.Helper.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Basics
{
	public class InvoiceExpenseTypeService : BaseService<InvoiceExpenseType>, IInvoiceExpenseTypeService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<InvoiceExpenseTypeService> _localizer;

		public InvoiceExpenseTypeService(IRepository<InvoiceExpenseType> repository, IHttpContextAccessor httpContextAccessor, IStringLocalizer<InvoiceExpenseTypeService> localizer) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
		}


		public IQueryable<InvoiceExpenseTypeDto> GetAllInvoiceExpenseTypes()
		{
			var data =
				from invoiceExpenseType in _repository.GetAll()
				select new InvoiceExpenseTypeDto()
				{
					InvoiceExpenseTypeId = invoiceExpenseType.InvoiceExpenseTypeId,
					InvoiceExpenseTypeCode = invoiceExpenseType.InvoiceExpenseTypeCode,
					InvoiceExpenseTypeNameAr = invoiceExpenseType.InvoiceExpenseTypeNameAr,
					InvoiceExpenseTypeNameEn = invoiceExpenseType.InvoiceExpenseTypeNameEn,
					CompanyId = invoiceExpenseType.CompanyId
				};
			return data;
		}

        public IQueryable<InvoiceExpenseTypeDto> GetCompanyInvoiceExpenseTypes()
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return GetAllInvoiceExpenseTypes().Where(x => x.CompanyId == companyId);
        }

        public IQueryable<InvoiceExpenseTypeDropDownDto> GetAllInvoiceExpenseTypesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = GetCompanyInvoiceExpenseTypes().Select(x => new InvoiceExpenseTypeDropDownDto()
			{
				InvoiceExpenseTypeId = x.InvoiceExpenseTypeId,
				InvoiceExpenseTypeName = language == LanguageCode.Arabic ? x.InvoiceExpenseTypeNameAr : x.InvoiceExpenseTypeNameEn
			}).OrderBy(x => x.InvoiceExpenseTypeName);
			return data;
		}

		public async Task<InvoiceExpenseTypeDto?> GetInvoiceExpenseTypeById(int id)
		{
			return await GetAllInvoiceExpenseTypes().FirstOrDefaultAsync(x => x.InvoiceExpenseTypeId == id);
		}

		public async Task<ResponseDto> SaveInvoiceExpenseType(InvoiceExpenseTypeDto invoiceExpenseType)
		{
			var invoiceExpenseTypeExist = await IsInvoiceExpenseTypeExist(invoiceExpenseType.InvoiceExpenseTypeId, invoiceExpenseType.InvoiceExpenseTypeNameAr, invoiceExpenseType.InvoiceExpenseTypeNameEn);
			if (invoiceExpenseTypeExist.Success)
			{
				return new ResponseDto() { Id = invoiceExpenseTypeExist.Id, Success = false, Message = _localizer["InvoiceExpenseTypeAlreadyExist"] };
			}
			else
			{
				if (invoiceExpenseType.InvoiceExpenseTypeId == 0)
				{
					return await CreateInvoiceExpenseType(invoiceExpenseType);
				}
				else
				{
					return await UpdateInvoiceExpenseType(invoiceExpenseType);
				}
			}
		}

		public async Task<ResponseDto> IsInvoiceExpenseTypeExist(int id, string? nameAr, string? nameEn)
		{
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var invoiceExpenseType = await _repository.GetAll().FirstOrDefaultAsync(x => (x.InvoiceExpenseTypeNameAr == nameAr || x.InvoiceExpenseTypeNameEn == nameEn || x.InvoiceExpenseTypeNameAr == nameEn || x.InvoiceExpenseTypeNameEn == nameAr) && x.InvoiceExpenseTypeId != id && x.CompanyId == companyId);
			if (invoiceExpenseType != null)
			{
				return new ResponseDto() { Id = invoiceExpenseType.InvoiceExpenseTypeId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.InvoiceExpenseTypeId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.InvoiceExpenseTypeCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateInvoiceExpenseType(InvoiceExpenseTypeDto invoiceExpenseType)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var newInvoiceExpenseType = new InvoiceExpenseType()
			{
				InvoiceExpenseTypeId = await GetNextId(),
				InvoiceExpenseTypeCode = await GetNextCode(companyId),
				InvoiceExpenseTypeNameAr = invoiceExpenseType?.InvoiceExpenseTypeNameAr?.Trim(),
				InvoiceExpenseTypeNameEn = invoiceExpenseType?.InvoiceExpenseTypeNameEn?.Trim(),
				CompanyId = companyId,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false
			};

			var invoiceExpenseTypeValidator = await new InvoiceExpenseTypeValidator(_localizer).ValidateAsync(newInvoiceExpenseType);
			var validationResult = invoiceExpenseTypeValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newInvoiceExpenseType);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newInvoiceExpenseType.InvoiceExpenseTypeId, Success = true, Message = _localizer["NewInvoiceExpenseTypeSuccessMessage", ((language == LanguageCode.Arabic ? newInvoiceExpenseType.InvoiceExpenseTypeNameAr : newInvoiceExpenseType.InvoiceExpenseTypeNameEn) ?? ""), newInvoiceExpenseType.InvoiceExpenseTypeCode] };
			}
			else
			{
				return new ResponseDto() { Id = newInvoiceExpenseType.InvoiceExpenseTypeId, Success = false, Message = invoiceExpenseTypeValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateInvoiceExpenseType(InvoiceExpenseTypeDto invoiceExpenseType)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var invoiceExpenseTypeDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.InvoiceExpenseTypeId == invoiceExpenseType.InvoiceExpenseTypeId);
			if (invoiceExpenseTypeDb != null)
			{
				invoiceExpenseTypeDb.InvoiceExpenseTypeNameAr = invoiceExpenseType.InvoiceExpenseTypeNameAr?.Trim();
				invoiceExpenseTypeDb.InvoiceExpenseTypeNameEn = invoiceExpenseType.InvoiceExpenseTypeNameEn?.Trim();
				invoiceExpenseTypeDb.ModifiedAt = DateHelper.GetDateTimeNow();
				invoiceExpenseTypeDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				invoiceExpenseTypeDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var invoiceExpenseTypeValidator = await new InvoiceExpenseTypeValidator(_localizer).ValidateAsync(invoiceExpenseTypeDb);
				var validationResult = invoiceExpenseTypeValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(invoiceExpenseTypeDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = invoiceExpenseTypeDb.InvoiceExpenseTypeId, Success = true, Message = _localizer["UpdateInvoiceExpenseTypeSuccessMessage", ((language == LanguageCode.Arabic ? invoiceExpenseTypeDb.InvoiceExpenseTypeNameAr : invoiceExpenseTypeDb.InvoiceExpenseTypeNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = invoiceExpenseTypeValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoInvoiceExpenseTypeFound"] };
		}

		public async Task<ResponseDto> DeleteInvoiceExpenseType(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var invoiceExpenseTypeDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.InvoiceExpenseTypeId == id);
			if (invoiceExpenseTypeDb != null)
			{
				_repository.Delete(invoiceExpenseTypeDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteInvoiceExpenseTypeSuccessMessage", ((language == LanguageCode.Arabic ? invoiceExpenseTypeDb.InvoiceExpenseTypeNameAr : invoiceExpenseTypeDb.InvoiceExpenseTypeNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoInvoiceExpenseTypeFound"] };
		}
	}
}
