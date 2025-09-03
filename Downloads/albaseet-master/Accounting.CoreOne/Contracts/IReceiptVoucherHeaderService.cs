using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Helper.Models.Dtos;

namespace Accounting.CoreOne.Contracts
{
	public interface IReceiptVoucherHeaderService : IBaseService<ReceiptVoucherHeader>
	{
		IQueryable<ReceiptVoucherHeaderDto> GetReceiptVoucherHeaders();
		IQueryable<ReceiptVoucherHeaderDto> GetUserReceiptVoucherHeaders();
		Task<ReceiptVoucherHeaderDto> GetReceiptVoucherHeaderById(int headerId);
		Task<DocumentCodeDto> GetReceiptVoucherCode(int storeId, DateTime documentDate);
		Task<ResponseDto> SaveReceiptVoucherHeader(ReceiptVoucherHeaderDto receiptVoucherHeader, bool hasApprove, bool approved, int? requestId);
		Task<bool> UpdateReceiptVoucherWithJournalHeaderId(int receiptVoucherHeaderId,int journalHeaderId);
		Task<ResponseDto> DeleteReceiptVoucherHeader(int id);
	}
}
