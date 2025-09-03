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
	public class InternalTransferReceiveDetailService : BaseService<InternalTransferReceiveDetail>, IInternalTransferReceiveDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IHttpContextAccessor _httpContextAccessor;

		public InternalTransferReceiveDetailService(IRepository<InternalTransferReceiveDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, IHttpContextAccessor httpContextAccessor): base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
			_httpContextAccessor = httpContextAccessor;
        }

		public async Task<List<InternalTransferReceiveDetailDto>> GetInternalTransferReceiveDetails(int internalTransferReceiveHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var modelList = new List<InternalTransferReceiveDetailDto>();

			var data =
			 await (from internalTransferReceiveDetail in _repository.GetAll().Where(x => x.InternalTransferReceiveHeaderId == internalTransferReceiveHeaderId)
				   from item in _itemService.GetAll().Where(x => x.ItemId == internalTransferReceiveDetail.ItemId)
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == internalTransferReceiveDetail.ItemPackageId)
				   select new InternalTransferReceiveDetailDto
				   {
					   InternalTransferReceiveDetailId = internalTransferReceiveDetail.InternalTransferReceiveDetailId,
                       InternalTransferReceiveHeaderId = internalTransferReceiveDetail.InternalTransferReceiveHeaderId,
					   ItemId = internalTransferReceiveDetail.ItemId,
                       ItemCode = item.ItemCode,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                       ItemTypeId = item.ItemTypeId,
					   ItemPackageId = internalTransferReceiveDetail.ItemPackageId,
					   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					   BarCode = internalTransferReceiveDetail.BarCode,
					   Packing = internalTransferReceiveDetail.Packing,
					   Quantity = internalTransferReceiveDetail.Quantity,
					   ConsumerPrice = internalTransferReceiveDetail.ConsumerPrice,
					   ConsumerValue = internalTransferReceiveDetail.ConsumerValue,
					   CostPrice = internalTransferReceiveDetail.CostPrice,
					   CostPackage = internalTransferReceiveDetail.CostPackage,
					   CostValue = internalTransferReceiveDetail.CostValue,
					   ExpireDate = internalTransferReceiveDetail.ExpireDate,
					   BatchNumber = internalTransferReceiveDetail.BatchNumber,
                       CreatedAt = internalTransferReceiveDetail.CreatedAt,
					   IpAddressCreated = internalTransferReceiveDetail.IpAddressCreated,
					   UserNameCreated = internalTransferReceiveDetail.UserNameCreated
				   }).ToListAsync();

			var packages = await _itemBarCodeService.GetItemsPackages(data.Select(x => x.ItemId).ToList());

			foreach (var internalTransferReceiveDetail in data)
			{
				var model = new InternalTransferReceiveDetailDto();
				model = internalTransferReceiveDetail;
                model.Packages = JsonConvert.SerializeObject(packages.Where(x => x.ItemId == internalTransferReceiveDetail.ItemId).Select(s => s.Packages).FirstOrDefault());
				modelList.Add(model);
			}
			return modelList;
		}

        public async Task<bool> SaveInternalTransferReceiveDetails(int internalTransferReceiveHeaderId, List<InternalTransferReceiveDetailDto> internalTransferReceiveDetails)
        {
            if (internalTransferReceiveDetails.Any())
            {
                await DeleteInternalTransferReceiveDetail(internalTransferReceiveDetails, internalTransferReceiveHeaderId);
                await AddInternalTransferReceiveDetail(internalTransferReceiveDetails, internalTransferReceiveHeaderId);
                await EditInternalTransferReceiveDetail(internalTransferReceiveDetails);
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteInternalTransferReceiveDetails(int internalTransferReceiveHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.InternalTransferReceiveHeaderId == internalTransferReceiveHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteInternalTransferReceiveDetail(List<InternalTransferReceiveDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.InternalTransferReceiveHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.InternalTransferReceiveDetailId != p.InternalTransferReceiveDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> AddInternalTransferReceiveDetail(List<InternalTransferReceiveDetailDto> details, int headerId)
        {
            var current = details.Where(x => x.InternalTransferReceiveDetailId <= 0).ToList();
            var modelList = new List<InternalTransferReceiveDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new InternalTransferReceiveDetail()
                {
                    InternalTransferReceiveDetailId = newId,
                    InternalTransferReceiveHeaderId = headerId,
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

        private async Task<bool> EditInternalTransferReceiveDetail(List<InternalTransferReceiveDetailDto> internalTransferReceiveDetails)
        {
            var current = internalTransferReceiveDetails.Where(x => x.InternalTransferReceiveDetailId > 0).ToList();
            var modelList = new List<InternalTransferReceiveDetail>();
            foreach (var detail in current)
            {
                var model = new InternalTransferReceiveDetail()
                {
                    InternalTransferReceiveDetailId = detail.InternalTransferReceiveDetailId,
                    InternalTransferReceiveHeaderId = detail.InternalTransferReceiveHeaderId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.InternalTransferReceiveDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
