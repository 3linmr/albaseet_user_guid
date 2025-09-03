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
    public interface ISupplierCreditMemoService: IBaseService<SupplierCreditMemo>
    {
        IQueryable<SupplierCreditMemoDto> GetSupplierCreditMemos();
        Task<SupplierCreditMemoDto?> GetSupplierCreditMemoById(int supplierCreditMemoId);
        IQueryable<SupplierCreditMemoDto> GetUserSupplierCreditMemos();
        IQueryable<SupplierCreditMemoDto> GetSupplierCreditMemosByStoreId(int storeId, int? supplierId, int suppierCreditMemoId);
        Task<DocumentCodeDto> GetSupplierCreditMemoCode(int storeId, DateTime documentDate);
        Task<ResponseDto> SaveSupplierCreditMemo(SupplierCreditMemoDto supplierCreditMemo, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteSupplierCreditMemo(int supplierCreditMemoId);
        Task<int> GetNextId();
    }
}
