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
	public interface IJournalDetailService : IBaseService<JournalDetail>
	{
		Task<int> GetNextId();
		Task<List<JournalDetailDto>> GetJournalDetailByHeaderId(int headerId);
		Task<JournalDetailReturnDto> BuildJournalDetail(int journalHeaderId, int nextJournalDetailId,int serial, int? taxParentId, JournalDetailDto detailModel,bool hasCostCenter);
		Task<List<JournalDetail>> HandleDeletedJournalDetails(int journalHeaderId, List<JournalDetailDto>? journalDetail);
		Task<bool> CreateJournalDetail(List<JournalDetail> journalDetails);
		Task<bool> UpdateJournalDetail(List<JournalDetail> journalDetails);
		Task<bool> DeleteJournalDetail(List<JournalDetail> journalDetails);
		Task<bool> DeleteJournalDetail(int journalHeaderId);
		//Task<ResponseDto> SaveJournalDetail(int journalHeaderId, List<JournalDetailDto> detail);
	}
}
