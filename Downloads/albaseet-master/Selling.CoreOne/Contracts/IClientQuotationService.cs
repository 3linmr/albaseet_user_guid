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
    public interface IClientQuotationService
    {
        List<RequestChangesDto> GetClientQuotationRequestChanges(ClientQuotationDto oldItem, ClientQuotationDto newItem);
        Task<ClientQuotationDto> GetClientQuotation(int clientQuotationHeaderId);
        Task<ClientQuotationDto> GetClientQuotationFromClientPriceRequest(int clientPriceRequestHeaderId);
        Task<ExpirationDaysAndDateDto> GetDefaultValidDate(int storeId);
        Task<ResponseDto> SaveClientQuotation(ClientQuotationDto clientQuotation, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteClientQuotation(int clientQuotationHeaderId);
    }
}
