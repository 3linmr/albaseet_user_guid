using Inventory.CoreOne.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using Inventory.CoreOne.Models.Domain;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Service.Services.Modules;

namespace Inventory.Service.Services
{
	public class StockTakingCarryOverService : IStockTakingCarryOverService
	{
		private readonly IStockTakingCarryOverHeaderService _stockTakingCarryOverHeaderService;
		private readonly IStockTakingCarryOverDetailService _stockTakingCarryOverDetailService;
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
		private readonly IStockTakingHeaderService _stockTakingHeaderService;
		private readonly IStockTakingCarryOverEffectDetailService _stockTakingCarryOverEffectDetailService;
		private readonly IItemService _itemService;
		private readonly IItemCostService _itemCostService;
		private readonly IInternalTransferService _internalTransferService;
		private readonly IStringLocalizer<StockTakingCarryOverService> _localizer;
		private readonly IStoreService _storeService;
		private readonly IMenuNoteService _menuNoteService;
		private readonly IStringLocalizer<StockTakingCarryOverHeaderService> _headerLocalizer;

		public StockTakingCarryOverService(IStockTakingCarryOverHeaderService stockTakingCarryOverHeaderService, IStockTakingCarryOverDetailService stockTakingCarryOverDetailService, IItemCurrentBalanceService itemCurrentBalanceService, IStockTakingHeaderService stockTakingHeaderService, IStockTakingCarryOverEffectDetailService stockTakingCarryOverEffectDetailService, IItemService itemService,IItemCostService itemCostService,IInternalTransferService internalTransferService,IStringLocalizer<StockTakingCarryOverService> localizer, IStoreService storeService, IMenuNoteService menuNoteService, IStringLocalizer<StockTakingCarryOverHeaderService> headerLocalizer)
		{
			_stockTakingCarryOverHeaderService = stockTakingCarryOverHeaderService;
			_stockTakingCarryOverDetailService = stockTakingCarryOverDetailService;
			_itemCurrentBalanceService = itemCurrentBalanceService;
			_stockTakingHeaderService = stockTakingHeaderService;
			_stockTakingCarryOverEffectDetailService = stockTakingCarryOverEffectDetailService;
			_itemService = itemService;
			_itemCostService = itemCostService;
			_internalTransferService = internalTransferService;
			_localizer = localizer;
			_storeService = storeService;
			_menuNoteService = menuNoteService;
			_headerLocalizer = headerLocalizer;
		}
		public List<RequestChangesDto> GetCarryOverRequestChanges(StockTakingCarryOverDto oldItem, StockTakingCarryOverDto newItem)
		{
			var requestChanges = new List<RequestChangesDto>();
			var items = CompareLogic.GetDifferences(oldItem.StockTakingCarryOverHeader, newItem.StockTakingCarryOverHeader);
			requestChanges.AddRange(items);

			if (oldItem.StockTakingCarryOverDetails.Any() && newItem.StockTakingCarryOverDetails.Any())
			{
				var oldCount = oldItem.StockTakingCarryOverDetails.Count;
				var newCount = newItem.StockTakingCarryOverDetails.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.StockTakingCarryOverDetails[i], newItem.StockTakingCarryOverDetails[i]);
							requestChanges.AddRange(changes);
							index++;
							break;
						}
					}
				}
            }

            if (oldItem.MenuNotes != null && oldItem.MenuNotes.Any() && newItem.MenuNotes != null && newItem.MenuNotes.Any())
            {
                var oldCount = oldItem.MenuNotes.Count;
                var newCount = newItem.MenuNotes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.MenuNotes[i], newItem.MenuNotes[i]);
                            requestChanges.AddRange(changes);
                            index++;
                            break;
                        }
                    }
                }
            }

            return requestChanges;
		}

		public async Task<StockTakingCarryOverDto> GetCarryOver(int stockTakingCarryOverHeaderId)
		{
			var header = await _stockTakingCarryOverHeaderService.GetStockTakingCarryOverHeaderById(stockTakingCarryOverHeaderId);
			if (header == null) return new StockTakingCarryOverDto();

			var details = await _stockTakingCarryOverDetailService.GetCarryOversDetailsById(stockTakingCarryOverHeaderId).ToListAsync();
            var menuNotes = await _menuNoteService.GetMenuNotes(header.IsOpenBalance ? MenuCodeData.ApprovalStockTakingAsOpenBalance : MenuCodeData.ApprovalStockTakingAsCurrentBalance, stockTakingCarryOverHeaderId).ToListAsync();

            return new StockTakingCarryOverDto() { StockTakingCarryOverHeader = header!, StockTakingCarryOverDetails = details, MenuNotes = menuNotes };
		}

        public async Task<ResponseDto> SaveStockTaking(StockTakingCarryOverDto stockTaking, bool hasApprove, bool approved, int? requestId)
		{
			var isPendingTransferExist = await _internalTransferService.GetInternalTransferPendingItems(stockTaking.StockTakingCarryOverHeader.StoreId, stockTaking.StockTakingCarryOverHeader.IsAllItemsAffected, stockTaking.StockTakingCarryOverDetails.Select(x=>x.ItemId).ToList());
			if (!isPendingTransferExist)
			{
				var result = await _stockTakingCarryOverHeaderService.SaveStockTakingCarryOverHeader(stockTaking.StockTakingCarryOverHeader, hasApprove, approved, requestId);
				if (result.Success)
				{
					await _stockTakingCarryOverDetailService.SaveCarryOverDetails(result.Id, stockTaking.StockTakingCarryOverDetails);

					if (stockTaking.MenuNotes != null)
					{
						await _menuNoteService.SaveMenuNotes(stockTaking.MenuNotes, result.Id);
					}
					var effects = await _stockTakingCarryOverEffectDetailService.InsertCarryOverEffect(result.Id, stockTaking.StockTakingCarryOverHeader.StoreId, stockTaking.StockTakingCarryOverHeader.IsOpenBalance, stockTaking.StockTakingCarryOverHeader.IsAllItemsAffected, stockTaking.StockTakingCarryOverDetails);

					await DoCarryOver(stockTaking.StockTakingCarryOverHeader.StoreId, stockTaking.StockTakingCarryOverHeader.IsOpenBalance, stockTaking.StockTakingCarryOverHeader.DocumentDate, effects);

					var costHasDuplicated = CheckIfItemCostIsDuplicated(stockTaking.StockTakingCarryOverHeader.StoreId, stockTaking.StockTakingCarryOverHeader.IsOpenBalance,stockTaking.StockTakingCarryOverDetails);
					if (!costHasDuplicated)
					{
						await DoCarryOverCosts(stockTaking.StockTakingCarryOverHeader.StoreId, stockTaking.StockTakingCarryOverHeader.IsOpenBalance, stockTaking.StockTakingCarryOverDetails);
						var carriedOverIds = JsonConvert.DeserializeObject<List<int>>(stockTaking.StockTakingCarryOverHeader.StockTakingList ?? "") ?? new List<int>();
						await _stockTakingHeaderService.UpdateStockTakingToBeCarriedOver(carriedOverIds);
					}
					else
					{
						return new ResponseDto() { Id = 0, Success = false, Message = _localizer["CostHasDuplicated"] };
					}
				}
				return result;
			}
			else
			{
				return new ResponseDto() { Id = 0, Success = false, Message = _localizer["PendingInternalTransferExist"] };
			}
			
		}

		public async Task<ResponseDto> DeleteStockTaking(int stockTakingCarryOverHeaderId)
		{
			var header = await _stockTakingCarryOverHeaderService.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.StockTakingCarryOverHeaderId == stockTakingCarryOverHeaderId);
			if (header != null)
			{
				var effects = await _stockTakingCarryOverEffectDetailService.GetAll().Where(x => x.StockTakingCarryOverHeaderId == stockTakingCarryOverHeaderId).AsNoTracking().ToListAsync();
				await UnDoCarryOver(header.StoreId, header.IsOpenBalance, header.DocumentDate, effects);
				await _stockTakingCarryOverEffectDetailService.DeleteCarryOverEffect(stockTakingCarryOverHeaderId);
				await _menuNoteService.DeleteMenuNotes(header.IsOpenBalance ? MenuCodeData.ApprovalStockTakingAsOpenBalance : MenuCodeData.ApprovalStockTakingAsCurrentBalance, header.StockTakingCarryOverHeaderId);
				await _stockTakingCarryOverDetailService.DeleteCarryOverDetails(stockTakingCarryOverHeaderId);
				var result = await _stockTakingCarryOverHeaderService.DeleteStockTakingCarryOverHeader(stockTakingCarryOverHeaderId);
				var carriedOverIds = JsonConvert.DeserializeObject<List<int>>(header.StockTakingList ?? "") ?? new List<int>();
				await _stockTakingHeaderService.UpdateStockTakingToBeUnCarriedOver(carriedOverIds);
				return result;
			}
			return new ResponseDto { Id = stockTakingCarryOverHeaderId, Message = _headerLocalizer["NoStockTakingFound"] };
		}

		public async Task<bool> DoCarryOver(int storeId, bool isOpenBalance, DateTime openBalanceCarryOverDate, List<StockTakingCarryOverEffectDetail> effects)
		{
			var data = effects.Select(x => new ItemCurrentBalanceDto()
			{
				ItemId = x.ItemId,
				StoreId = storeId,
				ItemPackageId = x.ItemPackageId,
				BatchNumber = x.BatchNumber,
				ExpireDate = x.ExpireDate,
				InQuantity = x.InQuantity,
				OutQuantity = x.OutQuantity,
				PendingInQuantity = 0,
				PendingOutQuantity = 0,
				OpenQuantity = isOpenBalance ? x.OpenQuantity : 0,
				OpenDate = isOpenBalance ? openBalanceCarryOverDate : null
			}).ToList();

			var activeBalances = await _itemCurrentBalanceService.ReOrderInventory(storeId, openBalanceCarryOverDate, data);

			return true;
		}

		public async Task<bool> UnDoCarryOver(int storeId, bool isOpenBalance, DateTime openBalanceCarryOverDate, List<StockTakingCarryOverEffectDetail> effects)
		{
			var data = effects.Select(x => new ItemCurrentBalanceDto()
			{
				ItemId = x.ItemId,
				StoreId = storeId,
				ItemPackageId = x.ItemPackageId,
				BatchNumber = x.BatchNumber,
				ExpireDate = x.ExpireDate,
				InQuantity = x.InQuantity * -1,
				OutQuantity = x.OutQuantity * -1,
				PendingInQuantity = 0,
				PendingOutQuantity = 0,
				OpenQuantity = x.OpenQuantity * -1,
				OpenDate = isOpenBalance ? openBalanceCarryOverDate : null
			}).ToList();

			var activeBalances = await _itemCurrentBalanceService.ReOrderInventory(storeId, openBalanceCarryOverDate, data);
			return true;
		}

		public async Task<bool> DoCarryOverCosts(int storeId, bool isOpenBalance, List<StockTakingCarryOverDetailDto> carryOvers)
		{
			if (isOpenBalance)
			{
				var carryOversItems = carryOvers.Select(x => x.ItemId).Distinct().ToList();
				var items = await _itemService.GetAll().AsNoTracking().Where(x => carryOversItems.Contains(x.ItemId)).ToListAsync();
				var data =
					(from carryOver in carryOvers
					 from item in items.Where(x => x.ItemId == carryOver.ItemId)
					 select new ItemCostDto()
					 {
						 ItemPackageId = item.SingularPackageId,
						 StoreId = storeId,
						 ItemId = carryOver.ItemId,
						 CostPrice = carryOver.StockTakingCostPrice,
						 Packing = carryOver.Packing
					 }).ToList();

				var dataDistinct = data.DistinctBy(x=> new {x.ItemId,x.ItemPackageId}).ToList();
				await _itemCostService.SaveCosts(storeId, dataDistinct);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool CheckIfItemCostIsDuplicated(int storeId,bool isOpenBalance,List<StockTakingCarryOverDetailDto> carryOvers)
		{
			if (isOpenBalance)
			{
				var data = carryOvers.Select(x => new
				{
					ItemId = x.ItemId,
					CostPrice = x.StockTakingCostPrice
				}).ToList().Distinct().ToList();
				return data.GroupBy(n => n.ItemId).Any(c => c.Count() > 1);
			}
			else
			{
				return false;
			}
		}
	}
}
