using Inventory.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using Purchases.CoreOne.Contracts;
using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Purchases.CoreOne.Models.StaticData;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Extensions;
using Shared.Helper.Identity;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Purchases.Service.Services
{
    public class StockInReturnHandlingService: IStockInReturnHandlingService
    {
        private readonly IPurchaseMessageService _purchaseMessageService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly ISupplierDebitMemoService _supplierDebitMemoService;
        private readonly ISupplierCreditMemoService _supplierCreditMemoService;
        private readonly IStockInQuantityService _stockInQuantityService;
        private readonly IStockInDetailService _stockInDetailService;
        private readonly IStockInDetailTaxService _stockInDetailTaxService;
        private readonly IStockInHeaderService _stockInHeaderService;
        private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;
        private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IItemTaxService _itemTaxService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IItemService _itemService;
        private readonly IItemPackageService _itemPackageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
        private readonly IStockInReturnDetailService _stockInReturnDetailService;
        private readonly IStockInReturnDetailTaxService _stockInReturnDetailTaxService;
        private readonly IStockInReturnService _stockInReturnService;
        private readonly IPurchaseInvoiceReturnDetailService _purchaseInvoiceReturnDetailService;
        private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
        private readonly IPurchaseInvoiceDetailTaxService _purchaseInvoiceDetailTaxService;
        private readonly IPurchaseInvoiceService _purchaseInvoiceService;
        private readonly IItemPackingService _itemPackingService;
        private readonly IItemCostService _itemCostService;
        private readonly ITaxPercentService _taxPercentService;
        private readonly IPurchaseOrderStatusService _purchaseOrderStatusService;
        private readonly ITaxService _taxService;
        private readonly IPurchaseValueService _purchaseValueService;
        private readonly IGetPurchaseInvoiceSettleValueService _getPurchaseInvoiceSettleValueService;
        private readonly IItemNoteValidationService _itemNoteValidationService;

        public StockInReturnHandlingService(IPurchaseMessageService purchaseMessageService, IGenericMessageService genericMessageService, ISupplierDebitMemoService supplierDebitMemoService, ISupplierCreditMemoService supplierCreditMemoService, IStockInQuantityService stockInQuantityService, IStockInDetailService stockInDetailService, IStockInDetailTaxService stockInDetailTaxService, IStockInHeaderService stockInHeaderService, IPurchaseOrderDetailService purchaseOrderDetailService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IItemBarCodeService itemBarCodeService, IItemTaxService itemTaxService, IMenuNoteService menuNoteService, IItemService itemService, IItemPackageService itemPackageService, IHttpContextAccessor httpContextAccessor, IStockInReturnHeaderService stockInReturnHeaderService, IStockInReturnDetailService stockInReturnDetailService, IStockInReturnDetailTaxService stockInReturnDetailTaxService, IStockInReturnService stockInReturnService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IPurchaseInvoiceDetailTaxService purchaseInvoiceDetailTaxService, IPurchaseInvoiceService purchaseInvoiceService, IItemPackingService itemPackingService, IItemCostService itemCostService, ITaxPercentService taxPercentService, IPurchaseOrderStatusService purchaseOrderStatusService, ITaxService taxService, IPurchaseValueService purchaseValueService, IGetPurchaseInvoiceSettleValueService getPurchaseInvoiceSettleValueService, IItemNoteValidationService itemNoteValidationService)
        {
            _purchaseMessageService = purchaseMessageService;
            _genericMessageService = genericMessageService;
            _supplierDebitMemoService = supplierDebitMemoService;
            _supplierCreditMemoService = supplierCreditMemoService;
            _stockInQuantityService = stockInQuantityService;
            _stockInDetailService = stockInDetailService;
            _stockInDetailTaxService = stockInDetailTaxService;
            _stockInHeaderService = stockInHeaderService;
            _purchaseOrderDetailService = purchaseOrderDetailService;
            _purchaseOrderHeaderService = purchaseOrderHeaderService;
            _itemBarCodeService = itemBarCodeService;
            _itemTaxService = itemTaxService;
            _menuNoteService = menuNoteService;
            _itemService = itemService;
            _itemPackageService = itemPackageService;
            _httpContextAccessor = httpContextAccessor;
            _stockInReturnHeaderService = stockInReturnHeaderService;
            _stockInReturnDetailService = stockInReturnDetailService;
            _stockInReturnDetailTaxService = stockInReturnDetailTaxService;
            _stockInReturnService = stockInReturnService;
            _purchaseInvoiceReturnDetailService = purchaseInvoiceReturnDetailService;
            _purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _purchaseInvoiceDetailService = purchaseInvoiceDetailService;
            _purchaseInvoiceDetailTaxService = purchaseInvoiceDetailTaxService;
            _purchaseInvoiceService = purchaseInvoiceService;
            _itemPackingService = itemPackingService;
            _itemCostService = itemCostService;
            _taxPercentService = taxPercentService;
            _purchaseOrderStatusService = purchaseOrderStatusService;
            _taxService = taxService;
            _purchaseValueService = purchaseValueService;
            _getPurchaseInvoiceSettleValueService = getPurchaseInvoiceSettleValueService;
            _itemNoteValidationService = itemNoteValidationService;
        }

        public async Task<StockInReturnDto> GetStockInReturn(int stockInReturnHeaderId)
        {
            var header = await _stockInReturnHeaderService.GetStockInReturnHeaderById(stockInReturnHeaderId);
            if (header == null) { return new StockInReturnDto(); }

            var details = await GetStockInReturnDetailsCalculated(stockInReturnHeaderId, header.StockInHeaderId, header.PurchaseInvoiceHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(StockTypeData.ToMenuCode(header.StockTypeId), stockInReturnHeaderId).ToListAsync();
            var stockInReturnDetailTaxes = await _stockInReturnDetailTaxService.GetStockInReturnDetailTaxes(stockInReturnHeaderId).ToListAsync();

            foreach (var detail in details)
            {
                detail.StockInReturnDetailTaxes = stockInReturnDetailTaxes.Where(x => x.StockInReturnDetailId == detail.StockInReturnDetailId).ToList();
            }

            return new StockInReturnDto() { StockInReturnHeader = header, StockInReturnDetails = details, MenuNotes = menuNotes };
        }

        public async Task<StockInReturnDto> GetStockInReturnFromStockIn(int stockInHeaderId)
        {
            var stockInHeader = await _stockInHeaderService.GetStockInHeaderById(stockInHeaderId);
            if (stockInHeader == null)
            {
                return new StockInReturnDto();
            }

            var stockInDetails = await _stockInDetailService.GetStockInDetailsAsQueryable(stockInHeaderId).ToListAsync();
            var stockInDetailsGrouped = _stockInDetailService.GroupStockInDetailsWithAllKeys(stockInDetails);

            var itemIds = stockInDetails.Select(x => x.ItemId).ToList();

            var stocksReturned = await _stockInQuantityService.GetStocksReturnedFromStockIn(stockInHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

            var items = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var itemPackings = await _itemPackingService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var lastPurchasePrices = await _purchaseInvoiceService.GetMultipleLastPurchasePrices(itemIds);

            var stockInReturnDetails = (
                                    from stockInDetail in stockInDetails
                                    from item in items.Where(x => x.ItemId == stockInDetail.ItemId)
                                    from itemCost in itemCosts.Where(x => x.ItemId == stockInDetail.ItemId && x.StoreId == stockInHeader.StoreId).DefaultIfEmpty()
                                    from itemPacking in itemPackings.Where(x => x.ItemId == stockInDetail.ItemId && x.FromPackageId == stockInDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                                    from stockReturned in stocksReturned.Where(x => x.ItemId == stockInDetail.ItemId && x.ItemPackageId == stockInDetail.ItemPackageId && x.BatchNumber == stockInDetail.BatchNumber && x.ExpireDate == stockInDetail.ExpireDate && x.CostCenterId == stockInDetail.CostCenterId && x.BarCode == stockInDetail.BarCode && x.PurchasePrice == stockInDetail.PurchasePrice && x.ItemDiscountPercent == stockInDetail.ItemDiscountPercent).DefaultIfEmpty()
                                    from stockInDetailGroup in stockInDetailsGrouped.Where(x => x.ItemId == stockInDetail.ItemId && x.ItemPackageId == stockInDetail.ItemPackageId && x.BatchNumber == stockInDetail.BatchNumber && x.ExpireDate == stockInDetail.ExpireDate && x.CostCenterId == stockInDetail.CostCenterId && x.BarCode == stockInDetail.BarCode && x.PurchasePrice == stockInDetail.PurchasePrice && x.ItemDiscountPercent == stockInDetail.ItemDiscountPercent)
                                    from lastPurchasePrice in lastPurchasePrices.Where(x => x.ItemId == stockInDetail.ItemId && x.ItemPackageId == stockInDetail.ItemPackageId).DefaultIfEmpty()
                                    select new StockInReturnDetailDto()
                                    {
                                        StockInReturnDetailId = stockInDetail.StockInDetailId, // <-- This is used to get the related detail taxes
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
                                        StockInQuantity = stockInDetailGroup.Quantity,
                                        StockInBonusQuantity = stockInDetailGroup.BonusQuantity,
                                        AvailableQuantity = stockInDetailGroup.Quantity - (stockReturned != null ? stockReturned.QuantityReturned : 0),
                                        AvailableBonusQuantity = stockInDetailGroup.BonusQuantity - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0),
                                        PurchasePrice = stockInDetail.PurchasePrice,
                                        ItemDiscountPercent = stockInDetail.ItemDiscountPercent,
                                        VatPercent = stockInDetail.VatPercent,
                                        Notes = stockInDetail.Notes,
                                        ItemNote = stockInDetail.ItemNote,
                                        ConsumerPrice = item.ConsumerPrice,
                                        CostPrice = itemCost != null ? itemCost.CostPrice : 0,
                                        CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
                                        LastPurchasePrice = lastPurchasePrice != null ? lastPurchasePrice.PurchasePrice : 0,
                                    }).ToList();

			stockInReturnDetails = DistributeQuantityAndCalculateValues(stockInReturnDetails, stockInHeader.DiscountPercent);
			stockInReturnDetails = stockInReturnDetails.Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();

			var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);
            var stockInDetailTaxes = await _stockInDetailTaxService.GetStockInDetailTaxes(stockInHeaderId).ToListAsync();
            int newId = -1;
            int newSubId = -1;
            foreach (var stockInReturnDetail in stockInReturnDetails)
            {
                stockInReturnDetail.Packages = packages.Where(x => x.ItemId == stockInReturnDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                stockInReturnDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == stockInReturnDetail.ItemId).ToList();
                stockInReturnDetail.Taxes = stockInReturnDetail.ItemTaxData.ToJson();

                stockInReturnDetail.StockInReturnDetailTaxes = (
                    from itemTax in stockInDetailTaxes.Where(x => x.StockInDetailId == stockInReturnDetail.StockInReturnDetailId)
                    select new StockInReturnDetailTaxDto
                    {
                        TaxId = itemTax.TaxId,
                        CreditAccountId = itemTax.DebitAccountId,
                        TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
                        TaxPercent = itemTax.TaxPercent,
                        TaxValue = CalculateDetailValue.TaxValue(stockInReturnDetail.Quantity, stockInReturnDetail.PurchasePrice, stockInReturnDetail.ItemDiscountPercent, stockInReturnDetail.VatPercent, itemTax.TaxPercent, itemTax.TaxAfterVatInclusive, stockInHeader.DiscountPercent, false)
                    }
                ).ToList();

                stockInReturnDetail.OtherTaxValue = stockInReturnDetail.StockInReturnDetailTaxes.Sum(x => x.TaxValue);
                stockInReturnDetail.NetValue = CalculateDetailValue.NetValue(stockInReturnDetail.Quantity, stockInReturnDetail.PurchasePrice, stockInReturnDetail.ItemDiscountPercent, stockInReturnDetail.VatPercent, stockInReturnDetail.OtherTaxValue, stockInHeader.DiscountPercent, false);

                foreach (var stockInReturnDetailTax in stockInReturnDetail.StockInReturnDetailTaxes)
                {
                    stockInReturnDetailTax.StockInReturnDetailId = newId;
                    stockInReturnDetailTax.StockInReturnDetailTaxId = newSubId--;
                }
                stockInReturnDetail.StockInReturnDetailId = newId;
                newId--;
            }

            var totalValueFromDetail = stockInReturnDetails.Sum(x => x.TotalValue);
            var totalValueAfterDiscountFromDetail = stockInReturnDetails.Sum(x => x.TotalValueAfterDiscount);
            var totalItemDiscount = stockInReturnDetails.Sum(x => x.ItemDiscountValue);
            var grossValueFromDetail = stockInReturnDetails.Sum(x => x.GrossValue);
            var vatValueFromDetail = stockInReturnDetails.Sum(x => x.VatValue);
            var subNetValueFromDetail = stockInReturnDetails.Sum(x => x.SubNetValue);
            var otherTaxValueFromDetail = stockInReturnDetails.Sum(x => x.OtherTaxValue);
            var netValueFromDetail = stockInReturnDetails.Sum(x => x.NetValue);
            var totalCostValueFromDetail = stockInReturnDetails.Sum(x => x.CostValue);

            var stockInReturnHeader = new StockInReturnHeaderDto
            {
                StockInReturnHeaderId = 0,
                StockTypeId = stockInHeader.StockTypeId == StockTypeData.ReceiptStatement ? StockTypeData.ReceiptStatementReturn : StockTypeData.ReceiptFromPurchaseInvoiceOnTheWayReturn,
                StockInHeaderId = stockInHeader.StockInHeaderId,
                StockInFullCode = stockInHeader.DocumentFullCode,
                StockInDocumentReference = stockInHeader.DocumentReference,
                StockInPurchaseOrderHeaderId = stockInHeader.PurchaseOrderHeaderId,
                StockInPurchaseOrderFullCode = stockInHeader.PurchaseOrderFullCode,
                StockInPurchaseOrderDocumentReference = stockInHeader.PurchaseOrderDocumentReference,
                StockInPurchaseInvoiceHeaderId = stockInHeader.PurchaseInvoiceHeaderId,
                StockInPurchaseInvoiceFullCode = stockInHeader.PurchaseInvoiceFullCode,
                StockInPurchaseInvoiceDocumentReference = stockInHeader.PurchaseInvoiceDocumentReference,
                SupplierId = stockInHeader.SupplierId,
                SupplierCode = stockInHeader.SupplierCode,
                SupplierName = stockInHeader.SupplierName,
                StoreId = stockInHeader.StoreId,
                StoreName = stockInHeader.StoreName,
                DocumentDate = stockInHeader.DocumentDate,
                Reference = stockInHeader.Reference,
                TotalValue = totalValueFromDetail,
                DiscountPercent = stockInHeader.DiscountPercent,
                DiscountValue = CalculateHeaderValue.DiscountValue(totalValueAfterDiscountFromDetail, stockInHeader.DiscountPercent),
                TotalItemDiscount = totalItemDiscount,
                GrossValue = grossValueFromDetail,
                VatValue = vatValueFromDetail,
                SubNetValue = subNetValueFromDetail,
                OtherTaxValue = otherTaxValueFromDetail,
                NetValueBeforeAdditionalDiscount = netValueFromDetail,
                AdditionalDiscountValue = stockInHeader.AdditionalDiscountValue,
                NetValue = CalculateHeaderValue.NetValue(netValueFromDetail, stockInHeader.AdditionalDiscountValue),
                TotalCostValue = totalCostValueFromDetail,
                RemarksAr = stockInHeader.RemarksAr,
                RemarksEn = stockInHeader.RemarksEn,
                IsClosed = false,
                IsEnded = false,
                IsBlocked = false,
                ArchiveHeaderId = stockInHeader.ArchiveHeaderId,
            };

            return new StockInReturnDto { StockInReturnHeader = stockInReturnHeader, StockInReturnDetails = stockInReturnDetails };
        }

        public async Task<StockInReturnDto> GetStockInReturnFromPurchaseInvoice(int purchaseInvoiceHeaderId)
        {
            var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaderById(purchaseInvoiceHeaderId);
            if (purchaseInvoiceHeader == null)
            {
                return new StockInReturnDto();
            }

            var purchaseInvoiceDetails = await _purchaseInvoiceDetailService.GetPurchaseInvoiceDetailsAsQueryable(purchaseInvoiceHeaderId).ToListAsync();
            var groupedPurchaseInvoiceDetails = _purchaseInvoiceDetailService.GroupPurchaseInvoiceDetails(purchaseInvoiceDetails);

            var itemIds = purchaseInvoiceDetails.Select(x => x.ItemId).ToList();

            var stocksReturned = await _stockInQuantityService.GetStocksReturnedFromPurchaseInvoice(purchaseInvoiceHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

            var items = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var itemPackings = await _itemPackingService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var lastPurchasePrices = await _purchaseInvoiceService.GetMultipleLastPurchasePrices(itemIds);

            var purchaseInvoiceReturnOnTheWayDetailsGrouped = await GetPurchaseInvoiceReturnOnTheWayQuantities(purchaseInvoiceHeaderId);

            var stockInReturnDetails = (
                                    from purchaseInvoiceDetail in purchaseInvoiceDetails
                                    from item in items.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId)
                                    from itemCost in itemCosts.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.StoreId == purchaseInvoiceHeader.StoreId).DefaultIfEmpty()
                                    from itemPacking in itemPackings.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.FromPackageId == purchaseInvoiceDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                                    from stockReturned in stocksReturned.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId && x.BatchNumber == purchaseInvoiceDetail.BatchNumber && x.ExpireDate == purchaseInvoiceDetail.ExpireDate && x.CostCenterId == purchaseInvoiceDetail.CostCenterId && x.BarCode == purchaseInvoiceDetail.BarCode && x.PurchasePrice == purchaseInvoiceDetail.PurchasePrice && x.ItemDiscountPercent == purchaseInvoiceDetail.ItemDiscountPercent).DefaultIfEmpty()
                                    from purchaseInvoiceDetailGroup in groupedPurchaseInvoiceDetails.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId && x.BatchNumber == purchaseInvoiceDetail.BatchNumber && x.ExpireDate == purchaseInvoiceDetail.ExpireDate && x.CostCenterId == purchaseInvoiceDetail.CostCenterId && x.BarCode == purchaseInvoiceDetail.BarCode && x.PurchasePrice == purchaseInvoiceDetail.PurchasePrice && x.ItemDiscountPercent == purchaseInvoiceDetail.ItemDiscountPercent)
                                    from purchaseInvoiceReturnDetailGroup in purchaseInvoiceReturnOnTheWayDetailsGrouped.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId && x.BatchNumber == purchaseInvoiceDetail.BatchNumber && x.ExpireDate == purchaseInvoiceDetail.ExpireDate && x.CostCenterId == purchaseInvoiceDetail.CostCenterId && x.BarCode == purchaseInvoiceDetail.BarCode && x.PurchasePrice == purchaseInvoiceDetail.PurchasePrice && x.ItemDiscountPercent == purchaseInvoiceDetail.ItemDiscountPercent).DefaultIfEmpty()
                                    from lastPurchasePrice in lastPurchasePrices.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId).DefaultIfEmpty()
                                    select new StockInReturnDetailDto()
                                    {
                                        StockInReturnDetailId = purchaseInvoiceDetail.PurchaseInvoiceDetailId, // <-- used to join with detail taxes
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
                                        PurchaseInvoiceQuantity = purchaseInvoiceDetailGroup.Quantity,
                                        PurchaseInvoiceBonusQuantity = purchaseInvoiceDetailGroup.BonusQuantity,
                                        AvailableQuantity = purchaseInvoiceDetailGroup.Quantity - (purchaseInvoiceReturnDetailGroup != null ? purchaseInvoiceReturnDetailGroup.Quantity : 0) - (stockReturned != null ? stockReturned.QuantityReturned : 0),
                                        AvailableBonusQuantity = purchaseInvoiceDetailGroup.BonusQuantity - (purchaseInvoiceReturnDetailGroup != null ? purchaseInvoiceReturnDetailGroup.BonusQuantity : 0) - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0),
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

			stockInReturnDetails = DistributeQuantityAndCalculateValues(stockInReturnDetails, purchaseInvoiceHeader.DiscountPercent);
			stockInReturnDetails = stockInReturnDetails.Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();

			var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);
            var purchaseInvoiceDetailTaxes = await _purchaseInvoiceDetailTaxService.GetPurchaseInvoiceDetailTaxes(purchaseInvoiceHeaderId).ToListAsync();
            int newId = -1;
            int newSubId = -1;
            foreach (var stockInReturnDetail in stockInReturnDetails)
            {
                stockInReturnDetail.Packages = packages.Where(x => x.ItemId == stockInReturnDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                stockInReturnDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == stockInReturnDetail.ItemId).ToList();
                stockInReturnDetail.Taxes = stockInReturnDetail.ItemTaxData.ToJson();

                stockInReturnDetail.StockInReturnDetailTaxes = (
                    from itemTax in purchaseInvoiceDetailTaxes.Where(x => x.PurchaseInvoiceDetailId == stockInReturnDetail.StockInReturnDetailId)
                    select new StockInReturnDetailTaxDto
                    {
                        TaxId = itemTax.TaxId,
                        CreditAccountId = itemTax.DebitAccountId,
                        TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
                        TaxPercent = itemTax.TaxPercent,
                        TaxValue = CalculateDetailValue.TaxValue(stockInReturnDetail.Quantity, stockInReturnDetail.PurchasePrice, stockInReturnDetail.ItemDiscountPercent, stockInReturnDetail.VatPercent, itemTax.TaxPercent, itemTax.TaxAfterVatInclusive, purchaseInvoiceHeader.DiscountPercent, false)
                    }
                ).ToList();

                stockInReturnDetail.OtherTaxValue = stockInReturnDetail.StockInReturnDetailTaxes.Sum(x => x.TaxValue);
                stockInReturnDetail.NetValue = CalculateDetailValue.NetValue(stockInReturnDetail.Quantity, stockInReturnDetail.PurchasePrice, stockInReturnDetail.ItemDiscountPercent, stockInReturnDetail.VatPercent, stockInReturnDetail.OtherTaxValue, purchaseInvoiceHeader.DiscountPercent, false);

                foreach (var stockInReturnDetailTax in stockInReturnDetail.StockInReturnDetailTaxes)
                {
                    stockInReturnDetailTax.StockInReturnDetailId = newId;
                    stockInReturnDetailTax.StockInReturnDetailTaxId = newSubId--;
                }
                stockInReturnDetail.StockInReturnDetailId = newId;
                newId--;
            }

            var totalValueFromDetail = stockInReturnDetails.Sum(x => x.TotalValue);
            var totalValueAfterDiscountFromDetail = stockInReturnDetails.Sum(x => x.TotalValueAfterDiscount);
            var totalItemDiscount = stockInReturnDetails.Sum(x => x.ItemDiscountValue);
            var grossValueFromDetail = stockInReturnDetails.Sum(x => x.GrossValue);
            var vatValueFromDetail = stockInReturnDetails.Sum(x => x.VatValue);
            var subNetValueFromDetail = stockInReturnDetails.Sum(x => x.SubNetValue);
            var otherTaxValueFromDetail = stockInReturnDetails.Sum(x => x.OtherTaxValue);
            var netValueFromDetail = stockInReturnDetails.Sum(x => x.NetValue);
            var totalCostValueFromDetail = stockInReturnDetails.Sum(x => x.CostValue);

            var stockInReturnHeader = new StockInReturnHeaderDto
            {
                StockInReturnHeaderId = 0,
                StockTypeId = StockTypeData.ReturnFromPurchaseInvoice,
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

            return new StockInReturnDto { StockInReturnHeader = stockInReturnHeader, StockInReturnDetails = stockInReturnDetails };
        }

        public async Task<List<StockInReturnDetailDto>> GetStockInReturnDetailsCalculated(int stockInReturnHeaderId, int? stockInHeaderId = null, int? purchaseInvoiceHeaderId = null, List<StockInReturnDetailDto>? stockInReturnDetails = null)
        {
            var stockInReturnHeader = await _stockInReturnHeaderService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeaderId).Select(x => new { x.StockInHeaderId, x.PurchaseInvoiceHeaderId }).FirstOrDefaultAsync();
            stockInHeaderId ??= stockInReturnHeader?.StockInHeaderId;
            purchaseInvoiceHeaderId ??= stockInReturnHeader?.PurchaseInvoiceHeaderId;

            stockInReturnDetails ??= await _stockInReturnDetailService.GetStockInReturnDetailsAsQueryable(stockInReturnHeaderId).ToListAsync();
            var itemIds = stockInReturnDetails.Select(x => x.ItemId).ToList();

            if (stockInHeaderId != null)
            {
                var stocksReturned = await _stockInQuantityService.GetStocksReturnedFromStockInExceptStockInReturnHeaderId((int)stockInHeaderId, stockInReturnHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
                var stockInDetailsGrouped = await _stockInDetailService.GetStockInDetailsGroupedWithAllKeys((int)stockInHeaderId);

                stockInReturnDetails = (
                    from stockInReturnDetail in stockInReturnDetails
                    from stockInDetailGroup in stockInDetailsGrouped.Where(x => x.ItemId == stockInReturnDetail.ItemId && x.ItemPackageId == stockInReturnDetail.ItemPackageId && x.BatchNumber == stockInReturnDetail.BatchNumber && x.ExpireDate == stockInReturnDetail.ExpireDate && x.CostCenterId == stockInReturnDetail.CostCenterId && x.BarCode == stockInReturnDetail.BarCode && x.PurchasePrice == stockInReturnDetail.PurchasePrice && x.ItemDiscountPercent == stockInReturnDetail.ItemDiscountPercent).DefaultIfEmpty()
                    from stockReturned in stocksReturned.Where(x => x.ItemId == stockInReturnDetail.ItemId && x.ItemPackageId == stockInReturnDetail.ItemPackageId && x.BatchNumber == stockInReturnDetail.BatchNumber && x.ExpireDate == stockInReturnDetail.ExpireDate && x.CostCenterId == stockInReturnDetail.CostCenterId && x.BarCode == stockInReturnDetail.BarCode && x.PurchasePrice == stockInReturnDetail.PurchasePrice && x.ItemDiscountPercent == stockInReturnDetail.ItemDiscountPercent).DefaultIfEmpty()
                    select new StockInReturnDetailDto
                    {
                        StockInReturnDetailId = stockInReturnDetail.StockInReturnDetailId,
                        StockInReturnHeaderId = stockInReturnDetail.StockInReturnHeaderId,
                        CostCenterId = stockInReturnDetail.CostCenterId,
                        CostCenterName = stockInReturnDetail.CostCenterName,
                        ItemId = stockInReturnDetail.ItemId,
                        ItemCode = stockInReturnDetail.ItemCode,
                        ItemName = stockInReturnDetail.ItemName,
                        TaxTypeId = stockInReturnDetail.TaxTypeId,
                        ItemTypeId = stockInReturnDetail.ItemTypeId,
                        ItemPackageId = stockInReturnDetail.ItemPackageId,
                        ItemPackageName = stockInReturnDetail.ItemPackageName,
                        IsItemVatInclusive = stockInReturnDetail.IsItemVatInclusive,
                        BarCode = stockInReturnDetail.BarCode,
                        Packing = stockInReturnDetail.Packing,
                        ExpireDate = stockInReturnDetail.ExpireDate,
                        BatchNumber = stockInReturnDetail.BatchNumber,
                        Quantity = stockInReturnDetail.Quantity,
                        BonusQuantity = stockInReturnDetail.BonusQuantity,
                        StockInQuantity = stockInDetailGroup != null ? stockInDetailGroup.Quantity : 0,
                        StockInBonusQuantity = stockInDetailGroup != null ? stockInDetailGroup.BonusQuantity : 0,
                        AvailableQuantity = (stockInDetailGroup != null ? stockInDetailGroup.Quantity : 0) - (stockReturned != null ? stockReturned.QuantityReturned : 0),
                        AvailableBonusQuantity = (stockInDetailGroup != null ? stockInDetailGroup.BonusQuantity : 0) - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0),
                        PurchasePrice = stockInReturnDetail.PurchasePrice,
                        TotalValue = stockInReturnDetail.TotalValue,
                        ItemDiscountPercent = stockInReturnDetail.ItemDiscountPercent,
                        ItemDiscountValue = stockInReturnDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = stockInReturnDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = stockInReturnDetail.HeaderDiscountValue,
                        GrossValue = stockInReturnDetail.GrossValue,
                        VatPercent = stockInReturnDetail.VatPercent,
                        VatValue = stockInReturnDetail.VatValue,
                        SubNetValue = stockInReturnDetail.SubNetValue,
                        OtherTaxValue = stockInReturnDetail.OtherTaxValue,
                        NetValue = stockInReturnDetail.NetValue,
                        Notes = stockInReturnDetail.Notes,
                        ItemNote = stockInReturnDetail.ItemNote,
                        ConsumerPrice = stockInReturnDetail.ConsumerPrice,
                        CostPrice = stockInReturnDetail.CostPrice,
                        CostPackage = stockInReturnDetail.CostPackage,
                        LastPurchasePrice = stockInReturnDetail.LastPurchasePrice,

                        StockInReturnDetailTaxes = stockInReturnDetail.StockInReturnDetailTaxes,

                        CreatedAt = stockInReturnDetail.CreatedAt,
                        IpAddressCreated = stockInReturnDetail.IpAddressCreated,
                        UserNameCreated = stockInReturnDetail.UserNameCreated,
                    }).ToList();
            }
            else if(purchaseInvoiceHeaderId != null)
            {
                var stocksReturned = await _stockInQuantityService.GetStocksReturnedFromPurchaseInvoiceExceptStockInReturnHeaderId((int)purchaseInvoiceHeaderId, stockInReturnHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

                var purchaseInvoiceDetailsGrouped = await _purchaseInvoiceDetailService.GetPurchaseInvoiceDetailsGrouped((int)purchaseInvoiceHeaderId);

                var purchaseInvoiceReturnOnTheWayDetailsGrouped = await GetPurchaseInvoiceReturnOnTheWayQuantities((int)purchaseInvoiceHeaderId);

                stockInReturnDetails = (
                    from stockInReturnDetail in stockInReturnDetails
                    from purchaseInvoiceDetailGroup in purchaseInvoiceDetailsGrouped.Where(x => x.ItemId == stockInReturnDetail.ItemId && x.ItemPackageId == stockInReturnDetail.ItemPackageId && x.BatchNumber == stockInReturnDetail.BatchNumber && x.ExpireDate == stockInReturnDetail.ExpireDate && x.CostCenterId == stockInReturnDetail.CostCenterId && x.BarCode == stockInReturnDetail.BarCode && x.PurchasePrice == stockInReturnDetail.PurchasePrice && x.ItemDiscountPercent == stockInReturnDetail.ItemDiscountPercent).DefaultIfEmpty()
                    from stockReturned in stocksReturned.Where(x => x.ItemId == stockInReturnDetail.ItemId && x.ItemPackageId == stockInReturnDetail.ItemPackageId && x.BatchNumber == stockInReturnDetail.BatchNumber && x.ExpireDate == stockInReturnDetail.ExpireDate && x.CostCenterId == stockInReturnDetail.CostCenterId && x.BarCode == stockInReturnDetail.BarCode && x.PurchasePrice == stockInReturnDetail.PurchasePrice && x.ItemDiscountPercent == stockInReturnDetail.ItemDiscountPercent).DefaultIfEmpty()
                    from purchaseInvoiceReturnDetailGroup in purchaseInvoiceReturnOnTheWayDetailsGrouped.Where(x => x.ItemId == stockInReturnDetail.ItemId && x.ItemPackageId == stockInReturnDetail.ItemPackageId && x.BatchNumber == stockInReturnDetail.BatchNumber && x.ExpireDate == stockInReturnDetail.ExpireDate && x.CostCenterId == stockInReturnDetail.CostCenterId && x.BarCode == stockInReturnDetail.BarCode && x.PurchasePrice == stockInReturnDetail.PurchasePrice && x.ItemDiscountPercent == stockInReturnDetail.ItemDiscountPercent).DefaultIfEmpty()
                    select new StockInReturnDetailDto
                    {
                        StockInReturnDetailId = stockInReturnDetail.StockInReturnDetailId,
                        StockInReturnHeaderId = stockInReturnDetail.StockInReturnHeaderId,
                        CostCenterId = stockInReturnDetail.CostCenterId,
                        CostCenterName = stockInReturnDetail.CostCenterName,
                        ItemId = stockInReturnDetail.ItemId,
                        ItemCode = stockInReturnDetail.ItemCode,
                        ItemName = stockInReturnDetail.ItemName,
                        TaxTypeId = stockInReturnDetail.TaxTypeId,
                        ItemTypeId = stockInReturnDetail.ItemTypeId,
                        ItemPackageId = stockInReturnDetail.ItemPackageId,
                        ItemPackageName = stockInReturnDetail.ItemPackageName,
                        IsItemVatInclusive = stockInReturnDetail.IsItemVatInclusive,
                        BarCode = stockInReturnDetail.BarCode,
                        Packing = stockInReturnDetail.Packing,
                        ExpireDate = stockInReturnDetail.ExpireDate,
                        BatchNumber = stockInReturnDetail.BatchNumber,
                        Quantity = stockInReturnDetail.Quantity,
                        BonusQuantity = stockInReturnDetail.BonusQuantity,
                        PurchaseInvoiceQuantity = purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.Quantity : 0,
                        PurchaseInvoiceBonusQuantity = purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.BonusQuantity : 0,
                        AvailableQuantity = (purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.Quantity : 0) - (stockReturned != null ? stockReturned.QuantityReturned : 0) - (purchaseInvoiceReturnDetailGroup != null ? purchaseInvoiceReturnDetailGroup.Quantity : 0),
                        AvailableBonusQuantity = (purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.BonusQuantity : 0) - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0) - (purchaseInvoiceReturnDetailGroup != null ? purchaseInvoiceReturnDetailGroup.BonusQuantity : 0),
                        PurchasePrice = stockInReturnDetail.PurchasePrice,
                        TotalValue = stockInReturnDetail.TotalValue,
                        ItemDiscountPercent = stockInReturnDetail.ItemDiscountPercent,
                        ItemDiscountValue = stockInReturnDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = stockInReturnDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = stockInReturnDetail.HeaderDiscountValue,
                        GrossValue = stockInReturnDetail.GrossValue,
                        VatPercent = stockInReturnDetail.VatPercent,
                        VatValue = stockInReturnDetail.VatValue,
                        SubNetValue = stockInReturnDetail.SubNetValue,
                        OtherTaxValue = stockInReturnDetail.OtherTaxValue,
                        NetValue = stockInReturnDetail.NetValue,
                        Notes = stockInReturnDetail.Notes,
                        ItemNote = stockInReturnDetail.ItemNote,
                        ConsumerPrice = stockInReturnDetail.ConsumerPrice,
                        CostPrice = stockInReturnDetail.CostPrice,
                        CostPackage = stockInReturnDetail.CostPackage,
                        LastPurchasePrice = stockInReturnDetail.LastPurchasePrice,

						StockInReturnDetailTaxes = stockInReturnDetail.StockInReturnDetailTaxes,

						CreatedAt = stockInReturnDetail.CreatedAt,
                        IpAddressCreated = stockInReturnDetail.IpAddressCreated,
                        UserNameCreated = stockInReturnDetail.UserNameCreated,
                    }).ToList();
            }

            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

            foreach (var stockInReturnDetail in stockInReturnDetails)
            {
                stockInReturnDetail.Packages = packages.Where(x => x.ItemId == stockInReturnDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                stockInReturnDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == stockInReturnDetail.ItemId).ToList();
                stockInReturnDetail.Taxes = stockInReturnDetail.ItemTaxData.ToJson();
            }

            return stockInReturnDetails;
        }

		private List<StockInReturnDetailDto> DistributeQuantityAndCalculateValues(List<StockInReturnDetailDto> stockInReturnDetails, decimal headerDiscountPercent)
		{
			QuantityDistributionLogic.DistributeQuantitiesOnDetails(
				details: stockInReturnDetails,
				keySelector: x => (x.ItemId, x.ItemPackageId, x.BarCode, x.CostCenterId, x.PurchasePrice, x.ItemDiscountPercent, x.ExpireDate, x.BatchNumber),
				availableQuantitySelector: x => x.AvailableQuantity,
				availableBonusQuantitySelector: x => x.AvailableBonusQuantity,
				quantitySelector: x => x.Quantity,
				bonusQuantitySelector: x => x.BonusQuantity,
				quantityAssigner: (x, value) => x.Quantity = value,
				bonusQuantityAssigner: (x, value) => x.BonusQuantity = value
			);

			RecalculateDetailValue.RecalculateDetailValues(
				details: stockInReturnDetails,
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

			return stockInReturnDetails;
		}

		private async Task<int> GetParentMenuCode(int? stockInHeaderId, int? purchaseInvoiceHeaderId)
        {
            if (stockInHeaderId != null)
            {
                var stockTypeId = (await _stockInHeaderService.GetStockInHeaderById((int)stockInHeaderId))!.StockTypeId;
                return StockTypeData.ToMenuCode(stockTypeId);
            }
            else
            {
                var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaderById((int)purchaseInvoiceHeaderId!);
                return PurchaseInvoiceMenuCodeHelper.GetMenuCode(purchaseInvoiceHeader!);
            }
        }

		private async Task<int?> GetGrandParentMenuCode(int? stockInHeaderId)
		{
            if (stockInHeaderId == null) return null;

            var stockInHeader = await _stockInHeaderService.GetStockInHeaderById((int)stockInHeaderId);
			if (stockInHeader!.PurchaseOrderHeaderId != null)
			{
				return MenuCodeData.PurchaseOrder;
			}
			else
			{
				var isCredit = await _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == stockInHeader.PurchaseInvoiceHeaderId).Select(x => x.CreditPayment).FirstOrDefaultAsync();
				return isCredit ? MenuCodeData.PurchaseInvoiceOnTheWayCredit : MenuCodeData.PurchaseInvoiceOnTheWayCash;
			}
		}

		public async Task<ResponseDto> SaveStockInReturn(StockInReturnDto stockInReturn, bool hasApprove, bool approved, int? requestId)
        {
            TrimDetailStrings(stockInReturn.StockInReturnDetails);

            var menuCode = StockTypeData.ToMenuCode(stockInReturn.StockInReturnHeader!.StockTypeId);
            var parentMenuCode = await GetParentMenuCode(stockInReturn.StockInReturnHeader.StockInHeaderId, stockInReturn.StockInReturnHeader.PurchaseInvoiceHeaderId);
            var grandParentMenuCode = await GetGrandParentMenuCode(stockInReturn.StockInReturnHeader.StockInHeaderId);
			var stockInReturnValidationResult = await CheckStockInReturnIsValidForSave(stockInReturn, menuCode, parentMenuCode, grandParentMenuCode);
            if(stockInReturnValidationResult.Success == false)
            {
                return stockInReturnValidationResult;
            }

            if (stockInReturn.StockInReturnHeader!.StockInReturnHeaderId == 0)
            {
                await StockInReturnUpdateModelPrices(stockInReturn);
            }

            var result = await _stockInReturnService.SaveStockInReturn(stockInReturn, hasApprove, approved, requestId);
            if (result.Success)
            {
                if(stockInReturn.StockInReturnHeader?.StockTypeId == StockTypeData.ReturnFromPurchaseInvoice)
                {
                    await _purchaseInvoiceHeaderService.UpdateClosed(stockInReturn.StockInReturnHeader?.PurchaseInvoiceHeaderId, true);
                }
                else
                {
                    await _stockInHeaderService.UpdateClosed(stockInReturn.StockInReturnHeader?.StockInHeaderId, true);
                }

                await UpdatePurchaseOrderStatusFromStockInReturn(stockInReturn.StockInReturnHeader!.StockInHeaderId, stockInReturn.StockInReturnHeader!.PurchaseInvoiceHeaderId, StockTypeData.ToMenuCode(stockInReturn.StockInReturnHeader!.StockTypeId));
                await UpdatePurchaseInvoiceReturnOnTheWayEnded(stockInReturn.StockInReturnHeader.PurchaseInvoiceHeaderId, true);
			}
            return result;
        }

        private async Task<List<PurchaseInvoiceReturnDetailDto>> GetPurchaseInvoiceReturnOnTheWayQuantities(int purchaseInvoiceHeaderId)
        {
            return await (from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId)
                    from purchaseInvoiceReturnDetails in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId)
                    select purchaseInvoiceReturnDetails).GroupBy(x => new {x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber, x.CostCenterId, x.BarCode, x.PurchasePrice, x.ItemDiscountPercent}).Select(x => 
                    new PurchaseInvoiceReturnDetailDto
                    {
                        ItemId = x.Key.ItemId,
                        ItemPackageId = x.Key.ItemPackageId,
                        ExpireDate = x.Key.ExpireDate,
                        BatchNumber = x.Key.BatchNumber,
                        CostCenterId = x.Key.CostCenterId,
                        BarCode = x.Key.BarCode,
                        PurchasePrice = x.Key.PurchasePrice,
                        ItemDiscountPercent = x.Key.ItemDiscountPercent,
                        Quantity = x.Sum(x => x.Quantity),
                        BonusQuantity = x.Sum(x => x.BonusQuantity)
                    }).ToListAsync();
        }

        private async Task<ResponseDto> CheckStockInReturnIsValidForSave(StockInReturnDto stockInReturn, int menuCode, int parentMenuCode, int? grandParentMenuCode)
        {
            var stockInReturnHeaderId = stockInReturn.StockInReturnHeader?.StockInReturnHeaderId;
            ResponseDto result;

            if (stockInReturnHeaderId != 0)
            {
                result = await CheckStockInReturnIsClosed(stockInReturnHeaderId ?? 0, menuCode);
                if (result.Success == false) return result;
            }

            result = await CheckStockInReturnEnded(stockInReturnHeaderId, stockInReturn.StockInReturnHeader?.StockInHeaderId, stockInReturn.StockInReturnHeader?.PurchaseInvoiceHeaderId, menuCode);
            if (result.Success == false) return result;

            result = await CheckStockInReturnBlocked(stockInReturnHeaderId, stockInReturn.StockInReturnHeader?.StockInHeaderId, stockInReturn.StockInReturnHeader?.PurchaseInvoiceHeaderId, menuCode);
            if (result.Success == false) return result;

            result = await CheckStockInReturnFromPurchaseInvoiceOnTheWayCreatedBeforeInvoiceReturnOnTheWay(stockInReturn.StockInReturnHeader?.PurchaseInvoiceHeaderId, menuCode, parentMenuCode);
            if (result.Success == false) return result;

			result = await CheckStockInReturnCauseInvoiceToBecomeLessThanSettledValue(stockInReturn.StockInReturnHeader!.NetValue, stockInReturn.StockInReturnHeader!.PurchaseInvoiceHeaderId, menuCode, parentMenuCode);
			if (result.Success == false) return result;

			result = await _itemNoteValidationService.CheckItemNoteWithItemType(stockInReturn.StockInReturnDetails, x => x.ItemId, x => x.ItemNote);
			if (result.Success == false) return result;

			result = await CheckStockInReturnQuantityForSaving(stockInReturn, menuCode, parentMenuCode, grandParentMenuCode);
            if (result.Success == false) return result;

            return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockInReturnCauseInvoiceToBecomeLessThanSettledValue(decimal stockInReturnNetValue, int? purchaseInvoiceHeaderId, int menuCode, int parentMenuCode)
		{
			if (purchaseInvoiceHeaderId != null)
			{
				var invoiceValue = await _purchaseValueService.GetOverallValueOfPurchaseInvoice((int)purchaseInvoiceHeaderId);
				var settledValue = await _getPurchaseInvoiceSettleValueService.GetPurchaseInvoiceSettleValue((int)purchaseInvoiceHeaderId);

				if (invoiceValue - stockInReturnNetValue < settledValue)
				{
					return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, parentMenuCode, GenericMessageData.CannotSaveBecauseSettled) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockInReturnIsClosed(int stockInReturnHeaderId, int menuCode)
        {
            var isClosed = await _stockInReturnHeaderService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeaderId).Select(x => x.IsClosed).FirstOrDefaultAsync();
            if (isClosed)
            {
                return new ResponseDto { Id = stockInReturnHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotUpdateBecauseClosed) };
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckStockInReturnEnded(int? stockInReturnHeaderId, int? stockInHeaderId, int? purchaseInvoiceHeaderId, int menuCode)
        {
            if (stockInHeaderId != null)
            {
                return await CheckStockInReturnFromStockInEnded(stockInReturnHeaderId, stockInHeaderId, menuCode);
            }
            else if (purchaseInvoiceHeaderId != null)
            {
                return await CheckStockInReturnFromPurchaseInvoiceEnded((int)purchaseInvoiceHeaderId, menuCode);
            }
            else
            {
                return new ResponseDto { Success = false, Message = "StockInHeaderId and PurchaseInvoiceHeaderId cannot both be null" };
            }
        }

        private async Task<ResponseDto> CheckStockInReturnFromPurchaseInvoiceEnded(int purchaseInvoiceHeaderId, int menuCode)
        {
            var hasPurchaseInvoiceReturnThatIsNotOnTheWay = await _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId && !x.IsOnTheWay).AnyAsync();
            var hasSupplierDebitMemo = await _supplierDebitMemoService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).AnyAsync();
            var hasSupplierCreditMemo = await _supplierCreditMemoService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).AnyAsync();

            if (hasPurchaseInvoiceReturnThatIsNotOnTheWay || hasSupplierCreditMemo || hasSupplierDebitMemo)
            {
                return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseInvoiced) };
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckStockInReturnFromStockInEnded(int? stockInReturnHeaderId, int? stockInHeaderId, int menuCode)
        {
            //check if stockInreturn -> stockIn -> (purchaseOrder or purchaseInvoice) is ended
            var isStockInEnded = await (from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId)
                                        from stockInPurchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == stockInHeader.PurchaseOrderHeaderId).DefaultIfEmpty()
                                        from stockInPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == stockInHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
                                        select (stockInPurchaseOrderHeader != null && stockInPurchaseOrderHeader.IsEnded) || (stockInPurchaseInvoiceHeader != null && stockInPurchaseInvoiceHeader.IsEnded)).FirstOrDefaultAsync();
			if (isStockInEnded)
			{
                return new ResponseDto { Id = stockInReturnHeaderId ?? 0, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseInvoiced) };
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckStockInReturnFromPurchaseInvoiceOnTheWayCreatedBeforeInvoiceReturnOnTheWay(int? purchaseInvoiceHeaderId, int menuCode, int parentMenuCode)
        {
            if (purchaseInvoiceHeaderId != null)
            {
                var hasInvoiceReturnOnTheWay = await _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId && x.IsOnTheWay).AnyAsync();
                var fromPurchaseInvoiceOnTheWay = await _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.IsOnTheWay).FirstOrDefaultAsync();

                if (fromPurchaseInvoiceOnTheWay && !hasInvoiceReturnOnTheWay)
                {
                    return new ResponseDto { Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, MenuCodeData.PurchaseInvoiceReturnOnTheWay, PurchaseMessageData.StockInReturnFromPurchaseInvoiceCreatedBeforeInvoiceReturn) };
                }
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckStockInReturnBlocked(int? stockInReturnHeaderId, int? stockInHeaderId, int? purchaseInvoiceHeaderId, int menuCode)
        {
            var isBlocked = await (
                from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId).DefaultIfEmpty()
                from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).DefaultIfEmpty()
                select (stockInHeader != null && stockInHeader.IsBlocked) || (purchaseInvoiceHeader != null && purchaseInvoiceHeader.IsBlocked)
            ).FirstOrDefaultAsync();

            if (isBlocked)
            {
                return new ResponseDto { Id = stockInReturnHeaderId ?? 0, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckStockInReturnQuantityForSaving(StockInReturnDto stockInReturn, int menuCode, int parentMenuCode, int? grandParentMenuCode)
        {
            ResponseDto quantityExceeded;

            if (stockInReturn.StockInReturnHeader?.StockTypeId == StockTypeData.ReturnFromPurchaseInvoice)
            {
                quantityExceeded = await CheckStockInReturnFromPurchaseInvoiceQuantityExceeding(stockInReturn, menuCode, parentMenuCode);
            }
            else
            {
                quantityExceeded = await CheckStockInReturnFromStockInQuantityExceeding(stockInReturn, menuCode, parentMenuCode, grandParentMenuCode);
            }

            if (!quantityExceeded.Success)
            {
                return quantityExceeded;
            }

            return new ResponseDto { Success = true };
        }

        private async Task UpdatePurchaseInvoiceReturnOnTheWayEnded(int? purchaseInvoiceHeaderId, bool isEnded)
        {
            if (purchaseInvoiceHeaderId != null)
            {
                await _purchaseInvoiceReturnHeaderService.UpdatePurchaseInvoiceReturnOnTheWayEndedLinkedToPurchaseInvoice(purchaseInvoiceHeaderId, isEnded);
            }
        }

        private void TrimDetailStrings(List<StockInReturnDetailDto> stockInReturnDetails)
        {
            foreach (var stockInReturnDetail in stockInReturnDetails)
            {
                stockInReturnDetail.BatchNumber = string.IsNullOrWhiteSpace(stockInReturnDetail.BatchNumber) ? null : stockInReturnDetail.BatchNumber.Trim();
				stockInReturnDetail.ItemNote = string.IsNullOrWhiteSpace(stockInReturnDetail.ItemNote) ? null : stockInReturnDetail.ItemNote.Trim();
			}
		}

        private async Task StockInReturnUpdateModelPrices(StockInReturnDto stockInReturn)
        {
            await StockInReturnUpdateDetailPrices(stockInReturn.StockInReturnDetails, stockInReturn.StockInReturnHeader!.StoreId);
            stockInReturn.StockInReturnHeader!.TotalCostValue = stockInReturn.StockInReturnDetails.Sum(x => x.CostValue);
        }

        private async Task StockInReturnUpdateDetailPrices(List<StockInReturnDetailDto> stockInReturnDetails, int storeId)
        {
            var itemIds = stockInReturnDetails.Select(x => x.ItemId).ToList();

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

            foreach (var stockInReturnDetail in stockInReturnDetails)
            {
                var packing = packings.Where(x => x.ItemId == stockInReturnDetail.ItemId && x.FromPackageId == stockInReturnDetail.ItemPackageId).Select(x => x.Packing).FirstOrDefault(1);

                stockInReturnDetail.ConsumerPrice = consumerPrices.Where(x => x.ItemId == stockInReturnDetail.ItemId).Select(x => x.ConsumerPrice).FirstOrDefault(0);
                stockInReturnDetail.CostPrice = itemCosts.Where(x => x.ItemId == stockInReturnDetail.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefault(0);
                stockInReturnDetail.CostPackage = stockInReturnDetail.CostPrice * packing;
                stockInReturnDetail.CostValue = stockInReturnDetail.CostPackage * (stockInReturnDetail.Quantity + stockInReturnDetail.BonusQuantity);
                stockInReturnDetail.LastPurchasePrice = lastPurchasePrices.Where(x => x.ItemId == stockInReturnDetail.ItemId && x.ItemPackageId == stockInReturnDetail.ItemPackageId).Select(x => x.PurchasePrice).FirstOrDefault(0);
            }
        }

        private async Task UpdatePurchaseOrderStatusFromStockInReturn(int? stockInHeaderId, int? purchaseInvoiceHeaderId, int menuCode)
        {
            var purchaseOrderHeaderId = await GetPurchaseOrderRelatedToStockInReturn(stockInHeaderId, purchaseInvoiceHeaderId);
            await _purchaseOrderStatusService.UpdatePurchaseOrderStatus(purchaseOrderHeaderId, -1, menuCode);
        }

        private async Task<int> GetPurchaseOrderRelatedToStockInReturn(int? stockInHeaderId, int? purchaseInvoiceHeaderId)
        {
            if (stockInHeaderId != null)
            {
                var stockInHeader = await _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId).Select(x => new {x.PurchaseOrderHeaderId, x.PurchaseInvoiceHeaderId}).FirstOrDefaultAsync();
                return await GetPurchaseOrderRelatedToStockIn(stockInHeader!.PurchaseOrderHeaderId, stockInHeader!.PurchaseInvoiceHeaderId);
            }
            else
            {
                return await _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.PurchaseOrderHeaderId).FirstOrDefaultAsync();
            }
        }

        private async Task<int> GetPurchaseOrderRelatedToStockIn(int? purchaseOrderHeaderId, int? purchaseInvoiceHeaderId)
        {
            if (purchaseOrderHeaderId != null) return (int)purchaseOrderHeaderId;

            return await _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.PurchaseOrderHeaderId).FirstOrDefaultAsync();
        }

        public async Task<ResponseDto> DeleteStockInReturn(int stockInReturnHeaderId, int menuCode)
        {
            var stockInReturnHeader = await _stockInReturnHeaderService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeaderId).Select(x => new { x.StockInHeaderId, x.PurchaseInvoiceHeaderId, x.IsBlocked, x.StockTypeId }).FirstOrDefaultAsync();
            if (stockInReturnHeader == null) return new ResponseDto{ Id = stockInReturnHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };

            var grandParentMenuCode = await GetGrandParentMenuCode(stockInReturnHeader.StockInHeaderId);
            ResponseDto stockInReturnValidResult = await CheckStockInReturnIsValidForDelete(stockInReturnHeaderId, stockInReturnHeader.StockInHeaderId, stockInReturnHeader.PurchaseInvoiceHeaderId, stockInReturnHeader.IsBlocked, menuCode, grandParentMenuCode);
            if (stockInReturnValidResult.Success == false) return stockInReturnValidResult;

            ResponseDto result = await _stockInReturnService.DeleteStockInReturn(stockInReturnHeaderId, menuCode);
            
            if(result.Success)
            {
                await ReopenStockInReturnParent(stockInReturnHeader.StockInHeaderId, stockInReturnHeader.PurchaseInvoiceHeaderId);
                await UpdatePurchaseOrderStatusFromStockInReturn(stockInReturnHeader.StockInHeaderId, stockInReturnHeader.PurchaseInvoiceHeaderId, StockTypeData.ToMenuCode(stockInReturnHeader.StockTypeId));
				await UpdatePurchaseInvoiceReturnOnTheWayEnded(stockInReturnHeader.PurchaseInvoiceHeaderId, false);
			}
            return result;
        }

        private async Task<ResponseDto> CheckStockInReturnIsValidForDelete(int stockInReturnHeaderId, int? stockInHeaderId, int? purchaseInvoiceHeaderId, bool isBlocked, int menuCode, int? grandParentMenuCode)
        {
            if (isBlocked)
            {
                return new ResponseDto { Id = stockInReturnHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
            }

            ResponseDto isEnded = await CheckStockInReturnEnded(stockInReturnHeaderId, stockInHeaderId, purchaseInvoiceHeaderId, menuCode);
            if (isEnded.Success == false) return isEnded;

            if (stockInHeaderId != null)
            {
                ResponseDto quantityReceivedExceeding = await CheckForReceivedQuantityExccededAfterDeleting(stockInReturnHeaderId, (int)stockInHeaderId, menuCode, (int)grandParentMenuCode!);
                if (quantityReceivedExceeding.Success == false) return quantityReceivedExceeding;
            }

            return new ResponseDto { Success = true };
        }

        //Check if Deleting the StockInReturn will cause quantity received to exceed the purchaseOrder/purchaseInvoice that is the parent of stockIn
        private async Task<ResponseDto> CheckForReceivedQuantityExccededAfterDeleting(int stockInReturnHeaderId, int stockInHeaderId, int menuCode, int grandParentMenuCode)
        {
            var stockInHeader = await _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId).Select(x => new { x.PurchaseOrderHeaderId, x.PurchaseInvoiceHeaderId }).FirstOrDefaultAsync();
            ResponseDto? quantityResult = null;

            if (stockInHeader?.PurchaseOrderHeaderId != null)
            {
                quantityResult = await CheckStockInReturnFromStockInFromPurchaseOrderForDeletion(stockInReturnHeaderId, (int)stockInHeader.PurchaseOrderHeaderId, menuCode, grandParentMenuCode);
            }
            else if (stockInHeader?.PurchaseInvoiceHeaderId != null)
            {
                quantityResult = await CheckStockInReturnFromStockInFromPurchaseInvoiceForDeletion(stockInReturnHeaderId, (int)stockInHeader.PurchaseInvoiceHeaderId, menuCode, grandParentMenuCode);
            }

            if (quantityResult?.Success == false)
            {
                return quantityResult;
            }

            return new ResponseDto { Success = true };
        }

        private async Task ReopenStockInReturnParent(int? stockInHeaderId, int? purchaseInvoiceHeaderId)
        {
            if (stockInHeaderId != null)
            {
                var isReturnsRemaining = await _stockInReturnHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId).AnyAsync();
                if (!isReturnsRemaining)
                {
                    await _stockInHeaderService.UpdateClosed(stockInHeaderId, false);
                }
            }
            else if (purchaseInvoiceHeaderId != null)
            {
				//if there is any stock in, stock in return or purchase invoice return related to this purchase invoice header, then do not open it
				var isStocksRemaining = await _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.StockInReturnHeaderId)
											   	.Concat(_stockInHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.StockInHeaderId))
											 	.Concat(_purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.PurchaseInvoiceReturnHeaderId)).AnyAsync();
                if (!isStocksRemaining)
                {
                    await _purchaseInvoiceHeaderService.UpdateClosed(purchaseInvoiceHeaderId, false);
                }
            }
        }

        private async Task<ResponseDto> CheckStockInReturnFromStockInFromPurchaseOrderForDeletion(int stockInReturnHeaderId, int purchaseOrderHeaderId, int menuCode, int grandParentMenuCode)
        {
            var finalstocksReceived = _stockInQuantityService.GetFinalStocksReceivedFromPurchaseOrderExceptStockInReturnHeaderId(purchaseOrderHeaderId, stockInReturnHeaderId);
            var purchaseOrderDetailsGrouped = _purchaseOrderDetailService.GetPurchaseOrderDetailsGroupedQueryable(purchaseOrderHeaderId);

			var exceedingItem = await (
                    from finalstockReceived in finalstocksReceived
                    from purchaseOrderDetailGroup in purchaseOrderDetailsGrouped.Where(x => x.ItemId == finalstockReceived.ItemId && x.ItemPackageId == finalstockReceived.ItemPackageId && x.BarCode == finalstockReceived.BarCode && x.PurchasePrice == finalstockReceived.PurchasePrice && x.ItemDiscountPercent == finalstockReceived.ItemDiscountPercent)
                    select new
                    {
                        finalstockReceived.ItemId,
                        finalstockReceived.ItemPackageId,
                        PurchaseOrderQuantity = purchaseOrderDetailGroup.Quantity,
                        PurchaseOrderBonusQuantity = purchaseOrderDetailGroup.BonusQuantity,
                        QuantityReturnAvailable = finalstockReceived.QuantityReceived,
                        BonusQuantityReturnAvailable = finalstockReceived.BonusQuantityReceived
                    }
                ).FirstOrDefaultAsync(x => x.QuantityReturnAvailable > x.PurchaseOrderQuantity || x.BonusQuantityReturnAvailable > x.PurchaseOrderBonusQuantity);

            if (exceedingItem != null)
            {
                var language = _httpContextAccessor.GetProgramCurrentLanguage();

                var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
                var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

                if (exceedingItem.QuantityReturnAvailable > exceedingItem.PurchaseOrderQuantity)
                {
                    return new ResponseDto { Id = stockInReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, grandParentMenuCode, PurchaseMessageData.DeleteCauseQuantityExceed, itemName!, itemPackageName!, exceedingItem.QuantityReturnAvailable.ToNormalizedString(), exceedingItem.PurchaseOrderQuantity.ToNormalizedString()) };
                }
                else
                {
                    return new ResponseDto { Id = stockInReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, grandParentMenuCode, PurchaseMessageData.DeleteCauseBonusQuantityExceed, itemName!, itemPackageName!, exceedingItem.BonusQuantityReturnAvailable.ToNormalizedString(), exceedingItem.PurchaseOrderBonusQuantity.ToNormalizedString()) };
                }
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckStockInReturnFromStockInFromPurchaseInvoiceForDeletion(int stockInReturnHeaderId, int purchaseInvoiceHeaderId, int menuCode, int grandParentMenuCode)
        {
            var finalstocksReceived = _stockInQuantityService.GetFinalStocksReceivedFromPurchaseInvoiceExceptStockInReturnHeaderId(purchaseInvoiceHeaderId, stockInReturnHeaderId);
            var purchaseInvoiceDetailsGrouped = _purchaseInvoiceDetailService.GetPurchaseInvoiceDetailsGroupedQueryable(purchaseInvoiceHeaderId);

            var exceedingItem = await (
                    from finalstockReceived in finalstocksReceived
                    from purchaseInvoiceDetailGroup in purchaseInvoiceDetailsGrouped.Where(x => x.ItemId == finalstockReceived.ItemId && x.ItemPackageId == finalstockReceived.ItemPackageId && x.BatchNumber == finalstockReceived.BatchNumber && x.ExpireDate == finalstockReceived.ExpireDate && x.CostCenterId == finalstockReceived.CostCenterId && x.BarCode == finalstockReceived.BarCode && x.PurchasePrice == finalstockReceived.PurchasePrice && x.ItemDiscountPercent == finalstockReceived.ItemDiscountPercent)
                    select new
                    {
                        finalstockReceived.ItemId,
                        finalstockReceived.ItemPackageId,
                        PurchaseInvoiceQuantity = purchaseInvoiceDetailGroup.Quantity,
                        PurchaseInvoiceBonusQuantity = purchaseInvoiceDetailGroup.BonusQuantity,
                        QuantityReturnAvailable = finalstockReceived.QuantityReceived,
                        BonusQuantityReturnAvailable = finalstockReceived.BonusQuantityReceived
                    }
                ).FirstOrDefaultAsync(x => x.QuantityReturnAvailable > x.PurchaseInvoiceQuantity || x.BonusQuantityReturnAvailable > x.PurchaseInvoiceBonusQuantity);

            if (exceedingItem != null)
            {
                var language = _httpContextAccessor.GetProgramCurrentLanguage();

                var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
                var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

                if (exceedingItem.QuantityReturnAvailable > exceedingItem.PurchaseInvoiceQuantity)
                {
                    return new ResponseDto { Id = stockInReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, grandParentMenuCode, PurchaseMessageData.DeleteCauseQuantityExceed, itemName!, itemPackageName!, exceedingItem.QuantityReturnAvailable.ToNormalizedString(), exceedingItem.PurchaseInvoiceQuantity.ToNormalizedString()) };
                }
                else
                {
                    return new ResponseDto { Id = stockInReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, grandParentMenuCode, PurchaseMessageData.DeleteCauseBonusQuantityExceed, itemName!, itemPackageName!, exceedingItem.BonusQuantityReturnAvailable.ToNormalizedString(), exceedingItem.PurchaseInvoiceBonusQuantity.ToNormalizedString()) };
                }
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckStockInReturnFromStockInQuantityExceeding(StockInReturnDto stockInReturn, int menuCode, int parentMenuCode, int? grandParentMenuCode)
        {
            if (stockInReturn.StockInReturnHeader == null || !(stockInReturn.StockInReturnHeader.StockInHeaderId > 0)) return new ResponseDto{ Message = "StockType mismatches parent foreign keys" };

            var stockInReturnDetailsGrouped = _stockInReturnDetailService.GroupStockInReturnDetailsWithAllKeys(stockInReturn.StockInReturnDetails);
            var itemIds = stockInReturn.StockInReturnDetails.Select(x => x.ItemId).ToList();

            //Check if StockInReturn exceedes its parent StockIn
            var quantityResult = await CheckStockInReturnExceedingStockInItselfForSave(stockInReturn.StockInReturnHeader.StockInReturnHeaderId, stockInReturnDetailsGrouped, (int)stockInReturn.StockInReturnHeader.StockInHeaderId, itemIds, menuCode, parentMenuCode);
            if(quantityResult.Success == false)
            {
                return quantityResult;
            }

            //Check if modifying StockInReturn will cause quantity received to exceed the quantities in purchaseOrder/purchaseInvoice
            if (stockInReturn.StockInReturnHeader.StockInReturnHeaderId != 0)
            {
                var stockInReturnHeader = await _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInReturn.StockInReturnHeader.StockInHeaderId).Select(x => new { x.PurchaseInvoiceHeaderId, x.PurchaseOrderHeaderId }).FirstOrDefaultAsync();
                var purchaseInvoiceHeaderId = stockInReturnHeader?.PurchaseInvoiceHeaderId;
                var purchaseOrderHeaderId = stockInReturnHeader?.PurchaseOrderHeaderId;

                if (purchaseInvoiceHeaderId != null)
                {
                    quantityResult = await CheckStockInReturnFromStockInFromPurchaseInvoiceForSave(stockInReturn.StockInReturnHeader.StockInReturnHeaderId, stockInReturnDetailsGrouped, itemIds, (int)purchaseInvoiceHeaderId, menuCode, (int)grandParentMenuCode!);
                }
                else if (purchaseOrderHeaderId != null)
                {
                    quantityResult = await CheckStockInReturnFromStockInFromPurchaseOrderForSave(stockInReturn.StockInReturnHeader.StockInReturnHeaderId, stockInReturnDetailsGrouped, itemIds, (int)purchaseOrderHeaderId, menuCode, (int)grandParentMenuCode!);
                }

                if (quantityResult.Success == false)
                {
                    return quantityResult;
                }
            }

            return new ResponseDto { Id = stockInReturn.StockInReturnHeader.StockInReturnHeaderId, Success = true };
        }

        private async Task<ResponseDto> CheckStockInReturnExceedingStockInItselfForSave(int stockInReturnHeaderId, List<StockInReturnDetailDto> stockInReturnDetailsGrouped, int stockInHeaderId, List<int> itemIds, int menuCode, int parentMenuCode)
        {
            List<StockReturnedDto>? stocksReturned;
            if (stockInReturnHeaderId == 0)
            {
                stocksReturned = await _stockInQuantityService.GetStocksReturnedFromStockIn(stockInHeaderId!).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            }
            else
            {
                stocksReturned = await _stockInQuantityService.GetStocksReturnedFromStockInExceptStockInReturnHeaderId(stockInHeaderId!, stockInReturnHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            }

            var stockInDetailsGrouped = await _stockInDetailService.GetStockInDetailsGroupedWithAllKeys(stockInHeaderId);

            var availableReturnQuantities = (
                from stockInReturnDetailGroup in stockInReturnDetailsGrouped
				from stockReturned in stocksReturned.Where(x => x.ItemId == stockInReturnDetailGroup.ItemId && x.ItemPackageId == stockInReturnDetailGroup.ItemPackageId && x.BatchNumber == stockInReturnDetailGroup.BatchNumber && x.ExpireDate == stockInReturnDetailGroup.ExpireDate && x.CostCenterId == stockInReturnDetailGroup.CostCenterId && x.BarCode == stockInReturnDetailGroup.BarCode && x.PurchasePrice == stockInReturnDetailGroup.PurchasePrice && x.ItemDiscountPercent == stockInReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
                from stockInDetailGroup in stockInDetailsGrouped.Where(x => x.ItemId == stockInReturnDetailGroup.ItemId && x.ItemPackageId == stockInReturnDetailGroup.ItemPackageId && x.BatchNumber == stockInReturnDetailGroup.BatchNumber && x.ExpireDate == stockInReturnDetailGroup.ExpireDate && x.CostCenterId == stockInReturnDetailGroup.CostCenterId && x.BarCode == stockInReturnDetailGroup.BarCode && x.PurchasePrice == stockInReturnDetailGroup.PurchasePrice && x.ItemDiscountPercent == stockInReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
                select new
                {
                    stockInReturnDetailGroup.ItemId,
                    stockInReturnDetailGroup.ItemPackageId,
                    StockInQuantity = stockInDetailGroup != null ? stockInDetailGroup.Quantity : 0,
                    StockInBonusQuantity = stockInDetailGroup != null ? stockInDetailGroup.BonusQuantity : 0,
                    QuantityReturned = (stockReturned != null ? stockReturned.QuantityReturned : 0) + stockInReturnDetailGroup.Quantity,
                    BonusQuantityReturned = (stockReturned != null ? stockReturned.BonusQuantityReturned : 0) + stockInReturnDetailGroup.BonusQuantity,
                    QuantityReturnAvailable = (stockInDetailGroup != null ? stockInDetailGroup.Quantity : 0) - (stockReturned != null ? stockReturned.QuantityReturned : 0) - stockInReturnDetailGroup.Quantity,
                    BonusQuantityReturnAvailable = (stockInDetailGroup != null ? stockInDetailGroup.BonusQuantity : 0) - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0) - stockInReturnDetailGroup.BonusQuantity
                });

            var exceedingItem = availableReturnQuantities.FirstOrDefault(x => x.QuantityReturnAvailable < 0 || x.BonusQuantityReturnAvailable < 0);
            if (exceedingItem != null)
            {
                var language = _httpContextAccessor.GetProgramCurrentLanguage();

                var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
                var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

                if (exceedingItem.QuantityReturnAvailable < 0)
                {
                    return new ResponseDto { Id = stockInReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.QuantityExceeding, itemName!, itemPackageName!, exceedingItem.QuantityReturned.ToNormalizedString(), exceedingItem.StockInQuantity.ToNormalizedString()) };
                }
                else
                {
                    return new ResponseDto { Id = stockInReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.BonusQuantityExceeding, itemName!, itemPackageName!, exceedingItem.BonusQuantityReturned.ToNormalizedString(), exceedingItem.StockInBonusQuantity.ToNormalizedString()) };
                }
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckStockInReturnFromStockInFromPurchaseInvoiceForSave(int stockInReturnHeaderId, List<StockInReturnDetailDto> stockInReturnDetailGrouped, List<int> itemIds, int purchaseInvoiceHeaderId, int menuCode, int grandParentMenuCode)
        {
            var finalstocksReceived = await _stockInQuantityService.GetFinalStocksReceivedFromPurchaseInvoiceExceptStockInReturnHeaderId(purchaseInvoiceHeaderId, stockInReturnHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

            var purchaseInvoiceDetailsGrouped = await _purchaseInvoiceDetailService.GetPurchaseInvoiceDetailsGrouped(purchaseInvoiceHeaderId);

            var availableReturnQuantities = (
                from stockInReturnDetailGroup in stockInReturnDetailGrouped
				from finalstockReceived in finalstocksReceived.Where(x => x.ItemId == stockInReturnDetailGroup.ItemId && x.ItemPackageId == stockInReturnDetailGroup.ItemPackageId && x.BatchNumber == stockInReturnDetailGroup.BatchNumber && x.ExpireDate == stockInReturnDetailGroup.ExpireDate && x.CostCenterId == stockInReturnDetailGroup.CostCenterId && x.BarCode == stockInReturnDetailGroup.BarCode && x.PurchasePrice == stockInReturnDetailGroup.PurchasePrice && x.ItemDiscountPercent == stockInReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
                from purchaseInvoiceDetailGrouped in purchaseInvoiceDetailsGrouped.Where(x => x.ItemId == stockInReturnDetailGroup.ItemId && x.ItemPackageId == stockInReturnDetailGroup.ItemPackageId && x.BatchNumber == stockInReturnDetailGroup.BatchNumber && x.ExpireDate == stockInReturnDetailGroup.ExpireDate && x.CostCenterId == stockInReturnDetailGroup.CostCenterId && x.BarCode == stockInReturnDetailGroup.BarCode && x.PurchasePrice == stockInReturnDetailGroup.PurchasePrice && x.ItemDiscountPercent == stockInReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
                select new
                {
                    stockInReturnDetailGroup.ItemId,
                    stockInReturnDetailGroup.ItemPackageId,
                    PurchaseInvoiceQuantity = purchaseInvoiceDetailGrouped.Quantity,
                    PurchaseInvoiceBonusQuantity = purchaseInvoiceDetailGrouped.BonusQuantity,
                    QuantityReturnAvailable = (finalstockReceived != null ? finalstockReceived.QuantityReceived : 0) - stockInReturnDetailGroup.Quantity,
                    BonusQuantityReturnAvailable = (finalstockReceived != null ? finalstockReceived.BonusQuantityReceived : 0) - stockInReturnDetailGroup.BonusQuantity
                });

            var exceedingItem = availableReturnQuantities.FirstOrDefault(x => x.QuantityReturnAvailable > x.PurchaseInvoiceQuantity || x.BonusQuantityReturnAvailable > x.PurchaseInvoiceBonusQuantity);
            if (exceedingItem != null)
            {
                var language = _httpContextAccessor.GetProgramCurrentLanguage();

                var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
                var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

                if (exceedingItem.QuantityReturnAvailable > exceedingItem.PurchaseInvoiceQuantity)
                {
                    return new ResponseDto { Id = stockInReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, grandParentMenuCode, PurchaseMessageData.SaveCauseQuantityExceed, itemName!, itemPackageName!, exceedingItem.QuantityReturnAvailable.ToNormalizedString(), exceedingItem.PurchaseInvoiceQuantity.ToNormalizedString()) };
                }
                else
                {
                    return new ResponseDto { Id = stockInReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, grandParentMenuCode, PurchaseMessageData.SaveCauseBonusQuantityExceed, itemName!, itemPackageName!, exceedingItem.BonusQuantityReturnAvailable.ToNormalizedString(), exceedingItem.PurchaseInvoiceBonusQuantity.ToNormalizedString()) };
                }
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckStockInReturnFromStockInFromPurchaseOrderForSave(int stockInReturnHeaderId, List<StockInReturnDetailDto> stockInReturnDetailsGrouped, List<int> itemIds, int purchaseOrderHeaderId, int menuCode, int grandParentMenuCode)
        {
            var finalstocksReceived = await _stockInQuantityService.GetFinalStocksReceivedFromPurchaseOrderExceptStockInReturnHeaderId(purchaseOrderHeaderId, stockInReturnHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

            var purchaseOrderDetails = await _purchaseOrderDetailService.GetPurchaseOrderDetailsGrouped(purchaseOrderHeaderId);

            var stockInReturnGroupedWithLessKeys = _stockInReturnDetailService.GroupStockInReturnDetails(stockInReturnDetailsGrouped);

            var availableReturnQuantities = (
                from stockInReturnDetailGroup in stockInReturnGroupedWithLessKeys
                from finalstockReceived in finalstocksReceived.Where(x => x.ItemId == stockInReturnDetailGroup.ItemId && x.ItemPackageId == stockInReturnDetailGroup.ItemPackageId && x.BarCode == stockInReturnDetailGroup.BarCode && x.PurchasePrice == stockInReturnDetailGroup.PurchasePrice && x.ItemDiscountPercent == stockInReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
                from purchaseOrderDetail in purchaseOrderDetails.Where(x => x.ItemId == stockInReturnDetailGroup.ItemId && x.ItemPackageId == stockInReturnDetailGroup.ItemPackageId && x.BarCode == stockInReturnDetailGroup.BarCode && x.PurchasePrice == stockInReturnDetailGroup.PurchasePrice && x.ItemDiscountPercent == stockInReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
                select new
                {
                    stockInReturnDetailGroup.ItemId,
                    stockInReturnDetailGroup.ItemPackageId,
                    PurchaseOrderQuantity = purchaseOrderDetail.Quantity,
                    PurchaseOrderBonusQuantity = purchaseOrderDetail.BonusQuantity,
                    QuantityReturnAvailable = (finalstockReceived != null ? finalstockReceived.QuantityReceived : 0) - stockInReturnDetailGroup.Quantity,
                    BonusQuantityReturnAvailable = (finalstockReceived != null ? finalstockReceived.BonusQuantityReceived : 0) - stockInReturnDetailGroup.BonusQuantity
                });

            var exceedingItem = availableReturnQuantities.FirstOrDefault(x => x.QuantityReturnAvailable > x.PurchaseOrderQuantity || x.BonusQuantityReturnAvailable > x.PurchaseOrderBonusQuantity);
            if (exceedingItem != null)
            {
                var language = _httpContextAccessor.GetProgramCurrentLanguage();

                var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
                var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();


                if (exceedingItem.QuantityReturnAvailable > exceedingItem.PurchaseOrderQuantity)
                {
                    return new ResponseDto { Id = stockInReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, grandParentMenuCode, PurchaseMessageData.SaveCauseQuantityExceed, itemName!, itemPackageName!, exceedingItem.QuantityReturnAvailable.ToNormalizedString(), exceedingItem.PurchaseOrderQuantity.ToNormalizedString()) };
                }
                else
                {
                    return new ResponseDto { Id = stockInReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, grandParentMenuCode, PurchaseMessageData.SaveCauseBonusQuantityExceed, itemName!, itemPackageName!, exceedingItem.BonusQuantityReturnAvailable.ToNormalizedString(), exceedingItem.PurchaseOrderBonusQuantity.ToNormalizedString()) };
                }
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckStockInReturnFromPurchaseInvoiceQuantityExceeding(StockInReturnDto stockInReturn, int menuCode, int parentMenuCode)
        {
            if (!(stockInReturn.StockInReturnHeader?.PurchaseInvoiceHeaderId > 0)) return new ResponseDto { Message = "StockType mismatches parent foreign keys" };

            var stockInReturnDetailsGrouped = _stockInReturnDetailService.GroupStockInReturnDetailsWithAllKeys(stockInReturn.StockInReturnDetails);
            var itemIds = stockInReturnDetailsGrouped.Select(x => x.ItemId).ToList();

            List<StockReturnedDto>? stocksReturned;
            if (stockInReturn.StockInReturnHeader.StockInReturnHeaderId == 0)
            {
                stocksReturned = await _stockInQuantityService.GetStocksReturnedFromPurchaseInvoice((int)stockInReturn.StockInReturnHeader!.PurchaseInvoiceHeaderId!).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            }
            else
            {
                stocksReturned = await _stockInQuantityService.GetStocksReturnedFromPurchaseInvoiceExceptStockInReturnHeaderId((int)stockInReturn.StockInReturnHeader.PurchaseInvoiceHeaderId, stockInReturn.StockInReturnHeader.StockInReturnHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            }

            var purchaseInvoiceDetailsGrouped = await _purchaseInvoiceDetailService.GetPurchaseInvoiceDetailsGrouped((int)stockInReturn.StockInReturnHeader.PurchaseInvoiceHeaderId);
            var purchaseInvoiceReturnOnTheWayDetailsGrouped = await GetPurchaseInvoiceReturnOnTheWayQuantities((int)stockInReturn.StockInReturnHeader.PurchaseInvoiceHeaderId);

            var availableReturnQuantities = (
                from stockInReturnDetail in stockInReturnDetailsGrouped
				from stockReturned in stocksReturned.Where(x => x.ItemId == stockInReturnDetail.ItemId && x.ItemPackageId == stockInReturnDetail.ItemPackageId && x.BatchNumber == stockInReturnDetail.BatchNumber && x.ExpireDate == stockInReturnDetail.ExpireDate && x.CostCenterId == stockInReturnDetail.CostCenterId && x.BarCode == stockInReturnDetail.BarCode && x.PurchasePrice == stockInReturnDetail.PurchasePrice && x.ItemDiscountPercent == stockInReturnDetail.ItemDiscountPercent).DefaultIfEmpty()
                from purchaseInvoiceDetail in purchaseInvoiceDetailsGrouped.Where(x => x.ItemId == stockInReturnDetail.ItemId && x.ItemPackageId == stockInReturnDetail.ItemPackageId && x.BatchNumber == stockInReturnDetail.BatchNumber && x.ExpireDate == stockInReturnDetail.ExpireDate && x.CostCenterId == stockInReturnDetail.CostCenterId && x.BarCode == stockInReturnDetail.BarCode && x.PurchasePrice == stockInReturnDetail.PurchasePrice && x.ItemDiscountPercent == stockInReturnDetail.ItemDiscountPercent).DefaultIfEmpty()
                from purchaseInvoiceReturnDetailGrouped in purchaseInvoiceReturnOnTheWayDetailsGrouped.Where(x => x.ItemId == stockInReturnDetail.ItemId && x.ItemPackageId == stockInReturnDetail.ItemPackageId && x.BatchNumber == stockInReturnDetail.BatchNumber && x.ExpireDate == stockInReturnDetail.ExpireDate && x.CostCenterId == stockInReturnDetail.CostCenterId && x.BarCode == stockInReturnDetail.BarCode && x.PurchasePrice == stockInReturnDetail.PurchasePrice && x.ItemDiscountPercent == stockInReturnDetail.ItemDiscountPercent).DefaultIfEmpty()
                select new
                {
                    stockInReturnDetail.ItemId,
                    stockInReturnDetail.ItemPackageId,
                    PurchaseInvoiceQuantity = purchaseInvoiceDetail != null ? purchaseInvoiceDetail.Quantity - (purchaseInvoiceReturnDetailGrouped != null ? purchaseInvoiceReturnDetailGrouped.Quantity : 0) : 0,
                    PurchaseInvoiceBonusQuantity = purchaseInvoiceDetail != null ? purchaseInvoiceDetail.BonusQuantity - (purchaseInvoiceReturnDetailGrouped != null ? purchaseInvoiceReturnDetailGrouped.BonusQuantity : 0) : 0,
                    QuantityReturned = (stockReturned != null ? stockReturned.QuantityReturned : 0) + stockInReturnDetail.Quantity,
                    BonusQuantityReturned = (stockReturned != null ? stockReturned.BonusQuantityReturned : 0) + stockInReturnDetail.BonusQuantity,
                    QuantityReturnAvailable = (purchaseInvoiceDetail != null ? purchaseInvoiceDetail.Quantity : 0) - (purchaseInvoiceReturnDetailGrouped != null ? purchaseInvoiceReturnDetailGrouped.Quantity : 0) - (stockReturned != null ? stockReturned.QuantityReturned : 0) - stockInReturnDetail.Quantity,
                    BonusQuantityReturnAvailable = (purchaseInvoiceDetail != null ? purchaseInvoiceDetail.BonusQuantity : 0) - (purchaseInvoiceReturnDetailGrouped != null ? purchaseInvoiceReturnDetailGrouped.BonusQuantity : 0) - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0) - stockInReturnDetail.BonusQuantity
                });

            var exceedingItem = availableReturnQuantities.FirstOrDefault(x => x.QuantityReturnAvailable < 0 || x.BonusQuantityReturnAvailable < 0);
            if (exceedingItem != null)
            {
                var language = _httpContextAccessor.GetProgramCurrentLanguage();

                var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
                var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

                if (exceedingItem.QuantityReturnAvailable < 0)
                {
                    return new ResponseDto { Id = stockInReturn.StockInReturnHeader.StockInReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.QuantityExceeding, itemName!, itemPackageName!, exceedingItem.QuantityReturned.ToNormalizedString(), exceedingItem.PurchaseInvoiceQuantity.ToNormalizedString()) };
                }
                else
                {
                    return new ResponseDto { Id = stockInReturn.StockInReturnHeader.StockInReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.BonusQuantityExceeding, itemName!, itemPackageName!, exceedingItem.BonusQuantityReturned.ToNormalizedString(), exceedingItem.PurchaseInvoiceBonusQuantity.ToNormalizedString()) };
                }
            }
            return new ResponseDto { Success = true };
        }
    }
}
