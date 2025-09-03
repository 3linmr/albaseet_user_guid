using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Items
{
    public interface IItemAttributeTypeService : IBaseService<ItemAttributeType>
	{
		IQueryable<ItemAttributeTypeDto> GetItemAttributeTypes();
		IQueryable<ItemAttributeTypeDto> GetCompanyItemAttributeTypes();
		IQueryable<ItemAttributeTypeDropDownDto> GetItemAttributeTypesDropDown();
		Task<ItemAttributeTypeDto?> GetItemAttributeTypeById(int id);
		Task<ResponseDto> SaveItemAttributeType(ItemAttributeTypeDto itemAttributeType);
		Task<ResponseDto> DeleteItemAttributeType(int id);
	}
}
