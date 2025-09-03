using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;

namespace Shared.CoreOne.Contracts.Journal
{
	public interface IJournalTypeService : IBaseService<JournalType>
	{
		IQueryable<JournalTypeDto> GetJournalTypes();
		Task<List<JournalTypeDropDownDto>> GetJournalTypesDropDown();
		Task<List<JournalTypeDropDownDto>> GetJournalTypesForJournalEntriesDropDown();
	}
}
