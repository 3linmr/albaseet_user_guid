using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Models.StaticData;
using Shared.Helper.Models.UserDetail;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class CompanyService : BaseService<Company>, ICompanyService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<CompanyService> _localizer;
		private readonly ICurrencyService _currencyService;

		public CompanyService(IRepository<Company> repository, IHttpContextAccessor httpContextAccessor, IStringLocalizer<CompanyService> localizer, ICurrencyService currencyService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_currencyService = currencyService;
		}

		public async Task<bool> IsSystemHasCompanies()
		{
			return await _repository.GetAll().AnyAsync();
		}

		public IQueryable<CompanyDto> GetAllCompanies()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var active = _localizer["Active"].Value;
			var inActive = _localizer["InActive"].Value;
			var data =
				from company in _repository.GetAll()
				from currency in _currencyService.GetAll().Where(x => x.CurrencyId == company.CurrencyId)
				select new CompanyDto()
				{
					CompanyId = company.CompanyId,
					CompanyNameAr = company.CompanyNameAr,
					CompanyNameEn = company.CompanyNameEn,
					Phone = company.Phone,
					Website = company.Website,
					Address = company.Address,
					Email = company.Email,
					FooterUrl = company.FooterUrl,
					HeaderUrl = company.HeaderUrl,
					IsActive = company.IsActive,
					InActiveReasons = company.InActiveReasons,
					IsActiveName = company.IsActive ? active : inActive,
					LogoUrl = company.LogoUrl,
					TaxCode = company.TaxCode,
					WhatsApp = company.WhatsApp,
					CurrencyId = company.CurrencyId,
					CurrencyName = language == LanguageCode.Arabic ? currency.CurrencyNameAr : currency.CurrencyNameEn
				};
			return data;
		}

		public IQueryable<CompanyDropDownDto> GetAllCompaniesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetAllCompanies().Where(x => x.IsActive == true).Select(x => new CompanyDropDownDto()
			{
				CompanyId = x.CompanyId,
				CompanyName = language == LanguageCode.Arabic ? x.CompanyNameAr : x.CompanyNameEn
			}).OrderBy(x => x.CompanyName);
		}

		public IQueryable<NameDto> GetAllCompaniesForAdmin()
		{
			return GetAllCompaniesDropDown().Select(x => new NameDto()
			{
				Id = x.CompanyId,
				Name = x.CompanyName
			});
		}

		public async Task<List<CompanyDropDownDto>> GetAllUserCompaniesDropDown()
		{
			var userCompanies = await _httpContextAccessor.GetUserCompanies();
			var companies = await GetAllCompaniesDropDown().Where(x => userCompanies.Contains(x.CompanyId)).ToListAsync();
			return companies;
		}

		public async Task<bool> IsCompanyExist(int companyId)
		{
			return await _repository.GetAll().AnyAsync(x => x.CompanyId == companyId);
		}

		public async Task<ResponseDto> IsCompanyLimitReached()
		{
			var companyCount = await _repository.GetAll().CountAsync();
			var businessCount = await IdentityHelper.GetSubscriptionBusinessCount();
			if (businessCount.BusinessCount <= companyCount)
			{
				return new ResponseDto() { Success = true,Message = _localizer["CompanyLimitReached"] };
			}
			return new ResponseDto() { Success = false};
		}

		public Task<CompanyDto?> GetCompanyById(int id)
		{
			return GetAllCompanies().FirstOrDefaultAsync(x => x.CompanyId == id);
		}

		public async Task<ResponseDto> SaveCompany(CompanyDto company, bool fromSelfCreation)
		{
			var companyExist = await IsCompanyExist(company.CompanyId, company.CompanyNameAr, company.CompanyNameEn);
			if (companyExist.Success)
			{
				return new ResponseDto() { Id = companyExist.Id, Success = false, Message = _localizer["CompanyAlreadyExist"] };
			}
			else
			{
				if (company.CompanyId == 0)
				{
					return await CreateCompany(company,fromSelfCreation);
				}
				else
				{
					return await UpdateCompany(company);
				}
			}
		}

		public async Task<ResponseDto> IsCompanyExist(int id, string? nameAr, string? nameEn)
		{
			var company = await _repository.GetAll().FirstOrDefaultAsync(x => (x.CompanyNameAr == nameAr || x.CompanyNameEn == nameEn || x.CompanyNameAr == nameEn || x.CompanyNameEn == nameAr) && x.CompanyId != id);
			if (company != null)
			{
				return new ResponseDto() { Id = company.CompanyId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.CompanyId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateCompany(CompanyDto company, bool fromSelfCreation)
		{
			var companyCount = await _repository.GetAll().CountAsync();
			var businessCount = new SubscriptionBusinessCountDto();
			if (!fromSelfCreation)
			{ 
				businessCount = await IdentityHelper.GetSubscriptionBusinessCount();
			}
			if (businessCount.BusinessCount > companyCount || fromSelfCreation)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();
				var newCompany = new Company()
				{
					CompanyId = await GetNextId(),
					CompanyNameAr = company?.CompanyNameAr?.Trim(),
					CompanyNameEn = company?.CompanyNameEn?.Trim(),
					CurrencyId = company!.CurrencyId,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor!.GetUserName(),
					IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
					Phone = company?.Phone,
					WhatsApp = company?.WhatsApp,
					Address = company?.Address,
					Email = company?.Email,
					Website = company?.Website,
					TaxCode = company?.TaxCode,
					Hide = false,
					IsActive = company!.IsActive,
					InActiveReasons = company?.InActiveReasons,
					HeaderUrl = null,
					FooterUrl = null,
					LogoUrl = null
				};

				var companyValidator = await new CompanyValidator(_localizer).ValidateAsync(newCompany);
				var validationResult = companyValidator.IsValid;
				if (validationResult)
				{
					await _repository.Insert(newCompany);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = newCompany.CompanyId, Success = true, Message = _localizer["NewCompanySuccessMessage", ((language == LanguageCode.Arabic ? newCompany.CompanyNameAr : newCompany.CompanyNameEn) ?? ""), newCompany.CompanyId] };
				}
				else
				{
					return new ResponseDto() { Id = newCompany.CompanyId, Success = false, Message = companyValidator.ToString("~") };
				}
			}
			else
			{
				return new ResponseDto() { Id = 0, Success = false, Message = _localizer["CompanyLimitReached"] };
			}
		}

		public async Task<ResponseDto> UpdateCompany(CompanyDto company)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var companyDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CompanyId == company.CompanyId);
			if (companyDb != null)
			{
				companyDb.CompanyNameAr = company.CompanyNameAr?.Trim();
				companyDb.CompanyNameEn = company.CompanyNameEn?.Trim();
				companyDb.CurrencyId = company!.CurrencyId;
				companyDb.Phone = company?.Phone;
				companyDb.WhatsApp = company?.WhatsApp;
				companyDb.Address = company?.Address;
				companyDb.Email = company?.Email;
				companyDb.Website = company?.Website;
				companyDb.TaxCode = company?.TaxCode;
				companyDb.Hide = false;
				companyDb.IsActive = (bool)company?.IsActive;
				companyDb.InActiveReasons = company?.InActiveReasons;
				companyDb.HeaderUrl = null;
				companyDb.FooterUrl = null;
				companyDb.LogoUrl = null;
				companyDb.ModifiedAt = DateHelper.GetDateTimeNow();
				companyDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				companyDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var companyValidator = await new CompanyValidator(_localizer).ValidateAsync(companyDb);
				var validationResult = companyValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(companyDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = companyDb.CompanyId, Success = true, Message = _localizer["UpdateCompanySuccessMessage", ((language == LanguageCode.Arabic ? companyDb.CompanyNameAr : companyDb.CompanyNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = companyValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoCompanyFound"] };
		}

		public async Task<ResponseDto> DeleteCompany(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var companyDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CompanyId == id);
			if (companyDb != null)
			{
				_repository.Delete(companyDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteCompanySuccessMessage", ((language == LanguageCode.Arabic ? companyDb.CompanyNameAr : companyDb.CompanyNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoCompanyFound"] };
		}
	}
}
