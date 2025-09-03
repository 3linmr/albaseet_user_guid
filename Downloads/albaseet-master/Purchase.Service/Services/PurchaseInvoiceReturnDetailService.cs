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
    public class PurchaseInvoiceReturnDetailService : BaseService<PurchaseInvoiceReturnDetail>, IPurchaseInvoiceReturnDetailService
    {
        private readonly IItemPackageService _itemPackageService;
        private readonly IItemService _itemService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemTaxService _itemTaxService;

        public PurchaseInvoiceReturnDetailService(IRepository<PurchaseInvoiceReturnDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService, IHttpContextAccessor httpContextAccessor, IItemTaxService itemTaxService) : base(repository)
        {
            _itemPackageService = itemPackageService;
            _itemService = itemService;
            _itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
            _httpContextAccessor = httpContextAccessor;
            _itemTaxService = itemTaxService;
        }

        public IQueryable<PurchaseInvoiceReturnDetailDto> GetPurchaseInvoiceReturnDetailsAsQueryable(int purchaseInvoiceReturnHeaderId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return from purchaseInvoiceReturnDetail in _repository.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeaderId)
                   from item in _itemService.GetAll().Where(x => x.ItemId == purchaseInvoiceReturnDetail.ItemId)
                   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == purchaseInvoiceReturnDetail.ItemPackageId)
                   from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == purchaseInvoiceReturnDetail.CostCenterId).DefaultIfEmpty()
                   select new PurchaseInvoiceReturnDetailDto
                   {
                       PurchaseInvoiceReturnDetailId = purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailId,
                       PurchaseInvoiceReturnHeaderId = purchaseInvoiceReturnDetail.PurchaseInvoiceReturnHeaderId,
                       CostCenterId = purchaseInvoiceReturnDetail.CostCenterId,
                       CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                       ItemId = purchaseInvoiceReturnDetail.ItemId,
                       ItemCode = item.ItemCode,
                       ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                       ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? purchaseInvoiceReturnDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
                       TaxTypeId = item.TaxTypeId,
                       ItemTypeId = item.ItemTypeId,
                       ItemPackageId = purchaseInvoiceReturnDetail.ItemPackageId,
                       ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                       IsItemVatInclusive = purchaseInvoiceReturnDetail.IsItemVatInclusive,
                       BarCode = purchaseInvoiceReturnDetail.BarCode,
                       Packing = purchaseInvoiceReturnDetail.Packing,
                       ExpireDate = purchaseInvoiceReturnDetail.ExpireDate,
                       BatchNumber = purchaseInvoiceReturnDetail.BatchNumber,
                       Quantity = purchaseInvoiceReturnDetail.Quantity,
                       BonusQuantity = purchaseInvoiceReturnDetail.BonusQuantity,
                       PurchasePrice = purchaseInvoiceReturnDetail.PurchasePrice,
                       TotalValue = purchaseInvoiceReturnDetail.TotalValue,
                       ItemDiscountPercent = purchaseInvoiceReturnDetail.ItemDiscountPercent,
                       ItemDiscountValue = purchaseInvoiceReturnDetail.ItemDiscountValue,
                       TotalValueAfterDiscount = purchaseInvoiceReturnDetail.TotalValueAfterDiscount,
                       HeaderDiscountValue = purchaseInvoiceReturnDetail.HeaderDiscountValue,
                       GrossValue = purchaseInvoiceReturnDetail.GrossValue,
                       VatPercent = purchaseInvoiceReturnDetail.VatPercent,
                       VatValue = purchaseInvoiceReturnDetail.VatValue,
                       SubNetValue = purchaseInvoiceReturnDetail.SubNetValue,
                       OtherTaxValue = purchaseInvoiceReturnDetail.OtherTaxValue,
                       NetValue = purchaseInvoiceReturnDetail.NetValue,
                       Notes = purchaseInvoiceReturnDetail.Notes,
                       ItemNote = purchaseInvoiceReturnDetail.ItemNote,
                       VatTaxId = purchaseInvoiceReturnDetail.VatTaxId,
                       VatTaxTypeId = purchaseInvoiceReturnDetail.VatTaxTypeId,
                       ConsumerPrice = purchaseInvoiceReturnDetail.ConsumerPrice,
                       CostPrice = purchaseInvoiceReturnDetail.CostPrice,
                       CostPackage = purchaseInvoiceReturnDetail.CostPackage,
                       CostValue = purchaseInvoiceReturnDetail.CostValue,
                       LastPurchasePrice = purchaseInvoiceReturnDetail.LastPurchasePrice,
                       CreatedAt = purchaseInvoiceReturnDetail.CreatedAt,
                       IpAddressCreated = purchaseInvoiceReturnDetail.IpAddressCreated,
                       UserNameCreated = purchaseInvoiceReturnDetail.UserNameCreated,
                   };
		}

        public async Task<List<PurchaseInvoiceReturnDetailDto>> GetPurchaseInvoiceReturnDetails(int purchaseInvoiceReturnHeaderId)
        {
            var purchaseInvoiceReturnDetails = await GetPurchaseInvoiceReturnDetailsAsQueryable(purchaseInvoiceReturnHeaderId).ToListAsync();

            var itemIds = purchaseInvoiceReturnDetails.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var purchaseInvoiceReturnDetail in purchaseInvoiceReturnDetails)
            {
                purchaseInvoiceReturnDetail.Packages = packages.Where(x => x.ItemId == purchaseInvoiceReturnDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                purchaseInvoiceReturnDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == purchaseInvoiceReturnDetail.ItemId).ToList();
                purchaseInvoiceReturnDetail.Taxes = purchaseInvoiceReturnDetail.ItemTaxData.ToJson();
            }

            return purchaseInvoiceReturnDetails;
		}

		public List<PurchaseInvoiceReturnDetailDto> GroupPurchaseInvoiceReturnDetails(List<PurchaseInvoiceReturnDetailDto> details)
		{
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber, x.CostCenterId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent }).Select(
				x => new PurchaseInvoiceReturnDetailDto
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

		public async Task<List<PurchaseInvoiceReturnDetailDto>> SavePurchaseInvoiceReturnDetails(int purchaseInvoiceReturnHeaderId, List<PurchaseInvoiceReturnDetailDto> purchaseInvoiceReturnDetails)
        {
            if (purchaseInvoiceReturnDetails.Any())
            {
                await EditPurchaseInvoiceReturnDetail(purchaseInvoiceReturnDetails);
                return await AddPurchaseInvoiceReturnDetail(purchaseInvoiceReturnDetails, purchaseInvoiceReturnHeaderId);
            }
            return purchaseInvoiceReturnDetails;
        }

        public async Task<bool> DeletePurchaseInvoiceReturnDetails(int purchaseInvoiceReturnHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeletePurchaseInvoiceReturnDetailList(List<PurchaseInvoiceReturnDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.PurchaseInvoiceReturnDetailId != p.PurchaseInvoiceReturnDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<List<PurchaseInvoiceReturnDetailDto>> AddPurchaseInvoiceReturnDetail(List<PurchaseInvoiceReturnDetailDto> details, int headerId)
        {
            var current = details.Where(x => x.PurchaseInvoiceReturnDetailId <= 0).ToList();
            var modelList = new List<PurchaseInvoiceReturnDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new PurchaseInvoiceReturnDetail()
                {
                    PurchaseInvoiceReturnDetailId = newId,
                    PurchaseInvoiceReturnHeaderId = headerId,
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

                detail.PurchaseInvoiceReturnDetailId = newId;
                detail.PurchaseInvoiceReturnHeaderId = headerId;

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

        private async Task<bool> EditPurchaseInvoiceReturnDetail(List<PurchaseInvoiceReturnDetailDto> purchaseInvoiceReturnDetails)
        {
            var current = purchaseInvoiceReturnDetails.Where(x => x.PurchaseInvoiceReturnDetailId > 0).ToList();
            var modelList = new List<PurchaseInvoiceReturnDetail>();
            foreach (var detail in current)
            {
                var model = new PurchaseInvoiceReturnDetail()
                {
                    PurchaseInvoiceReturnDetailId = detail.PurchaseInvoiceReturnDetailId,
                    PurchaseInvoiceReturnHeaderId = detail.PurchaseInvoiceReturnHeaderId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.PurchaseInvoiceReturnDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
