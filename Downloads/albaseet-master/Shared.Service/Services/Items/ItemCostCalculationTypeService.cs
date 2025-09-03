using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Items
{
	public class ItemCostCalculationTypeService : BaseService<ItemCostCalculationType>, IItemCostCalculationTypeService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ItemCostCalculationTypeService(IRepository<ItemCostCalculationType> repository,IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public IQueryable<ItemCostCalculationTypeDto> GetItemCostCalculationTypes()
		{
			return _repository.GetAll().Select(x => new ItemCostCalculationTypeDto()
			{
				ItemCostCalculationTypeId = x.ItemCostCalculationTypeId,
				ItemCostCalculationTypeNameAr = x.ItemCostCalculationTypeNameAr,
				ItemCostCalculationTypeNameEn = x.ItemCostCalculationTypeNameEn,
			});
		}

		public IQueryable<ItemCostCalculationTypeDropDownDto> GetItemCostCalculationTypesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return GetItemCostCalculationTypes().Select(x => new ItemCostCalculationTypeDropDownDto()
			{
				ItemCostCalculationTypeId = x.ItemCostCalculationTypeId,
				ItemCostCalculationTypeName = language == LanguageCode.Arabic ? x.ItemCostCalculationTypeNameAr : x.ItemCostCalculationTypeNameEn
			}).OrderBy(x => x.ItemCostCalculationTypeName);
		}
	}
}
