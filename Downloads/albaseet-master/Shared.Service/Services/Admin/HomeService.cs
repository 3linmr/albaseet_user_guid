using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Admin;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;

namespace Shared.Service.Services.Admin
{
    public class HomeService : IHomeService
    {
        private readonly ICompanyService _companyService;
        private readonly IBranchService _branchService;
        private readonly IStoreService _storeService;

        public HomeService(ICompanyService companyService,IBranchService branchService,IStoreService storeService)
        {
            _companyService = companyService;
            _branchService = branchService;
            _storeService = storeService;
        }
        public async Task<HomeSettingDto> GetHomeSetting()
        {
            var hasCompanies = await _companyService.IsSystemHasCompanies();
            if (hasCompanies)
            {
                var companies = await _companyService.GetAllUserCompaniesDropDown();
                var branches = await _branchService.GetUserBranchesDropDown();
                var stores = await _storeService.GetUserStoresDropDown();
                return new HomeSettingDto() { HasCompanies = hasCompanies, Companies = companies.OrderBy(x=>x.CompanyId).ToList(), Branches = branches.OrderBy(x=>x.BranchId).ToList(), Stores = stores.OrderBy(x=>x.StoreId).ToList() };
            }
            else
            {
                return new HomeSettingDto() { HasCompanies = hasCompanies };
            }
        }
    }
}
