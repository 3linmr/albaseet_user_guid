using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Contracts.Items
{
	public interface IItemNoteValidationService
	{
		Task<ResponseDto> CheckItemNoteWithItemType<DetailType>(List<DetailType> details, Func<DetailType, int> itemIdSelector, Func<DetailType, string?> itemNoteSelector);
	}
}
