using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Dtos.ViewModels;

namespace Accounting.CoreOne.Contracts
{
	public interface IReceiptVoucherService
	{
		List<RequestChangesDto> GetReceiptVoucherRequestChanges(ReceiptVoucherDto oldItem, ReceiptVoucherDto newItem);
		Task<ReceiptVoucherDto> GetReceiptVoucher(int receiptVoucherHeaderId);
		Task<ResponseDto> SaveReceiptVoucher(ReceiptVoucherDto receiptVoucher, bool hasApprove, bool approved, int? requestId);
		Task<ResponseDto> DeleteReceiptVoucher(int receiptVoucherHeaderId);
	}
}
