using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
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
    public class StateService : BaseService<State>, IStateService
    {
        private readonly ICountryService _countryService;
        private readonly IStringLocalizer<StateService> _localizer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StateService(IRepository<State> repository, ICountryService countryService, IStringLocalizer<StateService> localizer, IHttpContextAccessor httpContextAccessor) : base(repository)
        {
            _countryService = countryService;
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<StateDto> GetAllStates()
        {
            var data =
                from state in _repository.GetAll()
                from country in _countryService.GetAll().Where(x => x.CountryId == state.CountryId)
                select new StateDto()
                {
                    StateId = state.StateId,
                    CountryId = state.CountryId,
                    CountryNameAr = country.CountryNameAr,
                    CountryNameEn = country.CountryNameEn,
                    StateNameAr = state.StateNameAr,
                    StateNameEn = state.StateNameEn
                };
            return data;
        }

        public IQueryable<StateDto> GetAllStatesByCountryId(int countryId)
        {
            return GetAllStates().Where(x => x.CountryId == countryId);
        }

        public IQueryable<StateDropDownDto> GetStatesByCountryIdDropDown(int countryId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            return GetAllStatesByCountryId(countryId).Select(x => new StateDropDownDto()
            {
                StateId = x.StateId,
                StateName = language == LanguageData.LanguageCode.Arabic ? x.StateNameAr : x.StateNameEn,
            }).OrderBy(x=>x.StateName);
        }

        public Task<StateDto?> GetStateById(int id)
        {
            return GetAllStates().FirstOrDefaultAsync(x => x.StateId == id);
        }
        
        public async Task<ResponseDto> SaveState(StateDto state)
        {
            var countryExist = await IsStateExist(state.StateId, state.StateNameAr, state.StateNameEn);
            if (countryExist.Success)
            {
                return new ResponseDto() { Id = countryExist.Id, Success = false, Message = _localizer["StateAlreadyExist"] };
            }
            else
            {
                if (state.StateId == 0)
                {
                    return await CreateState(state);
                }
                else
                {
                    return await UpdateState(state);
                }
            }
        }

        public async Task<ResponseDto> IsStateExist(int id, string? nameAr, string? nameEn)
        {
            var state = await _repository.GetAll().FirstOrDefaultAsync(x => (x.StateNameAr == nameAr || x.StateNameEn == nameEn || x.StateNameAr == nameEn || x.StateNameEn == nameAr) && x.StateId != id);
            if (state != null)
            {
                return new ResponseDto() { Id = state.StateId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.StateId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> CreateState(StateDto state)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var newState = new State()
            {
                StateId = await GetNextId(),
                CountryId = state.CountryId,
                StateNameAr = state?.StateNameAr?.Trim(),
                StateNameEn = state?.StateNameEn?.Trim(),
                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false
            };

            var stateValidator = await new StateValidator(_localizer).ValidateAsync(newState);
            var validationResult = stateValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newState);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newState.StateId, Success = true, Message = _localizer["NewStateSuccessMessage", ((language == LanguageCode.Arabic ? newState.StateNameAr : newState.StateNameEn) ?? ""), newState.StateId] };
            }
            else
            {
                return new ResponseDto() { Id = newState.StateId, Success = false, Message = stateValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateState(StateDto state)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var stateDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.StateId == state.StateId);
            if (stateDb != null)
            {
                stateDb.CountryId = state.CountryId;
                stateDb.StateNameAr = state.StateNameAr?.Trim();
                stateDb.StateNameEn = state.StateNameEn?.Trim();
                stateDb.ModifiedAt = DateHelper.GetDateTimeNow();
                stateDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                stateDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

                var stateValidator = await new StateValidator(_localizer).ValidateAsync(stateDb);
                var validationResult = stateValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(stateDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = stateDb.StateId, Success = true, Message = _localizer["UpdateStateSuccessMessage", ((language == LanguageCode.Arabic ? stateDb.StateNameAr : stateDb.StateNameEn) ?? "")] };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = stateValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoStateFound"] };
        }

        public async Task<ResponseDto> DeleteState(int id)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var stateDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.StateId == id);
            if (stateDb != null)
            {
                _repository.Delete(stateDb);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteStateSuccessMessage", ((language == LanguageCode.Arabic ? stateDb.StateNameAr : stateDb.StateNameEn) ?? "")] };
            }
            return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoStateFound"] };
        }
    }
}
