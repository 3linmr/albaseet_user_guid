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
    public interface IClientPriceRequestHeaderService: IBaseService<ClientPriceRequestHeader>
    {
        IQueryable<ClientPriceRequestHeaderDto> GetClientPriceRequestHeaders();
        IQueryable<ClientPriceRequestHeaderDto> GetUserClientPriceRequestHeaders();
        IQueryable<ClientPriceRequestHeaderDto> GetClientPriceRequestHeadersByStoreId(int storeId, int? clientId, int clientPriceRequestHeaderId);
        Task<ClientPriceRequestHeaderDto?> GetClientPriceRequestHeaderById(int id);
        Task<DocumentCodeDto> GetClientPriceRequestCode(int storeId, DateTime documentDate);
        Task<ResponseDto> UpdateClosed(int? clientPriceRequestHeaderId, bool isClosed);
        Task<ResponseDto> SaveClientPriceRequestHeader(ClientPriceRequestHeaderDto clientPriceRequest, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteClientPriceRequestHeader(int id);
    }
}
