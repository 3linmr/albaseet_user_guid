using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Items
{
    public interface IItemPackageService : IBaseService<ItemPackage>
    {
        IQueryable<ItemPackageDto> GetAllItemPackages();
        IQueryable<ItemPackageDto> GetCompanyItemPackages();
        IQueryable<ItemPackageDropDownDto> GetItemPackagesDropDown();
        Task<ItemPackageDto?> GetItemPackageById(int id);
        Task<ResponseDto> SaveItemPackage(ItemPackageDto itemPackage);
        Task<ResponseDto> DeleteItemPackage(int id);
    }
}
