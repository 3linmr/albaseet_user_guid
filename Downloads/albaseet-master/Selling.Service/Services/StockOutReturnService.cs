using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.CoreOne.Contracts.Menus;

namespace Sales.Service.Services
{
    public class StockOutReturnService: IStockOutReturnService
    {
        private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
        private readonly IStockOutReturnDetailService _stockOutReturnDetailService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IStockOutReturnDetailTaxService _stockOutReturnDetailTaxService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemService _itemService;
        private readonly IItemCostService _itemCostService;
        private readonly IItemPackingService _itemPackingService;
        private readonly ISalesInvoiceService _salesInvoiceService;
        private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
        private readonly IGenericMessageService _genericMessageService;

        public StockOutReturnService(IStockOutReturnHeaderService stockOutReturnHeaderService, IStockOutReturnDetailService stockOutReturnDetailService, IMenuNoteService menuNoteService, IStockOutReturnDetailTaxService stockOutReturnDetailTaxService, IHttpContextAccessor httpContextAccessor, IItemService itemService, IItemCostService itemCostService, IItemPackingService itemPackingService, ISalesInvoiceService salesInvoiceService, IItemCurrentBalanceService itemCurrentBalanceService, IGenericMessageService genericMessageService)
        {
            _stockOutReturnHeaderService = stockOutReturnHeaderService;
            _stockOutReturnDetailService = stockOutReturnDetailService;
            _menuNoteService = menuNoteService;
            _stockOutReturnDetailTaxService = stockOutReturnDetailTaxService;
            _httpContextAccessor = httpContextAccessor;
            _itemService = itemService;
            _itemCostService = itemCostService;
            _itemPackingService = itemPackingService;
            _salesInvoiceService = salesInvoiceService;
            _itemCurrentBalanceService = itemCurrentBalanceService;
            _genericMessageService = genericMessageService;
        }
        public List<RequestChangesDto> GetStockOutReturnRequestChanges(StockOutReturnDto oldItem, StockOutReturnDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.StockOutReturnHeader, newItem.StockOutReturnHeader);
            requestChanges.AddRange(items);

            if (oldItem.StockOutReturnDetails.Any() && newItem.StockOutReturnDetails.Any())
            {
                var oldCount = oldItem.StockOutReturnDetails.Count;
                var newCount = newItem.StockOutReturnDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.StockOutReturnDetails[i], newItem.StockOutReturnDetails[i]);
                            requestChanges.AddRange(changes);

                            var detailTaxChanges = GetStockOutReturnDetailTaxesRequestChanges(oldItem.StockOutReturnDetails[i], newItem.StockOutReturnDetails[i]);
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

            requestChanges.RemoveAll(x => x.ColumnName == "StockOutReturnDetailTaxes" || x.ColumnName == "ItemTaxData");

            return requestChanges;
        }

        private static List<RequestChangesDto> GetStockOutReturnDetailTaxesRequestChanges(StockOutReturnDetailDto oldItem, StockOutReturnDetailDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            if (oldItem.StockOutReturnDetailTaxes.Any() && newItem.StockOutReturnDetailTaxes.Any())
            {
                var oldCount = oldItem.StockOutReturnDetailTaxes.Count;
                var newCount = newItem.StockOutReturnDetailTaxes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.StockOutReturnDetailTaxes[i], newItem.StockOutReturnDetailTaxes[i]);
                            requestChanges.AddRange(changes);

                            index++;
                            break;
                        }
                    }
                }
            }

            return requestChanges;
        }

        public async Task<StockOutReturnDto> GetStockOutReturn(int stockOutReturnHeaderId)
        {
            var header = await _stockOutReturnHeaderService.GetStockOutReturnHeaderById(stockOutReturnHeaderId);
            if (header == null) { return new StockOutReturnDto(); }

            var details = await _stockOutReturnDetailService.GetStockOutReturnDetails(stockOutReturnHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(StockTypeData.ToMenuCode(header.StockTypeId), stockOutReturnHeaderId).ToListAsync();
            var stockOutReturnDetailTaxes = await _stockOutReturnDetailTaxService.GetStockOutReturnDetailTaxes(stockOutReturnHeaderId).ToListAsync();

            foreach (var detail in details)
            {
                detail.StockOutReturnDetailTaxes = stockOutReturnDetailTaxes.Where(x => x.StockOutReturnDetailId == detail.StockOutReturnDetailId).ToList();
            }

            return new StockOutReturnDto() { StockOutReturnHeader = header, StockOutReturnDetails = details, MenuNotes = menuNotes };
        }

        public async Task<ResponseDto> SaveStockOutReturn(StockOutReturnDto stockOutReturn, bool hasApprove, bool approved, int? requestId, string? documentReference = null, bool shouldInitializeFlags = false)
        {
            if (stockOutReturn.StockOutReturnHeader != null)
            {
                List<StockOutReturnDetailDto>? oldStockOutReturnDetails = null;
                int? oldStoreId = null;
                if (stockOutReturn.StockOutReturnHeader.StockOutReturnHeaderId != 0)
                {
                    oldStockOutReturnDetails = await _stockOutReturnDetailService.GetStockOutReturnDetails(stockOutReturn.StockOutReturnHeader.StockOutReturnHeaderId);
                    oldStoreId = (await _stockOutReturnHeaderService.GetStockOutReturnHeaderById(stockOutReturn.StockOutReturnHeader.StockOutReturnHeaderId))?.StoreId;
                }

                var result = await SaveStockOutReturnInternal(stockOutReturn, hasApprove, approved, requestId, documentReference, shouldInitializeFlags);
                if (result.Success)
                {
                    await UpdateCurrentBalances(stockOutReturn.StockOutReturnHeader.StockTypeId, oldStoreId, stockOutReturn.StockOutReturnHeader.StoreId, oldStockOutReturnDetails, stockOutReturn.StockOutReturnDetails);
                }

                return result;
            }
            return new ResponseDto { Message = "Header should not be null" };
        }

		public async Task<ResponseDto> SaveStockOutReturnWithoutUpdatingBalances(StockOutReturnDto stockOutReturn, bool hasApprove, bool approved, int? requestId, string? documentReference = null, bool shouldInitializeFlags = false)
		{
			return await SaveStockOutReturnInternal(stockOutReturn, hasApprove, approved, requestId, documentReference, shouldInitializeFlags);
		}

		private async Task<ResponseDto> SaveStockOutReturnInternal(StockOutReturnDto stockOutReturn, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags)
		{
			if (stockOutReturn.StockOutReturnHeader != null)
			{

				if (stockOutReturn.StockOutReturnHeader.StockTypeId != StockTypeData.StockOutReturnFromStockOut && stockOutReturn.StockOutReturnHeader.StockTypeId != StockTypeData.StockOutReturnFromInvoicedStockOut && stockOutReturn.StockOutReturnHeader.StockTypeId != StockTypeData.StockOutReturnFromSalesInvoice)
				{
					return new ResponseDto { Message = "StockTypeId is invalid" };
				}

				if (!stockOutReturn.StockOutReturnDetails.Any()) return new ResponseDto { Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockOutReturn.StockOutReturnHeader!.StockTypeId), GenericMessageData.DetailIsEmpty) };

				var result = await _stockOutReturnHeaderService.SaveStockOutReturnHeader(stockOutReturn.StockOutReturnHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags);
				if (result.Success)
				{
					var modifiedStockOutReturnDetails = await _stockOutReturnDetailService.SaveStockOutReturnDetails(result.Id, stockOutReturn.StockOutReturnDetails);
					await SaveStockOutReturnDetailTaxes(result.Id, modifiedStockOutReturnDetails);
					await _stockOutReturnDetailService.DeleteStockOutReturnDetailList(modifiedStockOutReturnDetails, result.Id);

					if (stockOutReturn.MenuNotes != null)
					{
						await _menuNoteService.SaveMenuNotes(stockOutReturn.MenuNotes, result.Id);
					}
				}

				return result;
			}
			return new ResponseDto { Message = "Header should not be null" };
		}

		private async Task SaveStockOutReturnDetailTaxes(int stockOutReturnHeaderId, List<StockOutReturnDetailDto> stockOutReturnDetails)
        {
            List<StockOutReturnDetailTaxDto> stockOutReturnDetailTaxes = new List<StockOutReturnDetailTaxDto>();

            foreach (var stockOutReturnDetail in stockOutReturnDetails)
            {
                foreach (var stockOutReturnDetailTax in stockOutReturnDetail.StockOutReturnDetailTaxes)
                {
                    stockOutReturnDetailTax.StockOutReturnDetailId = stockOutReturnDetail.StockOutReturnDetailId;
                    stockOutReturnDetailTaxes.Add(stockOutReturnDetailTax);
                }
            }

            await _stockOutReturnDetailTaxService.SaveStockOutReturnDetailTaxes(stockOutReturnHeaderId, stockOutReturnDetailTaxes);
        }

        public async Task<ResponseDto> DeleteStockOutReturn(int stockOutReturnHeaderId, int menuCode)
        {
            var stockOutReturnHeader = await _stockOutReturnHeaderService.GetStockOutReturnHeaderById(stockOutReturnHeaderId);
            if (stockOutReturnHeader == null) return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };

            var stockOutReturnDetails = await _stockOutReturnDetailService.GetStockOutReturnDetails(stockOutReturnHeaderId);

            var result = await DeleteStockOutReturnInternal(stockOutReturnHeaderId, menuCode);

            if (result.Success)
            {
                await DeductCurrentBalances(stockOutReturnHeader.StockTypeId, stockOutReturnHeader.StoreId, stockOutReturnDetails);
            }
            return result;
        }

		public async Task<ResponseDto> DeleteStockOutReturnWithoutUpdatingBalances(int stockOutReturnHeaderId, int menuCode)
		{
			return await DeleteStockOutReturnInternal(stockOutReturnHeaderId, menuCode);
		}

		private async Task<ResponseDto> DeleteStockOutReturnInternal(int stockOutReturnHeaderId, int menuCode)
		{
			await _menuNoteService.DeleteMenuNotes(menuCode, stockOutReturnHeaderId);
			await _stockOutReturnDetailTaxService.DeleteStockOutReturnDetailTaxes(stockOutReturnHeaderId);
			await _stockOutReturnDetailService.DeleteStockOutReturnDetails(stockOutReturnHeaderId);

			return await _stockOutReturnHeaderService.DeleteStockOutReturnHeader(stockOutReturnHeaderId, menuCode);
		}

		private async Task UpdateCurrentBalances(byte stockTypeId, int? oldStoreId, int newStoreId, List<StockOutReturnDetailDto>? oldStockOutReturnDetails, List<StockOutReturnDetailDto> newStockOutReturnDetails)
		{
			var newItemBalances = GetItemCurrentBalancesFromStockOutReturn(newStoreId, newStockOutReturnDetails);
			if (oldStockOutReturnDetails is null || oldStoreId is null)
			{
				await _itemCurrentBalanceService.InventoryIn(newStoreId, newItemBalances);
			}
			else
			{
				if (oldStoreId == newStoreId)
				{
					var oldItemBalances = GetItemCurrentBalancesFromStockOutReturn(newStoreId, oldStockOutReturnDetails);
					await _itemCurrentBalanceService.InventoryInOut(newStoreId, oldItemBalances, newItemBalances);
				}
				else
				{
					await DeductCurrentBalances(StockTypeData.StockOutReturnFromStockOut, (int)oldStoreId, oldStockOutReturnDetails);
					await _itemCurrentBalanceService.InventoryIn(newStoreId, newItemBalances);
				}
			}
		}

        private async Task DeductCurrentBalances(byte stockTypeId, int storeId, List<StockOutReturnDetailDto> stockOutReturnDetails)
        {
			var oldItemCurrentBalances = GetItemCurrentBalancesFromStockOutReturn(storeId, stockOutReturnDetails);

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

        private List<ItemCurrentBalanceDto> GetItemCurrentBalancesFromStockOutReturn(int storeId, List<StockOutReturnDetailDto> stockOutReturnDetails)
        {
            var currentBalances = (from stockOutReturnDetail in stockOutReturnDetails
                                   select new ItemCurrentBalanceDto()
                                   {
                                       StoreId = storeId,
                                       ItemId = stockOutReturnDetail.ItemId,
                                       ItemPackageId = stockOutReturnDetail.ItemPackageId,
                                       ExpireDate = stockOutReturnDetail.ExpireDate,
                                       BatchNumber = stockOutReturnDetail.BatchNumber,
                                       InQuantity = stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity
                                   }).ToList();

            return currentBalances;
        }
    }
}
