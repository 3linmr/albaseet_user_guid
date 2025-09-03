using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Taxes;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Items
{
	public interface IItemAttributeService :IBaseService<ItemAttribute>
	{
		IQueryable<ItemAttributeDto> GetAllItemAttributes();
		IQueryable<ItemAttributeDto> GetItemAttributesByItemId(int itemId);
		Task<ItemAttributeDto?> GetItemAttributeById(int id);
		Task<ResponseDto> SaveItemAttributes(List<ItemAttributeDto> attributes, int itemId);
		Task<ResponseDto> DeleteItemAttributesByItemId(int itemId);
	}
}
