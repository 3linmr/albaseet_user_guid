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
	public class StockInReturnDetailService : BaseService<StockInReturnDetail>, IStockInReturnDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemTaxService _itemTaxService;

		public StockInReturnDetailService(IRepository<StockInReturnDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService,IHttpContextAccessor httpContextAccessor, IItemTaxService itemTaxService) : base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
			_httpContextAccessor = httpContextAccessor;
            _itemTaxService = itemTaxService;
        }

        public async Task<List<StockInReturnDetailDto>> GetStockInReturnDetails(int stockInReturnHeaderId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var stockInReturnDetails = 
                await (from stockInReturnDetail in _repository.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeaderId)
                    from item in _itemService.GetAll().Where(x => x.ItemId == stockInReturnDetail.ItemId)
                    from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == stockInReturnDetail.ItemPackageId)
                    from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == stockInReturnDetail.CostCenterId).DefaultIfEmpty()
                    select new StockInReturnDetailDto
                    {
                        StockInReturnDetailId = stockInReturnDetail.StockInReturnDetailId,
                        StockInReturnHeaderId = stockInReturnDetail.StockInReturnHeaderId,
                        CostCenterId = stockInReturnDetail.CostCenterId,
                        CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                        ItemId = stockInReturnDetail.ItemId,
                        ItemCode = item.ItemCode,
                        ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                        ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? stockInReturnDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
                        TaxTypeId = item.TaxTypeId,
                        ItemTypeId = item.ItemTypeId,
                        ItemPackageId = stockInReturnDetail.ItemPackageId,
                        ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                        IsItemVatInclusive = stockInReturnDetail.IsItemVatInclusive,
                        BarCode = stockInReturnDetail.BarCode,
                        Packing = stockInReturnDetail.Packing,
                        ExpireDate = stockInReturnDetail.ExpireDate,
                        BatchNumber = stockInReturnDetail.BatchNumber,
                        Quantity = stockInReturnDetail.Quantity,
                        BonusQuantity = stockInReturnDetail.BonusQuantity,
                        //PurchaseOrderQuantity = 0,
                        //PurchaseOrderBonusQuantity = 0,
                        //QuantityReceived = 0,
                        //BonusQuantityReceived = 0,
                        PurchasePrice = stockInReturnDetail.PurchasePrice,
                        TotalValue = stockInReturnDetail.TotalValue,
                        ItemDiscountPercent = stockInReturnDetail.ItemDiscountPercent,
                        ItemDiscountValue = stockInReturnDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = stockInReturnDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = stockInReturnDetail.HeaderDiscountValue,
                        GrossValue = stockInReturnDetail.GrossValue,
                        VatPercent = stockInReturnDetail.VatPercent,
                        VatValue = stockInReturnDetail.VatValue,
                        SubNetValue = stockInReturnDetail.SubNetValue,
                        OtherTaxValue = stockInReturnDetail.OtherTaxValue,
                        NetValue = stockInReturnDetail.NetValue,
                        Notes = stockInReturnDetail.Notes,
                        ItemNote = stockInReturnDetail.ItemNote,
                        ConsumerPrice = stockInReturnDetail.ConsumerPrice,
                        CostPrice = stockInReturnDetail.CostPrice,
                        CostPackage = stockInReturnDetail.CostPackage,
                        CostValue = stockInReturnDetail.CostValue,
                        LastPurchasePrice = stockInReturnDetail.LastPurchasePrice,
                        CreatedAt = stockInReturnDetail.CreatedAt,
                        IpAddressCreated = stockInReturnDetail.IpAddressCreated,
                        UserNameCreated = stockInReturnDetail.UserNameCreated,
                    }).ToListAsync();

            var itemIds = stockInReturnDetails.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var stockInReturnDetail in stockInReturnDetails)
            {
                stockInReturnDetail.Packages = packages.Where(x => x.ItemId == stockInReturnDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                stockInReturnDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == stockInReturnDetail.ItemId).ToList();
                stockInReturnDetail.Taxes = stockInReturnDetail.ItemTaxData.ToJson();
            }

            return stockInReturnDetails;
        }

        public IQueryable<StockInReturnDetailDto> GetStockInReturnDetailsAsQueryable(int stockInReturnHeaderId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data =
                   (from stockInReturnDetail in _repository.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeaderId)
                    from item in _itemService.GetAll().Where(x => x.ItemId == stockInReturnDetail.ItemId)
                    from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == stockInReturnDetail.ItemPackageId)
                    from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == stockInReturnDetail.CostCenterId).DefaultIfEmpty()
                    select new StockInReturnDetailDto
                    {
                        StockInReturnDetailId = stockInReturnDetail.StockInReturnDetailId,
                        StockInReturnHeaderId = stockInReturnDetail.StockInReturnHeaderId,
                        CostCenterId = stockInReturnDetail.CostCenterId,
                        CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                        ItemId = stockInReturnDetail.ItemId,
                        ItemCode = item.ItemCode,
                        ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                        ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? stockInReturnDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
						TaxTypeId = item.TaxTypeId,
                        ItemTypeId = item.ItemTypeId,
                        ItemPackageId = stockInReturnDetail.ItemPackageId,
                        ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                        IsItemVatInclusive = stockInReturnDetail.IsItemVatInclusive,
                        BarCode = stockInReturnDetail.BarCode,
                        Packing = stockInReturnDetail.Packing,
                        ExpireDate = stockInReturnDetail.ExpireDate,
                        BatchNumber = stockInReturnDetail.BatchNumber,
                        Quantity = stockInReturnDetail.Quantity,
                        BonusQuantity = stockInReturnDetail.BonusQuantity,
                        //PurchaseOrderQuantity = 0,
                        //PurchaseOrderBonusQuantity = 0,
                        //QuantityReceived = 0,
                        //BonusQuantityReceived = 0,
                        PurchasePrice = stockInReturnDetail.PurchasePrice,
                        TotalValue = stockInReturnDetail.TotalValue,
                        ItemDiscountPercent = stockInReturnDetail.ItemDiscountPercent,
                        ItemDiscountValue = stockInReturnDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = stockInReturnDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = stockInReturnDetail.HeaderDiscountValue,
                        GrossValue = stockInReturnDetail.GrossValue,
                        VatPercent = stockInReturnDetail.VatPercent,
                        VatValue = stockInReturnDetail.VatValue,
                        SubNetValue = stockInReturnDetail.SubNetValue,
                        OtherTaxValue = stockInReturnDetail.OtherTaxValue,
                        NetValue = stockInReturnDetail.NetValue,
                        Notes = stockInReturnDetail.Notes,
                        ItemNote = stockInReturnDetail.ItemNote,
                        ConsumerPrice = stockInReturnDetail.ConsumerPrice,
                        CostPrice = stockInReturnDetail.CostPrice,
                        CostPackage = stockInReturnDetail.CostPackage,
                        CostValue = stockInReturnDetail.CostValue,
                        LastPurchasePrice = stockInReturnDetail.LastPurchasePrice,
                        CreatedAt = stockInReturnDetail.CreatedAt,
                        IpAddressCreated = stockInReturnDetail.IpAddressCreated,
                        UserNameCreated = stockInReturnDetail.UserNameCreated,
                    });

            return data;
		}

		public List<StockInReturnDetailDto> GroupStockInReturnDetailsWithAllKeys(List<StockInReturnDetailDto> details)
		{
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber, x.CostCenterId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent }).Select(
				x => new StockInReturnDetailDto
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

		public List<StockInReturnDetailDto> GroupStockInReturnDetails(List<StockInReturnDetailDto> details)
		{
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent }).Select(
				x => new StockInReturnDetailDto
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

		public async Task<List<StockInReturnDetailDto>> SaveStockInReturnDetails(int stockInReturnHeaderId, List<StockInReturnDetailDto> stockInReturnDetails)
        {
            if (stockInReturnDetails.Any())
            {
                await EditStockInReturnDetail(stockInReturnDetails);
                return await AddStockInReturnDetail(stockInReturnDetails, stockInReturnHeaderId);
            }
            return stockInReturnDetails;
        }

        public async Task<bool> DeleteStockInReturnDetails(int stockInReturnHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteStockInReturnDetailList(List<StockInReturnDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.StockInReturnHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.StockInReturnDetailId != p.StockInReturnDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<List<StockInReturnDetailDto>> AddStockInReturnDetail(List<StockInReturnDetailDto> details, int headerId)
        {            
            var current = details.Where(x => x.StockInReturnDetailId <= 0).ToList();
            var modelList = new List<StockInReturnDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new StockInReturnDetail()
                {
                    StockInReturnDetailId = newId,
                    StockInReturnHeaderId = headerId,
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

                detail.StockInReturnDetailId = newId;
                detail.StockInReturnHeaderId = headerId;

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

        private async Task<bool> EditStockInReturnDetail(List<StockInReturnDetailDto> stockInReturnDetails)
        {
            var current = stockInReturnDetails.Where(x => x.StockInReturnDetailId > 0).ToList();
            var modelList = new List<StockInReturnDetail>();
            foreach (var detail in current)
            {
                var model = new StockInReturnDetail()
                {
                    StockInReturnDetailId = detail.StockInReturnDetailId,
                    StockInReturnHeaderId = detail.StockInReturnHeaderId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.StockInReturnDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
