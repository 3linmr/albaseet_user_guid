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
using Shared.Service.Logic.Calculation;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Text.Json;
using Shared.Helper.Extensions;
using Shared.Helper.Logic;
using Shared.CoreOne.Models.StaticData;


namespace Purchases.Service.Services
{
	public class ProductRequestPriceDetailService : BaseService<ProductRequestPriceDetail>, IProductRequestPriceDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemTaxService _itemTaxService;

		public ProductRequestPriceDetailService(IRepository<ProductRequestPriceDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService,IHttpContextAccessor httpContextAccessor,IItemTaxService itemTaxService) : base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
			_httpContextAccessor = httpContextAccessor;
            _itemTaxService = itemTaxService;
        }

		public async Task<List<ProductRequestPriceDetailDto>> GetProductRequestPriceDetails(int productRequestPriceHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var modelList = new List<ProductRequestPriceDetailDto>();

			var data =
			 await (from productRequestPriceDetail in _repository.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeaderId)
				   from item in _itemService.GetAll().Where(x => x.ItemId == productRequestPriceDetail.ItemId)
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == productRequestPriceDetail.ItemPackageId)
				   from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == productRequestPriceDetail.CostCenterId).DefaultIfEmpty()
                   select new ProductRequestPriceDetailDto
				   {
					   ProductRequestPriceDetailId = productRequestPriceDetail.ProductRequestPriceDetailId,
                       ProductRequestPriceHeaderId = productRequestPriceDetail.ProductRequestPriceHeaderId,
					   CostCenterId = productRequestPriceDetail.CostCenterId,
                       CostCenterName = costCenter != null? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                       ItemId = productRequestPriceDetail.ItemId,
                       ItemCode = item.ItemCode,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					   ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? productRequestPriceDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
                       TaxTypeId = item.TaxTypeId,
                       ItemTypeId = item.ItemTypeId,
					   ItemPackageId = productRequestPriceDetail.ItemPackageId,
					   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                       IsItemVatInclusive = productRequestPriceDetail.IsItemVatInclusive,
					   BarCode = productRequestPriceDetail.BarCode,
					   Packing = productRequestPriceDetail.Packing,
					   Quantity = productRequestPriceDetail.Quantity,
					   RequestedPrice = productRequestPriceDetail.RequestedPrice,
					   TotalValue = productRequestPriceDetail.TotalValue,
					   ItemDiscountPercent = productRequestPriceDetail.ItemDiscountPercent,
					   ItemDiscountValue = productRequestPriceDetail.ItemDiscountValue,
                       TotalValueAfterDiscount = productRequestPriceDetail.TotalValueAfterDiscount,
                       HeaderDiscountValue = productRequestPriceDetail.HeaderDiscountValue,
					   GrossValue = productRequestPriceDetail.GrossValue,
					   VatPercent = productRequestPriceDetail.VatPercent,
					   VatValue = productRequestPriceDetail.VatValue,
					   SubNetValue = productRequestPriceDetail.SubNetValue,
					   OtherTaxValue = productRequestPriceDetail.OtherTaxValue,
					   NetValue = productRequestPriceDetail.NetValue,
                       Notes = productRequestPriceDetail.Notes,
                       ItemNote = productRequestPriceDetail.ItemNote,
                       ConsumerPrice = productRequestPriceDetail.ConsumerPrice,
                       CostPrice = productRequestPriceDetail.CostPrice,
                       CostPackage = productRequestPriceDetail.CostPackage,
                       CostValue = productRequestPriceDetail.CostValue,
                       LastPurchasePrice = productRequestPriceDetail.LastPurchasePrice,
                       CreatedAt = productRequestPriceDetail.CreatedAt,
                       IpAddressCreated = productRequestPriceDetail.IpAddressCreated,
                       UserNameCreated = productRequestPriceDetail.UserNameCreated,
				   }).ToListAsync();

            var itemIds = data.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

			foreach (var productRequestPriceDetail in data)
			{
				var model = new ProductRequestPriceDetailDto();
				model = productRequestPriceDetail;
                model.Packages = packages.Where(x => x.ItemId == productRequestPriceDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                model.ItemTaxData = itemTaxData.Where(x => x.ItemId == productRequestPriceDetail.ItemId).ToList();
				model.Taxes = model.ItemTaxData.ToJson();
				modelList.Add(model);
			}
			return modelList;
		}

        public async Task<List<ProductRequestPriceDetailDto>> SaveProductRequestPriceDetails(int productRequestPriceHeaderId, List<ProductRequestPriceDetailDto> productRequestPriceDetails)
        {
            if (productRequestPriceDetails.Any())
            {
                await EditProductRequestPriceDetail(productRequestPriceDetails);
                return await AddProductRequestPriceDetail(productRequestPriceDetails, productRequestPriceHeaderId);
            }
            return productRequestPriceDetails;
        }
        public async Task<bool> DeleteProductRequestPriceDetails(int productRequestPriceHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteProductRequestPriceDetailList(List<ProductRequestPriceDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.ProductRequestPriceHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.ProductRequestPriceDetailId != p.ProductRequestPriceDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<List<ProductRequestPriceDetailDto>> AddProductRequestPriceDetail(List<ProductRequestPriceDetailDto> details, int headerId)
        {
            var current = details.Where(x => x.ProductRequestPriceDetailId <= 0).ToList();
            var modelList = new List<ProductRequestPriceDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new ProductRequestPriceDetail()
                {
                    ProductRequestPriceDetailId = newId,
                    ProductRequestPriceHeaderId = headerId,
                    CostCenterId = detail.CostCenterId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    Quantity = detail.Quantity,
                    RequestedPrice = detail.RequestedPrice,
					TotalValue = detail.TotalValue,
					ItemDiscountPercent = detail.ItemDiscountPercent,
					ItemDiscountValue = detail.ItemDiscountValue,
                    TotalValueAfterDiscount = detail.TotalValueAfterDiscount,
                    HeaderDiscountValue = detail.HeaderDiscountValue,
                    GrossValue = detail.GrossValue,
                    VatPercent = detail.VatPercent,
                    VatValue = detail.VatValue,
                    SubNetValue = detail.SubNetValue,
                    OtherTaxValue = detail.OtherTaxValue,
                    NetValue = detail.NetValue,
                    Notes = detail.Notes,
                    ItemNote = detail.ItemNote,
                    ConsumerPrice = detail.ConsumerPrice,
                    CostPrice = detail.CostPrice,
                    CostPackage = detail.CostPackage,
                    CostValue = detail.CostValue,
                    LastPurchasePrice = detail.LastPurchasePrice,

                    Hide = false,
                    CreatedAt = DateHelper.GetDateTimeNow(),
                    UserNameCreated = await _httpContextAccessor!.GetUserName(),
                    IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                };

                detail.ProductRequestPriceDetailId = newId;
                detail.ProductRequestPriceHeaderId = headerId;

                modelList.Add(model);
                newId++;
            }

            if (modelList.Any())
            {
                await _repository.InsertRange(modelList);
                await _repository.SaveChanges();
            }
            return details;
        }

        private async Task<bool> EditProductRequestPriceDetail(List<ProductRequestPriceDetailDto> productRequestPriceDetails)
        {            
            var current = productRequestPriceDetails.Where(x => x.ProductRequestPriceDetailId > 0).ToList();
            var modelList = new List<ProductRequestPriceDetail>();
            foreach (var detail in current)
            {
                var model = new ProductRequestPriceDetail()
                {
                    ProductRequestPriceDetailId = detail.ProductRequestPriceDetailId,
                    ProductRequestPriceHeaderId = detail.ProductRequestPriceHeaderId,
                    CostCenterId = detail.CostCenterId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    Quantity = detail.Quantity,
                    RequestedPrice = detail.RequestedPrice,
					TotalValue = detail.TotalValue,
					ItemDiscountPercent = detail.ItemDiscountPercent,
					ItemDiscountValue = detail.ItemDiscountValue,
                    TotalValueAfterDiscount = detail.TotalValueAfterDiscount,
                    HeaderDiscountValue = detail.HeaderDiscountValue,
                    GrossValue = detail.GrossValue,
                    VatPercent = detail.VatPercent,
                    VatValue = detail.VatValue,
                    SubNetValue = detail.SubNetValue,
                    OtherTaxValue = detail.OtherTaxValue,
                    NetValue = detail.NetValue,
                    Notes = detail.Notes,
                    ItemNote = detail.ItemNote,
                    ConsumerPrice = detail.ConsumerPrice,
                    CostPrice = detail.CostPrice,
                    CostPackage = detail.CostPackage,
                    CostValue = detail.CostValue,
                    LastPurchasePrice = detail.LastPurchasePrice,

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
            try { id = await _repository.GetAll().MaxAsync(a => a.ProductRequestPriceDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
