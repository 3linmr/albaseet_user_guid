using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class StoreClassificationService : BaseService<StoreClassification>, IStoreClassificationService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public StoreClassificationService(IRepository<StoreClassification> repository,IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public Task<List<StoreClassificationDto>> GetStoreClassifications()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return (_repository.GetAll().Select(x => new StoreClassificationDto()
			{
				StoreClassificationId = x.StoreClassificationId,
				StoreClassificationName =
					language == LanguageCode.Arabic ? x.ClassificationNameAr : x.ClassificationNameEn
			})).ToListAsync();
		}
	}
}
