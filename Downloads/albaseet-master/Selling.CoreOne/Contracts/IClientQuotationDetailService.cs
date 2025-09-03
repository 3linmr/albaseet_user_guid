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
    public interface IClientQuotationDetailService : IBaseService<ClientQuotationDetail>
    {
        Task<List<ClientQuotationDetailDto>> GetClientQuotationDetails(int clientQuotationHeaderId);
        Task<List<ClientQuotationDetailDto>> SaveClientQuotationDetails(int clientQuotationHeaderId, List<ClientQuotationDetailDto> clientQuotationDetails);
        Task<bool> DeleteClientQuotationDetails(int clientQuotationHeaderId);
        Task<bool> DeleteClientQuotationDetailList(List<ClientQuotationDetailDto> details, int headerId);
    }
}
