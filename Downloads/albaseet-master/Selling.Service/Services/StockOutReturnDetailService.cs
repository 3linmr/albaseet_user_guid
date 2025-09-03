using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne;
using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Helper.Identity;
using Shared.Helper.Extensions;
using Shared.Helper.Logic;
using Shared.CoreOne.Models.StaticData;

namespace Sales.Service.Services
{
    public class StockOutReturnDetailService: BaseService<StockOutReturnDetail>, IStockOutReturnDetailService
    {
        private readonly IItemPackageService _itemPackageService;
        private readonly IItemService _itemService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemTaxService _itemTaxService;

        public StockOutReturnDetailService(IRepository<StockOutReturnDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService, IHttpContextAccessor httpContextAccessor, IItemTaxService itemTaxService) : base(repository)
        {
            _itemPackageService = itemPackageService;
            _itemService = itemService;
            _itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
            _httpContextAccessor = httpContextAccessor;
            _itemTaxService = itemTaxService;
        }

        public IQueryable<StockOutReturnDetailDto> GetStockOutReturnDetailsAsQueryable(int stockOutReturnHeaderId)
        {
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return from stockOutReturnDetail in _repository.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeaderId)
				   from item in _itemService.GetAll().Where(x => x.ItemId == stockOutReturnDetail.ItemId)
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == stockOutReturnDetail.ItemPackageId)
				   from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == stockOutReturnDetail.CostCenterId).DefaultIfEmpty()
				   select new StockOutReturnDetailDto
				   {
					   StockOutReturnDetailId = stockOutReturnDetail.StockOutReturnDetailId,
					   StockOutReturnHeaderId = stockOutReturnDetail.StockOutReturnHeaderId,
					   CostCenterId = stockOutReturnDetail.CostCenterId,
					   CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
					   ItemId = stockOutReturnDetail.ItemId,
					   ItemCode = item.ItemCode,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					   ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? stockOutReturnDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
					   TaxTypeId = item.TaxTypeId,
                       ItemTypeId = item.ItemTypeId,
					   ItemPackageId = stockOutReturnDetail.ItemPackageId,
					   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					   IsItemVatInclusive = stockOutReturnDetail.IsItemVatInclusive,
					   BarCode = stockOutReturnDetail.BarCode,
					   Packing = stockOutReturnDetail.Packing,
					   ExpireDate = stockOutReturnDetail.ExpireDate,
					   BatchNumber = stockOutReturnDetail.BatchNumber,
					   Quantity = stockOutReturnDetail.Quantity,
					   BonusQuantity = stockOutReturnDetail.BonusQuantity,
					   /*Other quantites computed in handling service*/
					   SellingPrice = stockOutReturnDetail.SellingPrice,
					   TotalValue = stockOutReturnDetail.TotalValue,
					   ItemDiscountPercent = stockOutReturnDetail.ItemDiscountPercent,
					   ItemDiscountValue = stockOutReturnDetail.ItemDiscountValue,
					   TotalValueAfterDiscount = stockOutReturnDetail.TotalValueAfterDiscount,
					   HeaderDiscountValue = stockOutReturnDetail.HeaderDiscountValue,
					   GrossValue = stockOutReturnDetail.GrossValue,
					   VatPercent = stockOutReturnDetail.VatPercent,
					   VatValue = stockOutReturnDetail.VatValue,
					   SubNetValue = stockOutReturnDetail.SubNetValue,
					   OtherTaxValue = stockOutReturnDetail.OtherTaxValue,
					   NetValue = stockOutReturnDetail.NetValue,
					   Notes = stockOutReturnDetail.Notes,
                       ItemNote = stockOutReturnDetail.ItemNote,
					   ConsumerPrice = stockOutReturnDetail.ConsumerPrice,
					   CostPrice = stockOutReturnDetail.CostPrice,
					   CostPackage = stockOutReturnDetail.CostPackage,
					   CostValue = stockOutReturnDetail.CostValue,
					   LastSalesPrice = stockOutReturnDetail.LastSalesPrice,

					   CreatedAt = stockOutReturnDetail.CreatedAt,
					   IpAddressCreated = stockOutReturnDetail.IpAddressCreated,
					   UserNameCreated = stockOutReturnDetail.UserNameCreated,
				   };
		}

        public async Task<List<StockOutReturnDetailDto>> GetStockOutReturnDetails(int stockOutReturnHeaderId)
        {
            var modelList = new List<StockOutReturnDetailDto>();

            var data = await GetStockOutReturnDetailsAsQueryable(stockOutReturnHeaderId).ToListAsync();

            var itemIds = data.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var stockOutReturnDetail in data)
            {
                var model = new StockOutReturnDetailDto();
                model = stockOutReturnDetail;
                model.Packages = packages.Where(x => x.ItemId == stockOutReturnDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                model.ItemTaxData = itemTaxData.Where(x => x.ItemId == stockOutReturnDetail.ItemId).ToList();
                model.Taxes = model.ItemTaxData.ToJson();
                modelList.Add(model);
            }
            return modelList;
		}

		public List<StockOutReturnDetailDto> GroupStockOutReturnDetailsWithAllKeys(List<StockOutReturnDetailDto> details)
		{
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber, x.CostCenterId, x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(
				x => new StockOutReturnDetailDto
				{
					ItemId = x.Key.ItemId,
					ItemPackageId = x.Key.ItemPackageId,
					ExpireDate = x.Key.ExpireDate,
					BatchNumber = x.Key.BatchNumber,
					CostCenterId = x.Key.CostCenterId,
					BarCode = x.Key.BarCode,
					SellingPrice = x.Key.SellingPrice,
					ItemDiscountPercent = x.Key.ItemDiscountPercent,
					Quantity = x.Sum(y => y.Quantity),
					BonusQuantity = x.Sum(y => y.BonusQuantity)
				}).ToList();
		}

		public List<StockOutReturnDetailDto> GroupStockOutReturnDetails(List<StockOutReturnDetailDto> details)
		{
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(
				x => new StockOutReturnDetailDto
				{
					ItemId = x.Key.ItemId,
					ItemPackageId = x.Key.ItemPackageId,
					BarCode = x.Key.BarCode,
					SellingPrice = x.Key.SellingPrice,
					ItemDiscountPercent = x.Key.ItemDiscountPercent,
					Quantity = x.Sum(y => y.Quantity),
					BonusQuantity = x.Sum(y => y.BonusQuantity)
				}).ToList();
		}

		public async Task<List<StockOutReturnDetailDto>> SaveStockOutReturnDetails(int stockOutReturnHeaderId, List<StockOutReturnDetailDto> stockOutReturnDetails)
        {
            if (stockOutReturnDetails.Any())
            {
                await EditStockOutReturnDetail(stockOutReturnDetails);
                return await AddStockOutReturnDetail(stockOutReturnDetails, stockOutReturnHeaderId);
            }
            return stockOutReturnDetails;
        }
        public async Task<bool> DeleteStockOutReturnDetails(int stockOutReturnHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteStockOutReturnDetailList(List<StockOutReturnDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.StockOutReturnHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.StockOutReturnDetailId != p.StockOutReturnDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<List<StockOutReturnDetailDto>> AddStockOutReturnDetail(List<StockOutReturnDetailDto> details, int headerId)
        {
            var current = details.Where(x => x.StockOutReturnDetailId <= 0).ToList();
            var modelList = new List<StockOutReturnDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new StockOutReturnDetail()
                {
                    StockOutReturnDetailId = newId,
                    StockOutReturnHeaderId = headerId,
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
                    SellingPrice = detail.SellingPrice,
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
                    LastSalesPrice = detail.LastSalesPrice,

                    Hide = false,
                    CreatedAt = DateHelper.GetDateTimeNow(),
                    UserNameCreated = await _httpContextAccessor!.GetUserName(),
                    IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                };

                detail.StockOutReturnDetailId = newId;
                detail.StockOutReturnHeaderId = headerId;

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

        private async Task<bool> EditStockOutReturnDetail(List<StockOutReturnDetailDto> stockOutReturnDetails)
        {
            var current = stockOutReturnDetails.Where(x => x.StockOutReturnDetailId > 0).ToList();
            var modelList = new List<StockOutReturnDetail>();
            foreach (var detail in current)
            {
                var model = new StockOutReturnDetail()
                {
                    StockOutReturnDetailId = detail.StockOutReturnDetailId,
                    StockOutReturnHeaderId = detail.StockOutReturnHeaderId,
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
                    SellingPrice = detail.SellingPrice,
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
                    LastSalesPrice = detail.LastSalesPrice,

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
            try { id = await _repository.GetAll().MaxAsync(a => a.StockOutReturnDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
