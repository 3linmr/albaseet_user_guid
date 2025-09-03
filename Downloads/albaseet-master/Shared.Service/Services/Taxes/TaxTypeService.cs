using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.Domain.Taxes;
using Shared.CoreOne.Models.Dtos.ViewModels.Taxes;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Taxes
{
	public class TaxTypeService : BaseService<TaxType>, ITaxTypeService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public TaxTypeService(IRepository<TaxType> repository,IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public IQueryable<TaxTypeDto> GetTaxTypes()
		{
			return _repository.GetAll().Select(x => new TaxTypeDto()
			{
				TaxTypeId = x.TaxTypeId,
				TaxTypeNameAr = x.TaxTypeNameAr,
				TaxTypeNameEn = x.TaxTypeNameEn
			});
		}

		public IQueryable<TaxTypeDropDownDto> GetAllTaxTypesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetTaxTypes().Select(x => new TaxTypeDropDownDto()
			{
				TaxTypeId = x.TaxTypeId,
				TaxTypeName = language == LanguageCode.Arabic ? x.TaxTypeNameAr : x.TaxTypeNameEn
			}).OrderBy(x => x.TaxTypeName);
		}

		public IQueryable<TaxTypeDropDownDto> GetLimitedTaxTypesDropDown()
		{
			return GetAllTaxTypesDropDown().Where(x => x.TaxTypeId <= 3);
		}
	}
}
