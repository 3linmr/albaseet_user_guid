using Purchases.CoreOne.Contracts;
using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.Helper.Identity;
using Shared.Service;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.Helper.Logic;
using Shared.CoreOne.Models.StaticData;

namespace Purchases.Service.Services
{
	public class ProductRequestDetailService : BaseService<ProductRequestDetail>, IProductRequestDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;

		public ProductRequestDetailService(IRepository<ProductRequestDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService,IHttpContextAccessor httpContextAccessor): base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
			_httpContextAccessor = httpContextAccessor;
        }

		public async Task<List<ProductRequestDetailDto>> GetProductRequestDetails(int productRequestHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var modelList = new List<ProductRequestDetailDto>();

			var data =
			 await (from productRequestDetail in _repository.GetAll().Where(x => x.ProductRequestHeaderId == productRequestHeaderId)
				   from item in _itemService.GetAll().Where(x => x.ItemId == productRequestDetail.ItemId)
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == productRequestDetail.ItemPackageId)
				   from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == productRequestDetail.CostCenterId).DefaultIfEmpty()
                   select new ProductRequestDetailDto
				   {
					   ProductRequestDetailId = productRequestDetail.ProductRequestDetailId,
                       ProductRequestHeaderId = productRequestDetail.ProductRequestHeaderId,
					   CostCenterId = productRequestDetail.CostCenterId,
                       CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                       ItemId = productRequestDetail.ItemId,
                       ItemCode = item.ItemCode,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					   ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? productRequestDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
                       TaxTypeId = item.TaxTypeId,
                       ItemTypeId = item.ItemTypeId,
					   ItemPackageId = productRequestDetail.ItemPackageId,
					   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                       IsItemVatInclusive = productRequestDetail.IsItemVatInclusive,
					   BarCode = productRequestDetail.BarCode,
					   Packing = productRequestDetail.Packing,
					   Quantity = productRequestDetail.Quantity,
					   ConsumerPrice = productRequestDetail.ConsumerPrice,
					   ConsumerValue = productRequestDetail.ConsumerValue,
					   CostPrice = productRequestDetail.CostPrice,
					   CostPackage = productRequestDetail.CostPackage,
					   CostValue = productRequestDetail.CostValue,
                       Notes = productRequestDetail.Notes,
                       ItemNote = productRequestDetail.ItemNote,
                       CreatedAt = productRequestDetail.CreatedAt,
					   IpAddressCreated = productRequestDetail.IpAddressCreated,
					   UserNameCreated = productRequestDetail.UserNameCreated
				   }).ToListAsync();

			var packages = await _itemBarCodeService.GetItemsPackages(data.Select(x => x.ItemId).ToList());

			foreach (var productRequestDetail in data)
			{
				var model = new ProductRequestDetailDto();
				model = productRequestDetail;
                model.Packages = JsonConvert.SerializeObject(packages.Where(x => x.ItemId == productRequestDetail.ItemId).Select(s => s.Packages).FirstOrDefault());
				modelList.Add(model);
			}
			return modelList;
		}

        public async Task<bool> SaveProductRequestDetails(int productRequestHeaderId, List<ProductRequestDetailDto> productRequestDetails)
        {
            if (productRequestDetails.Any())
            {
                await DeleteProductRequestDetail(productRequestDetails, productRequestHeaderId);
                await AddProductRequestDetail(productRequestDetails, productRequestHeaderId);
                await EditProductRequestDetail(productRequestDetails);
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteProductRequestDetails(int productRequestHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.ProductRequestHeaderId == productRequestHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteProductRequestDetail(List<ProductRequestDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.ProductRequestHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.ProductRequestDetailId != p.ProductRequestDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> AddProductRequestDetail(List<ProductRequestDetailDto> details, int headerId)
        {
            var current = details.Where(x => x.ProductRequestDetailId <= 0).ToList();
            var modelList = new List<ProductRequestDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new ProductRequestDetail()
                {
                    ProductRequestDetailId = newId,
                    ProductRequestHeaderId = headerId,
                    CostCenterId = detail.CostCenterId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    Quantity = detail.Quantity,
                    ConsumerPrice = detail.ConsumerPrice,
                    ConsumerValue = detail.ConsumerValue,
                    CostPrice = detail.CostPrice,
                    CostPackage = detail.CostPackage,
                    CostValue = detail.CostValue,
                    Notes = detail.Notes,
                    ItemNote = detail.ItemNote,

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

        private async Task<bool> EditProductRequestDetail(List<ProductRequestDetailDto> productRequestDetails)
        {
            var current = productRequestDetails.Where(x => x.ProductRequestDetailId > 0).ToList();
            var modelList = new List<ProductRequestDetail>();
            foreach (var detail in current)
            {
                var model = new ProductRequestDetail()
                {
                    ProductRequestDetailId = detail.ProductRequestDetailId,
                    ProductRequestHeaderId = detail.ProductRequestHeaderId,
                    CostCenterId = detail.CostCenterId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    Quantity = detail.Quantity,
                    ConsumerPrice = detail.ConsumerPrice,
                    ConsumerValue = detail.ConsumerValue,
                    CostPrice = detail.CostPrice,
                    CostPackage = detail.CostPackage,
                    CostValue = detail.CostValue,
                    Notes = detail.Notes,
                    ItemNote = detail.ItemNote,

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
            try { id = await _repository.GetAll().MaxAsync(a => a.ProductRequestDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
