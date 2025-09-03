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
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Basics
{
	public class CountryService : BaseService<Country>, ICountryService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<CountryService> _localizer;
		private readonly ICurrencyService _currencyService;

		public CountryService(IRepository<Country> repository, IHttpContextAccessor httpContextAccessor, IStringLocalizer<CountryService> localizer, ICurrencyService currencyService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_currencyService = currencyService;
		}


		public IQueryable<CountryDto> GetAllCountries()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				from country in _repository.GetAll()
				from currency in _currencyService.GetAll().Where(x => x.CurrencyId == country.CurrencyId).DefaultIfEmpty()
				select new CountryDto()
				{
					CountryId = country.CountryId,
					CountryNameAr = country.CountryNameAr,
					CountryNameEn = country.CountryNameEn,
					CurrencyId = country.CurrencyId,
					CurrencyName = currency != null ? language == LanguageCode.Arabic ? currency.CurrencyNameAr : currency.CurrencyNameEn : "",
					CountryCode = country.CountryCode,
					PhoneCode = country.PhoneCode
				};
			return data;
		}

		public IQueryable<CountryDropDownDto> GetAllCountriesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = GetAllCountries().Select(x => new CountryDropDownDto()
			{
				CountryId = x.CountryId,
				CountryName = language == LanguageCode.Arabic ? x.CountryNameAr : x.CountryNameEn,
				CountryCode = x.CountryCode,
				PhoneCode = x.PhoneCode
			}).OrderBy(x => x.CountryName);
			return data;
		}

		public async Task<CountryDto?> GetCountryById(int id)
		{
			return await GetAllCountries().FirstOrDefaultAsync(x => x.CountryId == id);
		}

		public async Task<ResponseDto> SaveCountry(CountryDto country)
		{
			var countryExist = await IsCountryExist(country.CountryId, country.CountryNameAr, country.CountryNameEn);
			if (countryExist.Success)
			{
				return new ResponseDto() { Id = countryExist.Id, Success = false, Message = _localizer["CountryAlreadyExist"] };
			}
			else
			{
				if (country.CountryId == 0)
				{
					return await CreateCountry(country);
				}
				else
				{
					return await UpdateCountry(country);
				}
			}
		}

		public async Task<ResponseDto> IsCountryExist(int id, string? nameAr, string? nameEn)
		{
			var country = await _repository.GetAll().FirstOrDefaultAsync(x => (x.CountryNameAr == nameAr || x.CountryNameEn == nameEn || x.CountryNameAr == nameEn || x.CountryNameEn == nameAr) && x.CountryId != id);
			if (country != null)
			{
				return new ResponseDto() { Id = country.CountryId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.CountryId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateCountry(CountryDto country)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var newCountry = new Country()
			{
				CountryId = await GetNextId(),
				CountryNameAr = country?.CountryNameAr?.Trim(),
				CountryNameEn = country?.CountryNameEn?.Trim(),
				CurrencyId = country?.CurrencyId,
				CountryCode = country?.CountryCode,
				PhoneCode = country?.PhoneCode,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false
			};

			var countryValidator = await new CountryValidator(_localizer).ValidateAsync(newCountry);
			var validationResult = countryValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newCountry);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newCountry.CountryId, Success = true, Message = _localizer["NewCountrySuccessMessage", ((language == LanguageCode.Arabic ? newCountry.CountryNameAr : newCountry.CountryNameEn) ?? ""), newCountry.CountryId] };
			}
			else
			{
				return new ResponseDto() { Id = newCountry.CountryId, Success = false, Message = countryValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateCountry(CountryDto country)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var countryDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CountryId == country.CountryId);
			if (countryDb != null)
			{
				countryDb.CountryNameAr = country.CountryNameAr?.Trim();
				countryDb.CountryNameEn = country.CountryNameEn?.Trim();
				countryDb.CurrencyId = country?.CurrencyId;
				countryDb.CountryCode = country?.CountryCode;
				countryDb.PhoneCode = country?.PhoneCode;
				countryDb.ModifiedAt = DateHelper.GetDateTimeNow();
				countryDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				countryDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var countryValidator = await new CountryValidator(_localizer).ValidateAsync(countryDb);
				var validationResult = countryValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(countryDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = countryDb.CountryId, Success = true, Message = _localizer["UpdateCountrySuccessMessage", ((language == LanguageCode.Arabic ? countryDb.CountryNameAr : countryDb.CountryNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = countryValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoCountryFound"] };
		}

		public async Task<ResponseDto> DeleteCountry(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var countryDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CountryId == id);
			if (countryDb != null)
			{
				_repository.Delete(countryDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteCountrySuccessMessage", ((language == LanguageCode.Arabic ? countryDb.CountryNameAr : countryDb.CountryNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoCountryFound"] };
		}
	}
}
