using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Journal
{
	public interface ICostCenterJournalDetailService : IBaseService<CostCenterJournalDetail>
	{
		Task<int> GetNextId();
		Task<List<CostCenterJournalDetailDto>> GetCostCenterJournalDetails(int journalHeaderId);
		Task<CostCenterJournalDetailReturnDto?> BuildCostCenterJournalDetail(int journalDetailId, int nextCostCenterJournalDetailId, List<CostCenterJournalDetailDto> details);
		Task<List<CostCenterJournalDetail>> HandleDeletedCostCenterJournalDetails(int journalHeaderId, List<CostCenterJournalDetailDto>? costCenterJournalDetail);
		Task<bool> CreateCostCenterJournalDetail(List<CostCenterJournalDetail> journalDetails);
		Task<bool> UpdateCostCenterJournalDetail(List<CostCenterJournalDetail> journalDetails);
		Task<bool> DeleteCostCenterJournalDetail(List<CostCenterJournalDetail> journalDetails);
		Task<bool> DeleteCostCenterJournalDetail(int journalHeaderId);
        Task<List<CostCenterJournalDetailDto>> GetCostCenterJournalDetails(int referenceHeaderId, short? menuCode);
        Task<bool> SaveCostCenterJournalDetails(int referenceHeaderId, List<CostCenterJournalDetailDto> costCenterJournalDetails, short? menuCode);
        Task<bool> DeleteCostCenterJournalDetails(int referenceHeaderId, short? menuCode);
		Task UpdateCostCenterJournalDetailsBasedOnInvoiceDetails<DetailType>(int invoiceHeaderId, List<DetailType> invoiceDetails, Func<DetailType, int> detailIdSelector, Func<DetailType, int> itemIdSelector, Func<DetailType, int?> costCenterIdSelector, Func<DetailType, decimal> creditValueSelector, Func<DetailType, decimal> debitValueSelector, Func<DetailType, string?> remarksSelector, short menuCode);
    }
}
