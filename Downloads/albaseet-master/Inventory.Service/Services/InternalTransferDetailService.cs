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
	public class InternalTransferDetailService : BaseService<InternalTransferDetail>, IInternalTransferDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IHttpContextAccessor _httpContextAccessor;

		public InternalTransferDetailService(IRepository<InternalTransferDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, IHttpContextAccessor httpContextAccessor): base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
			_httpContextAccessor = httpContextAccessor;
        }

		public async Task<List<InternalTransferDetailDto>> GetInternalTransferDetails(int internalTransferHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var modelList = new List<InternalTransferDetailDto>();

			var data =
			 await (from internalTransferDetail in _repository.GetAll().Where(x => x.InternalTransferHeaderId == internalTransferHeaderId)
				   from item in _itemService.GetAll().Where(x => x.ItemId == internalTransferDetail.ItemId)
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == internalTransferDetail.ItemPackageId)
				   select new InternalTransferDetailDto
				   {
					   InternalTransferDetailId = internalTransferDetail.InternalTransferDetailId,
                       InternalTransferHeaderId = internalTransferDetail.InternalTransferHeaderId,
					   ItemId = internalTransferDetail.ItemId,
                       ItemCode = item.ItemCode,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                       ItemTypeId = item.ItemTypeId,
					   ItemPackageId = internalTransferDetail.ItemPackageId,
					   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					   BarCode = internalTransferDetail.BarCode,
					   Packing = internalTransferDetail.Packing,
					   Quantity = internalTransferDetail.Quantity,
					   ConsumerPrice = internalTransferDetail.ConsumerPrice,
					   ConsumerValue = internalTransferDetail.ConsumerValue,
					   CostPrice = internalTransferDetail.CostPrice,
					   CostPackage = internalTransferDetail.CostPackage,
					   CostValue = internalTransferDetail.CostValue,
					   ExpireDate = internalTransferDetail.ExpireDate,
					   BatchNumber = internalTransferDetail.BatchNumber,
                       CreatedAt = internalTransferDetail.CreatedAt,
					   IpAddressCreated = internalTransferDetail.IpAddressCreated,
					   UserNameCreated = internalTransferDetail.UserNameCreated
				   }).ToListAsync();

			var packages = await _itemBarCodeService.GetItemsPackages(data.Select(x => x.ItemId).ToList());

			foreach (var internalTransferDetail in data)
			{
				var model = new InternalTransferDetailDto();
				model = internalTransferDetail;
                model.Packages = JsonConvert.SerializeObject(packages.Where(x => x.ItemId == internalTransferDetail.ItemId).Select(s => s.Packages).FirstOrDefault());
				modelList.Add(model);
			}
			return modelList;
		}

        public async Task<bool> SaveInternalTransferDetails(int internalTransferHeaderId, List<InternalTransferDetailDto> internalTransferDetails)
        {
            if (internalTransferDetails.Any())
            {
                await DeleteInternalTransferDetail(internalTransferDetails, internalTransferHeaderId);
                await AddInternalTransferDetail(internalTransferDetails, internalTransferHeaderId);
                await EditInternalTransferDetail(internalTransferDetails);
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteInternalTransferDetails(int internalTransferHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.InternalTransferHeaderId == internalTransferHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteInternalTransferDetail(List<InternalTransferDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.InternalTransferHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.InternalTransferDetailId != p.InternalTransferDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> AddInternalTransferDetail(List<InternalTransferDetailDto> details, int headerId)
        {
            var current = details.Where(x => x.InternalTransferDetailId <= 0).ToList();
            var modelList = new List<InternalTransferDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new InternalTransferDetail()
                {
                    InternalTransferDetailId = newId,
                    InternalTransferHeaderId = headerId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    Quantity = detail.Quantity,
                    ConsumerPrice = detail.ConsumerPrice,
                    ConsumerValue = detail.ConsumerValue,
                    CostPrice = detail.CostPrice,
                    CostPackage = detail.CostPackage,
                    CostValue = detail.CostValue,
                    ExpireDate = detail.ExpireDate,
                    BatchNumber = detail.BatchNumber,

                    Hide = false,
                    CreatedAt = DateHelper.GetDateTimeNow(),
                    UserNameCreated = await _httpContextAccessor!.GetUserName(),
                    IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                };

                modelList.Add(model);
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

        private async Task<bool> EditInternalTransferDetail(List<InternalTransferDetailDto> internalTransferDetails)
        {
            var current = internalTransferDetails.Where(x => x.InternalTransferDetailId > 0).ToList();
            var modelList = new List<InternalTransferDetail>();
            foreach (var detail in current)
            {
                var model = new InternalTransferDetail()
                {
                    InternalTransferDetailId = detail.InternalTransferDetailId,
                    InternalTransferHeaderId = detail.InternalTransferHeaderId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    Quantity = detail.Quantity,
                    ConsumerPrice = detail.ConsumerPrice,
                    ConsumerValue = detail.ConsumerValue,
                    CostPrice = detail.CostPrice,
                    CostPackage = detail.CostPackage,
                    CostValue = detail.CostValue,
                    ExpireDate = detail.ExpireDate,
                    BatchNumber = detail.BatchNumber,

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

        private async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.InternalTransferDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
