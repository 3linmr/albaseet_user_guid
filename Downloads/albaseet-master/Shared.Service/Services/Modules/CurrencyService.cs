using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class CurrencyService : BaseService<Currency>, ICurrencyService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<CurrencyService> _localizer;

		public CurrencyService(IRepository<Currency> repository,IHttpContextAccessor httpContextAccessor,IStringLocalizer<CurrencyService>localizer) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
		}

		public IQueryable<CurrencyDto> GetAllCurrencies()
		{
			return _repository.GetAll().Select(x => new CurrencyDto()
			{
				CurrencyId = x.CurrencyId,
				FractionalUnitAr = x.FractionalUnitAr,
				FractionalUnitEn = x.FractionalUnitEn,
				CurrencyNameAr = x.CurrencyNameAr,
				CurrencyNameEn = x.CurrencyNameEn,
				IsoCode = x.IsoCode,
				Symbol = x.Symbol,
				NumberToBasic = x.NumberToBasic
			});
		}

		public async Task<List<CurrencyDropDownDto>> GetCurrenciesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return await GetAllCurrencies().Select(x => new CurrencyDropDownDto()
			{
				CurrencyId = x.CurrencyId,
				CurrencyName = language == LanguageCode.Arabic ? x.CurrencyNameAr : x.CurrencyNameEn
			}).OrderBy(x => x.CurrencyName).ToListAsync();
		}

		public async Task<CurrencyDto?> GetCurrencyById(int id)
		{
			return await GetAllCurrencies().FirstOrDefaultAsync(x => x.CurrencyId == id);
		}

		public async Task<string?> GetCurrencySymbolById(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =  await GetCurrencyById(id);
			if (data != null)
			{
				if (!String.IsNullOrEmpty(data.Symbol))
				{
					return $"({data.Symbol})";

				}
				else if (!String.IsNullOrEmpty(data.IsoCode))
				{
					return $"({data.IsoCode})";
				}
				else
				{
					return language == LanguageCode.Arabic ? $"({data.CurrencyNameAr})" : $"({data.CurrencyNameEn})";
				}
			}
			return "";
		}

		public async Task<ResponseDto> SaveCurrency(CurrencyDto currency)
		{
			var currencyExist = await IsCurrencyExist(currency.CurrencyId, currency.CurrencyNameAr, currency.CurrencyNameEn);
			if (currencyExist.Success)
			{
				return new ResponseDto() { Id = currencyExist.Id, Success = false, Message = _localizer["CurrencyAlreadyExist"] };
			}
			else
			{
				if (currency.CurrencyId == 0)
				{
					return await CreateCurrency(currency);
				}
				else
				{
					return await UpdateCurrency(currency);
				}
			}
		}

		public async Task<ResponseDto> IsCurrencyExist(int id, string? nameAr, string? nameEn)
		{
			var currency = await _repository.GetAll().FirstOrDefaultAsync(x => (x.CurrencyNameAr == nameAr || x.CurrencyNameEn == nameEn || x.CurrencyNameAr == nameEn || x.CurrencyNameEn == nameAr) && x.CurrencyId != id);
			if (currency != null)
			{
				return new ResponseDto() { Id = currency.CurrencyId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<short> GetNextId()
		{
			var id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.CurrencyId) + 1; } catch { id = 1; }
			return (short)id;
		}

		public async Task<ResponseDto> CreateCurrency(CurrencyDto currency)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var newCurrency = new Currency()
			{
				CurrencyId = await GetNextId(),
				CurrencyNameAr = currency?.CurrencyNameAr?.Trim(),
				CurrencyNameEn = currency?.CurrencyNameEn?.Trim(),
				IsoCode = currency?.IsoCode?.Trim(),
				Symbol = currency?.Symbol?.Trim(),
				FractionalUnitAr = currency?.FractionalUnitAr?.Trim(),
				FractionalUnitEn = currency?.FractionalUnitEn?.Trim(),
				NumberToBasic = currency!.NumberToBasic,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var currencyValidator = await new CurrencyValidator(_localizer).ValidateAsync(newCurrency);
			var validationResult = currencyValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newCurrency);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newCurrency.CurrencyId, Success = true, Message = _localizer["NewCurrencySuccessMessage", ((language == LanguageCode.Arabic ? newCurrency.CurrencyNameAr : newCurrency.CurrencyNameEn) ?? ""), newCurrency.CurrencyId] };
			}
			else
			{
				return new ResponseDto() { Id = newCurrency.CurrencyId, Success = false, Message = currencyValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateCurrency(CurrencyDto currency)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var currencyDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CurrencyId == currency.CurrencyId);
			if (currencyDb != null)
			{
				currencyDb.CurrencyNameAr = currency.CurrencyNameAr?.Trim();
				currencyDb.CurrencyNameEn = currency.CurrencyNameEn?.Trim();
				currencyDb.IsoCode = currency?.IsoCode?.Trim();
				currencyDb.Symbol = currency?.Symbol?.Trim();
				currencyDb.FractionalUnitAr = currency?.FractionalUnitAr?.Trim();
				currencyDb.FractionalUnitEn = currency?.FractionalUnitEn?.Trim();
				currencyDb.NumberToBasic = currency!.NumberToBasic;
				currencyDb.ModifiedAt = DateHelper.GetDateTimeNow();
				currencyDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				currencyDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var currencyValidator = await new CurrencyValidator(_localizer).ValidateAsync(currencyDb);
				var validationResult = currencyValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(currencyDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = currencyDb.CurrencyId, Success = true, Message = _localizer["UpdateCurrencySuccessMessage", ((language == LanguageCode.Arabic ? currencyDb.CurrencyNameAr : currencyDb.CurrencyNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = currencyValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoCurrencyFound"] };
		}

		public async Task<ResponseDto> DeleteCurrency(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var currencyDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CurrencyId == id);
			if (currencyDb != null)
			{
				_repository.Delete(currencyDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteCurrencySuccessMessage", ((language == LanguageCode.Arabic ? currencyDb.CurrencyNameAr : currencyDb.CurrencyNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoCurrencyFound"] };
		}
	}
}
