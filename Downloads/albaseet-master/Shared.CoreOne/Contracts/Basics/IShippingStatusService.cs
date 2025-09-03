using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Contracts.Basics
{
    public interface IShippingStatusService: IBaseService<ShippingStatus>
    {
        IQueryable<ShippingStatusDto> GetAllShippingStatuses();
        IQueryable<ShippingStatusDto> GetCompanyShippingStatuses();
        IQueryable<ShippingStatusDropDownDto> GetAllShippingStatusesDropDown(int menuCode);
        Task<ShippingStatusDto?> GetShippingStatusById(int id);
        Task<ResponseDto> SaveShippingStatus(ShippingStatusDto shippingStatus);
        Task<ResponseDto> DeleteShippingStatus(int id);
    }
}
