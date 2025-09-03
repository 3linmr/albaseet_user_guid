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
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Contracts.CostCenters;
using Purchases.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Settings;

namespace Purchases.Service.Services
{
    public class PurchaseInvoiceHandlingService : IPurchaseInvoiceHandlingService
    {
        private readonly IInvoiceStockInService _invoiceStockInService;
        private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
        private readonly IPurchaseValueService _purchaseValueService;
        private readonly ICostCenterService _costCenterService;
        private readonly ISupplierQuotationDetailTaxService _supplierQuotationDetailTaxService;
        private readonly ISupplierQuotationHeaderService _supplierQuotationHeaderService;
        private readonly ISupplierQuotationDetailService _supplierQuotationDetailService;
        private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
        private readonly IPurchaseInvoiceService _purchaseInvoiceService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
        private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;
        private readonly IPurchaseOrderDetailTaxService _purchaseOrderDetailTaxService;
        private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
        private readonly IStockInHeaderService _stockInHeaderService;
        private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
        private readonly IStockInQuantityService _stockInQuantityService;
        private readonly IStockInService _stockInService;
        private readonly ISupplierDebitMemoService _supplierDebitMemoService;
        private readonly ISupplierCreditMemoService _supplierCreditMemoService;
        private readonly IItemCostHandlingService _itemCostHandlingService;
        private readonly IItemCostService _itemCostService;
        private readonly IItemService _itemService;
        private readonly IItemPackingService _itemPackingService;
        private readonly ITaxPercentService _taxPercentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemPackageService _itemPackageService;
        private readonly IStoreService _storeService;
        private readonly IPurchaseOrderStatusService _purchaseOrderStatusService;
        private readonly IItemTaxService _itemTaxService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly ITaxService _taxService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly IPurchaseMessageService _purchaseMessageService;
        private readonly IItemNoteValidationService _itemNoteValidationService;
        private readonly IDocumentExceedValueSettingService _documentExceedValueSettingService;

       public PurchaseInvoiceHandlingService(IInvoiceStockInService invoiceStockInService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseValueService purchaseValueService, ICostCenterService costCenterService, ISupplierQuotationDetailTaxService supplierQuotationDetailTaxService, ISupplierQuotationHeaderService supplierQuotationHeaderService, ISupplierQuotationDetailService supplierQuotationDetailService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IPurchaseInvoiceService purchaseInvoiceService, IPurchaseOrderService purchaseOrderService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IPurchaseOrderDetailService purchaseOrderDetailService, IPurchaseOrderDetailTaxService purchaseOrderDetailTaxService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IStockInHeaderService stockInHeaderService, IStockInReturnHeaderService stockInReturnHeaderService, IStockInQuantityService stockInQuantityService, IStockInService stockInService, ISupplierDebitMemoService supplierDebitMemoService, ISupplierCreditMemoService supplierCreditMemoService, IItemCostHandlingService itemCostHandlingService, IItemCostService itemCostService, IItemService itemService, IItemPackingService itemPackingService, ITaxPercentService taxPercentService, IHttpContextAccessor httpContextAccessor, IItemPackageService itemPackageService, IStoreService storeService, IPurchaseOrderStatusService purchaseOrderStatusService, IItemBarCodeService itemBarCodeService, IItemTaxService itemTaxService, ITaxService taxService, IGenericMessageService genericMessageService, IPurchaseMessageService purchaseMessageService, IItemNoteValidationService itemNoteValidationService, IDocumentExceedValueSettingService documentExceedValueSettingService)
        {
            _invoiceStockInService = invoiceStockInService;
            _purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
            _purchaseValueService = purchaseValueService;
            _costCenterService = costCenterService;
            _supplierQuotationDetailTaxService = supplierQuotationDetailTaxService;
            _supplierQuotationHeaderService = supplierQuotationHeaderService;
            _supplierQuotationDetailService = supplierQuotationDetailService;
            _purchaseInvoiceDetailService = purchaseInvoiceDetailService;
            _purchaseInvoiceService = purchaseInvoiceService;
            _purchaseOrderService = purchaseOrderService;
            _purchaseOrderHeaderService = purchaseOrderHeaderService;
            _purchaseOrderDetailService = purchaseOrderDetailService;
            _purchaseOrderDetailTaxService = purchaseOrderDetailTaxService;
            _purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
            _stockInHeaderService = stockInHeaderService;
            _stockInReturnHeaderService = stockInReturnHeaderService;
            _stockInQuantityService = stockInQuantityService;
            _stockInService = stockInService;
            _supplierDebitMemoService = supplierDebitMemoService;
            _supplierCreditMemoService = supplierCreditMemoService;
            _itemCostHandlingService = itemCostHandlingService;
            _itemCostService = itemCostService;
            _itemService = itemService;
            _itemPackingService = itemPackingService;
            _taxPercentService = taxPercentService;
            _itemPackageService = itemPackageService;
            _httpContextAccessor = httpContextAccessor;
            _storeService = storeService;
            _purchaseOrderStatusService = purchaseOrderStatusService;
            _itemTaxService = itemTaxService;
            _itemBarCodeService = itemBarCodeService;
            _taxService = taxService;
            _genericMessageService = genericMessageService;
            _purchaseMessageService = purchaseMessageService;
            _itemNoteValidationService = itemNoteValidationService;
            _documentExceedValueSettingService = documentExceedValueSettingService;
        }

        public async Task<PurchaseInvoiceDto> GetPurchaseInvoiceFromPurchaseOrder(int purchaseOrderHeaderId)
        {
            var purchaseOrderHeader = await _purchaseOrderHeaderService.GetPurchaseOrderHeaderById(purchaseOrderHeaderId);
            if (purchaseOrderHeader == null)
            {
                return new PurchaseInvoiceDto();
            }

            var purchaseOrderDetails = await _purchaseOrderDetailService.GetPurchaseOrderDetailsFullyGrouped(purchaseOrderHeaderId);

            var itemIds = purchaseOrderDetails.Select(x => x.ItemId).ToList();
            
            var finalStocksReceived = await _stockInQuantityService.GetFinalStocksReceivedFromPurchaseOrderWithAllKeys(purchaseOrderHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

            var items = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var itemPackings = await _itemPackingService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
            var lastPurchasePrices = await _purchaseInvoiceService.GetMultipleLastPurchasePrices(itemIds);

            var costCenterIds = finalStocksReceived.Select(x => x.CostCenterId).ToList();
            var costCenters = await _costCenterService.GetAll().Where(x => costCenterIds.Contains(x.CostCenterId)).ToListAsync();

            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var vatTaxId = (await _taxPercentService.GetVatByStoreId(purchaseOrderHeader.StoreId, DateTime.Now)).TaxId; //The date doesn't matter, I only want the taxId

            var purchaseInvoiceDetails = (
                                    from purchaseOrderDetail in purchaseOrderDetails
                                    from item in items.Where(x => x.ItemId == purchaseOrderDetail.ItemId)
                                    from itemCost in itemCosts.Where(x => x.ItemId == purchaseOrderDetail.ItemId && x.StoreId == purchaseOrderHeader.StoreId).DefaultIfEmpty()
                                    from itemPacking in itemPackings.Where(x => x.ItemId == purchaseOrderDetail.ItemId && x.FromPackageId == purchaseOrderDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                                    from finalStockReceived in finalStocksReceived.Where(x => x.ItemId == purchaseOrderDetail.ItemId && x.ItemPackageId == purchaseOrderDetail.ItemPackageId && x.BarCode == purchaseOrderDetail.BarCode && x.PurchasePrice == purchaseOrderDetail.PurchasePrice && x.ItemDiscountPercent == purchaseOrderDetail.ItemDiscountPercent)
                                    from costCenter in costCenters.Where(x => x.CostCenterId == finalStockReceived.CostCenterId).DefaultIfEmpty()
                                    from lastPurchasePrice in lastPurchasePrices.Where(x => x.ItemId == purchaseOrderDetail.ItemId && x.ItemPackageId == purchaseOrderDetail.ItemPackageId).DefaultIfEmpty()
                                    select new PurchaseInvoiceDetailDto()
                                    {
                                        PurchaseInvoiceDetailId = purchaseOrderDetail.PurchaseOrderDetailId, // <-- used to join with detail taxes
                                        CostCenterId = finalStockReceived.CostCenterId,
                                        CostCenterName = costCenter != null ? (language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn) : null,
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
                                        ExpireDate = finalStockReceived.ExpireDate,
                                        BatchNumber = finalStockReceived.BatchNumber,
                                        Quantity = finalStockReceived.QuantityReceived,
                                        BonusQuantity = finalStockReceived.BonusQuantityReceived,
                                        PurchasePrice = purchaseOrderDetail.PurchasePrice,
                                        ItemDiscountPercent = purchaseOrderDetail.ItemDiscountPercent,
                                        VatPercent = purchaseOrderDetail.VatPercent,
                                        ItemExpensePercent = 0,
                                        ItemExpenseValue = 0,
                                        Notes = purchaseOrderDetail.Notes,
                                        ItemNote = purchaseOrderDetail.ItemNote,
                                        VatTaxId = vatTaxId,
                                        VatTaxTypeId = item.TaxTypeId,
                                        ConsumerPrice = item.ConsumerPrice,
                                        CostPrice = itemCost != null ? itemCost.CostPrice : 0,
                                        CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
                                        LastPurchasePrice = lastPurchasePrice != null ? lastPurchasePrice.PurchasePrice : 0,
                                    }).ToList();

			purchaseInvoiceDetails = CalculateValues(purchaseInvoiceDetails, purchaseOrderHeader.DiscountPercent);
			purchaseInvoiceDetails = purchaseInvoiceDetails.Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();

			var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
            var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);
            var purchaseOrderDetailTaxes = await _purchaseOrderDetailTaxService.GetPurchaseOrderDetailTaxes(purchaseOrderHeaderId).ToListAsync();
            int newId = -1;
            int newSubId = -1;
            foreach (var purchaseInvoiceDetail in purchaseInvoiceDetails)
            {
                purchaseInvoiceDetail.Packages = packages.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
                purchaseInvoiceDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId).ToList();
                purchaseInvoiceDetail.Taxes = purchaseInvoiceDetail.ItemTaxData.ToJson();

                purchaseInvoiceDetail.PurchaseInvoiceDetailTaxes = (
                    from itemTax in purchaseOrderDetailTaxes.Where(x => x.PurchaseOrderDetailId == purchaseInvoiceDetail.PurchaseInvoiceDetailId)
                    select new PurchaseInvoiceDetailTaxDto
                    {
                        TaxId = itemTax.TaxId,
                        TaxTypeId = purchaseInvoiceDetail.VatTaxTypeId, //the VatTaxTypeId should contain the taxTypeId from item
                        DebitAccountId = itemTax.DebitAccountId,
                        TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
                        TaxPercent = itemTax.TaxPercent,
                        TaxValue = CalculateDetailValue.TaxValue(purchaseInvoiceDetail.Quantity, purchaseInvoiceDetail.PurchasePrice, purchaseInvoiceDetail.ItemDiscountPercent, purchaseInvoiceDetail.VatPercent, itemTax.TaxPercent, itemTax.TaxAfterVatInclusive, purchaseOrderHeader.DiscountPercent, false)
                    }
                ).ToList();

                purchaseInvoiceDetail.OtherTaxValue = purchaseInvoiceDetail.PurchaseInvoiceDetailTaxes.Sum(x => x.TaxValue);
                purchaseInvoiceDetail.NetValue = CalculateDetailValue.NetValue(purchaseInvoiceDetail.Quantity, purchaseInvoiceDetail.PurchasePrice, purchaseInvoiceDetail.ItemDiscountPercent, purchaseInvoiceDetail.VatPercent, purchaseInvoiceDetail.OtherTaxValue, purchaseOrderHeader.DiscountPercent, false);

                foreach (var purchaseInvoiceDetailTax in purchaseInvoiceDetail.PurchaseInvoiceDetailTaxes)
                {
                    purchaseInvoiceDetailTax.PurchaseInvoiceDetailId = newId;
                    purchaseInvoiceDetailTax.PurchaseInvoiceDetailTaxId = newSubId--;
                }
                purchaseInvoiceDetail.PurchaseInvoiceDetailId = newId;
                newId--;
            }

            var totalValueFromDetail = purchaseInvoiceDetails.Sum(x => x.TotalValue);
            var totalValueAfterDiscountFromDetail = purchaseInvoiceDetails.Sum(x => x.TotalValueAfterDiscount);
            var totalItemDiscount = purchaseInvoiceDetails.Sum(x => x.ItemDiscountValue);
            var grossValueFromDetail = purchaseInvoiceDetails.Sum(x => x.GrossValue);
            var vatValueFromDetail = purchaseInvoiceDetails.Sum(x => x.VatValue);
            var subNetValueFromDetail = purchaseInvoiceDetails.Sum(x => x.SubNetValue);
            var otherTaxValueFromDetail = purchaseInvoiceDetails.Sum(x => x.OtherTaxValue);
            var netValueFromDetail = purchaseInvoiceDetails.Sum(x => x.NetValue);
            var totalCostValueFromDetail = purchaseInvoiceDetails.Sum(x => x.CostValue);

            var purchaseInvoiceHeader = new PurchaseInvoiceHeaderDto
            {
                PurchaseInvoiceHeaderId = 0,
                PurchaseOrderHeaderId = purchaseOrderHeader.PurchaseOrderHeaderId,
                PurchaseOrderFullCode = purchaseOrderHeader.DocumentFullCode,
                PurchaseOrderDocumentReference = purchaseOrderHeader.DocumentReference,
                SupplierId = purchaseOrderHeader.SupplierId,
                SupplierName = purchaseOrderHeader.SupplierName,
                StoreId = purchaseOrderHeader.StoreId,
                SupplierCode = purchaseOrderHeader.SupplierCode,
                StoreName = purchaseOrderHeader.StoreName,
                DocumentDate = purchaseOrderHeader.DocumentDate,
                Reference = purchaseOrderHeader.Reference,
                IsDirectInvoice = false,
                CreditPayment = purchaseOrderHeader.CreditPayment,
                TaxTypeId = TaxTypeData.Taxable,
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
                TotalInvoiceExpense = 0,
                TotalCostValue = totalCostValueFromDetail,
                DebitAccountId = 0,
                CreditAccountId = 0,
                JournalHeaderId = 0,
                RemarksAr = purchaseOrderHeader.RemarksAr,
                RemarksEn = purchaseOrderHeader.RemarksEn,
                IsOnTheWay = false,
                IsClosed = false,
                IsEnded = false,
                IsBlocked = false,
                ArchiveHeaderId = purchaseOrderHeader.ArchiveHeaderId,
            };

            return new PurchaseInvoiceDto { PurchaseInvoiceHeader = purchaseInvoiceHeader, PurchaseInvoiceDetails = purchaseInvoiceDetails };
        }

		private List<PurchaseInvoiceDetailDto> CalculateValues(List<PurchaseInvoiceDetailDto> purchaseInvoiceDetails, decimal headerDiscountPercent)
		{
			RecalculateDetailValue.RecalculateDetailValues(
				details: purchaseInvoiceDetails,
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

			return purchaseInvoiceDetails;
		}

		public async Task<PurchaseInvoiceDto> GetPurchaseInvoiceFromSupplierQuotation(int supplierQuotationHeaderId)
		{
			var supplierQuotationHeader = await _supplierQuotationHeaderService.GetSupplierQuotationHeaderById(supplierQuotationHeaderId);

			if (supplierQuotationHeader == null)
			{
				return new PurchaseInvoiceDto();
			}

			var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var vatTaxId = (await _taxPercentService.GetVatByStoreId(supplierQuotationHeader.StoreId, DateTime.Now)).TaxId; //The date doesn't matter, I only want the taxId

			var purchaseInvoiceDetails =
			 await (from supplierQuotationDetail in _supplierQuotationDetailService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeaderId).OrderBy(x => x.SupplierQuotationDetailId)
					from item in _itemService.GetAll().Where(x => x.ItemId == supplierQuotationDetail.ItemId)
					from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == supplierQuotationDetail.ItemPackageId)
					from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == supplierQuotationDetail.CostCenterId).DefaultIfEmpty()
					from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == supplierQuotationDetail.ItemId && x.StoreId == supplierQuotationHeader.StoreId).DefaultIfEmpty()
					from itemPacking in _itemPackingService.GetAll().Where(x => x.ItemId == supplierQuotationDetail.ItemId && x.FromPackageId == supplierQuotationDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
					from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.ItemId == supplierQuotationDetail.ItemId && x.ItemPackageId == supplierQuotationDetail.ItemPackageId).OrderByDescending(x => x.PurchaseInvoiceDetailId).Take(1).DefaultIfEmpty()
					select new PurchaseInvoiceDetailDto
					{
						PurchaseInvoiceDetailId = supplierQuotationDetail.SupplierQuotationDetailId, // <-- This is used to get the related detail taxes
						CostCenterId = supplierQuotationDetail.CostCenterId,
						CostCenterName = costCenter != null ? language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn : null,
						ItemId = supplierQuotationDetail.ItemId,
						ItemCode = item.ItemCode,
						ItemName = language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn,
						TaxTypeId = item.TaxTypeId,
                        ItemTypeId = item.ItemTypeId,
						ItemPackageId = supplierQuotationDetail.ItemPackageId,
						ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
						IsItemVatInclusive = supplierQuotationDetail.IsItemVatInclusive,
						BarCode = supplierQuotationDetail.BarCode,
						Packing = itemPacking.Packing,
						Quantity = supplierQuotationDetail.Quantity,
						BonusQuantity = 0,
						PurchasePrice = supplierQuotationDetail.ReceivedPrice,
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
                        VatTaxId = vatTaxId,
                        VatTaxTypeId = item.TaxTypeId,
						ConsumerPrice = item.ConsumerPrice,
						CostPrice = itemCost != null ? itemCost.CostPrice : 0,
						CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
						CostValue = itemCost != null ? itemCost.CostPrice * itemPacking.Packing * supplierQuotationDetail.Quantity : 0,
						LastPurchasePrice = purchaseInvoiceDetail != null ? purchaseInvoiceDetail.PurchasePrice : 0
					}).ToListAsync();


			var itemIds = purchaseInvoiceDetails.Select(x => x.ItemId).ToList();
			var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
			var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);
			var supplierQuotationDetailTaxData = await _supplierQuotationDetailTaxService.GetSupplierQuotationDetailTaxes(supplierQuotationHeaderId).ToListAsync();

			int newId = -1;
			int newSubId = -1;
			foreach (var purchaseInvoiceDetail in purchaseInvoiceDetails)
			{

				purchaseInvoiceDetail.Packages = packages.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
				purchaseInvoiceDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId).ToList();
				purchaseInvoiceDetail.Taxes = purchaseInvoiceDetail.ItemTaxData.ToJson();

				purchaseInvoiceDetail.PurchaseInvoiceDetailTaxes = (
						from itemTax in supplierQuotationDetailTaxData.Where(x => x.SupplierQuotationDetailId == purchaseInvoiceDetail.PurchaseInvoiceDetailId)
						select new PurchaseInvoiceDetailTaxDto
						{
							TaxId = itemTax.TaxId,
                            TaxTypeId = purchaseInvoiceDetail.VatTaxTypeId, //the VatTaxTypeId should contain the taxTypeId from item
							DebitAccountId = itemTax.DebitAccountId,
							TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
							TaxPercent = itemTax.TaxPercent,
							TaxValue = itemTax.TaxValue
						}
					).ToList();

				purchaseInvoiceDetail.PurchaseInvoiceDetailId = newId;
				purchaseInvoiceDetail.PurchaseInvoiceDetailTaxes.ForEach(y =>
				{
					y.PurchaseInvoiceDetailId = newId;
					y.PurchaseInvoiceDetailTaxId = newSubId--;
				});
				newId--;
			}

			var totalCostValueFromDetail = purchaseInvoiceDetails.Sum(x => x.CostValue);

			var purchaseInvoiceHeader = new PurchaseInvoiceHeaderDto
			{
				PurchaseInvoiceHeaderId = 0,
                SupplierQuotationHeaderId = supplierQuotationHeaderId,
				SupplierId = supplierQuotationHeader.SupplierId,
                SupplierCode = supplierQuotationHeader.SupplierCode,
				SupplierName = supplierQuotationHeader.SupplierName,
				StoreId = supplierQuotationHeader.StoreId,
				StoreName = supplierQuotationHeader.StoreName,
				DocumentDate = supplierQuotationHeader.DocumentDate,
				Reference = supplierQuotationHeader.Reference,
                IsDirectInvoice = true,
				CreditPayment = false,
                TaxTypeId = supplierQuotationHeader.TaxTypeId,
				TotalValue = supplierQuotationHeader.TotalValue,
				DiscountPercent = supplierQuotationHeader.DiscountPercent,
				DiscountValue = supplierQuotationHeader.DiscountValue,
				TotalItemDiscount = supplierQuotationHeader.TotalItemDiscount,
				GrossValue = supplierQuotationHeader.GrossValue,
				VatValue = supplierQuotationHeader.VatValue,
				SubNetValue = supplierQuotationHeader.SubNetValue,
				OtherTaxValue = supplierQuotationHeader.OtherTaxValue,
				NetValueBeforeAdditionalDiscount = supplierQuotationHeader.NetValueBeforeAdditionalDiscount,
				AdditionalDiscountValue = supplierQuotationHeader.AdditionalDiscountValue,
				NetValue = supplierQuotationHeader.NetValue,
				TotalCostValue = totalCostValueFromDetail,
				RemarksAr = supplierQuotationHeader.RemarksAr,
				RemarksEn = supplierQuotationHeader.RemarksEn,
                IsOnTheWay = false,
				IsClosed = false,
				IsEnded = false,
				IsBlocked = false,
				ArchiveHeaderId = supplierQuotationHeader.ArchiveHeaderId,
			};

			return new PurchaseInvoiceDto { PurchaseInvoiceHeader = purchaseInvoiceHeader, PurchaseInvoiceDetails = purchaseInvoiceDetails };
		}

		public IQueryable<PurchaseInvoiceHeaderDto> GetPurchaseInvoiceHeadersByStoreIdAndMenuCode(int storeId, int? supplierId, int menuCode, int purchaseInvoiceHeaderId)
        {
            supplierId = supplierId ?? 0;
            if (purchaseInvoiceHeaderId == 0)
            {
                return menuCode switch
                {
                    MenuCodeData.ReceiptFromPurchaseInvoiceOnTheWay => GetPurchaseInvoiceHeadersForStockIn(storeId, (int)supplierId),
                    MenuCodeData.ReturnFromPurchaseInvoice => GetPurchaseInvoiceHeadersForStockInReturn(storeId, (int)supplierId),
                    MenuCodeData.PurchaseInvoiceReturn => GetPurchaseInvoiceHeadersForPurchaseInvoiceReturn(storeId, (int)supplierId),
                    MenuCodeData.CashPurchaseInvoiceReturn => GetPurchaseInvoiceHeadersForDirectPurchaseInvoiceReturn(storeId, (int)supplierId, false),
                    MenuCodeData.CreditPurchaseInvoiceReturn => GetPurchaseInvoiceHeadersForDirectPurchaseInvoiceReturn(storeId, (int)supplierId, true),
                    MenuCodeData.PurchaseInvoiceReturnOnTheWay => GetPurchaseInvoiceHeadersForPurchaseInvoiceReturnOnTheWay(storeId, (int)supplierId),
                    MenuCodeData.SupplierDebitMemo => GetPurchaseInvoiceHeadersForSupplierDebitMemo(storeId, (int)supplierId),
                    MenuCodeData.SupplierCreditMemo => GetPurchaseInvoiceHeadersForSupplierCreditMemo(storeId, (int)supplierId),
                    _ => Enumerable.Empty<PurchaseInvoiceHeaderDto>().AsQueryable()
                };
            }
            else
            {
                return _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaders().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId);
            }
        }

        private IQueryable<PurchaseInvoiceHeaderDto> GetPurchaseInvoiceHeadersForStockIn(int storeId, int supplierId)
        {
            var purchaseInvoiceHeaders = _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsEnded == false && x.IsBlocked == false && x.IsOnTheWay == true);

            return from purchaseInvoiceHeader in purchaseInvoiceHeaders
                   from overallQuantityAvailable in _stockInQuantityService.GetOverallQuantityAvailableFromPurchaseInvoices().Where(x => x.ParentId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId && x.Quantity > 0)
                   select purchaseInvoiceHeader;
        }

        private IQueryable<PurchaseInvoiceHeaderDto> GetPurchaseInvoiceHeadersForStockInReturn(int storeId, int supplierId)
        {
            //StockInReturn cannot be made on direct invoices that are not on the way
            var purchaseInvoiceHeaders = _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsBlocked == false && (x.IsOnTheWay || !x.IsDirectInvoice) && !x.IsSettlementCompleted);

            var excludePurchaseInvoiceIds = _supplierCreditMemoService.GetAll().Select(x => x.PurchaseInvoiceHeaderId)
                .Concat(_supplierDebitMemoService.GetAll().Select(x => x.PurchaseInvoiceHeaderId))
                .Concat(_purchaseInvoiceReturnHeaderService.GetAll().Where(x => !x.IsOnTheWay).Select(x => x.PurchaseInvoiceHeaderId));

            //StockInReturn cannot be made on purchaseinvoice on the way if it does not have purchaseinvoice return on the way
            var hasPurchaseInvoiceReturnOnTheWay = _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.IsOnTheWay).Select(x => x.PurchaseInvoiceHeaderId);

			return from purchaseInvoiceHeader in purchaseInvoiceHeaders.Where(x => !excludePurchaseInvoiceIds.Contains(x.PurchaseInvoiceHeaderId) && (!x.IsOnTheWay || hasPurchaseInvoiceReturnOnTheWay.Contains(x.PurchaseInvoiceHeaderId)))
                   from overallQuantityReturneAvailable in _stockInQuantityService.GetOverallQuantityAvailableReturnFromPurchaseInvoices().Where(x => x.ParentId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId && x.Quantity > 0)
                   select purchaseInvoiceHeader;
        }

        private IQueryable<PurchaseInvoiceHeaderDto> GetPurchaseInvoiceHeadersForPurchaseInvoiceReturn(int storeId, int supplierId)
        {
			//PurchaseInvoiceReturn cannot be made on direct invoices that are not on the way (cash purchase invoice, credit sales invoice)
			var purchaseInvoiceHeaders = _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsBlocked == false && (x.IsOnTheWay || !x.IsDirectInvoice) && !x.IsSettlementCompleted);

            var invoiceIdsAfterStock = _stockInQuantityService.GetOverallQuantityReturnedFromPurchaseInvoices().Where(x => x.Quantity > 0).Select(x => x.ParentId);

			var excludePurchaseInvoiceIds = _supplierCreditMemoService.GetAll().Select(x => x.PurchaseInvoiceHeaderId)
	            .Concat(_supplierDebitMemoService.GetAll().Select(x => x.PurchaseInvoiceHeaderId))
	            .Concat(_purchaseInvoiceReturnHeaderService.GetAll().Where(x => !x.IsOnTheWay).Select(x => x.PurchaseInvoiceHeaderId));

			return purchaseInvoiceHeaders.Where(x => invoiceIdsAfterStock.Contains(x.PurchaseInvoiceHeaderId) && !excludePurchaseInvoiceIds.Contains(x.PurchaseInvoiceHeaderId));
        }

		private IQueryable<PurchaseInvoiceHeaderDto> GetPurchaseInvoiceHeadersForPurchaseInvoiceReturnOnTheWay(int storeId, int supplierId)
		{
            //isEnded check that no invoice returns are already created on the purchase Invoice
			var purchaseInvoiceHeaders = _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsEnded == false && x.IsBlocked == false && x.IsOnTheWay && !x.IsSettlementCompleted);

			var invoiceIdsOnTheWay = _stockInQuantityService.GetOverallQuantityAvailableFromPurchaseInvoices().Where(x => x.Quantity > 0).Select(x => x.ParentId);

			return purchaseInvoiceHeaders.Where(x => invoiceIdsOnTheWay.Contains(x.PurchaseInvoiceHeaderId));
		}

		private IQueryable<PurchaseInvoiceHeaderDto> GetPurchaseInvoiceHeadersForDirectPurchaseInvoiceReturn(int storeId, int supplierId, bool isCreditPayment)
        {
            //Direct purchase invoice returns cannot be created on purchase invoices that have a stock in return
            //Now direct invoice returns can only be made on direct invoices that are not on the way
            var purchaseInvoiceHeaders = _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsEnded == false && x.IsBlocked == false && x.CreditPayment == isCreditPayment && (x.IsDirectInvoice && !x.IsOnTheWay) && !x.IsSettlementCompleted);

            var excludePurchaseInvoiceHeaderIds = _stockInReturnHeaderService.GetAll().Select(x => x.PurchaseInvoiceHeaderId);

            return purchaseInvoiceHeaders.Where(x => !excludePurchaseInvoiceHeaderIds.Contains(x.PurchaseInvoiceHeaderId));
        }

        private IQueryable<PurchaseInvoiceHeaderDto> GetPurchaseInvoiceHeadersForSupplierDebitMemo(int storeId, int supplierId)
        {
            var purchaseInvoiceHeaders = _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsBlocked == false && x.CreditPayment && !x.IsSettlementCompleted);

            var excludePurchaseInvoiceHeaderIds = _purchaseValueService.GetOverallValueOfPurchaseInvoices().Where(x => x.OverallValue <= 0).Select(x => x.PurchaseInvoiceHeaderId);
            
            return purchaseInvoiceHeaders.Where(x => !excludePurchaseInvoiceHeaderIds.Contains(x.PurchaseInvoiceHeaderId));
        }

        private IQueryable<PurchaseInvoiceHeaderDto> GetPurchaseInvoiceHeadersForSupplierCreditMemo(int storeId, int supplierId)
        {
            var purchaseInvoiceHeaders = _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaders().Where(x => x.StoreId == storeId && (supplierId == 0 || x.SupplierId == supplierId) && x.IsBlocked == false && x.CreditPayment);

            return purchaseInvoiceHeaders;
        }

        public async Task<ResponseDto> SavePurchaseInvoice(PurchaseInvoiceDto purchaseInvoice, bool hasApprove, bool approved, int? requestId)
        {
            TrimDetailStrings(purchaseInvoice.PurchaseInvoiceDetails);
            ResponseDto result;

            result = await CheckPurchaseInvoiceValid(purchaseInvoice, PurchaseInvoiceMenuCodeHelper.GetMenuCode(purchaseInvoice.PurchaseInvoiceHeader!));
            if(result.Success == false)
            {
                return result;
            }

            if (purchaseInvoice.PurchaseInvoiceHeader!.PurchaseInvoiceHeaderId == 0)
            {
                await UpdateModelPrices(purchaseInvoice);
            }

            if (purchaseInvoice.PurchaseInvoiceHeader!.IsDirectInvoice == false && purchaseInvoice.PurchaseInvoiceHeader!.IsOnTheWay == false)
            {
                result = await _purchaseInvoiceService.SavePurchaseInvoice(purchaseInvoice, hasApprove, approved, requestId);

                if (result.Success)
                {
                    await _purchaseOrderHeaderService.UpdateEndedAndClosed(purchaseInvoice.PurchaseInvoiceHeader!.PurchaseOrderHeaderId, true, true);
                    await _stockInHeaderService.UpdateAllStockInsEndedDirectlyFromPurchaseOrder(purchaseInvoice.PurchaseInvoiceHeader!.PurchaseOrderHeaderId, true);
                    await _stockInReturnHeaderService.UpdateAllStockInReturnsEndedDirectlyFromPurchaseOrder(purchaseInvoice.PurchaseInvoiceHeader!.PurchaseOrderHeaderId, true);

                    await UpdatePurchaseOrderStatusOnSave(purchaseInvoice.PurchaseInvoiceHeader!, purchaseInvoice.PurchaseInvoiceHeader!.IsOnTheWay);
                }
            }
            else
            {
                result = await SaveDirectPurchaseInvoice(purchaseInvoice, hasApprove, approved, requestId);
            }

            if(result.Success)
            {
                await UpdateItemCosts(purchaseInvoice.PurchaseInvoiceDetails, purchaseInvoice.PurchaseInvoiceHeader!.StoreId,result.Id);

                //The SaveDirectPurchaseInvoice sets the purchaseOrderHeaderId to that of the created PO
                await LinkStockInsToCreatedInvoice(result.Id, purchaseInvoice.PurchaseInvoiceHeader.PurchaseOrderHeaderId, purchaseInvoice.PurchaseInvoiceHeader!.PurchaseInvoiceHeaderId == 0);
            }
            return result;
        }

        private async Task LinkStockInsToCreatedInvoice(int purchaseInvoiceHeaderId, int purchaseOrderHeaderId, bool isCreate)
        {
            if (isCreate)
            {
                var stockInIdsRelatedToPurchaseOrder = await _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).Select(x => x.StockInHeaderId).ToListAsync();

                await _invoiceStockInService.LinkStockInsToPurchaseInvoice(purchaseInvoiceHeaderId, stockInIdsRelatedToPurchaseOrder); 
            }
        }

        private async Task UpdatePurchaseOrderStatusOnSave(PurchaseInvoiceHeaderDto purchaseInvoiceHeader, bool isOnTheWay)
        {
            if(purchaseInvoiceHeader.PurchaseInvoiceHeaderId == 0)
            {
                await _purchaseOrderStatusService.UpdatePurchaseOrderStatus(purchaseInvoiceHeader.PurchaseOrderHeaderId, isOnTheWay ? DocumentStatusData.PurchaseInvoiceCreatedWaitingReceive : DocumentStatusData.PurchaseInvoiceCreated, MenuCodeData.PurchaseInvoiceInterim);
            }
        }

        private void TrimDetailStrings(List<PurchaseInvoiceDetailDto> purchaseInvoiceDetails)
        {
            foreach (var purchaseInvoiceDetail in purchaseInvoiceDetails)
            {
                purchaseInvoiceDetail.BatchNumber = string.IsNullOrWhiteSpace(purchaseInvoiceDetail.BatchNumber) ? null : purchaseInvoiceDetail.BatchNumber.Trim();
                purchaseInvoiceDetail.ItemNote = string.IsNullOrWhiteSpace(purchaseInvoiceDetail.ItemNote) ? null : purchaseInvoiceDetail.ItemNote.Trim();
            }
        }

        public async Task<ResponseDto> DeletePurchaseInvoice(int purchaseInvoiceHeaderId, int menuCode)
        {
            var purchaseInvoiceHeader = await _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => new { x.IsDirectInvoice, x.PurchaseOrderHeaderId, x.IsOnTheWay, x.IsBlocked, x.HasSettlement }).FirstOrDefaultAsync();
            if (purchaseInvoiceHeader == null)
            {
                return new ResponseDto { Id = purchaseInvoiceHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };
            }

            if (purchaseInvoiceHeader.IsBlocked)
            {
                return new ResponseDto { Id = purchaseInvoiceHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
            }

            if (purchaseInvoiceHeader.HasSettlement)
            {
                return new ResponseDto { Id = purchaseInvoiceHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotDeleteBecauseSettlementStarted) };
            }

            //unlinking must be done before delete to prevent foreign key constraint violation
            await _invoiceStockInService.UnlinkStockInsFromPurchaseInvoice(purchaseInvoiceHeaderId);

            var purchaseInvoiceResult = await _purchaseInvoiceService.DeletePurchaseInvoice(purchaseInvoiceHeaderId, menuCode);
            if (purchaseInvoiceResult.Success == false)
            {
                return purchaseInvoiceResult;
            }

            if (purchaseInvoiceHeader.IsDirectInvoice == false && purchaseInvoiceHeader.IsOnTheWay == false)
            {
                //mark purchaseOrder as not ended, I assume there is atleast one stockIn so no need to change isClosed
                await _purchaseOrderHeaderService.UpdateEnded(purchaseInvoiceHeader.PurchaseOrderHeaderId, false);

                await _stockInHeaderService.UpdateAllStockInsEndedDirectlyFromPurchaseOrder(purchaseInvoiceHeader.PurchaseOrderHeaderId, false);
                await _stockInReturnHeaderService.UpdateAllStockInReturnsEndedDirectlyFromPurchaseOrder(purchaseInvoiceHeader.PurchaseOrderHeaderId, false);

                await UpdatePurchaseOrderStatusOnDelete(purchaseInvoiceHeader.PurchaseOrderHeaderId);
                return purchaseInvoiceResult;
            }

            if (!purchaseInvoiceHeader.IsOnTheWay)
            {
                var stockInHeaderId = _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseInvoiceHeader.PurchaseOrderHeaderId).Select(x => x.StockInHeaderId).FirstOrDefault();
                var stockInResult = await _stockInService.DeleteStockIn(stockInHeaderId, MenuCodeData.ReceiptStatement); //not using stockInhandlingService because no validation is needed
                if (stockInResult.Success == false)
                {
                    return stockInResult;
                }
            }

            var purchaseOrderResult = await _purchaseOrderService.DeletePurchaseOrder(purchaseInvoiceHeader.PurchaseOrderHeaderId);
            if(purchaseOrderResult.Success == false)
            {
                return purchaseOrderResult;
            }

            // TODO: Update ItemCost when deleting purchase invoice

            return purchaseInvoiceResult;
        }

        private async Task UpdatePurchaseOrderStatusOnDelete(int purchaseOrderHeaderId)
        {
            await _purchaseOrderStatusService.UpdatePurchaseOrderStatus(purchaseOrderHeaderId, -1, MenuCodeData.PurchaseInvoiceInterim);
        }

        private async Task<ResponseDto> SaveDirectPurchaseInvoice(PurchaseInvoiceDto purchaseInvoice, bool hasApprove, bool approved, int? requestId)
        {
            string? documentReference = null;
            if (purchaseInvoice.PurchaseInvoiceHeader!.PurchaseInvoiceHeaderId == 0)
            {
                if (hasApprove)
                {
                    documentReference = $"{DocumentReferenceData.Approval}{requestId}";
                }
                else
                {
                    int nextPurchaseInvoiceHeaderId = await _purchaseInvoiceHeaderService.GetNextId();
                    documentReference = $"{PurchaseInvoiceMenuCodeHelper.GetDocumentReference(purchaseInvoice.PurchaseInvoiceHeader)}{nextPurchaseInvoiceHeaderId}";
                }
            }

            PurchaseOrderDto purchaseOrder = CreatePurchaseOrderModelFromPurchaseInvoice(purchaseInvoice);

            var purchaseOrderResult = await _purchaseOrderService.SavePurchaseOrder(purchaseOrder, hasApprove, approved, requestId, false, documentReference, !purchaseInvoice.PurchaseInvoiceHeader!.IsOnTheWay ? DocumentStatusData.PurchaseInvoiceCreated : DocumentStatusData.PurchaseInvoiceCreatedWaitingReceive, true);
            if (purchaseOrderResult.Success == false)
            {
                return purchaseOrderResult;
            }

            if (purchaseInvoice.PurchaseInvoiceHeader!.IsOnTheWay == false)
            {
                int? stockInHeaderId = null;
                if (purchaseInvoice.PurchaseInvoiceHeader!.PurchaseInvoiceHeaderId != 0)
                {
                    stockInHeaderId = await _stockInHeaderService.GetStockInHeaders().Where(x => x.PurchaseOrderHeaderId == purchaseInvoice.PurchaseInvoiceHeader!.PurchaseOrderHeaderId).Select(x => (int?)x.StockInHeaderId).FirstOrDefaultAsync();
                    if (stockInHeaderId == null) return new ResponseDto { Message = "No stock In was found" };
                }

                StockInDto stockIn = CreateStockInModelFromPurchaseInvoice(purchaseInvoice, purchaseOrderResult.Id, stockInHeaderId);

                var stockInResult = await _stockInService.SaveStockIn(stockIn, hasApprove, approved, requestId, documentReference, true); //not using stockInhandlingService because no validation is needed
                if (stockInResult.Success == false)
                {
                    return stockInResult;
                } 
            }

            purchaseInvoice.PurchaseInvoiceHeader!.PurchaseOrderHeaderId = purchaseOrderResult.Id;
            return await _purchaseInvoiceService.SavePurchaseInvoice(purchaseInvoice, hasApprove, approved, requestId, documentReference);
        }

        private PurchaseOrderDto CreatePurchaseOrderModelFromPurchaseInvoice(PurchaseInvoiceDto purchaseInvoice)
        {
            return new PurchaseOrderDto
            {
                PurchaseOrderHeader = new PurchaseOrderHeaderDto
                {
                    PurchaseOrderHeaderId = purchaseInvoice.PurchaseInvoiceHeader!.PurchaseInvoiceHeaderId == 0 ? 0 : purchaseInvoice.PurchaseInvoiceHeader!.PurchaseOrderHeaderId,
                    SupplierQuotationHeaderId = purchaseInvoice.PurchaseInvoiceHeader!.IsDirectInvoice ? purchaseInvoice.PurchaseInvoiceHeader!.SupplierQuotationHeaderId : null,
                    DocumentDate = purchaseInvoice.PurchaseInvoiceHeader!.DocumentDate,
                    SupplierId = purchaseInvoice.PurchaseInvoiceHeader!.SupplierId,
                    StoreId = purchaseInvoice.PurchaseInvoiceHeader!.StoreId,
                    Reference = purchaseInvoice.PurchaseInvoiceHeader!.Reference,
                    CreditPayment = purchaseInvoice.PurchaseInvoiceHeader!.CreditPayment,
                    PaymentPeriodDays = null,
                    DueDate = null,
                    ShipmentTypeId = null,
                    DeliveryDate = null,
                    TotalValue = purchaseInvoice.PurchaseInvoiceHeader!.TotalValue,
                    DiscountPercent = purchaseInvoice.PurchaseInvoiceHeader!.DiscountPercent,
                    DiscountValue = purchaseInvoice.PurchaseInvoiceHeader!.DiscountValue,
                    TotalItemDiscount = purchaseInvoice.PurchaseInvoiceHeader!.TotalItemDiscount,
                    GrossValue = purchaseInvoice.PurchaseInvoiceHeader!.GrossValue,
                    VatValue = purchaseInvoice.PurchaseInvoiceHeader!.VatValue,
                    SubNetValue = purchaseInvoice.PurchaseInvoiceHeader!.SubNetValue,
                    OtherTaxValue = purchaseInvoice.PurchaseInvoiceHeader!.OtherTaxValue,
                    NetValueBeforeAdditionalDiscount = purchaseInvoice.PurchaseInvoiceHeader!.NetValueBeforeAdditionalDiscount,
                    AdditionalDiscountValue = purchaseInvoice.PurchaseInvoiceHeader!.AdditionalDiscountValue,
                    NetValue = purchaseInvoice.PurchaseInvoiceHeader!.NetValue,
                    TotalCostValue = purchaseInvoice.PurchaseInvoiceHeader!.TotalCostValue,
                    RemarksAr = purchaseInvoice.PurchaseInvoiceHeader!.RemarksAr,
                    RemarksEn = purchaseInvoice.PurchaseInvoiceHeader!.RemarksEn,
                    IsClosed = true,
                    IsCancelled = false,
                    IsEnded = true,
                    ArchiveHeaderId = purchaseInvoice.PurchaseInvoiceHeader!.ArchiveHeaderId
                },
                PurchaseOrderDetails = (
                    from purchaseInvoiceDetail in purchaseInvoice.PurchaseInvoiceDetails
                    select new PurchaseOrderDetailDto
                    {
                        PurchaseOrderDetailId = 0,
                        PurchaseOrderHeaderId = 0,
                        CostCenterId = purchaseInvoiceDetail.CostCenterId,
                        ItemId = purchaseInvoiceDetail.ItemId,
                        ItemPackageId = purchaseInvoiceDetail.ItemPackageId,
                        BarCode = purchaseInvoiceDetail.BarCode,
                        Packing = purchaseInvoiceDetail.Packing,
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
                        Notes = purchaseInvoiceDetail.Notes,
                        ItemNote = purchaseInvoiceDetail.ItemNote,
                        ConsumerPrice = purchaseInvoiceDetail.ConsumerPrice,
                        CostPrice = purchaseInvoiceDetail.CostPrice,
                        CostPackage = purchaseInvoiceDetail.CostPackage,
                        CostValue = purchaseInvoiceDetail.CostValue,
                        LastPurchasePrice = purchaseInvoiceDetail.LastPurchasePrice,
                        PurchaseOrderDetailTaxes = (
                            from purchaseInvoiceDetailTax in purchaseInvoiceDetail.PurchaseInvoiceDetailTaxes
                            select new PurchaseOrderDetailTaxDto
                            {
                                PurchaseOrderDetailTaxId = 0,
                                PurchaseOrderDetailId = 0,
                                TaxId = purchaseInvoiceDetailTax.TaxId,
                                DebitAccountId = purchaseInvoiceDetailTax.DebitAccountId,
                                TaxPercent = purchaseInvoiceDetailTax.TaxPercent,
                                TaxValue = purchaseInvoiceDetailTax.TaxValue,
                            }).ToList()
                    }).ToList()
            };
        }

        private StockInDto CreateStockInModelFromPurchaseInvoice(PurchaseInvoiceDto purchaseInvoice, int purchaseOrderHeaderId, int? stockInHeaderId)
        {
            return new StockInDto
            {
                StockInHeader = new StockInHeaderDto
                {
                    StockInHeaderId = purchaseInvoice.PurchaseInvoiceHeader!.PurchaseInvoiceHeaderId == 0 ? 0 : (int)stockInHeaderId!,
                    StockTypeId = StockTypeData.ReceiptStatement,
                    PurchaseOrderHeaderId = purchaseOrderHeaderId,
                    PurchaseInvoiceHeaderId = null,
                    SupplierId = purchaseInvoice.PurchaseInvoiceHeader!.SupplierId,
                    StoreId = purchaseInvoice.PurchaseInvoiceHeader!.StoreId,
                    DocumentDate = purchaseInvoice.PurchaseInvoiceHeader!.DocumentDate,
                    Reference = purchaseInvoice.PurchaseInvoiceHeader!.Reference,
                    TotalValue = purchaseInvoice.PurchaseInvoiceHeader!.TotalValue,
                    DiscountPercent = purchaseInvoice.PurchaseInvoiceHeader!.DiscountPercent,
                    DiscountValue = purchaseInvoice.PurchaseInvoiceHeader!.DiscountValue,
                    TotalItemDiscount = purchaseInvoice.PurchaseInvoiceHeader!.TotalItemDiscount,
                    GrossValue = purchaseInvoice.PurchaseInvoiceHeader!.GrossValue,
                    VatValue = purchaseInvoice.PurchaseInvoiceHeader!.VatValue,
                    SubNetValue = purchaseInvoice.PurchaseInvoiceHeader!.SubNetValue,
                    OtherTaxValue = purchaseInvoice.PurchaseInvoiceHeader!.OtherTaxValue,
                    NetValueBeforeAdditionalDiscount = purchaseInvoice.PurchaseInvoiceHeader!.NetValueBeforeAdditionalDiscount,
                    AdditionalDiscountValue = purchaseInvoice.PurchaseInvoiceHeader!.AdditionalDiscountValue,
                    NetValue = purchaseInvoice.PurchaseInvoiceHeader!.NetValue,
                    TotalCostValue = purchaseInvoice.PurchaseInvoiceHeader!.TotalCostValue,
                    RemarksAr = purchaseInvoice.PurchaseInvoiceHeader!.RemarksAr,
                    RemarksEn = purchaseInvoice.PurchaseInvoiceHeader!.RemarksEn,
                    IsClosed = false,
                    IsEnded = true,
                    ArchiveHeaderId = purchaseInvoice.PurchaseInvoiceHeader!.ArchiveHeaderId
                },
                StockInDetails = (
                    from purchaseInvoiceDetail in purchaseInvoice.PurchaseInvoiceDetails
                    select new StockInDetailDto
                    {
                        StockInDetailId = 0,
                        StockInHeaderId = 0,
                        CostCenterId = purchaseInvoiceDetail.CostCenterId,
                        ItemId = purchaseInvoiceDetail.ItemId,
                        ItemPackageId = purchaseInvoiceDetail.ItemPackageId,
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
                        Notes = purchaseInvoiceDetail.Notes,
                        ItemNote = purchaseInvoiceDetail.ItemNote,
                        ConsumerPrice = purchaseInvoiceDetail.ConsumerPrice,
                        CostPrice = purchaseInvoiceDetail.CostPrice,
                        CostPackage = purchaseInvoiceDetail.CostPackage,
                        CostValue = purchaseInvoiceDetail.CostValue,
                        LastPurchasePrice = purchaseInvoiceDetail.LastPurchasePrice,
                        StockInDetailTaxes = (
                            from purchaseInvoiceDetailTax in purchaseInvoiceDetail.PurchaseInvoiceDetailTaxes
                            select new StockInDetailTaxDto
                            {
                                StockInDetailTaxId = 0,
                                StockInDetailId = 0,
                                TaxId = purchaseInvoiceDetailTax.TaxId,
                                DebitAccountId = purchaseInvoiceDetailTax.DebitAccountId,
                                TaxPercent = purchaseInvoiceDetailTax.TaxPercent,
                                TaxValue = purchaseInvoiceDetailTax.TaxValue,
                            }).ToList()
                    }).ToList()
            };
        }

        private async Task UpdateItemCosts(List<PurchaseInvoiceDetailDto> purchaseInvoiceDetails, int storeId,int purchaseInvoiceHeaderId)
        {
            var calculateItemCosts = new CalculateItemCost
            {
                StoreId = storeId,
                CurrentPurchaseInvoiceHeaderId = purchaseInvoiceHeaderId,
				Items = purchaseInvoiceDetails.Select(x => new ItemCostVm
                {
                    ItemId = x.ItemId,
                    ItemPacking = x.Packing,
                    ItemInvoiceQuantity = x.Quantity + x.BonusQuantity,
                    ItemInvoicePrice = x.PurchasePrice,
                    ItemExpenseValue = x.ItemExpenseValue
                }).ToList()
            };

            await _itemCostHandlingService.UpdateItemCosts(calculateItemCosts);
        }

        private async Task UpdateModelPrices(PurchaseInvoiceDto purchaseInvoice)
        {
            await UpdateDetailPrices(purchaseInvoice.PurchaseInvoiceDetails, purchaseInvoice.PurchaseInvoiceHeader!.StoreId);
            purchaseInvoice.PurchaseInvoiceHeader!.TotalCostValue = purchaseInvoice.PurchaseInvoiceDetails.Sum(x => x.CostValue);
        }

        private async Task UpdateDetailPrices(List<PurchaseInvoiceDetailDto> purchaseInvoiceDetails, int storeId)
        {
            var itemIds = purchaseInvoiceDetails.Select(x => x.ItemId).ToList();

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

            foreach (var purchaseInvoiceDetail in purchaseInvoiceDetails)
            {
                var packing = packings.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.FromPackageId == purchaseInvoiceDetail.ItemPackageId).Select(x => x.Packing).FirstOrDefault(1);

                purchaseInvoiceDetail.ConsumerPrice = consumerPrices.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId).Select(x => x.ConsumerPrice).FirstOrDefault(0);
                purchaseInvoiceDetail.CostPrice = itemCosts.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefault(0);
                purchaseInvoiceDetail.CostPackage = purchaseInvoiceDetail.CostPrice * packing;
                purchaseInvoiceDetail.CostValue = purchaseInvoiceDetail.CostPackage * (purchaseInvoiceDetail.Quantity + purchaseInvoiceDetail.BonusQuantity);
                purchaseInvoiceDetail.LastPurchasePrice = lastPurchasePrices.Where(x => x.ItemId == purchaseInvoiceDetail.ItemId && x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId).Select(x => x.PurchasePrice).FirstOrDefault(0);
            }
        }

        private async Task<ResponseDto> CheckPurchaseInvoiceValid(PurchaseInvoiceDto purchaseInvoice, int menuCode)
        {
            var purchaseInvoiceHeader = purchaseInvoice.PurchaseInvoiceHeader;
			ResponseDto flagResult = CheckPurchaseInvoiceFlags(purchaseInvoiceHeader!);
			if (flagResult.Success == false)
			{
				return flagResult;
			}

			ResponseDto closedResult = await CheckPurchaseInvoiceIsClosed(purchaseInvoiceHeader!.PurchaseInvoiceHeaderId, menuCode);
            if (closedResult.Success == false)
            {
                return closedResult;
            }

			ResponseDto settlementResult = await CheckPurchaseInvoiceHasSettlement(purchaseInvoiceHeader!.PurchaseInvoiceHeaderId, menuCode);
			if (settlementResult.Success == false)
			{
				return settlementResult;
			}

			ResponseDto mismatchResult = await CheckPurchaseExpensesJournalMismatch(purchaseInvoiceHeader!, purchaseInvoice.Journal?.JournalHeader, menuCode);
            if (mismatchResult.Success == false)
            {
                return mismatchResult;
            }

            ResponseDto purchaseOrderInvoiced = await CheckPurchaseOrderAlreadyInvoicedOrBlocked(purchaseInvoiceHeader!.PurchaseInvoiceHeaderId, purchaseInvoiceHeader!.PurchaseOrderHeaderId, purchaseInvoiceHeader!.IsDirectInvoice, purchaseInvoiceHeader.IsOnTheWay, menuCode);
            if(purchaseOrderInvoiced.Success == false)
            {
                return purchaseOrderInvoiced;
			}

			ResponseDto itemNoteValidationResult = await _itemNoteValidationService.CheckItemNoteWithItemType(purchaseInvoice.PurchaseInvoiceDetails, x => x.ItemId, x => x.ItemNote);
			if (itemNoteValidationResult.Success == false) return itemNoteValidationResult;

			if (purchaseInvoiceHeader!.IsDirectInvoice == false && purchaseInvoiceHeader!.IsOnTheWay == false)
            {
                var quantityResult = await CheckPurchaseInvoiceQuantity(purchaseInvoice, menuCode);
                if (quantityResult.Success == false)
                {
                    return quantityResult;
                }
            }

            return new ResponseDto { Success = true };
        }

        private ResponseDto CheckPurchaseInvoiceFlags(PurchaseInvoiceHeaderDto purchaseInvoiceHeader)
        {
            if (purchaseInvoiceHeader.IsOnTheWay && !purchaseInvoiceHeader.IsDirectInvoice)
            {
                return new ResponseDto { Success = false, Message = "If IsOnTheWay is true, IsDirectInvoice must also be true" };
            }

			return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckPurchaseInvoiceIsClosed(int purchaseInvoiceHeaderId, int menuCode)
        {
            if (purchaseInvoiceHeaderId != 0)
            {
                var isClosed = await _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.IsClosed).FirstOrDefaultAsync();
                if (isClosed)
                {
                    return new ResponseDto { Id = purchaseInvoiceHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotUpdateBecauseClosed) };
                }
            }

            return new ResponseDto { Success = true };
        }

		private async Task<ResponseDto> CheckPurchaseInvoiceHasSettlement(int purchaseInvoiceHeaderId, int menuCode)
		{
			if (purchaseInvoiceHeaderId != 0)
			{
				var hasSettlement = await _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.HasSettlement).FirstOrDefaultAsync();
				if (hasSettlement)
				{
					return new ResponseDto { Id = purchaseInvoiceHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotUpdateBecauseSettlementStarted) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckPurchaseOrderAlreadyInvoicedOrBlocked(int purchaseInvoiceHeaderId, int purchaseOrderHeaderId, bool isDirectInvoice, bool isOnTheWay, int menuCode)
        {
            var purchaseOrderHeader = await _purchaseOrderHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeaderId).Select(x => new { x.IsEnded, x.IsBlocked }).FirstOrDefaultAsync();
            
            if (isDirectInvoice == false && isOnTheWay == false && purchaseInvoiceHeaderId == 0)
            {
                if (purchaseOrderHeader?.IsEnded == true)
                {
                    return new ResponseDto { Id = purchaseInvoiceHeaderId, Message = await _purchaseMessageService.GetMessage(menuCode, MenuCodeData.PurchaseOrder, PurchaseMessageData.AlreadyInvoiced) };
                }
            }

            if (purchaseOrderHeader?.IsBlocked == true)
            {
                return new ResponseDto { Id = purchaseInvoiceHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckPurchaseInvoiceQuantity(PurchaseInvoiceDto purchaseInvoice, int menuCode)
        {
            var finalStocksReceived = await _stockInQuantityService.GetFinalStocksReceivedFromPurchaseOrderWithAllKeys(purchaseInvoice.PurchaseInvoiceHeader!.PurchaseOrderHeaderId).ToListAsync();
            var purchaseInvoiceDetailsGrouped = _purchaseInvoiceDetailService.GroupPurchaseInvoiceDetails(purchaseInvoice.PurchaseInvoiceDetails);

			var unmatchedQuantity = (from purchaseInvoiceDetailGroup in purchaseInvoiceDetailsGrouped
									 from finalStockReceived in finalStocksReceived.Where(x => x.ItemId == purchaseInvoiceDetailGroup.ItemId && x.ItemPackageId == purchaseInvoiceDetailGroup.ItemPackageId && x.BatchNumber == purchaseInvoiceDetailGroup.BatchNumber && x.ExpireDate == purchaseInvoiceDetailGroup.ExpireDate && x.CostCenterId == purchaseInvoiceDetailGroup.CostCenterId && x.BarCode == purchaseInvoiceDetailGroup.BarCode && x.PurchasePrice == purchaseInvoiceDetailGroup.PurchasePrice && x.ItemDiscountPercent == purchaseInvoiceDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
									 select new
									 {
										 purchaseInvoiceDetailGroup.ItemId,
										 purchaseInvoiceDetailGroup.ItemPackageId,
										 PurchaseInvoiceQuantity = purchaseInvoiceDetailGroup.Quantity,
										 PurchaseInvoiceBonusQuantity = purchaseInvoiceDetailGroup.BonusQuantity,
										 QuantityReceived = finalStockReceived != null ? finalStockReceived.QuantityReceived : 0,
										 BonusQuantityReceived = finalStockReceived != null ? finalStockReceived.BonusQuantityReceived : 0
									 }).FirstOrDefault(x => x.QuantityReceived != x.PurchaseInvoiceQuantity || x.BonusQuantityReceived != x.PurchaseInvoiceBonusQuantity);

			if (unmatchedQuantity != null)
            {
                var language = _httpContextAccessor.GetProgramCurrentLanguage();

                var itemName = await _itemService.GetAll().Where(x => x.ItemId == unmatchedQuantity.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
                var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == unmatchedQuantity.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

                if (unmatchedQuantity.QuantityReceived != unmatchedQuantity.PurchaseInvoiceQuantity)
                {
                    return new ResponseDto { Id = purchaseInvoice.PurchaseInvoiceHeader.PurchaseInvoiceHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, MenuCodeData.PurchaseOrder, PurchaseMessageData.QuantityNotMatchingReceived, itemName!, itemPackageName!, unmatchedQuantity.PurchaseInvoiceQuantity.ToNormalizedString(), unmatchedQuantity.QuantityReceived.ToNormalizedString()) };
                }
                else
                {
                    return new ResponseDto { Id = purchaseInvoice.PurchaseInvoiceHeader.PurchaseInvoiceHeaderId, Success = false, Message = await _purchaseMessageService.GetMessage(menuCode, MenuCodeData.PurchaseOrder, PurchaseMessageData.BonusQuantityNotMatchingReceived, itemName!, itemPackageName!, unmatchedQuantity.PurchaseInvoiceBonusQuantity.ToNormalizedString(), unmatchedQuantity.BonusQuantityReceived.ToNormalizedString()) };
                }
            }

            return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckPurchaseExpensesJournalMismatch(PurchaseInvoiceHeaderDto purchaseInvoiceHeader, JournalHeaderDto? journalHeader, int menuCode)
        {
            var exceedValueSetting = await _documentExceedValueSettingService.GetSettingByMenuCode(menuCode, purchaseInvoiceHeader.StoreId);
            if (exceedValueSetting) return new ResponseDto { Success = true };

            int rounding = await _storeService.GetStoreHeaderRounding(purchaseInvoiceHeader.StoreId);

            decimal journalDebitValue = NumberHelper.RoundNumber(journalHeader?.TotalDebitValue ?? 0, rounding);
            decimal invoiceValue = NumberHelper.RoundNumber(purchaseInvoiceHeader.NetValue + purchaseInvoiceHeader.AdditionalDiscountValue + purchaseInvoiceHeader.TotalInvoiceExpense, rounding);

            if (journalDebitValue != invoiceValue)
            {
                return new ResponseDto { Success = false, Id = purchaseInvoiceHeader.PurchaseInvoiceHeaderId, Message = await _purchaseMessageService.GetMessage(menuCode, PurchaseMessageData.ValueNotMatchingJournalDebit) };
            }
            else
            {
                return new ResponseDto { Success = true };
            }
        }
    }
}
