using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
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
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.StaticData;

namespace Sales.Service.Services
{
	public class ProformaInvoiceDetailService : BaseService<ProformaInvoiceDetail>, IProformaInvoiceDetailService
	{
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemService _itemService;
		private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ICostCenterService _costCenterService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemTaxService _itemTaxService;

		public ProformaInvoiceDetailService(IRepository<ProformaInvoiceDetail> repository, IItemPackageService itemPackageService, IItemService itemService, IItemBarCodeService itemBarCodeService, ICostCenterService costCenterService,IHttpContextAccessor httpContextAccessor,IItemTaxService itemTaxService) : base(repository)
		{
			_itemPackageService = itemPackageService;
			_itemService = itemService;
			_itemBarCodeService = itemBarCodeService;
            _costCenterService = costCenterService;
			_httpContextAccessor = httpContextAccessor;
            _itemTaxService = itemTaxService;
        }

		public IQueryable<ProformaInvoiceDetailDto> GetProformaInvoiceDetailsAsQueryable(int proformaInvoiceHeaderId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

            return from proformaInvoiceDetail in _repository.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
				   from item in _itemService.GetAll().Where(x => x.ItemId == proformaInvoiceDetail.ItemId)
				   from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == proformaInvoiceDetail.ItemPackageId)
				   from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == proformaInvoiceDetail.CostCenterId).DefaultIfEmpty()
				   select new ProformaInvoiceDetailDto
				   {
					   ProformaInvoiceDetailId = proformaInvoiceDetail.ProformaInvoiceDetailId,
					   ProformaInvoiceHeaderId = proformaInvoiceDetail.ProformaInvoiceHeaderId,
					   CostCenterId = proformaInvoiceDetail.CostCenterId,
					   CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
					   ItemId = proformaInvoiceDetail.ItemId,
					   ItemCode = item.ItemCode,
					   ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
					   ItemNameReport = item.ItemTypeId == ItemTypeData.Note ? proformaInvoiceDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
                       ItemTypeId = item.ItemTypeId,
					   TaxTypeId = item.TaxTypeId,
					   ItemPackageId = proformaInvoiceDetail.ItemPackageId,
					   ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
					   IsItemVatInclusive = proformaInvoiceDetail.IsItemVatInclusive,
					   BarCode = proformaInvoiceDetail.BarCode,
					   Packing = proformaInvoiceDetail.Packing,
					   Quantity = proformaInvoiceDetail.Quantity,
					   BonusQuantity = proformaInvoiceDetail.BonusQuantity,
					   SellingPrice = proformaInvoiceDetail.SellingPrice,
					   TotalValue = proformaInvoiceDetail.TotalValue,
					   ItemDiscountPercent = proformaInvoiceDetail.ItemDiscountPercent,
					   ItemDiscountValue = proformaInvoiceDetail.ItemDiscountValue,
					   TotalValueAfterDiscount = proformaInvoiceDetail.TotalValueAfterDiscount,
					   HeaderDiscountValue = proformaInvoiceDetail.HeaderDiscountValue,
					   GrossValue = proformaInvoiceDetail.GrossValue,
					   VatPercent = proformaInvoiceDetail.VatPercent,
					   VatValue = proformaInvoiceDetail.VatValue,
					   SubNetValue = proformaInvoiceDetail.SubNetValue,
					   OtherTaxValue = proformaInvoiceDetail.OtherTaxValue,
					   NetValue = proformaInvoiceDetail.NetValue,
					   Notes = proformaInvoiceDetail.Notes,
                       ItemNote = proformaInvoiceDetail.ItemNote,
					   ConsumerPrice = proformaInvoiceDetail.ConsumerPrice,
					   CostPrice = proformaInvoiceDetail.CostPrice,
					   CostPackage = proformaInvoiceDetail.CostPackage,
					   CostValue = proformaInvoiceDetail.CostValue,
					   LastSalesPrice = proformaInvoiceDetail.LastSalesPrice,

					   CreatedAt = proformaInvoiceDetail.CreatedAt,
					   IpAddressCreated = proformaInvoiceDetail.IpAddressCreated,
					   UserNameCreated = proformaInvoiceDetail.UserNameCreated,
				   };
		}

		public async Task<List<ProformaInvoiceDetailDto>> GetProformaInvoiceDetails(int proformaInvoiceHeaderId)
		{
			var modelList = new List<ProformaInvoiceDetailDto>();

			var data = await GetProformaInvoiceDetailsAsQueryable(proformaInvoiceHeaderId).ToListAsync();

            var itemIds = data.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var proformaInvoiceDetail in data)
			{
				var model = new ProformaInvoiceDetailDto();
				model = proformaInvoiceDetail;
                model.Packages = packages.Where(x => x.ItemId == proformaInvoiceDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
				model.ItemTaxData = itemTaxData.Where(x => x.ItemId == proformaInvoiceDetail.ItemId).ToList();
                model.Taxes = model.ItemTaxData.ToJson();
                modelList.Add(model);
			}
			return modelList;
		}

		public IQueryable<ProformaInvoiceDetailDto> GetProformaInvoiceDetailsGroupedQueryable(int proformaInvoiceHeaderId)
		{
			return _repository.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(
				x => new ProformaInvoiceDetailDto
				{
					ItemId = x.Key.ItemId,
					ItemPackageId = x.Key.ItemPackageId,
					BarCode = x.Key.BarCode,
					SellingPrice = x.Key.SellingPrice,
					ItemDiscountPercent = x.Key.ItemDiscountPercent,
					Quantity = x.Sum(y => y.Quantity),
					BonusQuantity = x.Sum(y => y.BonusQuantity)
				});
		}

		public async Task<List<ProformaInvoiceDetailDto>> GetProformaInvoiceDetailsGrouped(int proformaInvoiceHeaderId)
		{
			var details = await GetProformaInvoiceDetailsAsQueryable(proformaInvoiceHeaderId).ToListAsync();
			return GroupProformaInvoiceDetails(details);
		}

        public async Task<List<ProformaInvoiceDetailDto>> GetProformaInvoiceDetailsFullyGrouped(int proformaInvoiceHeaderId)
        {
            //(ItemId, ItemPackageId, BarCode, SellingPrice, ItemDiscountPercent) are the main keys, the other keys are related to these 5
            return await GetProformaInvoiceDetailsAsQueryable(proformaInvoiceHeaderId).GroupBy(x => new { x.ItemId, x.ItemCode, x.ItemName, x.TaxTypeId, x.ItemTypeId, x.ItemPackageId, x.ItemPackageName, x.IsItemVatInclusive, x.BarCode, x.Packing, x.SellingPrice, x.ItemDiscountPercent, x.VatPercent, x.Notes, x.ItemNote })
                .Select(x => new ProformaInvoiceDetailDto
                {
					ProformaInvoiceDetailId = x.Select(y => y.ProformaInvoiceDetailId).First(),
					ItemId = x.Key.ItemId,
                    ItemCode = x.Key.ItemCode, 
                    ItemName = x.Key.ItemName, 
                    ItemTypeId = x.Key.ItemTypeId, 
                    TaxTypeId = x.Key.TaxTypeId, 
                    ItemPackageId = x.Key.ItemPackageId, 
                    ItemPackageName = x.Key.ItemPackageName, 
                    IsItemVatInclusive = x.Key.IsItemVatInclusive, 
                    BarCode = x.Key.BarCode, 
                    Packing = x.Key.Packing, 
                    //Quantity = x.Sum(y => y.Quantity), 
                    //BonusQuantity = x.Sum(y => y.BonusQuantity), 
                    SellingPrice = x.Key.SellingPrice, 
                    ItemDiscountPercent = x.Key.ItemDiscountPercent, 
                    VatPercent = x.Key.VatPercent, 
                    Notes = x.Key.Notes, 
                    ItemNote = x.Key.ItemNote
                }).ToListAsync();
        }

		public List<ProformaInvoiceDetailDto> GroupProformaInvoiceDetails(List<ProformaInvoiceDetailDto> details)
        {
			return details.GroupBy(x => new { x.ItemId, x.ItemPackageId , x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(
				x => new ProformaInvoiceDetailDto
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

		public async Task<List<ProformaInvoiceDetailDto>> SaveProformaInvoiceDetails(int proformaInvoiceHeaderId, List<ProformaInvoiceDetailDto> proformaInvoiceDetails)
        {
            if (proformaInvoiceDetails.Any())
            {
                await EditProformaInvoiceDetail(proformaInvoiceDetails);
                return await AddProformaInvoiceDetail(proformaInvoiceDetails, proformaInvoiceHeaderId);
            }
            return proformaInvoiceDetails;
        }
        public async Task<bool> DeleteProformaInvoiceDetails(int proformaInvoiceHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteProformaInvoiceDetailList(List<ProformaInvoiceDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.ProformaInvoiceHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.ProformaInvoiceDetailId != p.ProformaInvoiceDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        private async Task<List<ProformaInvoiceDetailDto>> AddProformaInvoiceDetail(List<ProformaInvoiceDetailDto> details, int headerId)
        {   
            var current = details.Where(x => x.ProformaInvoiceDetailId <= 0).ToList();
            var modelList = new List<ProformaInvoiceDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new ProformaInvoiceDetail()
                {
                    ProformaInvoiceDetailId = newId,
                    ProformaInvoiceHeaderId = headerId,
                    CostCenterId = detail.CostCenterId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
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

                detail.ProformaInvoiceDetailId = newId;
                detail.ProformaInvoiceHeaderId = headerId;

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

        private async Task<bool> EditProformaInvoiceDetail(List<ProformaInvoiceDetailDto> proformaInvoiceDetails)
        {   
            var current = proformaInvoiceDetails.Where(x => x.ProformaInvoiceDetailId > 0).ToList();
            var modelList = new List<ProformaInvoiceDetail>();
            foreach (var detail in current)
            {
                var model = new ProformaInvoiceDetail()
                {
                    ProformaInvoiceDetailId = detail.ProformaInvoiceDetailId,
                    ProformaInvoiceHeaderId = detail.ProformaInvoiceHeaderId,
                    CostCenterId = detail.CostCenterId,
                    ItemId = detail.ItemId,
                    ItemPackageId = detail.ItemPackageId,
                    IsItemVatInclusive = detail.IsItemVatInclusive,
                    BarCode = detail.BarCode,
                    Packing = detail.Packing,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.ProformaInvoiceDetailId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
