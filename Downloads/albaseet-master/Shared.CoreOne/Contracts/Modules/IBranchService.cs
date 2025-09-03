using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Shared;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Modules
{
	public interface IBranchService : IBaseService<Branch>
	{
		IQueryable<BranchDto> GetAllBranches();
		IQueryable<BranchDropDownDto> GetAllBranchesDropDown();
		Task<List<BranchDropDownDto>> GetUserBranchesDropDown();
		IQueryable<BranchDropDownDto> GetBranchesByCompanyIdDropDown(int companyId);
		IQueryable<NameDto> GetBranchesDropDownForAdmin(int companyId);
		Task<BranchDto?> GetBranchById(int id);
		Task<ResponseDto?> CreateBranchFromCompany(CompanyDto company,int companyId);
		Task<ResponseDto> SaveBranch(BranchDto branch);
		Task<ResponseDto> DeleteBranch(int id);
	}
}
