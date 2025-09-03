using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Items;
using Shared.Helper.Extensions;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.StaticData;

namespace Sales.Service.Services
{
    public class SalesInvoiceDetailService : BaseService<SalesInvoiceDetail>, ISalesInvoiceDetailService
    {
        private readonly IItemPackageService _itemPackageService;
        private readonly IItemService _itemService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemTaxService _itemTaxService;

        public SalesInvoiceDetailService(IRepository<SalesInvoiceDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService, IHttpContextAccessor httpContextAccessor, IItemTaxService itemTaxService) : base(repository)
        {
            _itemPackageService = itemPackageService;
            _itemService = itemService;
            _itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
            _httpContextAccessor = httpContextAccessor;
            _itemTaxService = itemTaxService;
        }

		public IQueryable<SalesInvoiceDetailDto> GetSalesInvoiceDetailsAsQueryable(int salesInvoiceHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return from salesInvoiceDetail in _repository.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId)
				   from item in _itemService.GetAll().Where(x => x.ItemId == salesInvoiceDetail.ItemId)
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == salesInvoiceDetail.ItemPackageId)
				   from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == salesInvoiceDetail.CostCenterId).DefaultIfEmpty()
				   select new SalesInvoiceDetailDto
				   {
					   SalesInvoiceDetailId = salesInvoiceDetail.SalesInvoiceDetailId,
					   SalesInvoiceHeaderId = salesInvoiceDetail.SalesInvoiceHeaderId,
					   ItemId = salesInvoiceDetail.ItemId,
					   ItemCode = item.ItemCode,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					   ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? salesInvoiceDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
                       ItemTypeId = item.ItemTypeId,
					   TaxTypeId = item.TaxTypeId,
					   ItemPackageId = salesInvoiceDetail.ItemPackageId,
					   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					   IsItemVatInclusive = salesInvoiceDetail.IsItemVatInclusive,
					   CostCenterId = salesInvoiceDetail.CostCenterId,
					   CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
					   BarCode = salesInvoiceDetail.BarCode,
					   Packing = salesInvoiceDetail.Packing,
					   ExpireDate = salesInvoiceDetail.ExpireDate,
					   BatchNumber = salesInvoiceDetail.BatchNumber,
					   Quantity = salesInvoiceDetail.Quantity,
					   BonusQuantity = salesInvoiceDetail.BonusQuantity,
					   SellingPrice = salesInvoiceDetail.SellingPrice,
					   TotalValue = salesInvoiceDetail.TotalValue,
					   ItemDiscountPercent = salesInvoiceDetail.ItemDiscountPercent,
					   ItemDiscountValue = salesInvoiceDetail.ItemDiscountValue,
					   TotalValueAfterDiscount = salesInvoiceDetail.TotalValueAfterDiscount,
					   HeaderDiscountValue = salesInvoiceDetail.HeaderDiscountValue,
					   GrossValue = salesInvoiceDetail.GrossValue,
					   VatPercent = salesInvoiceDetail.VatPercent,
					   VatValue = salesInvoiceDetail.VatValue,
					   SubNetValue = salesInvoiceDetail.SubNetValue,
					   OtherTaxValue = salesInvoiceDetail.OtherTaxValue,
					   NetValue = salesInvoiceDetail.NetValue,
					   Notes = salesInvoiceDetail.Notes,
                       ItemNote = salesInvoiceDetail.ItemNote,
                       VatTaxTypeId = salesInvoiceDetail.VatTaxTypeId,
                       VatTaxId = salesInvoiceDetail.VatTaxId,
					   ConsumerPrice = salesInvoiceDetail.ConsumerPrice,
					   CostPrice = salesInvoiceDetail.CostPrice,
					   CostPackage = salesInvoiceDetail.CostPackage,
					   CostValue = salesInvoiceDetail.CostValue,
					   LastSalesPrice = salesInvoiceDetail.LastSalesPrice,

					   CreatedAt = salesInvoiceDetail.CreatedAt,
					   IpAddressCreated = salesInvoiceDetail.IpAddressCreated,
					   UserNameCreated = salesInvoiceDetail.UserNameCreated,
				   };
		}

        public async Task<List<SalesInvoiceDetailDto>> GetSalesInvoiceDetails(int salesInvoiceHeaderId)
        {

            var modelList = new List<SalesInvoiceDetailDto>();

            var data = await GetSalesInvoiceDetailsAsQueryable(salesInvoiceHeaderId).ToListAsync();

            var itemIds = data.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var salesInvoiceDetail in data)
            {
                var model = new SalesInvoiceDetailDto();
                model = salesInvoiceDetail;
                model.Packages = packages.Where(x => x.ItemId == salesInvoiceDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                model.ItemTaxData = itemTaxData.Where(x => x.ItemId == salesInvoiceDetail.ItemId).ToList();
                model.Taxes = model.ItemTaxData.ToJson();
                modelList.Add(model);
            }
            return modelList;
		}

		public async Task<List<SalesInvoiceDetailDto>> GetSalesInvoiceDetailsGrouped(int salesInvoiceHeaderId)
		{
			var details = await GetSalesInvoiceDetailsAsQueryable(salesInvoiceHeaderId).ToListAsync();
			return GroupSalesInvoiceDetails(details);
		}

		public IQueryable<SalesInvoiceDetailDto> GetSalesInvoiceDetailsGroupedQueryable(int salesInvoiceHeaderId)
		{
			return _repository.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber, x.CostCenterId, x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(
				x => new SalesInvoiceDetailDto
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
				});
		}

		public List<SalesInvoiceDetailDto> GroupSalesInvoiceDetails(List<SalesInvoiceDetailDto> details)
		{
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber, x.CostCenterId, x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(
				x => new SalesInvoiceDetailDto
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

		public List<SalesInvoiceDetailDto> GroupSalesInvoiceDetailsWithoutExpireAndBatch(List<SalesInvoiceDetailDto> details)
		{
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.CostCenterId, x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(
				x => new SalesInvoiceDetailDto
				{
					ItemId = x.Key.ItemId,
					ItemPackageId = x.Key.ItemPackageId,
					CostCenterId = x.Key.CostCenterId,
					BarCode = x.Key.BarCode,
					SellingPrice = x.Key.SellingPrice,
					ItemDiscountPercent = x.Key.ItemDiscountPercent,
					Quantity = x.Sum(y => y.Quantity),
					BonusQuantity = x.Sum(y => y.BonusQuantity)
				}).ToList();
		}

		public async Task<List<SalesInvoiceDetailDto>> SaveSalesInvoiceDetails(int salesInvoiceHeaderId, List<SalesInvoiceDetailDto> salesInvoiceDetails)
        {
            if (salesInvoiceDetails.Any())
            {
                await EditSalesInvoiceDetail(salesInvoiceDetails);
                return await AddSalesInvoiceDetail(salesInvoiceDetails, salesInvoiceHeaderId);
            }
            return salesInvoiceDetails;
        }
        public async Task<bool> DeleteSalesInvoiceDetails(int salesInvoiceHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteSalesInvoiceDetailList(List<SalesInvoiceDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.SalesInvoiceHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.SalesInvoiceDetailId != p.SalesInvoiceDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<List<SalesInvoiceDetailDto>> AddSalesInvoiceDetail(List<SalesInvoiceDetailDto> details, int headerId)
        {
            var current = details.Where(x => x.SalesInvoiceDetailId <= 0).ToList();
            var modelList = new List<SalesInvoiceDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new SalesInvoiceDetail()
                {
                    SalesInvoiceDetailId = newId,
                    SalesInvoiceHeaderId = headerId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    CostCenterId = detail.CostCenterId,
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
                    VatTaxTypeId = detail.VatTaxTypeId,
                    VatTaxId = detail.VatTaxId,
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

                detail.SalesInvoiceDetailId = newId;
                detail.SalesInvoiceHeaderId = headerId;

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

        private async Task<bool> EditSalesInvoiceDetail(List<SalesInvoiceDetailDto> salesInvoiceDetails)
        {
            var current = salesInvoiceDetails.Where(x => x.SalesInvoiceDetailId > 0).ToList();
            var modelList = new List<SalesInvoiceDetail>();
            foreach (var detail in current)
            {
                var model = new SalesInvoiceDetail()
                {
                    SalesInvoiceDetailId = detail.SalesInvoiceDetailId,
                    SalesInvoiceHeaderId = detail.SalesInvoiceHeaderId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    CostCenterId = detail.CostCenterId,
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
                    VatTaxTypeId = detail.VatTaxTypeId,
                    VatTaxId = detail.VatTaxId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.SalesInvoiceDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
