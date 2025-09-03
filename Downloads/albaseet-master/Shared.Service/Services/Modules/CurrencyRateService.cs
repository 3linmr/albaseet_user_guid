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
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class CurrencyRateService : BaseService<CurrencyRate>, ICurrencyRateService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ICurrencyService _currencyService;
		private readonly IStringLocalizer<CurrencyRateService> _localizer;

		public CurrencyRateService(IRepository<CurrencyRate> repository, IHttpContextAccessor httpContextAccessor, ICurrencyService currencyService, IStringLocalizer<CurrencyRateService> localizer) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_currencyService = currencyService;
			_localizer = localizer;
		}

		public IQueryable<CurrencyRateDto> GetCurrencyRates()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				from currencyRate in _repository.GetAll()
				from fromCurrency in _currencyService.GetAll().Where(x => x.CurrencyId == currencyRate.FromCurrencyId)
				from toCurrency in _currencyService.GetAll().Where(x => x.CurrencyId == currencyRate.ToCurrencyId)
				select new CurrencyRateDto
				{
					CurrencyRateId = currencyRate.CurrencyRateId,
					FromCurrencyId = currencyRate.FromCurrencyId,
					ToCurrencyId = currencyRate.ToCurrencyId,
					CreatedAt = currencyRate.CreatedAt,
					ModifiedAt = currencyRate.ModifiedAt,
					UserNameCreated = currencyRate.UserNameCreated,
					UserNameModified = currencyRate.UserNameModified,
					CurrencyRateValue = currencyRate.CurrencyRateValue,
					FromCurrencyName = language == LanguageCode.Arabic ? fromCurrency.CurrencyNameAr : fromCurrency.CurrencyNameEn,
					ToCurrencyName = language == LanguageCode.Arabic ? toCurrency.CurrencyNameAr : toCurrency.CurrencyNameEn
				};
			return data;
		}

		public async Task<CurrencyRateDto> GetCurrencyRateData(int fromCurrencyId, int toCurrencyId)
		{
			return await GetCurrencyRates().FirstOrDefaultAsync(x => x.FromCurrencyId == fromCurrencyId && x.ToCurrencyId == toCurrencyId) ?? new CurrencyRateDto();
		}

		public async Task<decimal> GetCurrencyRate(int fromCurrencyId, int toCurrencyId)
		{
			if (fromCurrencyId == toCurrencyId)
			{
				return 1;
			}
			else
			{
				return await GetCurrencyRates().Where(x => x.FromCurrencyId == fromCurrencyId && x.ToCurrencyId == toCurrencyId).Select(x=>x.CurrencyRateValue).FirstOrDefaultAsync();
			}
		}

		public async Task<CurrencyRateDto?> GetCurrencyRateById(int currencyRateId)
		{
			return await GetCurrencyRates().FirstOrDefaultAsync(x => x.CurrencyRateId == currencyRateId);
		}

		public async Task<ResponseDto> SaveCurrencyRate(CurrencyRateDto currencyRate)
		{
			if (currencyRate.CurrencyRateId == 0)
			{

				return await CreateCurrencyRate(currencyRate);
			}
			else
			{
				return await UpdateCurrencyRate(currencyRate);
			}
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.CurrencyRateId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateCurrencyRate(CurrencyRateDto currencyRate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var newCurrencyRate = new CurrencyRate()
			{
				CurrencyRateId = await GetNextId(),
				FromCurrencyId = currencyRate.FromCurrencyId,
				ToCurrencyId = currencyRate.ToCurrencyId,
				CurrencyRateValue = currencyRate.CurrencyRateValue,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false
			};

			var currencyRateValidator = await new CurrencyRateValidator(_localizer).ValidateAsync(newCurrencyRate);
			var validationResult = currencyRateValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newCurrencyRate);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newCurrencyRate.CurrencyRateId, Success = true, Message = _localizer["NewCurrencyRateSuccessMessage"] };
			}
			else
			{
				return new ResponseDto() { Id = newCurrencyRate.CurrencyRateId, Success = false, Message = currencyRateValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateCurrencyRate(CurrencyRateDto currencyRate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var currencyRateDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CurrencyRateId == currencyRate.CurrencyRateId);
			if (currencyRateDb != null)
			{
				currencyRateDb.FromCurrencyId = currencyRate.FromCurrencyId;
				currencyRateDb.ToCurrencyId = currencyRate.ToCurrencyId;
				currencyRateDb.CurrencyRateValue = currencyRate.CurrencyRateValue;
				currencyRateDb.ModifiedAt = DateHelper.GetDateTimeNow();
				currencyRateDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				currencyRateDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var currencyRateValidator = await new CurrencyRateValidator(_localizer).ValidateAsync(currencyRateDb);
				var validationResult = currencyRateValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(currencyRateDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = currencyRateDb.CurrencyRateId, Success = true, Message = _localizer["UpdateCurrencyRateSuccessMessage"] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = currencyRateValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoCurrencyRateFound"] };
		}

		public async Task<ResponseDto> DeleteCurrencyRate(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var currencyRateDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CurrencyRateId == id);
			if (currencyRateDb != null)
			{
				_repository.Delete(currencyRateDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteCurrencyRateSuccessMessage"] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoCurrencyRateFound"] };
		}
	}
}
