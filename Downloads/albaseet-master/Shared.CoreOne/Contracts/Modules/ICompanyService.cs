using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Modules
{
	public interface ICompanyService : IBaseService<Company>
	{
        Task<bool> IsSystemHasCompanies();
        IQueryable<CompanyDto> GetAllCompanies();
		IQueryable<CompanyDropDownDto> GetAllCompaniesDropDown();
		IQueryable<NameDto> GetAllCompaniesForAdmin();
		Task<List<CompanyDropDownDto>> GetAllUserCompaniesDropDown();
		Task<bool> IsCompanyExist(int companyId);
		Task<ResponseDto> IsCompanyLimitReached();
		Task<CompanyDto?> GetCompanyById(int id);
		Task<ResponseDto> SaveCompany(CompanyDto company, bool fromSelfCreation);
		Task<ResponseDto> DeleteCompany(int id);
	}
}
