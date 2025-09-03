using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Inventory
{
	public class ItemCostService : BaseService<ItemCost>, IItemCostService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemService _itemService;
		private readonly IStoreService _storeService;
		private readonly IItemPackageService _itemPackageService;

		public ItemCostService(IRepository<ItemCost> repository, IHttpContextAccessor httpContextAccessor, IItemService itemService, IStoreService storeService, IItemPackageService itemPackageService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_itemService = itemService;
			_storeService = storeService;
			_itemPackageService = itemPackageService;
		}

		public IQueryable<ItemCostDto> GetItemCosts()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				from itemCost in _repository.GetAll()
				from item in _itemService.GetAll().Where(x => x.ItemId == itemCost.ItemId)
				from store in _storeService.GetAll().Where(x => x.StoreId == itemCost.StoreId)
				from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemCost.ItemPackageId)
				select new ItemCostDto
				{
					ItemCostId = itemCost.ItemCostId,
					ItemId = itemCost.ItemId,
					ItemCode = item.ItemCode,
					StoreId = itemCost.StoreId,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					CostPrice = itemCost.CostPrice,
					ItemPackageId = itemCost.ItemPackageId,
					ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					LastCostPrice = itemCost.LastCostPrice,
					LastPurchasePrice = itemCost.LastPurchasePrice,
					LastUpdateDate = itemCost.ModifiedAt ?? itemCost.CreatedAt,
					LastUserNameUpdate = itemCost.UserNameModified ?? itemCost.UserNameCreated
				};
			return data;
		}

		public IQueryable<ItemCostDto> GetItemCostsByStoreId()
		{
			var storeId = _httpContextAccessor.GetCurrentUserStore();
			return GetItemCosts().Where(x => x.StoreId == storeId);
		}

		public IQueryable<ItemCostDto> GetStoreItemCost(int storeId)
		{
			return GetItemCosts().Where(x => x.StoreId == storeId);
		}

		public async Task<ItemCostDto?> GetItemCostById(int costId)
		{
			return await GetItemCosts().FirstOrDefaultAsync(x => x.ItemCostId == costId);
		}

		public async Task<ItemCostDto?> GetItemCostByItemId(int itemId, int storeId)
		{
			return await GetItemCosts().FirstOrDefaultAsync(x => x.ItemId == itemId && x.StoreId == storeId);
		}

		public async Task<decimal> GetItemCostPriceByItemId(int itemId, int storeId)
		{
			var itemCost = await GetItemCosts().FirstOrDefaultAsync(x => x.ItemId == itemId && x.StoreId == storeId);
			return itemCost?.CostPrice ?? 0;
		}

		public async Task<bool> SaveCosts(int storeId, List<ItemCostDto> costs)
		{
			var balanceId = costs.Select(x => x.ItemId).ToList();
			var currentCosts = await _repository.GetAll().Where(x => balanceId.Contains(x.ItemId) && x.StoreId == storeId).AsNoTracking().ToListAsync();

			var userName = await _httpContextAccessor.GetUserName();

			var data =
				(from newCost in costs
				 from costDb in currentCosts.Where(x => x.ItemId == newCost.ItemId && x.StoreId == newCost.StoreId).DefaultIfEmpty()
				 select new ItemCost()
				 {
					 ItemCostId = costDb != null ? costDb.ItemCostId : 0,
					 CostPrice = newCost.CostPrice,
					 ItemId = newCost.ItemId,
					 StoreId = newCost.StoreId,
					 CreatedAt = costDb != null ? costDb.CreatedAt : DateHelper.GetDateTimeNow(),
					 Hide = false,
					 UserNameCreated = costDb != null ? costDb.UserNameCreated : userName,
					 IpAddressCreated = costDb != null ? costDb.UserNameCreated : _httpContextAccessor?.GetIpAddress(),
					 ModifiedAt = DateHelper.GetDateTimeNow(),
					 UserNameModified = userName,
					 IpAddressModified = _httpContextAccessor?.GetIpAddress(),
					 ItemPackageId = costDb != null ? costDb.ItemPackageId : newCost.ItemPackageId
				 }).ToList();


			var dataToBeSaved = data
				.GroupBy(i => i.ItemId)
				.Select(g => g.FirstOrDefault())
				.ToList();

			var updateData = dataToBeSaved.Where(x => x.ItemCostId > 0).ToList();
			var newData = dataToBeSaved.Where(x => x.ItemCostId == 0).ToList();

			if (updateData.Any())
			{
				_repository.UpdateRange(updateData);
				await _repository.SaveChanges();
			}
			if (newData.Any())
			{
				var nextId = await GetNextId();
				newData.ForEach(x => x.ItemCostId = nextId++);

				await _repository.InsertRange(newData);
				await _repository.SaveChanges();
			}
			return true;
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemCostId) + 1; } catch { id = 1; }
			return id;
		}
	}
}
