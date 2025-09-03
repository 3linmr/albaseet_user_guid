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
    public interface IClientQuotationApprovalDetailTaxService: IBaseService<ClientQuotationApprovalDetailTax>
    {
        IQueryable<ClientQuotationApprovalDetailTaxDto> GetClientQuotationApprovalDetailTaxes(int clientQuotationApprovalHeaderId);
        Task<bool> SaveClientQuotationApprovalDetailTaxes(int clientQuotationApprovalHeaderId, List<ClientQuotationApprovalDetailTaxDto> clientQuotationApprovalDetailTaxes);
        Task<bool> DeleteClientQuotationApprovalDetailTaxes(int clientQuotationApprovalHeaderId);
    }
}
