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
    public interface IClientQuotationHeaderService : IBaseService<ClientQuotationHeader>
    {
        IQueryable<ClientQuotationHeaderDto> GetClientQuotationHeaders();
        IQueryable<ClientQuotationHeaderDto> GetClientQuotationHeadersByStoreId(int storeId, int? clientId, int clientQuotationHeaderId);
        Task<ClientQuotationHeaderDto?> GetClientQuotationHeaderById(int id);
        IQueryable<ClientQuotationHeaderDto> GetUserClientQuotationHeaders();
        Task<DocumentCodeDto> GetClientQuotationCode(int storeId, DateTime documentDate);
        Task<ResponseDto> UpdateClosed(int? clientQuotationHeaderId, bool isClosed);
        Task<ResponseDto> SaveClientQuotationHeader(ClientQuotationHeaderDto clientQuotation, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteClientQuotationHeader(int id);
    }
}
