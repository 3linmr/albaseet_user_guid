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
    public class StockOutService: IStockOutService
    {
        private readonly IStockOutHeaderService _stockOutHeaderService;
        private readonly IStockOutDetailService _stockOutDetailService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IStockOutDetailTaxService _stockOutDetailTaxService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemService _itemService;
        private readonly IItemCostService _itemCostService;
        private readonly IItemPackingService _itemPackingService;
        private readonly ISalesInvoiceService _salesInvoiceService;
        private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
        private readonly IGenericMessageService _genericMessageService;

        public StockOutService(IStockOutHeaderService stockOutHeaderService, IStockOutDetailService stockOutDetailService, IMenuNoteService menuNoteService, IStockOutDetailTaxService stockOutDetailTaxService, IHttpContextAccessor httpContextAccessor, IItemService itemService, IItemCostService itemCostService, IItemPackingService itemPackingService, ISalesInvoiceService salesInvoiceService, IItemCurrentBalanceService itemCurrentBalanceService, IGenericMessageService genericMessageService)
        {
            _stockOutHeaderService = stockOutHeaderService;
            _stockOutDetailService = stockOutDetailService;
            _menuNoteService = menuNoteService;
            _stockOutDetailTaxService = stockOutDetailTaxService;
            _httpContextAccessor = httpContextAccessor;
            _itemService = itemService;
            _itemCostService = itemCostService;
            _itemPackingService = itemPackingService;
            _salesInvoiceService = salesInvoiceService;
            _itemCurrentBalanceService = itemCurrentBalanceService;
            _genericMessageService = genericMessageService;
        }
        public List<RequestChangesDto> GetStockOutRequestChanges(StockOutDto oldItem, StockOutDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.StockOutHeader, newItem.StockOutHeader);
            requestChanges.AddRange(items);

            if (oldItem.StockOutDetails.Any() && newItem.StockOutDetails.Any())
            {
                var oldCount = oldItem.StockOutDetails.Count;
                var newCount = newItem.StockOutDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.StockOutDetails[i], newItem.StockOutDetails[i]);
                            requestChanges.AddRange(changes);

                            var detailTaxChanges = GetStockOutDetailTaxesRequestChanges(oldItem.StockOutDetails[i], newItem.StockOutDetails[i]);
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

            requestChanges.RemoveAll(x => x.ColumnName == "StockOutDetailTaxes" || x.ColumnName == "ItemTaxData");

            return requestChanges;
        }

        private static List<RequestChangesDto> GetStockOutDetailTaxesRequestChanges(StockOutDetailDto oldItem, StockOutDetailDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            if (oldItem.StockOutDetailTaxes.Any() && newItem.StockOutDetailTaxes.Any())
            {
                var oldCount = oldItem.StockOutDetailTaxes.Count;
                var newCount = newItem.StockOutDetailTaxes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.StockOutDetailTaxes[i], newItem.StockOutDetailTaxes[i]);
                            requestChanges.AddRange(changes);

                            index++;
                            break;
                        }
                    }
                }
            }

            return requestChanges;
        }

        public async Task<StockOutDto> GetStockOut(int stockOutHeaderId)
        {
            var header = await _stockOutHeaderService.GetStockOutHeaderById(stockOutHeaderId);
            if (header == null) { return new StockOutDto(); }

            var details = await _stockOutDetailService.GetStockOutDetails(stockOutHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(StockTypeData.ToMenuCode(header.StockTypeId), stockOutHeaderId).ToListAsync();
            var stockOutDetailTaxes = await _stockOutDetailTaxService.GetStockOutDetailTaxes(stockOutHeaderId).ToListAsync();

            foreach (var detail in details)
            {
                detail.StockOutDetailTaxes = stockOutDetailTaxes.Where(x => x.StockOutDetailId == detail.StockOutDetailId).ToList();
            }

            return new StockOutDto() { StockOutHeader = header, StockOutDetails = details, MenuNotes = menuNotes };
        }

        public async Task<ResponseDto> SaveStockOut(StockOutDto stockOut, bool hasApprove, bool approved, int? requestId, string? documentReference = null, bool shouldInitializeFlags = false)
        {
            if (stockOut.StockOutHeader != null)
            {
                List<StockOutDetailDto>? oldStockOutDetails = null;
                int? oldStoreId = null;
                if (stockOut.StockOutHeader.StockOutHeaderId != 0)
                {
                    oldStockOutDetails = await _stockOutDetailService.GetStockOutDetails(stockOut.StockOutHeader.StockOutHeaderId);
                    oldStoreId = (await _stockOutHeaderService.GetStockOutHeaderById(stockOut.StockOutHeader.StockOutHeaderId))?.StoreId;
                }

                var result = await SaveStockOutInternal(stockOut, hasApprove, approved, requestId, documentReference, shouldInitializeFlags);
                if (result.Success)
                {
                    await UpdateCurrentBalances(stockOut.StockOutHeader.StockTypeId, oldStoreId, stockOut.StockOutHeader.StoreId, oldStockOutDetails, stockOut.StockOutDetails);
                }

                return result;
            }
            return new ResponseDto { Message = "Header should not be null" };
        }

		public async Task<ResponseDto> SaveStockOutWithoutUpdatingBalances(StockOutDto stockOut, bool hasApprove, bool approved, int? requestId, string? documentReference = null, bool shouldInitializeFlags = false)
		{
			return await SaveStockOutInternal(stockOut, hasApprove, approved, requestId, documentReference, shouldInitializeFlags);
		}

		private async Task<ResponseDto> SaveStockOutInternal(StockOutDto stockOut, bool hasApprove, bool approved, int? requestId, string? documentReference, bool shouldInitializeFlags)
		{
			if (stockOut.StockOutHeader != null)
			{
				if (stockOut.StockOutHeader.StockTypeId != StockTypeData.StockOutFromProformaInvoice && stockOut.StockOutHeader.StockTypeId != StockTypeData.StockOutFromSalesInvoice)
				{
					return new ResponseDto { Message = "StockTypeId is invalid" };
				}

				if (!stockOut.StockOutDetails.Any()) return new ResponseDto { Message = await _genericMessageService.GetMessage(StockTypeData.ToMenuCode(stockOut.StockOutHeader!.StockTypeId), GenericMessageData.DetailIsEmpty) };

				var result = await _stockOutHeaderService.SaveStockOutHeader(stockOut.StockOutHeader, hasApprove, approved, requestId, documentReference, shouldInitializeFlags);
				if (result.Success)
				{
					var modifiedStockOutDetails = await _stockOutDetailService.SaveStockOutDetails(result.Id, stockOut.StockOutDetails);
					await SaveStockOutDetailTaxes(result.Id, modifiedStockOutDetails);
					await _stockOutDetailService.DeleteStockOutDetailList(modifiedStockOutDetails, result.Id);

					if (stockOut.MenuNotes != null)
					{
						await _menuNoteService.SaveMenuNotes(stockOut.MenuNotes, result.Id);
					}
				}

				return result;
			}
			return new ResponseDto { Message = "Header should not be null" };
		}

		private async Task SaveStockOutDetailTaxes(int stockOutHeaderId, List<StockOutDetailDto> stockOutDetails)
        {
            List<StockOutDetailTaxDto> stockOutDetailTaxes = new List<StockOutDetailTaxDto>();

            foreach (var stockOutDetail in stockOutDetails)
            {
                foreach (var stockOutDetailTax in stockOutDetail.StockOutDetailTaxes)
                {
                    stockOutDetailTax.StockOutDetailId = stockOutDetail.StockOutDetailId;
                    stockOutDetailTaxes.Add(stockOutDetailTax);
                }
            }

            await _stockOutDetailTaxService.SaveStockOutDetailTaxes(stockOutHeaderId, stockOutDetailTaxes);
        }

        public async Task<ResponseDto> DeleteStockOut(int stockOutHeaderId, int menuCode)
        {
            var stockOutHeader = await _stockOutHeaderService.GetStockOutHeaderById(stockOutHeaderId);
            if (stockOutHeader == null) return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound)};

            var stockOutDetails = await _stockOutDetailService.GetStockOutDetails(stockOutHeaderId);

            var result = await DeleteStockOutInternal(stockOutHeaderId, menuCode);

			if (result.Success)
            {
                await DeductCurrentBalances(stockOutHeader.StockTypeId, stockOutHeader.StoreId, stockOutDetails);
            }
            return result;
        }

		public async Task<ResponseDto> DeleteStockOutWithoutUpdatingBalances(int stockOutHeaderId, int menuCode)
		{
			return await DeleteStockOutInternal(stockOutHeaderId, menuCode);
		}

		private async Task<ResponseDto> DeleteStockOutInternal(int stockOutHeaderId, int menuCode)
		{
			await _menuNoteService.DeleteMenuNotes(menuCode, stockOutHeaderId);
			await _stockOutDetailTaxService.DeleteStockOutDetailTaxes(stockOutHeaderId);
			await _stockOutDetailService.DeleteStockOutDetails(stockOutHeaderId);
			var result = await _stockOutHeaderService.DeleteStockOutHeader(stockOutHeaderId, menuCode);

			return result;
		}

		private async Task UpdateCurrentBalances(int stockTypeId, int? oldStoreId, int newStoreId, List<StockOutDetailDto>? oldStockOutDetails, List<StockOutDetailDto> newStockOutDetails)
        {
			var newItemBalances = GetItemCurrentBalancesFromStockOut(newStoreId, newStockOutDetails);
			if (oldStockOutDetails is null || oldStoreId is null)
			{
				await _itemCurrentBalanceService.InventoryOut(newStoreId, newItemBalances);
			}
			else
			{
				if (oldStoreId == newStoreId)
				{
					var oldItemBalances = GetItemCurrentBalancesFromStockOut(newStoreId, oldStockOutDetails);
					await _itemCurrentBalanceService.InventoryInOut(newStoreId, oldItemBalances, newItemBalances);
				}
				else
				{
					await DeductCurrentBalances(stockTypeId, (int)oldStoreId, oldStockOutDetails);
					await _itemCurrentBalanceService.InventoryOut(newStoreId, newItemBalances);
				}
			}
		}

        private async Task DeductCurrentBalances(int stockTypeId, int storeId, List<StockOutDetailDto> stockOutDetails)
        {
			var oldItemCurrentBalances = GetItemCurrentBalancesFromStockOut(storeId, stockOutDetails);

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

		private List<ItemCurrentBalanceDto> GetItemCurrentBalancesFromStockOut(int storeId, List<StockOutDetailDto> stockOutDetails)
        {
            var currentBalances = (from stockOutDetail in stockOutDetails
                                   select new ItemCurrentBalanceDto()
                                   {
                                       StoreId = storeId,
                                       ItemId = stockOutDetail.ItemId,
                                       ItemPackageId = stockOutDetail.ItemPackageId,
                                       ExpireDate = stockOutDetail.ExpireDate,
                                       BatchNumber = stockOutDetail.BatchNumber,
                                       OutQuantity = stockOutDetail.Quantity + stockOutDetail.BonusQuantity,
                                   }).ToList();

            return currentBalances;
        }
    }
}
