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
    public interface ISupplierCreditMemoHandlingService
    {
        public List<RequestChangesDto> GetSupplierCreditMemoRequestChanges(SupplierCreditMemoVm oldItem, SupplierCreditMemoVm newItem);
        public Task<SupplierCreditMemoVm> GetSupplierCreditMemo(int supplierCreditMemoId);
        public Task<SupplierCreditMemoVm> GetSupplierCreditMemoFromPurchaseInvoice(int purchaseInvoiceHeaderId);
        public Task<ResponseDto> SaveSupplierCreditMemoInFull(SupplierCreditMemoVm supplierCreditMemo, bool hasApprove, bool approved, int? requestId);
        public Task<ResponseDto> DeleteSupplierCreditMemoInFull(int supplierCreditMemoId);
    }
}
