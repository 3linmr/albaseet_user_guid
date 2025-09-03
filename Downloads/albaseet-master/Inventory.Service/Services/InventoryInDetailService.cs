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
	public class InventoryInDetailService : BaseService<InventoryInDetail>, IInventoryInDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public InventoryInDetailService(IRepository<InventoryInDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, IHttpContextAccessor httpContextAccessor): base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
			_httpContextAccessor = httpContextAccessor;
        }

		public async Task<List<InventoryInDetailDto>> GetInventoryInDetails(int inventoryInHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var modelList = new List<InventoryInDetailDto>();

            var data =
             await (from inventoryInDetail in _repository.GetAll().Where(x => x.InventoryInHeaderId == inventoryInHeaderId)
                    from item in _itemService.GetAll().Where(x => x.ItemId == inventoryInDetail.ItemId)
                    from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == inventoryInDetail.ItemPackageId)
                    select new InventoryInDetailDto
                    {
                        InventoryInDetailId = inventoryInDetail.InventoryInDetailId,
                        InventoryInHeaderId = inventoryInDetail.InventoryInHeaderId,
                        ItemPackageId = inventoryInDetail.ItemPackageId,
                        ItemId = inventoryInDetail.ItemId,
                        ItemCode = item.ItemCode,
                        ConsumerPrice = inventoryInDetail.ConsumerPrice,
                        Packing = inventoryInDetail.Packing,
                        BarCode = inventoryInDetail.BarCode,
                        CostPrice = inventoryInDetail.CostPrice,
                        BatchNumber = inventoryInDetail.BatchNumber,
                        ConsumerValue = inventoryInDetail.ConsumerValue,
                        CostValue = inventoryInDetail.CostValue,
                        ExpireDate = inventoryInDetail.ExpireDate,
                        Quantity = inventoryInDetail.Quantity,
                        CostPackage = inventoryInDetail.CostPackage,
                        ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                        ItemTypeId = item.ItemTypeId,
                        ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                        IsCostCenterDistributed = inventoryInDetail.IsCostCenterDistributed,
                        IsLinkedToCostCenters = inventoryInDetail.IsLinkedToCostCenters,
                        CreatedAt = inventoryInDetail.CreatedAt,
                        IpAddressCreated = inventoryInDetail.IpAddressCreated,
                        UserNameCreated = inventoryInDetail.UserNameCreated
                    }).ToListAsync();

			var packages = await _itemBarCodeService.GetItemsPackages(data.Select(x => x.ItemId).ToList());

            foreach (var inventoryInDetail in data)
			{
				var model = new InventoryInDetailDto();
				model = inventoryInDetail;
				model.Packages = JsonConvert.SerializeObject(packages.Where(x => x.ItemId == inventoryInDetail.ItemId).Select(s => s.Packages).FirstOrDefault());
				modelList.Add(model);
			}
			return modelList;
		}

        public async Task<List<InventoryInDetailDto>> SaveInventoryInDetails(int inventoryInHeaderId, List<InventoryInDetailDto> inventoryInDetails)
        {
            if (inventoryInDetails.Any())
            {
                await DeleteInventoryInDetail(inventoryInDetails, inventoryInHeaderId);
                await EditInventoryInDetail(inventoryInDetails, inventoryInHeaderId); 
                return await AddInventoryInDetail(inventoryInDetails, inventoryInHeaderId);
            }
            return inventoryInDetails;
        }

        public async Task<bool> DeleteInventoryInDetail(List<InventoryInDetailDto> inventoryInDetails, int inventoryInHeaderId)
        {
            if (inventoryInDetails.Any())
            {
                var current = _repository.GetAll().Where(x => x.InventoryInHeaderId == inventoryInHeaderId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => inventoryInDetails.All(p2 => p2.InventoryInDetailId != p.InventoryInDetailId)).ToList();

                if (toBeDeleted.Any())
                {   
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public async Task<List<InventoryInDetailDto>> AddInventoryInDetail(List<InventoryInDetailDto> inventoryInDetails, int inventoryInHeaderId)
        {
            var current = inventoryInDetails.Where(x => x.InventoryInDetailId <= 0).ToList();
            var modelList = new List<InventoryInDetail>();
            var newId = await GetNextId();

            foreach (var detail in current)
            {
                var model = new InventoryInDetail()
                {
                    InventoryInDetailId = newId,
                    InventoryInHeaderId = inventoryInHeaderId,
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

                detail.InventoryInDetailId = newId;
                detail.InventoryInHeaderId = inventoryInHeaderId;

                modelList.Add(model);
                newId++;
            }

            if (modelList.Any())
            {
                await _repository.InsertRange(modelList);
                await _repository.SaveChanges();
            }
            return inventoryInDetails;
        }

        public async Task<bool> EditInventoryInDetail(List<InventoryInDetailDto> inventoryInDetails, int inventoryInHeaderId)
        {
            var current = inventoryInDetails.Where(x => x.InventoryInDetailId > 0).ToList();
            var modelList = new List<InventoryInDetail>();

            foreach (var detail in current)
            {
                var model = new InventoryInDetail()
                {
                    InventoryInDetailId = detail.InventoryInDetailId,
                    InventoryInHeaderId = inventoryInHeaderId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.InventoryInDetailId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<bool> DeleteInventoryInDetails(int inventoryInHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.InventoryInHeaderId == inventoryInHeaderId).ToListAsync();
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
