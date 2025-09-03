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
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Items;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Service;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Inventory.Service.Services
{
	public class StockTakingCarryOverDetailService : BaseService<StockTakingCarryOverDetail>, IStockTakingCarryOverDetailService
	{
		private readonly IItemService _itemService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public StockTakingCarryOverDetailService(IRepository<StockTakingCarryOverDetail> repository, IItemService itemService, IItemPackageService itemPackageService, IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_itemService = itemService;
			_itemPackageService = itemPackageService;
			_httpContextAccessor = httpContextAccessor;
		}

		public IQueryable<StockTakingCarryOverDetailDto> GetCarryOversDetailsById(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var data =
				from stockTakingCarryOverDetail in _repository.GetAll().Where(x => x.StockTakingCarryOverHeaderId == id)
				from item in _itemService.GetAll().Where(x => x.ItemId == stockTakingCarryOverDetail.ItemId)
				from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == stockTakingCarryOverDetail.ItemPackageId)
				select new StockTakingCarryOverDetailDto
				{
					ItemId = stockTakingCarryOverDetail.ItemId,
					StockTakingCarryOverHeaderId = stockTakingCarryOverDetail.StockTakingCarryOverHeaderId,
					StockTakingCarryOverDetailId = stockTakingCarryOverDetail.StockTakingCarryOverDetailId,
					ItemPackageId = stockTakingCarryOverDetail.ItemPackageId,
					BatchNumber = stockTakingCarryOverDetail.BatchNumber,
					ExpireDate = stockTakingCarryOverDetail.ExpireDate,
					OpenQuantity = stockTakingCarryOverDetail.OpenQuantity,
					InQuantity = stockTakingCarryOverDetail.InQuantity,
					OutQuantity = stockTakingCarryOverDetail.OutQuantity,
					ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					ItemTypeId = item.ItemTypeId,
					BarCode = stockTakingCarryOverDetail.BarCode,
					Packing = stockTakingCarryOverDetail.Packing,
					ItemCode = item.ItemCode,
					OldOpenQuantity = stockTakingCarryOverDetail.OldOpenQuantity,
					StockTakingCostValue = stockTakingCarryOverDetail.StockTakingCostValue,
					StockTakingQuantity = stockTakingCarryOverDetail.StockTakingQuantity,
					StockTakingConsumerValue = stockTakingCarryOverDetail.StockTakingConsumerValue,
					CurrentConsumerPrice = stockTakingCarryOverDetail.CurrentConsumerPrice,
					StockTakingConsumerPrice = stockTakingCarryOverDetail.StockTakingConsumerPrice,
					CurrentConsumerValue = stockTakingCarryOverDetail.CurrentConsumerValue,
					CurrentCostPackage = stockTakingCarryOverDetail.CurrentCostPackage,
					CurrentCostPrice = stockTakingCarryOverDetail.CurrentCostPrice,
					CurrentCostValue = stockTakingCarryOverDetail.CurrentCostValue,
					CurrentQuantity = stockTakingCarryOverDetail.CurrentQuantity,
					StockTakingCostPackage = stockTakingCarryOverDetail.StockTakingCostPackage,
					StockTakingCostPrice = stockTakingCarryOverDetail.StockTakingCostPrice
				};
			return data;
		}

		public async Task<bool> SaveCarryOverDetails(int headerId,List<StockTakingCarryOverDetailDto> details)
		{
			var modelList = new List<StockTakingCarryOverDetail>();
			var nextId = await GetNextId();
			var userName = await _httpContextAccessor.GetUserName();
			if (details.Any())
			{
				foreach (var detail in details)
				{
					var model = new StockTakingCarryOverDetail()
					{
						StockTakingCarryOverHeaderId = headerId,
						StockTakingCarryOverDetailId = nextId,
						ItemId = detail.ItemId,
						BatchNumber = detail.BatchNumber,
						ExpireDate = detail.ExpireDate,
						OpenQuantity = detail.OpenQuantity,
						InQuantity = detail.InQuantity,
						OutQuantity = detail.OutQuantity,
						ItemPackageId = detail.ItemPackageId,
						StockTakingCostValue = detail.StockTakingCostValue,
						StockTakingQuantity = detail.StockTakingQuantity,
						StockTakingConsumerValue = detail.StockTakingConsumerValue,
						CurrentConsumerPrice = detail.CurrentConsumerPrice,
						StockTakingConsumerPrice = detail.StockTakingConsumerPrice,
						CurrentConsumerValue = detail.CurrentConsumerValue,
						CurrentCostPackage = detail.CurrentCostPackage,
						CurrentCostPrice = detail.CurrentCostPrice,
						CurrentCostValue = detail.CurrentCostValue,
						CurrentQuantity = detail.CurrentQuantity,
						StockTakingCostPackage = detail.StockTakingCostPackage,
						StockTakingCostPrice = detail.StockTakingCostPrice,
						BarCode = detail.BarCode,
						Packing = detail.Packing,
						OldOpenQuantity = detail.OldOpenQuantity,
						CreatedAt = DateHelper.GetDateTimeNow(),
						UserNameCreated = userName,
						IpAddressCreated = _httpContextAccessor.GetIpAddress(),
						Hide = false
					};
					modelList.Add(model);
					nextId++;
				}

				await _repository.InsertRange(modelList);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.StockTakingCarryOverDetailId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<bool> DeleteCarryOverDetails(int headerId)
		{
			var data = await _repository.GetAll().Where(x => x.StockTakingCarryOverHeaderId == headerId).AsNoTracking().ToListAsync();
			_repository.RemoveRange(data);
			await _repository.SaveChanges();
			return true;
		}
	}
}
