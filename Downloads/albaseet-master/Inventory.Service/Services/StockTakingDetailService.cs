using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.CoreOne.Contracts;
using Inventory.CoreOne.Models.Domain;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Service;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Inventory.Service.Services
{
	public class StockTakingDetailService : BaseService<StockTakingDetail>, IStockTakingDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public StockTakingDetailService(IRepository<StockTakingDetail> repository,IItemPackageService itemPackageService,IItemService itemService,IItemBarCodeService itemBarCodeService,IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<List<StockTakingDetailDto>> GetStockTakingDetails(int stockTakingHeaderId, int storeId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var modelList = new List<StockTakingDetailDto>();

			var data =
			 await	(from stockTakingDetail in _repository.GetAll().Where(x => x.StockTakingHeaderId == stockTakingHeaderId)
				from item in _itemService.GetAll().Where(x=>x.ItemId == stockTakingDetail.ItemId)
				from itemPackage in _itemPackageService.GetAll().Where(x=>x.ItemPackageId == stockTakingDetail.ItemPackageId)
				select new StockTakingDetailDto
				{
					StockTakingHeaderId = stockTakingDetail.StockTakingHeaderId,
					ItemPackageId = stockTakingDetail.ItemPackageId,
					StoreId = storeId,
					ItemId = stockTakingDetail.ItemId,
					ItemCode = item.ItemCode,
					ConsumerPrice = stockTakingDetail.ConsumerPrice,
					Packing = stockTakingDetail.Packing,
					BarCode = stockTakingDetail.BarCode,
					CostPrice = stockTakingDetail.CostPrice,
					BatchNumber = stockTakingDetail.BatchNumber,
					ConsumerValue = stockTakingDetail.ConsumerValue,
					CostValue = stockTakingDetail.CostValue,
					ExpireDate = stockTakingDetail.ExpireDate,
					Quantity = stockTakingDetail.Quantity,
					StockTakingDetailId = stockTakingDetail.StockTakingDetailId,
					CostPackage = stockTakingDetail.CostPackage,
					ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					ItemTypeId = item.ItemTypeId,
					ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					CreatedAt = stockTakingDetail.CreatedAt,
					IpAddressCreated = stockTakingDetail.IpAddressCreated,
					UserNameCreated = stockTakingDetail.UserNameCreated
				}).ToListAsync();

			var packages = await _itemBarCodeService.GetItemsPackages(data.Select(x => x.ItemId).ToList());

			foreach (var stock in data)
			{
				var model = new StockTakingDetailDto();
				model = stock;
				model.Packages = JsonConvert.SerializeObject(packages.Where(x => x.ItemId == stock.ItemId).Select(s => s.Packages).FirstOrDefault());
				modelList.Add(model);
			}
			return modelList;
		}

		public async Task<bool> SaveStockTakingDetails(int stockTakingHeaderId, List<StockTakingDetailDto> stockTakingDetails)
		{
			if (stockTakingDetails.Any())
			{
				await DeleteStockTakingDetail(stockTakingDetails, stockTakingHeaderId);
				await AddStockTakingDetail(stockTakingDetails, stockTakingHeaderId);
				await EditStockTakingDetail(stockTakingDetails);
				return true;
			}
			return false;
		}

		public async Task<bool> DeleteStockTakingDetail(List<StockTakingDetailDto> details, int headerId)
		{
			if (details.Any())
			{
				var current = _repository.GetAll().Where(x => x.StockTakingHeaderId == headerId).AsNoTracking().ToList();
				var toBeDeleted = current.Where(p => details.All(p2 => p2.StockTakingDetailId != p.StockTakingDetailId)).ToList();
				if (toBeDeleted.Any())
				{
					_repository.RemoveRange(toBeDeleted);
					await _repository.SaveChanges();
					return true;
				}
			}
			return false;
		}

		public async Task<bool> AddStockTakingDetail(List<StockTakingDetailDto> details, int headerId)
		{
			var current = details.Where(x => x.StockTakingDetailId <= 0).ToList();
			var modelList = new List<StockTakingDetail>();
			var newId = await GetNextId();
			foreach (var detail in current)
			{
				var newNote = new StockTakingDetail()
				{
					StockTakingDetailId = newId,
					StockTakingHeaderId = headerId,
					BarCode = detail.BarCode,
					BatchNumber = detail.BatchNumber,
					ConsumerPrice = detail.ConsumerPrice,
					ConsumerValue = detail.ConsumerValue,
					CostPackage = detail.CostPackage,
					CostPrice = detail.CostPrice,
					CostValue = detail.CostValue,
					ExpireDate = detail.ExpireDate,
					ItemId = detail.ItemId,
					ItemPackageId = detail.ItemPackageId,
					Packing = detail.Packing,
					Quantity = detail.Quantity,
					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor!.GetUserName(),
					IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				};
				modelList.Add(newNote);
				newId++;
			}

			if (modelList.Any())
			{
				await _repository.InsertRange(modelList);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}

		public async Task<bool> EditStockTakingDetail(List<StockTakingDetailDto> notes)
		{
			var current = notes.Where(x => x.StockTakingDetailId > 0).ToList();
			var modelList = new List<StockTakingDetail>();
			foreach (var detail in current)
			{
				var newNote = new StockTakingDetail()
				{
					StockTakingDetailId = detail.StockTakingDetailId,
					StockTakingHeaderId = detail.StockTakingHeaderId,
					BarCode = detail.BarCode,
					BatchNumber = detail.BatchNumber,
					ConsumerPrice = detail.ConsumerPrice,
					ConsumerValue = detail.ConsumerValue,
					CostPackage = detail.CostPackage,
					CostPrice = detail.CostPrice,
					CostValue = detail.CostValue,
					ExpireDate = detail.ExpireDate,
					ItemId = detail.ItemId,
					ItemPackageId = detail.ItemPackageId,
					Packing = detail.Packing,
					Quantity = detail.Quantity,
					CreatedAt = detail.CreatedAt,
					UserNameCreated = detail.UserNameCreated,
					IpAddressCreated = detail.IpAddressCreated,
					ModifiedAt = DateHelper.GetDateTimeNow(),
					UserNameModified = await _httpContextAccessor!.GetUserName(),
					IpAddressModified = _httpContextAccessor?.GetIpAddress(),
					Hide = false
				};
				modelList.Add(newNote);
			}

			if (modelList.Any())
			{
				_repository.UpdateRange(modelList);
				await _repository.SaveChanges();
				return true;
			}
			return false;

		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.StockTakingDetailId) + 1; } catch { id = 1; }
			return id;
		}
		public async Task<bool> DeleteStockTakingDetails(int stockTakingHeaderId)
		{
			var data = await _repository.GetAll().Where(x => x.StockTakingHeaderId == stockTakingHeaderId).ToListAsync();
			if (data.Any())
			{
				_repository.RemoveRange(data);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}
	}
}
