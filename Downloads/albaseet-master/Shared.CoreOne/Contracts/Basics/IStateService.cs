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
    public interface IStateService : IBaseService<State>
    {
        IQueryable<StateDto> GetAllStates();
        IQueryable<StateDto> GetAllStatesByCountryId(int countryId);
        IQueryable<StateDropDownDto> GetStatesByCountryIdDropDown(int countryId);
        Task<StateDto?> GetStateById(int id);
        Task<ResponseDto> SaveState(StateDto state);
        Task<ResponseDto> DeleteState(int id);
    }
}
