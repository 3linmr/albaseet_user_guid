using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Journal
{
	public class JournalTypeService : BaseService<JournalType>, IJournalTypeService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public JournalTypeService(IRepository<JournalType> repository, IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public IQueryable<JournalTypeDto> GetJournalTypes()
		{
			return _repository.GetAll().Select(x => new JournalTypeDto()
			{
				JournalTypeId = x.JournalTypeId,
				JournalTypeNameAr = x.JournalTypeNameAr,
				JournalTypeNameEn = x.JournalTypeNameEn
			});
		}

		public Task<List<JournalTypeDropDownDto>> GetJournalTypesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetJournalTypes().Select(x => new JournalTypeDropDownDto()
			{
				JournalTypeId = x.JournalTypeId,
				JournalTypeName = language == LanguageCode.Arabic ? x.JournalTypeNameAr : x.JournalTypeNameEn
			}).ToListAsync();
		}

		public Task<List<JournalTypeDropDownDto>> GetJournalTypesForJournalEntriesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetJournalTypes().Where(x=>x.JournalTypeId == JournalTypeData.OpeningBalance || x.JournalTypeId == JournalTypeData.JournalEntry).Select(x => new JournalTypeDropDownDto()
			{
				JournalTypeId = x.JournalTypeId,
				JournalTypeName = language == LanguageCode.Arabic ? x.JournalTypeNameAr : x.JournalTypeNameEn
			}).ToListAsync();
		}
	}
}
