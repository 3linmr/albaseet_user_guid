using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Models.Dtos;

namespace Accounting.CoreOne.Contracts
{
	public interface IReceiptVoucherDetailService : IBaseService<ReceiptVoucherDetail>
	{
		IQueryable<ReceiptVoucherDetailDto> GetAllReceiptVoucherDetail();
		Task<List<ReceiptVoucherDetailDto>> GetReceiptVoucherDetail(int headerId);
		Task<List<ReceiptVoucherDetailDto>> GetReceiptVoucherDetailsWithPaymentMethods(int storeId, int receiptVoucherHeaderId);
		Task<bool> SaveReceiptVoucherDetail(List<ReceiptVoucherDetailDto> details, int headerId);
		Task<bool> DeleteReceiptVoucherDetail(int headerId);
	}
}
