using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;

namespace Shared.CoreOne.Contracts.Items
{
	public interface IItemCostCalculationTypeService : IBaseService<ItemCostCalculationType>
	{
		IQueryable<ItemCostCalculationTypeDto> GetItemCostCalculationTypes();
		IQueryable<ItemCostCalculationTypeDropDownDto> GetItemCostCalculationTypesDropDown();
	}
}
