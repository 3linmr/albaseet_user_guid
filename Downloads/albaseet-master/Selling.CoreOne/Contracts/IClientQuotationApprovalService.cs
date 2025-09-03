using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
    public interface IClientQuotationApprovalService
    {
        List<RequestChangesDto> GetClientQuotationApprovalRequestChanges(ClientQuotationApprovalDto oldItem, ClientQuotationApprovalDto newItem);
        Task<ClientQuotationApprovalDto> GetClientQuotationApproval(int clientQuotationApprovalHeaderId);
        Task<ClientQuotationApprovalDto> GetClientQuotationApprovalFromClientQuotation(int clientQuotationHeaderId);
        Task<ResponseDto> SaveClientQuotationApproval(ClientQuotationApprovalDto clientQuotationApproval, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteClientQuotationApproval(int clientQuotationApprovalHeaderId);
    }
}
