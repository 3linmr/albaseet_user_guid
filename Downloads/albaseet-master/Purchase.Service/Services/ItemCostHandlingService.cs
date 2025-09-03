using Purchases.CoreOne.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.CoreOne.Models.StaticData;
using Shared.Service.Logic.Calculation;
using Shared.CoreOne.Models.Domain.Items;
using Shared.Helper.Identity;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Logic;

namespace Purchases.Service.Services
{
	public class ItemCostHandlingService : IItemCostHandlingService
	{
		private readonly IApplicationSettingService _applicationSettingService;
		private readonly IItemCostService _itemCostService;
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
		private readonly IPurchaseInvoiceService _purchaseInvoiceService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ItemCostHandlingService(IApplicationSettingService applicationSettingService,IItemCostService itemCostService,IItemCurrentBalanceService itemCurrentBalanceService,IPurchaseInvoiceService purchaseInvoiceService,IHttpContextAccessor httpContextAccessor)
		{
			_applicationSettingService = applicationSettingService;
			_itemCostService = itemCostService;
			_itemCurrentBalanceService = itemCurrentBalanceService;
			_purchaseInvoiceService = purchaseInvoiceService;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<List<ItemCostDto>> CalculateItemCost(CalculateItemCost model)
        {
			var itemCosts = new List<ItemCostDto>();
           var items =  GetItemCostGrouping(model.Items);

            foreach (var item in items)
			{
				var currentBalance = await _itemCurrentBalanceService.GetItemCurrentBalanceInfoByItemId(model.StoreId,item.ItemId,true,true);
				var currentBalanceQuantity = currentBalance.AvailableBalance - item.ItemInvoiceQuantity;
				if (currentBalanceQuantity > 0)
				{
					var itemStockPrice = await GetItemStockPrice(model.CurrentPurchaseInvoiceHeaderId,item.ItemId, currentBalanceQuantity,item.ItemInvoicePrice);
					var actualAverage = ItemCostCalculation.GetActualAverageCost(currentBalanceQuantity, item.ItemInvoiceQuantity, itemStockPrice, item.ItemInvoicePrice, item.ItemExpenseValue);

					var itemCost = new ItemCostDto()
					{
						ItemId = item.ItemId,
						CostPrice = actualAverage,
						StoreId = model.StoreId,
						ItemPackageId = currentBalance.ItemPackageId, //SingularPackage
						LastPurchasePrice = GetLastPurchasePrice(items, item.ItemId),
						LastCostPrice = GetLastPurchasePrice(items, item.ItemId) + item.ItemExpenseValue
					};
					itemCosts.Add(itemCost);
				}
			}
			return itemCosts;
		}

        public static List<ItemCostVm> GetItemCostGrouping(List<ItemCostVm> items)
        {
            var data =
                (from item in items
                group new { item } by new {item.ItemId}
                into g
                select new ItemCostVm()
                {
					ItemId = g.Key.ItemId,
					ItemExpenseValue = g.Sum(x=>x.item.ItemExpenseValue),
					ItemInvoiceQuantity = g.Sum(x=>x.item.ItemInvoiceQuantity * x.item.ItemPacking),
					ItemInvoicePrice = g.Sum(x => x.item.ItemInvoiceQuantity * x.item.ItemPacking) != 0 ?
						((g.Sum(x => x.item.ItemInvoiceQuantity *(x.item.ItemInvoicePrice/ x.item.ItemPacking) * x.item.ItemPacking)) / g.Sum(x => x.item.ItemInvoiceQuantity * x.item.ItemPacking)) :0
                }).ToList();
            return data;
        }
        public static decimal GetLastPurchasePrice(List<ItemCostVm> items,int itemId)
        {
            return (items.Where(x => x.ItemId == itemId).Select(x => x.ItemInvoicePrice).LastOrDefault());
        }
		public async Task<decimal> GetItemStockPrice(int currentPurchaseInvoiceHeaderId,int itemId,decimal itemStockQuantity,decimal currentInvoicePrice)
		{
			var invoiceQuantities = await _purchaseInvoiceService.GetLatestInvoicesBasedOnQuantity(currentPurchaseInvoiceHeaderId,itemId, itemStockQuantity);
			return ItemCostCalculation.GetAverageItemCost(invoiceQuantities,currentInvoicePrice);
		}

		public async Task<bool> UpdateItemCosts(CalculateItemCost model)
		{
			var newCosts = await CalculateItemCost(model);

			var itemsId = model.Items.Select(x => x.ItemId).ToList();
			var currentCosts = await _itemCostService.GetAll().Where(x => itemsId.Contains(x.ItemId) && x.StoreId == model.StoreId).AsNoTracking().ToListAsync();
			var userName = await _httpContextAccessor.GetUserName();

			var updatedData =
				(from itemCost in currentCosts
				from newCost in newCosts.Where(x=>x.ItemId == itemCost.ItemId && x.ItemPackageId == itemCost.ItemPackageId && x.StoreId == itemCost.StoreId)
				select new ItemCost()
				{
					ItemCostId = itemCost.ItemCostId,
					StoreId = itemCost.StoreId,
					ItemPackageId = itemCost.ItemPackageId,
					ItemId = itemCost.ItemId,
					CostPrice = newCost.CostPrice,
					LastCostPrice = newCost.LastCostPrice,
					LastPurchasePrice = newCost.LastPurchasePrice,
					CreatedAt = itemCost.CreatedAt,
					ModifiedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = itemCost.UserNameCreated,
					UserNameModified = userName,
					IpAddressCreated = itemCost.IpAddressCreated,
					IpAddressModified = _httpContextAccessor?.GetIpAddress(),
					Hide = false
				}).ToList();


			var insertedCosts =
				(from newCost in newCosts
				from currentCost in currentCosts.Where(x => x.ItemId == newCost.ItemId && x.ItemPackageId == newCost.ItemPackageId && x.StoreId == newCost.StoreId).DefaultIfEmpty()
				where currentCost == null
				select new ItemCost()
				{
					ItemCostId = 0,
					StoreId = newCost.StoreId,
					ItemPackageId = newCost.ItemPackageId,
					ItemId = newCost.ItemId,
					CostPrice = newCost.CostPrice,
					LastCostPrice = newCost.LastCostPrice,
					LastPurchasePrice = newCost.LastPurchasePrice,
					CreatedAt = DateHelper.GetDateTimeNow(),
					ModifiedAt = null,
					UserNameCreated = userName,
					UserNameModified = null,
					IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
					IpAddressModified =null,
					Hide = false
				}).ToList();


			if (updatedData.Any())
			{
				await _itemCostService.UpdateRange(updatedData);
			}

			if (newCosts.Any())
			{
				var nextId = await _itemCostService.GetNextId();
				insertedCosts.ForEach(x => x.ItemCostId = nextId++);
				await _itemCostService.InsertRange(insertedCosts);
			}

			return true;
		}
	}
}
