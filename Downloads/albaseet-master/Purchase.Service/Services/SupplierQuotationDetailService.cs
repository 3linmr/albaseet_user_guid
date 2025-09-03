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
	public class SupplierQuotationDetailService : BaseService<SupplierQuotationDetail>, ISupplierQuotationDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemTaxService _itemTaxService;

		public SupplierQuotationDetailService(IRepository<SupplierQuotationDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService,IHttpContextAccessor httpContextAccessor,IItemTaxService itemTaxService) : base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
			_httpContextAccessor = httpContextAccessor;
            _itemTaxService = itemTaxService;
        }

		public async Task<List<SupplierQuotationDetailDto>> GetSupplierQuotationDetails(int supplierQuotationHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var modelList = new List<SupplierQuotationDetailDto>();

			var data =
			 await (from supplierQuotationDetail in _repository.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeaderId)
				   from item in _itemService.GetAll().Where(x => x.ItemId == supplierQuotationDetail.ItemId)
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == supplierQuotationDetail.ItemPackageId)
				   from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == supplierQuotationDetail.CostCenterId).DefaultIfEmpty()
                   select new SupplierQuotationDetailDto
				   {
					   SupplierQuotationDetailId = supplierQuotationDetail.SupplierQuotationDetailId,
                       SupplierQuotationHeaderId = supplierQuotationDetail.SupplierQuotationHeaderId,
					   CostCenterId = supplierQuotationDetail.CostCenterId,
                       CostCenterName = costCenter != null? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                       ItemId = supplierQuotationDetail.ItemId,
                       ItemCode = item.ItemCode,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					   ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? supplierQuotationDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
                       TaxTypeId = item.TaxTypeId,
                       ItemTypeId = item.ItemTypeId,
					   ItemPackageId = supplierQuotationDetail.ItemPackageId,
					   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                       IsItemVatInclusive = supplierQuotationDetail.IsItemVatInclusive,
					   BarCode = supplierQuotationDetail.BarCode,
					   Packing = supplierQuotationDetail.Packing,
					   Quantity = supplierQuotationDetail.Quantity,
                       ReceivedPrice = supplierQuotationDetail.ReceivedPrice,
                       TotalValue = supplierQuotationDetail.TotalValue,
                       ItemDiscountPercent = supplierQuotationDetail.ItemDiscountPercent,
                       ItemDiscountValue = supplierQuotationDetail.ItemDiscountValue,
                       TotalValueAfterDiscount = supplierQuotationDetail.TotalValueAfterDiscount,
                       HeaderDiscountValue = supplierQuotationDetail.HeaderDiscountValue,
                       GrossValue = supplierQuotationDetail.GrossValue,
                       VatPercent = supplierQuotationDetail.VatPercent,
                       VatValue = supplierQuotationDetail.VatValue,
                       SubNetValue = supplierQuotationDetail.SubNetValue,
                       OtherTaxValue = supplierQuotationDetail.OtherTaxValue,
                       NetValue = supplierQuotationDetail.NetValue,
                       Notes = supplierQuotationDetail.Notes,
                       ItemNote = supplierQuotationDetail.ItemNote,
                       ConsumerPrice = supplierQuotationDetail.ConsumerPrice,
                       CostPrice = supplierQuotationDetail.CostPrice,
                       CostPackage = supplierQuotationDetail.CostPackage,
                       CostValue = supplierQuotationDetail.CostValue,
                       LastPurchasePrice = supplierQuotationDetail.LastPurchasePrice,

                       CreatedAt = supplierQuotationDetail.CreatedAt,
                       IpAddressCreated = supplierQuotationDetail.IpAddressCreated,
                       UserNameCreated = supplierQuotationDetail.UserNameCreated,
				   }).ToListAsync();

            var itemIds = data.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var supplierQuotationDetail in data)
			{
				var model = new SupplierQuotationDetailDto();
				model = supplierQuotationDetail;
                model.Packages = packages.Where(x => x.ItemId == supplierQuotationDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
				model.ItemTaxData = itemTaxData.Where(x => x.ItemId == supplierQuotationDetail.ItemId).ToList();
                model.Taxes = model.ItemTaxData.ToJson();
                modelList.Add(model);
			}
			return modelList;
		}

        public async Task<List<SupplierQuotationDetailDto>> SaveSupplierQuotationDetails(int supplierQuotationHeaderId, List<SupplierQuotationDetailDto> supplierQuotationDetails)
        {
            if (supplierQuotationDetails.Any())
            {
                await EditSupplierQuotationDetail(supplierQuotationDetails);
                return await AddSupplierQuotationDetail(supplierQuotationDetails, supplierQuotationHeaderId);
            }
            return supplierQuotationDetails;
        }
        public async Task<bool> DeleteSupplierQuotationDetails(int supplierQuotationHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteSupplierQuotationDetailList(List<SupplierQuotationDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.SupplierQuotationHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.SupplierQuotationDetailId != p.SupplierQuotationDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<List<SupplierQuotationDetailDto>> AddSupplierQuotationDetail(List<SupplierQuotationDetailDto> details, int headerId)
        {   
            var current = details.Where(x => x.SupplierQuotationDetailId <= 0).ToList();
            var modelList = new List<SupplierQuotationDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new SupplierQuotationDetail()
                {
                    SupplierQuotationDetailId = newId,
                    SupplierQuotationHeaderId = headerId,
                    CostCenterId = detail.CostCenterId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    Quantity = detail.Quantity,
                    ReceivedPrice = detail.ReceivedPrice,
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

                detail.SupplierQuotationDetailId = newId;
                detail.SupplierQuotationHeaderId = headerId;

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

        private async Task<bool> EditSupplierQuotationDetail(List<SupplierQuotationDetailDto> supplierQuotationDetails)
        {   
            var current = supplierQuotationDetails.Where(x => x.SupplierQuotationDetailId > 0).ToList();
            var modelList = new List<SupplierQuotationDetail>();
            foreach (var detail in current)
            {
                var model = new SupplierQuotationDetail()
                {
                    SupplierQuotationDetailId = detail.SupplierQuotationDetailId,
                    SupplierQuotationHeaderId = detail.SupplierQuotationHeaderId,
                    CostCenterId = detail.CostCenterId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
                    Quantity = detail.Quantity,
                    ReceivedPrice = detail.ReceivedPrice,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.SupplierQuotationDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
