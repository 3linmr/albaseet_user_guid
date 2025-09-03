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
    public class PurchaseInvoiceDetailService : BaseService<PurchaseInvoiceDetail>, IPurchaseInvoiceDetailService
    {
        private readonly IItemPackageService _itemPackageService;
        private readonly IItemService _itemService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemTaxService _itemTaxService;

        public PurchaseInvoiceDetailService(IRepository<PurchaseInvoiceDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService, IHttpContextAccessor httpContextAccessor, IItemTaxService itemTaxService) : base(repository)
        {
            _itemPackageService = itemPackageService;
            _itemService = itemService;
            _itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
            _httpContextAccessor = httpContextAccessor;
            _itemTaxService = itemTaxService;
        }

        public async Task<List<PurchaseInvoiceDetailDto>> GetPurchaseInvoiceDetails(int purchaseInvoiceHeaderId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var purchaseInvoiceDetails =
                await (from purchaseInvoiceDetail in _repository.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId)
                       from item in _itemService.GetAll().Where(x => x.ItemId == purchaseInvoiceDetail.ItemId)
                       from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId)
                       from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == purchaseInvoiceDetail.CostCenterId).DefaultIfEmpty()
                       select new PurchaseInvoiceDetailDto
                       {
                           PurchaseInvoiceDetailId = purchaseInvoiceDetail.PurchaseInvoiceDetailId,
                           PurchaseInvoiceHeaderId = purchaseInvoiceDetail.PurchaseInvoiceHeaderId,
                           CostCenterId = purchaseInvoiceDetail.CostCenterId,
                           CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                           ItemId = purchaseInvoiceDetail.ItemId,
                           ItemCode = item.ItemCode,
                           ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                           ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? purchaseInvoiceDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
                           TaxTypeId = item.TaxTypeId,
                           ItemTypeId = item.ItemTypeId,
                           ItemPackageId = purchaseInvoiceDetail.ItemPackageId,
                           ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                           IsItemVatInclusive = purchaseInvoiceDetail.IsItemVatInclusive,
                           BarCode = purchaseInvoiceDetail.BarCode,
                           Packing = purchaseInvoiceDetail.Packing,
                           ExpireDate = purchaseInvoiceDetail.ExpireDate,
                           BatchNumber = purchaseInvoiceDetail.BatchNumber,
                           Quantity = purchaseInvoiceDetail.Quantity,
                           BonusQuantity = purchaseInvoiceDetail.BonusQuantity,
                           PurchasePrice = purchaseInvoiceDetail.PurchasePrice,
                           TotalValue = purchaseInvoiceDetail.TotalValue,
                           ItemDiscountPercent = purchaseInvoiceDetail.ItemDiscountPercent,
                           ItemDiscountValue = purchaseInvoiceDetail.ItemDiscountValue,
                           TotalValueAfterDiscount = purchaseInvoiceDetail.TotalValueAfterDiscount,
                           HeaderDiscountValue = purchaseInvoiceDetail.HeaderDiscountValue,
                           GrossValue = purchaseInvoiceDetail.GrossValue,
                           VatPercent = purchaseInvoiceDetail.VatPercent,
                           VatValue = purchaseInvoiceDetail.VatValue,
                           SubNetValue = purchaseInvoiceDetail.SubNetValue,
                           OtherTaxValue = purchaseInvoiceDetail.OtherTaxValue,
                           NetValue = purchaseInvoiceDetail.NetValue,
                           ItemExpensePercent = purchaseInvoiceDetail.ItemExpensePercent,
                           ItemExpenseValue = purchaseInvoiceDetail.ItemExpenseValue,
                           Notes = purchaseInvoiceDetail.Notes,
                           ItemNote = purchaseInvoiceDetail.ItemNote,
                           VatTaxTypeId = purchaseInvoiceDetail.VatTaxTypeId,
                           VatTaxId = purchaseInvoiceDetail.VatTaxId,
                           ConsumerPrice = purchaseInvoiceDetail.ConsumerPrice,
                           CostPrice = purchaseInvoiceDetail.CostPrice,
                           CostPackage = purchaseInvoiceDetail.CostPackage,
                           CostValue = purchaseInvoiceDetail.CostValue,
                           LastPurchasePrice = purchaseInvoiceDetail.LastPurchasePrice,
                           CreatedAt = purchaseInvoiceDetail.CreatedAt,
                           IpAddressCreated = purchaseInvoiceDetail.IpAddressCreated,
                           UserNameCreated = purchaseInvoiceDetail.UserNameCreated,
                       }).ToListAsync();

            var itemIds = purchaseInvoiceDetails.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var purchaseInvoiceDetail in purchaseInvoiceDetails)
            {
                purchaseInvoiceDetail.Packages = packages.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                purchaseInvoiceDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId).ToList();
                purchaseInvoiceDetail.Taxes = purchaseInvoiceDetail.ItemTaxData.ToJson();
            }

            return purchaseInvoiceDetails;
        }

        public IQueryable<PurchaseInvoiceDetailDto> GetPurchaseInvoiceDetailsAsQueryable(int purchaseInvoiceHeaderId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data =
                   (from purchaseInvoiceDetail in _repository.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId)
                    from item in _itemService.GetAll().Where(x => x.ItemId == purchaseInvoiceDetail.ItemId)
                    from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId)
                    from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == purchaseInvoiceDetail.CostCenterId).DefaultIfEmpty()
                    select new PurchaseInvoiceDetailDto
                    {
                        PurchaseInvoiceDetailId = purchaseInvoiceDetail.PurchaseInvoiceDetailId,
                        PurchaseInvoiceHeaderId = purchaseInvoiceDetail.PurchaseInvoiceHeaderId,
                        CostCenterId = purchaseInvoiceDetail.CostCenterId,
                        CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                        ItemId = purchaseInvoiceDetail.ItemId,
                        ItemCode = item.ItemCode,
                        ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                        ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? purchaseInvoiceDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
                        TaxTypeId = item.TaxTypeId,
                        ItemTypeId = item.ItemTypeId,
                        ItemPackageId = purchaseInvoiceDetail.ItemPackageId,
                        ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                        IsItemVatInclusive = purchaseInvoiceDetail.IsItemVatInclusive,
                        BarCode = purchaseInvoiceDetail.BarCode,
                        Packing = purchaseInvoiceDetail.Packing,
                        ExpireDate = purchaseInvoiceDetail.ExpireDate,
                        BatchNumber = purchaseInvoiceDetail.BatchNumber,
                        Quantity = purchaseInvoiceDetail.Quantity,
                        BonusQuantity = purchaseInvoiceDetail.BonusQuantity,
                        PurchasePrice = purchaseInvoiceDetail.PurchasePrice,
                        TotalValue = purchaseInvoiceDetail.TotalValue,
                        ItemDiscountPercent = purchaseInvoiceDetail.ItemDiscountPercent,
                        ItemDiscountValue = purchaseInvoiceDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = purchaseInvoiceDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = purchaseInvoiceDetail.HeaderDiscountValue,
                        GrossValue = purchaseInvoiceDetail.GrossValue,
                        VatPercent = purchaseInvoiceDetail.VatPercent,
                        VatValue = purchaseInvoiceDetail.VatValue,
                        SubNetValue = purchaseInvoiceDetail.SubNetValue,
                        OtherTaxValue = purchaseInvoiceDetail.OtherTaxValue,
                        NetValue = purchaseInvoiceDetail.NetValue,
                        ItemExpensePercent = purchaseInvoiceDetail.ItemExpensePercent,
                        ItemExpenseValue = purchaseInvoiceDetail.ItemExpenseValue,
                        Notes = purchaseInvoiceDetail.Notes,
                        ItemNote = purchaseInvoiceDetail.ItemNote,
                        VatTaxTypeId = purchaseInvoiceDetail.VatTaxTypeId,
                        VatTaxId = purchaseInvoiceDetail.VatTaxId,
                        ConsumerPrice = purchaseInvoiceDetail.ConsumerPrice,
                        CostPrice = purchaseInvoiceDetail.CostPrice,
                        CostPackage = purchaseInvoiceDetail.CostPackage,
                        CostValue = purchaseInvoiceDetail.CostValue,
                        LastPurchasePrice = purchaseInvoiceDetail.LastPurchasePrice,
                        CreatedAt = purchaseInvoiceDetail.CreatedAt,
                        IpAddressCreated = purchaseInvoiceDetail.IpAddressCreated,
                        UserNameCreated = purchaseInvoiceDetail.UserNameCreated,
                    });

            return data;
		}

		public async Task<List<PurchaseInvoiceDetailDto>> GetPurchaseInvoiceDetailsGrouped(int purchaseInvoiceHeaderId)
		{
			var details = await GetPurchaseInvoiceDetailsAsQueryable(purchaseInvoiceHeaderId).ToListAsync();
			return GroupPurchaseInvoiceDetails(details);
		}

		public IQueryable<PurchaseInvoiceDetailDto> GetPurchaseInvoiceDetailsGroupedQueryable(int purchaseInvoiceHeaderId)
		{
            return _repository.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber, x.CostCenterId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent }).Select(
				x => new PurchaseInvoiceDetailDto
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
				});
		}

		public List<PurchaseInvoiceDetailDto> GroupPurchaseInvoiceDetails(List<PurchaseInvoiceDetailDto> details)
		{
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber, x.CostCenterId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent }).Select(
				x => new PurchaseInvoiceDetailDto
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

		public List<PurchaseInvoiceDetailDto> GroupPurchaseInvoiceDetailsWithoutExpireAndBatch(List<PurchaseInvoiceDetailDto> details)
		{
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.CostCenterId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent }).Select(
				x => new PurchaseInvoiceDetailDto
				{
					ItemId = x.Key.ItemId,
					ItemPackageId = x.Key.ItemPackageId,
					CostCenterId = x.Key.CostCenterId,
					BarCode = x.Key.BarCode,
					PurchasePrice = x.Key.PurchasePrice,
					ItemDiscountPercent = x.Key.ItemDiscountPercent,
					Quantity = x.Sum(y => y.Quantity),
					BonusQuantity = x.Sum(y => y.BonusQuantity)
				}).ToList();
		}

		public async Task<List<PurchaseInvoiceDetailDto>> SavePurchaseInvoiceDetails(int purchaseInvoiceHeaderId, List<PurchaseInvoiceDetailDto> purchaseInvoiceDetails)
        {
            if (purchaseInvoiceDetails.Any())
            {
                await EditPurchaseInvoiceDetail(purchaseInvoiceDetails);
                return await AddPurchaseInvoiceDetail(purchaseInvoiceDetails, purchaseInvoiceHeaderId);
            }
            return purchaseInvoiceDetails;
        }

        public async Task<bool> DeletePurchaseInvoiceDetails(int purchaseInvoiceHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeletePurchaseInvoiceDetailList(List<PurchaseInvoiceDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.PurchaseInvoiceHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.PurchaseInvoiceDetailId != p.PurchaseInvoiceDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<List<PurchaseInvoiceDetailDto>> AddPurchaseInvoiceDetail(List<PurchaseInvoiceDetailDto> details, int headerId)
        {
            var current = details.Where(x => x.PurchaseInvoiceDetailId <= 0).ToList();
            var modelList = new List<PurchaseInvoiceDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new PurchaseInvoiceDetail()
                {
                    PurchaseInvoiceDetailId = newId,
                    PurchaseInvoiceHeaderId = headerId,
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
                    ItemExpensePercent = detail.ItemExpensePercent,
                    ItemExpenseValue = detail.ItemExpenseValue,
                    NetValue = detail.NetValue,
                    Notes = detail.Notes,
                    ItemNote = detail.ItemNote,
                    VatTaxId = detail.VatTaxId,
                    VatTaxTypeId = detail.VatTaxTypeId,
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

                detail.PurchaseInvoiceDetailId = newId;
                detail.PurchaseInvoiceHeaderId = headerId;

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

        private async Task<bool> EditPurchaseInvoiceDetail(List<PurchaseInvoiceDetailDto> purchaseInvoiceDetails)
        {
            var current = purchaseInvoiceDetails.Where(x => x.PurchaseInvoiceDetailId > 0).ToList();
            var modelList = new List<PurchaseInvoiceDetail>();
            foreach (var detail in current)
            {
                var model = new PurchaseInvoiceDetail()
                {
                    PurchaseInvoiceDetailId = detail.PurchaseInvoiceDetailId,
                    PurchaseInvoiceHeaderId = detail.PurchaseInvoiceHeaderId,
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
                    ItemExpensePercent = detail.ItemExpensePercent,
                    ItemExpenseValue = detail.ItemExpenseValue,
                    NetValue = detail.NetValue,
                    Notes = detail.Notes,
                    ItemNote = detail.ItemNote,
                    VatTaxId = detail.VatTaxId,
                    VatTaxTypeId = detail.VatTaxTypeId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.PurchaseInvoiceDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
