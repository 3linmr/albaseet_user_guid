using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Basics
{
    public interface ICountryService : IBaseService<Country>
    {
        IQueryable<CountryDto> GetAllCountries();
        IQueryable<CountryDropDownDto> GetAllCountriesDropDown();
        Task<CountryDto?> GetCountryById(int id);
        Task<ResponseDto> SaveCountry(CountryDto country);
        Task<ResponseDto> DeleteCountry(int id);
    }
}
