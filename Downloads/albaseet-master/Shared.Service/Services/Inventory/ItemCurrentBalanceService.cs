using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MoreLinq.Experimental;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Inventory;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Tree;
using Shared.Service.Services.Basics;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Inventory
{
	public class ItemCurrentBalanceService : BaseService<ItemCurrentBalance>, IItemCurrentBalanceService
	{
		private readonly IItemService _itemService;
		private readonly IStoreService _storeService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemBarCodeService _itemBarCodeService;
		private readonly IItemBarCodeDetailService _itemBarCodeDetailService;
		private readonly IItemPackingService _itemPackingService;
		private readonly IItemDisassembleService _itemDisassembleService;
		private readonly IStringLocalizer<ItemCurrentBalanceService> _localizer;


		public ItemCurrentBalanceService(IRepository<ItemCurrentBalance> repository, IItemService itemService, IStoreService storeService, IItemPackageService itemPackageService, IHttpContextAccessor httpContextAccessor, IItemBarCodeService itemBarCodeService, IItemBarCodeDetailService itemBarCodeDetailService, IItemPackingService itemPackingService, IItemDisassembleService itemDisassembleService, IStringLocalizer<ItemCurrentBalanceService> localizer) : base(repository)
		{
			_itemService = itemService;
			_storeService = storeService;
			_itemPackageService = itemPackageService;
			_httpContextAccessor = httpContextAccessor;
			_itemBarCodeService = itemBarCodeService;
			_itemBarCodeDetailService = itemBarCodeDetailService;
			_itemPackingService = itemPackingService;
			_itemDisassembleService = itemDisassembleService;
			_localizer = localizer;
		}

		public IQueryable<ItemCurrentBalanceDto> GetItemCurrentBalances()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				from itemCurrentBalance in _repository.GetAll()
				from item in _itemService.GetAll().Where(x => x.ItemId == itemCurrentBalance.ItemId)
				from store in _storeService.GetAll().Where(x => x.StoreId == itemCurrentBalance.StoreId)
				from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemCurrentBalance.ItemPackageId)
				from itemBarCode in _itemBarCodeService.GetAll().Where(x => x.ItemId == itemCurrentBalance.ItemId && x.FromPackageId == itemPackage.ItemPackageId).DefaultIfEmpty()
				from itemBarCodeDetail in _itemBarCodeDetailService.GetAll().Where(x => x.ItemBarCodeDetailId == itemBarCode.ItemBarCodeId).DefaultIfEmpty()
				group new { itemCurrentBalance, itemBarCodeDetail } by new { itemCurrentBalance.StoreId, store.StoreNameAr, store.StoreNameEn, itemCurrentBalance.ItemId, item.ItemCode, item.ItemNameAr, item.ItemNameEn, item.ItemTypeId, itemPackage.PackageNameAr, itemPackage.PackageNameEn, itemCurrentBalance.ItemPackageId, itemCurrentBalance.BatchNumber, itemCurrentBalance.ExpireDate, itemCurrentBalance.OpenDate } into g
				select new ItemCurrentBalanceDto
				{
					StoreId = g.Key.StoreId,
					StoreName = language == LanguageCode.Arabic ? g.Key.StoreNameAr : g.Key.StoreNameEn,
					ItemId = g.Key.ItemId,
					ItemCode = g.Key.ItemCode,
					ItemName = language == LanguageCode.Arabic ? g.Key.ItemNameAr : g.Key.ItemNameEn,
					ItemTypeId = g.Key.ItemTypeId,
					ItemPackageId = g.Key.ItemPackageId,
					ItemPackageName = language == LanguageCode.Arabic ? g.Key.PackageNameAr : g.Key.PackageNameEn,
					BatchNumber = String.IsNullOrWhiteSpace(g.Key.BatchNumber) ? null : g.Key.BatchNumber.Trim(),
					ExpireDate = g.Key.ExpireDate,
					OpenDate = g.Key.OpenDate,
					OpenQuantity = g.Sum(x => x.itemCurrentBalance.OpenQuantity),
					InQuantity = g.Sum(x => x.itemCurrentBalance.InQuantity),
					OutQuantity = g.Sum(x => x.itemCurrentBalance.OutQuantity),
					PendingInQuantity = g.Sum(x => x.itemCurrentBalance.PendingInQuantity),
					PendingOutQuantity = g.Sum(x => x.itemCurrentBalance.PendingOutQuantity),
					CurrentBalance = (g.Sum(x => x.itemCurrentBalance.OpenQuantity)) + (g.Sum(x => x.itemCurrentBalance.InQuantity)) - (g.Sum(x => x.itemCurrentBalance.OutQuantity)),
					AvailableBalance = (g.Sum(x => x.itemCurrentBalance.OpenQuantity)) + (g.Sum(x => x.itemCurrentBalance.InQuantity)) - (g.Sum(x => x.itemCurrentBalance.OutQuantity)) - (g.Sum(x => x.itemCurrentBalance.PendingOutQuantity)),
					ConsumerPrice = g.Select(x => x.itemBarCodeDetail.ConsumerPrice).FirstOrDefault(),
					BarCode = g.Select(x => x.itemBarCodeDetail.BarCode).FirstOrDefault(),
				};
			return data;
		}
		public IQueryable<ItemCurrentBalanceDto> GetStoreItemCurrentBalancesWithBarCodes(int storeId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				from itemCurrentBalance in _repository.GetAll().Where(x => x.StoreId == storeId)
				from item in _itemService.GetAll().Where(x => x.ItemId == itemCurrentBalance.ItemId)
				from store in _storeService.GetAll().Where(x => x.StoreId == itemCurrentBalance.StoreId)
				from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemCurrentBalance.ItemPackageId)
				from itemBarCode in _itemBarCodeService.GetAll().Where(x => x.FromPackageId == itemCurrentBalance.ItemPackageId && x.ItemId == itemCurrentBalance.ItemId).DefaultIfEmpty()
				from barCodeDetail in _itemBarCodeDetailService.GetAll().Where(x => x.ItemBarCodeId == itemBarCode.ItemBarCodeId).DefaultIfEmpty()
				select new ItemCurrentBalanceDto
				{
					ItemCurrentBalanceId = itemCurrentBalance.ItemCurrentBalanceId,
					ItemId = itemCurrentBalance.ItemId,
					ItemCode = item.ItemCode,
					BatchNumber = String.IsNullOrWhiteSpace(itemCurrentBalance.BatchNumber) ? null : itemCurrentBalance.BatchNumber.Trim(),
					ExpireDate = itemCurrentBalance.ExpireDate,
					StoreId = itemCurrentBalance.StoreId,
					ItemPackageId = itemCurrentBalance.ItemPackageId,
					OpenDate = itemCurrentBalance.OpenDate,
					OpenQuantity = itemCurrentBalance.OpenQuantity,
					InQuantity = itemCurrentBalance.InQuantity,
					OutQuantity = itemCurrentBalance.OutQuantity,
					PendingInQuantity = itemCurrentBalance.PendingInQuantity,
					PendingOutQuantity = itemCurrentBalance.PendingOutQuantity,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					CurrentBalance = itemCurrentBalance.OpenQuantity + itemCurrentBalance.InQuantity - itemCurrentBalance.OutQuantity,
					AvailableBalance = itemCurrentBalance.OpenQuantity + itemCurrentBalance.InQuantity - itemCurrentBalance.OutQuantity - itemCurrentBalance.PendingOutQuantity,
					BarCode = barCodeDetail != null ? barCodeDetail.BarCode : null,
					ConsumerPrice = barCodeDetail != null ? barCodeDetail.ConsumerPrice : item.ConsumerPrice
				};
			return data;
		}

		public IQueryable<ItemCurrentBalanceDto> GetStoreItemCurrentBalances(int storeId)
		{
			return GetItemCurrentBalances().Where(x => x.StoreId == storeId);
		}

		public async Task<List<ItemCurrentBalanceDto>> GetItemCurrentBalanceByItemId(int storeId, int itemId)
		{
			return await GetItemCurrentBalances().Where(x => x.StoreId == storeId && x.ItemId == itemId).ToListAsync();
		}

		public async Task<ItemCurrentBalanceDto> GetItemCurrentBalanceInfo(int storeId, int itemId, int itemPackageId, DateTime? expireDate, string? batchNumber)
		{
			return await GetItemCurrentBalances().FirstOrDefaultAsync(x => x.StoreId == storeId && x.ItemId == itemId && x.ItemPackageId == itemPackageId && x.ExpireDate == expireDate && x.BatchNumber == batchNumber) ?? new ItemCurrentBalanceDto();
		}

		public async Task<List<ItemCurrentBalanceDto>> GetItemCurrentBalanceByItemCode(int storeId, string itemCode)
		{
			return await GetItemCurrentBalances().Where(x => x.StoreId == storeId && x.ItemCode.Trim() == itemCode.Trim()).ToListAsync();
		}

		public async Task<List<ItemCurrentBalanceDto>> GetItemCurrentBalanceByBarCode(int storeId, string barCode)
		{
			var itemId = await _itemService.GetItemItemIdByBarCode(barCode);
			return await GetItemCurrentBalances().Where(x => x.StoreId == storeId && x.ItemId == itemId).ToListAsync();
		}

		public IQueryable<ItemCurrentBalanceDto> GetItemCurrentBalanceInfo(int storeId, bool isSinglePackage, bool isGrouped)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from itemCurrentBalance in GetStoreItemCurrentBalances(storeId)
				from item in _itemService.GetAll().Where(x => x.ItemId == itemCurrentBalance.ItemId)
				from itemPacking in _itemPackingService.GetAll().Where(x => x.ItemId == itemCurrentBalance.ItemId && x.FromPackageId == itemCurrentBalance.ItemPackageId && x.ToPackageId == item.SingularPackageId)
				from singularPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == item.SingularPackageId)
				select new ItemCurrentBalanceDto
				{
					ItemCurrentBalanceId = itemCurrentBalance.ItemCurrentBalanceId,
					ItemId = itemCurrentBalance.ItemId,
					ItemCode = item.ItemCode,
					ItemTypeId = item.ItemTypeId,
					BatchNumber = itemCurrentBalance.BatchNumber,
					ExpireDate = itemCurrentBalance.ExpireDate,
					StoreId = itemCurrentBalance.StoreId,
					ItemPackageId = isSinglePackage ? singularPackage.ItemPackageId : itemCurrentBalance.ItemPackageId,
					ItemPackageName = isSinglePackage ? (language == LanguageCode.Arabic ? singularPackage.PackageNameAr : singularPackage.PackageNameEn) : itemCurrentBalance.ItemPackageName,
					//BarCode = itemCurrentBalance.BarCode,
					//ConsumerPrice = itemCurrentBalance.ConsumerPrice,
					StoreName = itemCurrentBalance.StoreName,
					ItemName = itemCurrentBalance.ItemName,
					OpenDate = itemCurrentBalance.OpenDate,
					OpenQuantity = isSinglePackage ? itemCurrentBalance.OpenQuantity * itemPacking.Packing : itemCurrentBalance.OpenQuantity,
					InQuantity = isSinglePackage ? itemCurrentBalance.InQuantity * itemPacking.Packing : itemCurrentBalance.InQuantity,
					OutQuantity = isSinglePackage ? itemCurrentBalance.OutQuantity * itemPacking.Packing : itemCurrentBalance.OutQuantity,
					PendingInQuantity = isSinglePackage ? itemCurrentBalance.PendingInQuantity * itemPacking.Packing : itemCurrentBalance.PendingInQuantity,
					PendingOutQuantity = isSinglePackage ? itemCurrentBalance.PendingOutQuantity * itemPacking.Packing : itemCurrentBalance.PendingOutQuantity,
					CurrentBalance = isSinglePackage ? itemCurrentBalance.CurrentBalance * itemPacking.Packing : itemCurrentBalance.CurrentBalance,
					AvailableBalance = isSinglePackage ? itemCurrentBalance.AvailableBalance * itemPacking.Packing : itemCurrentBalance.AvailableBalance
				};

			if (isGrouped)
			{
				var grouped = data.GroupBy(x => new { x.StoreId, x.ItemId, x.ItemPackageId, x.StoreName, x.ItemName, x.ItemCode, x.ItemTypeId, x.ItemPackageName }).Select(x => new ItemCurrentBalanceDto
				{
					StoreId = x.Key.StoreId,
					StoreName = x.Key.StoreName,
					ItemId = x.Key.ItemId,
					ItemCode = x.Key.ItemCode,
					ItemName = x.Key.ItemName,
					ItemTypeId = x.Key.ItemTypeId,
					ItemPackageId = x.Key.ItemPackageId,
					ItemPackageName = x.Key.ItemPackageName,
					OpenQuantity = x.Sum(y => y.OpenQuantity),
					InQuantity = x.Sum(y => y.InQuantity),
					OutQuantity = x.Sum(y => y.OutQuantity),
					PendingInQuantity = x.Sum(y => y.PendingInQuantity),
					PendingOutQuantity = x.Sum(y => y.PendingOutQuantity),
					CurrentBalance = x.Sum(y => y.CurrentBalance),
					AvailableBalance = x.Sum(y => y.AvailableBalance),
				});
				return grouped;
			}
			return data;
		}

		public async Task<ItemCurrentBalanceDto> GetItemCurrentBalanceInfoByItemId(int storeId, int itemId, bool isSinglePackage, bool isGrouped)
		{
			return await GetItemCurrentBalanceInfo(storeId, isSinglePackage, isGrouped).FirstOrDefaultAsync(x => x.ItemId == itemId) ?? new ItemCurrentBalanceDto();
		}

		public async Task<ItemCurrentBalanceDto> GetItemCurrentBalanceInfoByItemAndPackageId(int storeId, int itemId, int itemPackageId, bool isGrouped)
		{
			return await GetItemCurrentBalanceInfo(storeId, false, isGrouped).FirstOrDefaultAsync(x => x.ItemId == itemId && x.ItemPackageId == itemPackageId) ?? new ItemCurrentBalanceDto();
		}

		public async Task<ItemCurrentBalanceDto?> GetItemCurrentBalance(int balanceId)
		{
			return await GetItemCurrentBalances().FirstOrDefaultAsync(x => x.ItemCurrentBalanceId == balanceId);
		}

		public async Task<bool> InventoryIn(int storeId, List<ItemCurrentBalanceDto> balances)
		{
			var balanceId = balances.Select(x => x.ItemId).ToList();
			var currentBalances = await _repository.GetAll().Where(x => balanceId.Contains(x.ItemId) && x.StoreId == storeId).AsNoTracking().ToListAsync();

			var userName = await _httpContextAccessor.GetUserName();

			var newBalances = balances.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.StoreId, x.ExpireDate, x.BatchNumber }).Select(s => new ItemCurrentBalance()
			{
				ItemId = s.Key.ItemId,
				ItemPackageId = s.Key.ItemPackageId,
				StoreId = s.Key.StoreId,
				ExpireDate = s.Key.ExpireDate,
				BatchNumber = String.IsNullOrWhiteSpace(s.Key.BatchNumber) ? null : s.Key.BatchNumber.Trim(),
				InQuantity = s.Sum(x => x.InQuantity),
				OutQuantity = s.Sum(x => x.OutQuantity),
				OpenQuantity = s.Sum(x => x.OpenQuantity),
				PendingInQuantity = s.Sum(x => x.PendingInQuantity),
				PendingOutQuantity = s.Sum(x => x.PendingOutQuantity)
			}).ToList();


			var data =
				(from newBalance in newBalances
				 from balanceDb in currentBalances.Where(x => x.ItemId == newBalance.ItemId && x.ExpireDate == newBalance.ExpireDate && x.BatchNumber == newBalance.BatchNumber && x.StoreId == newBalance.StoreId && x.ItemPackageId == newBalance.ItemPackageId).DefaultIfEmpty()
				 select new ItemCurrentBalance()
				 {
					 ItemCurrentBalanceId = balanceDb != null ? balanceDb.ItemCurrentBalanceId : 0,
					 InQuantity = balanceDb != null ? (balanceDb.InQuantity + newBalance.InQuantity) : newBalance.InQuantity,
					 OutQuantity = balanceDb != null ? (balanceDb.OutQuantity) : 0,
					 OpenQuantity = balanceDb != null ? (balanceDb.OpenQuantity) : 0,
					 OpenDate = balanceDb != null ? balanceDb.OpenDate : null,
					 PendingInQuantity = balanceDb != null ? (balanceDb.PendingInQuantity + newBalance.PendingInQuantity) : newBalance.PendingInQuantity,
					 PendingOutQuantity = balanceDb != null ? (balanceDb.PendingOutQuantity) : 0,
					 ItemId = newBalance.ItemId,
					 ExpireDate = newBalance.ExpireDate,
					 BatchNumber = newBalance.BatchNumber,
					 ItemPackageId = newBalance.ItemPackageId,
					 StoreId = newBalance.StoreId,
					 CreatedAt = balanceDb != null ? balanceDb.CreatedAt : DateHelper.GetDateTimeNow(),
					 Hide = false,
					 UserNameCreated = balanceDb != null ? balanceDb.UserNameCreated : userName,
					 IpAddressCreated = balanceDb != null ? balanceDb.UserNameCreated : _httpContextAccessor?.GetIpAddress(),
					 ModifiedAt = DateHelper.GetDateTimeNow(),
					 UserNameModified = userName,
					 IpAddressModified = _httpContextAccessor?.GetIpAddress()
				 }).ToList();

			var updateData = data.Where(x => x.ItemCurrentBalanceId > 0).ToList();
			var newData = data.Where(x => x.ItemCurrentBalanceId == 0).ToList();

			if (updateData.Any())
			{
				_repository.UpdateRange(updateData);
				await _repository.SaveChanges();
			}
			if (newData.Any())
			{
				var nextId = await GetNextId();
				newData.ForEach(x => x.ItemCurrentBalanceId = nextId++);

				await _repository.InsertRange(newData);
				await _repository.SaveChanges();
			}
			return true;
		}

		public async Task<bool> InventoryOut(int storeId, List<ItemCurrentBalanceDto> balances)
		{
			var balanceId = balances.Select(x => x.ItemId).ToList();
			var currentBalances = await _repository.GetAll().Where(x => balanceId.Contains(x.ItemId) && x.StoreId == storeId).AsNoTracking().ToListAsync();

			var userName = await _httpContextAccessor.GetUserName();

			var newBalances = balances.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.StoreId, x.ExpireDate, x.BatchNumber }).Select(s => new ItemCurrentBalance()
			{
				ItemId = s.Key.ItemId,
				ItemPackageId = s.Key.ItemPackageId,
				StoreId = s.Key.StoreId,
				ExpireDate = s.Key.ExpireDate,
				BatchNumber = String.IsNullOrWhiteSpace(s.Key.BatchNumber) ? null : s.Key.BatchNumber.Trim(),
				InQuantity = s.Sum(x => x.InQuantity),
				OutQuantity = s.Sum(x => x.OutQuantity),
				OpenQuantity = s.Sum(x => x.OpenQuantity),
				PendingInQuantity = s.Sum(x => x.PendingInQuantity),
				PendingOutQuantity = s.Sum(x => x.PendingOutQuantity)
			}).ToList();

			var data =
				(from newBalance in newBalances
				 from balanceDb in currentBalances.Where(x => x.ItemId == newBalance.ItemId && x.ExpireDate == newBalance.ExpireDate && x.BatchNumber == newBalance.BatchNumber && x.StoreId == newBalance.StoreId && x.ItemPackageId == newBalance.ItemPackageId).DefaultIfEmpty()
				 select new ItemCurrentBalance()
				 {
					 ItemCurrentBalanceId = balanceDb != null ? balanceDb.ItemCurrentBalanceId : 0,
					 OutQuantity = balanceDb != null ? (balanceDb.OutQuantity + newBalance.OutQuantity) : newBalance.OutQuantity,
					 InQuantity = balanceDb != null ? (balanceDb.InQuantity) : 0,
					 OpenQuantity = balanceDb != null ? (balanceDb.OpenQuantity) : 0,
					 OpenDate = balanceDb != null ? balanceDb.OpenDate : null,
					 PendingInQuantity = balanceDb != null ? (balanceDb.PendingInQuantity) : 0,
					 PendingOutQuantity = balanceDb != null ? (balanceDb.PendingOutQuantity + newBalance.PendingOutQuantity) : newBalance.PendingOutQuantity,
					 ItemId = newBalance.ItemId,
					 ExpireDate = newBalance.ExpireDate,
					 BatchNumber = newBalance.BatchNumber,
					 ItemPackageId = newBalance.ItemPackageId,
					 StoreId = newBalance.StoreId,
					 CreatedAt = balanceDb != null ? balanceDb.CreatedAt : DateHelper.GetDateTimeNow(),
					 Hide = false,
					 UserNameCreated = balanceDb != null ? balanceDb.UserNameCreated : userName,
					 IpAddressCreated = balanceDb != null ? balanceDb.UserNameCreated : _httpContextAccessor?.GetIpAddress(),
					 ModifiedAt = DateHelper.GetDateTimeNow(),
					 UserNameModified = userName,
					 IpAddressModified = _httpContextAccessor?.GetIpAddress()
				 }).ToList();

			var updateData = data.Where(x => x.ItemCurrentBalanceId > 0).ToList();
			var newData = data.Where(x => x.ItemCurrentBalanceId == 0).ToList();


			if (updateData.Any())
			{
				_repository.UpdateRange(updateData);
				await _repository.SaveChanges();
			}
			if (newData.Any())
			{
				var nextId = await GetNextId();
				newData.ForEach(x => x.ItemCurrentBalanceId = nextId++);

				await _repository.InsertRange(newData);
				await _repository.SaveChanges();
			}
			return true;
		}

		public async Task<bool> InventoryInOut(int storeId, List<ItemCurrentBalanceDto> oldBalances, List<ItemCurrentBalanceDto> newBalances)
		{
			var balanceId = oldBalances.Select(x => x.ItemId).ToList().Union(newBalances.Select(x => x.ItemId).ToList());
			var currentBalances = await _repository.GetAll().Where(x => balanceId.Contains(x.ItemId) && x.StoreId == storeId).AsNoTracking().ToListAsync();

			var userName = await _httpContextAccessor.GetUserName();

			var oldBalancesGrouped = oldBalances.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.StoreId, x.ExpireDate, x.BatchNumber }).Select(s => new ItemCurrentBalance()
			{
				ItemId = s.Key.ItemId,
				ItemPackageId = s.Key.ItemPackageId,
				StoreId = s.Key.StoreId,
				ExpireDate = s.Key.ExpireDate,
				BatchNumber = String.IsNullOrWhiteSpace(s.Key.BatchNumber) ? null : s.Key.BatchNumber.Trim(),
				InQuantity = s.Sum(x => x.InQuantity),
				OutQuantity = s.Sum(x => x.OutQuantity),
				OpenQuantity = s.Sum(x => x.OpenQuantity),
				PendingInQuantity = s.Sum(x => x.PendingInQuantity),
				PendingOutQuantity = s.Sum(x => x.PendingOutQuantity),
			}).ToList();

			var newBalancesGrouped = newBalances.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.StoreId, x.ExpireDate, x.BatchNumber }).Select(s => new ItemCurrentBalance()
			{
				ItemId = s.Key.ItemId,
				ItemPackageId = s.Key.ItemPackageId,
				StoreId = s.Key.StoreId,
				ExpireDate = s.Key.ExpireDate,
				BatchNumber = String.IsNullOrWhiteSpace(s.Key.BatchNumber) ? null : s.Key.BatchNumber.Trim(),
				InQuantity = s.Sum(x => x.InQuantity),
				OutQuantity = s.Sum(x => x.OutQuantity),
				OpenQuantity = s.Sum(x => x.OpenQuantity),
				PendingInQuantity = s.Sum(x => x.PendingInQuantity),
				PendingOutQuantity = s.Sum(x => x.PendingOutQuantity),
			}).ToList();

			var updatedBalances =
				(
					from currentBalance in currentBalances
					from newBalance in newBalancesGrouped.Where(x => x.ItemId == currentBalance.ItemId && x.ExpireDate == currentBalance.ExpireDate && x.BatchNumber == currentBalance.BatchNumber && x.StoreId == currentBalance.StoreId && x.ItemPackageId == currentBalance.ItemPackageId)
					from oldBalance in oldBalancesGrouped.Where(x => x.ItemId == currentBalance.ItemId && x.ExpireDate == currentBalance.ExpireDate && x.BatchNumber == currentBalance.BatchNumber && x.StoreId == currentBalance.StoreId && x.ItemPackageId == currentBalance.ItemPackageId)
					select new ItemCurrentBalance()
					{
						ItemCurrentBalanceId = currentBalance.ItemCurrentBalanceId,
						InQuantity = ((currentBalance.InQuantity - oldBalance.InQuantity) + newBalance.InQuantity),
						OutQuantity = ((currentBalance.OutQuantity - oldBalance.OutQuantity) + newBalance.OutQuantity),
						OpenQuantity = ((currentBalance.OpenQuantity - oldBalance.OpenQuantity) + newBalance.OpenQuantity),
						PendingInQuantity = ((currentBalance.PendingInQuantity - oldBalance.PendingInQuantity) + newBalance.PendingInQuantity),
						PendingOutQuantity = ((currentBalance.PendingOutQuantity - oldBalance.PendingOutQuantity) + newBalance.PendingOutQuantity),
						OpenDate = currentBalance.OpenDate,
						ItemId = currentBalance.ItemId,
						ExpireDate = currentBalance.ExpireDate,
						BatchNumber = currentBalance.BatchNumber,
						ItemPackageId = currentBalance.ItemPackageId,
						StoreId = currentBalance.StoreId,
						CreatedAt = currentBalance.CreatedAt,
						Hide = false,
						UserNameCreated = currentBalance.UserNameCreated,
						IpAddressCreated = currentBalance.UserNameCreated,
						ModifiedAt = DateHelper.GetDateTimeNow(),
						UserNameModified = userName,
						IpAddressModified = _httpContextAccessor?.GetIpAddress()
					}).ToList();


			var decreasedBalances =
				(from oldBalance in oldBalancesGrouped
				 from currentBalance in currentBalances.Where(x => x.ItemId == oldBalance.ItemId && x.ExpireDate == oldBalance.ExpireDate && x.BatchNumber == oldBalance.BatchNumber && x.StoreId == oldBalance.StoreId && x.ItemPackageId == oldBalance.ItemPackageId).DefaultIfEmpty()
				 from newBalance in newBalancesGrouped.Where(x => x.ItemId == oldBalance.ItemId && x.ExpireDate == oldBalance.ExpireDate && x.BatchNumber == oldBalance.BatchNumber && x.StoreId == oldBalance.StoreId && x.ItemPackageId == oldBalance.ItemPackageId).DefaultIfEmpty()
				 where newBalance == null
				 select new ItemCurrentBalance()
				 {
					 ItemCurrentBalanceId = currentBalance.ItemCurrentBalanceId,
					 InQuantity = (currentBalance.InQuantity - oldBalance.InQuantity),
					 OutQuantity = (currentBalance.OutQuantity - oldBalance.OutQuantity),
					 OpenQuantity = (currentBalance.OpenQuantity - oldBalance.OpenQuantity),
					 PendingInQuantity = (currentBalance.PendingInQuantity - oldBalance.PendingInQuantity),
					 PendingOutQuantity = (currentBalance.PendingOutQuantity - oldBalance.PendingOutQuantity),
					 OpenDate = currentBalance.OpenDate,
					 ItemId = oldBalance.ItemId,
					 ExpireDate = oldBalance.ExpireDate,
					 BatchNumber = oldBalance.BatchNumber,
					 ItemPackageId = oldBalance.ItemPackageId,
					 StoreId = oldBalance.StoreId,
					 CreatedAt = currentBalance.CreatedAt,
					 Hide = false,
					 UserNameCreated = currentBalance.UserNameCreated,
					 IpAddressCreated = currentBalance.UserNameCreated,
					 ModifiedAt = DateHelper.GetDateTimeNow(),
					 UserNameModified = userName,
					 IpAddressModified = _httpContextAccessor?.GetIpAddress()
				 }).ToList();


			var insertedBalances =
				(from newBalance in newBalancesGrouped
				 from currentBalance in currentBalances.Where(x => x.ItemId == newBalance.ItemId && x.ExpireDate == newBalance.ExpireDate && x.BatchNumber == newBalance.BatchNumber && x.StoreId == newBalance.StoreId && x.ItemPackageId == newBalance.ItemPackageId).DefaultIfEmpty()
				 from oldBalance in oldBalancesGrouped.Where(x => x.ItemId == newBalance.ItemId && x.ExpireDate == newBalance.ExpireDate && x.BatchNumber == newBalance.BatchNumber && x.StoreId == newBalance.StoreId && x.ItemPackageId == newBalance.ItemPackageId).DefaultIfEmpty()
				 where oldBalance == null
				 select new ItemCurrentBalance()
				 {
					 ItemCurrentBalanceId = currentBalance != null ? currentBalance.ItemCurrentBalanceId : 0,
					 OpenQuantity = currentBalance != null ? (currentBalance.OpenQuantity + newBalance.OpenQuantity) : newBalance.OpenQuantity,
					 InQuantity = currentBalance != null ? (currentBalance.InQuantity + newBalance.InQuantity) : newBalance.InQuantity,
					 OutQuantity = currentBalance != null ? (currentBalance.OutQuantity + newBalance.OutQuantity) : newBalance.OutQuantity,
					 PendingInQuantity = currentBalance != null ? (currentBalance.PendingInQuantity + newBalance.PendingInQuantity) : newBalance.PendingInQuantity,
					 PendingOutQuantity = currentBalance != null ? (currentBalance.PendingOutQuantity + newBalance.PendingOutQuantity) : newBalance.PendingOutQuantity,
					 OpenDate = currentBalance == null && newBalance.OpenQuantity > 0 ? DateHelper.GetDateTimeNow() : null,
					 ItemId = newBalance.ItemId,
					 ExpireDate = newBalance.ExpireDate,
					 BatchNumber = newBalance.BatchNumber,
					 ItemPackageId = newBalance.ItemPackageId,
					 StoreId = newBalance.StoreId,
					 CreatedAt = currentBalance != null ? currentBalance.CreatedAt : DateHelper.GetDateTimeNow(),
					 Hide = false,
					 UserNameCreated = currentBalance != null ? currentBalance.UserNameCreated : userName,
					 IpAddressCreated = currentBalance != null ? currentBalance.UserNameCreated : _httpContextAccessor?.GetIpAddress(),
					 ModifiedAt = DateHelper.GetDateTimeNow(),
					 UserNameModified = userName,
					 IpAddressModified = _httpContextAccessor?.GetIpAddress()
				 }).ToList();


			var newToBeUpdated = insertedBalances.Where(x => x.ItemCurrentBalanceId > 0).ToList();
			var toBeUpdated = updatedBalances.Union(decreasedBalances).Union(newToBeUpdated).ToList();
			var newToBeInserted = insertedBalances.Where(x => x.ItemCurrentBalanceId == 0).ToList();

			if (toBeUpdated.Any())
			{
				_repository.UpdateRange(toBeUpdated);
				await _repository.SaveChanges();
			}
			if (newToBeInserted.Any())
			{
				var nextId = await GetNextId();
				newToBeInserted.ForEach(x => x.ItemCurrentBalanceId = nextId++);

				await _repository.InsertRange(newToBeInserted);
				await _repository.SaveChanges();
			}
			return true;
		}

		public async Task<List<int>> ReOrderInventory(int storeId, DateTime openBalanceCarryOverDate, List<ItemCurrentBalanceDto> balances)
		{
			var balanceId = balances.Select(x => x.ItemId).ToList();
			var currentBalances = await _repository.GetAll().Where(x => balanceId.Contains(x.ItemId) && x.StoreId == storeId).AsNoTracking().ToListAsync();

			var userName = await _httpContextAccessor.GetUserName();

			var newBalances = balances.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.StoreId, x.ExpireDate, x.BatchNumber }).Select(s => new ItemCurrentBalance()
			{
				ItemId = s.Key.ItemId,
				ItemPackageId = s.Key.ItemPackageId,
				StoreId = s.Key.StoreId,
				ExpireDate = s.Key.ExpireDate,
				BatchNumber = String.IsNullOrWhiteSpace(s.Key.BatchNumber) ? null : s.Key.BatchNumber.Trim(),
				InQuantity = s.Sum(x => x.InQuantity),
				OutQuantity = s.Sum(x => x.OutQuantity),
				OpenQuantity = s.Sum(x => x.OpenQuantity),
				PendingInQuantity = s.Sum(x => x.PendingInQuantity),
				PendingOutQuantity = s.Sum(x => x.PendingOutQuantity),
			}).ToList();

			var data =
				(from newBalance in newBalances
				 from balanceDb in currentBalances.Where(x => x.ItemId == newBalance.ItemId && x.ExpireDate == newBalance.ExpireDate && x.BatchNumber == newBalance.BatchNumber && x.StoreId == newBalance.StoreId && x.ItemPackageId == newBalance.ItemPackageId).DefaultIfEmpty()
				 select new ItemCurrentBalance()
				 {
					 ItemCurrentBalanceId = balanceDb != null ? balanceDb.ItemCurrentBalanceId : 0,
					 InQuantity = balanceDb != null ? (balanceDb.InQuantity + newBalance.InQuantity) : newBalance.InQuantity,
					 OutQuantity = balanceDb != null ? (balanceDb.OutQuantity + newBalance.OutQuantity) : newBalance.OutQuantity,
					 OpenQuantity = balanceDb != null ? (balanceDb.OpenQuantity + newBalance.OpenQuantity) : newBalance.OpenQuantity,
					 OpenDate = newBalance != null ? (newBalance.OpenQuantity != 0 ? openBalanceCarryOverDate : balanceDb != null && newBalance.OpenQuantity == 0 ? balanceDb.OpenDate : null) :null,
					 PendingInQuantity = balanceDb != null ? (balanceDb.PendingInQuantity + newBalance.PendingInQuantity) : newBalance.PendingInQuantity,
					 PendingOutQuantity = balanceDb != null ? (balanceDb.PendingOutQuantity + newBalance.PendingOutQuantity) : newBalance.PendingOutQuantity,
					 ItemId = newBalance.ItemId,
					 ExpireDate = newBalance.ExpireDate,
					 BatchNumber = newBalance.BatchNumber,
					 ItemPackageId = newBalance.ItemPackageId,
					 StoreId = newBalance.StoreId,
					 CreatedAt = balanceDb != null ? balanceDb.CreatedAt : DateHelper.GetDateTimeNow(),
					 Hide = false,
					 UserNameCreated = balanceDb != null ? balanceDb.UserNameCreated : userName,
					 IpAddressCreated = balanceDb != null ? balanceDb.UserNameCreated : _httpContextAccessor?.GetIpAddress(),
					 ModifiedAt = DateHelper.GetDateTimeNow(),
					 UserNameModified = userName,
					 IpAddressModified = _httpContextAccessor?.GetIpAddress()
				 }).ToList();

			var updateData = data.Where(x => x.ItemCurrentBalanceId > 0).ToList();
			var newData = data.Where(x => x.ItemCurrentBalanceId == 0).ToList();

			if (updateData.Any())
			{
				_repository.UpdateRange(updateData);
				await _repository.SaveChanges();
			}
			if (newData.Any())
			{
				var nextId = await GetNextId();
				newData.ForEach(x => x.ItemCurrentBalanceId = nextId++);

				await _repository.InsertRange(newData);
				await _repository.SaveChanges();
			}
			return updateData.Select(x => x.ItemCurrentBalanceId).Union(newData.Select(x => x.ItemCurrentBalanceId)).ToList();
		}

		public async Task<bool> ReOrderInventory(int storeId, List<ItemCurrentBalanceDto> balances)
		{
			var balanceId = balances.Select(x => x.ItemId).ToList();
			var currentBalances = await _repository.GetAll().Where(x => balanceId.Contains(x.ItemId) && x.StoreId == storeId).AsNoTracking().ToListAsync();

			var userName = await _httpContextAccessor.GetUserName();

			var newBalances = balances.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.StoreId, x.ExpireDate, x.BatchNumber }).Select(s => new ItemCurrentBalance()
			{
				ItemId = s.Key.ItemId,
				ItemPackageId = s.Key.ItemPackageId,
				StoreId = s.Key.StoreId,
				ExpireDate = s.Key.ExpireDate,
				BatchNumber = String.IsNullOrWhiteSpace(s.Key.BatchNumber) ? null : s.Key.BatchNumber.Trim(),
				InQuantity = s.Sum(x => x.InQuantity),
				OutQuantity = s.Sum(x => x.OutQuantity),
				OpenQuantity = s.Sum(x => x.OpenQuantity),
				PendingInQuantity = s.Sum(x => x.PendingInQuantity),
				PendingOutQuantity = s.Sum(x => x.PendingOutQuantity),
			}).ToList();

			var data =
				(from newBalance in newBalances
				 from balanceDb in currentBalances.Where(x => x.ItemId == newBalance.ItemId && x.ExpireDate == newBalance.ExpireDate && x.BatchNumber == newBalance.BatchNumber && x.StoreId == newBalance.StoreId && x.ItemPackageId == newBalance.ItemPackageId).DefaultIfEmpty()
				 select new ItemCurrentBalance()
				 {
					 ItemCurrentBalanceId = balanceDb != null ? balanceDb.ItemCurrentBalanceId : 0,
					 InQuantity = balanceDb != null ? (balanceDb.InQuantity + newBalance.InQuantity) : newBalance.InQuantity,
					 OutQuantity = balanceDb != null ? (balanceDb.OutQuantity + newBalance.OutQuantity) : newBalance.OutQuantity,
					 OpenQuantity = balanceDb != null ? (balanceDb.OpenQuantity + newBalance.OpenQuantity) : newBalance.OpenQuantity,
					 OpenDate = balanceDb != null ? balanceDb.OpenDate : null,
					 PendingInQuantity = balanceDb != null ? (balanceDb.PendingInQuantity + newBalance.PendingInQuantity) : newBalance.PendingInQuantity,
					 PendingOutQuantity = balanceDb != null ? (balanceDb.PendingOutQuantity + newBalance.PendingOutQuantity) : newBalance.PendingOutQuantity,
					 ItemId = newBalance.ItemId,
					 ExpireDate = newBalance.ExpireDate,
					 BatchNumber = newBalance.BatchNumber,
					 ItemPackageId = newBalance.ItemPackageId,
					 StoreId = newBalance.StoreId,
					 CreatedAt = balanceDb != null ? balanceDb.CreatedAt : DateHelper.GetDateTimeNow(),
					 Hide = false,
					 UserNameCreated = balanceDb != null ? balanceDb.UserNameCreated : userName,
					 IpAddressCreated = balanceDb != null ? balanceDb.UserNameCreated : _httpContextAccessor?.GetIpAddress(),
					 ModifiedAt = DateHelper.GetDateTimeNow(),
					 UserNameModified = userName,
					 IpAddressModified = _httpContextAccessor?.GetIpAddress()
				 }).ToList();

			var updateData = data.Where(x => x.ItemCurrentBalanceId > 0).ToList();
			var newData = data.Where(x => x.ItemCurrentBalanceId == 0).ToList();

			if (updateData.Any())
			{
				_repository.UpdateRange(updateData);
				await _repository.SaveChanges();
			}
			if (newData.Any())
			{
				var nextId = await GetNextId();
				newData.ForEach(x => x.ItemCurrentBalanceId = nextId++);

				await _repository.InsertRange(newData);
				await _repository.SaveChanges();
			}

			return true;
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemCurrentBalanceId) + 1; } catch { id = 1; }
			return id;
		}
	}
}
