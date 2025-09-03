using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Taxes;
using Shared.CoreOne.Models.Dtos.ViewModels.Taxes;

namespace Shared.CoreOne.Contracts.Taxes
{
	public interface ITaxTypeService : IBaseService<TaxType>
	{
		IQueryable<TaxTypeDto> GetTaxTypes();
		IQueryable<TaxTypeDropDownDto> GetAllTaxTypesDropDown();
		IQueryable<TaxTypeDropDownDto> GetLimitedTaxTypesDropDown();
	}
}
