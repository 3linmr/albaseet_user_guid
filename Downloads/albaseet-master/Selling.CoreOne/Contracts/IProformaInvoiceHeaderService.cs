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
    public interface IProformaInvoiceHeaderService : IBaseService<ProformaInvoiceHeader>
    {
        IQueryable<ProformaInvoiceHeaderDto> GetProformaInvoiceHeaders();
        IQueryable<ProformaInvoiceHeaderDto> GetProformaInvoiceHeadersByStoreId(int storeId, int? clientId, int proformaInvoiceHeaderId);
        Task<ProformaInvoiceHeaderDto?> GetProformaInvoiceHeaderById(int id);
        IQueryable<ProformaInvoiceHeaderDto> GetUserProformaInvoiceHeaders();
        Task<DocumentCodeDto> GetProformaInvoiceCode(int storeId, DateTime documentDate);
        Task<bool> UpdateBlocked(int? proformaInvoiceHeaderId, bool isBlocked);
        Task<bool> UpdateClosed(int? proformaInvoiceHeaderId, bool isClosed);
        Task<bool> UpdateEnded(int? proformaInvoiceHeaderId, bool isEnded);
        Task<bool> UpdateEndedAndClosed(int? proformaInvoiceHeaderId, bool isEnded, bool isClosed);
        Task<bool> UpdateDocumentStatus(int proformaInvoiceHeaderId, int documentStatusId);
        Task<ResponseDto> UpdateShippingStatus(int proformaInvoiceHeaderId, int shippingStatusId);
        Task<ResponseDto> SaveProformaInvoiceHeader(ProformaInvoiceHeaderDto proformaInvoice, bool hasApprove, bool approved, int? requestId, bool shouldValidate, string? documentReference, int documentStatusId, bool shouldInitializeFlags);
        Task<ResponseDto> DeleteProformaInvoiceHeader(int id);
    }
}
