using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
    public interface IClientQuotationDetailTaxService: IBaseService<ClientQuotationDetailTax>
    {
        IQueryable<ClientQuotationDetailTaxDto> GetClientQuotationDetailTaxes(int clientQuotationHeaderId);
        Task<bool> SaveClientQuotationDetailTaxes(int clientQuotationHeaderId, List<ClientQuotationDetailTaxDto> clientQuotationDetailTaxes);
        Task<bool> DeleteClientQuotationDetailTaxes(int clientQuotationHeaderId);
    }
}
