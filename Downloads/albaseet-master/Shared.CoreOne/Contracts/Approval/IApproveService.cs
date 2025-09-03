using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Menus;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Approval
{
    public interface IApproveService : IBaseService<Approve>
    {
	    IQueryable<ApproveDto> GetAllApproves();
	    IQueryable<ApproveDto> GetCompanyApproves();
        Task<ApproveDto> GetApproveById(int id);
        Task<List<MenuCodeDropDownDto>> GetMenusForApprovesDropDown();
		Task<ResponseDto> SaveApprove(ApproveDto approve);
        ResponseDto ValidateApproveStatus(ApproveDefinitionDto approve);
        Task<ResponseDto> DeleteApprove(int id);
    }
}
