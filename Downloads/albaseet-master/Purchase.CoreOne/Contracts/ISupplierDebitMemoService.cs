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
    public interface ISupplierDebitMemoService: IBaseService<SupplierDebitMemo>
    {
        IQueryable<SupplierDebitMemoDto> GetSupplierDebitMemos();
        Task<SupplierDebitMemoDto?> GetSupplierDebitMemoById(int supplierDebitMemoId);
        IQueryable<SupplierDebitMemoDto> GetUserSupplierDebitMemos();
        IQueryable<SupplierDebitMemoDto> GetSupplierDebitMemosByStoreId(int storeId, int? supplierId, int supplierDebitMemoId);
        Task<DocumentCodeDto> GetSupplierDebitMemoCode(int storeId, DateTime documentDate);
        Task<ResponseDto> SaveSupplierDebitMemo(SupplierDebitMemoDto supplierDebitMemo, bool hasApprove, bool approved, int? requestId);
        Task<ResponseDto> DeleteSupplierDebitMemo(int supplierDebitMemoId);
        Task<int> GetNextId();
    }
}
