using Purchases.CoreOne.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Approval;
using Shared.CoreOne.Contracts.Inventory;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Modules;
using Shared.Service.Logic.Calculation;
using Shared.CoreOne.Contracts.Taxes;
using Shared.Service.Services.Taxes;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.StaticData;
using Purchases.CoreOne.Models.Domain;
using Shared.Helper.Extensions;
using Shared.Service.Services.Items;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.Service.Services.Inventory;
using Shared.CoreOne.Models.Domain.Items;
using Microsoft.Extensions.Localization;

namespace Purchases.Service.Services
{
    public class SupplierQuotationService : ISupplierQuotationService
    {
        private readonly ISupplierQuotationHeaderService _supplierQuotationHeaderService;
        private readonly ISupplierQuotationDetailService _supplierQuotationDetailService;
        private readonly IProductRequestPriceHeaderService _productRequestPriceHeaderService;
        private readonly IProductRequestPriceDetailService _productRequestPriceDetailService;
        private readonly IProductRequestPriceDetailTaxService _productRequestPriceDetailTaxService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly ISupplierQuotationDetailTaxService _supplierQuotationDetailTaxService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemService _itemService;
        private readonly ICostCenterService _costCenterService;
        private readonly IItemPackageService _itemPackageService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IItemTaxService _itemTaxService;
        private readonly IItemCostService _itemCostService;
        private readonly IItemPackingService _itemPackingService;
        private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
        private readonly IPurchaseInvoiceService _purchaseInvoiceService;
        private readonly ITaxPercentService _taxPercentService;
        private readonly ITaxService _taxService;
        private readonly IItemNoteValidationService _itemNoteValidationService;

        public SupplierQuotationService(ISupplierQuotationHeaderService supplierQuotationHeaderService, ISupplierQuotationDetailService supplierQuotationDetailService, IProductRequestPriceHeaderService productRequestPriceHeaderService, IProductRequestPriceDetailTaxService productRequestPriceDetailTaxService, IProductRequestPriceDetailService productRequestPriceDetailService, IMenuNoteService menuNoteService, ISupplierQuotationDetailTaxService supplierQuotationDetailTaxService, IHttpContextAccessor httpContextAccessor, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService, IItemBarCodeService itemBarCodeService, IItemTaxService itemTaxService, IItemCostService itemCostService, IItemPackingService itemPackingService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IPurchaseInvoiceService purchaseInvoiceService, ITaxPercentService taxPercentService, ITaxService taxService, IItemNoteValidationService itemNoteValidationService)
        {
            _supplierQuotationHeaderService = supplierQuotationHeaderService;
            _supplierQuotationDetailService = supplierQuotationDetailService;
            _productRequestPriceDetailService = productRequestPriceDetailService;
            _productRequestPriceHeaderService = productRequestPriceHeaderService;
            _productRequestPriceDetailTaxService = productRequestPriceDetailTaxService;
            _menuNoteService = menuNoteService;
            _supplierQuotationDetailTaxService = supplierQuotationDetailTaxService;
            _httpContextAccessor = httpContextAccessor;
            _itemService = itemService;
            _costCenterService = costCenterService;
            _itemPackageService = itemPackageService;
            _itemBarCodeService = itemBarCodeService;
            _itemTaxService = itemTaxService;
            _itemCostService = itemCostService;
            _itemPackingService = itemPackingService;
            _purchaseInvoiceDetailService = purchaseInvoiceDetailService;
            _purchaseInvoiceService = purchaseInvoiceService;
            _taxPercentService = taxPercentService;
            _taxService = taxService;
            _itemNoteValidationService = itemNoteValidationService;
        }
        public List<RequestChangesDto> GetSupplierQuotationRequestChanges(SupplierQuotationDto oldItem, SupplierQuotationDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.SupplierQuotationHeader, newItem.SupplierQuotationHeader);
            requestChanges.AddRange(items);

            if (oldItem.SupplierQuotationDetails.Any() && newItem.SupplierQuotationDetails.Any())
            {
                var oldCount = oldItem.SupplierQuotationDetails.Count;
                var newCount = newItem.SupplierQuotationDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.SupplierQuotationDetails[i], newItem.SupplierQuotationDetails[i]);
                            requestChanges.AddRange(changes);

                            var detailTaxChanges = GetSupplierQuotationDetailTaxesRequestChanges(oldItem.SupplierQuotationDetails[i], newItem.SupplierQuotationDetails[i]);
                            requestChanges.AddRange(detailTaxChanges);
                            index++;
                            break;
                        }
                    }
                }
            }

            if (oldItem.MenuNotes != null && oldItem.MenuNotes.Any() && newItem.MenuNotes != null && newItem.MenuNotes.Any())
            {
                var oldCount = oldItem.MenuNotes.Count;
                var newCount = newItem.MenuNotes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.MenuNotes[i], newItem.MenuNotes[i]);
                            requestChanges.AddRange(changes);
                            index++;
                            break;
                        }
                    }
                }
            }

            requestChanges.RemoveAll(x => x.ColumnName == "SupplierQuotationDetailTaxes" || x.ColumnName == "ItemTaxData");

            return requestChanges;
        }

        private static List<RequestChangesDto> GetSupplierQuotationDetailTaxesRequestChanges(SupplierQuotationDetailDto oldItem, SupplierQuotationDetailDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            if (oldItem.SupplierQuotationDetailTaxes.Any() && newItem.SupplierQuotationDetailTaxes.Any())
            {
                var oldCount = oldItem.SupplierQuotationDetailTaxes.Count;
                var newCount = newItem.SupplierQuotationDetailTaxes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.SupplierQuotationDetailTaxes[i], newItem.SupplierQuotationDetailTaxes[i]);
                            requestChanges.AddRange(changes);

                            index++;
                            break;
                        }
                    }
                }
            }

            return requestChanges;
        }

        public async Task<SupplierQuotationDto> GetSupplierQuotation(int supplierQuotationHeaderId)
        {
            var header = await _supplierQuotationHeaderService.GetSupplierQuotationHeaderById(supplierQuotationHeaderId);
            var details = await _supplierQuotationDetailService.GetSupplierQuotationDetails(supplierQuotationHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.SupplierQuotation, supplierQuotationHeaderId).ToListAsync();
            var supplierQuotationDetailTaxes = await _supplierQuotationDetailTaxService.GetSupplierQuotationDetailTaxes(supplierQuotationHeaderId).ToListAsync();

            foreach (var detail in details)
            {
                detail.SupplierQuotationDetailTaxes = supplierQuotationDetailTaxes.Where(x => x.SupplierQuotationDetailId == detail.SupplierQuotationDetailId).ToList();
            }

            return new SupplierQuotationDto() { SupplierQuotationHeader = header, SupplierQuotationDetails = details, MenuNotes = menuNotes };
        }

        public async Task<SupplierQuotationDto> GetSupplierQuotationFromProductRequestPrice(int productRequestPriceHeaderId)
        {
            var productRequestPriceHeader = await _productRequestPriceHeaderService.GetProductRequestPriceHeaderById(productRequestPriceHeaderId);
            
            if (productRequestPriceHeader == null)
            {
                return new SupplierQuotationDto();
            }

            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var supplierQuotationDetails =
             await (from productRequestPriceDetail in _productRequestPriceDetailService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeaderId).OrderBy(x => x.ProductRequestPriceDetailId)
                    from item in _itemService.GetAll().Where(x => x.ItemId == productRequestPriceDetail.ItemId)
                    from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == productRequestPriceDetail.ItemPackageId)
                    from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == productRequestPriceDetail.CostCenterId).DefaultIfEmpty()
                    from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == productRequestPriceDetail.ItemId && x.StoreId == productRequestPriceHeader.StoreId).DefaultIfEmpty()
                    from itemPacking in _itemPackingService.GetAll().Where(x => x.ItemId == productRequestPriceDetail.ItemId && x.FromPackageId == productRequestPriceDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                    from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.ItemId == productRequestPriceDetail.ItemId && x.ItemPackageId == productRequestPriceDetail.ItemPackageId).OrderByDescending(x => x.PurchaseInvoiceDetailId).Take(1).DefaultIfEmpty()
                    select new SupplierQuotationDetailDto
                    {
                        SupplierQuotationDetailId = productRequestPriceDetail.ProductRequestPriceDetailId, // <-- This is used to get the related detail taxes
                        CostCenterId = productRequestPriceDetail.CostCenterId,
                        CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                        ItemId = productRequestPriceDetail.ItemId,
                        ItemCode = item.ItemCode,
                        ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                        TaxTypeId = item.TaxTypeId,
                        ItemTypeId = item.ItemTypeId,
                        ItemPackageId = productRequestPriceDetail.ItemPackageId,
                        ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                        IsItemVatInclusive = productRequestPriceDetail.IsItemVatInclusive,
                        BarCode = productRequestPriceDetail.BarCode,
                        Packing = productRequestPriceDetail.Packing,
                        Quantity = productRequestPriceDetail.Quantity,
                        ReceivedPrice = productRequestPriceDetail.RequestedPrice,
                        TotalValue = productRequestPriceDetail.TotalValue,
                        ItemDiscountPercent = productRequestPriceDetail.ItemDiscountPercent,
                        ItemDiscountValue = productRequestPriceDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = productRequestPriceDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = productRequestPriceDetail.HeaderDiscountValue,
                        GrossValue = productRequestPriceDetail.GrossValue,
                        VatPercent = productRequestPriceDetail.VatPercent,
                        VatValue = productRequestPriceDetail.VatValue,
                        SubNetValue = productRequestPriceDetail.SubNetValue,
                        OtherTaxValue = productRequestPriceDetail.OtherTaxValue,
                        NetValue = productRequestPriceDetail.NetValue,
                        Notes = productRequestPriceDetail.Notes,
                        ItemNote = productRequestPriceDetail.ItemNote,
                        ConsumerPrice = item.ConsumerPrice,
                        CostPrice = itemCost != null ? itemCost.CostPrice: 0,
                        CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
                        CostValue = itemCost != null ? itemCost.CostPrice * itemPacking.Packing * productRequestPriceDetail.Quantity : 0,
                        LastPurchasePrice = purchaseInvoiceDetail != null ? purchaseInvoiceDetail.PurchasePrice : 0,
                        CreatedAt = productRequestPriceDetail.CreatedAt,
                        IpAddressCreated = productRequestPriceDetail.IpAddressCreated,
                        UserNameCreated = productRequestPriceDetail.UserNameCreated
                    }).ToListAsync();


            var itemIds = supplierQuotationDetails.Select(x => x.ItemId).ToList();
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);
            var productRequestPriceDetailTaxes = await _productRequestPriceDetailTaxService.GetProductRequestPriceDetailTaxes(productRequestPriceHeaderId).ToListAsync();

            int newId = -1;
            int newSubId = -1;
            foreach (var supplierQuotationDetail in supplierQuotationDetails)
            {
                supplierQuotationDetail.Packages = packages.Where(x => x.ItemId == supplierQuotationDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                supplierQuotationDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == supplierQuotationDetail.ItemId).ToList();
                supplierQuotationDetail.Taxes = supplierQuotationDetail.ItemTaxData.ToJson();

                supplierQuotationDetail.SupplierQuotationDetailTaxes = (
                        from itemTax in productRequestPriceDetailTaxes.Where(x => x.ProductRequestPriceDetailId == supplierQuotationDetail.SupplierQuotationDetailId)
                        select new SupplierQuotationDetailTaxDto
                        {
                            TaxId = itemTax.TaxId,
                            DebitAccountId = itemTax.DebitAccountId,
                            TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
                            TaxPercent = itemTax.TaxPercent,
                            TaxValue = itemTax.TaxValue
                        }
                    ).ToList();

                supplierQuotationDetail.SupplierQuotationDetailId = newId;
                supplierQuotationDetail.SupplierQuotationDetailTaxes.ForEach(y =>
                {
                    y.SupplierQuotationDetailId = newId;
                    y.SupplierQuotationDetailTaxId = newSubId--;
                });
                newId--;
            }

            var totalCostValueFromDetail = supplierQuotationDetails.Sum(x => x.CostValue);

            var supplierQuotationHeader = new SupplierQuotationHeaderDto
            {
                SupplierQuotationHeaderId = 0,
                ProductRequestPriceHeaderId = productRequestPriceHeader.ProductRequestPriceHeaderId,
                SupplierId = productRequestPriceHeader.SupplierId,
                SupplierCode = productRequestPriceHeader.SupplierCode,
                SupplierName = productRequestPriceHeader.SupplierName,
                StoreId = productRequestPriceHeader.StoreId,
                StoreName = productRequestPriceHeader.StoreName,
                TaxTypeId = productRequestPriceHeader.TaxTypeId,
                DocumentDate = productRequestPriceHeader.DocumentDate,
                Reference = productRequestPriceHeader.Reference,
                TotalValue = productRequestPriceHeader.TotalValue,
                DiscountPercent = productRequestPriceHeader.DiscountPercent,
                DiscountValue = productRequestPriceHeader.DiscountValue,
                TotalItemDiscount = productRequestPriceHeader.TotalItemDiscount,
                GrossValue = productRequestPriceHeader.GrossValue,
                VatValue = productRequestPriceHeader.VatValue,
                SubNetValue = productRequestPriceHeader.SubNetValue,
                OtherTaxValue = productRequestPriceHeader.OtherTaxValue,
				NetValueBeforeAdditionalDiscount = productRequestPriceHeader.NetValueBeforeAdditionalDiscount,
				AdditionalDiscountValue = productRequestPriceHeader.AdditionalDiscountValue,
                NetValue = productRequestPriceHeader.NetValue,
                TotalCostValue = totalCostValueFromDetail,
                RemarksAr = productRequestPriceHeader.RemarksAr,
                RemarksEn = productRequestPriceHeader.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = productRequestPriceHeader.ArchiveHeaderId,
            };

            return new SupplierQuotationDto { SupplierQuotationHeader = supplierQuotationHeader, SupplierQuotationDetails = supplierQuotationDetails};
        }

        public async Task<ResponseDto> SaveSupplierQuotation(SupplierQuotationDto supplierQuotation, bool hasApprove, bool approved, int? requestId)
        {
            TrimDetailStrings(supplierQuotation.SupplierQuotationDetails);
            if (supplierQuotation.SupplierQuotationHeader != null)
			{
				var itemNoteValidationResult = await _itemNoteValidationService.CheckItemNoteWithItemType(supplierQuotation.SupplierQuotationDetails, x => x.ItemId, x => x.ItemNote);
				if (itemNoteValidationResult.Success == false) return itemNoteValidationResult;

				if (supplierQuotation.SupplierQuotationHeader.SupplierQuotationHeaderId == 0)
                {
                    await UpdateModelPrices(supplierQuotation);
                }

                var result = await _supplierQuotationHeaderService.SaveSupplierQuotationHeader(supplierQuotation.SupplierQuotationHeader, hasApprove, approved, requestId);
                if (result.Success)
                {
                    var modifiedSupplierQuotationDetails = await _supplierQuotationDetailService.SaveSupplierQuotationDetails(result.Id, supplierQuotation.SupplierQuotationDetails);
                    await SaveSupplierQuotationDetailTaxes(result.Id, modifiedSupplierQuotationDetails);
                    await _supplierQuotationDetailService.DeleteSupplierQuotationDetailList(modifiedSupplierQuotationDetails, result.Id);
                    
                    if (supplierQuotation.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(supplierQuotation.MenuNotes, result.Id);
                    }
                }

                return result;
            }
            return new ResponseDto{ Message = "Header should not be null" };
		}

		private void TrimDetailStrings(List<SupplierQuotationDetailDto> supplierQuotationDetails)
		{
			foreach (var supplierQuotationDetail in supplierQuotationDetails)
			{
				supplierQuotationDetail.ItemNote = string.IsNullOrWhiteSpace(supplierQuotationDetail.ItemNote) ? null : supplierQuotationDetail.ItemNote.Trim();
			}
		}

		private async Task UpdateModelPrices(SupplierQuotationDto supplierQuotation)
        {
            await UpdateDetailPrices(supplierQuotation.SupplierQuotationDetails, supplierQuotation.SupplierQuotationHeader!.StoreId);
            supplierQuotation.SupplierQuotationHeader!.TotalCostValue = supplierQuotation.SupplierQuotationDetails.Sum(x => x.CostValue);
        }

        private async Task UpdateDetailPrices(List<SupplierQuotationDetailDto> supplierQuotationDetails, int storeId)
        {
            var itemIds = supplierQuotationDetails.Select(x => x.ItemId).ToList();

            var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId) && x.StoreId == storeId).Select(x => new { x.StoreId, x.ItemId, x.CostPrice }).ToListAsync();
            var consumerPrices = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).Select(x => new { x.ItemId, x.ConsumerPrice }).ToListAsync();
            var lastPurchasePrices = await _purchaseInvoiceService.GetMultipleLastPurchasePrices(itemIds);

            var packings = await (
                        from item in _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId))
                        from itemPacking in _itemPackingService.GetAll().Where(x => x.ItemId == item.ItemId && x.ToPackageId == item.SingularPackageId)
                        select new
                        {
                            item.ItemId,
                            itemPacking.FromPackageId,
                            itemPacking.Packing
                        }
                    ).ToListAsync();

            foreach (var supplierQuotationDetail in supplierQuotationDetails)
            {
                var packing = packings.Where(x => x.ItemId == supplierQuotationDetail.ItemId && x.FromPackageId == supplierQuotationDetail.ItemPackageId).Select(x => x.Packing).FirstOrDefault(1);

                supplierQuotationDetail.ConsumerPrice = consumerPrices.Where(x => x.ItemId == supplierQuotationDetail.ItemId).Select(x => x.ConsumerPrice).FirstOrDefault(0);
                supplierQuotationDetail.CostPrice = itemCosts.Where(x => x.ItemId == supplierQuotationDetail.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefault(0);
                supplierQuotationDetail.CostPackage = supplierQuotationDetail.CostPrice * packing;
                supplierQuotationDetail.CostValue = supplierQuotationDetail.CostPackage * supplierQuotationDetail.Quantity;
                supplierQuotationDetail.LastPurchasePrice = lastPurchasePrices.Where(x => x.ItemId == supplierQuotationDetail.ItemId && x.ItemPackageId == supplierQuotationDetail.ItemPackageId).Select(x => x.PurchasePrice).FirstOrDefault(0);
            }
        }

        private async Task SaveSupplierQuotationDetailTaxes(int supplierQuotationHeaderId, List<SupplierQuotationDetailDto> supplierQuotationDetails)
        {
            List<SupplierQuotationDetailTaxDto> supplierQuotationDetailTaxes = new List<SupplierQuotationDetailTaxDto>();

            foreach (var supplierQuotationDetail in supplierQuotationDetails)
            {
                foreach (var supplierQuotationDetailTax in supplierQuotationDetail.SupplierQuotationDetailTaxes)
                {
                    supplierQuotationDetailTax.SupplierQuotationDetailId = supplierQuotationDetail.SupplierQuotationDetailId;
                    supplierQuotationDetailTaxes.Add(supplierQuotationDetailTax);
                }
            }

            await _supplierQuotationDetailTaxService.SaveSupplierQuotationDetailTaxes(supplierQuotationHeaderId, supplierQuotationDetailTaxes);
        }

        public async Task<ResponseDto> DeleteSupplierQuotation(int supplierQuotationHeaderId)
        {
            await _menuNoteService.DeleteMenuNotes(MenuCodeData.SupplierQuotation, supplierQuotationHeaderId);
            await _supplierQuotationDetailTaxService.DeleteSupplierQuotationDetailTaxes(supplierQuotationHeaderId);
            await _supplierQuotationDetailService.DeleteSupplierQuotationDetails(supplierQuotationHeaderId);
            var result = await _supplierQuotationHeaderService.DeleteSupplierQuotationHeader(supplierQuotationHeaderId);
            return result;
        }
    }
}
