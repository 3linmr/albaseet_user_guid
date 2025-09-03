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
    public interface ICityService : IBaseService<City>
    {
        IQueryable<CityDto> GetAllCities();
        IQueryable<CityDto> GetAllCitiesByStateId(int stateId);
        IQueryable<CityDropDownDto> GetCitiesByStateIdDropDown(int stateId);
        Task<CityDto?> GetCityById(int id);
        Task<ResponseDto> SaveCity(CityDto city);
        Task<ResponseDto> DeleteCity(int id);
    }
}
