using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;

namespace Shared.CoreOne.Contracts.Items
{
	public interface IItemDivisionService
	{
		Task<ItemDivisionNamesDto> GetItemDivisions();
	}
}
