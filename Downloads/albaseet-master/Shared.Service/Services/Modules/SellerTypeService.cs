using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class SellerTypeService : BaseService<SellerType>, ISellerTypeService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public SellerTypeService(IRepository<SellerType> repository,IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public IQueryable<SellerTypeDto> GetSellerTypes()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return _repository.GetAll().Select(x => new SellerTypeDto()
			{
				SellerTypeId = x.SellerTypeId,
				SellerTypeName = language == LanguageCode.Arabic ? x.SellerTypeNameAr : x.SellerTypeNameEn
			});
		}
	}
}
