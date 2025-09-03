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
	public interface ICurrencyService : IBaseService<Currency>
	{
		IQueryable<CurrencyDto> GetAllCurrencies();
		Task<List<CurrencyDropDownDto>> GetCurrenciesDropDown();
		Task<CurrencyDto?> GetCurrencyById(int id);
		Task<string?> GetCurrencySymbolById(int id);
        Task<ResponseDto> SaveCurrency(CurrencyDto currency);
        Task<ResponseDto> DeleteCurrency(int id);
	}
}
