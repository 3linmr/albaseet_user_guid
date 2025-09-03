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

namespace Inventory.Service.Services
{
    public class InventoryInService : IInventoryInService
    {
        private readonly IInventoryInHeaderService _inventoryInHeaderService;
        private readonly IInventoryInDetailService _inventoryInDetailService;
        private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
        private readonly ICostCenterJournalDetailService _costCenterJournalDetailService;
        private readonly IMenuNoteService _menuNoteService;

        public InventoryInService(IInventoryInHeaderService inventoryInHeaderService, IInventoryInDetailService inventoryInDetailService, IItemCurrentBalanceService itemCurrentBalanceService, ICostCenterJournalDetailService costCenterJournalDetailService, IMenuNoteService menuNoteService)
        {
            _inventoryInHeaderService = inventoryInHeaderService;
            _inventoryInDetailService = inventoryInDetailService;
            _itemCurrentBalanceService = itemCurrentBalanceService;
            _costCenterJournalDetailService = costCenterJournalDetailService;
            _menuNoteService = menuNoteService;
        }
        public List<RequestChangesDto> GetInventoryInRequestChanges(InventoryInDto oldItem, InventoryInDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.InventoryInHeader, newItem.InventoryInHeader);
            requestChanges.AddRange(items);

            if (oldItem.InventoryInDetails.Any() && newItem.InventoryInDetails.Any())
            {
                var oldCount = oldItem.InventoryInDetails.Count;
                var newCount = newItem.InventoryInDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.InventoryInDetails[i], newItem.InventoryInDetails[i]);
                            requestChanges.AddRange(changes);
                            
                            requestChanges.RemoveAll(x => x.ColumnName == "CostCenters");

                            var costCenterChanges = GetCostCentersRequestChanges(oldItem.InventoryInDetails[i], newItem.InventoryInDetails[i]);
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

        private static List<RequestChangesDto> GetCostCentersRequestChanges(InventoryInDetailDto oldItem, InventoryInDetailDto newItem) 
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

        public async Task<InventoryInDto> GetInventoryIn(int inventoryInHeaderId)
        {
            var header = await _inventoryInHeaderService.GetInventoryInHeaderById(inventoryInHeaderId);
            var details = await _inventoryInDetailService.GetInventoryInDetails(inventoryInHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.InventoryIn, inventoryInHeaderId).ToListAsync();
            var costCenters = await _costCenterJournalDetailService.GetCostCenterJournalDetails(inventoryInHeaderId, MenuCodeData.InventoryIn);
            
            foreach(var detail in details) 
            {
                detail.CostCenters = costCenters.Where(x => x.ReferenceDetailId == detail.InventoryInDetailId).ToList();
            }

            return new InventoryInDto() { InventoryInHeader = header, InventoryInDetails = details, MenuNotes = menuNotes };
        }

        public async Task<ResponseDto> SaveInventoryIn(InventoryInDto inventoryIn, bool hasApprove, bool approved, int? requestId)
        {
            TrimDetailStrings(inventoryIn.InventoryInDetails);
            if (inventoryIn.InventoryInHeader != null)
            {
                List<InventoryInDetailDto>? oldInventoryInDetails = null;
                int? oldStoreId = null;
                if(inventoryIn.InventoryInHeader.InventoryInHeaderId != 0)
                {
                    var oldInventoryIn = await GetInventoryIn(inventoryIn.InventoryInHeader.InventoryInHeaderId);
                    oldInventoryInDetails = oldInventoryIn.InventoryInDetails;
                    oldStoreId = oldInventoryIn.InventoryInHeader?.StoreId;
                }

                var result = await _inventoryInHeaderService.SaveInventoryInHeader(inventoryIn.InventoryInHeader, hasApprove, approved, requestId);
                if (result.Success)
                {
                    var modifiedInventoryInDetailsList = await _inventoryInDetailService.SaveInventoryInDetails(result.Id, inventoryIn.InventoryInDetails);
                    await SaveCostCenters(result.Id, modifiedInventoryInDetailsList);
                    await UpdateCurrentBalances(oldStoreId, inventoryIn.InventoryInHeader.StoreId,oldInventoryInDetails,inventoryIn.InventoryInDetails);
                    if (inventoryIn.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(inventoryIn.MenuNotes, result.Id);
                    }
                }

                return result;
            }
            return new ResponseDto{ Message = "Header should not be null" };
        }

        private void TrimDetailStrings(List<InventoryInDetailDto> inventoryInDetails)
        {
            foreach (var inventoryInDetail in inventoryInDetails)
            {
                inventoryInDetail.BatchNumber = string.IsNullOrWhiteSpace(inventoryInDetail.BatchNumber) ? null : inventoryInDetail.BatchNumber.Trim();
            }
        }

        public async Task<ResponseDto> DeleteInventoryIn(int inventoryInHeaderId)
        {
            var inventoryIn = await GetInventoryIn(inventoryInHeaderId);

            await _menuNoteService.DeleteMenuNotes(MenuCodeData.InventoryIn, inventoryInHeaderId);
            await _costCenterJournalDetailService.DeleteCostCenterJournalDetails(inventoryInHeaderId, MenuCodeData.InventoryIn);
            await _inventoryInDetailService.DeleteInventoryInDetails(inventoryInHeaderId);
            var result = await _inventoryInHeaderService.DeleteInventoryInHeader(inventoryInHeaderId);

            if (inventoryIn.InventoryInHeader is not null && result.Success)
            {
                await DeductCurrentBalances(inventoryIn.InventoryInHeader.StoreId, inventoryIn.InventoryInDetails);
            }

            return result;
        }

        private async Task UpdateCurrentBalances(int? oldStoreId, int newStoreId, List<InventoryInDetailDto>? oldInventoryInDetails, List<InventoryInDetailDto> newInventoryInDetails)
        {
            var newItemBalances = GetItemCurrentBalancesFromInventoryIn(newStoreId, newInventoryInDetails);
            if (oldInventoryInDetails is null || oldStoreId is null)
            {
                await _itemCurrentBalanceService.InventoryIn(newStoreId, newItemBalances);
            }
            else
            {
                if (oldStoreId == newStoreId)
                {
                    var oldItemBalances = GetItemCurrentBalancesFromInventoryIn(newStoreId, oldInventoryInDetails);
                    await _itemCurrentBalanceService.InventoryInOut(newStoreId, oldItemBalances, newItemBalances);
                }
                else
                {
                    await DeductCurrentBalances((int)oldStoreId, oldInventoryInDetails);
                    await _itemCurrentBalanceService.InventoryIn(newStoreId, newItemBalances);
                }
            }
        }

        private async Task DeductCurrentBalances(int storeId, List<InventoryInDetailDto> inventoryInDetails)
        {
            var oldItemCurrentBalances = GetItemCurrentBalancesFromInventoryIn(storeId, inventoryInDetails);

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

        private List<ItemCurrentBalanceDto> GetItemCurrentBalancesFromInventoryIn(int storeId, List<InventoryInDetailDto> inventoryInDetails)
        {
            var currentBalances = (from inventoryInDetail in inventoryInDetails
                                   select new ItemCurrentBalanceDto()
                                   {
                                       StoreId = storeId,
                                       ItemId = inventoryInDetail.ItemId,
                                       ItemPackageId = inventoryInDetail.ItemPackageId,
                                       ExpireDate = inventoryInDetail.ExpireDate,
                                       BatchNumber = inventoryInDetail.BatchNumber,
                                       InQuantity = inventoryInDetail.Quantity
                                   }).ToList();

            return currentBalances;
        }

        private async Task SaveCostCenters(int inventoryInHeaderId, List<InventoryInDetailDto> inventoryInDetails)
        {
            List<CostCenterJournalDetailDto> costCenterJournalDetailList = new List<CostCenterJournalDetailDto>();

            foreach (var inventoryInDetail in inventoryInDetails)
            {
                foreach (var costCenter in inventoryInDetail.CostCenters)
                {
                    costCenter.ReferenceDetailId = inventoryInDetail.InventoryInDetailId;
                    costCenterJournalDetailList.Add(costCenter);
                }
            }

            await _costCenterJournalDetailService.SaveCostCenterJournalDetails(inventoryInHeaderId, costCenterJournalDetailList, MenuCodeData.InventoryIn);
        }
    }
}
