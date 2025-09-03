using Purchases.CoreOne.Contracts;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Extensions;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Inventory;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using Purchases.CoreOne.Models.Domain;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.Domain.Menus;

namespace Purchases.Service.Services
{
    public class StockInHandlingService: IStockInHandlingService
    {
        private readonly IPurchaseMessageService _purchaseMessageService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly IStockInService _stockInService;
        private readonly IStockInQuantityService _stockInQuantityService;
        private readonly IStockInHeaderService _stockInHeaderService;
        private readonly IStockInDetailService _stockInDetailService;
        private readonly IStockInDetailTaxService _stockInDetailTaxService;
        private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
        private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;
        private readonly IPurchaseOrderDetailTaxService _purchaseOrderDetailTaxService;
        private readonly IPurchaseOrderStatusService _purchaseOrderStatusService;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
        private readonly IPurchaseInvoiceDetailTaxService _purchaseInvoiceDetailTaxService;
        private readonly IPurchaseInvoiceService _purchaseInvoiceService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly ITaxService _taxService;
        private readonly IItemService _itemService;
        private readonly IItemTaxService _itemTaxService;
        private readonly ITaxPercentService _taxPercentService;
        private readonly IItemPackingService _itemPackingService;
        private readonly IItemPackageService _itemPackageService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IItemCostService _itemCostService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemNoteValidationService _itemNoteValidationService;
        
        public StockInHandlingService(IPurchaseMessageService purchaseMessageService, IGenericMessageService genericMessageService, IStockInService stockInService, IStockInQuantityService stockInQuantityService, IStockInHeaderService stockInHeaderService, IStockInDetailService stockInDetailService, IStockInDetailTaxService stockInDetailTaxService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IPurchaseOrderDetailService purchaseOrderDetailService, IPurchaseOrderDetailTaxService purchaseOrderDetailTaxService, IPurchaseOrderStatusService purchaseOrderStatusService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IPurchaseInvoiceDetailTaxService purchaseInvoiceDetailTaxService, IPurchaseInvoiceService purchaseInvoiceService, IMenuNoteService menuNoteService, ITaxService taxService, IItemService itemService, IItemTaxService itemTaxService, ITaxPercentService taxPercentService, IItemPackingService itemPackingService, IItemPackageService itemPackageService, IItemBarCodeService itemBarCodeService, IItemCostService itemCostService, IHttpContextAccessor httpContextAccessor, IItemNoteValidationService itemNoteValidationService) 
        {
            _purchaseMessageService = purchaseMessageService;
            _genericMessageService = genericMessageService;
            _stockInService = stockInService;
            _stockInQuantityService = stockInQuantityService;
            _stockInHeaderService = stockInHeaderService;
            _stockInDetailService = stockInDetailService;
            _stockInDetailTaxService = stockInDetailTaxService;
            _purchaseOrderHeaderService = purchaseOrderHeaderService;
            _purchaseOrderDetailService = purchaseOrderDetailService;
            _purchaseOrderDetailTaxService = purchaseOrderDetailTaxService;
            _purchaseOrderStatusService = purchaseOrderStatusService;
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _purchaseInvoiceDetailService = purchaseInvoiceDetailService;
            _purchaseInvoiceDetailTaxService = purchaseInvoiceDetailTaxService;
            _purchaseInvoiceService = purchaseInvoiceService;
            _menuNoteService = menuNoteService;
            _taxService = taxService;
            _itemService = itemService;
            _itemTaxService = itemTaxService;
            _taxPercentService = taxPercentService;
            _itemPackingService = itemPackingService;
            _itemPackageService = itemPackageService;
            _itemBarCodeService = itemBarCodeService;
            _itemCostService = itemCostService;
            _httpContextAccessor = httpContextAccessor;
            _itemNoteValidationService = itemNoteValidationService;
        }

        public IQueryable<StockInHeaderDto> GetStockInHeadersByStoreId(int storeId, int? supplierId, int stockTypeId, int stockInHeaderId)
        {
            supplierId ??= 0;
            if (stockInHeaderId == 0)
            {
                return from stockInHeader in _stockInHeaderService.GetStockInHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.StockTypeId == stockTypeId && x.IsEnded == false && x.IsBlocked == false)
                       from overallQuantityReceived in _stockInQuantityService.GetOverallQuantityAvailableReturnFromStockIns().Where(x => x.ParentId == stockInHeader.StockInHeaderId && x.Quantity > 0)
                       select stockInHeader;
            }
            else
            {
                return _stockInHeaderService.GetStockInHeaders().Where(x => x.StockInHeaderId == stockInHeaderId);
            }
        }

        public async Task<StockInDto> GetStockIn(int stockInHeaderId)
        {
            var header = await _stockInHeaderService.GetStockInHeaderById(stockInHeaderId);
            if (header == null) { return new StockInDto(); }

            var details = await GetStockInDetailsCalculated(stockInHeaderId, header.PurchaseOrderHeaderId, header.PurchaseInvoiceHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(StockTypeData.ToMenuCode(header.StockTypeId), stockInHeaderId).ToListAsync();
            var stockInDetailTaxes = await _stockInDetailTaxService.GetStockInDetailTaxes(stockInHeaderId).ToListAsync();

            foreach (var detail in details)
            {
                detail.StockInDetailTaxes = stockInDetailTaxes.Where(x => x.StockInDetailId == detail.StockInDetailId).ToList();
            }

            return new StockInDto() { StockInHeader = header, StockInDetails = details, MenuNotes = menuNotes };
        }

        public async Task<StockInDto> GetStockInFromPurchaseOrder(int purchaseOrderHeaderId)
        {
            var purchaseOrderHeader = await _purchaseOrderHeaderService.GetPurchaseOrderHeaderById(purchaseOrderHeaderId);
            if (purchaseOrderHeader == null)
            {
                return new StockInDto();
            }

            var purchaseOrderDetails = await _purchaseOrderDetailService.GetPurchaseOrderDetailsAsQueryable(purchaseOrderHeaderId).ToListAsync();
            var purchaseOrderDetaisGrouped = _purchaseOrderDetailService.GroupPurchaseOrderDetails(purchaseOrderDetails);

            var itemIds = purchaseOrderDetails.Select(x => x.ItemId).ToList();

            var finalStocksReceived = await _stockInQuantityService.GetFinalStocksReceivedFromPurchaseOrder(purchaseOrderHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

            var items = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var itemPackings = await _itemPackingService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var lastPurchasePrices = await _purchaseInvoiceService.GetMultipleLastPurchasePrices(itemIds);

            var stockInDetails = (
                                    from purchaseOrderDetail in purchaseOrderDetails
                                    from item in items.Where(x => x.ItemId == purchaseOrderDetail.ItemId)
                                    from itemCost in itemCosts.Where(x => x.ItemId == purchaseOrderDetail.ItemId && x.StoreId == purchaseOrderHeader.StoreId).DefaultIfEmpty()
                                    from itemPacking in itemPackings.Where(x => x.ItemId == purchaseOrderDetail.ItemId && x.FromPackageId == purchaseOrderDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                                    from finalStockReceived in finalStocksReceived.Where(x => x.ItemId == purchaseOrderDetail.ItemId && x.ItemPackageId == purchaseOrderDetail.ItemPackageId && x.BarCode == purchaseOrderDetail.BarCode && x.PurchasePrice == purchaseOrderDetail.PurchasePrice && x.ItemDiscountPercent == purchaseOrderDetail.ItemDiscountPercent).DefaultIfEmpty()
                                    from purchaseOrderGroup in purchaseOrderDetaisGrouped.Where(x => x.ItemId == purchaseOrderDetail.ItemId && x.ItemPackageId == purchaseOrderDetail.ItemPackageId && x.BarCode == purchaseOrderDetail.BarCode && x.PurchasePrice == purchaseOrderDetail.PurchasePrice && x.ItemDiscountPercent == purchaseOrderDetail.ItemDiscountPercent)
									from lastPurchasePrice in lastPurchasePrices.Where(x => x.ItemId == purchaseOrderDetail.ItemId && x.ItemPackageId == purchaseOrderDetail.ItemPackageId).DefaultIfEmpty()
                                    select new StockInDetailDto()
                                    {
                                        StockInDetailId = purchaseOrderDetail.PurchaseOrderDetailId, // <-- This is used to get the related detail taxes
                                        CostCenterId = purchaseOrderDetail.CostCenterId,
                                        CostCenterName = purchaseOrderDetail.CostCenterName,
                                        ItemId = purchaseOrderDetail.ItemId,
                                        ItemCode = purchaseOrderDetail.ItemCode,
                                        ItemName = purchaseOrderDetail.ItemName,
                                        TaxTypeId = purchaseOrderDetail.TaxTypeId,
                                        ItemTypeId = purchaseOrderDetail.ItemTypeId,
                                        ItemPackageId = purchaseOrderDetail.ItemPackageId,
                                        ItemPackageName = purchaseOrderDetail.ItemPackageName,
                                        IsItemVatInclusive = purchaseOrderDetail.IsItemVatInclusive,
                                        BarCode = purchaseOrderDetail.BarCode,
                                        Packing = purchaseOrderDetail.Packing,
                                        Quantity = purchaseOrderDetail.Quantity,
                                        BonusQuantity = purchaseOrderDetail.BonusQuantity,
                                        PurchaseOrderQuantity = purchaseOrderGroup.Quantity,
                                        PurchaseOrderBonusQuantity = purchaseOrderGroup.BonusQuantity,
                                        AvailableQuantity = purchaseOrderGroup.Quantity - (finalStockReceived != null ? finalStockReceived.QuantityReceived : 0),
                                        AvailableBonusQuantity = purchaseOrderGroup.BonusQuantity - (finalStockReceived != null ? finalStockReceived.BonusQuantityReceived : 0),
                                        PurchasePrice = purchaseOrderDetail.PurchasePrice,
                                        ItemDiscountPercent = purchaseOrderDetail.ItemDiscountPercent,
                                        VatPercent = purchaseOrderDetail.VatPercent,
                                        Notes = purchaseOrderDetail.Notes,
                                        ItemNote = purchaseOrderDetail.ItemNote,
                                        ConsumerPrice = item.ConsumerPrice,
                                        CostPrice = itemCost != null ? itemCost.CostPrice : 0,
                                        CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
                                        LastPurchasePrice = lastPurchasePrice != null ? lastPurchasePrice.PurchasePrice : 0,
                                    }).ToList();

            stockInDetails = DistributeQuantityAndCalculateValues(stockInDetails, purchaseOrderHeader.DiscountPercent, false);
            stockInDetails = stockInDetails.Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();

            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);
            var purchaseOrderDetailTaxes = await _purchaseOrderDetailTaxService.GetPurchaseOrderDetailTaxes(purchaseOrderHeaderId).ToListAsync();
            int newId = -1;
            int newSubId = -1;
            foreach (var stockInDetail in stockInDetails)
            {
                stockInDetail.Packages = packages.Where(x => x.ItemId == stockInDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                stockInDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == stockInDetail.ItemId).ToList();
                stockInDetail.Taxes = stockInDetail.ItemTaxData.ToJson();

                stockInDetail.StockInDetailTaxes = (
                    from itemTax in purchaseOrderDetailTaxes.Where(x => x.PurchaseOrderDetailId == stockInDetail.StockInDetailId)
                    select new StockInDetailTaxDto
                    {
                        TaxId = itemTax.TaxId,
                        DebitAccountId = itemTax.DebitAccountId,
                        TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
                        TaxPercent = itemTax.TaxPercent,
                        TaxValue = CalculateDetailValue.TaxValue(stockInDetail.Quantity, stockInDetail.PurchasePrice, stockInDetail.ItemDiscountPercent, stockInDetail.VatPercent, itemTax.TaxPercent, itemTax.TaxAfterVatInclusive, purchaseOrderHeader.DiscountPercent, false)
                    }
                ).ToList();

                stockInDetail.OtherTaxValue = stockInDetail.StockInDetailTaxes.Sum(x => x.TaxValue);
                stockInDetail.NetValue = CalculateDetailValue.NetValue(stockInDetail.Quantity, stockInDetail.PurchasePrice, stockInDetail.ItemDiscountPercent, stockInDetail.VatPercent, stockInDetail.OtherTaxValue, purchaseOrderHeader.DiscountPercent, false);

                foreach (var stockInDetailTax in stockInDetail.StockInDetailTaxes)
                {
                    stockInDetailTax.StockInDetailId = newId;
                    stockInDetailTax.StockInDetailTaxId = newSubId--;
                }
                stockInDetail.StockInDetailId = newId;
                newId--;
            }

            var totalValueFromDetail = stockInDetails.Sum(x => x.TotalValue);
            var totalValueAfterDiscountFromDetail = stockInDetails.Sum(x => x.TotalValueAfterDiscount);
            var totalItemDiscount = stockInDetails.Sum(x => x.ItemDiscountValue);
            var grossValueFromDetail = stockInDetails.Sum(x => x.GrossValue);
            var vatValueFromDetail = stockInDetails.Sum(x => x.VatValue);
            var subNetValueFromDetail = stockInDetails.Sum(x => x.SubNetValue);
            var otherTaxValueFromDetail = stockInDetails.Sum(x => x.OtherTaxValue);
            var netValueFromDetail = stockInDetails.Sum(x => x.NetValue);
            var totalCostValueFromDetail = stockInDetails.Sum(x => x.CostValue);

            var stockInHeader = new StockInHeaderDto
            {
                StockInHeaderId = 0,
                StockTypeId = StockTypeData.ReceiptStatement,
                PurchaseOrderHeaderId = purchaseOrderHeader.PurchaseOrderHeaderId,
                PurchaseOrderFullCode = purchaseOrderHeader.DocumentFullCode,
                PurchaseOrderDocumentReference = purchaseOrderHeader.DocumentReference,
                SupplierId = purchaseOrderHeader.SupplierId,
                SupplierCode = purchaseOrderHeader.SupplierCode,
                SupplierName = purchaseOrderHeader.SupplierName,
                StoreId = purchaseOrderHeader.StoreId,
                StoreName = purchaseOrderHeader.StoreName,
                DocumentDate = purchaseOrderHeader.DocumentDate,
                Reference = purchaseOrderHeader.Reference,
                TotalValue = totalValueFromDetail,
                DiscountPercent = purchaseOrderHeader.DiscountPercent,
                DiscountValue = CalculateHeaderValue.DiscountValue(totalValueAfterDiscountFromDetail, purchaseOrderHeader.DiscountPercent),
                TotalItemDiscount = totalItemDiscount,
                GrossValue = grossValueFromDetail,
                VatValue = vatValueFromDetail,
                SubNetValue = subNetValueFromDetail,
                OtherTaxValue = otherTaxValueFromDetail,
				NetValueBeforeAdditionalDiscount = netValueFromDetail,
				AdditionalDiscountValue = purchaseOrderHeader.AdditionalDiscountValue,
                NetValue = CalculateHeaderValue.NetValue(netValueFromDetail, purchaseOrderHeader.AdditionalDiscountValue),
                TotalCostValue = totalCostValueFromDetail,
                RemarksAr = purchaseOrderHeader.RemarksAr,
                RemarksEn = purchaseOrderHeader.RemarksEn,
                IsClosed = false,
                IsEnded = false,
                IsBlocked = false,
                ArchiveHeaderId = purchaseOrderHeader.ArchiveHeaderId,
            };

            return new StockInDto { StockInHeader = stockInHeader, StockInDetails = stockInDetails };
        }

        public async Task<StockInDto> GetStockInFromPurchaseInvoice(int purchaseInvoiceHeaderId)
        {
            var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaderById(purchaseInvoiceHeaderId);
            if (purchaseInvoiceHeader == null || !purchaseInvoiceHeader.IsOnTheWay)
            {
                return new StockInDto();
            }

            var purchaseInvoiceDetails = await _purchaseInvoiceDetailService.GetPurchaseInvoiceDetailsAsQueryable(purchaseInvoiceHeaderId).ToListAsync();
            var purchaseInvoiceDetailsGrouped = _purchaseInvoiceDetailService.GroupPurchaseInvoiceDetails(purchaseInvoiceDetails);

            decimal vatPercent = await _taxPercentService.GetVatTaxByStoreId(purchaseInvoiceHeader.StoreId, DateHelper.GetDateTimeNow());

            var itemIds = purchaseInvoiceDetails.Select(x => x.ItemId).ToList();

            var finalStocksReceived = await _stockInQuantityService.GetFinalStocksReceivedFromPurchaseInvoice(purchaseInvoiceHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

            var items = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var itemPackings = await _itemPackingService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var lastPurchasePrices = await _purchaseInvoiceService.GetMultipleLastPurchasePrices(itemIds);

            var stockInDetails = (
                                    from purchaseInvoiceDetail in purchaseInvoiceDetails
                                    from item in items.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId)
                                    from itemCost in itemCosts.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.StoreId == purchaseInvoiceHeader.StoreId).DefaultIfEmpty()
                                    from itemPacking in itemPackings.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.FromPackageId == purchaseInvoiceDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                                    from finalStockReceived in finalStocksReceived.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId && x.BatchNumber == purchaseInvoiceDetail.BatchNumber && x.ExpireDate == purchaseInvoiceDetail.ExpireDate && x.CostCenterId == purchaseInvoiceDetail.CostCenterId && x.BarCode == purchaseInvoiceDetail.BarCode && x.PurchasePrice == purchaseInvoiceDetail.PurchasePrice).DefaultIfEmpty()
                                    from purchaseInvoiceGroup in purchaseInvoiceDetailsGrouped.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId && x.BatchNumber == purchaseInvoiceDetail.BatchNumber && x.ExpireDate == purchaseInvoiceDetail.ExpireDate && x.CostCenterId == purchaseInvoiceDetail.CostCenterId && x.BarCode == purchaseInvoiceDetail.BarCode && x.PurchasePrice == purchaseInvoiceDetail.PurchasePrice)
                                    from lastPurchasePrice in lastPurchasePrices.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId).DefaultIfEmpty()
                                    select new StockInDetailDto()
                                    {
                                        StockInDetailId = purchaseInvoiceDetail.PurchaseInvoiceDetailId, // <-- This is used to get the related detail taxes
                                        CostCenterId = purchaseInvoiceDetail.CostCenterId,
                                        CostCenterName = purchaseInvoiceDetail.CostCenterName,
                                        ItemId = purchaseInvoiceDetail.ItemId,
                                        ItemCode = purchaseInvoiceDetail.ItemCode,
                                        ItemName = purchaseInvoiceDetail.ItemName,
                                        TaxTypeId = purchaseInvoiceDetail.TaxTypeId,
                                        ItemTypeId = purchaseInvoiceDetail.ItemTypeId,
                                        ItemPackageId = purchaseInvoiceDetail.ItemPackageId,
                                        ItemPackageName = purchaseInvoiceDetail.ItemPackageName,
                                        IsItemVatInclusive = purchaseInvoiceDetail.IsItemVatInclusive,
                                        BarCode = purchaseInvoiceDetail.BarCode,
                                        Packing = purchaseInvoiceDetail.Packing,
                                        ExpireDate = purchaseInvoiceDetail.ExpireDate,
                                        BatchNumber = purchaseInvoiceDetail.BatchNumber,
                                        Quantity = purchaseInvoiceDetail.Quantity,
                                        BonusQuantity = purchaseInvoiceDetail.BonusQuantity,
                                        PurchaseInvoiceQuantity = purchaseInvoiceGroup.Quantity,
                                        PurchaseInvoiceBonusQuantity = purchaseInvoiceGroup.BonusQuantity,
                                        AvailableQuantity = purchaseInvoiceGroup.Quantity - (finalStockReceived != null ? finalStockReceived.QuantityReceived : 0),
                                        AvailableBonusQuantity = purchaseInvoiceGroup.BonusQuantity - (finalStockReceived != null ? finalStockReceived.BonusQuantityReceived : 0),
                                        PurchasePrice = purchaseInvoiceDetail.PurchasePrice,
                                        ItemDiscountPercent = purchaseInvoiceDetail.ItemDiscountPercent,
                                        VatPercent = purchaseInvoiceDetail.VatPercent,
                                        Notes = purchaseInvoiceDetail.Notes,
                                        ItemNote = purchaseInvoiceDetail.ItemNote,
                                        ConsumerPrice = item.ConsumerPrice,
                                        CostPrice = itemCost != null ? itemCost.CostPrice : 0,
                                        CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
                                        LastPurchasePrice = lastPurchasePrice != null ? lastPurchasePrice.PurchasePrice : 0,
                                    }).ToList();

			stockInDetails = DistributeQuantityAndCalculateValues(stockInDetails, purchaseInvoiceHeader.DiscountPercent, true);
			stockInDetails = stockInDetails.Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();

			var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);
            var purchaseInvoiceDetailTaxes = await _purchaseInvoiceDetailTaxService.GetPurchaseInvoiceDetailTaxes(purchaseInvoiceHeaderId).ToListAsync();
            int newId = -1;
            int newSubId = -1;
            foreach (var stockInDetail in stockInDetails)
            {
                stockInDetail.Packages = packages.Where(x => x.ItemId == stockInDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                stockInDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == stockInDetail.ItemId).ToList();
                stockInDetail.Taxes = stockInDetail.ItemTaxData.ToJson();

                stockInDetail.StockInDetailTaxes = (
                    from itemTax in purchaseInvoiceDetailTaxes.Where(x => x.PurchaseInvoiceDetailId == stockInDetail.StockInDetailId)
                    select new StockInDetailTaxDto
                    {
                        TaxId = itemTax.TaxId,
                        DebitAccountId = itemTax.DebitAccountId,
                        TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
                        TaxPercent = itemTax.TaxPercent,
                        TaxValue = CalculateDetailValue.TaxValue(stockInDetail.Quantity, stockInDetail.PurchasePrice, stockInDetail.ItemDiscountPercent, stockInDetail.VatPercent, itemTax.TaxPercent, itemTax.TaxAfterVatInclusive, purchaseInvoiceHeader.DiscountPercent, false)
                    }
                ).ToList();

                stockInDetail.OtherTaxValue = stockInDetail.StockInDetailTaxes.Sum(x => x.TaxValue);
                stockInDetail.NetValue = CalculateDetailValue.NetValue(stockInDetail.Quantity, stockInDetail.PurchasePrice, stockInDetail.ItemDiscountPercent, stockInDetail.VatPercent, stockInDetail.OtherTaxValue, purchaseInvoiceHeader.DiscountPercent, false);

                foreach (var stockInDetailTax in stockInDetail.StockInDetailTaxes)
                {
                    stockInDetailTax.StockInDetailId = newId;
                    stockInDetailTax.StockInDetailTaxId = newSubId--;
                }
                stockInDetail.StockInDetailId = newId;
                newId--;
            }

            var totalValueFromDetail = stockInDetails.Sum(x => x.TotalValue);
            var totalValueAfterDiscountFromDetail = stockInDetails.Sum(x => x.TotalValueAfterDiscount);
            var totalItemDiscount = stockInDetails.Sum(x => x.ItemDiscountValue);
            var grossValueFromDetail = stockInDetails.Sum(x => x.GrossValue);
            var vatValueFromDetail = stockInDetails.Sum(x => x.VatValue);
            var subNetValueFromDetail = stockInDetails.Sum(x => x.SubNetValue);
            var otherTaxValueFromDetail = stockInDetails.Sum(x => x.OtherTaxValue);
            var netValueFromDetail = stockInDetails.Sum(x => x.NetValue);
            var totalCostValueFromDetail = stockInDetails.Sum(x => x.CostValue);

            var stockInHeader = new StockInHeaderDto
            {
                StockInHeaderId = 0,
                StockTypeId = StockTypeData.ReceiptFromPurchaseInvoiceOnTheWay,
                PurchaseInvoiceHeaderId = purchaseInvoiceHeader.PurchaseInvoiceHeaderId,
                PurchaseInvoiceFullCode = purchaseInvoiceHeader.DocumentFullCode,
                PurchaseInvoiceDocumentReference = purchaseInvoiceHeader.DocumentReference,
                SupplierId = purchaseInvoiceHeader.SupplierId,
                SupplierCode = purchaseInvoiceHeader.SupplierCode,
                SupplierName = purchaseInvoiceHeader.SupplierName,
                StoreId = purchaseInvoiceHeader.StoreId,
                StoreName = purchaseInvoiceHeader.StoreName,
                DocumentDate = purchaseInvoiceHeader.DocumentDate,
                Reference = purchaseInvoiceHeader.Reference,
                TotalValue = totalValueFromDetail,
                DiscountPercent = purchaseInvoiceHeader.DiscountPercent,
                DiscountValue = CalculateHeaderValue.DiscountValue(totalValueAfterDiscountFromDetail, purchaseInvoiceHeader.DiscountPercent),
                TotalItemDiscount = totalItemDiscount,
                GrossValue = grossValueFromDetail,
                VatValue = vatValueFromDetail,
                SubNetValue = subNetValueFromDetail,
                OtherTaxValue = otherTaxValueFromDetail,
                NetValueBeforeAdditionalDiscount = netValueFromDetail,
                AdditionalDiscountValue = purchaseInvoiceHeader.AdditionalDiscountValue,
                NetValue = CalculateHeaderValue.NetValue(netValueFromDetail, purchaseInvoiceHeader.AdditionalDiscountValue),
                TotalCostValue = totalCostValueFromDetail,
                RemarksAr = purchaseInvoiceHeader.RemarksAr,
                RemarksEn = purchaseInvoiceHeader.RemarksEn,
                IsClosed = false,
                IsEnded = false,
                IsBlocked = false,
                ArchiveHeaderId = purchaseInvoiceHeader.ArchiveHeaderId,
            };

            return new StockInDto { StockInHeader = stockInHeader, StockInDetails = stockInDetails };
        }

        private List<StockInDetailDto> DistributeQuantityAndCalculateValues(List<StockInDetailDto> stockInDetails, decimal headerDiscountPercent, bool useAllKeys)
        {
			QuantityDistributionLogic.DistributeQuantitiesOnDetails(
				details: stockInDetails,
				keySelector: x => (x.ItemId, x.ItemPackageId, x.BarCode, useAllKeys ? x.CostCenterId : null, x.PurchasePrice, x.ItemDiscountPercent, useAllKeys ? x.ExpireDate : null, useAllKeys ? x.BatchNumber : null),
				availableQuantitySelector: x => x.AvailableQuantity,
				availableBonusQuantitySelector: x => x.AvailableBonusQuantity,
				quantitySelector: x => x.Quantity,
				bonusQuantitySelector: x => x.BonusQuantity,
				quantityAssigner: (x, value) => x.Quantity = value,
				bonusQuantityAssigner: (x, value) => x.BonusQuantity = value
			);

			RecalculateDetailValue.RecalculateDetailValues(
				details: stockInDetails,
				quantitySelector: x => x.Quantity,
                bonusQuantitySelector: x => x.BonusQuantity,
				priceSelector: x => x.PurchasePrice,
				isItemVatInclusiveSelector: x => x.IsItemVatInclusive,
				vatPercentSelector: x => x.VatPercent,
				itemDiscountPercentSelector: x => x.ItemDiscountPercent,
                costPackageSelector: x => x.CostPackage,
				totalValueAssigner: (x, value) => x.TotalValue = value,
				itemDiscountValueAssigner: (x, value) => x.ItemDiscountValue = value,
				totalValueAfterDiscountAssigner: (x, value) => x.TotalValueAfterDiscount = value,
				headerDiscountValueAssigner: (x, value) => x.HeaderDiscountValue = value,
				grossValueAssigner: (x, value) => x.GrossValue = value,
				vatValueAssigner: (x, value) => x.VatValue = value,
				subNetValueAssigner: (x, value) => x.SubNetValue = value,
                costValueAssigner: (x, value) => x.CostValue = value,
				headerDiscountPercent: headerDiscountPercent
			);

			return stockInDetails;
        }

        public async Task<List<StockInDetailDto>> GetStockInDetailsCalculated(int stockInHeaderId, int? purchaseOrderHeaderId = null, int? purchaseInvoiceHeaderId = null, List<StockInDetailDto>? stockInDetails = null)
        {
            var stockInHeader = await _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId).Select(x => new { x.PurchaseOrderHeaderId, x.PurchaseInvoiceHeaderId }).FirstOrDefaultAsync();

            purchaseOrderHeaderId ??= stockInHeader?.PurchaseOrderHeaderId;
            purchaseInvoiceHeaderId ??= stockInHeader?.PurchaseInvoiceHeaderId;

            stockInDetails ??= await _stockInDetailService.GetStockInDetailsAsQueryable(stockInHeaderId).ToListAsync();
            var itemIds = stockInDetails.Select(x => x.ItemId).ToList();

            if (purchaseOrderHeaderId != null)
            {
                var finalStocksReceived = await _stockInQuantityService.GetFinalStocksReceivedFromPurchaseOrderExceptStockInHeaderId((int)purchaseOrderHeaderId, stockInHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
                var purchaseOrderDetaisGrouped = await _purchaseOrderDetailService.GetPurchaseOrderDetailsGrouped((int)purchaseOrderHeaderId);

				stockInDetails = (
                    from stockInDetail in stockInDetails
                    from purchaseOrderDetailGroup in purchaseOrderDetaisGrouped.Where(x => x.ItemId == stockInDetail.ItemId && x.ItemPackageId == stockInDetail.ItemPackageId && x.BarCode == stockInDetail.BarCode && x.PurchasePrice == stockInDetail.PurchasePrice && x.ItemDiscountPercent == stockInDetail.ItemDiscountPercent).DefaultIfEmpty()
                    from finalStockReceived in finalStocksReceived.Where(x => x.ItemId == stockInDetail.ItemId && x.ItemPackageId == stockInDetail.ItemPackageId && x.BarCode == stockInDetail.BarCode && x.PurchasePrice == stockInDetail.PurchasePrice && x.ItemDiscountPercent == stockInDetail.ItemDiscountPercent).DefaultIfEmpty()
                    select new StockInDetailDto
                    {
                        StockInDetailId = stockInDetail.StockInDetailId,
                        StockInHeaderId = stockInDetail.StockInHeaderId,
                        CostCenterId = stockInDetail.CostCenterId,
                        CostCenterName = stockInDetail.CostCenterName,
                        ItemId = stockInDetail.ItemId,
                        ItemCode = stockInDetail.ItemCode,
                        ItemName = stockInDetail.ItemName,
                        TaxTypeId = stockInDetail.TaxTypeId,
                        ItemTypeId = stockInDetail.ItemTypeId,
                        ItemPackageId = stockInDetail.ItemPackageId,
                        ItemPackageName = stockInDetail.ItemPackageName,
                        IsItemVatInclusive = stockInDetail.IsItemVatInclusive,
                        BarCode = stockInDetail.BarCode,
                        Packing = stockInDetail.Packing,
                        ExpireDate = stockInDetail.ExpireDate,
                        BatchNumber = stockInDetail.BatchNumber,
                        Quantity = stockInDetail.Quantity,
                        BonusQuantity = stockInDetail.BonusQuantity,
                        PurchaseOrderQuantity = purchaseOrderDetailGroup != null ? purchaseOrderDetailGroup.Quantity : 0,
                        PurchaseOrderBonusQuantity = purchaseOrderDetailGroup != null ? purchaseOrderDetailGroup.BonusQuantity : 0,
                        AvailableQuantity = (purchaseOrderDetailGroup != null ? purchaseOrderDetailGroup.Quantity : 0) - (finalStockReceived != null ? finalStockReceived.QuantityReceived : 0),
                        AvailableBonusQuantity = (purchaseOrderDetailGroup != null ? purchaseOrderDetailGroup.BonusQuantity : 0) - (finalStockReceived != null ? finalStockReceived.BonusQuantityReceived : 0),
                        PurchasePrice = stockInDetail.PurchasePrice,
                        TotalValue = stockInDetail.TotalValue,
                        ItemDiscountPercent = stockInDetail.ItemDiscountPercent,
                        ItemDiscountValue = stockInDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = stockInDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = stockInDetail.HeaderDiscountValue,
                        GrossValue = stockInDetail.GrossValue,
                        VatPercent = stockInDetail.VatPercent,
                        VatValue = stockInDetail.VatValue,
                        SubNetValue = stockInDetail.SubNetValue,
                        OtherTaxValue = stockInDetail.OtherTaxValue,
                        NetValue = stockInDetail.NetValue,
                        Notes = stockInDetail.Notes,
                        ItemNote = stockInDetail.ItemNote,
                        ConsumerPrice = stockInDetail.ConsumerPrice,
                        CostPrice = stockInDetail.CostPrice,
                        CostPackage = stockInDetail.CostPackage,
                        LastPurchasePrice = stockInDetail.LastPurchasePrice,

                        StockInDetailTaxes = stockInDetail.StockInDetailTaxes,

                        CreatedAt = stockInDetail.CreatedAt,
                        IpAddressCreated = stockInDetail.IpAddressCreated,
                        UserNameCreated = stockInDetail.UserNameCreated,
                    }).ToList();
            }
            else if (purchaseInvoiceHeaderId != null)
            {
                var finalStocksReceived = await _stockInQuantityService.GetFinalStocksReceivedFromPurchaseInvoiceExceptStockInHeaderId((int)purchaseInvoiceHeaderId, stockInHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
				var purchaseInvoiceDetaisGrouped = await _purchaseInvoiceDetailService.GetPurchaseInvoiceDetailsGrouped((int)purchaseInvoiceHeaderId);

				stockInDetails = (
                    from stockInDetail in stockInDetails
                    from purchaseInvoiceDetailGroup in purchaseInvoiceDetaisGrouped.Where(x => x.ItemId == stockInDetail.ItemId && x.ItemPackageId == stockInDetail.ItemPackageId && x.BatchNumber == stockInDetail.BatchNumber && x.ExpireDate == stockInDetail.ExpireDate && x.CostCenterId == stockInDetail.CostCenterId && x.BarCode == stockInDetail.BarCode && x.PurchasePrice == stockInDetail.PurchasePrice && x.ItemDiscountPercent == stockInDetail.ItemDiscountPercent).DefaultIfEmpty()
                    from finalstockReceived in finalStocksReceived.Where(x => x.ItemId == stockInDetail.ItemId && x.ItemPackageId == stockInDetail.ItemPackageId && x.BatchNumber == stockInDetail.BatchNumber && x.ExpireDate == stockInDetail.ExpireDate && x.CostCenterId == stockInDetail.CostCenterId && x.BarCode == stockInDetail.BarCode && x.PurchasePrice == stockInDetail.PurchasePrice && x.ItemDiscountPercent == stockInDetail.ItemDiscountPercent).DefaultIfEmpty()
                    select new StockInDetailDto
                    {
                        StockInDetailId = stockInDetail.StockInDetailId,
                        StockInHeaderId = stockInDetail.StockInHeaderId,
                        CostCenterId = stockInDetail.CostCenterId,
                        CostCenterName = stockInDetail.CostCenterName,
                        ItemId = stockInDetail.ItemId,
                        ItemCode = stockInDetail.ItemCode,
                        ItemName = stockInDetail.ItemName,
                        TaxTypeId = stockInDetail.TaxTypeId,
                        ItemTypeId = stockInDetail.ItemTypeId,
                        ItemPackageId = stockInDetail.ItemPackageId,
                        ItemPackageName = stockInDetail.ItemPackageName,
                        IsItemVatInclusive = stockInDetail.IsItemVatInclusive,
                        BarCode = stockInDetail.BarCode,
                        Packing = stockInDetail.Packing,
                        ExpireDate = stockInDetail.ExpireDate,
                        BatchNumber = stockInDetail.BatchNumber,
                        Quantity = stockInDetail.Quantity,
                        BonusQuantity = stockInDetail.BonusQuantity,
                        PurchaseInvoiceQuantity = purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.Quantity : 0,
                        PurchaseInvoiceBonusQuantity = purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.BonusQuantity : 0,
                        AvailableQuantity = (purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.Quantity : 0) - (finalstockReceived != null ? finalstockReceived.QuantityReceived : 0),
                        AvailableBonusQuantity = (purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.BonusQuantity : 0) - (finalstockReceived != null ? finalstockReceived.BonusQuantityReceived : 0),
                        PurchasePrice = stockInDetail.PurchasePrice,
                        TotalValue = stockInDetail.TotalValue,
                        ItemDiscountPercent = stockInDetail.ItemDiscountPercent,
                        ItemDiscountValue = stockInDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = stockInDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = stockInDetail.HeaderDiscountValue,
                        GrossValue = stockInDetail.GrossValue,
                        VatPercent = stockInDetail.VatPercent,
                        VatValue = stockInDetail.VatValue,
                        SubNetValue = stockInDetail.SubNetValue,
                        OtherTaxValue = stockInDetail.OtherTaxValue,
                        NetValue = stockInDetail.NetValue,
                        Notes = stockInDetail.Notes,
                        ItemNote = stockInDetail.ItemNote,
                        ConsumerPrice = stockInDetail.ConsumerPrice,
                        CostPrice = stockInDetail.CostPrice,
                        CostPackage = stockInDetail.CostPackage,
                        LastPurchasePrice = stockInDetail.LastPurchasePrice,

						StockInDetailTaxes = stockInDetail.StockInDetailTaxes,

						CreatedAt = stockInDetail.CreatedAt,
                        IpAddressCreated = stockInDetail.IpAddressCreated,
                        UserNameCreated = stockInDetail.UserNameCreated,
                    }).ToList();
            }

            if (stockInDetails == null) return new List<StockInDetailDto>();

            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var stockInDetail in stockInDetails)
            {
                stockInDetail.Packages = packages.Where(x => x.ItemId == stockInDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                stockInDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == stockInDetail.ItemId).ToList();
                stockInDetail.Taxes = stockInDetail.ItemTaxData.ToJson();
            }

            return stockInDetails;
        }

        public async Task<ResponseDto> SaveStockIn(StockInDto stockIn, bool hasApprove, bool approved, int? requestId)
        {
            TrimDetailStrings(stockIn.StockInDetails);

            var menuCode = StockTypeData.ToMenuCode(stockIn.StockInHeader!.StockTypeId);
            var parentMenuCode = await GetParentMenuCode(stockIn.StockInHeader!.PurchaseOrderHeaderId, stockIn.StockInHeader.PurchaseInvoiceHeaderId);

			var stockInValidResult = await CheckStockInIsValidForSave(stockIn, menuCode, parentMenuCode);
            if (!stockInValidResult.Success)
            {
                return stockInValidResult;
            }

            if (stockIn.StockInHeader!.StockInHeaderId == 0)
            {
                await StockInUpdateModelPrices(stockIn);
            }

            var result = await _stockInService.SaveStockIn(stockIn, hasApprove, approved, requestId);
            if (result.Success)
            {
                if (stockIn.StockInHeader?.StockTypeId == StockTypeData.ReceiptStatement)
                {
                    await _purchaseOrderHeaderService.UpdateClosed(stockIn.StockInHeader?.PurchaseOrderHeaderId, true);
                }
                else
                {
                    await _purchaseInvoiceHeaderService.UpdateClosed(stockIn.StockInHeader?.PurchaseInvoiceHeaderId, true);
                }

                await UpdatePurchaseOrderStatusFromStockIn(stockIn.StockInHeader!.PurchaseOrderHeaderId, stockIn.StockInHeader.PurchaseInvoiceHeaderId);
            }
            return result;
        }

        private async Task<int> GetParentMenuCode(int? purchaseOrderHeaderId, int? purchaseInvoiceHeaderId)
        {
            if (purchaseOrderHeaderId != null)
            {
                return MenuCodeData.PurchaseOrder;
            }
            else
            {
                var isCredit = await _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.CreditPayment).FirstOrDefaultAsync();
                return isCredit ? MenuCodeData.PurchaseInvoiceOnTheWayCredit : MenuCodeData.PurchaseInvoiceOnTheWayCash;
            }
        }

        private async Task<ResponseDto> CheckStockInIsValidForSave(StockInDto stockIn, int menuCode, int parentMenuCode)
        {
            ResponseDto result;
            if (stockIn.StockInHeader?.StockInHeaderId != 0)
            {
                result = await CheckStockInClosed(stockIn.StockInHeader?.StockInHeaderId ?? 0, menuCode);
                if (result.Success == false) return result;
            }

            result = await CheckStockInEnded(stockIn.StockInHeader!.StockInHeaderId, stockIn.StockInHeader!.PurchaseOrderHeaderId, stockIn.StockInHeader!.PurchaseInvoiceHeaderId, menuCode);
            if (result.Success == false) return result;


            result = await CheckPurchaseOrderOrPurchaseInvoiceBlocked(stockIn.StockInHeader!.PurchaseOrderHeaderId, stockIn.StockInHeader!.PurchaseInvoiceHeaderId, menuCode);
            if (result.Success == false) return result;


			result = await _itemNoteValidationService.CheckItemNoteWithItemType(stockIn.StockInDetails, x => x.ItemId, x => x.ItemNote);
			if (result.Success == false) return result;


			result = await CheckStockInQuantityForSaving(stockIn, menuCode, parentMenuCode);
            if (result.Success == false) return result;

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckStockInClosed(int stockInHeaderId, int menuCode)
        {
            var isClosed = await _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId).Select(x => x.IsClosed).FirstOrDefaultAsync();
            if (isClosed)
            {
                return new ResponseDto { Id = stockInHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotUpdateBecauseClosed) };
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckStockInEnded(int stockInHeaderId, int? purchaseOrderHeaderId, int? purchaseInvoiceHeaderId, int menuCode)
        {
            var isEnded = await (
                        from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).DefaultIfEmpty()
                        from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).DefaultIfEmpty()
                        select (purchaseOrderHeader != null && purchaseOrderHeader.IsEnded) || (purchaseInvoiceHeader != null && purchaseInvoiceHeader.IsEnded)
                    ).FirstOrDefaultAsync();
			if (isEnded)
			{
                return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseInvoiced) };
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckPurchaseOrderOrPurchaseInvoiceBlocked(int? purchaseOrderHeaderId, int? purchaseInvoiceHeaderId, int menuCode)
        {
            var isBlocked = await (
                from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).DefaultIfEmpty()
                from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).DefaultIfEmpty()
                select (purchaseOrderHeader != null && purchaseOrderHeader.IsBlocked) || (purchaseInvoiceHeader != null && purchaseInvoiceHeader.IsBlocked)
                ).FirstOrDefaultAsync();

            if (isBlocked)
            {
                return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckStockInQuantityForSaving(StockInDto stockIn, int menuCode, int parentMenuCode)
        {
            ResponseDto quantityExceeded;
            if (stockIn.StockInHeader?.StockTypeId == StockTypeData.ReceiptStatement)
            {
                quantityExceeded = await CheckStockInFromPurchaseOrderQuantityExceeding(stockIn, menuCode, parentMenuCode);
            }
            else
            {
                quantityExceeded = await CheckStockInFromPurchaseInvoiceQuantityExceeding(stockIn, menuCode, parentMenuCode);
            }

            if (quantityExceeded.Success == false)
            {
                return quantityExceeded;
            }

            return new ResponseDto { Success = true };
        }

        private void TrimDetailStrings(List<StockInDetailDto> stockInDetails)
        {
            foreach (var stockInDetail in stockInDetails)
            {
				stockInDetail.BatchNumber = string.IsNullOrWhiteSpace(stockInDetail.BatchNumber) ? null : stockInDetail.BatchNumber.Trim();
				stockInDetail.ItemNote = string.IsNullOrWhiteSpace(stockInDetail.ItemNote) ? null : stockInDetail.ItemNote.Trim();
            }
        }

        private async Task StockInUpdateModelPrices(StockInDto stockIn)
        {
            await StockInUpdateDetailPrices(stockIn.StockInDetails, stockIn.StockInHeader!.StoreId);
            stockIn.StockInHeader!.TotalCostValue = stockIn.StockInDetails.Sum(x => x.CostValue);
        }

        private async Task StockInUpdateDetailPrices(List<StockInDetailDto> stockInDetails, int storeId)
        {
            var itemIds = stockInDetails.Select(x => x.ItemId).ToList();

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

            foreach (var stockInDetail in stockInDetails)
            {
                var packing = packings.Where(x => x.ItemId == stockInDetail.ItemId && x.FromPackageId == stockInDetail.ItemPackageId).Select(x => x.Packing).FirstOrDefault(1);

                stockInDetail.ConsumerPrice = consumerPrices.Where(x => x.ItemId == stockInDetail.ItemId).Select(x => x.ConsumerPrice).FirstOrDefault(0);
                stockInDetail.CostPrice = itemCosts.Where(x => x.ItemId == stockInDetail.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefault(0);
                stockInDetail.CostPackage = stockInDetail.CostPrice * packing;
                stockInDetail.CostValue = stockInDetail.CostPackage * (stockInDetail.Quantity + stockInDetail.BonusQuantity);
                stockInDetail.LastPurchasePrice = lastPurchasePrices.Where(x => x.ItemId == stockInDetail.ItemId && x.ItemPackageId == stockInDetail.ItemPackageId).Select(x => x.PurchasePrice).FirstOrDefault(0);
            }
        }

        private async Task UpdatePurchaseOrderStatusFromStockIn(int? purchaseOrderHeaderId, int? purchaseInvoiceHeaderId)
        {
            var affectedPurchaseOrderHeaderId = await GetPurchaseOrderRelatedToStockIn(purchaseOrderHeaderId, purchaseInvoiceHeaderId);
            await _purchaseOrderStatusService.UpdatePurchaseOrderStatus(affectedPurchaseOrderHeaderId, -1, purchaseOrderHeaderId != null ? MenuCodeData.ReceiptStatement : MenuCodeData.ReceiptFromPurchaseInvoiceOnTheWay);
        }

        private async Task<int> GetPurchaseOrderRelatedToStockIn(int? purchaseOrderHeaderId, int? purchaseInvoiceHeaderId)
        {
            if (purchaseOrderHeaderId != null) return (int)purchaseOrderHeaderId;

            return await _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.PurchaseOrderHeaderId).FirstOrDefaultAsync();
        }

        public async Task<ResponseDto> DeleteStockIn(int stockInHeaderId, int menuCode)
        {
            var stockInHeader = await _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId).Select(x => new { x.PurchaseOrderHeaderId, x.PurchaseInvoiceHeaderId, x.IsBlocked, x.StockTypeId }).FirstOrDefaultAsync();
            if (stockInHeader == null) return new ResponseDto { Id = stockInHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };

            ResponseDto stockInValidResult = await CheckStockInIsValidForDelete(stockInHeaderId, stockInHeader.PurchaseOrderHeaderId, stockInHeader.PurchaseInvoiceHeaderId, stockInHeader.IsBlocked, menuCode);
            if(stockInValidResult.Success == false) return stockInValidResult;

            ResponseDto result = await _stockInService.DeleteStockIn(stockInHeaderId, menuCode);

            if (result.Success)
            {
                await ReopenStockInParent(stockInHeader.PurchaseOrderHeaderId, stockInHeader.PurchaseInvoiceHeaderId);
                await UpdatePurchaseOrderStatusFromStockIn(stockInHeader.PurchaseOrderHeaderId, stockInHeader.PurchaseInvoiceHeaderId);
            }

            return result;
        }

        private async Task<ResponseDto> CheckStockInIsValidForDelete(int stockInHeaderId, int? purchaseOrderHeaderId, int? purchaseInvoiceHeaderId, bool isBlocked, int menuCode)
        {
            if (isBlocked == true)
            {
                return new ResponseDto { Id = stockInHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
            }

            ResponseDto isEnded = await CheckStockInEnded(stockInHeaderId, purchaseOrderHeaderId, purchaseInvoiceHeaderId, menuCode);
            if (isEnded.Success == false) return isEnded;

            return new ResponseDto { Success = true };
        }

        private async Task ReopenStockInParent(int? purchaseOrderHeaderId, int? purchaseInvoiceHeaderId)
        {
            //If there are no other stockIn linked to the parent, reopen the parent
            if (purchaseOrderHeaderId != null)
            {
                var isStocksRemaining = await _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).AnyAsync();
                if (!isStocksRemaining)
                {
                    await _purchaseOrderHeaderService.UpdateClosed(purchaseOrderHeaderId, false);
                }
            }
            else if (purchaseInvoiceHeaderId != null)
            {
                var isStocksRemaining = await _stockInHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).AnyAsync();
                if (!isStocksRemaining)
                {
                    await _purchaseInvoiceHeaderService.UpdateClosed(purchaseInvoiceHeaderId, false);
                }
            }
        }

        private async Task<ResponseDto> CheckStockInFromPurchaseOrderQuantityExceeding(StockInDto stockIn, int menuCode, int parentMenuCode)
        {
            if (stockIn.StockInHeader == null || !(stockIn.StockInHeader.PurchaseOrderHeaderId > 0)) return new ResponseDto { Message = "StockType mismatches parent foreign keys"};

			var groupedStockInDetails = _stockInDetailService.GroupStockInDetails(stockIn.StockInDetails);
			var itemIds = groupedStockInDetails.Select(x => x.ItemId).ToList();

            List<StockReceivedFromPurchaseOrderDto>? finalstocksReceived;
            if (stockIn.StockInHeader.StockInHeaderId == 0)
            {
                finalstocksReceived = await _stockInQuantityService.GetFinalStocksReceivedFromPurchaseOrder((int)stockIn.StockInHeader.PurchaseOrderHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            }
            else
            {
                finalstocksReceived = await _stockInQuantityService.GetFinalStocksReceivedFromPurchaseOrderExceptStockInHeaderId((int)stockIn.StockInHeader.PurchaseOrderHeaderId, stockIn.StockInHeader.StockInHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            }

			var purchaseOrderDetaisGrouped = await _purchaseOrderDetailService.GetPurchaseOrderDetailsGrouped((int)stockIn.StockInHeader.PurchaseOrderHeaderId);

			var availableQuantities = (from stockInDetail in groupedStockInDetails
									   from finalstockReceived in finalstocksReceived.Where(x => x.ItemId == stockInDetail.ItemId && x.ItemPackageId == stockInDetail.ItemPackageId && x.BarCode == stockInDetail.BarCode && x.PurchasePrice == stockInDetail.PurchasePrice && x.ItemDiscountPercent == stockInDetail.ItemDiscountPercent).DefaultIfEmpty()
                                       from purchaseOrderDetailGroup in purchaseOrderDetaisGrouped.Where(x => x.ItemId == stockInDetail.ItemId && x.ItemPackageId == stockInDetail.ItemPackageId && x.BarCode == stockInDetail.BarCode && x.PurchasePrice == stockInDetail.PurchasePrice && x.ItemDiscountPercent == stockInDetail.ItemDiscountPercent).DefaultIfEmpty()
                                       select new
                                       {
                                           stockInDetail.ItemId,
                                           stockInDetail.ItemPackageId,
                                           PurchaseOrderQuantity = purchaseOrderDetailGroup != null ? purchaseOrderDetailGroup.Quantity : 0,
                                           PurchaseOrderBonusQuantity = purchaseOrderDetailGroup != null ? purchaseOrderDetailGroup.BonusQuantity : 0,
                                           QuantityReceived = (finalstockReceived != null ? finalstockReceived.QuantityReceived : 0) + stockInDetail.Quantity,
                                           BonusQuantityReceived = (finalstockReceived != null ? finalstockReceived.BonusQuantityReceived : 0) + stockInDetail.BonusQuantity,
                                           QuantityAvailable = (purchaseOrderDetailGroup != null ? purchaseOrderDetailGroup.Quantity : 0) - (finalstockReceived != null ? finalstockReceived.QuantityReceived : 0) - stockInDetail.Quantity,
                                           BonusQuantityAvailable = (purchaseOrderDetailGroup != null ? purchaseOrderDetailGroup.BonusQuantity : 0) - (finalstockReceived != null ? finalstockReceived.BonusQuantityReceived : 0) - stockInDetail.BonusQuantity
                                       });

            var exceedingItem = availableQuantities.FirstOrDefault(x => x.QuantityAvailable < 0 || x.BonusQuantityAvailable < 0);
            if (exceedingItem != null)
            {
                var language = _httpContextAccessor.GetProgramCurrentLanguage();

                var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
                var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

                if (exceedingItem.QuantityAvailable < 0)
                {
                    return new ResponseDto { Id = stockIn.StockInHeader.StockInHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.QuantityExceeding, itemName!, itemPackageName!, exceedingItem.QuantityReceived.ToNormalizedString(), exceedingItem.PurchaseOrderQuantity.ToNormalizedString()) };
                }
                else
                {
                    return new ResponseDto { Id = stockIn.StockInHeader.StockInHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.BonusQuantityExceeding, itemName!, itemPackageName!, exceedingItem.BonusQuantityReceived.ToNormalizedString(), exceedingItem.PurchaseOrderQuantity.ToNormalizedString()) };
                }
            }
            else
            {
                return new ResponseDto { Id = stockIn.StockInHeader.StockInHeaderId, Success = true };
            }
        }

        private async Task<ResponseDto> CheckStockInFromPurchaseInvoiceQuantityExceeding(StockInDto stockIn, int menuCode, int parentMenuCode)
        {
            if (stockIn.StockInHeader == null || !(stockIn.StockInHeader.PurchaseInvoiceHeaderId > 0)) return new ResponseDto { Message = "StockType mismatches parent foreign keys" };

            var groupedStockInDetails = _stockInDetailService.GroupStockInDetailsWithAllKeys(stockIn.StockInDetails);
            var itemIds = groupedStockInDetails.Select(x => x.ItemId).ToList();

            List<StockReceivedFromPurchaseInvoiceDto>? finalstocksReceived;
            if (stockIn.StockInHeader.StockInHeaderId == 0)
            {
                finalstocksReceived = await _stockInQuantityService.GetFinalStocksReceivedFromPurchaseInvoice((int)stockIn.StockInHeader.PurchaseInvoiceHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            }
            else
            {
                finalstocksReceived = await _stockInQuantityService.GetFinalStocksReceivedFromPurchaseInvoiceExceptStockInHeaderId((int)stockIn.StockInHeader.PurchaseInvoiceHeaderId, stockIn.StockInHeader.StockInHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            }

            var purchaseInvoiceDetaisGrouped = await _purchaseInvoiceDetailService.GetPurchaseInvoiceDetailsGrouped((int)stockIn.StockInHeader.PurchaseInvoiceHeaderId);

			var availableQuantities = (from stockInDetail in groupedStockInDetails
									   from finalstockReceived in finalstocksReceived.Where(x => x.ItemId == stockInDetail.ItemId && x.ItemPackageId == stockInDetail.ItemPackageId && x.BatchNumber == stockInDetail.BatchNumber && x.ExpireDate == stockInDetail.ExpireDate && x.CostCenterId == stockInDetail.CostCenterId && x.BarCode == stockInDetail.BarCode && x.PurchasePrice == stockInDetail.PurchasePrice && x.ItemDiscountPercent == stockInDetail.ItemDiscountPercent).DefaultIfEmpty()
                                       from purchaseInvoiceDetailGroup in purchaseInvoiceDetaisGrouped.Where(x => x.ItemId == stockInDetail.ItemId && x.ItemPackageId == stockInDetail.ItemPackageId && x.BatchNumber == stockInDetail.BatchNumber && x.ExpireDate == stockInDetail.ExpireDate && x.CostCenterId == stockInDetail.CostCenterId && x.BarCode == stockInDetail.BarCode && x.PurchasePrice == stockInDetail.PurchasePrice && x.ItemDiscountPercent == stockInDetail.ItemDiscountPercent).DefaultIfEmpty()
                                       select new
                                       {
                                           stockInDetail.ItemId,
                                           stockInDetail.ItemPackageId,
                                           PurchaseInvoiceQuantity = purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.Quantity : 0,
                                           PurchaseInvoiceBonusQuantity = purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.BonusQuantity : 0,
                                           QuantityReceived = (finalstockReceived != null ? finalstockReceived.QuantityReceived : 0) + stockInDetail.Quantity,
                                           BonusQuantityReceived = (finalstockReceived != null ? finalstockReceived.BonusQuantityReceived : 0) + stockInDetail.BonusQuantity,
                                           QuantityAvailable = (purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.Quantity : 0) - (finalstockReceived != null ? finalstockReceived.QuantityReceived : 0) - stockInDetail.Quantity,
                                           BonusQuantityAvailable = (purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.BonusQuantity : 0) - (finalstockReceived != null ? finalstockReceived.BonusQuantityReceived : 0) - stockInDetail.BonusQuantity
                                       });

            var exceedingItem = availableQuantities.FirstOrDefault(x => x.QuantityAvailable < 0 || x.BonusQuantityAvailable < 0);
            if (exceedingItem != null)
            {
                var language = _httpContextAccessor.GetProgramCurrentLanguage();

                var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
                var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

                if (exceedingItem.QuantityAvailable < 0)
                {
                    return new ResponseDto { Id = stockIn.StockInHeader.StockInHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.QuantityExceeding, itemName!, itemPackageName!, exceedingItem.QuantityReceived.ToNormalizedString(), exceedingItem.PurchaseInvoiceQuantity.ToNormalizedString()) };
                }
                else
                {
                    return new ResponseDto { Id = stockIn.StockInHeader.StockInHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.BonusQuantityExceeding, itemName!, itemPackageName!, exceedingItem.BonusQuantityReceived.ToNormalizedString(), exceedingItem.PurchaseInvoiceBonusQuantity.ToNormalizedString()) };
                }
            }
            else
            {
                return new ResponseDto { Id = stockIn.StockInHeader.StockInHeaderId, Success = true };
            }
        }
    }
}
