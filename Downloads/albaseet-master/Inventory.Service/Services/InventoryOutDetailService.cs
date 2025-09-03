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
	public class InventoryOutDetailService : BaseService<InventoryOutDetail>, IInventoryOutDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IHttpContextAccessor _httpContextAccessor;

		public InventoryOutDetailService(IRepository<InventoryOutDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, IHttpContextAccessor httpContextAccessor): base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
			_httpContextAccessor = httpContextAccessor;
        }

		public async Task<List<InventoryOutDetailDto>> GetInventoryOutDetails(int inventoryOutHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var modelList = new List<InventoryOutDetailDto>();

			var data =
			 await (from inventoryOutDetail in _repository.GetAll().Where(x => x.InventoryOutHeaderId == inventoryOutHeaderId)
				   from item in _itemService.GetAll().Where(x => x.ItemId == inventoryOutDetail.ItemId)
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == inventoryOutDetail.ItemPackageId)
				   select new InventoryOutDetailDto
				   {
					   InventoryOutDetailId = inventoryOutDetail.InventoryOutDetailId,
                       InventoryOutHeaderId = inventoryOutDetail.InventoryOutHeaderId,
					   ItemPackageId = inventoryOutDetail.ItemPackageId,
					   ItemId = inventoryOutDetail.ItemId,
                       ItemCode = item.ItemCode,
					   ConsumerPrice = inventoryOutDetail.ConsumerPrice,
					   Packing = inventoryOutDetail.Packing,
					   BarCode = inventoryOutDetail.BarCode,
					   CostPrice = inventoryOutDetail.CostPrice,
					   BatchNumber = inventoryOutDetail.BatchNumber,
					   ConsumerValue = inventoryOutDetail.ConsumerValue,
					   CostValue = inventoryOutDetail.CostValue,
					   ExpireDate = inventoryOutDetail.ExpireDate,
					   Quantity = inventoryOutDetail.Quantity,
					   CostPackage = inventoryOutDetail.CostPackage,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                       ItemTypeId = item.ItemTypeId,
					   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                       IsCostCenterDistributed = inventoryOutDetail.IsCostCenterDistributed,
                       IsLinkedToCostCenters = inventoryOutDetail.IsLinkedToCostCenters,
                       CreatedAt = inventoryOutDetail.CreatedAt,
					   IpAddressCreated = inventoryOutDetail.IpAddressCreated,
					   UserNameCreated = inventoryOutDetail.UserNameCreated
				   }).ToListAsync();

			var packages = await _itemBarCodeService.GetItemsPackages(data.Select(x => x.ItemId).ToList());

            foreach (var inventoryOutDetail in data)
			{
				var model = new InventoryOutDetailDto();
				model = inventoryOutDetail;
                model.Packages = JsonConvert.SerializeObject(packages.Where(x => x.ItemId == inventoryOutDetail.ItemId).Select(s => s.Packages).FirstOrDefault());
				modelList.Add(model);
			}
			return modelList;
		}

        public async Task<List<InventoryOutDetailDto>> SaveInventoryOutDetails(int inventoryOutHeaderId, List<InventoryOutDetailDto> inventoryOutDetails)
        {
            if (inventoryOutDetails.Any())
            {
                await DeleteInventoryOutDetail(inventoryOutDetails, inventoryOutHeaderId);
                await EditInventoryOutDetail(inventoryOutDetails, inventoryOutHeaderId);
                return await AddInventoryOutDetail(inventoryOutDetails, inventoryOutHeaderId);
            }
            return inventoryOutDetails;
        }

        public async Task<bool> DeleteInventoryOutDetail(List<InventoryOutDetailDto> inventoryOutDetails, int inventoryOutHeaderId)
        {
            if (inventoryOutDetails.Any())
            {
                var current = _repository.GetAll().Where(x => x.InventoryOutHeaderId == inventoryOutHeaderId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => inventoryOutDetails.All(p2 => p2.InventoryOutDetailId != p.InventoryOutDetailId)).ToList();

                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public async Task<List<InventoryOutDetailDto>> AddInventoryOutDetail(List<InventoryOutDetailDto> inventoryOutDetails, int inventoryOutHeaderId)
        {
            var current = inventoryOutDetails.Where(x => x.InventoryOutDetailId <= 0).ToList();
            var modelList = new List<InventoryOutDetail>();
            var newId = await GetNextId();

            foreach (var detail in current)
            {
                var model = new InventoryOutDetail()
                {
                    InventoryOutDetailId = newId,
                    InventoryOutHeaderId = inventoryOutHeaderId,
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
                    IsCostCenterDistributed = detail.IsCostCenterDistributed,
                    IsLinkedToCostCenters = detail.IsLinkedToCostCenters,

                    Hide = false,
                    CreatedAt = DateHelper.GetDateTimeNow(),
                    UserNameCreated = await _httpContextAccessor!.GetUserName(),
                    IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                };

                detail.InventoryOutDetailId = newId;
                detail.InventoryOutHeaderId = inventoryOutHeaderId;

                modelList.Add(model);
                newId++;
            }

            if (modelList.Any())
            {
                await _repository.InsertRange(modelList);
                await _repository.SaveChanges();
            }
            return inventoryOutDetails;
        }

        public async Task<bool> EditInventoryOutDetail(List<InventoryOutDetailDto> inventoryOutDetails, int inventoryOutHeaderId)
        {
            var current = inventoryOutDetails.Where(x => x.InventoryOutDetailId > 0).ToList();
            var modelList = new List<InventoryOutDetail>();

            foreach (var detail in current)
            {
                var model = new InventoryOutDetail()
                {
                    InventoryOutDetailId = detail.InventoryOutDetailId,
                    InventoryOutHeaderId = inventoryOutHeaderId,
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
                    IsCostCenterDistributed = detail.IsCostCenterDistributed,
                    IsLinkedToCostCenters = detail.IsLinkedToCostCenters,

                    CreatedAt = detail.CreatedAt,
                    UserNameCreated = detail.UserNameCreated,
                    IpAddressCreated = detail.IpAddressCreated,
                    ModifiedAt = DateHelper.GetDateTimeNow(),
                    UserNameModified = await _httpContextAccessor!.GetUserName(),
                    IpAddressModified = _httpContextAccessor?.GetIpAddress(),
                    Hide = false
                };

                modelList.Add(model);
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
            try { id = await _repository.GetAll().MaxAsync(a => a.InventoryOutDetailId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<bool> DeleteInventoryOutDetails(int inventoryOutHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.InventoryOutHeaderId == inventoryOutHeaderId).ToListAsync();
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
