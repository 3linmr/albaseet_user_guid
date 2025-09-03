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
using System.Security.Cryptography.X509Certificates;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Text.Json;
using Shared.Helper.Extensions;
using Shared.Helper.Logic;
using static Shared.Helper.Models.StaticData.LanguageData;
using Shared.CoreOne.Models.StaticData;

namespace Purchases.Service.Services
{
	public class StockInDetailService : BaseService<StockInDetail>, IStockInDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemTaxService _itemTaxService;

		public StockInDetailService(IRepository<StockInDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService,IHttpContextAccessor httpContextAccessor, IItemTaxService itemTaxService) : base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
			_httpContextAccessor = httpContextAccessor;
            _itemTaxService = itemTaxService;
        }

        public async Task<List<StockInDetailDto>> GetStockInDetails(int stockInHeaderId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var stockInDetails = 
                await (from stockInDetail in _repository.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId)
                    from item in _itemService.GetAll().Where(x => x.ItemId == stockInDetail.ItemId)
                    from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == stockInDetail.ItemPackageId)
                    from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == stockInDetail.CostCenterId).DefaultIfEmpty()
                    select new StockInDetailDto
                    {
                        StockInDetailId = stockInDetail.StockInDetailId,
                        StockInHeaderId = stockInDetail.StockInHeaderId,
                        CostCenterId = stockInDetail.CostCenterId,
                        CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                        ItemId = stockInDetail.ItemId,
                        ItemCode = item.ItemCode,
                        ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                        ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? stockInDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
                        TaxTypeId = item.TaxTypeId,
                        ItemTypeId = item.ItemTypeId,
                        ItemPackageId = stockInDetail.ItemPackageId,
                        ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                        IsItemVatInclusive = stockInDetail.IsItemVatInclusive,
                        BarCode = stockInDetail.BarCode,
                        Packing = stockInDetail.Packing,
                        ExpireDate = stockInDetail.ExpireDate,
                        BatchNumber = stockInDetail.BatchNumber,
                        Quantity = stockInDetail.Quantity,
                        BonusQuantity = stockInDetail.BonusQuantity,
                        //PurchaseOrderQuantity = 0,
                        //PurchaseOrderBonusQuantity = 0,
                        //QuantityReceived = 0,
                        //BonusQuantityReceived = 0,
                        PurchasePrice = stockInDetail.PurchasePrice,
                        TotalValue = stockInDetail.TotalValue,
                        ItemDiscountPercent = stockInDetail.ItemDiscountPercent,
                        ItemDiscountValue = stockInDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = stockInDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = stockInDetail.HeaderDiscountValue,
                        GrossValue = stockInDetail.GrossValue,
                        VatPercent = stockInDetail.VatPercent,
                        VatValue = stockInDetail.VatValue,
                        SubNetValue = stockInDetail.SubNetValue,
                        OtherTaxValue = stockInDetail.OtherTaxValue,
                        NetValue = stockInDetail.NetValue,
                        Notes = stockInDetail.Notes,
                        ItemNote = stockInDetail.ItemNote,
                        ConsumerPrice = stockInDetail.ConsumerPrice,
                        CostPrice = stockInDetail.CostPrice,
                        CostPackage = stockInDetail.CostPackage,
                        CostValue = stockInDetail.CostValue,
                        LastPurchasePrice = stockInDetail.LastPurchasePrice,
                        CreatedAt = stockInDetail.CreatedAt,
                        IpAddressCreated = stockInDetail.IpAddressCreated,
                        UserNameCreated = stockInDetail.UserNameCreated,
                    }).ToListAsync();

            var itemIds = stockInDetails.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var stockInDetail in stockInDetails)
            {
                stockInDetail.Packages = packages.Where(x => x.ItemId == stockInDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                stockInDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == stockInDetail.ItemId).ToList();
                stockInDetail.Taxes = stockInDetail.ItemTaxData.ToJson();
            }

            return stockInDetails;
        }

        public IQueryable<StockInDetailDto> GetStockInDetailsAsQueryable(int stockInHeaderId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data =
                   (from stockInDetail in _repository.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId)
                    from item in _itemService.GetAll().Where(x => x.ItemId == stockInDetail.ItemId)
                    from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == stockInDetail.ItemPackageId)
                    from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == stockInDetail.CostCenterId).DefaultIfEmpty()
                    select new StockInDetailDto
                    {
                        StockInDetailId = stockInDetail.StockInDetailId,
                        StockInHeaderId = stockInDetail.StockInHeaderId,
                        CostCenterId = stockInDetail.CostCenterId,
                        CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                        ItemId = stockInDetail.ItemId,
                        ItemCode = item.ItemCode,
                        ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                        ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? stockInDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
						TaxTypeId = item.TaxTypeId,
                        ItemTypeId = item.ItemTypeId,
                        ItemPackageId = stockInDetail.ItemPackageId,
                        ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                        IsItemVatInclusive = stockInDetail.IsItemVatInclusive,
                        BarCode = stockInDetail.BarCode,
                        Packing = stockInDetail.Packing,
                        ExpireDate = stockInDetail.ExpireDate,
                        BatchNumber = stockInDetail.BatchNumber,
                        Quantity = stockInDetail.Quantity,
                        BonusQuantity = stockInDetail.BonusQuantity,
                        //PurchaseOrderQuantity = 0,
                        //PurchaseOrderBonusQuantity = 0,
                        //QuantityReceived = 0,
                        //BonusQuantityReceived = 0,
                        PurchasePrice = stockInDetail.PurchasePrice,
                        TotalValue = stockInDetail.TotalValue,
                        ItemDiscountPercent = stockInDetail.ItemDiscountPercent,
                        ItemDiscountValue = stockInDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = stockInDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = stockInDetail.HeaderDiscountValue,
                        GrossValue = stockInDetail.GrossValue,
                        VatPercent = stockInDetail.VatPercent,
                        VatValue = stockInDetail.VatValue,
                        SubNetValue = stockInDetail.SubNetValue,
                        OtherTaxValue = stockInDetail.OtherTaxValue,
                        NetValue = stockInDetail.NetValue,
                        Notes = stockInDetail.Notes,
                        ItemNote = stockInDetail.ItemNote,
                        ConsumerPrice = stockInDetail.ConsumerPrice,
                        CostPrice = stockInDetail.CostPrice,
                        CostPackage = stockInDetail.CostPackage,
                        CostValue = stockInDetail.CostValue,
                        LastPurchasePrice = stockInDetail.LastPurchasePrice,
                        CreatedAt = stockInDetail.CreatedAt,
                        IpAddressCreated = stockInDetail.IpAddressCreated,
                        UserNameCreated = stockInDetail.UserNameCreated,
                    });

            return data;
		}

		public async Task<List<StockInDetailDto>> GetStockInDetailsGrouped(int stockInHeaderId)
		{
			var details = await GetStockInDetailsAsQueryable(stockInHeaderId).ToListAsync();
			return GroupStockInDetails(details);
		}

		public async Task<List<StockInDetailDto>> GetStockInDetailsGroupedWithAllKeys(int stockInHeaderId)
		{
			var details = await GetStockInDetailsAsQueryable(stockInHeaderId).ToListAsync();
			return GroupStockInDetailsWithAllKeys(details);
		}

		public List<StockInDetailDto> GroupStockInDetails(List<StockInDetailDto> details)
		{
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent }).Select(
				x => new StockInDetailDto
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

		public List<StockInDetailDto> GroupStockInDetailsWithAllKeys(List<StockInDetailDto> details)
		{
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber, x.CostCenterId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent }).Select(
				x => new StockInDetailDto
				{
					ItemId = x.Key.ItemId,
					ItemPackageId = x.Key.ItemPackageId,
					ExpireDate = x.Key.ExpireDate,
					BatchNumber = x.Key.BatchNumber,
					CostCenterId = x.Key.CostCenterId,
					BarCode = x.Key.BarCode,
					PurchasePrice = x.Key.PurchasePrice,
					ItemDiscountPercent = x.Key.ItemDiscountPercent,
					Quantity = x.Sum(y => y.Quantity),
					BonusQuantity = x.Sum(y => y.BonusQuantity)
				}).ToList();
		}

		public async Task<List<StockInDetailDto>> SaveStockInDetails(int stockInHeaderId, List<StockInDetailDto> stockInDetails)
        {
            if (stockInDetails.Any())
            {
                await EditStockInDetail(stockInDetails);
                return await AddStockInDetail(stockInDetails, stockInHeaderId);
            }
            return stockInDetails;
        }

        public async Task<bool> DeleteStockInDetails(int stockInHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteStockInDetailList(List<StockInDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.StockInHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.StockInDetailId != p.StockInDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<List<StockInDetailDto>> AddStockInDetail(List<StockInDetailDto> details, int headerId)
        {            
            var current = details.Where(x => x.StockInDetailId <= 0).ToList();
            var modelList = new List<StockInDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new StockInDetail()
                {
                    StockInDetailId = newId,
                    StockInHeaderId = headerId,
                    CostCenterId = detail.CostCenterId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    ExpireDate = detail.ExpireDate,
                    BatchNumber = detail.BatchNumber,
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

                detail.StockInDetailId = newId;
                detail.StockInHeaderId = headerId;

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

        private async Task<bool> EditStockInDetail(List<StockInDetailDto> stockInDetails)
        {
            var current = stockInDetails.Where(x => x.StockInDetailId > 0).ToList();
            var modelList = new List<StockInDetail>();
            foreach (var detail in current)
            {
                var model = new StockInDetail()
                {
                    StockInDetailId = detail.StockInDetailId,
                    StockInHeaderId = detail.StockInHeaderId,
                    CostCenterId = detail.CostCenterId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    ExpireDate = detail.ExpireDate,
                    BatchNumber = detail.BatchNumber,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.StockInDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
