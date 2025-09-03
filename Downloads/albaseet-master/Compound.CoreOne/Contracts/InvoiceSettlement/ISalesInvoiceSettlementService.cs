using Accounting.CoreOne.Models.Dtos.ViewModels;
using Compound.CoreOne.Models.Domain.InvoiceSettlement;
using Compound.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.InvoiceSettlement
{
	public interface ISalesInvoiceSettlementService : IBaseService<SalesInvoiceSettlement>
    {
		Task<ResponseDto> IsSettlementOnInvoiceStarted(int salesInvoiceHeaderId);
		Task<decimal> GetSalesInvoiceSettledValue(int salesInvoiceHeaderId);
		Task<ReceiptVoucherDto> GetReceiptVoucherWithAllUnSettledSalesInvoices(int receiptVoucherHeaderId);
		Task<List<SalesInvoiceSettlementDto>> GetSalesInvoicesForReceiptVoucherRequest(int receiptVoucherHeaderId, int clientId, int storeId, List<SalesInvoiceSettlementDto> settlements, bool allInvoices);
		Task<ReceiptVoucherDto> GetReceiptVoucherWithSalesInvoices(int receiptVoucherHeaderId);
		IQueryable<SalesInvoiceSettlementDto> GetUnSettledInvoices(int? clientId, int storeId, int? exceptReceiptVoucherHeaderId = null, IEnumerable<int> salesInvoicesThatMustBeIncluded = null);
		Task<IQueryable<SalesInvoiceSettlementDto>> GetSalesInvoicesForReceiptVoucher(int receiptVoucherHeaderId, bool allInvoices = true);
		List<RequestChangesDto> GetRequestChangesWithSalesInvoiceSettlements(ReceiptVoucherDto oldItem, ReceiptVoucherDto newItem);
		Task<ResponseDto> SaveReceiptVoucherWithInvoiceSettlements(ReceiptVoucherDto receiptVoucher, bool hasApprove, bool approved, int? requestId);
		Task<ResponseDto> DeleteReceiptVoucherWithInvoiceSettlements(int receiptVoucherHeaderId);
        IQueryable<SalesInvoiceSettledValueDto> GetSalesInvoiceSettledValues();
		IQueryable<ReceiptVoucherSettledValueDto> GetReceiptVoucherSettledValues();
    }
}
