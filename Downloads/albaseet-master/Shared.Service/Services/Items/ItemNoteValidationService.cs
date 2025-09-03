using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Shared.Service.Services.Items
{
	public class ItemNoteValidationService : IItemNoteValidationService
	{
		private IItemService _itemService;

		public ItemNoteValidationService(IItemService itemService)
		{
			_itemService = itemService;
		}

		public async Task<ResponseDto> CheckItemNoteWithItemType<DetailType>(List<DetailType> details, Func<DetailType,int> itemIdSelector, Func<DetailType, string?> itemNoteSelector)
		{
			var itemIds = details.Select(itemIdSelector).ToList();
			var itemTypes = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToDictionaryAsync(x => x.ItemId, x => x.ItemTypeId);

			foreach (var detail in details)
			{
				var isNoteType = itemTypes[itemIdSelector(detail)] == ItemTypeData.Note;
				var hasNote = itemNoteSelector(detail) != null;

				if (isNoteType && !hasNote)
				{
					return new ResponseDto { Success = false, Message = "Item with 'note' type must have non empty 'itemNotes' field" };
				}

				if (!isNoteType && hasNote)
				{
					return new ResponseDto { Success = false, Message = "Item without 'note' type must have empty 'itemNotes' field" };
				}
			}

			return new ResponseDto { Success = true };
		}
	}
}
