using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;

namespace Accounting.CoreOne.Contracts
{
	public interface IPaymentVoucherHeaderService : IBaseService<PaymentVoucherHeader>
	{
		IQueryable<PaymentVoucherHeaderDto> GetPaymentVoucherHeaders();
		IQueryable<PaymentVoucherHeaderDto> GetUserPaymentVoucherHeaders();
		Task<PaymentVoucherHeaderDto> GetPaymentVoucherHeaderById(int headerId);
		Task<DocumentCodeDto> GetPaymentVoucherCode(int storeId, DateTime documentDate);
		Task<ResponseDto> SavePaymentVoucherHeader(PaymentVoucherHeaderDto paymentVoucherHeader, bool hasApprove, bool approved, int? requestId);
		Task<bool> UpdatePaymentVoucherWithJournalHeaderId(int paymentVoucherHeaderId, int journalHeaderId);
		Task<ResponseDto> DeletePaymentVoucherHeader(int id);
	}
}
