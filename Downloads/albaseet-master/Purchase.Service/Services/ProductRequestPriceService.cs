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
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Newtonsoft.Json;
using Shared.CoreOne.Models.Domain.Items;
using Purchases.CoreOne.Models.Domain;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Text.Json;
using Shared.Service.Services.Items;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Shared.Helper.Extensions;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.Domain.Taxes;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.Helper.Identity;
using Shared.CoreOne.Models.Domain.Inventory;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Localization;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Menus;

namespace Purchases.Service.Services
{
    public class ProductRequestPriceService : IProductRequestPriceService
    {
        private readonly IProductRequestPriceHeaderService _productRequestPriceHeaderService;
        private readonly IProductRequestPriceDetailService _productRequestPriceDetailService;
        private readonly IProductRequestDetailService _productRequestDetailService;
        private readonly IProductRequestHeaderService _productRequestHeaderService;
        private readonly IProductRequestPriceDetailTaxService _productRequestPriceDetailTaxService;
        private readonly ITaxPercentService _taxPercentService;
        private readonly IItemService _itemService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IItemTaxService _itemTaxService;
        private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IItemPackageService _itemPackageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICostCenterService _costCenterService;
        private readonly IItemCostService _itemCostService;
        private readonly IItemPackingService _itemPackingService;
        private readonly IPurchaseInvoiceService _purchaseInvoiceService;
        private readonly ITaxService _taxService;
        private readonly IItemNoteValidationService _itemNoteValidationService;

        public ProductRequestPriceService(IProductRequestPriceHeaderService productRequestPriceHeaderService, IProductRequestPriceDetailService productRequestPriceDetailService, IProductRequestDetailService productRequestDetailService, ITaxPercentService taxPercentService, IProductRequestHeaderService productRequestHeaderService, IItemService itemService, IMenuNoteService menuNoteService, IProductRequestPriceDetailTaxService productRequestPriceDetailTaxService, IItemTaxService itemTaxService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IItemBarCodeService itemBarCodeService, IItemPackageService itemPackageService, IHttpContextAccessor httpContextAccessor, ICostCenterService costCenterService, IItemCostService itemCostService, IItemPackingService itemPackingService, IPurchaseInvoiceService purchaseInvoiceService, ITaxService taxService, IItemNoteValidationService itemNoteValidationService)
        {
            _productRequestPriceHeaderService = productRequestPriceHeaderService;
            _productRequestPriceDetailService = productRequestPriceDetailService;
            _productRequestDetailService = productRequestDetailService;
            _productRequestHeaderService = productRequestHeaderService;
            _productRequestPriceDetailTaxService = productRequestPriceDetailTaxService;
            _taxPercentService = taxPercentService;
            _itemService = itemService;
            _menuNoteService = menuNoteService;
            _itemTaxService = itemTaxService;
            _purchaseInvoiceDetailService = purchaseInvoiceDetailService;
            _itemBarCodeService = itemBarCodeService;
            _itemPackageService = itemPackageService;
            _httpContextAccessor = httpContextAccessor;
            _costCenterService = costCenterService;
            _itemCostService = itemCostService;
            _itemPackageService = itemPackageService;
            _itemPackingService = itemPackingService;
            _purchaseInvoiceService = purchaseInvoiceService;
            _taxService = taxService;
            _itemNoteValidationService = itemNoteValidationService;
        }
        public List<RequestChangesDto> GetProductRequestPriceRequestChanges(ProductRequestPriceDto oldItem, ProductRequestPriceDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.ProductRequestPriceHeader, newItem.ProductRequestPriceHeader);
            requestChanges.AddRange(items);

            if (oldItem.ProductRequestPriceDetails.Any() && newItem.ProductRequestPriceDetails.Any())
            {
                var oldCount = oldItem.ProductRequestPriceDetails.Count;
                var newCount = newItem.ProductRequestPriceDetails.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.ProductRequestPriceDetails[i], newItem.ProductRequestPriceDetails[i]);
                            requestChanges.AddRange(changes);

                            var detailTaxChanges = GetProductRequestPriceDetailTaxesRequestChanges(oldItem.ProductRequestPriceDetails[i], newItem.ProductRequestPriceDetails[i]);
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

            requestChanges.RemoveAll(x => x.ColumnName == "ProductRequestPriceDetailTaxes" || x.ColumnName == "ItemTaxData");

            return requestChanges;
        }

        private static List<RequestChangesDto> GetProductRequestPriceDetailTaxesRequestChanges(ProductRequestPriceDetailDto oldItem, ProductRequestPriceDetailDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();

            if (oldItem.ProductRequestPriceDetailTaxes.Any() && newItem.ProductRequestPriceDetailTaxes.Any())
            {
                var oldCount = oldItem.ProductRequestPriceDetailTaxes.Count;
                var newCount = newItem.ProductRequestPriceDetailTaxes.Count;
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.ProductRequestPriceDetailTaxes[i], newItem.ProductRequestPriceDetailTaxes[i]);
                            requestChanges.AddRange(changes);

                            index++;
                            break;
                        }
                    }
                }
            }

            return requestChanges;
        }

        public async Task<ProductRequestPriceDto> GetProductRequestPrice(int productRequestPriceHeaderId)
        {
            var header = await _productRequestPriceHeaderService.GetProductRequestPriceHeaderById(productRequestPriceHeaderId);
            var details = await _productRequestPriceDetailService.GetProductRequestPriceDetails(productRequestPriceHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.ProductRequestPrice, productRequestPriceHeaderId).ToListAsync();
            var productRequestPriceDetailTaxes = await _productRequestPriceDetailTaxService.GetProductRequestPriceDetailTaxes(productRequestPriceHeaderId).ToListAsync();

            foreach(var detail in details)
            {
                detail.ProductRequestPriceDetailTaxes = productRequestPriceDetailTaxes.Where(x => x.ProductRequestPriceDetailId == detail.ProductRequestPriceDetailId).ToList();
            }

            return new ProductRequestPriceDto() { ProductRequestPriceHeader = header, ProductRequestPriceDetails = details, MenuNotes = menuNotes};
        }

        public async Task<ProductRequestPriceDto> GetProductRequestPriceFromProductRequest(int productRequestHeaderId, DateTime? currentDate = null)
        {
            var productRequestHeader = await _productRequestHeaderService.GetProductRequestHeaderById(productRequestHeaderId);
            
            if (productRequestHeader == null)
            {
                return new ProductRequestPriceDto();
            }

            currentDate ??= DateHelper.GetDateTimeNow();
            
            decimal vatPercent = await _taxPercentService.GetVatTaxByStoreId(productRequestHeader.StoreId, (DateTime)currentDate);

            var productRequestPriceDetails = await GetProductRequestPriceDetailFromProductRequest(productRequestHeaderId, (DateTime)currentDate, vatPercent, productRequestHeader.StoreId);

			var totalValueFromDetail = productRequestPriceDetails.Sum(x => x.TotalValue);
            var grossValueFromDetail = productRequestPriceDetails.Sum(x => x.GrossValue);
            var vatValueFromDetail = productRequestPriceDetails.Sum(x => x.VatValue);
            var subNetValueFromDetail = productRequestPriceDetails.Sum(x => x.SubNetValue);
            var otherTaxValueFromDetail = productRequestPriceDetails.Sum(x => x.OtherTaxValue);
            var netValueFromDetail = productRequestPriceDetails.Sum(x => x.NetValue);
            var totalCostValueFromDetail = productRequestPriceDetails.Sum(x => x.CostValue);

            var productRequestPriceHeader = new ProductRequestPriceHeaderDto
            {
                ProductRequestPriceHeaderId = 0,
                ProductRequestHeaderId = productRequestHeader.ProductRequestHeaderId,
                SupplierId = 0,
                SupplierCode = 0,
                SupplierName = null,
                StoreId = productRequestHeader.StoreId,
                StoreName = productRequestHeader.StoreName,
                TaxTypeId = TaxTypeData.Taxable,
                DocumentDate = productRequestHeader.DocumentDate,
                TotalValue = totalValueFromDetail,
                DiscountPercent = 0,
                DiscountValue = 0,
                TotalItemDiscount = 0,
                GrossValue = grossValueFromDetail,
                VatValue = vatValueFromDetail,
                SubNetValue = subNetValueFromDetail,
                OtherTaxValue = otherTaxValueFromDetail,
                NetValueBeforeAdditionalDiscount = netValueFromDetail,
                AdditionalDiscountValue = 0,
                NetValue = CalculateHeaderValue.NetValue(netValueFromDetail, 0),
                TotalCostValue = totalCostValueFromDetail,
                Reference = productRequestHeader.Reference,
                RemarksAr = productRequestHeader.RemarksAr,
                RemarksEn = productRequestHeader.RemarksEn,
                IsClosed = false,
                ArchiveHeaderId = productRequestHeader.ArchiveHeaderId,
            };

			return new ProductRequestPriceDto { ProductRequestPriceHeader = productRequestPriceHeader, ProductRequestPriceDetails = productRequestPriceDetails };
        }

        private async Task<List<ProductRequestPriceDetailDto>> GetProductRequestPriceDetailFromProductRequest(int productRequestHeaderId, DateTime currentDate, decimal vatPercent, int storeId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            bool isVatInclusive = await _itemService.IsItemsVatInclusive(storeId);

            var productRequestPriceDetails = await (from productRequestDetail in _productRequestDetailService.GetAll().Where(x => x.ProductRequestHeaderId == productRequestHeaderId)
                                                    from item in _itemService.GetAll().Where(x => x.ItemId == productRequestDetail.ItemId)
                                                    from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == productRequestDetail.ItemPackageId)
                                                    from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == productRequestDetail.CostCenterId).DefaultIfEmpty()
                                                    from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.ItemId == productRequestDetail.ItemId && x.ItemPackageId == productRequestDetail.ItemPackageId).OrderByDescending(x => x.PurchaseInvoiceDetailId).Take(1).DefaultIfEmpty()
                                                    from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == productRequestDetail.ItemId && x.StoreId == storeId).DefaultIfEmpty()
                                                    from itemPacking in _itemPackingService.GetAll().Where(x => x.ItemId == productRequestDetail.ItemId && x.FromPackageId == productRequestDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                                                    select new ProductRequestPriceDetailDto()
                                                    {
                                                        CostCenterId = productRequestDetail.CostCenterId,
                                                        CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
                                                        ItemId = productRequestDetail.ItemId,
                                                        ItemCode = item.ItemCode,
                                                        ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
                                                        TaxTypeId = item.TaxTypeId,
                                                        ItemTypeId = item.ItemTypeId,
                                                        ItemPackageId = productRequestDetail.ItemPackageId,
                                                        ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
                                                        IsItemVatInclusive = isVatInclusive,
                                                        BarCode = productRequestDetail.BarCode,
                                                        Packing = itemPacking.Packing,
                                                        Quantity = productRequestDetail.Quantity,
                                                        RequestedPrice = (itemCost != null ? itemCost.CostPrice * itemPacking.Packing: 0),
                                                        TotalValue = CalculateDetailValue.TotalValue(productRequestDetail.Quantity, (itemCost != null ? itemCost.CostPrice * itemPacking.Packing: 0), false, item.TaxTypeId == TaxTypeData.Taxable ? vatPercent : 0),
                                                        ItemDiscountPercent = 0,
                                                        ItemDiscountValue = 0,
                                                        TotalValueAfterDiscount = CalculateDetailValue.TotalValueAfterDiscount(productRequestDetail.Quantity, (itemCost != null ? itemCost.CostPrice * itemPacking.Packing: 0), 0, false, item.TaxTypeId == TaxTypeData.Taxable ? vatPercent : 0),
                                                        HeaderDiscountValue = 0,
                                                        GrossValue = CalculateDetailValue.GrossValue(productRequestDetail.Quantity, (itemCost != null ? itemCost.CostPrice * itemPacking.Packing: 0), 0, 0, false, item.TaxTypeId == TaxTypeData.Taxable ? vatPercent : 0),
                                                        VatPercent = item.TaxTypeId == TaxTypeData.Taxable ? vatPercent : 0,
                                                        VatValue = CalculateDetailValue.VatValue(productRequestDetail.Quantity, (itemCost != null ? itemCost.CostPrice * itemPacking.Packing: 0), 0, (item.TaxTypeId == TaxTypeData.Taxable ? vatPercent : 0), 0, false),
                                                        SubNetValue = CalculateDetailValue.SubNetValue(productRequestDetail.Quantity, (itemCost != null ? itemCost.CostPrice * itemPacking.Packing: 0), 0, (item.TaxTypeId == TaxTypeData.Taxable ? vatPercent : 0), 0, false),
                                                        Notes = productRequestDetail.Notes,
                                                        ItemNote = productRequestDetail.ItemNote,
                                                        ConsumerPrice = item.ConsumerPrice,
                                                        CostPrice = itemCost != null ? itemCost.CostPrice : 0,
                                                        CostPackage = (itemCost != null ? itemCost.CostPrice * itemPacking.Packing: 0),
                                                        CostValue = (itemCost != null ? itemCost.CostPrice * itemPacking.Packing * productRequestDetail.Quantity : 0),
                                                        LastPurchasePrice = purchaseInvoiceDetail != null ? purchaseInvoiceDetail.PurchasePrice : 0
                                                    }).ToListAsync();

            var itemIds = productRequestPriceDetails.Select(x => x.ItemId).ToList();
            var itemTaxes = await _itemTaxService.GetItemTaxDataByItemIds(itemIds, currentDate);
            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            
            int newId = -1;
            int newSubId = -1;
            foreach (var productRequestPriceDetail in productRequestPriceDetails)
            {
                var thisItemTaxes = itemTaxes.Where(x => x.ItemId == productRequestPriceDetail.ItemId).ToList();
                
                productRequestPriceDetail.ProductRequestPriceDetailTaxes = (
                        from itemTax in thisItemTaxes
                        select new ProductRequestPriceDetailTaxDto
                        {
                            TaxId = itemTax.TaxId,
                            DebitAccountId = (int)itemTax.DebitAccountId!,
                            TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
                            TaxPercent = productRequestPriceDetail.TaxTypeId == TaxTypeData.Taxable ? itemTax.TaxPercent : 0,
                            TaxValue = CalculateDetailValue.TaxValue(productRequestPriceDetail.Quantity, productRequestPriceDetail.RequestedPrice, 0, productRequestPriceDetail.VatPercent, productRequestPriceDetail.TaxTypeId == TaxTypeData.Taxable ? itemTax.TaxPercent : 0, itemTax.TaxAfterVatInclusive, 0, false)
                        }
                    ).ToList();

                productRequestPriceDetail.OtherTaxValue = productRequestPriceDetail.ProductRequestPriceDetailTaxes.Sum(x => x.TaxValue);
                productRequestPriceDetail.NetValue = CalculateDetailValue.NetValue(productRequestPriceDetail.Quantity, productRequestPriceDetail.RequestedPrice, 0, productRequestPriceDetail.VatPercent, productRequestPriceDetail.OtherTaxValue, 0, false);

                productRequestPriceDetail.ItemTaxData = thisItemTaxes.ToList();
                productRequestPriceDetail.Taxes = thisItemTaxes.ToJson();
                productRequestPriceDetail.Packages = packages.Where(x => x.ItemId == productRequestPriceDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();

                productRequestPriceDetail.ProductRequestPriceDetailId = newId;
                productRequestPriceDetail.ProductRequestPriceDetailTaxes.ForEach(x =>
                {
                    x.ProductRequestPriceDetailId = newId;
                    x.ProductRequestPriceDetailTaxId = newSubId--;
                });
                newId--;
            }

            return productRequestPriceDetails;
        }

        public async Task<ResponseDto> SaveProductRequestPrice(ProductRequestPriceDto productRequestPrice, bool hasApprove, bool approved, int? requestId)
        {
            TrimDetailStrings(productRequestPrice.ProductRequestPriceDetails);
            if (productRequestPrice.ProductRequestPriceHeader != null)
			{
				var itemNoteValidationResult = await _itemNoteValidationService.CheckItemNoteWithItemType(productRequestPrice.ProductRequestPriceDetails, x => x.ItemId, x => x.ItemNote);
				if (itemNoteValidationResult.Success == false) return itemNoteValidationResult;

				if (productRequestPrice.ProductRequestPriceHeader.ProductRequestPriceHeaderId == 0)
                {
                    await UpdateModelPrices(productRequestPrice);
                }

                var result = await _productRequestPriceHeaderService.SaveProductRequestPriceHeader(productRequestPrice.ProductRequestPriceHeader, hasApprove, approved, requestId);
                if (result.Success)
                {
                    var modifiedProductRequestPriceDetails = await _productRequestPriceDetailService.SaveProductRequestPriceDetails(result.Id, productRequestPrice.ProductRequestPriceDetails);
                    await SaveProductRequestPriceDetailTaxes(result.Id, modifiedProductRequestPriceDetails);
                    await _productRequestPriceDetailService.DeleteProductRequestPriceDetailList(modifiedProductRequestPriceDetails, result.Id);
                    if (productRequestPrice.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(productRequestPrice.MenuNotes, result.Id);
                    }
                }

                return result;
            }
            return new ResponseDto{ Message = "Header should not be null" };
		}

		private void TrimDetailStrings(List<ProductRequestPriceDetailDto> productRequestPriceDetails)
		{
			foreach (var productRequestPriceDetail in productRequestPriceDetails)
			{
				productRequestPriceDetail.ItemNote = string.IsNullOrWhiteSpace(productRequestPriceDetail.ItemNote) ? null : productRequestPriceDetail.ItemNote.Trim();
			}
		}

		private async Task UpdateModelPrices(ProductRequestPriceDto productRequestPrice)
        {
            await UpdateDetailPrices(productRequestPrice.ProductRequestPriceDetails, productRequestPrice.ProductRequestPriceHeader!.StoreId);

            productRequestPrice.ProductRequestPriceHeader!.TotalCostValue = productRequestPrice.ProductRequestPriceDetails.Sum(x => x.CostValue);
        }

        private async Task UpdateDetailPrices(List<ProductRequestPriceDetailDto> productRequestPriceDetails, int storeId)
        {
            var itemIds = productRequestPriceDetails.Select(x => x.ItemId).ToList();
            
            var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId) && x.StoreId == storeId).Select(x => new {x.StoreId, x.ItemId, x.CostPrice}).ToListAsync();
            var consumerPrices = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).Select(x => new {x.ItemId, x.ConsumerPrice}).ToListAsync();
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

            foreach (var productRequestPriceDetail in productRequestPriceDetails)
            {
                var packing = packings.Where(x => x.ItemId == productRequestPriceDetail.ItemId && x.FromPackageId == productRequestPriceDetail.ItemPackageId).Select(x => x.Packing).FirstOrDefault(1);

                productRequestPriceDetail.ConsumerPrice = consumerPrices.Where(x => x.ItemId == productRequestPriceDetail.ItemId).Select(x => x.ConsumerPrice).FirstOrDefault(0);
                productRequestPriceDetail.CostPrice = itemCosts.Where(x => x.ItemId == productRequestPriceDetail.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefault(0);
                productRequestPriceDetail.CostPackage = productRequestPriceDetail.CostPrice * packing;
                productRequestPriceDetail.CostValue = productRequestPriceDetail.CostPackage * productRequestPriceDetail.Quantity;
                productRequestPriceDetail.LastPurchasePrice = lastPurchasePrices.Where(x => x.ItemId == productRequestPriceDetail.ItemId && x.ItemPackageId == productRequestPriceDetail.ItemPackageId).Select(x => x.PurchasePrice).FirstOrDefault(0);
            }
        }

        private async Task SaveProductRequestPriceDetailTaxes(int productRequestPriceHeaderId, List<ProductRequestPriceDetailDto> productRequestPriceDetails)
        {
            List<ProductRequestPriceDetailTaxDto> productRequestPriceDetailTaxes = new List<ProductRequestPriceDetailTaxDto>();

            foreach(var productRequestPriceDetail in productRequestPriceDetails)
            {
                foreach(var productRequestPriceDetailTax in productRequestPriceDetail.ProductRequestPriceDetailTaxes)
                {
                    productRequestPriceDetailTax.ProductRequestPriceDetailId = productRequestPriceDetail.ProductRequestPriceDetailId;
                    productRequestPriceDetailTaxes.Add(productRequestPriceDetailTax);
                }
            }

            await _productRequestPriceDetailTaxService.SaveProductRequestPriceDetailTaxes(productRequestPriceHeaderId, productRequestPriceDetailTaxes);
        }

        public async Task<ResponseDto> DeleteProductRequestPrice(int productRequestPriceHeaderId)
        {
            await _menuNoteService.DeleteMenuNotes(MenuCodeData.ProductRequestPrice, productRequestPriceHeaderId);
            await _productRequestPriceDetailTaxService.DeleteProductRequestPriceDetailTaxes(productRequestPriceHeaderId);
            await _productRequestPriceDetailService.DeleteProductRequestPriceDetails(productRequestPriceHeaderId);
            var result = await _productRequestPriceHeaderService.DeleteProductRequestPriceHeader(productRequestPriceHeaderId);
            return result;
        }
    }
}
