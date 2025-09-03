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
	public class PurchaseOrderDetailService : BaseService<PurchaseOrderDetail>, IPurchaseOrderDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemTaxService _itemTaxService;

		public PurchaseOrderDetailService(IRepository<PurchaseOrderDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService,IHttpContextAccessor httpContextAccessor, IItemTaxService itemTaxService) : base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
			_httpContextAccessor = httpContextAccessor;
            _itemTaxService = itemTaxService;
        }

		public async Task<List<PurchaseOrderDetailDto>> GetPurchaseOrderDetails(int purchaseOrderHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var modelList = new List<PurchaseOrderDetailDto>();

			var data =
			 await (from purchaseOrderDetail in _repository.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
				   from item in _itemService.GetAll().Where(x => x.ItemId == purchaseOrderDetail.ItemId)
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == purchaseOrderDetail.ItemPackageId)
				   from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == purchaseOrderDetail.CostCenterId).DefaultIfEmpty()
                   select new PurchaseOrderDetailDto
				   {
					   PurchaseOrderDetailId = purchaseOrderDetail.PurchaseOrderDetailId,
                       PurchaseOrderHeaderId = purchaseOrderDetail.PurchaseOrderHeaderId,
					   CostCenterId = purchaseOrderDetail.CostCenterId,
                       CostCenterName = costCenter != null? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                       ItemId = purchaseOrderDetail.ItemId,
                       ItemCode = item.ItemCode,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					   ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? purchaseOrderDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
                       TaxTypeId = item.TaxTypeId,
                       ItemTypeId = item.ItemTypeId,
					   ItemPackageId = purchaseOrderDetail.ItemPackageId,
					   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                       IsItemVatInclusive = purchaseOrderDetail.IsItemVatInclusive,
					   BarCode = purchaseOrderDetail.BarCode,
					   Packing = purchaseOrderDetail.Packing,
					   Quantity = purchaseOrderDetail.Quantity,
                       BonusQuantity = purchaseOrderDetail.BonusQuantity,
                       PurchasePrice = purchaseOrderDetail.PurchasePrice,
                       TotalValue = purchaseOrderDetail.TotalValue,
                       ItemDiscountPercent = purchaseOrderDetail.ItemDiscountPercent,
                       ItemDiscountValue = purchaseOrderDetail.ItemDiscountValue,
                       TotalValueAfterDiscount = purchaseOrderDetail.TotalValueAfterDiscount,
                       HeaderDiscountValue = purchaseOrderDetail.HeaderDiscountValue,
                       GrossValue = purchaseOrderDetail.GrossValue,
                       VatPercent = purchaseOrderDetail.VatPercent,
                       VatValue = purchaseOrderDetail.VatValue,
                       SubNetValue = purchaseOrderDetail.SubNetValue,
                       OtherTaxValue = purchaseOrderDetail.OtherTaxValue,
                       NetValue = purchaseOrderDetail.NetValue,
                       Notes = purchaseOrderDetail.Notes,
                       ItemNote = purchaseOrderDetail.ItemNote,
                       ConsumerPrice = purchaseOrderDetail.ConsumerPrice,
                       CostPrice = purchaseOrderDetail.CostPrice,
                       CostPackage = purchaseOrderDetail.CostPackage,
                       CostValue = purchaseOrderDetail.CostValue,
                       LastPurchasePrice = purchaseOrderDetail.LastPurchasePrice,

                       CreatedAt = purchaseOrderDetail.CreatedAt,
                       IpAddressCreated = purchaseOrderDetail.IpAddressCreated,
                       UserNameCreated = purchaseOrderDetail.UserNameCreated,
				   }).ToListAsync();

            var itemIds = data.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var purchaseOrderDetail in data)
			{
				var model = new PurchaseOrderDetailDto();
				model = purchaseOrderDetail;
                model.Packages = packages.Where(x => x.ItemId == purchaseOrderDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
				model.ItemTaxData = itemTaxData.Where(x => x.ItemId == purchaseOrderDetail.ItemId).ToList();
                model.Taxes = model.ItemTaxData.ToJson();
                modelList.Add(model);
			}
			return modelList;
		}

        public IQueryable<PurchaseOrderDetailDto> GetPurchaseOrderDetailsAsQueryable(int purchaseOrderHeaderId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data =
                   (from purchaseOrderDetail in _repository.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId)
                    from item in _itemService.GetAll().Where(x => x.ItemId == purchaseOrderDetail.ItemId)
                    from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == purchaseOrderDetail.ItemPackageId)
                    from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == purchaseOrderDetail.CostCenterId).DefaultIfEmpty()
                    select new PurchaseOrderDetailDto
                    {
                        PurchaseOrderDetailId = purchaseOrderDetail.PurchaseOrderDetailId,
                        PurchaseOrderHeaderId = purchaseOrderDetail.PurchaseOrderHeaderId,
                        CostCenterId = purchaseOrderDetail.CostCenterId,
                        CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                        ItemId = purchaseOrderDetail.ItemId,
                        ItemCode = item.ItemCode,
					    ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					    ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? purchaseOrderDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
						TaxTypeId = item.TaxTypeId,
                        ItemTypeId = item.ItemTypeId,
                        ItemPackageId = purchaseOrderDetail.ItemPackageId,
                        ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                        IsItemVatInclusive = purchaseOrderDetail.IsItemVatInclusive,
                        BarCode = purchaseOrderDetail.BarCode,
                        Packing = purchaseOrderDetail.Packing,
                        Quantity = purchaseOrderDetail.Quantity,
                        BonusQuantity = purchaseOrderDetail.BonusQuantity,
                        PurchasePrice = purchaseOrderDetail.PurchasePrice,
                        TotalValue = purchaseOrderDetail.TotalValue,
                        ItemDiscountPercent = purchaseOrderDetail.ItemDiscountPercent,
                        ItemDiscountValue = purchaseOrderDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = purchaseOrderDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = purchaseOrderDetail.HeaderDiscountValue,
                        GrossValue = purchaseOrderDetail.GrossValue,
                        VatPercent = purchaseOrderDetail.VatPercent,
                        VatValue = purchaseOrderDetail.VatValue,
                        SubNetValue = purchaseOrderDetail.SubNetValue,
                        OtherTaxValue = purchaseOrderDetail.OtherTaxValue,
                        NetValue = purchaseOrderDetail.NetValue,
                        Notes = purchaseOrderDetail.Notes,
                        ItemNote = purchaseOrderDetail.ItemNote,
                        ConsumerPrice = purchaseOrderDetail.ConsumerPrice,
                        CostPrice = purchaseOrderDetail.CostPrice,
                        CostPackage = purchaseOrderDetail.CostPackage,
                        CostValue = purchaseOrderDetail.CostValue,
                        LastPurchasePrice = purchaseOrderDetail.LastPurchasePrice,
                        CreatedAt = purchaseOrderDetail.CreatedAt,
                        IpAddressCreated = purchaseOrderDetail.IpAddressCreated,
                        UserNameCreated = purchaseOrderDetail.UserNameCreated,
                    });

            return data;
        }

        public async Task<List<PurchaseOrderDetailDto>> GetPurchaseOrderDetailsGrouped(int purchaseOrderHeaderId)
        {
            var details = await GetPurchaseOrderDetailsAsQueryable(purchaseOrderHeaderId).ToListAsync();
            return GroupPurchaseOrderDetails(details);
        }

        public async Task<List<PurchaseOrderDetailDto>> GetPurchaseOrderDetailsFullyGrouped(int purchaseOrderHeaderId)
        {
            //(ItemId, ItemPackageId, BarCode, PurchasePrice, ItemDiscountPercent) are the main keys, the other keys are related to these 5
            return await GetPurchaseOrderDetailsAsQueryable(purchaseOrderHeaderId).GroupBy(x => new { x.ItemId, x.ItemCode, x.ItemName, x.TaxTypeId, x.ItemTypeId, x.ItemPackageId, x.ItemPackageName, x.IsItemVatInclusive, x.BarCode, x.Packing, x.PurchasePrice, x.ItemDiscountPercent, x.VatPercent, x.Notes, x.ItemNote })
                .Select(x => new PurchaseOrderDetailDto
                {
					PurchaseOrderDetailId = x.Select(y => y.PurchaseOrderDetailId).First(),
					ItemId = x.Key.ItemId,
                    ItemCode = x.Key.ItemCode, 
                    ItemName = x.Key.ItemName, 
                    TaxTypeId = x.Key.TaxTypeId, 
                    ItemTypeId = x.Key.ItemTypeId, 
                    ItemPackageId = x.Key.ItemPackageId, 
                    ItemPackageName = x.Key.ItemPackageName, 
                    IsItemVatInclusive = x.Key.IsItemVatInclusive, 
                    BarCode = x.Key.BarCode, 
                    Packing = x.Key.Packing, 
                    //Quantity = x.Sum(y => y.Quantity), 
                    //BonusQuantity = x.Sum(y => y.BonusQuantity), 
                    PurchasePrice = x.Key.PurchasePrice, 
                    ItemDiscountPercent = x.Key.ItemDiscountPercent, 
                    VatPercent = x.Key.VatPercent, 
                    Notes = x.Key.Notes, 
                    ItemNote = x.Key.ItemNote
                }).ToListAsync();
        }

		public IQueryable<PurchaseOrderDetailDto> GetPurchaseOrderDetailsGroupedQueryable(int purchaseOrderHeaderId)
		{
            return _repository.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent }).Select(
                x => new PurchaseOrderDetailDto
                {
                    ItemId = x.Key.ItemId,
                    ItemPackageId = x.Key.ItemPackageId,
                    BarCode = x.Key.BarCode,
                    PurchasePrice = x.Key.PurchasePrice,
                    ItemDiscountPercent = x.Key.ItemDiscountPercent,
                    Quantity = x.Sum(y => y.Quantity),
                    BonusQuantity = x.Sum(y => y.BonusQuantity)
                });
		}

		public List<PurchaseOrderDetailDto> GroupPurchaseOrderDetails(List<PurchaseOrderDetailDto> details)
        {
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent }).Select(
	            x => new PurchaseOrderDetailDto
	            {
		            ItemId = x.Key.ItemId,
		            ItemPackageId = x.Key.ItemPackageId,
		            BarCode = x.Key.BarCode,
		            PurchasePrice = x.Key.PurchasePrice,
		            ItemDiscountPercent = x.Key.ItemDiscountPercent,
		            Quantity = x.Sum(y => y.Quantity),
		            BonusQuantity = x.Sum(y => y.BonusQuantity)
	            }).ToList();
		}

        public async Task<List<PurchaseOrderDetailDto>> SavePurchaseOrderDetails(int purchaseOrderHeaderId, List<PurchaseOrderDetailDto> purchaseOrderDetails)
        {
            if (purchaseOrderDetails.Any())
            {
                await EditPurchaseOrderDetail(purchaseOrderDetails);
                return await AddPurchaseOrderDetail(purchaseOrderDetails, purchaseOrderHeaderId);
            }
            return purchaseOrderDetails;
        }
        public async Task<bool> DeletePurchaseOrderDetails(int purchaseOrderHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeletePurchaseOrderDetailList(List<PurchaseOrderDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.PurchaseOrderHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.PurchaseOrderDetailId != p.PurchaseOrderDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<List<PurchaseOrderDetailDto>> AddPurchaseOrderDetail(List<PurchaseOrderDetailDto> details, int headerId)
        {            
            var current = details.Where(x => x.PurchaseOrderDetailId <= 0).ToList();
            var modelList = new List<PurchaseOrderDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new PurchaseOrderDetail()
                {
                    PurchaseOrderDetailId = newId,
                    PurchaseOrderHeaderId = headerId,
                    CostCenterId = detail.CostCenterId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    Quantity = detail.Quantity,
                    BonusQuantity = detail.BonusQuantity,
                    PurchasePrice = detail.PurchasePrice,
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

                detail.PurchaseOrderDetailId = newId;
                detail.PurchaseOrderHeaderId = headerId;

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

        private async Task<bool> EditPurchaseOrderDetail(List<PurchaseOrderDetailDto> purchaseOrderDetails)
        {
            var current = purchaseOrderDetails.Where(x => x.PurchaseOrderDetailId > 0).ToList();
            var modelList = new List<PurchaseOrderDetail>();
            foreach (var detail in current)
            {
                var model = new PurchaseOrderDetail()
                {
                    PurchaseOrderDetailId = detail.PurchaseOrderDetailId,
                    PurchaseOrderHeaderId = detail.PurchaseOrderHeaderId,
                    CostCenterId = detail.CostCenterId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    Quantity = detail.Quantity,
                    BonusQuantity = detail.BonusQuantity,
                    PurchasePrice = detail.PurchasePrice,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.PurchaseOrderDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
