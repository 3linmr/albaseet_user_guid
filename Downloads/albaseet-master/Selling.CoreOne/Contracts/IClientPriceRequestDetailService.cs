using Sales.CoreOne.Models.Domain;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.CoreOne.Models.Dtos.ViewModels;

namespace Sales.CoreOne.Contracts
{
    public interface IClientPriceRequestDetailService: IBaseService<ClientPriceRequestDetail>
    {
        Task<List<ClientPriceRequestDetailDto>> GetClientPriceRequestDetails(int clientPriceRequestHeaderId);
        Task<bool> SaveClientPriceRequestDetails(int clientPriceRequestHeaderId, List<ClientPriceRequestDetailDto> clientPriceRequestDetails);
        Task<bool> DeleteClientPriceRequestDetails(int clientPriceRequestHeaderId);
    }
}
