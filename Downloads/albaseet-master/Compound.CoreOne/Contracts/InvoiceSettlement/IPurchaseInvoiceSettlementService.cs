using Accounting.CoreOne.Models.Dtos.ViewModels;
using Compound.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Domain.InvoiceSettlement;

namespace Compound.CoreOne.Contracts.InvoiceSettlement
{
	public interface IPurchaseInvoiceSettlementService: IBaseService<PurchaseInvoiceSettlement>
	{
		Task<ResponseDto> IsSettlementOnInvoiceStarted(int purchaseInvoiceHeaderId);
		Task<decimal> GetPurchaseInvoiceSettledValue(int purchaseInvoiceHeaderId);
		IQueryable<PurchaseInvoiceSettledValueDto> GetPurchaseInvoiceSettledValues();
		IQueryable<PaymentVoucherSettledValueDto> GetPaymentVoucherSettledValues();
		Task<PaymentVoucherDto> GetPaymentVoucherWithAllUnSettledPurchaseInvoices(int paymentVoucherHeaderId);
		Task<List<PurchaseInvoiceSettlementDto>> GetPurchaseInvoicesForPaymentVoucherRequest(int paymentVoucherHeaderId, int supplierId, int storeId, List<PurchaseInvoiceSettlementDto> settlements, bool allInvoices);
		Task<PaymentVoucherDto> GetPaymentVoucherWithPurchaseInvoices(int paymentVoucherHeaderId);
		IQueryable<PurchaseInvoiceSettlementDto> GetUnSettledInvoices(int? supplierId, int storeId, int? exceptPaymentVoucherHeaderId = null, IEnumerable<int>? purchaseInvoicesThatMustBeIncluded = null);
		Task<IQueryable<PurchaseInvoiceSettlementDto>> GetPurchaseInvoicesForPaymentVoucher(int paymentVoucherHeaderId, bool allInvoices = true);
		List<RequestChangesDto> GetRequestChangesWithPurchaseInvoiceSettlements(PaymentVoucherDto oldItem, PaymentVoucherDto newItem);
		Task<ResponseDto> SavePaymentVoucherWithInvoiceSettlements(PaymentVoucherDto paymentVoucher, bool hasApprove, bool approved, int? requestId);
		Task<ResponseDto> DeletePaymentVoucherWithInvoiceSettlements(int paymentVoucherHeaderId);
	}
}
