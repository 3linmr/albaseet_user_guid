using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Modules
{
	public interface ICurrencyRateService : IBaseService<CurrencyRate>
	{
		IQueryable<CurrencyRateDto> GetCurrencyRates();
		Task<CurrencyRateDto> GetCurrencyRateData(int fromCurrencyId, int toCurrencyId);
		Task<decimal> GetCurrencyRate(int fromCurrencyId, int toCurrencyId);
		Task<CurrencyRateDto?> GetCurrencyRateById(int currencyRateId);
		Task<ResponseDto> SaveCurrencyRate(CurrencyRateDto currencyRate);
		Task<ResponseDto> DeleteCurrencyRate(int id);
	}
}
