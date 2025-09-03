using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Basics
{
    public class CityService : BaseService<City>,ICityService
    {
        private readonly ICountryService _countryService;
        private readonly IStateService _stateService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<CityService> _localizer;

        public CityService(IRepository<City> repository,ICountryService countryService,IStateService stateService,IHttpContextAccessor httpContextAccessor,IStringLocalizer<CityService> localizer) : base(repository)
        {
            _countryService = countryService;
            _stateService = stateService;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
        }

        public IQueryable<CityDto> GetAllCities()
        {
            var data =
                from country in _countryService.GetAll() 
                from state in _stateService.GetAll().Where(x=>x.CountryId == country.CountryId)
                from city in _repository.GetAll().Where(x => x.StateId == state.StateId)
                select new CityDto()
                {
                    CityId = city.CityId,
                    CityNameAr = city.CityNameAr,
                    CityNameEn = city.CityNameEn,
                    CountryId = country.CountryId,
                    CountryNameAr = country.CountryNameAr,
                    CountryNameEn = country.CountryNameEn,
                    StateId = state.StateId,
                    StateNameAr = state.StateNameAr,
                    StateNameEn = state.StateNameEn
                };
            return data;
        }

        public IQueryable<CityDto> GetAllCitiesByStateId(int stateId)
        {
            return GetAllCities().Where(x => x.StateId == stateId);
        }

        public IQueryable<CityDropDownDto> GetCitiesByStateIdDropDown(int stateId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            return GetAllCities().Where(x => x.StateId == stateId).Select(x=> new CityDropDownDto()
            {
                CityId = x.CityId,
                CityName = language == LanguageData.LanguageCode.Arabic ? x.CityNameAr : x.CityNameEn,
            }).OrderBy(x=>x.CityName);
        }

        public Task<CityDto?> GetCityById(int id)
        {
            return GetAllCities().FirstOrDefaultAsync(x => x.CityId == id);
        }

        public async Task<ResponseDto> SaveCity(CityDto city)
        {
            var cityExist = await IsCityExist(city.CityId, city.CityNameAr, city.CityNameEn);
            if (cityExist.Success)
            {
                return new ResponseDto() { Id = cityExist.Id, Success = false, Message = _localizer["CityAlreadyExist"] };
            }
            else
            {
                if (city.CityId == 0)
                {

                    return await CreateCity(city);
                }
                else
                {
                    return await UpdateCity(city);
                }
            }
        }

        public async Task<ResponseDto> IsCityExist(int id, string? nameAr, string? nameEn)
        {
            var city = await _repository.GetAll().FirstOrDefaultAsync(x => (x.CityNameAr == nameAr || x.CityNameEn == nameEn || x.CityNameAr == nameEn || x.CityNameEn == nameAr) && x.CityId != id);
            if (city != null)
            {
                return new ResponseDto() { Id = city.CityId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.CityId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> CreateCity(CityDto city)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var newCity = new City()
            {
                CityId = await GetNextId(),
                StateId = city.StateId,
                CityNameAr = city.CityNameAr?.Trim(),
                CityNameEn = city.CityNameEn?.Trim(),
                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false
            };

            var cityValidator = await new CityValidator(_localizer).ValidateAsync(newCity);
            var validationResult = cityValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newCity);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newCity.CityId, Success = true, Message = _localizer["NewCitySuccessMessage", ((language == LanguageCode.Arabic ? newCity.CityNameAr : newCity.CityNameEn) ?? ""),newCity.CityId] };
            }
            else
            {
                return new ResponseDto() { Id = newCity.CityId, Success = false, Message = cityValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateCity(CityDto city)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var cityDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CityId == city.CityId);
            if (cityDb != null)
            {
                cityDb.CityNameAr = city.CityNameAr?.Trim();
                cityDb.CityNameEn = city.CityNameEn?.Trim();
                cityDb.StateId = city.StateId;
                cityDb.ModifiedAt = DateHelper.GetDateTimeNow();
                cityDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                cityDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

                var cityValidator = await new CityValidator(_localizer).ValidateAsync(cityDb);
                var validationResult = cityValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(cityDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = cityDb.CityId, Success = true, Message = _localizer["UpdateCitySuccessMessage", ((language == LanguageCode.Arabic ? cityDb.CityNameAr : cityDb.CityNameEn) ?? "")] };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = cityValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoCityFound"] };
        }

        public async Task<ResponseDto> DeleteCity(int id)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var cityDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.CityId == id);
            if (cityDb != null)
            {
                _repository.Delete(cityDb);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteCitySuccessMessage", ((language == LanguageCode.Arabic ? cityDb.CityNameAr : cityDb.CityNameEn) ?? "")] };
            }
            return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoCityFound"] };
        }
    }
}
