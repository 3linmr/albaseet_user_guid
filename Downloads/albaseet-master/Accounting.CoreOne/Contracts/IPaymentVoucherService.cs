using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.CoreOne.Contracts
{
	public interface IPaymentVoucherService
	{
		List<RequestChangesDto> GetPaymentVoucherRequestChanges(PaymentVoucherDto oldItem, PaymentVoucherDto newItem);
		Task<PaymentVoucherDto> GetPaymentVoucher(int paymentVoucherHeaderId);
		Task<ResponseDto> SavePaymentVoucher(PaymentVoucherDto paymentVoucher, bool hasApprove, bool approved, int? requestId);
		Task<ResponseDto> DeletePaymentVoucher(int paymentVoucherHeaderId);

    }
}
