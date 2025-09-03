using Inventory.CoreOne.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Identity;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using static Inventory.Service.Logic.StockTakingLogic;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Modules;
using Inventory.CoreOne.Models.Domain;
using Shared.CoreOne.Models.StaticData;
using System.Reflection.PortableExecutable;
using Microsoft.Extensions.Localization;

namespace Inventory.Service.Services
{
	public class StockTakingService : IStockTakingService
	{
		private readonly IStockTakingHeaderService _stockTakingHeaderService;
		private readonly IStockTakingDetailService _stockTakingDetailService;
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
		private readonly IItemService _itemService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemCostService _itemCostService;
		private readonly IStoreService _storeService;
		private readonly IMenuNoteService _menuNoteService;
		private readonly IStringLocalizer<StockTakingHeaderService> _headerLocalizer;

		public StockTakingService(IStockTakingHeaderService stockTakingHeaderService, IStockTakingDetailService stockTakingDetailService,IItemCurrentBalanceService itemCurrentBalanceService,IItemService itemService,IHttpContextAccessor httpContextAccessor,IItemPackageService itemPackageService,IItemCostService itemCostService, IStoreService storeService, IMenuNoteService menuNoteService, IStringLocalizer<StockTakingHeaderService> headerLocalizer)
		{
			_stockTakingHeaderService = stockTakingHeaderService;
			_stockTakingDetailService = stockTakingDetailService;
			_itemCurrentBalanceService = itemCurrentBalanceService;
			_itemService = itemService;
			_httpContextAccessor = httpContextAccessor;
			_itemPackageService = itemPackageService;
			_itemCostService = itemCostService;
			_storeService = storeService;
			_menuNoteService = menuNoteService;
			_headerLocalizer = headerLocalizer;
		}
		public List<RequestChangesDto> GetStockTakingRequestChanges(StockTakingDto oldItem, StockTakingDto newItem)
		{
			var requestChanges = new List<RequestChangesDto>();
			var items = CompareLogic.GetDifferences(oldItem.StockTakingHeader, newItem.StockTakingHeader);
			requestChanges.AddRange(items);

			if (oldItem.StockTakingDetails.Any() && newItem.StockTakingDetails.Any())
			{
				var oldCount = oldItem.StockTakingDetails.Count;
				var newCount = newItem.StockTakingDetails.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.StockTakingDetails[i], newItem.StockTakingDetails[i]);
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

		public async Task<StockTakingDto> GetStockTaking(int stockTakingHeaderId)
		{
			var header = await _stockTakingHeaderService.GetStockTakingHeaderById(stockTakingHeaderId);
			if(header == null) return new StockTakingDto(); 

			var detail = await _stockTakingDetailService.GetStockTakingDetails(stockTakingHeaderId, header.StoreId);
            var menuNotes = await _menuNoteService.GetMenuNotes(header.IsOpenBalance ? MenuCodeData.StockTakingOpenBalance : MenuCodeData.StockTakingCurrentBalance, stockTakingHeaderId).ToListAsync();
            
			return new StockTakingDto() { StockTakingHeader = header, StockTakingDetails = detail, MenuNotes = menuNotes };
		}

        public async Task<List<StockTakingCarryOverDetailDto>> GetStockTakingCompareData(bool isOpenBalance,int storeId,DateTime carryOverDate, List<int> stockTakingIds)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var stockTakings = await _stockTakingDetailService.GetAll().Where(x => stockTakingIds.Contains(x.StockTakingHeaderId)).ToListAsync();
			var itemsId = stockTakings.Select(x => x.ItemId).ToList();
			var currentBalances = await _itemCurrentBalanceService.GetAll().Where(x=> itemsId.Contains(x.ItemId)).ToListAsync();
			var itemCosts = await _itemCostService.GetAll().Where(x=> itemsId.Contains(x.ItemId)).ToListAsync();
			var items = await _itemService.GetAll().Where(x=> itemsId.Contains(x.ItemId)).ToListAsync();
			var packages = await _itemPackageService.GetCompanyItemPackages().ToListAsync();
			var stockTakingGrouped = stockTakings.GroupBy(x=> new {x.ItemId,x.ItemPackageId,x.BatchNumber,x.ExpireDate}).Select(s=> new StockTakingDetailDto()
			{
				ItemId = s.Key.ItemId,
				ExpireDate = s.Key.ExpireDate,
				BatchNumber = s.Key.BatchNumber,
				ItemPackageId = s.Key.ItemPackageId,
				StoreId = storeId,
				Quantity = s.Sum(x=>x.Quantity),
				ConsumerValue = s.Sum(x=>x.ConsumerValue),
				CostValue = s.Sum(x=>x.CostValue),
				ConsumerPrice = s.Select(x=>x.ConsumerPrice).FirstOrDefault(),
				CostPrice = s.Select(x=>x.CostPrice).FirstOrDefault(),
				CostPackage = s.Select(x=>x.CostPackage).FirstOrDefault(),
				Packing = s.Select(x=>x.Packing).FirstOrDefault(),
				BarCode = s.Select(x=>x.BarCode).FirstOrDefault(),
			}).ToList();

			// TODO Calculate Cost Price On Date

			var data =
				(from stockTaking in stockTakingGrouped
					from item in items.Where(x=>x.ItemId == stockTaking.ItemId)
					from itemPackage in packages.Where(x=>x.ItemPackageId == stockTaking.ItemPackageId)
					from itemCost in itemCosts.Where(x=>x.ItemId == item.ItemId).DefaultIfEmpty()
				 from currentBalance in currentBalances.Where(x=> x.ItemId == stockTaking.ItemId && x.ItemPackageId == stockTaking.ItemPackageId && x.ExpireDate == stockTaking.ExpireDate && x.BatchNumber == stockTaking.BatchNumber && x.StoreId == stockTaking.StoreId).DefaultIfEmpty()
				select new StockTakingCarryOverDetailDto
				{
					ItemId = stockTaking.ItemId,
					ItemPackageId = stockTaking.ItemPackageId,
					ExpireDate = stockTaking.ExpireDate,
					BatchNumber = stockTaking.BatchNumber,
					ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					ItemTypeId = item.ItemTypeId,
					ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					ItemCode = item.ItemCode,
					BarCode = stockTaking.BarCode,
					Packing = stockTaking.Packing,
					StockTakingConsumerPrice = stockTaking.ConsumerPrice,
					StockTakingCostPrice = stockTaking.CostPrice,
					StockTakingCostPackage = stockTaking.CostPackage,
					StockTakingCostValue = stockTaking.CostValue,
					StockTakingQuantity = stockTaking.Quantity,
					StockTakingConsumerValue = stockTaking.ConsumerValue,
					CurrentQuantity = currentBalance != null ? CalculateBalanceQuantity(isOpenBalance, currentBalance.OpenQuantity, currentBalance.InQuantity, currentBalance.OutQuantity, currentBalance.PendingOutQuantity) : 0,
					CurrentConsumerPrice = stockTaking.ConsumerPrice,
					CurrentConsumerValue = currentBalance != null ? CalculateCurrentConsumerValue(isOpenBalance,stockTaking.ConsumerPrice, currentBalance.OpenQuantity, currentBalance.InQuantity, currentBalance.OutQuantity, currentBalance.PendingOutQuantity) : 0,
					CurrentCostPrice = 0,
					ItemCostPrice = itemCost != null ? itemCost.CostPrice : 0,
					CurrentCostValue = 0,
					CurrentCostPackage = 0,
					//CostPrice = stockTaking.CostPrice,
					//CostPackage = stockTaking.CostPackage,
					//BalanceQuantity = currentBalance != null ? CalculateBalanceQuantity(isOpenBalance,currentBalance.OpenQuantity,currentBalance.InQuantity,currentBalance.OutQuantity) : 0,
					//BalanceConsumerValue = currentBalance != null ? CalculateBalanceConsumerValue(isOpenBalance,stockTaking.ConsumerPrice, currentBalance.OpenQuantity, currentBalance.InQuantity, currentBalance.OutQuantity) : 0,
					//BalanceCostValue = currentBalance != null ? CalculateBalanceCostValue(isOpenBalance, stockTaking.CostPackage, currentBalance.OpenQuantity, currentBalance.InQuantity, currentBalance.OutQuantity) : 0,
					OldOpenQuantity = currentBalance != null ? currentBalance.OpenQuantity : 0,
					OpenQuantity = CalculateOpenQuantity(isOpenBalance,stockTaking.Quantity,(currentBalance != null ? currentBalance.OpenQuantity : 0)),
					DifferenceBetweenStockTakingAndCurrent = CalculateDifferenceBetweenStockTakingAndCurrent(isOpenBalance,(currentBalance != null ? currentBalance.OpenQuantity : 0), stockTaking.Quantity, (currentBalance != null ? currentBalance.OpenQuantity : 0), (currentBalance != null ? currentBalance.InQuantity : 0), (currentBalance != null ? currentBalance.OutQuantity : 0), (currentBalance != null ? currentBalance.PendingOutQuantity : 0)),
					InQuantity = CalculateInQuantity(isOpenBalance,stockTaking.Quantity, (currentBalance != null ? currentBalance.OpenQuantity : 0), (currentBalance != null ? currentBalance.InQuantity : 0), (currentBalance != null ? currentBalance.OutQuantity : 0), (currentBalance != null ? currentBalance.PendingOutQuantity : 0)),
					OutQuantity = CalculateOutQuantity(isOpenBalance, stockTaking.Quantity, (currentBalance != null ? currentBalance.OpenQuantity : 0), (currentBalance != null ? currentBalance.InQuantity : 0), (currentBalance != null ? currentBalance.OutQuantity : 0), (currentBalance != null ? currentBalance.PendingOutQuantity : 0)),
					DifferenceBetweenStockTakingConsumerAndCurrent = CalculateDifferenceBetweenStockTakingConsumerAndCurrent(isOpenBalance, stockTaking.ConsumerPrice, (currentBalance != null ? currentBalance.OpenQuantity : 0), (currentBalance != null ? currentBalance.InQuantity : 0), (currentBalance != null ? currentBalance.OutQuantity : 0), (currentBalance != null ? currentBalance.PendingOutQuantity : 0),stockTaking.ConsumerValue),
					DifferenceBetweenStockTakingCostAndCurrent = CalculateDifferenceBetweenStockTakingCostAndCurrent(isOpenBalance, stockTaking.ConsumerPrice, (currentBalance != null ? currentBalance.OpenQuantity : 0), (currentBalance != null ? currentBalance.InQuantity : 0), (currentBalance != null ? currentBalance.OutQuantity : 0), (currentBalance != null ? currentBalance.PendingOutQuantity : 0), stockTaking.CostValue),
				}).ToList();

			return data;
		}


		public async Task<ResponseDto> SaveStockTaking(StockTakingDto stockTaking, bool hasApprove, bool approved, int? requestId)
		{
			if (stockTaking.StockTakingHeader != null)
			{
				var result =  await _stockTakingHeaderService.SaveStockTakingHeader(stockTaking.StockTakingHeader, hasApprove, approved, requestId);
				if (result.Success)
				{
					await _stockTakingDetailService.SaveStockTakingDetails(result.Id, stockTaking.StockTakingDetails);

                    if (stockTaking.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(stockTaking.MenuNotes, result.Id);
                    }
                }

				return result;
			}
			return new ResponseDto{ Message = "Header should not be null" };
		}

		public async Task<ResponseDto> DeleteStockTaking(int stockTakingHeaderId)
        {
            var header = await _stockTakingHeaderService.GetStockTakingHeaderById(stockTakingHeaderId);
			if (header == null) return new ResponseDto { Id = stockTakingHeaderId, Message = _headerLocalizer["NoStockTakingFound"] };

			await _menuNoteService.DeleteMenuNotes(header.IsOpenBalance ? MenuCodeData.StockTakingOpenBalance : MenuCodeData.StockTakingCurrentBalance, header.StockTakingHeaderId);
            await _stockTakingDetailService.DeleteStockTakingDetails(stockTakingHeaderId);
			var result = await _stockTakingHeaderService.DeleteStockTakingHeader(stockTakingHeaderId);
			return result;
		}
	}
}
