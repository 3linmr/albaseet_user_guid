using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
    public interface IClientQuotationApprovalDetailService : IBaseService<ClientQuotationApprovalDetail>
    {
		IQueryable<ClientQuotationApprovalDetailDto> GetClientQuotationApprovalDetailsAsQueryable(int clientQuotationApprovalHeaderId);
		Task<List<ClientQuotationApprovalDetailDto>> GetClientQuotationApprovalDetails(int clientQuotationApprovalHeaderId);
        Task<List<ClientQuotationApprovalDetailDto>> SaveClientQuotationApprovalDetails(int clientQuotationApprovalHeaderId, List<ClientQuotationApprovalDetailDto> clientQuotationApprovalDetails);
        Task<bool> DeleteClientQuotationApprovalDetails(int clientQuotationApprovalHeaderId);
        Task<bool> DeleteClientQuotationApprovalDetailList(List<ClientQuotationApprovalDetailDto> details, int headerId);
    }
}
