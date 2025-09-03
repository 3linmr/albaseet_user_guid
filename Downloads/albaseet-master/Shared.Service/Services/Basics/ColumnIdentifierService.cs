using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Menus;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Basics
{
	public class ColumnIdentifierService : BaseService<ColumnIdentifier>, IColumnIdentifierService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ColumnIdentifierService(IRepository<ColumnIdentifier> repository,IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public IQueryable<ColumnIdentifierDto> GetColumnIdentifiers()
		{
			var data = _repository.GetAll().Select(x => new ColumnIdentifierDto()
			{
				ColumnIdentifierId = x.ColumnIdentifierId,
				ColumnIdentifierNameAr = x.ColumnIdentifierNameAr,
				ColumnIdentifierNameEn = x.ColumnIdentifierNameEn
			});
			return data;
		}

		public IQueryable<ColumnIdentifierDropDownDto> GetColumnIdentifiersDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data = GetColumnIdentifiers().Select(x => new ColumnIdentifierDropDownDto()
			{
				ColumnIdentifierId = x.ColumnIdentifierId,
				ColumnIdentifierName = language == LanguageCode.Arabic ? x.ColumnIdentifierNameAr : x.ColumnIdentifierNameEn
			}).OrderBy(x => x.ColumnIdentifierId);
			return data;
		}
	}
}
