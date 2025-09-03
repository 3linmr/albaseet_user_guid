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
    public interface IClientQuotationApprovalHeaderService : IBaseService<ClientQuotationApprovalHeader>
    {
        IQueryable<ClientQuotationApprovalHeaderDto> GetClientQuotationApprovalHeaders();
        IQueryable<ClientQuotationApprovalHeaderDto> GetClientQuotationApprovalHeadersByStoreId(int storeId, int? clientId, int clientQuotationApprovalHeaderId);
        Task<ClientQuotationApprovalHeaderDto?> GetClientQuotationApprovalHeaderById(int id);
        IQueryable<ClientQuotationApprovalHeaderDto> GetUserClientQuotationApprovalHeaders();
        Task<DocumentCodeDto> GetClientQuotationApprovalCode(int storeId, DateTime documentDate);
        Task<ResponseDto> UpdateClosed(int? clientQuotationApprovalHeaderId, bool isClosed);
        Task<ResponseDto> SaveClientQuotationApprovalHeader(ClientQuotationApprovalHeaderDto clientQuotationApproval, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteClientQuotationApprovalHeader(int id);
    }
}
