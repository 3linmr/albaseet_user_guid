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
    public interface IDistrictService : IBaseService<District>
    {
        IQueryable<DistrictDto> GetAllDistricts();
        IQueryable<DistrictDto> GetAllDistrictsByCityId(int cityId);
        IQueryable<DistrictDropDownDto> GetDistrictsByCityIdIdDropDown(int cityId);
        Task<DistrictDto?> GetDistrictById(int id);
        Task<ResponseDto> SaveDistrict(DistrictDto district);
        Task<ResponseDto> DeleteDistrict(int id);
    }
}
