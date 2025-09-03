using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne;
using Shared.Service;
using Shared.Helper.Identity;
using Shared.Helper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Logic;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.Helper.Models.StaticData.LanguageData;
using Shared.CoreOne.Models.StaticData;

namespace Sales.Service.Services
{
    public class StockOutDetailService: BaseService<StockOutDetail>, IStockOutDetailService
    {
        private readonly IItemPackageService _itemPackageService;
        private readonly IItemService _itemService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemTaxService _itemTaxService;

        public StockOutDetailService(IRepository<StockOutDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService, IHttpContextAccessor httpContextAccessor, IItemTaxService itemTaxService) : base(repository)
        {
            _itemPackageService = itemPackageService;
            _itemService = itemService;
            _itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
            _httpContextAccessor = httpContextAccessor;
            _itemTaxService = itemTaxService;
        }

		public IQueryable<StockOutDetailDto> GetStockOutDetailsAsQueryable(int stockOutHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return from stockOutDetail in _repository.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId)
				   from item in _itemService.GetAll().Where(x => x.ItemId == stockOutDetail.ItemId)
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == stockOutDetail.ItemPackageId)
				   from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == stockOutDetail.CostCenterId).DefaultIfEmpty()
				   select new StockOutDetailDto
				   {
					   StockOutDetailId = stockOutDetail.StockOutDetailId,
					   StockOutHeaderId = stockOutDetail.StockOutHeaderId,
					   CostCenterId = stockOutDetail.CostCenterId,
					   CostCenterName = costCenter != null
						   ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn
						   : null,
					   ItemId = stockOutDetail.ItemId,
					   ItemCode = item.ItemCode,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					   ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? stockOutDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
					   TaxTypeId = item.TaxTypeId,
                       ItemTypeId = item.ItemTypeId,
					   ItemPackageId = stockOutDetail.ItemPackageId,
					   ItemPackageName = language == LanguageCode.Arabic
						   ? itemPackage.PackageNameAr
						   : itemPackage.PackageNameEn,
					   IsItemVatInclusive = stockOutDetail.IsItemVatInclusive,
					   BarCode = stockOutDetail.BarCode,
					   Packing = stockOutDetail.Packing,
					   ExpireDate = stockOutDetail.ExpireDate,
					   BatchNumber = stockOutDetail.BatchNumber,
					   Quantity = stockOutDetail.Quantity,
					   BonusQuantity = stockOutDetail.BonusQuantity,
					   /*Other quantities computed in handling service*/
					   SellingPrice = stockOutDetail.SellingPrice,
					   TotalValue = stockOutDetail.TotalValue,
					   ItemDiscountPercent = stockOutDetail.ItemDiscountPercent,
					   ItemDiscountValue = stockOutDetail.ItemDiscountValue,
					   TotalValueAfterDiscount = stockOutDetail.TotalValueAfterDiscount,
					   HeaderDiscountValue = stockOutDetail.HeaderDiscountValue,
					   GrossValue = stockOutDetail.GrossValue,
					   VatPercent = stockOutDetail.VatPercent,
					   VatValue = stockOutDetail.VatValue,
					   SubNetValue = stockOutDetail.SubNetValue,
					   OtherTaxValue = stockOutDetail.OtherTaxValue,
					   NetValue = stockOutDetail.NetValue,
					   Notes = stockOutDetail.Notes,
                       ItemNote = stockOutDetail.ItemNote,
					   ConsumerPrice = stockOutDetail.ConsumerPrice,
					   CostPrice = stockOutDetail.CostPrice,
					   CostPackage = stockOutDetail.CostPackage,
					   CostValue = stockOutDetail.CostValue,
					   LastSalesPrice = stockOutDetail.LastSalesPrice,

					   CreatedAt = stockOutDetail.CreatedAt,
					   IpAddressCreated = stockOutDetail.IpAddressCreated,
					   UserNameCreated = stockOutDetail.UserNameCreated,
				   };
		}

		public async Task<List<StockOutDetailDto>> GetStockOutDetails(int stockOutHeaderId)
        {
            var modelList = new List<StockOutDetailDto>();

            var data = await GetStockOutDetailsAsQueryable(stockOutHeaderId).ToListAsync();

            var itemIds = data.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var stockOutDetail in data)
            {
                var model = new StockOutDetailDto();
                model = stockOutDetail;
                model.Packages = packages.Where(x => x.ItemId == stockOutDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                model.ItemTaxData = itemTaxData.Where(x => x.ItemId == stockOutDetail.ItemId).ToList();
                model.Taxes = model.ItemTaxData.ToJson();
                modelList.Add(model);
            }
            return modelList;
		}

		public async Task<List<StockOutDetailDto>> GetStockOutDetailsGrouped(int stockOutHeaderId)
		{
			var details = await GetStockOutDetailsAsQueryable(stockOutHeaderId).ToListAsync();
			return GroupStockOutDetails(details);
		}

		public async Task<List<StockOutDetailDto>> GetStockOutDetailsGroupedWithAllKeys(int stockOutHeaderId)
		{
			var details = await GetStockOutDetailsAsQueryable(stockOutHeaderId).ToListAsync();
			return GroupStockOutDetailsWithAllKeys(details);
		}

		public List<StockOutDetailDto> GroupStockOutDetails(List<StockOutDetailDto> details)
		{
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(
				x => new StockOutDetailDto
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

		public List<StockOutDetailDto> GroupStockOutDetailsWithAllKeys(List<StockOutDetailDto> details)
		{
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber, x.CostCenterId, x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(
				x => new StockOutDetailDto
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

		public async Task<List<StockOutDetailDto>> SaveStockOutDetails(int stockOutHeaderId, List<StockOutDetailDto> stockOutDetails)
        {
            if (stockOutDetails.Any())
            {
                await EditStockOutDetail(stockOutDetails);
                return await AddStockOutDetail(stockOutDetails, stockOutHeaderId);
            }
            return stockOutDetails;
        }
        public async Task<bool> DeleteStockOutDetails(int stockOutHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteStockOutDetailList(List<StockOutDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.StockOutHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.StockOutDetailId != p.StockOutDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<List<StockOutDetailDto>> AddStockOutDetail(List<StockOutDetailDto> details, int headerId)
        {
            var current = details.Where(x => x.StockOutDetailId <= 0).ToList();
            var modelList = new List<StockOutDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new StockOutDetail()
                {
                    StockOutDetailId = newId,
                    StockOutHeaderId = headerId,
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

                detail.StockOutDetailId = newId;
                detail.StockOutHeaderId = headerId;

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

        private async Task<bool> EditStockOutDetail(List<StockOutDetailDto> stockOutDetails)
        {
            var current = stockOutDetails.Where(x => x.StockOutDetailId > 0).ToList();
            var modelList = new List<StockOutDetail>();
            foreach (var detail in current)
            {
                var model = new StockOutDetail()
                {
                    StockOutDetailId = detail.StockOutDetailId,
                    StockOutHeaderId = detail.StockOutHeaderId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.StockOutDetailId) + 1; } catch { id = 1; }
            return id;
        }
    }
}
