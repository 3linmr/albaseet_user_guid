using Purchases.CoreOne.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using Shared.CoreOne.Contracts.Inventory;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Modules;
using Shared.Service.Logic.Calculation;
using Shared.CoreOne.Contracts.Taxes;
using Shared.Service.Services.Taxes;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.StaticData;
using System.ComponentModel.DataAnnotations;
using Shared.Service.Services.Modules;
using System.Reflection.PortableExecutable;
using Purchases.CoreOne.Models.Domain;
using System.Runtime.CompilerServices;
using Shared.Helper.Extensions;
using Shared.Service.Services.Items;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.Service.Services.Inventory;
using Shared.CoreOne.Contracts.Menus;

namespace Purchases.Service.Services
{
    public class StockInService : IStockInService
    {
        private readonly IStockInHeaderService _stockInHeaderService;
        private readonly IStockInDetailService _stockInDetailService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IStockInDetailTaxService _stockInDetailTaxService;
        private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
        private readonly IGenericMessageService _genericMessageService;

        public StockInService(IStockInHeaderService stockInHeaderService, IStockInDetailService stockInDetailService, IMenuNoteService menuNoteService, IStockInDetailTaxService stockInDetailTaxService, IItemCurrentBalanceService itemCurrentBalanceService, IGenericMessageService genericMessageService)
        {
            _stockInHeaderService = stockInHeaderService;
            _stockInDetailService = stockInDetailService;
            _menuNoteService = menuNoteService;
            _stockInDetailTaxService = stockInDetailTaxService;
            _itemCurrentBalanceService = itemCurrentBalanceService;
            _genericMessageService = genericMessageService;
        }

        public List<RequestChangesDto> GetStockInRequestChanges(StockInDto oldItem, StockInDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.StockInHeader, newItem.StockInHeader);
            requestChanges.AddRange(items);

            if (oldItem.StockInDetails.Any() && newItem.StockInDetails.Any())
            {
                var oldCount = oldItem.StockInDetails.Count;
                var newCount = newItem.StockInDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.StockInDetails[i], newItem.StockInDetails[i]);
                            requestChanges.AddRange(changes);

                            var detailTaxChanges = GetStockInDetailTaxesRequestChanges(oldItem.StockInDetails[i], newItem.StockInDetails[i]);
                            requestChanges.AddRange(detailTaxChanges);

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

            requestChanges.RemoveAll(x => x.ColumnName == "StockInDetailTaxes" || x.ColumnName == "ItemTaxData");

            return requestChanges;
        }

        private static List<RequestChangesDto> GetStockInDetailTaxesRequestChanges(StockInDetailDto oldItem, StockInDetailDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            if (oldItem.StockInDetailTaxes.Any() && newItem.StockInDetailTaxes.Any())
            {
                var oldCount = oldItem.StockInDetailTaxes.Count;
                var newCount = newItem.StockInDetailTaxes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.StockInDetailTaxes[i], newItem.StockInDetailTaxes[i]);
                            requestChanges.AddRange(changes);

                            index++;
                            break;
                        }
                    }
                }
            }

            return requestChanges;
        }

        public async Task<ResponseDto> SaveStockIn(StockInDto stockIn, bool hasApprove, bool approved, int? requestId, string? documentReference = null, bool shouldInitializeFlags = false)
        {
            if (stockIn.StockInHeader != null)
            {
                if(stockIn.StockInHeader.StockTypeId != StockTypeData.ReceiptStatement && stockIn.StockInHeader.StockTypeId != StockTypeData.ReceiptFromPurchaseInvoiceOnTheWay)
                {
                    return new ResponseDto { Message = "StockTypeId is invalid" };
                }

                List<StockInDetailDto>? oldStockInDetails = null;
                int? oldStoreId = null;
                if (stockIn.StockInHeader.StockInHeaderId != 0)
                {
                    oldStockInDetails = await _stockInDetailService.GetStockInDetails(stockIn.StockInHeader.StockInHeaderId);
                    oldStoreId = (await _stockInHeaderService.GetStockInHeaderById(stockIn.StockInHeader.StockInHeaderId))?.StoreId;
                }

                var result = await _stockInHeaderService.SaveStockInHeader(stockIn.StockInHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags);
                if (result.Success)
                {
                    var modifiedStockInDetails = await _stockInDetailService.SaveStockInDetails(result.Id, stockIn.StockInDetails);
                    await SaveStockInDetailTaxes(result.Id, modifiedStockInDetails);
                    await _stockInDetailService.DeleteStockInDetailList(modifiedStockInDetails, result.Id);

                    if (stockIn.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(stockIn.MenuNotes, result.Id);
                    }

                    await UpdateCurrentBalances(oldStoreId, stockIn.StockInHeader.StoreId, oldStockInDetails, stockIn.StockInDetails);
                }

                return result;
            }
            return new ResponseDto{ Message = "Header should not be null" };
        }

        private async Task SaveStockInDetailTaxes(int stockInHeaderId, List<StockInDetailDto> stockInDetails)
        {
            List<StockInDetailTaxDto> stockInDetailTaxes = new List<StockInDetailTaxDto>();

            foreach (var stockInDetail in stockInDetails)
            {
                foreach (var stockInDetailTax in stockInDetail.StockInDetailTaxes)
                {
                    stockInDetailTax.StockInDetailId = stockInDetail.StockInDetailId;
                    stockInDetailTaxes.Add(stockInDetailTax);
                }
            }

            await _stockInDetailTaxService.SaveStockInDetailTaxes(stockInHeaderId, stockInDetailTaxes);
        }

        public async Task<ResponseDto> DeleteStockIn(int stockInHeaderId, int menuCode)
        {
            var stockInHeader = await _stockInHeaderService.GetStockInHeaderById(stockInHeaderId);
            if(stockInHeader == null) return new ResponseDto{ Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound)};
            
            var stockInDetails = await _stockInDetailService.GetStockInDetails(stockInHeaderId);

            await _menuNoteService.DeleteMenuNotes(menuCode, stockInHeaderId);
            await _stockInDetailTaxService.DeleteStockInDetailTaxes(stockInHeaderId);
            await _stockInDetailService.DeleteStockInDetails(stockInHeaderId);
            var result = await _stockInHeaderService.DeleteStockInHeader(stockInHeaderId, menuCode);

            if(result.Success)
            {
                await DeductCurrentBalances(stockInHeader.StoreId, stockInDetails);
            }
            return result;
        }

        private async Task UpdateCurrentBalances(int? oldStoreId, int newStoreId, List<StockInDetailDto>? oldStockInDetails, List<StockInDetailDto> newStockInDetails)
        {
            var newItemBalances = GetItemCurrentBalancesFromStockIn(newStoreId, newStockInDetails);
            if (oldStockInDetails is null || oldStoreId is null)
            {
                await _itemCurrentBalanceService.InventoryIn(newStoreId, newItemBalances);
            }
            else
            {
                if (oldStoreId == newStoreId)
                {
                    var oldItemBalances = GetItemCurrentBalancesFromStockIn(newStoreId, oldStockInDetails);
                    await _itemCurrentBalanceService.InventoryInOut(newStoreId, oldItemBalances, newItemBalances);
                }
                else
                {
                    await DeductCurrentBalances((int)oldStoreId, oldStockInDetails);
                    await _itemCurrentBalanceService.InventoryIn(newStoreId, newItemBalances);
                }
            }
        }

        private async Task DeductCurrentBalances(int storeId, List<StockInDetailDto> stockInDetails)
        {
            var oldItemCurrentBalances = GetItemCurrentBalancesFromStockIn(storeId, stockInDetails);

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

        private List<ItemCurrentBalanceDto> GetItemCurrentBalancesFromStockIn(int storeId, List<StockInDetailDto> stockInDetails)
        {
            var currentBalances = (from stockInDetail in stockInDetails
                                   select new ItemCurrentBalanceDto()
                                   {
                                       StoreId = storeId,
                                       ItemId = stockInDetail.ItemId,
                                       ItemPackageId = stockInDetail.ItemPackageId,
                                       ExpireDate = stockInDetail.ExpireDate,
                                       BatchNumber = stockInDetail.BatchNumber,
                                       InQuantity = stockInDetail.Quantity + stockInDetail.BonusQuantity,
                                       //PendingInQuantity = stockInDetail.PendingQuantity
                                   }).ToList();

            return currentBalances;
        }
    }
}
