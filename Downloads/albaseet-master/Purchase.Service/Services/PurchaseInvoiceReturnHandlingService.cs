using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Purchases.CoreOne.Contracts;
using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Extensions;
using Shared.Helper.Identity;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Calculation;
using Shared.Service.Services.Inventory;
using Shared.Service.Services.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Service.Services.Taxes;
using Purchases.CoreOne.Models.StaticData;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.Domain.Menus;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Contracts;

namespace Purchases.Service.Services
{
    public class PurchaseInvoiceReturnHandlingService : IPurchaseInvoiceReturnHandlingService
    {
        private readonly IPurchaseMessageService _purchaseMessageService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly IInvoiceStockInReturnService _invoiceStockInReturnService;
        private readonly IPurchaseInvoiceReturnService _purchaseInvoiceReturnService;
        private readonly IPurchaseInvoiceService _purchaseInvoiceService;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
        private readonly IPurchaseInvoiceDetailTaxService _purchaseInvoiceDetailTaxService;
        private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
        private readonly IStockInHeaderService _stockInHeaderService;
        private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
        private readonly IStockInQuantityService _stockInQuantityService;
        private readonly IStockInReturnService _stockInReturnService;
        private readonly IItemCostService _itemCostService;
        private readonly IItemService _itemService;
        private readonly IItemPackingService _itemPackingService;
        private readonly ITaxPercentService _taxPercentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemPackageService _itemPackageService;
        private readonly IStoreService _storeService;
        private readonly ISupplierDebitMemoService _supplierDebitMemoService;
        private readonly ISupplierCreditMemoService _supplierCreditMemoService;
        private readonly IPurchaseOrderStatusService _purchaseOrderStatusService;
        private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IItemTaxService _itemTaxService;
        private readonly ITaxService _taxService;
        private readonly IPurchaseInvoiceReturnDetailService _purchaseInvoiceReturnDetailService;
        private readonly IGetPurchaseInvoiceSettleValueService _getPurchaseInvoiceSettleValueService;
        private readonly IItemNoteValidationService _itemNoteValidationService;

       public PurchaseInvoiceReturnHandlingService(IPurchaseMessageService purchaseMessageService, IGenericMessageService genericMessageService, IInvoiceStockInReturnService invoiceStockInReturnService, IPurchaseInvoiceReturnService purchaseInvoiceReturnService, IPurchaseInvoiceService purchaseInvoiceService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IPurchaseInvoiceDetailTaxService purchaseInvoiceDetailTaxService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IStockInHeaderService stockInHeaderService, IStockInReturnHeaderService stockInReturnHeaderService, IStockInQuantityService stockInQuantityService, IStockInReturnService stockInReturnService, IItemCostService itemCostService, IItemService itemService, IItemPackingService itemPackingService, ITaxPercentService taxPercentService, IHttpContextAccessor httpContextAccessor, IItemPackageService itemPackageService, IStoreService storeService, ISupplierDebitMemoService supplierDebitMemoService, ISupplierCreditMemoService supplierCreditMemoService, IPurchaseOrderStatusService purchaseOrderStatusService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IItemBarCodeService itemBarCodeService, IItemTaxService itemTaxService, ITaxService taxService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, IGetPurchaseInvoiceSettleValueService getPurchaseInvoiceSettleValueService, IItemNoteValidationService itemNoteValidationService)
        {
            _purchaseMessageService = purchaseMessageService;
            _genericMessageService = genericMessageService;
            _invoiceStockInReturnService = invoiceStockInReturnService;
            _purchaseInvoiceReturnService = purchaseInvoiceReturnService;
            _purchaseInvoiceService = purchaseInvoiceService;
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _purchaseInvoiceDetailService = purchaseInvoiceDetailService;
            _purchaseInvoiceDetailTaxService = purchaseInvoiceDetailTaxService;
            _purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
            _stockInHeaderService = stockInHeaderService;
            _stockInReturnHeaderService = stockInReturnHeaderService;
            _stockInQuantityService = stockInQuantityService;
            _stockInReturnService = stockInReturnService;
            _itemCostService = itemCostService;
            _itemService = itemService;
            _itemPackingService = itemPackingService;
            _taxPercentService = taxPercentService;
            _itemPackageService = itemPackageService;
            _httpContextAccessor = httpContextAccessor;
            _storeService = storeService;
            _supplierDebitMemoService = supplierDebitMemoService;
            _supplierCreditMemoService = supplierCreditMemoService;
            _purchaseOrderStatusService = purchaseOrderStatusService;
            _purchaseOrderHeaderService = purchaseOrderHeaderService;
            _itemBarCodeService = itemBarCodeService;
            _itemTaxService = itemTaxService;
            _taxService = taxService;
			_purchaseInvoiceReturnDetailService = purchaseInvoiceReturnDetailService;
            _getPurchaseInvoiceSettleValueService = getPurchaseInvoiceSettleValueService;
            _itemNoteValidationService = itemNoteValidationService;
		}

        public async Task<PurchaseInvoiceReturnDto> GetPurchaseInvoiceReturnFromPurchaseInvoice(int purchaseInvoiceHeaderId, bool isDirectInvoice, bool isOnTheWay)
        {
            var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaderById(purchaseInvoiceHeaderId);
            if (purchaseInvoiceHeader == null)
            {
                return new PurchaseInvoiceReturnDto();
            }

            var purchaseInvoiceDetails = await _purchaseInvoiceDetailService.GetPurchaseInvoiceDetails(purchaseInvoiceHeaderId);
            var purchaseInvoiceDetailsGrouped = _purchaseInvoiceDetailService.GroupPurchaseInvoiceDetails(purchaseInvoiceDetails);

            var itemIds = purchaseInvoiceDetails.Select(x => x.ItemId).ToList();

            var items = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var itemPackings = await _itemPackingService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var lastPurchasePrices = await _purchaseInvoiceService.GetMultipleLastPurchasePrices(itemIds);

            decimal vatPercent = await _taxPercentService.GetVatTaxByStoreId(purchaseInvoiceHeader.StoreId, DateHelper.GetDateTimeNow());

            List<PurchaseInvoiceReturnDetailDto> purchaseInvoiceReturnDetails;

            if (isOnTheWay)
            {
                var finalStocksReceived = await _stockInQuantityService.GetFinalStocksReceivedFromPurchaseInvoice(purchaseInvoiceHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

                purchaseInvoiceReturnDetails = (
                                from purchaseInvoiceDetail in purchaseInvoiceDetails
                                from item in items.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId)
                                from itemCost in itemCosts.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.StoreId == purchaseInvoiceHeader.StoreId).DefaultIfEmpty()
                                from itemPacking in itemPackings.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.FromPackageId == purchaseInvoiceDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                                from finalStockReceived in finalStocksReceived.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId && x.BatchNumber == purchaseInvoiceDetail.BatchNumber && x.ExpireDate == purchaseInvoiceDetail.ExpireDate&& x.CostCenterId == purchaseInvoiceDetail.CostCenterId && x.BarCode == purchaseInvoiceDetail.BarCode && x.PurchasePrice == purchaseInvoiceDetail.PurchasePrice && x.ItemDiscountPercent == purchaseInvoiceDetail.ItemDiscountPercent).DefaultIfEmpty()
                                from purchaseInvoiceDetailGroup in purchaseInvoiceDetailsGrouped.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId && x.BatchNumber == purchaseInvoiceDetail.BatchNumber && x.ExpireDate == purchaseInvoiceDetail.ExpireDate&& x.CostCenterId == purchaseInvoiceDetail.CostCenterId && x.BarCode == purchaseInvoiceDetail.BarCode && x.PurchasePrice == purchaseInvoiceDetail.PurchasePrice && x.ItemDiscountPercent == purchaseInvoiceDetail.ItemDiscountPercent)
                                from lastPurchasePrice in lastPurchasePrices.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId).DefaultIfEmpty()
                                select new PurchaseInvoiceReturnDetailDto()
                                {
                                    PurchaseInvoiceReturnDetailId = purchaseInvoiceDetail.PurchaseInvoiceDetailId, // <-- used to join with detail taxes
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
									//key level quantities are internal to the backend and marked with JsonIgnore, I just need it 
									//for the quantity distribution algorithm
									KeyLevelQuantity = purchaseInvoiceDetailGroup.Quantity - (finalStockReceived != null ? finalStockReceived.QuantityReceived : 0),
                                    KeyLevelBonusQuantity = purchaseInvoiceDetailGroup.BonusQuantity - (finalStockReceived != null ? finalStockReceived.BonusQuantityReceived : 0),
									PurchaseInvoiceQuantity = purchaseInvoiceDetailGroup.Quantity,
									PurchaseInvoiceBonusQuantity = purchaseInvoiceDetailGroup.BonusQuantity,
									AvailableQuantity = 0,
									AvailableBonusQuantity = 0,
									PurchasePrice = purchaseInvoiceDetail.PurchasePrice,
                                    ItemDiscountPercent = purchaseInvoiceDetail.ItemDiscountPercent,
                                    VatPercent = purchaseInvoiceDetail.VatPercent,
                                    Notes = purchaseInvoiceDetail.Notes,
                                    ItemNote = purchaseInvoiceDetail.ItemNote,
                                    VatTaxId = purchaseInvoiceDetail.VatTaxId,
                                    VatTaxTypeId = purchaseInvoiceDetail.VatTaxTypeId,
                                    ConsumerPrice = item.ConsumerPrice,
                                    CostPrice = itemCost != null ? itemCost.CostPrice : 0,
                                    CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
                                    LastPurchasePrice = lastPurchasePrice != null ? lastPurchasePrice.PurchasePrice : 0,
                                    Packages = purchaseInvoiceDetail.Packages,
                                    Taxes = purchaseInvoiceDetail.Taxes,
                                    ItemTaxData = purchaseInvoiceDetail.ItemTaxData,
                                }).ToList();

				purchaseInvoiceReturnDetails = DistributeQuantityAndCalculateValues(purchaseInvoiceReturnDetails, purchaseInvoiceHeader.DiscountPercent);
				purchaseInvoiceReturnDetails = purchaseInvoiceReturnDetails.Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();
			}
            else if (!isDirectInvoice)
            {
                var stocksReturned = await _stockInQuantityService.GetStocksReturnedFromPurchaseInvoice(purchaseInvoiceHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

                purchaseInvoiceReturnDetails = (
                                from purchaseInvoiceDetail in purchaseInvoiceDetails
                                from item in items.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId)
                                from itemCost in itemCosts.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.StoreId == purchaseInvoiceHeader.StoreId).DefaultIfEmpty()
                                from itemPacking in itemPackings.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.FromPackageId == purchaseInvoiceDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                                from stockReturned in stocksReturned.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId && x.BatchNumber == purchaseInvoiceDetail.BatchNumber && x.ExpireDate == purchaseInvoiceDetail.ExpireDate && x.CostCenterId == purchaseInvoiceDetail.CostCenterId && x.BarCode == purchaseInvoiceDetail.BarCode && x.PurchasePrice == purchaseInvoiceDetail.PurchasePrice && x.ItemDiscountPercent == purchaseInvoiceDetail.ItemDiscountPercent).DefaultIfEmpty()
                                from purchaseInvoiceDetailGroup in purchaseInvoiceDetailsGrouped.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId && x.BatchNumber == purchaseInvoiceDetail.BatchNumber && x.ExpireDate == purchaseInvoiceDetail.ExpireDate&& x.CostCenterId == purchaseInvoiceDetail.CostCenterId && x.BarCode == purchaseInvoiceDetail.BarCode && x.PurchasePrice == purchaseInvoiceDetail.PurchasePrice && x.ItemDiscountPercent == purchaseInvoiceDetail.ItemDiscountPercent)
                                from lastPurchasePrice in lastPurchasePrices.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId).DefaultIfEmpty()
                                select new PurchaseInvoiceReturnDetailDto()
                                {
                                    PurchaseInvoiceReturnDetailId = purchaseInvoiceDetail.PurchaseInvoiceDetailId, // <-- used to join with detail taxes
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
									//key level quantities are internal to the backend and marked with JsonIgnore, I just need it 
									//for the quantity distribution algorithm
									KeyLevelQuantity = stockReturned != null ? stockReturned.QuantityReturned : 0,
                                    KeyLevelBonusQuantity = stockReturned != null ? stockReturned.BonusQuantityReturned : 0,
                                    PurchaseInvoiceQuantity = purchaseInvoiceDetailGroup.Quantity,
                                    PurchaseInvoiceBonusQuantity = purchaseInvoiceDetailGroup.BonusQuantity,
                                    AvailableQuantity = 0,
                                    AvailableBonusQuantity = 0,
									PurchasePrice = purchaseInvoiceDetail.PurchasePrice,
                                    ItemDiscountPercent = purchaseInvoiceDetail.ItemDiscountPercent,
                                    VatPercent = purchaseInvoiceDetail.VatPercent,
                                    Notes = purchaseInvoiceDetail.Notes,
                                    ItemNote = purchaseInvoiceDetail.ItemNote,
                                    VatTaxId = purchaseInvoiceDetail.VatTaxId,
                                    VatTaxTypeId = purchaseInvoiceDetail.VatTaxTypeId,
                                    ConsumerPrice = item.ConsumerPrice,
                                    CostPrice = itemCost != null ? itemCost.CostPrice : 0,
                                    CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
                                    LastPurchasePrice = lastPurchasePrice != null ? lastPurchasePrice.PurchasePrice : 0,
                                    Packages = purchaseInvoiceDetail.Packages,
                                    Taxes = purchaseInvoiceDetail.Taxes,
                                    ItemTaxData = purchaseInvoiceDetail.ItemTaxData,
                                }).ToList();

				purchaseInvoiceReturnDetails = DistributeQuantityAndCalculateValues(purchaseInvoiceReturnDetails, purchaseInvoiceHeader.DiscountPercent);
				purchaseInvoiceReturnDetails = purchaseInvoiceReturnDetails.Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();
			}
            else
            {
                purchaseInvoiceReturnDetails = (
                                from purchaseInvoiceDetail in purchaseInvoiceDetails
                                from purchaseInvoiceDetailGroup in purchaseInvoiceDetailsGrouped.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId && x.BatchNumber == purchaseInvoiceDetail.BatchNumber && x.ExpireDate == purchaseInvoiceDetail.ExpireDate&& x.CostCenterId == purchaseInvoiceDetail.CostCenterId && x.BarCode == purchaseInvoiceDetail.BarCode && x.PurchasePrice == purchaseInvoiceDetail.PurchasePrice && x.ItemDiscountPercent == purchaseInvoiceDetail.ItemDiscountPercent)
                                from item in items.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId)
                                from itemCost in itemCosts.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.StoreId == purchaseInvoiceHeader.StoreId).DefaultIfEmpty()
                                from itemPacking in itemPackings.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.FromPackageId == purchaseInvoiceDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                                from lastPurchasePrice in lastPurchasePrices.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId).DefaultIfEmpty()
                                select new PurchaseInvoiceReturnDetailDto()
                                {
                                    PurchaseInvoiceReturnDetailId = purchaseInvoiceDetail.PurchaseInvoiceDetailId, // <-- used to join with detail taxes
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
									AvailableQuantity = purchaseInvoiceDetailGroup.Quantity,
									AvailableBonusQuantity = purchaseInvoiceDetailGroup.BonusQuantity,
									PurchasePrice = purchaseInvoiceDetail.PurchasePrice,
                                    TotalValue = CalculateDetailValue.TotalValue(purchaseInvoiceDetail.Quantity, purchaseInvoiceDetail.PurchasePrice, false, purchaseInvoiceDetail.VatPercent),
                                    ItemDiscountPercent = purchaseInvoiceDetail.ItemDiscountPercent,
                                    ItemDiscountValue = CalculateDetailValue.DiscountValue(purchaseInvoiceDetail.Quantity, purchaseInvoiceDetail.PurchasePrice, purchaseInvoiceDetail.ItemDiscountPercent, false, purchaseInvoiceDetail.VatPercent),
                                    TotalValueAfterDiscount = CalculateDetailValue.TotalValueAfterDiscount(purchaseInvoiceDetail.Quantity, purchaseInvoiceDetail.PurchasePrice, purchaseInvoiceDetail.ItemDiscountPercent, false, purchaseInvoiceDetail.VatPercent),
                                    HeaderDiscountValue = CalculateDetailValue.HeaderDiscountValue(purchaseInvoiceDetail.Quantity, purchaseInvoiceDetail.PurchasePrice, purchaseInvoiceDetail.ItemDiscountPercent, purchaseInvoiceHeader.DiscountPercent, false, purchaseInvoiceDetail.VatPercent),
                                    GrossValue = CalculateDetailValue.GrossValue(purchaseInvoiceDetail.Quantity, purchaseInvoiceDetail.PurchasePrice, purchaseInvoiceDetail.ItemDiscountPercent, purchaseInvoiceHeader.DiscountPercent, false, purchaseInvoiceDetail.VatPercent),
                                    VatPercent = purchaseInvoiceDetail.VatPercent,
                                    VatValue = CalculateDetailValue.VatValue(purchaseInvoiceDetail.Quantity, purchaseInvoiceDetail.PurchasePrice, purchaseInvoiceDetail.ItemDiscountPercent, purchaseInvoiceDetail.VatPercent, purchaseInvoiceHeader.DiscountPercent, false),
                                    SubNetValue = CalculateDetailValue.SubNetValue(purchaseInvoiceDetail.Quantity, purchaseInvoiceDetail.PurchasePrice, purchaseInvoiceDetail.ItemDiscountPercent, purchaseInvoiceDetail.VatPercent, purchaseInvoiceHeader.DiscountPercent, false),
                                    Notes = purchaseInvoiceDetail.Notes,
                                    ItemNote = purchaseInvoiceDetail.ItemNote,
                                    VatTaxId = purchaseInvoiceDetail.VatTaxId,
                                    VatTaxTypeId = purchaseInvoiceDetail.VatTaxTypeId,
                                    ConsumerPrice = item.ConsumerPrice,
                                    CostPrice = itemCost != null ? itemCost.CostPrice : 0,
                                    CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
                                    CostValue = itemCost != null ? itemCost.CostPrice * itemPacking.Packing * (purchaseInvoiceDetail.Quantity + purchaseInvoiceDetail.BonusQuantity) : 0,
                                    LastPurchasePrice = lastPurchasePrice != null ? lastPurchasePrice.PurchasePrice : 0,
                                }).Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();
            }

            var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);
            var purchaseInvoiceDetailTaxes = await _purchaseInvoiceDetailTaxService.GetPurchaseInvoiceDetailTaxes(purchaseInvoiceHeaderId).ToListAsync();
            int newId = -1;
            int newSubId = -1;
            foreach (var purchaseInvoiceReturnDetail in purchaseInvoiceReturnDetails)
            {
                purchaseInvoiceReturnDetail.Packages = packages.Where(x => x.ItemId == purchaseInvoiceReturnDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                purchaseInvoiceReturnDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == purchaseInvoiceReturnDetail.ItemId).ToList();
                purchaseInvoiceReturnDetail.Taxes = purchaseInvoiceReturnDetail.ItemTaxData.ToJson();

                purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailTaxes = (
                    from itemTax in purchaseInvoiceDetailTaxes.Where(x => x.PurchaseInvoiceDetailId ==  purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailId)
                    select new PurchaseInvoiceReturnDetailTaxDto
                    {
                        TaxId = itemTax.TaxId,
                        TaxTypeId = itemTax.TaxTypeId,
                        CreditAccountId = itemTax.DebitAccountId,
                        TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
                        TaxPercent = itemTax.TaxPercent,
                        TaxValue = CalculateDetailValue.TaxValue(purchaseInvoiceReturnDetail.Quantity, purchaseInvoiceReturnDetail.PurchasePrice, purchaseInvoiceReturnDetail.ItemDiscountPercent, purchaseInvoiceReturnDetail.VatPercent, itemTax.TaxPercent, itemTax.TaxAfterVatInclusive, purchaseInvoiceHeader.DiscountPercent, false)
                    }
                ).ToList();

                purchaseInvoiceReturnDetail.OtherTaxValue = purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailTaxes.Sum(x => x.TaxValue);
                purchaseInvoiceReturnDetail.NetValue = CalculateDetailValue.NetValue(purchaseInvoiceReturnDetail.Quantity, purchaseInvoiceReturnDetail.PurchasePrice, purchaseInvoiceReturnDetail.ItemDiscountPercent, purchaseInvoiceReturnDetail.VatPercent, purchaseInvoiceReturnDetail.OtherTaxValue, purchaseInvoiceHeader.DiscountPercent, false);

                foreach (var purchaseInvoiceReturnDetailTax in purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailTaxes)
                {
                    purchaseInvoiceReturnDetailTax.PurchaseInvoiceReturnDetailId = newId;
                    purchaseInvoiceReturnDetailTax.PurchaseInvoiceReturnDetailTaxId = newSubId--;
                }
                purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailId = newId;
                newId--;
            }

            var totalValueFromDetail = purchaseInvoiceReturnDetails.Sum(x => x.TotalValue);
            var totalValueAfterDiscountFromDetail = purchaseInvoiceReturnDetails.Sum(x => x.TotalValueAfterDiscount);
            var totalItemDiscount = purchaseInvoiceReturnDetails.Sum(x => x.ItemDiscountValue);
            var grossValueFromDetail = purchaseInvoiceReturnDetails.Sum(x => x.GrossValue);
            var vatValueFromDetail = purchaseInvoiceReturnDetails.Sum(x => x.VatValue);
            var subNetValueFromDetail = purchaseInvoiceReturnDetails.Sum(x => x.SubNetValue);
            var otherTaxValueFromDetail = purchaseInvoiceReturnDetails.Sum(x => x.OtherTaxValue);
            var netValueFromDetail = purchaseInvoiceReturnDetails.Sum(x => x.NetValue);
            var totalCostValueFromDetail = purchaseInvoiceReturnDetails.Sum(x => x.CostValue);

            var purchaseInvoiceReturnHeader = new PurchaseInvoiceReturnHeaderDto
            {
                PurchaseInvoiceReturnHeaderId = 0,
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
                IsDirectInvoice = isDirectInvoice,
                CreditPayment = purchaseInvoiceHeader.CreditPayment,
                TaxTypeId = purchaseInvoiceHeader.TaxTypeId,
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
                DebitAccountId = purchaseInvoiceHeader.CreditAccountId,
                CreditAccountId = purchaseInvoiceHeader.DebitAccountId,
                JournalHeaderId = 0,
                RemarksAr = purchaseInvoiceHeader.RemarksAr,
                RemarksEn = purchaseInvoiceHeader.RemarksEn,
                IsOnTheWay = isOnTheWay,
                IsClosed = false,
                IsEnded = false,
                IsBlocked = false,
                ArchiveHeaderId = purchaseInvoiceHeader.ArchiveHeaderId,
            };

            return new PurchaseInvoiceReturnDto { PurchaseInvoiceReturnHeader = purchaseInvoiceReturnHeader, PurchaseInvoiceReturnDetails = purchaseInvoiceReturnDetails };
		}

		private List<PurchaseInvoiceReturnDetailDto> DistributeQuantityAndCalculateValues(List<PurchaseInvoiceReturnDetailDto> purchaseInvoiceReturnDetails, decimal headerDiscountPercent)
		{
			QuantityDistributionLogic.DistributeQuantitiesOnDetails(
				details: purchaseInvoiceReturnDetails,
				keySelector: x => (x.ItemId, x.ItemPackageId, x.BarCode, x.CostCenterId, x.PurchasePrice, x.ItemDiscountPercent, x.ExpireDate, x.BatchNumber),
				availableQuantitySelector: x => x.KeyLevelQuantity,
				availableBonusQuantitySelector: x => x.KeyLevelBonusQuantity,
				quantitySelector: x => x.Quantity,
				bonusQuantitySelector: x => x.BonusQuantity,
				quantityAssigner: (x, value) => x.Quantity = value,
				bonusQuantityAssigner: (x, value) => x.BonusQuantity = value
			);

			RecalculateDetailValue.RecalculateDetailValues(
				details: purchaseInvoiceReturnDetails,
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

			return purchaseInvoiceReturnDetails;
		}

		private async Task<int> GetParentMenuCode(int purchaseInvoiceHeaderid)
        {
            var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaderById(purchaseInvoiceHeaderid);
            return PurchaseInvoiceMenuCodeHelper.GetMenuCode(purchaseInvoiceHeader!);
        }

        public async Task<ResponseDto> SavePurchaseInvoiceReturn(PurchaseInvoiceReturnDto purchaseInvoiceReturn, bool hasApprove, bool approved, int? requestId)
        {
            TrimDetailStrings(purchaseInvoiceReturn.PurchaseInvoiceReturnDetails);
            ResponseDto result;

            var menuCode = PurchaseInvoiceReturnMenuCodeHelper.GetMenuCode(purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!);
            var parentMenuCode = await GetParentMenuCode(purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId);
			result = await CheckPurchaseInvoiceReturnIsValid(purchaseInvoiceReturn, menuCode, parentMenuCode);
            if(result.Success == false)
            {
                result.Id = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId;
                return result;
            }
                        
            if (purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId == 0)
            {
                await UpdateModelPrices(purchaseInvoiceReturn);
            }

            if (purchaseInvoiceReturn.PurchaseInvoiceReturnHeader?.IsDirectInvoice == false)
            {
                result = await _purchaseInvoiceReturnService.SavePurchaseInvoiceReturn(purchaseInvoiceReturn, hasApprove, approved, requestId);
            }
            else
            {
                result = await SaveDirectPurchaseInvoiceReturn(purchaseInvoiceReturn, hasApprove, approved, requestId);
            }

            if (result.Success)
            {
                await _purchaseInvoiceHeaderService.UpdateEndedAndClosed(purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId, true, true);
                await HandleMarkingOfStockInAndStockInReturnsAsEnded(purchaseInvoiceReturn.PurchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId, purchaseInvoiceReturn.PurchaseInvoiceReturnHeader.IsOnTheWay, true);

                await UpdatePurchaseOrderStatusOnSave(purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!);
                await LinkStockInReturnsToCreatedInvoice(result.Id, purchaseInvoiceReturn.PurchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId, purchaseInvoiceReturn.PurchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId == 0, purchaseInvoiceReturn.PurchaseInvoiceReturnHeader.IsOnTheWay);

				var settlementExccedResult = await _getPurchaseInvoiceSettleValueService.CheckSettlementExceedingAndUpdateSettlementCompletedFlag(purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId, menuCode, parentMenuCode, true);
				if (settlementExccedResult.Success == false) return settlementExccedResult;
			}

            return result;
        }

        private async Task UpdatePurchaseOrderStatusOnSave(PurchaseInvoiceReturnHeaderDto purchaseInvoiceReturnHeader)
        {
            if (purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId == 0)
            {
                var purchaseOrderHeaderId = await GetRelatedPurchaseOrder(purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId);
                await _purchaseOrderStatusService.UpdatePurchaseOrderStatus(purchaseOrderHeaderId, DocumentStatusData.PurchaseInvoiceReturnCreated, MenuCodeData.PurchaseInvoiceReturn /*Direct vs Indirect invoice does not matter for purchase order status so no need to check here*/);
            }
        }

        private async Task<int> GetRelatedPurchaseOrder(int purchaseInvoiceHeaderId)
        {
            return await (from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId)
                          from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseInvoiceHeader.PurchaseOrderHeaderId)
                          select purchaseInvoiceHeader.PurchaseOrderHeaderId).FirstOrDefaultAsync();
        }


        private static void TrimDetailStrings(List<PurchaseInvoiceReturnDetailDto> purchaseInvoiceReturnDetails)
        {
            foreach (var purchaseInvoiceReturnDetail in purchaseInvoiceReturnDetails)
            {
                purchaseInvoiceReturnDetail.BatchNumber = string.IsNullOrWhiteSpace(purchaseInvoiceReturnDetail.BatchNumber) ? null : purchaseInvoiceReturnDetail.BatchNumber.Trim();
                purchaseInvoiceReturnDetail.ItemNote = string.IsNullOrWhiteSpace(purchaseInvoiceReturnDetail.ItemNote) ? null : purchaseInvoiceReturnDetail.ItemNote.Trim();
            }
        }

		private async Task LinkStockInReturnsToCreatedInvoice(int purchaseInvoiceReturnHeaderId, int purchaseInvoiceHeaderId, bool isCreate, bool isOnTheWay)
		{
			if (isCreate && !isOnTheWay)
			{
				var stockInReturnIdsRelatedToPurchaseInvoice = await _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.StockInReturnHeaderId).ToListAsync();

				await _invoiceStockInReturnService.LinkStockInReturnsToPurchaseInvoiceReturn(purchaseInvoiceReturnHeaderId, stockInReturnIdsRelatedToPurchaseInvoice);
			}
		}

        private async Task HandleMarkingOfStockInAndStockInReturnsAsEnded(int purchaseInvoiceHeaderId, bool isOnTheWay, bool isEnded)
        {
            if (isOnTheWay)
            {
                await _stockInHeaderService.UpdateAllStockInsEndedDirectlyFromPurchaseInvoice(purchaseInvoiceHeaderId, isEnded);
                await _stockInReturnHeaderService.UpdateAllStockInReturnsOnTheWayEndedFromPurchaseInvoice(purchaseInvoiceHeaderId, isEnded);
            }
            else
            {
                await _stockInReturnHeaderService.UpdateAllStockInReturnsEndedDirectlyFromPurchaseInvoice(purchaseInvoiceHeaderId, isEnded);
            }
        }

		public async Task<ResponseDto> DeletePurchaseInvoiceReturn(int purchaseInvoiceReturnHeaderId, int menuCode)
        {
            var purchaseInvoiceReturnHeader = await _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeaderId).Select(x => new { x.IsDirectInvoice, x.PurchaseInvoiceHeaderId, x.IsBlocked, x.IsOnTheWay}).FirstOrDefaultAsync();
            if (purchaseInvoiceReturnHeader == null)
            {
                return new ResponseDto { Id = purchaseInvoiceReturnHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };
            }

            var parentMenuCode = await GetParentMenuCode(purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId);
            var validationResult = await CheckPurchaseInvoiceReturnIsValidForDelete(purchaseInvoiceReturnHeaderId, purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId, purchaseInvoiceReturnHeader.IsBlocked, purchaseInvoiceReturnHeader.IsOnTheWay, menuCode, parentMenuCode);
            if (validationResult.Success == false) return validationResult;

            await UnlinkStockInReturnFromDeletedPurchaseInvoice(purchaseInvoiceReturnHeaderId, purchaseInvoiceReturnHeader.IsOnTheWay);
            var purchaseInvoiceReturnResult = await _purchaseInvoiceReturnService.DeletePurchaseInvoiceReturn(purchaseInvoiceReturnHeaderId, menuCode);
            if (purchaseInvoiceReturnResult.Success == false)
            {
                return purchaseInvoiceReturnResult;
            }

			if (purchaseInvoiceReturnHeader.IsDirectInvoice) 
            {
                var stockInReturnHeaderId = _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId).Select(x => x.StockInReturnHeaderId).FirstOrDefault();
                var stockInReturnResult = await _stockInReturnService.DeleteStockInReturn(stockInReturnHeaderId, MenuCodeData.ReturnFromPurchaseInvoice); //not using stockInReturnhandlingService because no validation is needed
                if (stockInReturnResult.Success == false)
                {
                    return stockInReturnResult;
                }
            }

            var purchaseInvoiceHasStockInReturn = await _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId).AnyAsync();
            var purchaseInvoiceHasStockIn = await _stockInHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId).AnyAsync();
            var purchaseInvoiceHasOtherInvoiceReturn = await _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId).AnyAsync();
            //no need to check for supplier debit or credit, because PR cannot be deleted if they exist

            await _purchaseInvoiceHeaderService.UpdateEndedAndClosed(purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId, purchaseInvoiceHasOtherInvoiceReturn, purchaseInvoiceHasStockInReturn || purchaseInvoiceHasStockIn || purchaseInvoiceHasOtherInvoiceReturn);
            await HandleMarkingOfStockInAndStockInReturnsAsEnded(purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId, purchaseInvoiceReturnHeader.IsOnTheWay, false);
            await UpdatePurchaseOrderStatusOnDelete(purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId);

			var settlementExccedResult = await _getPurchaseInvoiceSettleValueService.CheckSettlementExceedingAndUpdateSettlementCompletedFlag(purchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId, menuCode, parentMenuCode, false);
			if (settlementExccedResult.Success == false) return settlementExccedResult;

			return purchaseInvoiceReturnResult;
        }

        private async Task UnlinkStockInReturnFromDeletedPurchaseInvoice(int purchaseInvoiceReturnHeaderId, bool isOnTheWay)
        {
            if (!isOnTheWay)
            {
				await _invoiceStockInReturnService.UnlinkStockInReturnsFromPurchaseInvoiceReturn(purchaseInvoiceReturnHeaderId);
			}
        }

        private async Task<ResponseDto> CheckPurchaseInvoiceReturnIsValidForDelete(int purchaseInvoiceReturnHeaderId, int purchaseInvoiceHeaderId, bool isBlocked, bool isOnTheWay, int menuCode, int parentMenuCode)
        {
			if (isBlocked)
			{
				return new ResponseDto { Id = purchaseInvoiceReturnHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
			}

			var hasSupplierDebitOrCreditMemo = await CheckPurchaseInvoiceHasSupplierDebitOrCreditMemo(purchaseInvoiceHeaderId, menuCode, parentMenuCode);
			if (hasSupplierDebitOrCreditMemo.Success == false)
			{
				return hasSupplierDebitOrCreditMemo;
			}

			// Assume that PurchaseInvoiceReturn cannot be created without stockInreturn so no need to check it seperately
			var stockInReturnFromInvoiceExists = await CheckPurchaseInvoiceHasStockInReturn(purchaseInvoiceHeaderId, isOnTheWay, menuCode, parentMenuCode);
			return stockInReturnFromInvoiceExists;
		}

        private async Task UpdatePurchaseOrderStatusOnDelete(int purchaseInvoiceHeaderId)
        {
            var purchaseOrderHeaderId = await GetRelatedPurchaseOrder(purchaseInvoiceHeaderId);
            await _purchaseOrderStatusService.UpdatePurchaseOrderStatus(purchaseOrderHeaderId, -1, MenuCodeData.PurchaseInvoiceReturn /*Direct vs Indirect invoice does not matter for purchase order status so no need to check here*/);
        }

        private async Task<ResponseDto> CheckPurchaseInvoiceHasStockInReturn(int purchaseInvoiceHeaderId, bool isOnTheWay, int menuCode, int parentMenuCode)
        {
            if (isOnTheWay)
            {
                var isEnded = await _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).AnyAsync();
                if (isEnded)
                {
                    return new ResponseDto { Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.CannotDeleteBecauseReturnedAfterInvoice) };
                }
            }
            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> SaveDirectPurchaseInvoiceReturn(PurchaseInvoiceReturnDto purchaseInvoiceReturn, bool hasApprove, bool approved, int? requestId)
        {
            string? documentReference = null;
            if (purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId == 0)
            {
                if (hasApprove)
                {
                    documentReference = $"{DocumentReferenceData.Approval}{requestId}";
                }
                else
                {
                    int nextPurchaseInvoiceReturnHeaderId = await _purchaseInvoiceReturnHeaderService.GetNextId();
                    documentReference = $"{PurchaseInvoiceReturnMenuCodeHelper.GetDocumentReference(purchaseInvoiceReturn.PurchaseInvoiceReturnHeader)}{nextPurchaseInvoiceReturnHeaderId}";
                }
            }
            
            int? stockInReturnHeaderId = null;

            if (purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId != 0)
            {
                stockInReturnHeaderId = await _stockInReturnHeaderService.GetStockInReturnHeaders().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId).Select(x => (int?)x.StockInReturnHeaderId).FirstOrDefaultAsync();
                if (stockInReturnHeaderId == null) return new ResponseDto { Message = "No Stock In return was found"};
            }

            StockInReturnDto stockInReturn = CreateStockInReturnModelFromPurchaseInvoiceReturn(purchaseInvoiceReturn, stockInReturnHeaderId);

            var stockInReturnResult = await _stockInReturnService.SaveStockInReturn(stockInReturn, hasApprove, approved, requestId, documentReference, true); //not using stockInReturnhandlingService because no validation is needed
            if (stockInReturnResult.Success == false)
            {
                return stockInReturnResult;
            }

            return await _purchaseInvoiceReturnService.SavePurchaseInvoiceReturn(purchaseInvoiceReturn, hasApprove, approved, requestId, documentReference);
        }

        private static StockInReturnDto CreateStockInReturnModelFromPurchaseInvoiceReturn(PurchaseInvoiceReturnDto purchaseInvoiceReturn, int? stockInReturnHeaderId)
        {
            return new StockInReturnDto
            {
                StockInReturnHeader = new StockInReturnHeaderDto
                {
                    StockInReturnHeaderId = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId == 0 ? 0 : (int)stockInReturnHeaderId!,
                    StockTypeId = StockTypeData.ReturnFromPurchaseInvoice,
                    PurchaseInvoiceHeaderId = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId,
                    StockInHeaderId = null,
                    SupplierId = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.SupplierId,
                    StoreId = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.StoreId,
                    DocumentDate = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.DocumentDate,
                    Reference = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.Reference,
                    TotalValue = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.TotalValue,
                    DiscountPercent = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.DiscountPercent,
                    DiscountValue = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.DiscountValue,
                    TotalItemDiscount = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.TotalItemDiscount,
                    GrossValue = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.GrossValue,
                    VatValue = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.VatValue,
                    SubNetValue = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.SubNetValue,
                    OtherTaxValue = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.OtherTaxValue,
					NetValueBeforeAdditionalDiscount = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.NetValueBeforeAdditionalDiscount,
					AdditionalDiscountValue = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.AdditionalDiscountValue,
                    NetValue = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.NetValue,
                    TotalCostValue = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.TotalCostValue,
                    RemarksAr = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.RemarksAr,
                    RemarksEn = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.RemarksEn,
                    IsClosed = false,
                    IsEnded = true,
                    ArchiveHeaderId = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.ArchiveHeaderId
                },
                StockInReturnDetails = (
                        from purchaseInvoiceReturnDetail in purchaseInvoiceReturn.PurchaseInvoiceReturnDetails
                        select new StockInReturnDetailDto
                        {
                            StockInReturnDetailId = 0,
                            StockInReturnHeaderId = 0,
                            CostCenterId = purchaseInvoiceReturnDetail.CostCenterId,
                            ItemId = purchaseInvoiceReturnDetail.ItemId,
                            ItemPackageId = purchaseInvoiceReturnDetail.ItemPackageId,
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
                            ConsumerPrice = purchaseInvoiceReturnDetail.ConsumerPrice,
                            CostPrice = purchaseInvoiceReturnDetail.CostPrice,
                            CostPackage = purchaseInvoiceReturnDetail.CostPackage,
                            CostValue = purchaseInvoiceReturnDetail.CostValue,
                            LastPurchasePrice = purchaseInvoiceReturnDetail.LastPurchasePrice,
                            StockInReturnDetailTaxes = (
                                from purchaseInvoiceReturnDetailTax in purchaseInvoiceReturnDetail.PurchaseInvoiceReturnDetailTaxes
                                select new StockInReturnDetailTaxDto
                                {
                                    StockInReturnDetailTaxId = 0,
                                    StockInReturnDetailId = 0,
                                    TaxId = purchaseInvoiceReturnDetailTax.TaxId,
                                    CreditAccountId = purchaseInvoiceReturnDetailTax.CreditAccountId,
                                    TaxPercent = purchaseInvoiceReturnDetailTax.TaxPercent,
                                    TaxValue = purchaseInvoiceReturnDetailTax.TaxValue,
                                }).ToList()
                        }).ToList()
            };
        }

        private async Task UpdateModelPrices(PurchaseInvoiceReturnDto purchaseInvoiceReturn)
        {
            await UpdateDetailPrices(purchaseInvoiceReturn.PurchaseInvoiceReturnDetails, purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.StoreId);
            purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.TotalCostValue = purchaseInvoiceReturn.PurchaseInvoiceReturnDetails.Sum(x => x.CostValue);
        }

        private async Task UpdateDetailPrices(List<PurchaseInvoiceReturnDetailDto> purchaseInvoiceReturnDetails, int storeId)
        {
            var itemIds = purchaseInvoiceReturnDetails.Select(x => x.ItemId).ToList();

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

            foreach (var purchaseInvoiceReturnDetail in purchaseInvoiceReturnDetails)
            {
                var packing = packings.Where(x => x.ItemId == purchaseInvoiceReturnDetail.ItemId && x.FromPackageId == purchaseInvoiceReturnDetail.ItemPackageId).Select(x => x.Packing).FirstOrDefault(1);

                purchaseInvoiceReturnDetail.ConsumerPrice = consumerPrices.Where(x => x.ItemId == purchaseInvoiceReturnDetail.ItemId).Select(x => x.ConsumerPrice).FirstOrDefault(0);
                purchaseInvoiceReturnDetail.CostPrice = itemCosts.Where(x => x.ItemId == purchaseInvoiceReturnDetail.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefault(0);
                purchaseInvoiceReturnDetail.CostPackage = purchaseInvoiceReturnDetail.CostPrice * packing;
                purchaseInvoiceReturnDetail.CostValue = purchaseInvoiceReturnDetail.CostValue * (purchaseInvoiceReturnDetail.Quantity + purchaseInvoiceReturnDetail.BonusQuantity);
                purchaseInvoiceReturnDetail.LastPurchasePrice = lastPurchasePrices.Where(x => x.ItemId == purchaseInvoiceReturnDetail.ItemId && x.ItemPackageId == purchaseInvoiceReturnDetail.ItemPackageId).Select(x => x.PurchasePrice).FirstOrDefault(0);
            }
        }

        private async Task<ResponseDto> CheckPurchaseInvoiceReturnIsValid(PurchaseInvoiceReturnDto purchaseInvoiceReturn, int menuCode, int parentMenuCode)
        {
            ResponseDto result;
            var purchaseInvoiceReturnHeader = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!;

            result = await CheckPurchaseInvoiceReturnFlagsAndConsistencyWithParent(purchaseInvoiceReturnHeader);
			if (result.Success == false) return result;

			result = await CheckPurchaseInvoiceReturnIsClosed(purchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId, menuCode);
            if (result.Success == false) return result;

			result = await CheckPurchaseExpensesJournalMismatch(purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!, purchaseInvoiceReturn.Journal?.JournalHeader, menuCode);
            if (result.Success == false) return result;

            result = await CheckPurchaseInvoiceBlocked(purchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId, menuCode);
            if (result.Success == false) return result;

            if (purchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId == 0)
            {
                result = purchaseInvoiceReturnHeader!.IsOnTheWay ? 
                    await CheckPurchaseInvoiceAlreadyHasPurchaseInvoiceReturnOnTheWay(purchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId, menuCode, parentMenuCode) :
                    await CheckPurchaseInvoiceAlreadyHasPurchaseInvoiceReturn(purchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId, menuCode, parentMenuCode);

                if (result.Success == false) return result;

                if (purchaseInvoiceReturnHeader!.IsDirectInvoice)
                {
                    result = await CheckPurchaseInvoiceAlreadyHasStockInReturn(purchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId, menuCode, parentMenuCode);
                    if (result.Success == false) return result;
                }
            }

            result = await CheckPurchaseInvoiceHasSupplierDebitOrCreditMemo(purchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId, menuCode, parentMenuCode);
            if (result.Success == false) return result;

			result = await _itemNoteValidationService.CheckItemNoteWithItemType(purchaseInvoiceReturn.PurchaseInvoiceReturnDetails, x => x.ItemId, x => x.ItemNote);
			if (result.Success == false) return result;

			result = await CheckPurchaseInvoiceReturnQuantity(purchaseInvoiceReturn, menuCode, parentMenuCode);
            if (result.Success == false) return result;

            result = await CheckTaxTypeMisMatchWithPurchaseInvoice(purchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId, purchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId, purchaseInvoiceReturnHeader!.TaxTypeId);
            if (result.Success == false) return result;

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckPurchaseInvoiceReturnIsClosed(int? purchaseInvoiceReturnHeaderId, int menuCode)
        {
            if (purchaseInvoiceReturnHeaderId != 0)
            {
                var isClosed = await _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeaderId).Select(x => x.IsClosed).FirstOrDefaultAsync();
				if (isClosed)
				{
                    return new ResponseDto { Id = purchaseInvoiceReturnHeaderId ?? 0, Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotUpdateBecauseClosed) };
                }
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckPurchaseInvoiceBlocked(int purchaseInvoiceHeaderId, int menuCode)
        {
            var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => new { x.IsBlocked }).FirstOrDefaultAsync();

            if (purchaseInvoiceHeader?.IsBlocked == true)
            {
                return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckPurchaseInvoiceReturnFlagsAndConsistencyWithParent(PurchaseInvoiceReturnHeaderDto purchaseInvoiceReturnHeader)
        {
			if (purchaseInvoiceReturnHeader.IsDirectInvoice && purchaseInvoiceReturnHeader.IsOnTheWay)
            {
                return new ResponseDto { Success = false, Message = "IsDirectInvoice and IsOnTheWay cannot both be set to true at the same time" };
            }

			var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceHeaderId).Select(x => new { x.IsOnTheWay, x.IsDirectInvoice }).FirstOrDefaultAsync();

			if ((purchaseInvoiceHeader!.IsOnTheWay || !purchaseInvoiceHeader!.IsDirectInvoice) && purchaseInvoiceReturnHeader.IsDirectInvoice)
			{
				return new ResponseDto { Success = false, Message = "Direct invoice return can only be created on direct invoice" };
			}

			if (!purchaseInvoiceHeader!.IsOnTheWay && purchaseInvoiceReturnHeader.IsOnTheWay)
			{
				return new ResponseDto { Success = false, Message = "You can create an invoice return on the way only from purchase invoice on the way" };
			}

            if (purchaseInvoiceHeader!.IsDirectInvoice && !purchaseInvoiceHeader.IsOnTheWay && !purchaseInvoiceReturnHeader.IsDirectInvoice)
            {
                return new ResponseDto { Success = false, Message = "PurchaseInvoiceReturn from direct invoice must also be direct" };
            }

			return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckPurchaseInvoiceAlreadyHasPurchaseInvoiceReturn(int purchaseInvoiceHeaderId, int menuCode, int parentMenuCode)
        {
            var alreadyHasPurchaseInvoiceReturn = await _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId && !x.IsOnTheWay).AnyAsync();

            if (alreadyHasPurchaseInvoiceReturn)
            {
				return new ResponseDto { Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.AlreadyHasDocument) };
			}

			return new ResponseDto { Success = true };
        }

		private async Task<ResponseDto> CheckPurchaseInvoiceAlreadyHasPurchaseInvoiceReturnOnTheWay(int purchaseInvoiceHeaderId, int menuCode, int parentMenuCode)
		{
			var alreadyHasPurchaseInvoiceReturn = await _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId && x.IsOnTheWay).AnyAsync();

			if (alreadyHasPurchaseInvoiceReturn)
			{
				return new ResponseDto { Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.AlreadyHasDocument) };
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckPurchaseInvoiceHasSupplierDebitOrCreditMemo(int purchaseInvoiceHeaderId, int menuCode, int parentMenuCode)
        {
            var alreadyHasSupplierDebitMemo = await _supplierDebitMemoService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).AnyAsync();

            if (alreadyHasSupplierDebitMemo)
            {
                return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, MenuCodeData.SupplierDebitMemo, parentMenuCode, GenericMessageData.HasDocument) };
            }

            var alreadyHasSupplierCreditMemo = await _supplierCreditMemoService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).AnyAsync();

            if (alreadyHasSupplierCreditMemo)
            {
				return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, MenuCodeData.SupplierCreditMemo, parentMenuCode, GenericMessageData.HasDocument) };
			}

			return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckPurchaseInvoiceAlreadyHasStockInReturn(int purchaseInvoiceHeaderId, int menuCode, int parentMenuCode)
        {
            bool purchaseInvoiceAlreadyHasStockInReturn = await _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).AnyAsync();
            if (purchaseInvoiceAlreadyHasStockInReturn)
            {
                return new ResponseDto { Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, MenuCodeData.PurchaseInvoiceReturn, PurchaseMessageData.PurchaseInvoiceAlreadyReturned) };
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckPurchaseInvoiceReturnQuantity(PurchaseInvoiceReturnDto purchaseInvoiceReturn, int menuCode, int parentMenuCode)
        {
            if (purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.IsDirectInvoice)
            {
                return await CheckDirectPurchaseInvoiceReturnQuantity(purchaseInvoiceReturn, menuCode, parentMenuCode);
            }
            else
            {
                return await CheckIndirectPurchaseInvoiceReturnQuantity(purchaseInvoiceReturn, menuCode, parentMenuCode);
            }
        }

        private async Task<ResponseDto> CheckTaxTypeMisMatchWithPurchaseInvoice(int purchaseInvoiceReturnHeaderId, int purchaseInvoiceHeaderId, int taxTypeId)
        {
            var purchaseInvoiceTaxType = await _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.TaxTypeId).FirstOrDefaultAsync();

            if(purchaseInvoiceTaxType != taxTypeId)
            {
                return new ResponseDto { Id = purchaseInvoiceReturnHeaderId, Success = false, Message = "TaxType of Purchase Invoice Return must match that of the Purchase Invoice"};
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckIndirectPurchaseInvoiceReturnQuantity(PurchaseInvoiceReturnDto purchaseInvoiceReturn, int menuCode, int parentMenuCode)
        {
            var purchaseInvoiceReturnDetailsGrouped = _purchaseInvoiceReturnDetailService.GroupPurchaseInvoiceReturnDetails(purchaseInvoiceReturn.PurchaseInvoiceReturnDetails);

            if (purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.IsOnTheWay)
            {
                var finalStocksReceived = await _stockInQuantityService.GetFinalStocksReceivedFromPurchaseInvoice(purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId).ToListAsync();
                var purchaseInvoiceDetailsGrouped = await _purchaseInvoiceDetailService.GetPurchaseInvoiceDetailsGrouped(purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId);

                var unmatchedQuantity = (from purchaseInvoiceReturnDetailGroup in purchaseInvoiceReturnDetailsGrouped
										 from purchaseInvoiceDetailGroup in purchaseInvoiceDetailsGrouped.Where(x => x.ItemId == purchaseInvoiceReturnDetailGroup.ItemId && x.ItemPackageId == purchaseInvoiceReturnDetailGroup.ItemPackageId && x.BatchNumber == purchaseInvoiceReturnDetailGroup.BatchNumber && x.ExpireDate == purchaseInvoiceReturnDetailGroup.ExpireDate && x.CostCenterId == purchaseInvoiceReturnDetailGroup.CostCenterId && x.BarCode == purchaseInvoiceReturnDetailGroup.BarCode && x.PurchasePrice == purchaseInvoiceReturnDetailGroup.PurchasePrice && x.ItemDiscountPercent == purchaseInvoiceReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
                                         from finalStockReceived in finalStocksReceived.Where(x => x.ItemId == purchaseInvoiceReturnDetailGroup.ItemId && x.ItemPackageId == purchaseInvoiceReturnDetailGroup.ItemPackageId && x.BatchNumber == purchaseInvoiceReturnDetailGroup.BatchNumber && x.ExpireDate == purchaseInvoiceReturnDetailGroup.ExpireDate && x.CostCenterId == purchaseInvoiceReturnDetailGroup.CostCenterId && x.BarCode == purchaseInvoiceReturnDetailGroup.BarCode && x.PurchasePrice == purchaseInvoiceReturnDetailGroup.PurchasePrice && x.ItemDiscountPercent == purchaseInvoiceReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
                                         select new
                                         {
                                             purchaseInvoiceReturnDetailGroup.ItemId,
                                             purchaseInvoiceReturnDetailGroup.ItemPackageId,
                                             PurchaseInvoiceReturnQuantity = purchaseInvoiceReturnDetailGroup.Quantity,
                                             PurchaseInvoiceReturnBonusQuantity = purchaseInvoiceReturnDetailGroup.BonusQuantity,
                                             RemainingQuantity = (purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.Quantity : 0) - (finalStockReceived != null ? finalStockReceived.QuantityReceived : 0),
                                             RemainingBonusQuantity = (purchaseInvoiceDetailGroup != null ? purchaseInvoiceDetailGroup.BonusQuantity : 0) - (finalStockReceived != null ? finalStockReceived.BonusQuantityReceived : 0),
                                         }).FirstOrDefault(x => x.RemainingQuantity != x.PurchaseInvoiceReturnQuantity || x.RemainingBonusQuantity != x.PurchaseInvoiceReturnBonusQuantity);

                if (unmatchedQuantity != null)
                {
                    var language = _httpContextAccessor.GetProgramCurrentLanguage();

                    var itemName = await _itemService.GetAll().Where(x => x.ItemId == unmatchedQuantity.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
                    var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == unmatchedQuantity.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

                    if (unmatchedQuantity.RemainingQuantity != unmatchedQuantity.PurchaseInvoiceReturnQuantity)
                    {
                        return new ResponseDto { Id = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.QuantityNotMatchingRemaining, itemName!, itemPackageName!, unmatchedQuantity.PurchaseInvoiceReturnQuantity.ToNormalizedString(), unmatchedQuantity.RemainingQuantity.ToNormalizedString()) };
                    }
                    else
                    {
                        return new ResponseDto { Id = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.BonusQuantityNotMatchingRemaining, itemName!, itemPackageName!, unmatchedQuantity.PurchaseInvoiceReturnBonusQuantity.ToNormalizedString(), unmatchedQuantity.RemainingBonusQuantity.ToNormalizedString()) };
                    }
                }
            }
            else
            {
                var stocksReturned = await _stockInQuantityService.GetStocksReturnedFromPurchaseInvoice(purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId).ToListAsync();

                var unmatchedQuantity = (from purchaseInvoiceReturnDetailGroup in purchaseInvoiceReturnDetailsGrouped
										 from stockReturned in stocksReturned.Where(x => x.ItemId == purchaseInvoiceReturnDetailGroup.ItemId && x.ItemPackageId == purchaseInvoiceReturnDetailGroup.ItemPackageId && x.BatchNumber == purchaseInvoiceReturnDetailGroup.BatchNumber && x.ExpireDate == purchaseInvoiceReturnDetailGroup.ExpireDate && x.CostCenterId == purchaseInvoiceReturnDetailGroup.CostCenterId && x.BarCode == purchaseInvoiceReturnDetailGroup.BarCode && x.PurchasePrice == purchaseInvoiceReturnDetailGroup.PurchasePrice && x.ItemDiscountPercent == purchaseInvoiceReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
                                         select new
                                         {
                                             purchaseInvoiceReturnDetailGroup.ItemId,
                                             purchaseInvoiceReturnDetailGroup.ItemPackageId,
                                             PurchaseInvoiceReturnQuantity = purchaseInvoiceReturnDetailGroup.Quantity,
                                             PurchaseInvoiceReturnBonusQuantity = purchaseInvoiceReturnDetailGroup.BonusQuantity,
                                             QuantityReturned = stockReturned != null ? stockReturned.QuantityReturned : 0,
                                             BonusQuantityReturned = stockReturned != null ? stockReturned.BonusQuantityReturned : 0
                                         }).FirstOrDefault(x => x.QuantityReturned != x.PurchaseInvoiceReturnQuantity || x.BonusQuantityReturned != x.PurchaseInvoiceReturnBonusQuantity);

                if (unmatchedQuantity != null)
                {
                    var language = _httpContextAccessor.GetProgramCurrentLanguage();

                    var itemName = await _itemService.GetAll().Where(x => x.ItemId == unmatchedQuantity.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
                    var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == unmatchedQuantity.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

                    if (unmatchedQuantity.QuantityReturned != unmatchedQuantity.PurchaseInvoiceReturnQuantity)
                    {
                        return new ResponseDto { Id = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.QuantityNotMatchingReturned, itemName!, itemPackageName!, unmatchedQuantity.PurchaseInvoiceReturnQuantity.ToNormalizedString(), unmatchedQuantity.QuantityReturned.ToNormalizedString()) };
                    }
                    else
                    {
                        return new ResponseDto { Id = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.BonusQuantityNotMatchingRemaining, itemName!, itemPackageName!, unmatchedQuantity.PurchaseInvoiceReturnBonusQuantity.ToNormalizedString(), unmatchedQuantity.BonusQuantityReturned.ToNormalizedString()) };
                    }
                }
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckDirectPurchaseInvoiceReturnQuantity(PurchaseInvoiceReturnDto purchaseInvoiceReturn, int menuCode, int parentMenuCode)
        {
            //No need to subtract PurchaseInvoiceReturn on the way from PurchaseInvoice because user can only create Direct Invoice return from a direct invoice
            var purchaseInvoiceDetailsGrouped = await _purchaseInvoiceDetailService.GetPurchaseInvoiceDetailsGrouped(purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceHeaderId);
            var purchaseInvoiceReturnDetailsGrouped = _purchaseInvoiceReturnDetailService.GroupPurchaseInvoiceReturnDetails(purchaseInvoiceReturn.PurchaseInvoiceReturnDetails);

			var unmatchedQuantity = (from purchaseInvoiceReturnDetailGroup in purchaseInvoiceReturnDetailsGrouped
									 from PurchaseInvoiceDetailGroup in purchaseInvoiceDetailsGrouped.Where(x => x.ItemId == purchaseInvoiceReturnDetailGroup.ItemId && x.ItemPackageId == purchaseInvoiceReturnDetailGroup.ItemPackageId && x.BatchNumber == purchaseInvoiceReturnDetailGroup.BatchNumber && x.ExpireDate == purchaseInvoiceReturnDetailGroup.ExpireDate && x.CostCenterId == purchaseInvoiceReturnDetailGroup.CostCenterId && x.BarCode == purchaseInvoiceReturnDetailGroup.BarCode && x.PurchasePrice == purchaseInvoiceReturnDetailGroup.PurchasePrice && x.ItemDiscountPercent == purchaseInvoiceReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
									 select new
									 {
										 purchaseInvoiceReturnDetailGroup.ItemId,
										 purchaseInvoiceReturnDetailGroup.ItemPackageId,
										 PurchaseInvoiceReturnQuantity = purchaseInvoiceReturnDetailGroup.Quantity,
										 PurchaseInvoiceReturnBonusQuantity = purchaseInvoiceReturnDetailGroup.BonusQuantity,
										 PurchaseInvoiceQuantity = PurchaseInvoiceDetailGroup != null ? PurchaseInvoiceDetailGroup.Quantity : 0,
										 PurchaseInvoiceBonusQuantity = PurchaseInvoiceDetailGroup != null ? PurchaseInvoiceDetailGroup.BonusQuantity : 0
									 }).FirstOrDefault(x => x.PurchaseInvoiceReturnQuantity > x.PurchaseInvoiceQuantity || x.PurchaseInvoiceReturnBonusQuantity > x.PurchaseInvoiceBonusQuantity);

			if (unmatchedQuantity != null)
            {
                var language = _httpContextAccessor.GetProgramCurrentLanguage();

                var itemName = await _itemService.GetAll().Where(x => x.ItemId == unmatchedQuantity.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
                var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == unmatchedQuantity.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

                if (unmatchedQuantity.PurchaseInvoiceReturnQuantity > unmatchedQuantity.PurchaseInvoiceQuantity)
                {
                    return new ResponseDto { Id = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.QuantityExceeding, itemName!, itemPackageName!, unmatchedQuantity.PurchaseInvoiceReturnQuantity.ToNormalizedString(), unmatchedQuantity.PurchaseInvoiceQuantity.ToNormalizedString()) };
                }
                else
                {
                    return new ResponseDto { Id = purchaseInvoiceReturn.PurchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, parentMenuCode, PurchaseMessageData.BonusQuantityExceeding, itemName!, itemPackageName!, unmatchedQuantity.PurchaseInvoiceReturnBonusQuantity.ToNormalizedString(), unmatchedQuantity.PurchaseInvoiceBonusQuantity.ToNormalizedString()) };
                }
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckPurchaseExpensesJournalMismatch(PurchaseInvoiceReturnHeaderDto purchaseInvoiceReturnHeader, JournalHeaderDto? journalHeader, int menuCode)
        {
            int rounding = await _storeService.GetStoreHeaderRounding(purchaseInvoiceReturnHeader.StoreId);

            decimal journalDebitValue = NumberHelper.RoundNumber(journalHeader?.TotalDebitValue ?? 0, rounding);
            decimal invoiceValue = NumberHelper.RoundNumber((purchaseInvoiceReturnHeader.NetValue + purchaseInvoiceReturnHeader.AdditionalDiscountValue), rounding);

            if (journalDebitValue != invoiceValue)
            {
                return new ResponseDto { Success = false, Id = purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId, Message = await _purchaseMessageService.GetMessage(menuCode, PurchaseMessageData.ValueNotMatchingJournalDebit) };
            }
            else
            {
                return new ResponseDto { Success = true };
            }
        }
    }
}
