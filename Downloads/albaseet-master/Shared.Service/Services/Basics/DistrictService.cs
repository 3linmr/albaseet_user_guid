using System;
using System.Collections.Generic;
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
    public class DistrictService : BaseService<District>, IDistrictService
    {
        private readonly ICountryService _countryService;
        private readonly IStateService _stateService;
        private readonly ICityService _cityService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<DistrictService> _localizer;

        public DistrictService(IRepository<District> repository,ICountryService countryService,IStateService stateService,ICityService cityService,IHttpContextAccessor httpContextAccessor,IStringLocalizer<DistrictService>localizer) : base(repository)
        {
            _countryService = countryService;
            _stateService = stateService;
            _cityService = cityService;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
        }

        public IQueryable<DistrictDto> GetAllDistricts()
        {
            var data =
                from country in _countryService.GetAll()
                from state in _stateService.GetAll().Where(x => x.CountryId == country.CountryId)
                from city in _cityService.GetAll().Where(x => x.StateId == state.StateId)
                from district in _repository.GetAll().Where(x => x.CityId == city.CityId)
                select new DistrictDto()
                {
                    DistrictId = district.DistrictId,
                    DistrictNameAr = district.DistrictNameAr,
                    DistrictNameEn = district.DistrictNameEn,
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

        public IQueryable<DistrictDto> GetAllDistrictsByCityId(int cityId)
        {
            return GetAllDistricts().Where(x => x.CityId == cityId);
        }

        public IQueryable<DistrictDropDownDto> GetDistrictsByCityIdIdDropDown(int cityId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            return GetAllDistricts().Where(x => x.CityId == cityId).Select(x=> new DistrictDropDownDto()
            {
                DistrictId = x.DistrictId,
                DistrictName = language == LanguageData.LanguageCode.Arabic ? x.DistrictNameAr : x.DistrictNameEn,
            }).OrderBy(x => x.DistrictName);
        }

        public Task<DistrictDto?> GetDistrictById(int id)
        {
            return GetAllDistricts().FirstOrDefaultAsync(x => x.DistrictId == id);
        }

        public async Task<ResponseDto> SaveDistrict(DistrictDto district)
        {
            var districtExist = await IsDistrictExist(district.DistrictId, district.DistrictNameAr, district.DistrictNameEn);
            if (districtExist.Success)
            {
                return new ResponseDto() { Id = districtExist.Id, Success = false, Message = _localizer["CityAlreadyExist"] };
            }
            else
            {
                if (district.DistrictId == 0)
                {

                    return await CreateDistrict(district);
                }
                else
                {
                    return await UpdateDistrict(district);
                }
            }
        }

        public async Task<ResponseDto> IsDistrictExist(int id, string? nameAr, string? nameEn)
        {
            var district = await _repository.GetAll().FirstOrDefaultAsync(x => (x.DistrictNameAr == nameAr || x.DistrictNameEn == nameEn || x.DistrictNameAr == nameEn || x.DistrictNameEn == nameAr) && x.DistrictId != id);
            if (district != null)
            {
                return new ResponseDto() { Id = district.DistrictId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.DistrictId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> CreateDistrict(DistrictDto district)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var newDistrict = new District()
            {
                DistrictId = await GetNextId(),
                CityId = district.CityId,
                DistrictNameAr = district.DistrictNameAr?.Trim(),
                DistrictNameEn = district.DistrictNameEn?.Trim(),
                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false
            };

            var districtValidator = await new DistrictValidator(_localizer).ValidateAsync(newDistrict);
            var validationResult = districtValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newDistrict);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newDistrict.DistrictId, Success = true, Message = _localizer["NewDistrictSuccessMessage", ((language == LanguageCode.Arabic ? newDistrict.DistrictNameAr : newDistrict.DistrictNameEn) ?? ""), newDistrict.DistrictId] };
            }
            else
            {
                return new ResponseDto() { Id = newDistrict.DistrictId, Success = false, Message = districtValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateDistrict(DistrictDto district)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var districtDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.DistrictId == district.DistrictId);
            if (districtDb != null)
            {
                districtDb.DistrictNameAr = district.DistrictNameAr?.Trim();
                districtDb.DistrictNameEn = district.DistrictNameEn?.Trim();
                districtDb.CityId = district.CityId;
                districtDb.ModifiedAt = DateHelper.GetDateTimeNow();
                districtDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                districtDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

                var districtValidator = await new DistrictValidator(_localizer).ValidateAsync(districtDb);
                var validationResult = districtValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(districtDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = districtDb.DistrictId, Success = true, Message = _localizer["UpdateDistrictSuccessMessage", ((language == LanguageCode.Arabic ? districtDb.DistrictNameAr : districtDb.DistrictNameEn) ?? "")] };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = districtValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoDistrictFound"] };
        }

        public async Task<ResponseDto> DeleteDistrict(int id)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var districtDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.DistrictId == id);
            if (districtDb != null)
            {
                _repository.Delete(districtDb);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteDistrictSuccessMessage", ((language == LanguageCode.Arabic ? districtDb.DistrictNameAr : districtDb.DistrictNameEn) ?? "")] };
            }
            return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoDistrictFound"] };
        }
    }
}
