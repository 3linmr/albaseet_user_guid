using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.CoreOne.Models;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.CoreOne.Contracts
{
    public interface IFixedAssetVoucherHeaderService : IBaseService<FixedAssetVoucherHeader>
    {
        Task<ResponseDto> SaveFixedAssetVoucherHeader(FixedAssetVoucherHeaderDto fixedAssetVoucher, bool hasApprove, bool approved, int? requestId);
		Task<bool> UpdateFixedAssetVoucherWithJournalHeaderId(int fixedAssetVoucherHeaderId, int journalHeaderId);
        Task<DateTime> GetFixedAssetVoucherMaxDate();
        Task<IQueryable<FixedAssetVoucherHeaderDto>> GetUserFixedAssetVoucherHeaders();
        //Task<DocumentCodeDto> GetFixedAssetVoucherCode(int storeId, DateTime documentDate);
        Task<FixedAssetVoucherHeaderDto> GetFixedAssetVoucherHeaderById(int headerId);
        Task<ResponseDto> DeleteFixedAssetVoucherHeader(int id);
        Task<string> GetNextFixedAssetAdditionCodeByStoreId(int storeId);
        Task<string> GetNextFixedAssetExclusionCodeByStoreId(int storeId);
    }
}
