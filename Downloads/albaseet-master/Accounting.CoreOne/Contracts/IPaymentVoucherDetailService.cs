using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;

namespace Accounting.CoreOne.Contracts
{
	public interface IPaymentVoucherDetailService : IBaseService<PaymentVoucherDetail>
	{
		IQueryable<PaymentVoucherDetailDto> GetAllPaymentVoucherDetail();
		Task<List<PaymentVoucherDetailDto>> GetPaymentVoucherDetail(int headerId);
		Task<List<PaymentVoucherDetailDto>> GetPaymentVoucherDetailsWithPaymentMethods(int storeId, int paymentVoucherHeaderId);
		Task<bool> SavePaymentVoucherDetail(List<PaymentVoucherDetailDto> details, int headerId);
		Task<bool> DeletePaymentVoucherDetail(int headerId);
	}
}
