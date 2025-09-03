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
	public interface IJournalHeaderService : IBaseService<JournalHeader>
	{
		IQueryable<JournalHeaderDto> GetJournalHeaders();
		IQueryable<JournalHeaderDto> GetUserJournalHeaders();
		Task<JournalHeaderDto> GetJournalHeader(int journalHeaderId);
		Task<DocumentCodeDto> GetJournalCode(int storeId, DateTime ticketDate);
		Task<ResponseDto> SaveJournalHeader(JournalHeaderDto journalHeader, bool hasApprove,bool approved,int? requestId);
		Task<ResponseDto> DeleteJournalHeader(int id);
	}
}
