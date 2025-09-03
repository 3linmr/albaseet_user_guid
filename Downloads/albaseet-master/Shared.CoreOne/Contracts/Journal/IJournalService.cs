using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Journal
{
	public interface IJournalService
	{
		List<RequestChangesDto> GetJournalEntryRequestChanges(JournalDto oldItem, JournalDto newItem);
		Task<JournalDto> GetJournal(int journalHeaderId);
		Task<ResponseDto> CheckExistingTaxReference(string taxReference, DateTime ticketDate);
		Task<ResponseDto> CheckExistingTaxReferences(List<string> taxReferences, DateTime ticketDate);
		Task<ResponseDto> SaveJournal(JournalDto journal, bool hasApprove, bool approved,int? requestId);
		Task<ResponseDto> DeleteJournal(int journalHeaderId);
	}
}
