using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;

namespace Purchases.CoreOne.Contracts
{
    public interface ISupplierDebitMemoHandlingService
    {
        public List<RequestChangesDto> GetSupplierDebitMemoRequestChanges(SupplierDebitMemoVm oldItem, SupplierDebitMemoVm newItem);
        public Task<SupplierDebitMemoVm> GetSupplierDebitMemo(int supplierDebitMemoId);
        public Task<SupplierDebitMemoVm> GetSupplierDebitMemoFromPurchaseInvoice(int purchaseInvoiceHeaderId);
        public Task<ResponseDto> SaveSupplierDebitMemoInFull(SupplierDebitMemoVm supplierDebitMemo, bool hasApprove, bool approved, int? requestId);
        public Task<ResponseDto> DeleteSupplierDebitMemoInFull(int supplierDebitMemoId);
    }
}
