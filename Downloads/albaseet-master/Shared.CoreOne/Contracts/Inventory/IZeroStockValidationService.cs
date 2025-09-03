using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Contracts.Inventory
{
	public interface IZeroStockValidationService
	{
		Task<ResponseDto> ValidateZeroStock<DetailType, KeyType>(
			int storeId, 
			List<DetailType> newDetails, List<DetailType> oldDetails, 
			Func<DetailType, KeyType> detailKeySelector, Func<KeyType, int> itemIdSelector, Func<DetailType, decimal> quantitySelector, 
			Func<ItemCurrentBalanceDto, KeyType> availableBalanceKeySelector, 
			bool isGrouped, int menuCode, int settingMenuCode, bool isSave) where KeyType : struct;

		Task<ResponseDto> ValidateZeroStockReturn<DetailType, KeyType>(
			int storeId,
			List<DetailType> newDetails, List<DetailType> oldDetails,
			Func<DetailType, KeyType> detailKeySelector, Func<KeyType, int> itemIdSelector, Func<DetailType, decimal> quantitySelector,
			Func<ItemCurrentBalanceDto, KeyType> availableBalanceKeySelector,
			bool isGrouped, int menuCode, int settingMenuCode, bool isSave) where KeyType : struct;
	}
}
