using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
    public interface IProformaInvoiceService
    {
        List<RequestChangesDto> GetProformaInvoiceRequestChanges(ProformaInvoiceDto oldItem, ProformaInvoiceDto newItem);
        IQueryable<ProformaInvoiceHeaderDto> GetProformaInvoiceHeadersByStoreIdAndMenuCode(int storeId, int? clientId, int menuCode, int proformaInvoiceHeaderId);
        Task ModifyProformaInvoiceDetailsWithStoreIdAndAvaialbleBalance(int storeId, List<ProformaInvoiceDetailDto> details);
		Task<ProformaInvoiceDto> GetProformaInvoice(int proformaInvoiceHeaderId);
        Task<ProformaInvoiceDto> GetProformaInvoiceFromClientQuotationApproval(int clientQuotationApprovalHeaderId);
        Task<ResponseDto> UpdateBlocked(int proformaInvoiceHeaderId, bool isBlocked);
        Task<ResponseDto> CheckProformaInvoiceIsValidForSave(ProformaInvoiceDto proformaInvoice);
		Task<ResponseDto> SaveProformaInvoice(ProformaInvoiceDto proformaInvoice, bool hasApprove, bool approved, int? requestId, bool shouldValidate = true, string? documentReference = null, int documentStatusId = DocumentStatusData.ProformaInvoiceCreated, bool shouldInitializeFlags = false);
        Task<ResponseDto> CheckProformaInvoiceIsValidForDelete(int proformaInvoiceHeaderId);
		Task<ResponseDto> DeleteProformaInvoice(int proformaInvoiceHeaderId );
    }
}
