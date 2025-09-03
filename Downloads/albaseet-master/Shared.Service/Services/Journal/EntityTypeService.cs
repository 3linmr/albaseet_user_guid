using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Journal;

namespace Shared.Service.Services.Journal
{
	public class EntityTypeService: BaseService<EntityType>, IEntityTypeService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public EntityTypeService(IRepository<EntityType> repository, IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public IQueryable<EntityTypeDto> GetEntityTypes()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return _repository.GetAll().Select(x => new EntityTypeDto
			{
				EntityTypeId = x.EntityTypeId,
				EntityTypeName = language == LanguageCode.Arabic ? x.EntityTypeNameAr : x.EntityTypeNameEn,
				Order = x.Order,
			});
		}
	}
}
