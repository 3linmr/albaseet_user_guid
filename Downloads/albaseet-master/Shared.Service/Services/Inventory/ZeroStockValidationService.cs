using AutoMapper.Configuration.Annotations;
using Microsoft.EntityFrameworkCore;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Items;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Inventory
{
	public class ZeroStockValidationService: IZeroStockValidationService
	{
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
		private readonly IGenericMessageService _genericMessageService;
		private readonly IZeroStockSettingService _zeroStockSettingService;
		private readonly IItemService _itemService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ZeroStockValidationService(IItemCurrentBalanceService itemCurrentBalanceService, IGenericMessageService genericMessageService, IZeroStockSettingService zeroStockSettingService, IItemService itemService, IHttpContextAccessor httpContextAccessor)
		{
			_itemCurrentBalanceService = itemCurrentBalanceService;
			_genericMessageService = genericMessageService;
			_zeroStockSettingService = zeroStockSettingService;
			_itemService = itemService;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<ResponseDto> ValidateZeroStock<DetailType, KeyType>(int storeId, List<DetailType> newDetails, List<DetailType> oldDetails, Func<DetailType, KeyType> detailKeySelector, Func<KeyType, int> itemIdSelector, Func<DetailType, decimal> quantitySelector, Func<ItemCurrentBalanceDto, KeyType> availableBalanceKeySelector, bool isGrouped, int menuCode, int settingMenuCode, bool isSave) where KeyType : struct 
		{
			var sellWithZeroStockFlag = await _zeroStockSettingService.GetZeroStockSettingByMenuCode(settingMenuCode, storeId);
			if (sellWithZeroStockFlag) return new ResponseDto { Success = true };

			List<int> affectedItemIds = newDetails.Select(x => itemIdSelector(detailKeySelector(x))).Union(oldDetails.Select(x => itemIdSelector(detailKeySelector(x)))).ToList();
			Dictionary<int, ItemDto> affectedItemsData = await _itemService.GetAllItems().Where(x => affectedItemIds.Contains(x.ItemId)).ToDictionaryAsync(x => x.ItemId);

			var availableBalances = await _itemCurrentBalanceService.GetItemCurrentBalanceInfo(storeId, false, isGrouped).Where(x => x.StoreId == storeId && affectedItemIds.Contains(x.ItemId)).ToListAsync();

			var validationResult = ValidateZeroStockInternal(newDetails, oldDetails, detailKeySelector, quantitySelector, x => affectedItemsData[itemIdSelector(detailKeySelector(x))].ItemTypeId == ItemTypeData.Goods, availableBalances, availableBalanceKeySelector);

			if (validationResult != null)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();
				var negativeItem = affectedItemsData[itemIdSelector((KeyType)validationResult)];
				var negativeItemName = language == LanguageCode.Arabic ? negativeItem.ItemNameAr : negativeItem.ItemNameEn;

				return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, isSave ? GenericMessageData.CannotSaveBecauseNegativeBalance : GenericMessageData.CannotDeleteBecauseNegativeBalance, negativeItemName ?? "") };
			}

			return new ResponseDto { Success = true };
		}

		public async Task<ResponseDto> ValidateZeroStockReturn<DetailType, KeyType>(int storeId, List<DetailType> newDetails, List<DetailType> oldDetails, Func<DetailType, KeyType> detailKeySelector, Func<KeyType, int> itemIdSelector, Func<DetailType, decimal> quantitySelector, Func<ItemCurrentBalanceDto, KeyType> availableBalanceKeySelector, bool isGrouped, int menuCode, int settingMenuCode, bool isSave) where KeyType : struct
		{
			//reverse the newDetails and oldDetails parameters
			return await ValidateZeroStock(storeId, oldDetails, newDetails, detailKeySelector, itemIdSelector, quantitySelector, availableBalanceKeySelector, isGrouped, menuCode, settingMenuCode, isSave);
		}

		public static KeyType? ValidateZeroStockInternal<DetailType, KeyType>(List<DetailType> newDetails, List<DetailType> oldDetails, Func<DetailType, KeyType> detailKeySelector, Func<DetailType, decimal> quantitySelector, Func<DetailType, bool> isItemTypeStockSelector, List<ItemCurrentBalanceDto> availableBalances, Func<ItemCurrentBalanceDto, KeyType> availableBalanceKeySelector) where KeyType: struct
		{
			//formula: detailChanges = newDetails - oldDetails
			var filteredNewDetails = newDetails.Where(isItemTypeStockSelector).Select(x => new { Key = detailKeySelector(x), Quantity = +quantitySelector(x) });
			var filteredOldDetails = oldDetails.Where(isItemTypeStockSelector).Select(x => new { Key = detailKeySelector(x), Quantity = -quantitySelector(x) });

			var detailChanges = filteredNewDetails.Concat(filteredOldDetails).GroupBy(x => x.Key).Select(x => new {x.Key, Quantity = x.Sum(y => y.Quantity)});

			var finalBalances = from detailChange in detailChanges
								from availableBalance in availableBalances.Where(x => availableBalanceKeySelector(x).Equals(detailChange.Key)).DefaultIfEmpty()
								select new
								{
									Key = detailChange.Key,
									FinalAvailableBalance = (availableBalance?.AvailableBalance ?? 0) - detailChange.Quantity,
								};

			var negativeBalanceKey = finalBalances.Where(x => x.FinalAvailableBalance < 0).Select(x => (KeyType?)x.Key).FirstOrDefault();

			return negativeBalanceKey;
		}
	}
}
