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
    public interface IProformaInvoiceDetailService : IBaseService<ProformaInvoiceDetail>
    {
        IQueryable<ProformaInvoiceDetailDto> GetProformaInvoiceDetailsAsQueryable(int proformaInvoiceHeaderId);
		Task<List<ProformaInvoiceDetailDto>> GetProformaInvoiceDetails(int proformaInvoiceHeaderId);
        IQueryable<ProformaInvoiceDetailDto> GetProformaInvoiceDetailsGroupedQueryable(int proformaInvoiceHeaderId);
		Task<List<ProformaInvoiceDetailDto>> GetProformaInvoiceDetailsGrouped(int proformaInvoiceHeaderId);
        Task<List<ProformaInvoiceDetailDto>> GetProformaInvoiceDetailsFullyGrouped(int proformaInvoiceHeaderId);
		List<ProformaInvoiceDetailDto> GroupProformaInvoiceDetails(List<ProformaInvoiceDetailDto> details);
		Task<List<ProformaInvoiceDetailDto>> SaveProformaInvoiceDetails(int proformaInvoiceHeaderId, List<ProformaInvoiceDetailDto> proformaInvoiceDetails);
        Task<bool> DeleteProformaInvoiceDetails(int proformaInvoiceHeaderId);
        Task<bool> DeleteProformaInvoiceDetailList(List<ProformaInvoiceDetailDto> details, int headerId);
    }
}
