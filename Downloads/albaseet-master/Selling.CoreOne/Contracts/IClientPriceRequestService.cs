using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;

namespace Sales.CoreOne.Contracts
{
    public interface IClientPriceRequestService
    {
        List<RequestChangesDto> GetClientPriceRequestRequestChanges(ClientPriceRequestDto oldItem, ClientPriceRequestDto newItem);
        Task<ClientPriceRequestDto> GetClientPriceRequest(int clientPriceRequestHeaderId);
        Task<ResponseDto> SaveClientPriceRequest(ClientPriceRequestDto clientPriceRequest, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteClientPriceRequest(int clientPriceRequestHeaderId);
    }
}
