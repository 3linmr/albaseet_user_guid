using Inventory.CoreOne.Contracts;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Contracts.Modules;
using Microsoft.EntityFrameworkCore;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Dtos.ViewModels;
using System.Linq;

namespace Inventory.Service.Services
{
    public class InventoryOutService : IInventoryOutService
    {
        private readonly IInventoryOutHeaderService _inventoryOutHeaderService;
        private readonly IInventoryOutDetailService _inventoryOutDetailService;
        private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
        private readonly ICostCenterJournalDetailService _costCenterJournalDetailService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IZeroStockValidationService _zeroStockValidationService;

        public InventoryOutService(IInventoryOutHeaderService inventoryOutHeaderService, IInventoryOutDetailService inventoryOutDetailService, IItemCurrentBalanceService itemCurrentBalanceService, ICostCenterJournalDetailService costCenterJournalDetailService, IMenuNoteService menuNoteService, IZeroStockValidationService zeroStockValidationService)
        {
            _inventoryOutHeaderService = inventoryOutHeaderService;
            _inventoryOutDetailService = inventoryOutDetailService;
            _itemCurrentBalanceService = itemCurrentBalanceService;
            _costCenterJournalDetailService = costCenterJournalDetailService;
            _menuNoteService = menuNoteService;
            _zeroStockValidationService = zeroStockValidationService;
        }
        public List<RequestChangesDto> GetInventoryOutRequestChanges(InventoryOutDto oldItem, InventoryOutDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.InventoryOutHeader, newItem.InventoryOutHeader);
            requestChanges.AddRange(items);

            if (oldItem.InventoryOutDetails.Any() && newItem.InventoryOutDetails.Any())
            {
                var oldCount = oldItem.InventoryOutDetails.Count;
                var newCount = newItem.InventoryOutDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.InventoryOutDetails[i], newItem.InventoryOutDetails[i]);
                            requestChanges.AddRange(changes);

                            requestChanges.RemoveAll(x => x.ColumnName == "CostCenters");

                            var costCenterChanges = GetCostCentersRequestChanges(oldItem.InventoryOutDetails[i], newItem.InventoryOutDetails[i]);
                            requestChanges.AddRange(costCenterChanges);
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

        private List<RequestChangesDto> GetCostCentersRequestChanges(InventoryOutDetailDto oldItem, InventoryOutDetailDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            if (oldItem.CostCenters.Any() && newItem.CostCenters.Any())
            {
                var oldCount = oldItem.CostCenters.Count;
                var newCount = newItem.CostCenters.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.CostCenters[i], newItem.CostCenters[i]);
                            requestChanges.AddRange(changes);

                            index++;
                            break;
                        }
                    }
                }
            }

            return requestChanges;
        }

        public async Task<InventoryOutDto> GetInventoryOut(int inventoryOutHeaderId)
        {
            var header = await _inventoryOutHeaderService.GetInventoryOutHeaderById(inventoryOutHeaderId);
            if (header == null) return new InventoryOutDto();

            var details = await _inventoryOutDetailService.GetInventoryOutDetails(inventoryOutHeaderId);

            await ModifyInventoryOutDetailsWithStoreIdAndAvaialbleBalance(inventoryOutHeaderId, header.StoreId, details, false);

            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.InventoryOut, inventoryOutHeaderId).ToListAsync();
            var costCenters = await _costCenterJournalDetailService.GetCostCenterJournalDetails(inventoryOutHeaderId, MenuCodeData.InventoryOut);

            foreach (var detail in details)
            {
                detail.CostCenters = costCenters.Where(x => x.ReferenceDetailId == detail.InventoryOutDetailId).ToList();
            }

            return new InventoryOutDto() { InventoryOutHeader = header, InventoryOutDetails = details, MenuNotes = menuNotes };
		}

		public async Task ModifyInventoryOutDetailsWithStoreIdAndAvaialbleBalance(int inventoryOutHeaderId, int storeId, List<InventoryOutDetailDto> details, bool isRequestData)
		{
			var itemIds = details.Select(x => x.ItemId).ToList();

			var availableBalances = await _itemCurrentBalanceService.GetItemCurrentBalanceInfo(storeId, false, false).Where(x => x.StoreId == storeId && itemIds.Contains(x.ItemId)).ToListAsync();
			var currentInventoryOutDetails = isRequestData ? await _inventoryOutDetailService.GetInventoryOutDetails(inventoryOutHeaderId): details;

			var filteredAvailableBalances = availableBalances.Select(x => new { Key = (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber), Quantity = x.AvailableBalance });
			var filteredCurrentDetails = currentInventoryOutDetails.Select(x => new { Key = (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber), Quantity = x.Quantity});

			var finalAvailableBalances = filteredAvailableBalances.Concat(filteredCurrentDetails).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));

			foreach (var detail in details)
			{
				detail.StoreId = storeId;
				detail.AvailableBalance = finalAvailableBalances.GetValueOrDefault((detail.ItemId, detail.ItemPackageId, detail.ExpireDate, detail.BatchNumber), 0);
			}
		}

		public async Task<ResponseDto> SaveInventoryOut(InventoryOutDto inventoryOut, bool hasApprove, bool approved, int? requestId)
        {
            TrimDetailStrings(inventoryOut.InventoryOutDetails);
            if (inventoryOut.InventoryOutHeader != null)
            {
                List<InventoryOutDetailDto>? oldInventoryOutDetails = [];
                int? oldStoreId = null;
                if (inventoryOut.InventoryOutHeader.InventoryOutHeaderId != 0)
                {
                    var oldInventoryOut = await GetInventoryOut(inventoryOut.InventoryOutHeader.InventoryOutHeaderId);
                    oldInventoryOutDetails = oldInventoryOut.InventoryOutDetails;
                    oldStoreId = oldInventoryOut.InventoryOutHeader?.StoreId;
                }

                var zeroStockValidationResult = await CheckInventoryZeroStock(inventoryOut.InventoryOutHeader.StoreId, inventoryOut.InventoryOutDetails, oldInventoryOutDetails);
                if (zeroStockValidationResult.Success == false) return zeroStockValidationResult;

                var result = await _inventoryOutHeaderService.SaveInventoryOutHeader(inventoryOut.InventoryOutHeader, hasApprove, approved, requestId);
                if (result.Success)
                {
                    var modifiedInventoryOutDetailsList = await _inventoryOutDetailService.SaveInventoryOutDetails(result.Id, inventoryOut.InventoryOutDetails);
                    await SaveCostCenters(result.Id, modifiedInventoryOutDetailsList);
                    await UpdateCurrentBalances(oldStoreId, inventoryOut.InventoryOutHeader.StoreId, oldInventoryOutDetails, inventoryOut.InventoryOutDetails);
                    if (inventoryOut.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(inventoryOut.MenuNotes, result.Id);
                    }
                }

                return result;
            }
            return new ResponseDto{ Message = "Header should not be null" };
        }

        public async Task<ResponseDto> CheckInventoryZeroStock(int storeId, List<InventoryOutDetailDto> newDetails, List<InventoryOutDetailDto> oldDetails)
        {
            return await _zeroStockValidationService.ValidateZeroStock(
                storeId: storeId,
                newDetails: newDetails,
                oldDetails: oldDetails,
                detailKeySelector: x => (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber),
                itemIdSelector: x => x.ItemId,
                quantitySelector: x => x.Quantity,
                availableBalanceKeySelector: x => (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber),
                isGrouped: false,
                menuCode: MenuCodeData.InventoryOut,
                settingMenuCode: MenuCodeData.InventoryOut,
                isSave: true
				);
        }

        private void TrimDetailStrings(List<InventoryOutDetailDto> inventoryOutDetails)
        {
            foreach (var inventoryOutDetail in inventoryOutDetails)
            {
                inventoryOutDetail.BatchNumber = string.IsNullOrWhiteSpace(inventoryOutDetail.BatchNumber) ? null : inventoryOutDetail.BatchNumber.Trim();
            }
        }

        public async Task<ResponseDto> DeleteInventoryOut(int inventoryOutHeaderId)
        {
            var inventoryOut = await GetInventoryOut(inventoryOutHeaderId);

            await _menuNoteService.DeleteMenuNotes(MenuCodeData.InventoryOut, inventoryOutHeaderId);
            await _inventoryOutDetailService.DeleteInventoryOutDetails(inventoryOutHeaderId);
            var result = await _inventoryOutHeaderService.DeleteInventoryOutHeader(inventoryOutHeaderId);

            if (inventoryOut.InventoryOutHeader is not null && result.Success)
            {
                await DeductCurrentBalances(inventoryOut.InventoryOutHeader.StoreId, inventoryOut.InventoryOutDetails);
            }

            return result;
        }

        private async Task UpdateCurrentBalances(int? oldStoreId, int newStoreId, List<InventoryOutDetailDto>? oldInventoryOutDetails, List<InventoryOutDetailDto> newInventoryOutDetails)
        {
            var newItemBalances = GetItemCurrentBalancesFromInventoryOut(newStoreId, newInventoryOutDetails);
            if (oldInventoryOutDetails is null || oldStoreId is null)
            {
                await _itemCurrentBalanceService.InventoryOut(newStoreId, newItemBalances);
            }
            else
            {
                if (oldStoreId == newStoreId)
                {
                    var oldItemBalances = GetItemCurrentBalancesFromInventoryOut(newStoreId, oldInventoryOutDetails);
                    await _itemCurrentBalanceService.InventoryInOut(newStoreId, oldItemBalances, newItemBalances);
                }
                else
                {
                    await DeductCurrentBalances((int)oldStoreId, oldInventoryOutDetails);
                    await _itemCurrentBalanceService.InventoryOut(newStoreId, newItemBalances);
                }
            }
        }

        private async Task DeductCurrentBalances(int storeId, List<InventoryOutDetailDto> inventoryOutDetails)
        {
            var oldItemCurrentBalances = GetItemCurrentBalancesFromInventoryOut(storeId, inventoryOutDetails);

            var newItemCurrentBalances = (from itemCurrentPrices in oldItemCurrentBalances
                                          select new ItemCurrentBalanceDto()
                                          {
                                              ItemCurrentBalanceId = itemCurrentPrices.ItemCurrentBalanceId,
                                              StoreId = itemCurrentPrices.StoreId,
                                              ItemId = itemCurrentPrices.ItemId,
                                              ItemPackageId = itemCurrentPrices.ItemPackageId,
                                              ExpireDate = itemCurrentPrices.ExpireDate,
                                              BatchNumber = itemCurrentPrices.BatchNumber,
                                              //Zero
                                          }).ToList();

            await _itemCurrentBalanceService.InventoryInOut(storeId, oldItemCurrentBalances, newItemCurrentBalances);
        }

        private List<ItemCurrentBalanceDto> GetItemCurrentBalancesFromInventoryOut(int storeId, List<InventoryOutDetailDto> inventoryOutDetails)
        {
            var currentBalances = (from inventoryOutDetail in inventoryOutDetails
                                   select new ItemCurrentBalanceDto()
                                   {
                                       StoreId = storeId,
                                       ItemId = inventoryOutDetail.ItemId,
                                       ItemPackageId = inventoryOutDetail.ItemPackageId,
                                       ExpireDate = inventoryOutDetail.ExpireDate,
                                       BatchNumber = inventoryOutDetail.BatchNumber,
                                       OutQuantity = inventoryOutDetail.Quantity,
                                   }).ToList();

            return currentBalances;
        }

        private async Task SaveCostCenters(int inventoryOutHeaderId, List<InventoryOutDetailDto> inventoryOutDetails)
        {
            List<CostCenterJournalDetailDto> costCenterJournalDetailList = new List<CostCenterJournalDetailDto>();

            foreach (var inventoryOutDetail in inventoryOutDetails)
            {
                foreach (var costCenter in inventoryOutDetail.CostCenters)
                {
                    costCenter.ReferenceDetailId = inventoryOutDetail.InventoryOutDetailId;
                    costCenterJournalDetailList.Add(costCenter);
                }
            }

            await _costCenterJournalDetailService.SaveCostCenterJournalDetails(inventoryOutHeaderId, costCenterJournalDetailList, MenuCodeData.InventoryOut);
        }
    }
}
