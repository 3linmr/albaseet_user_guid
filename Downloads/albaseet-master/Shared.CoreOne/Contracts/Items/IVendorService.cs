using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Items
{
    public interface IVendorService : IBaseService<Vendor>
    {
        IQueryable<VendorDto> GetAllVendors();
        IQueryable<VendorDto> GetCompanyVendors();
        IQueryable<VendorDropDownDto> GetVendorsDropDown();
        Task<VendorDto?> GetVendorById(int id);
        Task<ResponseDto> SaveVendor(VendorDto vendor);
        Task<ResponseDto> DeleteVendor(int id);
    }
}
