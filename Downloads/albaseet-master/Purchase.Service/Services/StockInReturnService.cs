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
    public class StockInReturnService : IStockInReturnService
    {
        private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
        private readonly IStockInReturnDetailService _stockInReturnDetailService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IStockInReturnDetailTaxService _stockInReturnDetailTaxService;
        private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
        private readonly IGenericMessageService _genericMessageService;

        public StockInReturnService(IStockInReturnHeaderService stockInReturnHeaderService, IStockInReturnDetailService stockInReturnDetailService, IMenuNoteService menuNoteService, IStockInReturnDetailTaxService stockInReturnDetailTaxService, IItemCurrentBalanceService itemCurrentBalanceService, IGenericMessageService genericMessageService)
        {
            _stockInReturnHeaderService = stockInReturnHeaderService;
            _stockInReturnDetailService = stockInReturnDetailService;
            _menuNoteService = menuNoteService;
            _stockInReturnDetailTaxService = stockInReturnDetailTaxService;
            _itemCurrentBalanceService = itemCurrentBalanceService;
            _genericMessageService = genericMessageService;
        }

        public List<RequestChangesDto> GetStockInReturnRequestChanges(StockInReturnDto oldItem, StockInReturnDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.StockInReturnHeader, newItem.StockInReturnHeader);
            requestChanges.AddRange(items);

            if (oldItem.StockInReturnDetails.Any() && newItem.StockInReturnDetails.Any())
            {
                var oldCount = oldItem.StockInReturnDetails.Count;
                var newCount = newItem.StockInReturnDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.StockInReturnDetails[i], newItem.StockInReturnDetails[i]);
                            requestChanges.AddRange(changes);

                            var detailTaxChanges = GetStockInReturnDetailTaxesRequestChanges(oldItem.StockInReturnDetails[i], newItem.StockInReturnDetails[i]);
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

            requestChanges.RemoveAll(x => x.ColumnName == "StockInReturnDetailTaxes" || x.ColumnName == "ItemTaxData");

            return requestChanges;
        }

        private static List<RequestChangesDto> GetStockInReturnDetailTaxesRequestChanges(StockInReturnDetailDto oldItem, StockInReturnDetailDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            if (oldItem.StockInReturnDetailTaxes.Any() && newItem.StockInReturnDetailTaxes.Any())
            {
                var oldCount = oldItem.StockInReturnDetailTaxes.Count;
                var newCount = newItem.StockInReturnDetailTaxes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.StockInReturnDetailTaxes[i], newItem.StockInReturnDetailTaxes[i]);
                            requestChanges.AddRange(changes);

                            index++;
                            break;
                        }
                    }
                }
            }

            return requestChanges;
        }

        public async Task<ResponseDto> SaveStockInReturn(StockInReturnDto stockInReturn, bool hasApprove, bool approved, int? requestId, string? documentReference = null, bool shouldInitializeFlags = false)
        {
            if (stockInReturn.StockInReturnHeader != null)
            {
                if (stockInReturn.StockInReturnHeader.StockTypeId != StockTypeData.ReceiptStatementReturn && stockInReturn.StockInReturnHeader.StockTypeId != StockTypeData.ReceiptFromPurchaseInvoiceOnTheWayReturn && stockInReturn.StockInReturnHeader.StockTypeId != StockTypeData.ReturnFromPurchaseInvoice)
                {
                    return new ResponseDto{ Message = "StockTypeId is invalid" };
                }

                List<StockInReturnDetailDto>? oldStockInReturnDetails = null;
                int? oldStoreId = null;
                if (stockInReturn.StockInReturnHeader.StockInReturnHeaderId != 0)
                {
                    oldStockInReturnDetails = await _stockInReturnDetailService.GetStockInReturnDetails(stockInReturn.StockInReturnHeader.StockInReturnHeaderId);
                    oldStoreId = (await _stockInReturnHeaderService.GetStockInReturnHeaderById(stockInReturn.StockInReturnHeader.StockInReturnHeaderId))?.StoreId;
                }

                var result = await _stockInReturnHeaderService.SaveStockInReturnHeader(stockInReturn.StockInReturnHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags);
                if (result.Success)
                {
                    var modifiedStockInReturnDetails = await _stockInReturnDetailService.SaveStockInReturnDetails(result.Id, stockInReturn.StockInReturnDetails);
                    await SaveStockInReturnDetailTaxes(result.Id, modifiedStockInReturnDetails);
                    await _stockInReturnDetailService.DeleteStockInReturnDetailList(modifiedStockInReturnDetails, result.Id);

                    if (stockInReturn.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(stockInReturn.MenuNotes, result.Id);
                    }

                    await UpdateCurrentBalances(oldStoreId, stockInReturn.StockInReturnHeader.StoreId, oldStockInReturnDetails, stockInReturn.StockInReturnDetails);
                }

                return result;
            }
            return new ResponseDto { Message = "Header should not be null" };
        }

        private async Task SaveStockInReturnDetailTaxes(int stockInReturnHeaderId, List<StockInReturnDetailDto> stockInReturnDetails)
        {
            List<StockInReturnDetailTaxDto> stockInReturnDetailTaxes = new List<StockInReturnDetailTaxDto>();

            foreach (var stockInReturnDetail in stockInReturnDetails)
            {
                foreach (var stockInReturnDetailTax in stockInReturnDetail.StockInReturnDetailTaxes)
                {
                    stockInReturnDetailTax.StockInReturnDetailId = stockInReturnDetail.StockInReturnDetailId;
                    stockInReturnDetailTaxes.Add(stockInReturnDetailTax);
                }
            }

            await _stockInReturnDetailTaxService.SaveStockInReturnDetailTaxes(stockInReturnHeaderId, stockInReturnDetailTaxes);
        }

        public async Task<ResponseDto> DeleteStockInReturn(int stockInReturnHeaderId, int menuCode)
        {
            var stockInReturnHeader = await _stockInReturnHeaderService.GetStockInReturnHeaderById(stockInReturnHeaderId);
            if (stockInReturnHeader == null) return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound)};

            var stockInReturnDetails = await _stockInReturnDetailService.GetStockInReturnDetails(stockInReturnHeaderId);

            await _menuNoteService.DeleteMenuNotes(menuCode, stockInReturnHeaderId);
            await _stockInReturnDetailTaxService.DeleteStockInReturnDetailTaxes(stockInReturnHeaderId);
            await _stockInReturnDetailService.DeleteStockInReturnDetails(stockInReturnHeaderId);
            var result = await _stockInReturnHeaderService.DeleteStockInReturnHeader(stockInReturnHeaderId, menuCode);

            if(stockInReturnHeader is not null && result.Success)
            {
                await DeductCurrentBalances(stockInReturnHeader.StoreId, stockInReturnDetails);
            }
            return result;
        }

        private async Task UpdateCurrentBalances(int? oldStoreId, int newStoreId, List<StockInReturnDetailDto>? oldStockInReturnDetails, List<StockInReturnDetailDto> newStockInReturnDetails)
        {
            var newItemBalances = GetItemCurrentBalancesFromStockInReturn(newStoreId, newStockInReturnDetails);
            if (oldStockInReturnDetails is null || oldStoreId is null)
            {
                await _itemCurrentBalanceService.InventoryOut(newStoreId, newItemBalances);
            }
            else
            {
                if (oldStoreId == newStoreId)
                {
                    var oldItemBalances = GetItemCurrentBalancesFromStockInReturn(newStoreId, oldStockInReturnDetails);
                    await _itemCurrentBalanceService.InventoryInOut(newStoreId, oldItemBalances, newItemBalances);
                }
                else
                {
                    await DeductCurrentBalances((int)oldStoreId, oldStockInReturnDetails);
                    await _itemCurrentBalanceService.InventoryOut(newStoreId, newItemBalances);
                }
            }
        }

        private async Task DeductCurrentBalances(int storeId, List<StockInReturnDetailDto> stockInReturnDetails)
        {
            var oldItemCurrentBalances = GetItemCurrentBalancesFromStockInReturn(storeId, stockInReturnDetails);

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

        private List<ItemCurrentBalanceDto> GetItemCurrentBalancesFromStockInReturn(int storeId, List<StockInReturnDetailDto> stockInReturnDetails)
        {
            var currentBalances = (from stockInReturnDetail in stockInReturnDetails
                                   select new ItemCurrentBalanceDto()
                                   {
                                       StoreId = storeId,
                                       ItemId = stockInReturnDetail.ItemId,
                                       ItemPackageId = stockInReturnDetail.ItemPackageId,
                                       ExpireDate = stockInReturnDetail.ExpireDate,
                                       BatchNumber = stockInReturnDetail.BatchNumber,
                                       OutQuantity = stockInReturnDetail.Quantity + stockInReturnDetail.BonusQuantity,
                                       //PendingOutQuantity = stockInReturnDetail.PendingQuantity
                                   }).ToList();

            return currentBalances;
        }
    }
}
