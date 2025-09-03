using Shared.CoreOne.Contracts.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.Helper.Models.Dtos;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Logic;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.Service.Services.Inventory
{
	public class ItemDisassembleLogicService : IItemDisassembleLogicService
	{
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
		private readonly IItemDisassembleService _itemDisassembleService;
		private readonly IItemBarCodeService _itemBarCodeService;
		private readonly IItemPackingService _itemPackingService;
		private readonly IStringLocalizer<ItemDisassembleLogicService> _localizer;

		private ItemConversionVm _itemConversion = new();


		public ItemDisassembleLogicService(IItemCurrentBalanceService itemCurrentBalanceService, IItemDisassembleService itemDisassembleService, IItemBarCodeService itemBarCodeService, IItemPackingService itemPackingService, IStringLocalizer<ItemDisassembleLogicService> localizer)
		{
			_itemCurrentBalanceService = itemCurrentBalanceService;
			_itemDisassembleService = itemDisassembleService;
			_itemBarCodeService = itemBarCodeService;
			_itemPackingService = itemPackingService;
			_localizer = localizer;
		}

		public async Task<List<ItemPackageTreeDto>> GetItemPackageBalanceAfterDisassembleQuantity(int itemId, int storeId, int fromPackageId, int toPackageId, string? batchNumber, DateTime? expireDate, decimal quantity)
		{
			var tree = new List<ItemPackageTreeDto>();
			var itemBalances = await _itemCurrentBalanceService.GetItemCurrentBalanceByItemId(storeId, itemId);

			var itemPackages = await _itemBarCodeService.GetAllItemBarCodes().Where(x => x.ItemId == itemId).ToListAsync();

			//var itemPackageSibling = await _itemPackingService.GetItemSiblingPackages(itemId, fromPackageId);
			//itemPackageSibling.Add(new ItemPackageVm() { ItemPackageId = fromPackageId, ItemPackageName = itemPackages.Where(x => x.FromPackageId == fromPackageId).Select(s => s.FromPackageName).FirstOrDefault(), Packing = 1 });
			var itemPackagesLevel = await _itemBarCodeService.GetItemPackagesLevel(itemId, fromPackageId, toPackageId);

			var itemName = itemBalances.Where(x => x.ItemId == itemId).Select(x => x.ItemName).FirstOrDefault();

			foreach (var package in itemPackagesLevel)
			{
				var packageName = itemPackages.Where(x => x.FromPackageId == package.ItemPackageId).Select(x => x.FromPackageName).FirstOrDefault();
				var packageBalance = itemBalances.Where(x => x.ItemPackageId == package.ItemPackageId && x.ExpireDate == expireDate && x.BatchNumber == batchNumber).Select(x => x.AvailableBalance).FirstOrDefault();
				var packing = itemPackages.Where(x => x.FromPackageId == package.MainItemPackageId && x.ToPackageId == package.ItemPackageId).Select(x => x.Packing).FirstOrDefault();
				var newPackage = new ItemPackageTreeDto()
				{
					ItemId = itemId,
					ItemName = itemName,
					ItemPackageId = package.ItemPackageId,
					MainItemPackageId = package.MainItemPackageId,
					ItemPackageName = packageName,
					ItemPackageBalanceBefore = NumberHelper.RoundNumber(packageBalance, 2),
					ItemPackageBalanceAfter = GetItemBalance(package.IsFirstLevel, package.IsSecondLevel, package.IsLastLevel, itemPackagesLevel.Count, packing, packageBalance, quantity),
					ItemPackageNameWithBalanceBefore = packageName != null ? $"{packageName} ({NumberHelper.RoundNumber(packageBalance, 2)})" : null,
					ItemPackageNameWithBalanceAfter = GetItemBalanceWithPackageName(packageName, package.IsFirstLevel, package.IsSecondLevel, package.IsLastLevel, itemPackagesLevel.Count, packing, packageBalance, quantity)
				};
				tree.Add(newPackage);
			}
			return tree;
		}

		private static decimal GetItemBalance(bool isFirstLevel, bool isSecondLevel, bool isLastLevel, int levelCount, decimal packing, decimal packageBalance, decimal quantity)
		{
			if (isFirstLevel)
			{
				return NumberHelper.RoundNumber((packageBalance - quantity), 2);
			}
			else if (isSecondLevel)
			{
				return NumberHelper.RoundNumber((packageBalance + ((packing * quantity) - 1)), 2);
			}
			else if (isLastLevel)
			{
				if (levelCount >= 3)
				{
					return NumberHelper.RoundNumber(packageBalance + packing, 2);
				}
				else
				{
					return NumberHelper.RoundNumber(packageBalance + (quantity * packing), 2);
				}
			}
			else
			{
				return NumberHelper.RoundNumber(packageBalance + (packing - 1), 2);
			}
		}

		private static string? GetItemBalanceWithPackageName(string? packageName, bool isFirstLevel, bool isSecondLevel, bool isLastLevel, int levelCount, decimal packing, decimal packageBalance, decimal quantity)
		{
			var balance = GetItemBalance(isFirstLevel, isSecondLevel, isLastLevel, levelCount, packing, packageBalance, quantity);
			return packageName != null ? $"{packageName} ({balance})" : null;
		}

		public async Task<ItemPackageConversionDto> GetItemPackageConversion(int itemId, int storeId, int fromPackageId, DateTime? fromPackageExpireDate, string? fromPackageBatchNumber, int toPackageId)
		{
			var batchNumber = !string.IsNullOrWhiteSpace(fromPackageBatchNumber) ? fromPackageBatchNumber.Trim() : null;
			var fromPackageData = await _itemCurrentBalanceService.GetItemCurrentBalances().Where(x => x.ItemId == itemId && x.StoreId == storeId && x.ItemPackageId == fromPackageId && x.ExpireDate == fromPackageExpireDate && x.BatchNumber!.Trim() == batchNumber).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.ItemPackageName }).Select(x => new ItemPackageConversionDto
			{
				ItemId = x.Key.ItemId,
				FromPackageId = x.Key.ItemPackageId,
				FromPackageName = x.Key.ItemPackageName,
				FromPackageAvailableBalance = x.Sum(s => s.AvailableBalance),
			}).FirstOrDefaultAsync() ?? new ItemPackageConversionDto();


			var toPackageData = await _itemCurrentBalanceService.GetItemCurrentBalances().Where(x => x.ItemId == itemId && x.StoreId == storeId && x.ItemPackageId == toPackageId && x.ExpireDate == fromPackageExpireDate && x.BatchNumber!.Trim() == batchNumber).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.ItemPackageName }).Select(x => new ItemPackageConversionDto
			{
				ItemId = x.Key.ItemId,
				ToPackageId = x.Key.ItemPackageId,
				ToPackageName = x.Key.ItemPackageName,
				ToPackageAvailableBalance = x.Sum(s => s.AvailableBalance),
			}).FirstOrDefaultAsync() ?? new ItemPackageConversionDto();

			var packing = await _itemPackingService.GetItemPacking(itemId, fromPackageId, toPackageId);


			return new ItemPackageConversionDto()
			{
				ItemId = itemId,
				FromPackageId = fromPackageData.FromPackageId,
				FromPackageAvailableBalance = fromPackageData.FromPackageAvailableBalance,
				FromPackageName = fromPackageData.FromPackageName,
				ToPackageId = toPackageData.ToPackageId,
				ToPackageAvailableBalance = toPackageData.ToPackageAvailableBalance,
				ToPackageName = toPackageData.ToPackageName,
				Packing = packing
			};
		}

		public async Task<ResponseDto> SetItemPackageConversion(int itemDisassembleHeaderId, ItemConversionDto model)
		{
			if (model.IsSerialConversion)
			{
				return await SetItemPackageSerialConversion(itemDisassembleHeaderId, model);
			}
			else
			{
				return await SetItemPackageDirectConversion(itemDisassembleHeaderId, model);
			}
		}

		private async Task<ResponseDto> SetItemPackageDirectConversion(int itemDisassembleHeaderId, ItemConversionDto model)
		{
			if (model.FromPackageId != 0)
			{
				var returnedData = await HandleDirectConversion(model, new ItemConversionVm());
				var newCurrentBalances = GetBalances(returnedData.InBalances, returnedData.OutBalances);
				await _itemCurrentBalanceService.ReOrderInventory(model.StoreId, newCurrentBalances);
				await _itemDisassembleService.SaveItemDisassemble(model.StoreId, itemDisassembleHeaderId, returnedData.ItemDisassembles);
				return new ResponseDto() { Id = model.ItemId, Success = true, Message = _localizer["ItemDisassembleSuccess"] };
			}
			return new ResponseDto() { Id = model.ItemId, Success = false, Message = _localizer["ItemPackageNotExist"] };
		}

		private async Task<ItemConversionVm> HandleDirectConversion(ItemConversionDto model, ItemConversionVm returnedModel)
		{
			var balancePackageFrom = await _itemCurrentBalanceService.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.StoreId == model.StoreId && x.ItemId == model.ItemId && x.ItemPackageId == model.FromPackageId && x.ExpireDate == model.ExpireDate && x.BatchNumber == model.BatchNumber);

			var balancePackageTo = await _itemCurrentBalanceService.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.StoreId == model.StoreId && x.ItemId == model.ItemId && x.ItemPackageId == model.ToPackageId && x.ExpireDate == model.ExpireDate && x.BatchNumber == model.BatchNumber);

			var packing = await _itemPackingService.GetItemPacking(model.ItemId, model.FromPackageId, model.ToPackageId);

			if (balancePackageFrom != null)
			{
				var outBalance = new ItemCurrentBalanceDto()
				{
					ItemCurrentBalanceId = balancePackageFrom.ItemCurrentBalanceId,
					ItemId = model.ItemId,
					ItemPackageId = model.FromPackageId,
					ExpireDate = model.ExpireDate,
					BatchNumber = model.BatchNumber,
					StoreId = model.StoreId,
					OutQuantity = model.Quantity
				};
				returnedModel.OutBalances.Add(outBalance);

				var inBalance = new ItemCurrentBalanceDto()
				{
					ItemCurrentBalanceId = balancePackageTo?.ItemCurrentBalanceId ?? 0,
					ItemId = model.ItemId,
					ItemPackageId = model.ToPackageId,
					ExpireDate = model.ExpireDate,
					BatchNumber = model.BatchNumber,
					StoreId = model.StoreId,
					InQuantity = (model.Quantity * packing)
				};

				returnedModel.InBalances.Add(inBalance);

				var disassembleOutModel = new ItemDisassembleDto()
				{
					StoreId = model.StoreId,
					ItemId = model.ItemId,
					ItemPackageId = model.FromPackageId,
					InQuantity = 0,
					OutQuantity = model.Quantity,
					ExpireDate = model.ExpireDate,
					BatchNumber = model.BatchNumber,
					EntryDate = DateHelper.GetDateTimeNow(),
				};

				var disassembleInModel = new ItemDisassembleDto()
				{
					StoreId = model.StoreId,
					ItemId = model.ItemId,
					ItemPackageId = model.ToPackageId,
					InQuantity = (model.Quantity * packing),
					OutQuantity = 0,
					ExpireDate = model.ExpireDate,
					BatchNumber = model.BatchNumber,
					EntryDate = DateHelper.GetDateTimeNow(),
				};

				returnedModel.ItemDisassembles.Add(disassembleOutModel);
				returnedModel.ItemDisassembles.Add(disassembleInModel);

				return returnedModel;
			}
			return returnedModel;
		}

		private async Task<ResponseDto> SetItemPackageSerialConversion(int itemDisassembleHeaderId, ItemConversionDto model)
		{
			var itemBalances = await _itemCurrentBalanceService.GetAll().AsNoTracking().Where(x => x.ItemId == model.ItemId && x.StoreId == model.StoreId).ToListAsync();

			var itemPackages = await _itemPackingService.GetItemPacking(model.ItemId).ToListAsync();

			var itemPackageSibling = await _itemPackingService.GetItemSiblingPackages(model.ItemId, model.FromPackageId);
			itemPackageSibling.Add(new ItemPackageVm() { ItemPackageId = model.FromPackageId, ItemPackageName = itemPackages.Where(x => x.FromPackageId == model.FromPackageId).Select(s => s.FromPackageName).FirstOrDefault(), Packing = 1 });

			var itemPackagesLevel = await _itemBarCodeService.GetItemPackagesLevel(model.ItemId, model.FromPackageId, model.ToPackageId);

			foreach (var packageTree in itemPackagesLevel)
			{
				var mainPackage = packageTree.MainItemPackageId ?? packageTree.ItemPackageId;
				var packing = itemPackages.Where(x => x.FromPackageId == mainPackage && x.ToPackageId == packageTree.ItemPackageId).Select(x => x.Packing).FirstOrDefault();

				var currentBalance = itemBalances.FirstOrDefault(x => x.StoreId == model.StoreId && x.ItemId == model.ItemId && x.ItemPackageId == packageTree.ItemPackageId && x.ExpireDate == model.ExpireDate && x.BatchNumber == model.BatchNumber);

				var packageConversion = new ItemSerialConversionDto()
				{
					ItemCurrentBalanceId = currentBalance?.ItemCurrentBalanceId ?? 0,
					ItemId = model.ItemId,
					BatchNumber = model.BatchNumber,
					ExpireDate = model.ExpireDate,
					StoreId = model.StoreId,
					IsSerialConversion = true,
					Quantity = model.Quantity,
					ItemPackageId = packageTree.ItemPackageId,
					Packing = packing,
					IsFirstLevel = packageTree.IsFirstLevel,
					IsSecondLevel = packageTree.IsSecondLevel,
					IsLastLevel = packageTree.IsLastLevel,
					LevelCount = itemPackagesLevel.Count,
					Level = packageTree.Level,
					RemarksAr = model.RemarksAr,
					RemarksEn = model.RemarksEn
				};
				_itemConversion = HandleSerialConversion(packageConversion, _itemConversion);
			}

			var newCurrentBalances = GetBalances(_itemConversion.InBalances, _itemConversion.OutBalances);
			await _itemCurrentBalanceService.ReOrderInventory(model.StoreId, newCurrentBalances);
			await _itemDisassembleService.SaveItemDisassemble(model.StoreId, itemDisassembleHeaderId, _itemConversion.ItemDisassembles);
			return new ResponseDto() { Id = model.ItemId, Success = true, Message = _localizer["ItemDisassembleSuccess"] };
		}

		private static ItemConversionVm HandleSerialConversion(ItemSerialConversionDto model, ItemConversionVm returnedModel)
		{
			if (model.IsFirstLevel)
			{
				var outBalance = new ItemCurrentBalanceDto()
				{
					ItemCurrentBalanceId = model.ItemCurrentBalanceId,
					ItemId = model.ItemId,
					ItemPackageId = model.ItemPackageId,
					ExpireDate = model.ExpireDate,
					BatchNumber = model.BatchNumber,
					StoreId = model.StoreId,
					OutQuantity = model.Quantity
				};
				returnedModel.OutBalances.Add(outBalance);

				var disassembleOutModel = new ItemDisassembleDto()
				{
					StoreId = model.StoreId,
					ItemId = model.ItemId,
					ItemPackageId = model.ItemPackageId,
					ExpireDate = model.ExpireDate,
					BatchNumber = model.BatchNumber,
					EntryDate = DateHelper.GetDateTimeNow(),
					OutQuantity = model.Quantity,
				};
				returnedModel.ItemDisassembles.Add(disassembleOutModel);

			}
			else if (model.IsSecondLevel)
			{
				var inBalance = new ItemCurrentBalanceDto()
				{
					ItemCurrentBalanceId = model.ItemCurrentBalanceId,
					ItemId = model.ItemId,
					ItemPackageId = model.ItemPackageId,
					ExpireDate = model.ExpireDate,
					BatchNumber = model.BatchNumber,
					StoreId = model.StoreId,
					InQuantity = model.Packing * model.Quantity,
				};
				returnedModel.InBalances.Add(inBalance);

				var outBalance = new ItemCurrentBalanceDto()
				{
					ItemCurrentBalanceId = model.ItemCurrentBalanceId,
					ItemId = model.ItemId,
					ItemPackageId = model.ItemPackageId,
					ExpireDate = model.ExpireDate,
					BatchNumber = model.BatchNumber,
					StoreId = model.StoreId,
					OutQuantity = 1
				};
				returnedModel.OutBalances.Add(outBalance);

				var disassembleInModel = new ItemDisassembleDto()
				{
					StoreId = model.StoreId,
					ItemId = model.ItemId,
					ItemPackageId = model.ItemPackageId,
					ExpireDate = model.ExpireDate,
					BatchNumber = model.BatchNumber,
					EntryDate = DateHelper.GetDateTimeNow(),
					InQuantity = model.Packing * model.Quantity,
				};
				returnedModel.ItemDisassembles.Add(disassembleInModel);

				var disassembleOutModel = new ItemDisassembleDto()
				{
					StoreId = model.StoreId,
					ItemId = model.ItemId,
					ItemPackageId = model.ItemPackageId,
					ExpireDate = model.ExpireDate,
					BatchNumber = model.BatchNumber,
					EntryDate = DateHelper.GetDateTimeNow(),
					OutQuantity = 1,
				};
				returnedModel.ItemDisassembles.Add(disassembleOutModel);

			}
			else if (model.IsLastLevel)
			{
				if (model.LevelCount >= 3)
				{
					var inBalance = new ItemCurrentBalanceDto()
					{
						ItemCurrentBalanceId = model.ItemCurrentBalanceId,
						ItemId = model.ItemId,
						ItemPackageId = model.ItemPackageId,
						ExpireDate = model.ExpireDate,
						BatchNumber = model.BatchNumber,
						StoreId = model.StoreId,
						InQuantity = model.Packing,
					};
					returnedModel.InBalances.Add(inBalance);

					var disassembleInModel = new ItemDisassembleDto()
					{
						StoreId = model.StoreId,
						ItemId = model.ItemId,
						ItemPackageId = model.ItemPackageId,
						ExpireDate = model.ExpireDate,
						BatchNumber = model.BatchNumber,
						EntryDate = DateHelper.GetDateTimeNow(),
						InQuantity = model.Packing,
					};
					returnedModel.ItemDisassembles.Add(disassembleInModel);
				}
				else
				{
					var inBalance = new ItemCurrentBalanceDto()
					{
						ItemCurrentBalanceId = model.ItemCurrentBalanceId,
						ItemId = model.ItemId,
						ItemPackageId = model.ItemPackageId,
						ExpireDate = model.ExpireDate,
						BatchNumber = model.BatchNumber,
						StoreId = model.StoreId,
						InQuantity = model.Packing * model.Quantity,
					};
					returnedModel.InBalances.Add(inBalance);

					var disassembleInModel = new ItemDisassembleDto()
					{
						StoreId = model.StoreId,
						ItemId = model.ItemId,
						ItemPackageId = model.ItemPackageId,
						ExpireDate = model.ExpireDate,
						BatchNumber = model.BatchNumber,
						EntryDate = DateHelper.GetDateTimeNow(),
						InQuantity = model.Packing * model.Quantity,
					};
					returnedModel.ItemDisassembles.Add(disassembleInModel);
				}
			}
			else
			{
				var inBalance = new ItemCurrentBalanceDto()
				{
					ItemCurrentBalanceId = model.ItemCurrentBalanceId,
					ItemId = model.ItemId,
					ItemPackageId = model.ItemPackageId,
					ExpireDate = model.ExpireDate,
					BatchNumber = model.BatchNumber,
					StoreId = model.StoreId,
					InQuantity = model.Packing,
				};
				returnedModel.InBalances.Add(inBalance);

				var outBalance = new ItemCurrentBalanceDto()
				{
					ItemCurrentBalanceId = model.ItemCurrentBalanceId,
					ItemId = model.ItemId,
					ItemPackageId = model.ItemPackageId,
					ExpireDate = model.ExpireDate,
					BatchNumber = model.BatchNumber,
					StoreId = model.StoreId,
					OutQuantity = 1
				};
				returnedModel.OutBalances.Add(outBalance);

				var disassembleInModel = new ItemDisassembleDto()
				{
					StoreId = model.StoreId,
					ItemId = model.ItemId,
					ItemPackageId = model.ItemPackageId,
					ExpireDate = model.ExpireDate,
					BatchNumber = model.BatchNumber,
					EntryDate = DateHelper.GetDateTimeNow(),
					InQuantity = model.Packing,
				};
				returnedModel.ItemDisassembles.Add(disassembleInModel);

				var disassembleOutModel = new ItemDisassembleDto()
				{
					StoreId = model.StoreId,
					ItemId = model.ItemId,
					ItemPackageId = model.ItemPackageId,
					ExpireDate = model.ExpireDate,
					BatchNumber = model.BatchNumber,
					EntryDate = DateHelper.GetDateTimeNow(),
					OutQuantity = 1,
				};
				returnedModel.ItemDisassembles.Add(disassembleOutModel);
			}

			return returnedModel;
		}
		private static List<ItemCurrentBalanceDto> GetBalances(List<ItemCurrentBalanceDto> inBalances, List<ItemCurrentBalanceDto> outBalances)
		{
			var newBalances = inBalances.Union(outBalances).GroupBy(x => new { x.ItemCurrentBalanceId, x.ItemId, x.ItemPackageId, x.StoreId, x.ExpireDate, x.BatchNumber }).Select(s => new ItemCurrentBalanceDto()
			{
				ItemCurrentBalanceId = s.Key.ItemCurrentBalanceId,
				ItemId = s.Key.ItemId,
				ItemPackageId = s.Key.ItemPackageId,
				StoreId = s.Key.StoreId,
				ExpireDate = s.Key.ExpireDate,
				BatchNumber = string.IsNullOrWhiteSpace(s.Key.BatchNumber) ? null : s.Key.BatchNumber.Trim(),
				InQuantity = s.Sum(x => x.InQuantity),
				OutQuantity = s.Sum(x => x.OutQuantity)
			}).ToList();
			return newBalances;
		}


		public async Task<ResponseDto> ReverseItemPackageConversion(int itemDisassembleHeaderId)
		{
			var itemConversionData = await _itemDisassembleService.GetItemDisassembleByHeaderId(itemDisassembleHeaderId);
			var storeId = itemConversionData.Any() ? itemConversionData[0].StoreId : 0;

			var oldBalances = itemConversionData.Select(x => new ItemCurrentBalanceDto()
			{
				ItemId = x.ItemId,
				ItemPackageId = x.ItemPackageId,
				StoreId = x.StoreId,
				ExpireDate = x.ExpireDate,
				BatchNumber = x.BatchNumber,
				InQuantity = x.InQuantity,
				OutQuantity = x.OutQuantity
			}).ToList();

			var newBalances = itemConversionData.Select(x => new ItemCurrentBalanceDto()
			{
				ItemId = x.ItemId,
				ItemPackageId = x.ItemPackageId,
				StoreId = x.StoreId,
				ExpireDate = x.ExpireDate,
				BatchNumber = x.BatchNumber,
				InQuantity = 0,
				OutQuantity = 0
			}).ToList();

			if (oldBalances.Any() && newBalances.Any() && storeId > 0)
			{
				await _itemCurrentBalanceService.InventoryInOut(storeId, oldBalances, newBalances);
				return new ResponseDto() { Success = true,Message = _localizer["DeleteSuccess"]};
			}
			return new ResponseDto() { Success = false, Message = _localizer["ReverseItemPackageFailed"] };
		}
	}
}
