using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Modules
{
    public interface ISupplierService : IBaseService<Supplier>
    {
        IQueryable<SupplierDto> GetAllSuppliers();
        IQueryable<SupplierDto> GetUserSuppliers();
        IQueryable<SupplierDropDownDto> GetAllSuppliersDropDown();
        IQueryable<SupplierDropDownDto> GetSuppliersDropDownByCompanyId(int companyId);
        Task<IQueryable<SupplierDropDownDto>> GetSuppliersDropDownByStoreId(int storeId);
        Task<SupplierDto?> GetSupplierById(int id);
        Task<SupplierDto?> GetSupplierByAccountId(int id);
		Task<List<SupplierAutoCompleteDto>> GetSuppliersAutoComplete(int companyId, string term);
        Task<List<SupplierAutoCompleteDto>> GetSuppliersAutoCompleteByStoreIds(string term, List<int> storeIds);
        Task<ResponseDto> LinkWithSupplierAccount(AccountDto account, bool update);
        Task<bool> UnLinkWithSupplierAccount(int accountId);
		Task<ResponseDto> SaveSupplier(SupplierDto supplier);
        Task<ResponseDto> DeleteSupplier(int id);
    }
}
