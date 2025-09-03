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
using Shared.CoreOne.Models.StaticData;

namespace Sales.Service.Services
{
    public class SalesInvoiceReturnDetailService : BaseService<SalesInvoiceReturnDetail>, ISalesInvoiceReturnDetailService
    {
        private readonly IItemPackageService _itemPackageService;
        private readonly IItemService _itemService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemTaxService _itemTaxService;

        public SalesInvoiceReturnDetailService(IRepository<SalesInvoiceReturnDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService, IHttpContextAccessor httpContextAccessor, IItemTaxService itemTaxService) : base(repository)
        {
            _itemPackageService = itemPackageService;
            _itemService = itemService;
            _itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
            _httpContextAccessor = httpContextAccessor;
            _itemTaxService = itemTaxService;
        }

		public IQueryable<SalesInvoiceReturnDetailDto> GetSalesInvoiceReturnDetailsAsQueryable(int salesInvoiceReturnHeaderId)
        {
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

            return from salesInvoiceReturnDetail in _repository.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeaderId)
                   from item in _itemService.GetAll().Where(x => x.ItemId == salesInvoiceReturnDetail.ItemId)
                   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == salesInvoiceReturnDetail.ItemPackageId)
                   from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == salesInvoiceReturnDetail.CostCenterId).DefaultIfEmpty()
                   select new SalesInvoiceReturnDetailDto
                   {
                       SalesInvoiceReturnDetailId = salesInvoiceReturnDetail.SalesInvoiceReturnDetailId,
                       SalesInvoiceReturnHeaderId = salesInvoiceReturnDetail.SalesInvoiceReturnHeaderId,
                       ItemId = salesInvoiceReturnDetail.ItemId,
                       ItemCode = item.ItemCode,
                       ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                       ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? salesInvoiceReturnDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
                       TaxTypeId = item.TaxTypeId,
                       ItemTypeId = item.ItemTypeId,
                       ItemPackageId = salesInvoiceReturnDetail.ItemPackageId,
                       ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                       IsItemVatInclusive = salesInvoiceReturnDetail.IsItemVatInclusive,
                       CostCenterId = salesInvoiceReturnDetail.CostCenterId,
                       CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                       BarCode = salesInvoiceReturnDetail.BarCode,
                       Packing = salesInvoiceReturnDetail.Packing,
                       ExpireDate = salesInvoiceReturnDetail.ExpireDate,
                       BatchNumber = salesInvoiceReturnDetail.BatchNumber,
                       Quantity = salesInvoiceReturnDetail.Quantity,
                       BonusQuantity = salesInvoiceReturnDetail.BonusQuantity,
                       SellingPrice = salesInvoiceReturnDetail.SellingPrice,
                       TotalValue = salesInvoiceReturnDetail.TotalValue,
                       ItemDiscountPercent = salesInvoiceReturnDetail.ItemDiscountPercent,
                       ItemDiscountValue = salesInvoiceReturnDetail.ItemDiscountValue,
                       TotalValueAfterDiscount = salesInvoiceReturnDetail.TotalValueAfterDiscount,
                       HeaderDiscountValue = salesInvoiceReturnDetail.HeaderDiscountValue,
                       GrossValue = salesInvoiceReturnDetail.GrossValue,
                       VatPercent = salesInvoiceReturnDetail.VatPercent,
                       VatValue = salesInvoiceReturnDetail.VatValue,
                       SubNetValue = salesInvoiceReturnDetail.SubNetValue,
                       OtherTaxValue = salesInvoiceReturnDetail.OtherTaxValue,
                       NetValue = salesInvoiceReturnDetail.NetValue,
                       Notes = salesInvoiceReturnDetail.Notes,
                       ItemNote = salesInvoiceReturnDetail.ItemNote,
                       VatTaxTypeId = salesInvoiceReturnDetail.VatTaxTypeId,
                       VatTaxId = salesInvoiceReturnDetail.VatTaxId,
                       ConsumerPrice = salesInvoiceReturnDetail.ConsumerPrice,
                       CostPrice = salesInvoiceReturnDetail.CostPrice,
                       CostPackage = salesInvoiceReturnDetail.CostPackage,
                       CostValue = salesInvoiceReturnDetail.CostValue,
                       LastSalesPrice = salesInvoiceReturnDetail.LastSalesPrice,

                       CreatedAt = salesInvoiceReturnDetail.CreatedAt,
                       IpAddressCreated = salesInvoiceReturnDetail.IpAddressCreated,
                       UserNameCreated = salesInvoiceReturnDetail.UserNameCreated,
                   };
		}

		public async Task<List<SalesInvoiceReturnDetailDto>> GetSalesInvoiceReturnDetails(int salesInvoiceReturnHeaderId)
        {

            var modelList = new List<SalesInvoiceReturnDetailDto>();

            var data = await GetSalesInvoiceReturnDetailsAsQueryable(salesInvoiceReturnHeaderId).ToListAsync();

            var itemIds = data.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var salesInvoiceReturnDetail in data)
            {
                var model = new SalesInvoiceReturnDetailDto();
                model = salesInvoiceReturnDetail;
                model.Packages = packages.Where(x => x.ItemId == salesInvoiceReturnDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                model.ItemTaxData = itemTaxData.Where(x => x.ItemId == salesInvoiceReturnDetail.ItemId).ToList();
                model.Taxes = model.ItemTaxData.ToJson();
                modelList.Add(model);
            }
            return modelList;
		}

		public List<SalesInvoiceReturnDetailDto> GroupSalesInvoiceReturnDetails(List<SalesInvoiceReturnDetailDto> details)
		{
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber, x.CostCenterId, x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(
				x => new SalesInvoiceReturnDetailDto
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

		public async Task<List<SalesInvoiceReturnDetailDto>> SaveSalesInvoiceReturnDetails(int salesInvoiceReturnHeaderId, List<SalesInvoiceReturnDetailDto> salesInvoiceReturnDetails)
        {
            if (salesInvoiceReturnDetails.Any())
            {
                await EditSalesInvoiceReturnDetail(salesInvoiceReturnDetails);
                return await AddSalesInvoiceReturnDetail(salesInvoiceReturnDetails, salesInvoiceReturnHeaderId);
            }
            return salesInvoiceReturnDetails;
        }

        public async Task<bool> DeleteSalesInvoiceReturnDetails(int salesInvoiceReturnHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteSalesInvoiceReturnDetailList(List<SalesInvoiceReturnDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.SalesInvoiceReturnDetailId != p.SalesInvoiceReturnDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<List<SalesInvoiceReturnDetailDto>> AddSalesInvoiceReturnDetail(List<SalesInvoiceReturnDetailDto> details, int headerId)
        {
            var current = details.Where(x => x.SalesInvoiceReturnDetailId <= 0).ToList();
            var modelList = new List<SalesInvoiceReturnDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new SalesInvoiceReturnDetail()
                {
                    SalesInvoiceReturnDetailId = newId,
                    SalesInvoiceReturnHeaderId = headerId,
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

                detail.SalesInvoiceReturnDetailId = newId;
                detail.SalesInvoiceReturnHeaderId = headerId;

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

        private async Task<bool> EditSalesInvoiceReturnDetail(List<SalesInvoiceReturnDetailDto> salesInvoiceReturnDetails)
        {
            var current = salesInvoiceReturnDetails.Where(x => x.SalesInvoiceReturnDetailId > 0).ToList();
            var modelList = new List<SalesInvoiceReturnDetail>();
            foreach (var detail in current)
            {
                var model = new SalesInvoiceReturnDetail()
                {
                    SalesInvoiceReturnDetailId = detail.SalesInvoiceReturnDetailId,
                    SalesInvoiceReturnHeaderId = detail.SalesInvoiceReturnHeaderId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.SalesInvoiceReturnDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
