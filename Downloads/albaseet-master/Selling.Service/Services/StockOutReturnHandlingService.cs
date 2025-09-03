using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Items;
using Shared.Helper.Extensions;
using Shared.Service.Services.Items;
using Shared.Service.Logic.Calculation;
using Sales.CoreOne.Models.Domain;
using Microsoft.Extensions.Localization;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Contracts.Menus;
using Sales.CoreOne.Models.StaticData;
using Shared.Helper.Logic;
using Purchases.Service.Services;

namespace Sales.Service.Services
{
    public class StockOutReturnHandlingService: IStockOutReturnHandlingService
    {
		private readonly IGenericMessageService _genericMessageService;
		private readonly ISalesMessageService _salesMessageService;
		private readonly IProformaInvoiceStatusService _proformaInvoiceStatusService;
		private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
		private readonly ISalesInvoiceDetailTaxService _salesInvoiceDetailTaxService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly ISalesInvoiceReturnDetailService _salesInvoiceReturnDetailService;
		private readonly IProformaInvoiceDetailService _proformaInvoiceDetailService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly IStockOutHeaderService _stockOutHeaderService;
		private readonly IStockOutQuantityService _stockOutQuantityService;
		private readonly IStockOutDetailService _stockOutDetailService;
        private readonly IStockOutDetailTaxService _stockOutDetailTaxService;
        private readonly IItemTaxService _itemTaxService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IStockOutReturnService _stockOutReturnService;
        private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
        private readonly IStockOutReturnDetailService _stockOutReturnDetailService;
        private readonly IStockOutReturnDetailTaxService _stockOutReturnDetailTaxService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IItemCostService _itemCostService;
        private readonly IItemService _itemService;
        private readonly ISalesInvoiceService _salesInvoiceService;
        private readonly IItemPackingService _itemPackingService;
		private readonly IClientCreditMemoService _clientCreditMemoService;
		private readonly IClientDebitMemoService _clientDebitMemoService;
		private readonly ISalesValueService _salesValueService;
		private readonly IGetSalesInvoiceSettleValueService _getSalesInvoiceSettleValueService;
		private readonly IZeroStockValidationService _zeroStockValidationService;
		private readonly IStoreService _storeService;
		private readonly IItemNoteValidationService _itemNoteValidationService;

        public StockOutReturnHandlingService(IGenericMessageService genericMessageService, ISalesMessageService salesMessageService, IProformaInvoiceStatusService proformaInvoiceStatusService, ISalesInvoiceDetailService salesInvoiceDetailService, ISalesInvoiceDetailTaxService salesInvoiceDetailTaxService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService, IProformaInvoiceDetailService proformaInvoiceDetailService, IItemPackageService itemPackageService, IHttpContextAccessor httpContextAccessor, ISalesInvoiceHeaderService salesInvoiceHeaderService, IStockOutHeaderService stockOutHeaderService, IStockOutQuantityService stockOutQuantityService, IStockOutDetailService stockOutDetailService, IStockOutDetailTaxService stockOutDetailTaxService, IItemTaxService itemTaxService, IItemBarCodeService itemBarCodeService, IStockOutReturnService stockOutReturnService, IStockOutReturnHeaderService stockOutReturnHeaderService, IStockOutReturnDetailService stockOutReturnDetailService, IStockOutReturnDetailTaxService stockOutReturnDetailTaxService, IMenuNoteService menuNoteService, IItemCostService itemCostService, IItemService itemService, ISalesInvoiceService salesInvoiceService, IItemPackingService itemPackingService, IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService, ISalesValueService salesValueService, IGetSalesInvoiceSettleValueService getSalesInvoiceSettleValueService, IZeroStockValidationService zeroStockValidationService, IStoreService storeService, IItemNoteValidationService itemNoteValidationService)
        {
			_genericMessageService = genericMessageService;
			_salesMessageService = salesMessageService;
			_proformaInvoiceStatusService = proformaInvoiceStatusService;
			_salesInvoiceDetailService = salesInvoiceDetailService;
			_salesInvoiceDetailTaxService = salesInvoiceDetailTaxService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_salesInvoiceReturnDetailService = salesInvoiceReturnDetailService;
			_proformaInvoiceDetailService = proformaInvoiceDetailService;
			_itemPackageService = itemPackageService;
			_httpContextAccessor = httpContextAccessor;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_stockOutHeaderService = stockOutHeaderService;
			_stockOutQuantityService = stockOutQuantityService;
			_stockOutDetailService = stockOutDetailService;
			_stockOutDetailTaxService = stockOutDetailTaxService;
            _itemTaxService = itemTaxService;
            _itemBarCodeService = itemBarCodeService;
            _stockOutReturnService = stockOutReturnService;
            _stockOutReturnHeaderService = stockOutReturnHeaderService;
            _stockOutReturnDetailService = stockOutReturnDetailService;
            _stockOutReturnDetailTaxService = stockOutReturnDetailTaxService;
            _menuNoteService = menuNoteService;
            _itemCostService = itemCostService;
            _itemService = itemService;
            _salesInvoiceService = salesInvoiceService;
            _itemPackingService = itemPackingService;
			_clientDebitMemoService = clientDebitMemoService;
			_clientCreditMemoService = clientCreditMemoService;
			_salesValueService = salesValueService;
			_getSalesInvoiceSettleValueService = getSalesInvoiceSettleValueService;
			_zeroStockValidationService = zeroStockValidationService;
			_storeService = storeService;
			_itemNoteValidationService = itemNoteValidationService;
        }

        public async Task<StockOutReturnDto> GetStockOutReturn(int stockOutReturnHeaderId)
        {
            var header = await _stockOutReturnHeaderService.GetStockOutReturnHeaderById(stockOutReturnHeaderId);
            if (header == null) { return new StockOutReturnDto(); }

            var details = await GetStockOutReturnDetailsCalculated(stockOutReturnHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(StockTypeData.ToMenuCode(header.StockTypeId), stockOutReturnHeaderId).ToListAsync();
            var stockOutReturnDetailTaxes = await _stockOutReturnDetailTaxService.GetStockOutReturnDetailTaxes(stockOutReturnHeaderId).ToListAsync();

            foreach (var detail in details)
            {
                detail.StockOutReturnDetailTaxes = stockOutReturnDetailTaxes.Where(x => x.StockOutReturnDetailId == detail.StockOutReturnDetailId).ToList();
            }

            return new StockOutReturnDto() { StockOutReturnHeader = header, StockOutReturnDetails = details, MenuNotes = menuNotes };
        }

        public async Task<StockOutReturnDto> GetStockOutReturnFromStockOut(int stockOutHeaderId)
        {
            var stockOutHeader = await _stockOutHeaderService.GetStockOutHeaderById(stockOutHeaderId);
			if (stockOutHeader == null)
			{
				return new StockOutReturnDto();
			}

			var stockTypeId = stockOutHeader.StockTypeId == StockTypeData.StockOutFromProformaInvoice ? StockTypeData.StockOutReturnFromStockOut : StockTypeData.StockOutReturnFromInvoicedStockOut;

			var stockOutReturnDetails = await GetStockOutReturnDetailsFromStockOut(stockOutHeaderId, stockOutHeader.DiscountPercent);
			var stockOutReturnHeader = GetStockOutReturnHeaderFromParent(stockOutReturnDetails, new StockOutReturnFromParentDto { 
				StockTypeId = stockTypeId, 
				StockOutHeaderId = stockOutHeader.StockOutHeaderId, 
				StockOutFullCode = stockOutHeader.DocumentFullCode, 
				StockOutDocumentReference = stockOutHeader.DocumentReference,
				StockOutProformaInvoiceHeaderId = stockOutHeader.ProformaInvoiceHeaderId,
				StockOutProformaInvoiceFullCode = stockOutHeader.ProformaInvoiceFullCode,
				StockOutProformaInvoiceDocumentReference = stockOutHeader.ProformaInvoiceDocumentReference,
				StockOutSalesInvoiceHeaderId = stockOutHeader.SalesInvoiceHeaderId,
				StockOutSalesInvoiceFullCode = stockOutHeader.SalesInvoiceFullCode,
				StockOutSalesInvoiceDocumentReference = stockOutHeader.SalesInvoiceDocumentReference, 
				ClientId = stockOutHeader.ClientId, 
				ClientCode = stockOutHeader.ClientCode,
				ClientName = stockOutHeader.ClientName, 
				SellerId = stockOutHeader.SellerId,
				SellerCode = stockOutHeader.SellerCode,
				SellerName = stockOutHeader.SellerName,
				StoreId = stockOutHeader.StoreId, 
				StoreName = stockOutHeader.StoreName, 
				DocumentDate = stockOutHeader.DocumentDate, 
				Reference = stockOutHeader.Reference, 
				DiscountPercent = stockOutHeader.DiscountPercent, 
				AdditionalDiscountValue = stockOutHeader.AdditionalDiscountValue,
				RemarksAr = stockOutHeader.RemarksAr, 
				RemarksEn = stockOutHeader.RemarksEn, 
				ArchiveHeaderId = stockOutHeader.ArchiveHeaderId });

			return new StockOutReturnDto { StockOutReturnHeader = stockOutReturnHeader, StockOutReturnDetails = stockOutReturnDetails };
        }

        private async Task<List<StockOutReturnDetailDto>> GetStockOutReturnDetailsFromStockOut(int stockOutHeaderId, decimal headerDiscountPercent)
        {
			var stockOutDetails = await _stockOutDetailService.GetStockOutDetailsAsQueryable(stockOutHeaderId).ToListAsync();
			var stockOutDetailsGrouped = _stockOutDetailService.GroupStockOutDetailsWithAllKeys(stockOutDetails);

			var itemIds = stockOutDetails.Select(x => x.ItemId).ToList();
			var items = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var itemPackings = await _itemPackingService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var lastSalesPrices = await _salesInvoiceService.GetMultipleLastSalesPrices(itemIds);

			var stocksReturned = await _stockOutQuantityService.GetStocksDisbursedReturnedFromStockOut(stockOutHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

			var stockOutReturnDetails = (from stockOutDetail in stockOutDetails
										 from stockReturned in stocksReturned.Where(x => x.ItemId == stockOutDetail.ItemId && x.ItemPackageId == stockOutDetail.ItemPackageId && x.BarCode == stockOutDetail.BarCode && x.SellingPrice == stockOutDetail.SellingPrice && x.CostCenterId == stockOutDetail.CostCenterId && x.ExpireDate == stockOutDetail.ExpireDate && x.BatchNumber == stockOutDetail.BatchNumber && x.ItemDiscountPercent == stockOutDetail.ItemDiscountPercent).DefaultIfEmpty()
										 from stockOutDetailGroup in stockOutDetailsGrouped.Where(x => x.ItemId == stockOutDetail.ItemId && x.ItemPackageId == stockOutDetail.ItemPackageId && x.BarCode == stockOutDetail.BarCode && x.SellingPrice == stockOutDetail.SellingPrice && x.CostCenterId == stockOutDetail.CostCenterId && x.ExpireDate == stockOutDetail.ExpireDate && x.BatchNumber == stockOutDetail.BatchNumber && x.ItemDiscountPercent == stockOutDetail.ItemDiscountPercent)
										 from item in items.Where(x => x.ItemId == stockOutDetail.ItemId)
										 from itemPacking in itemPackings.Where(x => x.ItemId == stockOutDetail.ItemId && x.FromPackageId == stockOutDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
										 from itemCost in itemCosts.Where(x => x.ItemId == stockOutDetail.ItemId && x.ItemPackageId == stockOutDetail.ItemPackageId).DefaultIfEmpty()
										 from lastSalesPrice in lastSalesPrices.Where(x => x.ItemId == stockOutDetail.ItemId && x.ItemPackageId == stockOutDetail.ItemPackageId).DefaultIfEmpty()
										 select new StockOutReturnDetailDto
										 {
											 StockOutReturnDetailId = stockOutDetail.StockOutDetailId, // <-- This is used to get the related detail taxes
											 StockOutReturnHeaderId = 0,
											 CostCenterId = stockOutDetail.CostCenterId,
											 CostCenterName = stockOutDetail.CostCenterName,
											 ItemId = stockOutDetail.ItemId,
											 ItemCode = stockOutDetail.ItemCode,
											 ItemName = stockOutDetail.ItemName,
											 TaxTypeId = stockOutDetail.TaxTypeId,
											 ItemTypeId = stockOutDetail.ItemTypeId,
											 ItemPackageId = stockOutDetail.ItemPackageId,
											 ItemPackageName = stockOutDetail.ItemPackageName,
											 IsItemVatInclusive = stockOutDetail.IsItemVatInclusive,
											 BarCode = stockOutDetail.BarCode,
											 Packing = stockOutDetail.Packing,
											 ExpireDate = stockOutDetail.ExpireDate,
											 BatchNumber = stockOutDetail.BatchNumber,
											 Quantity = stockOutDetail.Quantity,
											 BonusQuantity = stockOutDetail.BonusQuantity,
											 AvailableQuantity = stockOutDetailGroup.Quantity - (stockReturned != null ? stockReturned.QuantityReturned : 0),
											 AvailableBonusQuantity = stockOutDetailGroup.BonusQuantity - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0),
											 StockOutQuantity = stockOutDetailGroup.Quantity,
											 StockOutBonusQuantity = stockOutDetailGroup.BonusQuantity,
											 SellingPrice = stockOutDetail.SellingPrice,
											 ItemDiscountPercent = stockOutDetail.ItemDiscountPercent,
											 VatPercent = stockOutDetail.VatPercent,
											 Notes = stockOutDetail.Notes,
											 ItemNote = stockOutDetail.ItemNote,
											 ConsumerPrice = item.ConsumerPrice,
											 CostPrice = itemCost != null ? itemCost.CostPrice : 0,
											 CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
											 LastSalesPrice = lastSalesPrice != null ? lastSalesPrice.SellingPrice : 0,

											 CreatedAt = stockOutDetail.CreatedAt,
											 IpAddressCreated = stockOutDetail.IpAddressCreated,
											 UserNameCreated = stockOutDetail.UserNameCreated,
										 }).ToList();

			stockOutReturnDetails = DistributeQuantityAndCalculateValues(stockOutReturnDetails, headerDiscountPercent);
			stockOutReturnDetails = stockOutReturnDetails.Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();

			await CalculateOtherTaxesFromStockOut(stockOutReturnDetails, headerDiscountPercent, stockOutHeaderId);
			await GetAuxiliaryData(stockOutReturnDetails);
			SerializeStockOutReturnDetails(stockOutReturnDetails);

			return stockOutReturnDetails;
		}

		private async Task<List<StockOutReturnDetailDto>> GetStockOutReturnDetailsFromSalesInvoice(int salesInvoiceHeaderId, decimal headerDiscountPercent)
		{
			var salesInvoiceDetails = await _salesInvoiceDetailService.GetSalesInvoiceDetailsAsQueryable(salesInvoiceHeaderId).ToListAsync();
			var salesInvoiceDetailsGrouped = _salesInvoiceDetailService.GroupSalesInvoiceDetails(salesInvoiceDetails);

			var itemIds = salesInvoiceDetails.Select(x => x.ItemId).ToList();
			var items = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var itemPackings = await _itemPackingService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var lastSalesPrices = await _salesInvoiceService.GetMultipleLastSalesPrices(itemIds);

			var stocksReturned = await _stockOutQuantityService.GetStocksDisbursedReturnedFromSalesInvoice(salesInvoiceHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var reservationInvoiceCloseOutDetails = await GetReservationInvoiceCloseOutQuantities(salesInvoiceHeaderId);

			var stockOutReturnDetails = (from salesInvoiceDetail in salesInvoiceDetails
										 from stockReturned in stocksReturned.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId && x.BarCode == salesInvoiceDetail.BarCode && x.SellingPrice == salesInvoiceDetail.SellingPrice && x.CostCenterId == salesInvoiceDetail.CostCenterId && x.ExpireDate == salesInvoiceDetail.ExpireDate && x.BatchNumber == salesInvoiceDetail.BatchNumber && x.ItemDiscountPercent == salesInvoiceDetail.ItemDiscountPercent).DefaultIfEmpty()
										 from salesInvoiceDetailGroup in salesInvoiceDetailsGrouped.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId && x.BarCode == salesInvoiceDetail.BarCode && x.SellingPrice == salesInvoiceDetail.SellingPrice && x.CostCenterId == salesInvoiceDetail.CostCenterId && x.ExpireDate == salesInvoiceDetail.ExpireDate && x.BatchNumber == salesInvoiceDetail.BatchNumber && x.ItemDiscountPercent == salesInvoiceDetail.ItemDiscountPercent)
										 from reservationInvoiceCloseOutDetail in reservationInvoiceCloseOutDetails.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId && x.BarCode == salesInvoiceDetail.BarCode && x.SellingPrice == salesInvoiceDetail.SellingPrice && x.CostCenterId == salesInvoiceDetail.CostCenterId && x.ExpireDate == salesInvoiceDetail.ExpireDate && x.BatchNumber == salesInvoiceDetail.BatchNumber && x.ItemDiscountPercent == salesInvoiceDetail.ItemDiscountPercent).DefaultIfEmpty()
										 from item in items.Where(x => x.ItemId == salesInvoiceDetail.ItemId)
										 from itemPacking in itemPackings.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.FromPackageId == salesInvoiceDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
										 from itemCost in itemCosts.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId).DefaultIfEmpty()
										 from lastSalesPrice in lastSalesPrices.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId).DefaultIfEmpty()
										 select new StockOutReturnDetailDto
										 {
											 StockOutReturnDetailId = salesInvoiceDetail.SalesInvoiceDetailId, // <-- This is used to get the related detail taxes
											 StockOutReturnHeaderId = 0,
											 CostCenterId = salesInvoiceDetail.CostCenterId,
											 CostCenterName = salesInvoiceDetail.CostCenterName,
											 ItemId = salesInvoiceDetail.ItemId,
											 ItemCode = salesInvoiceDetail.ItemCode,
											 ItemName = salesInvoiceDetail.ItemName,
											 TaxTypeId = salesInvoiceDetail.TaxTypeId,
											 ItemTypeId = salesInvoiceDetail.ItemTypeId,
											 ItemPackageId = salesInvoiceDetail.ItemPackageId,
											 ItemPackageName = salesInvoiceDetail.ItemPackageName,
											 IsItemVatInclusive = salesInvoiceDetail.IsItemVatInclusive,
											 BarCode = salesInvoiceDetail.BarCode,
											 Packing = salesInvoiceDetail.Packing,
											 ExpireDate = salesInvoiceDetail.ExpireDate,
											 BatchNumber = salesInvoiceDetail.BatchNumber,
											 Quantity = salesInvoiceDetail.Quantity,
											 BonusQuantity = salesInvoiceDetail.BonusQuantity,
											 AvailableQuantity = salesInvoiceDetailGroup.Quantity - (reservationInvoiceCloseOutDetail != null ? reservationInvoiceCloseOutDetail.Quantity : 0) - (stockReturned != null ? stockReturned.QuantityReturned : 0),
											 AvailableBonusQuantity = salesInvoiceDetailGroup.BonusQuantity - (reservationInvoiceCloseOutDetail != null ? reservationInvoiceCloseOutDetail.BonusQuantity : 0) - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0),
											 SalesInvoiceQuantity = salesInvoiceDetailGroup.Quantity,
											 SalesInvoiceBonusQuantity = salesInvoiceDetailGroup.BonusQuantity,
											 SellingPrice = salesInvoiceDetail.SellingPrice,
											 ItemDiscountPercent = salesInvoiceDetail.ItemDiscountPercent,
											 VatPercent = salesInvoiceDetail.VatPercent,
											 Notes = salesInvoiceDetail.Notes,
											 ItemNote = salesInvoiceDetail.ItemNote,
											 ConsumerPrice = item.ConsumerPrice,
											 CostPrice = itemCost != null ? itemCost.CostPrice : 0,
											 CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
											 LastSalesPrice = lastSalesPrice != null ? lastSalesPrice.SellingPrice : 0,

											 CreatedAt = salesInvoiceDetail.CreatedAt,
											 IpAddressCreated = salesInvoiceDetail.IpAddressCreated,
											 UserNameCreated = salesInvoiceDetail.UserNameCreated,
										 }).ToList();

			stockOutReturnDetails = DistributeQuantityAndCalculateValues(stockOutReturnDetails, headerDiscountPercent);
			stockOutReturnDetails = stockOutReturnDetails.Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();

			await CalculateOtherTaxesFromSalesInvoice(stockOutReturnDetails, headerDiscountPercent, salesInvoiceHeaderId);
			await GetAuxiliaryData(stockOutReturnDetails);
			SerializeStockOutReturnDetails(stockOutReturnDetails);

			return stockOutReturnDetails;
		}

		private List<StockOutReturnDetailDto> DistributeQuantityAndCalculateValues(List<StockOutReturnDetailDto> stockOutReturnDetails, decimal headerDiscountPercent)
		{
			QuantityDistributionLogic.DistributeQuantitiesOnDetails(
				details: stockOutReturnDetails,
				keySelector: x => (x.ItemId, x.ItemPackageId, x.BarCode, x.CostCenterId, x.SellingPrice, x.ItemDiscountPercent, x.ExpireDate, x.BatchNumber),
				availableQuantitySelector: x => x.AvailableQuantity,
				availableBonusQuantitySelector: x => x.AvailableBonusQuantity,
				quantitySelector: x => x.Quantity,
				bonusQuantitySelector: x => x.BonusQuantity,
				quantityAssigner: (x, value) => x.Quantity = value,
				bonusQuantityAssigner: (x, value) => x.BonusQuantity = value
			);

			RecalculateDetailValue.RecalculateDetailValues(
				details: stockOutReturnDetails,
				quantitySelector: x => x.Quantity,
				bonusQuantitySelector: x => x.BonusQuantity,
				priceSelector: x => x.SellingPrice,
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

			return stockOutReturnDetails;
		}

		private StockOutReturnHeaderDto GetStockOutReturnHeaderFromParent(List<StockOutReturnDetailDto> stockOutReturnDetails, StockOutReturnFromParentDto parent)
		{
			var totalValueFromDetail = stockOutReturnDetails.Sum(x => x.TotalValue);
			var totalValueAfterDiscountFromDetail = stockOutReturnDetails.Sum(x => x.TotalValueAfterDiscount);
			var totalItemDiscount = stockOutReturnDetails.Sum(x => x.ItemDiscountValue);
			var grossValueFromDetail = stockOutReturnDetails.Sum(x => x.GrossValue);
			var vatValueFromDetail = stockOutReturnDetails.Sum(x => x.VatValue);
			var subNetValueFromDetail = stockOutReturnDetails.Sum(x => x.SubNetValue);
			var otherTaxValueFromDetail = stockOutReturnDetails.Sum(x => x.OtherTaxValue);
			var netValueFromDetail = stockOutReturnDetails.Sum(x => x.NetValue);
			var totalCostValueFromDetail = stockOutReturnDetails.Sum(x => x.CostValue);

			return new StockOutReturnHeaderDto
			{
				StockOutReturnHeaderId = 0,
				StockTypeId = parent.StockTypeId,
				StockOutHeaderId = parent.StockOutHeaderId,
				StockOutFullCode = parent.StockOutFullCode,
				StockOutDocumentReference = parent.StockOutDocumentReference,
				StockOutProformaInvoiceHeaderId = parent.StockOutProformaInvoiceHeaderId,
				StockOutProformaInvoiceFullCode = parent.StockOutProformaInvoiceFullCode,
				StockOutProformaInvoiceDocumentReference = parent.StockOutProformaInvoiceDocumentReference,
				StockOutSalesInvoiceHeaderId = parent.StockOutSalesInvoiceHeaderId,
				StockOutSalesInvoiceFullCode = parent.StockOutSalesInvoiceFullCode,
				StockOutSalesInvoiceDocumentReference = parent.StockOutSalesInvoiceDocumentReference,
				SalesInvoiceHeaderId = parent.SalesInvoiceHeaderId,
				SalesInvoiceFullCode = parent.SalesInvoiceFullCode,
				SalesInvoiceDocumentReference = parent.SalesInvoiceDocumentReference,
				ClientId = parent.ClientId,
				ClientCode = parent.ClientCode,
				ClientName = parent.ClientName,
				SellerId = parent.SellerId,
				SellerCode = parent.SellerCode,
				SellerName = parent.SellerName,
				StoreId = parent.StoreId,
				StoreName = parent.StoreName,
				DocumentDate = parent.DocumentDate,
				Reference = parent.Reference,
				TotalValue = totalValueFromDetail,
				DiscountPercent = parent.DiscountPercent,
				DiscountValue = CalculateHeaderValue.DiscountValue(totalValueAfterDiscountFromDetail, parent.DiscountPercent),
				TotalItemDiscount = totalItemDiscount,
				GrossValue = grossValueFromDetail,
				VatValue = vatValueFromDetail,
				SubNetValue = subNetValueFromDetail,
				OtherTaxValue = otherTaxValueFromDetail,
				NetValueBeforeAdditionalDiscount = netValueFromDetail,
				AdditionalDiscountValue = parent.AdditionalDiscountValue,
				NetValue = CalculateHeaderValue.NetValue(netValueFromDetail, parent.AdditionalDiscountValue),
				TotalCostValue = totalCostValueFromDetail,
				RemarksAr = parent.RemarksAr,
				RemarksEn = parent.RemarksEn,
				IsClosed = false,
				IsEnded = false,
				IsBlocked = false,
				ArchiveHeaderId = parent.ArchiveHeaderId
			};
		}

		public async Task<StockOutReturnDto> GetStockOutReturnFromSalesInvoice(int salesInvoiceHeaderId)
        {
			var salesInvoiceHeader = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById(salesInvoiceHeaderId);
			if (salesInvoiceHeader == null)
			{
				return new StockOutReturnDto();
			}

			var stockOutReturnDetails = await GetStockOutReturnDetailsFromSalesInvoice(salesInvoiceHeaderId, salesInvoiceHeader.DiscountPercent);
			var stockOutReturnHeader = GetStockOutReturnHeaderFromParent(stockOutReturnDetails, new StockOutReturnFromParentDto
			{
				StockTypeId = StockTypeData.StockOutReturnFromSalesInvoice,
				SalesInvoiceHeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
				SalesInvoiceFullCode = salesInvoiceHeader.DocumentFullCode,
				SalesInvoiceDocumentReference = salesInvoiceHeader.DocumentReference,
				ClientId = salesInvoiceHeader.ClientId,
				ClientCode = salesInvoiceHeader.ClientCode,
				ClientName = salesInvoiceHeader.ClientName,
				SellerId = salesInvoiceHeader.SellerId,
				SellerCode = salesInvoiceHeader.SellerCode,
				SellerName = salesInvoiceHeader.SellerName,
				StoreId = salesInvoiceHeader.StoreId,
				StoreName = salesInvoiceHeader.StoreName,
				DocumentDate = salesInvoiceHeader.DocumentDate,
				Reference = salesInvoiceHeader.Reference,
				DiscountPercent = salesInvoiceHeader.DiscountPercent,
				AdditionalDiscountValue = salesInvoiceHeader.AdditionalDiscountValue,
				RemarksAr = salesInvoiceHeader.RemarksAr,
				RemarksEn = salesInvoiceHeader.RemarksEn,
				ArchiveHeaderId = salesInvoiceHeader.ArchiveHeaderId
			});

			return new StockOutReturnDto { StockOutReturnHeader = stockOutReturnHeader, StockOutReturnDetails = stockOutReturnDetails };
		}

		private async Task GetAuxiliaryData(List<StockOutReturnDetailDto> stockOutReturnDetails)
		{
			var itemIds = stockOutReturnDetails.Select(x => x.ItemId).ToList();
			var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
			var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

			foreach (var stockOutReturnDetail in stockOutReturnDetails)
			{
				stockOutReturnDetail.Packages = packages.Where(x => x.ItemId == stockOutReturnDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
				stockOutReturnDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == stockOutReturnDetail.ItemId).ToList();
				stockOutReturnDetail.Taxes = stockOutReturnDetail.ItemTaxData.ToJson();
			}
		}

		private void SerializeStockOutReturnDetails(List<StockOutReturnDetailDto> stockOutReturnDetails)
		{
			int newId = -1;
			int newSubId = -1;
			foreach (var stockOutReturnDetail in stockOutReturnDetails)
			{
				stockOutReturnDetail.StockOutReturnDetailId = newId;

				stockOutReturnDetail.StockOutReturnDetailTaxes.ForEach(y =>
				{
					y.StockOutReturnDetailId = newId;
					y.StockOutReturnDetailTaxId = newSubId--;
				});

				newId--;
			}
		}

		private async Task CalculateOtherTaxesFromStockOut(List<StockOutReturnDetailDto> stockOutReturnDetails, decimal headerDiscountPercent, int stockOutHeaderId)
		{
			var stockOutDetailTaxes = await _stockOutDetailTaxService.GetStockOutDetailTaxes(stockOutHeaderId).ToListAsync();
			foreach (var stockOutReturnDetail in stockOutReturnDetails)
			{
				stockOutReturnDetail.StockOutReturnDetailTaxes = (
					from itemTax in stockOutDetailTaxes.Where(x => x.StockOutDetailId == stockOutReturnDetail.StockOutReturnDetailId)
					select new StockOutReturnDetailTaxDto
					{
						TaxId = itemTax.TaxId,
						DebitAccountId = itemTax.CreditAccountId,
						TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
						TaxPercent = itemTax.TaxPercent,
						TaxValue = CalculateDetailValue.TaxValue(stockOutReturnDetail.Quantity, stockOutReturnDetail.SellingPrice,
							stockOutReturnDetail.ItemDiscountPercent, stockOutReturnDetail.VatPercent, itemTax.TaxPercent,
							itemTax.TaxAfterVatInclusive, headerDiscountPercent, stockOutReturnDetail.IsItemVatInclusive)
					}
				).ToList();

				stockOutReturnDetail.OtherTaxValue = stockOutReturnDetail.StockOutReturnDetailTaxes.Sum(x => x.TaxValue);
				stockOutReturnDetail.NetValue = CalculateDetailValue.NetValue(stockOutReturnDetail.Quantity,
					stockOutReturnDetail.SellingPrice, stockOutReturnDetail.ItemDiscountPercent, stockOutReturnDetail.VatPercent,
					stockOutReturnDetail.OtherTaxValue, headerDiscountPercent, stockOutReturnDetail.IsItemVatInclusive);
			}
		}

		private async Task CalculateOtherTaxesFromSalesInvoice(List<StockOutReturnDetailDto> stockOutReturnDetails, decimal headerDiscountPercent, int salesInvoiceHeaderId)
		{
			var salesInvoiceDetailTaxes = await _salesInvoiceDetailTaxService.GetSalesInvoiceDetailTaxes(salesInvoiceHeaderId).ToListAsync();
			foreach (var stockOutReturnDetail in stockOutReturnDetails)
			{
				stockOutReturnDetail.StockOutReturnDetailTaxes = (
					from itemTax in salesInvoiceDetailTaxes.Where(x => x.SalesInvoiceDetailId == stockOutReturnDetail.StockOutReturnDetailId)
					select new StockOutReturnDetailTaxDto
					{
						TaxId = itemTax.TaxId,
						DebitAccountId = itemTax.CreditAccountId,
						TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
						TaxPercent = itemTax.TaxPercent,
						TaxValue = CalculateDetailValue.TaxValue(stockOutReturnDetail.Quantity, stockOutReturnDetail.SellingPrice,
							stockOutReturnDetail.ItemDiscountPercent, stockOutReturnDetail.VatPercent, itemTax.TaxPercent,
							itemTax.TaxAfterVatInclusive, headerDiscountPercent, stockOutReturnDetail.IsItemVatInclusive)
					}
				).ToList();

				stockOutReturnDetail.OtherTaxValue = stockOutReturnDetail.StockOutReturnDetailTaxes.Sum(x => x.TaxValue);
				stockOutReturnDetail.NetValue = CalculateDetailValue.NetValue(stockOutReturnDetail.Quantity,
					stockOutReturnDetail.SellingPrice, stockOutReturnDetail.ItemDiscountPercent, stockOutReturnDetail.VatPercent,
					stockOutReturnDetail.OtherTaxValue, headerDiscountPercent, stockOutReturnDetail.IsItemVatInclusive);
			}
		}

		public async Task<List<StockOutReturnDetailDto>> GetStockOutReturnDetailsCalculated(int stockOutReturnHeaderId)
        {
			var stockOutReturnHeader = await _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeaderId).Select(x => new { x.StockOutHeaderId, x.SalesInvoiceHeaderId }).FirstOrDefaultAsync();

            return await GetStockOutReturnDetailsCalculatedInternal(stockOutReturnHeaderId, stockOutReturnHeader?.StockOutHeaderId, stockOutReturnHeader?.SalesInvoiceHeaderId);
        }

		public async Task<List<StockOutReturnDetailDto>> ModifyStockOutReturnDetailsWithLiveAvailableQuantity(int stockOutReturnHeaderId, int? stockOutHeaderId, int? salesInvoiceHeaderId, List<StockOutReturnDetailDto> stockOutReturnDetails)
		{
			return await GetStockOutReturnDetailsCalculatedInternal(stockOutReturnHeaderId, stockOutHeaderId, salesInvoiceHeaderId, stockOutReturnDetails);
		}

		private async Task<List<StockOutReturnDetailDto>> GetStockOutReturnDetailsCalculatedInternal(int stockOutReturnHeaderId, int? stockOutHeaderId, int? salesInvoiceHeaderId, List<StockOutReturnDetailDto>? stockOutReturnDetails = null)
		{
			List<StockOutReturnDetailDto> results = [];
			if (stockOutHeaderId != null)
			{
				results = await GetStockOutReturnFromStockOutDetailCalculated(stockOutReturnHeaderId, (int)stockOutHeaderId, stockOutReturnDetails);
			}
			else if (salesInvoiceHeaderId != null)
			{
				results = await GetStockOutReturnFromSalesInvoiceDetailCalculated(stockOutReturnHeaderId, (int)salesInvoiceHeaderId, stockOutReturnDetails);
			}

			await GetAuxiliaryData(results);
			return results;
		}

		private async Task<List<StockOutReturnDetailDto>> GetStockOutReturnFromStockOutDetailCalculated(int stockOutReturnHeaderId, int stockOutHeaderId, List<StockOutReturnDetailDto>? stockOutReturnDetails)
		{
			stockOutReturnDetails ??= await _stockOutReturnDetailService.GetStockOutReturnDetailsAsQueryable(stockOutReturnHeaderId).ToListAsync();
			var itemIds = stockOutReturnDetails.Select(x => x.ItemId).ToList();

			var stockOutDetailsGrouped = await _stockOutDetailService.GetStockOutDetailsGroupedWithAllKeys(stockOutHeaderId);
			var stocksReturned = await _stockOutQuantityService.GetStocksDisbursedReturnedFromStockOutExceptStockOutReturnHeaderId(stockOutHeaderId, stockOutReturnHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

			return (from stockOutReturnDetail in stockOutReturnDetails
					from stockOutDetailGroup in stockOutDetailsGrouped.Where(x => x.ItemId == stockOutReturnDetail.ItemId && x.ItemPackageId == stockOutReturnDetail.ItemPackageId && x.CostCenterId == stockOutReturnDetail.CostCenterId && x.BarCode == stockOutReturnDetail.BarCode && x.SellingPrice == stockOutReturnDetail.SellingPrice && x.BatchNumber == stockOutReturnDetail.BatchNumber && x.ExpireDate == stockOutReturnDetail.ExpireDate && x.ItemDiscountPercent == stockOutReturnDetail.ItemDiscountPercent).DefaultIfEmpty()
					from stockReturned   in stocksReturned.Where(x => x.ItemId == stockOutReturnDetail.ItemId && x.ItemPackageId == stockOutReturnDetail.ItemPackageId && x.CostCenterId == stockOutReturnDetail.CostCenterId && x.BarCode == stockOutReturnDetail.BarCode && x.SellingPrice == stockOutReturnDetail.SellingPrice && x.BatchNumber == stockOutReturnDetail.BatchNumber && x.ExpireDate == stockOutReturnDetail.ExpireDate && x.ItemDiscountPercent == stockOutReturnDetail.ItemDiscountPercent).DefaultIfEmpty()
					select new StockOutReturnDetailDto
					{
						StockOutReturnDetailId = stockOutReturnDetail.StockOutReturnDetailId,
						StockOutReturnHeaderId = stockOutReturnDetail.StockOutReturnHeaderId,
						CostCenterId = stockOutReturnDetail.CostCenterId,
						CostCenterName = stockOutReturnDetail.CostCenterName,
						ItemId = stockOutReturnDetail.ItemId,
						ItemCode = stockOutReturnDetail.ItemCode,
						ItemName = stockOutReturnDetail.ItemName,
						TaxTypeId = stockOutReturnDetail.TaxTypeId,
						ItemTypeId = stockOutReturnDetail.ItemTypeId,
						ItemPackageId = stockOutReturnDetail.ItemPackageId,
						ItemPackageName = stockOutReturnDetail.ItemPackageName,
						IsItemVatInclusive = stockOutReturnDetail.IsItemVatInclusive,
						BarCode = stockOutReturnDetail.BarCode,
						Packing = stockOutReturnDetail.Packing,
						ExpireDate = stockOutReturnDetail.ExpireDate,
						BatchNumber = stockOutReturnDetail.BatchNumber,
						Quantity = stockOutReturnDetail.Quantity,
						BonusQuantity = stockOutReturnDetail.BonusQuantity,
						AvailableQuantity = (stockOutDetailGroup != null ? stockOutDetailGroup.Quantity : 0) - (stockReturned != null ? stockReturned.QuantityReturned : 0),
						AvailableBonusQuantity = (stockOutDetailGroup != null ? stockOutDetailGroup.BonusQuantity : 0) - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0),
						StockOutQuantity = (stockOutDetailGroup != null ? stockOutDetailGroup.Quantity : 0),
						StockOutBonusQuantity = (stockOutDetailGroup != null ? stockOutDetailGroup.BonusQuantity : 0),
						SellingPrice = stockOutReturnDetail.SellingPrice,
						TotalValue = stockOutReturnDetail.TotalValue,
						ItemDiscountPercent = stockOutReturnDetail.ItemDiscountPercent,
						ItemDiscountValue = stockOutReturnDetail.ItemDiscountValue,
						TotalValueAfterDiscount = stockOutReturnDetail.TotalValueAfterDiscount,
						HeaderDiscountValue = stockOutReturnDetail.HeaderDiscountValue,
						GrossValue = stockOutReturnDetail.GrossValue,
						VatPercent = stockOutReturnDetail.VatPercent,
						VatValue = stockOutReturnDetail.VatValue,
						SubNetValue = stockOutReturnDetail.SubNetValue,
						OtherTaxValue = stockOutReturnDetail.OtherTaxValue,
						NetValue = stockOutReturnDetail.NetValue,
						Notes = stockOutReturnDetail.Notes,
						ItemNote = stockOutReturnDetail.ItemNote,
						ConsumerPrice = stockOutReturnDetail.ConsumerPrice,
						CostPrice = stockOutReturnDetail.CostPrice,
						CostPackage = stockOutReturnDetail.CostPackage,
						CostValue = stockOutReturnDetail.CostValue,
						LastSalesPrice = stockOutReturnDetail.LastSalesPrice,

						StockOutReturnDetailTaxes = stockOutReturnDetail.StockOutReturnDetailTaxes,

						CreatedAt = stockOutReturnDetail.CreatedAt,
						IpAddressCreated = stockOutReturnDetail.IpAddressCreated,
						UserNameCreated = stockOutReturnDetail.UserNameCreated,
					}).ToList();
		}

		private async Task<List<StockOutReturnDetailDto>> GetStockOutReturnFromSalesInvoiceDetailCalculated(int stockOutReturnHeaderId, int salesInvoiceHeaderId, List<StockOutReturnDetailDto>? stockOutReturnDetails)
		{
			stockOutReturnDetails ??= await _stockOutReturnDetailService.GetStockOutReturnDetailsAsQueryable(stockOutReturnHeaderId).ToListAsync();
			var itemIds = stockOutReturnDetails.Select(x => x.ItemId).ToList();

			var salesInvoiceDetailsGrouped = await _salesInvoiceDetailService.GetSalesInvoiceDetailsGrouped(salesInvoiceHeaderId);
			var stocksReturned = await _stockOutQuantityService.GetStocksDisbursedReturnedFromSalesInvoiceExceptStockOutReturnHeaderId(salesInvoiceHeaderId, stockOutReturnHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var reservationInvoiceCloseOutDetails = await GetReservationInvoiceCloseOutQuantities(salesInvoiceHeaderId);

			return (from stockOutReturnDetail in stockOutReturnDetails
					from salesInvoiceDetailGroup in salesInvoiceDetailsGrouped.Where(x => x.ItemId == stockOutReturnDetail.ItemId && x.ItemPackageId == stockOutReturnDetail.ItemPackageId && x.CostCenterId == stockOutReturnDetail.CostCenterId && x.BarCode == stockOutReturnDetail.BarCode && x.SellingPrice == stockOutReturnDetail.SellingPrice && x.BatchNumber == stockOutReturnDetail.BatchNumber && x.ExpireDate == stockOutReturnDetail.ExpireDate && x.ItemDiscountPercent == stockOutReturnDetail.ItemDiscountPercent).DefaultIfEmpty()
					from stockReturned in stocksReturned.Where(x => x.ItemId == stockOutReturnDetail.ItemId && x.ItemPackageId == stockOutReturnDetail.ItemPackageId && x.CostCenterId == stockOutReturnDetail.CostCenterId && x.BarCode == stockOutReturnDetail.BarCode && x.SellingPrice == stockOutReturnDetail.SellingPrice && x.BatchNumber == stockOutReturnDetail.BatchNumber && x.ExpireDate == stockOutReturnDetail.ExpireDate && x.ItemDiscountPercent == stockOutReturnDetail.ItemDiscountPercent).DefaultIfEmpty()
					from reservationInvoiceCloseOutDetail in reservationInvoiceCloseOutDetails.Where(x => x.ItemId == stockOutReturnDetail.ItemId && x.ItemPackageId == stockOutReturnDetail.ItemPackageId && x.CostCenterId == stockOutReturnDetail.CostCenterId && x.BarCode == stockOutReturnDetail.BarCode && x.SellingPrice == stockOutReturnDetail.SellingPrice && x.BatchNumber == stockOutReturnDetail.BatchNumber && x.ExpireDate == stockOutReturnDetail.ExpireDate && x.ItemDiscountPercent == stockOutReturnDetail.ItemDiscountPercent).DefaultIfEmpty()
					select new StockOutReturnDetailDto
					{
						StockOutReturnDetailId = stockOutReturnDetail.StockOutReturnDetailId,
						StockOutReturnHeaderId = stockOutReturnDetail.StockOutReturnHeaderId,
						CostCenterId = stockOutReturnDetail.CostCenterId,
						CostCenterName = stockOutReturnDetail.CostCenterName,
						ItemId = stockOutReturnDetail.ItemId,
						ItemCode = stockOutReturnDetail.ItemCode,
						ItemName = stockOutReturnDetail.ItemName,
						TaxTypeId = stockOutReturnDetail.TaxTypeId,
						ItemTypeId = stockOutReturnDetail.ItemTypeId,
						ItemPackageId = stockOutReturnDetail.ItemPackageId,
						ItemPackageName = stockOutReturnDetail.ItemPackageName,
						IsItemVatInclusive = stockOutReturnDetail.IsItemVatInclusive,
						BarCode = stockOutReturnDetail.BarCode,
						Packing = stockOutReturnDetail.Packing,
						ExpireDate = stockOutReturnDetail.ExpireDate,
						BatchNumber = stockOutReturnDetail.BatchNumber,
						Quantity = stockOutReturnDetail.Quantity,
						BonusQuantity = stockOutReturnDetail.BonusQuantity,
						AvailableQuantity = (salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.Quantity : 0) - (reservationInvoiceCloseOutDetail != null ? reservationInvoiceCloseOutDetail.Quantity : 0) - (stockReturned != null ? stockReturned.QuantityReturned : 0),
						AvailableBonusQuantity = (salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.BonusQuantity : 0) - (reservationInvoiceCloseOutDetail != null ? reservationInvoiceCloseOutDetail.BonusQuantity : 0) - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0),
						SalesInvoiceQuantity = (salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.Quantity : 0),
						SalesInvoiceBonusQuantity = (salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.BonusQuantity : 0),
						SellingPrice = stockOutReturnDetail.SellingPrice,
						TotalValue = stockOutReturnDetail.TotalValue,
						ItemDiscountPercent = stockOutReturnDetail.ItemDiscountPercent,
						ItemDiscountValue = stockOutReturnDetail.ItemDiscountValue,
						TotalValueAfterDiscount = stockOutReturnDetail.TotalValueAfterDiscount,
						HeaderDiscountValue = stockOutReturnDetail.HeaderDiscountValue,
						GrossValue = stockOutReturnDetail.GrossValue,
						VatPercent = stockOutReturnDetail.VatPercent,
						VatValue = stockOutReturnDetail.VatValue,
						SubNetValue = stockOutReturnDetail.SubNetValue,
						OtherTaxValue = stockOutReturnDetail.OtherTaxValue,
						NetValue = stockOutReturnDetail.NetValue,
						Notes = stockOutReturnDetail.Notes,
						ItemNote = stockOutReturnDetail.ItemNote,
						ConsumerPrice = stockOutReturnDetail.ConsumerPrice,
						CostPrice = stockOutReturnDetail.CostPrice,
						CostPackage = stockOutReturnDetail.CostPackage,
						CostValue = stockOutReturnDetail.CostValue,
						LastSalesPrice = stockOutReturnDetail.LastSalesPrice,

						StockOutReturnDetailTaxes = stockOutReturnDetail.StockOutReturnDetailTaxes,

						CreatedAt = stockOutReturnDetail.CreatedAt,
						IpAddressCreated = stockOutReturnDetail.IpAddressCreated,
						UserNameCreated = stockOutReturnDetail.UserNameCreated,
					}).ToList();
		}

		public async Task<int> GetParentMenuCode(int? stockOutHeaderId, int? salesInvoiceHeaderId)
		{
			if (stockOutHeaderId != null)
			{
				var stockOutHeader = await _stockOutHeaderService.GetStockOutHeaderById((int)stockOutHeaderId);
				return StockTypeData.ToMenuCode(stockOutHeader!.StockTypeId);
			}
			else
			{
				var salseInvoiceHeader = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById((int)salesInvoiceHeaderId!);
				return SalesInvoiceMenuCodeHelper.GetMenuCode(salseInvoiceHeader!);
			}
		}

		public async Task<int?> GetGrandParentMenuCode(int? stockOutHeaderId)
		{
			if (stockOutHeaderId == null) return null;

			var stockOutHeader = await _stockOutHeaderService.GetStockOutHeaderById((int)stockOutHeaderId);
			if (stockOutHeader!.ProformaInvoiceHeaderId != null)
			{
				return MenuCodeData.ProformaInvoice;
			}
			else
			{
				var salesInvoiceHeader = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById((int)stockOutHeader.SalesInvoiceHeaderId!);
				return SalesInvoiceMenuCodeHelper.GetMenuCode(salesInvoiceHeader!);
			}
		}

		public async Task<ResponseDto> SaveStockOutReturn(StockOutReturnDto stockOutReturn, bool hasApprove, bool approved, int? requestId, bool affectBalances = true, string? documentReference = null, bool shouldInitializeFlags = false)
        {
            TrimDetailStrings(stockOutReturn.StockOutReturnDetails);

			var menuCode = StockTypeData.ToMenuCode(stockOutReturn.StockOutReturnHeader!.StockTypeId);
			var parentMenuCode = await GetParentMenuCode(stockOutReturn.StockOutReturnHeader!.StockOutHeaderId, stockOutReturn.StockOutReturnHeader!.SalesInvoiceHeaderId);
			var grandParentMenuCode = await GetGrandParentMenuCode(stockOutReturn.StockOutReturnHeader!.StockOutHeaderId);
			var validationResult = await CheckStockOutReturnIsValidForSave(stockOutReturn, menuCode, parentMenuCode, grandParentMenuCode);
			if (validationResult.Success == false) return validationResult;

            if (stockOutReturn.StockOutReturnHeader!.StockOutReturnHeaderId == 0)
            {
                await UpdateModelPrices(stockOutReturn);
			}

			var result = affectBalances ? 
				await _stockOutReturnService.SaveStockOutReturn(stockOutReturn, hasApprove, approved, requestId, documentReference, shouldInitializeFlags) :
				await _stockOutReturnService.SaveStockOutReturnWithoutUpdatingBalances(stockOutReturn, hasApprove, approved, requestId, documentReference, shouldInitializeFlags);
			if (result.Success)
			{
				await ApplyStockOutReturnSideEffects(stockOutReturn.StockOutReturnHeader.StockTypeId, stockOutReturn.StockOutReturnHeader.StockOutHeaderId, stockOutReturn.StockOutReturnHeader.SalesInvoiceHeaderId);
			}

			return result;
        }

		private async Task ApplyStockOutReturnSideEffects(byte stockTypeId, int? stockOutHeaderId, int? salesInvoiceHeaderId)
		{
			if(stockTypeId == StockTypeData.StockOutReturnFromSalesInvoice)
			{
				await _salesInvoiceHeaderService.UpdateClosed(salesInvoiceHeaderId, true);
			}
			else
			{
				await _stockOutHeaderService.UpdateClosed(stockOutHeaderId, true);
			}

			await UpdateProformaInvoiceStatusFromStockOutReturn(stockOutHeaderId, salesInvoiceHeaderId);
			await UpdateReservationInvoiceCloseOutEnded(salesInvoiceHeaderId, true);
		}

		private async Task UpdateProformaInvoiceStatusFromStockOutReturn(int? stockOutHeaderId, int? salesInvoiceHeaderId)
		{
			var proformaInvoiceHeaderId = await GetProformaInvoiceRelatedToStockOutReturn(stockOutHeaderId, salesInvoiceHeaderId);
			await _proformaInvoiceStatusService.UpdateProformaInvoiceStatus(proformaInvoiceHeaderId, -1);
		}

		private async Task<int> GetProformaInvoiceRelatedToStockOutReturn(int? stockOutHeaderId, int? salesInvoiceHeaderId)
		{
			if (stockOutHeaderId != null)
			{
				var stockOutHeader = await _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId).Select(x => new { x.ProformaInvoiceHeaderId, x.SalesInvoiceHeaderId }).FirstOrDefaultAsync();
				return await GetProformaInvoiceRelatedToStockOut(stockOutHeader!.ProformaInvoiceHeaderId, stockOutHeader!.SalesInvoiceHeaderId);
			}
			else
			{
				return await _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.ProformaInvoiceHeaderId).FirstOrDefaultAsync();
			}
		}

		private async Task<int> GetProformaInvoiceRelatedToStockOut(int? proformaInvoiceHeaderId, int? salesInvoiceHeaderId)
		{
			if (proformaInvoiceHeaderId != null) return (int)proformaInvoiceHeaderId;

			return await _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.ProformaInvoiceHeaderId).FirstOrDefaultAsync();
		}

		private async Task UpdateReservationInvoiceCloseOutEnded(int? salesInvoiceHeaderId, bool isEnded)
		{
			if (salesInvoiceHeaderId != null)
			{
				await _salesInvoiceReturnHeaderService.UpdateReservationInvoiceCloseOutEndedLinkedToSalesInvoice(salesInvoiceHeaderId, isEnded);
			}
		}

		public async Task<ResponseDto> CheckStockOutReturnIsValidForSave(StockOutReturnDto stockOutReturn, int menuCode, int parentMenuCode, int? grandParentMenuCode)
		{
			ResponseDto result;

			if (stockOutReturn.StockOutReturnHeader?.StockOutReturnHeaderId != 0)
			{
				result = await CheckStockOutReturnClosed(stockOutReturn.StockOutReturnHeader!.StockOutReturnHeaderId, menuCode);
				if(result.Success == false) return result;
			}

			result = await CheckStockOutReturnEnded(stockOutReturn.StockOutReturnHeader!.StockOutReturnHeaderId, stockOutReturn.StockOutReturnHeader!.StockOutHeaderId, stockOutReturn.StockOutReturnHeader!.SalesInvoiceHeaderId, menuCode, parentMenuCode);
			if (result.Success == false) return result;

			result = await CheckStockOutOrSalesInvoiceBlocked(stockOutReturn.StockOutReturnHeader!.StockOutHeaderId, stockOutReturn.StockOutReturnHeader!.SalesInvoiceHeaderId, menuCode);
			if (result.Success == false) return result;

			result = await CheckSalesInvoiceCanReturn(stockOutReturn.StockOutReturnHeader!.SalesInvoiceHeaderId, parentMenuCode, stockOutReturn.StockOutReturnHeader.StockOutReturnHeaderId == 0);
			if (result.Success == false) return result;

			result = await CheckStockOutReturnFromReservationInvoiceCreatedBeforeInvoiceCloseOut(stockOutReturn.StockOutReturnHeader!.SalesInvoiceHeaderId, menuCode, parentMenuCode);
			if (result.Success == false) return result;

			result = await CheckStockOutReturnCauseInvoiceToBecomeLessThanSettledValue(stockOutReturn.StockOutReturnHeader!.NetValue, stockOutReturn.StockOutReturnHeader!.SalesInvoiceHeaderId, menuCode, parentMenuCode);
			if (result.Success == false) return result;

			result = await _itemNoteValidationService.CheckItemNoteWithItemType(stockOutReturn.StockOutReturnDetails, x => x.ItemId, x => x.ItemNote);
			if (result.Success == false) return result;

			result = await CheckStockOutReturnQuantityForSaving(stockOutReturn, menuCode, parentMenuCode, grandParentMenuCode);
			if (result.Success == false) return result;

			result = await CheckStockOutReturnZeroStock(stockOutReturn.StockOutReturnHeader.StockOutReturnHeaderId, stockOutReturn.StockOutReturnHeader.StoreId, stockOutReturn.StockOutReturnDetails, menuCode, parentMenuCode, grandParentMenuCode, true);
			if (result.Success == false) return result;

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutReturnCauseInvoiceToBecomeLessThanSettledValue(decimal stockOutReturnNetValue, int? salesInvoiceHeaderId, int menuCode, int parentMenuCode)
		{
			if (salesInvoiceHeaderId != null)
			{
				var invoiceValue = await _salesValueService.GetOverallValueOfSalesInvoice((int)salesInvoiceHeaderId);
				var settledValue = await _getSalesInvoiceSettleValueService.GetSalesInvoiceSettleValue((int)salesInvoiceHeaderId);

				if (invoiceValue - stockOutReturnNetValue < settledValue)
				{
					return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, parentMenuCode, GenericMessageData.CannotSaveBecauseSettled) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutReturnClosed(int stockOutReturnHeaderId, int menuCode)
		{
			var isClosed = await _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeaderId).Select(x => (bool?)x.IsClosed).FirstOrDefaultAsync();
			if (isClosed == null)
			{
				return new ResponseDto { Id = stockOutReturnHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };
			}

			if (isClosed == true)
			{
				return new ResponseDto { Id = stockOutReturnHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotUpdateBecauseClosed) };
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutReturnEnded(int stockOutReturnHeaderId, int? stockOutHeaderId, int? salesInvoiceHeaderId, int menuCode, int parentMenuCode)
		{
			if (stockOutHeaderId != null)
			{
				return await CheckStockOutReturnFromStockOutEnded((int)stockOutHeaderId!, menuCode);
			}
			else
			{
				return await CheckStockOutReturnFromSalesInvoiceEnded(stockOutReturnHeaderId, (int)salesInvoiceHeaderId!, menuCode, parentMenuCode);
			}
		}

		private async Task<ResponseDto> CheckStockOutReturnFromStockOutEnded(int stockOutHeaderId, int menuCode)
		{
			var stockOutEnded = await (
				from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId).DefaultIfEmpty()
				select (stockOutHeader != null && stockOutHeader.IsEnded)
			).FirstOrDefaultAsync();

			if (stockOutEnded)
			{
				return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseInvoiced) };
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutReturnFromSalesInvoiceEnded(int stockOutReturnHeaderId, int salesInvoiceHeaderId, int menuCode, int parentMenuCode)
		{ 
			if (stockOutReturnHeaderId != 0)
			{
				var isEnded = await _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeaderId).Select(x => x.IsEnded).FirstOrDefaultAsync();

				if (isEnded)
				{
					return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseInvoiced) };
				}
			}
			else
			{
				var hasClientDebitOrCreditMemo = await CheckSalesInvoiceHasClientCreditOrDebitMemo(salesInvoiceHeaderId, menuCode, parentMenuCode);
				if (hasClientDebitOrCreditMemo.Success == false) return hasClientDebitOrCreditMemo;
			}


			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckSalesInvoiceHasClientCreditOrDebitMemo(int salesInvoiceHeaderId, int menuCode, int parentMenuCode)
		{
			var clientCredits = _clientCreditMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => MenuCodeData.ClientCreditMemo);
			var clientDebits = _clientDebitMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => MenuCodeData.SupplierDebitMemo);

			var creditOrDebitMenuCode = await clientCredits.Concat(clientDebits).FirstOrDefaultAsync();

			if (creditOrDebitMenuCode != 0)
			{
				return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, creditOrDebitMenuCode, parentMenuCode, GenericMessageData.HasDocument) };
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutOrSalesInvoiceBlocked(int? stockOutHeaderId, int? salesInvoiceHeaderId, int menuCode)
		{
			var isBlocked = await (
				from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId).DefaultIfEmpty()
				from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).DefaultIfEmpty()
				select (stockOutHeader != null && stockOutHeader.IsBlocked) || (salesInvoiceHeader != null && salesInvoiceHeader.IsBlocked)
			).FirstOrDefaultAsync();

			if (isBlocked)
			{
				return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn)};
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutReturnFromReservationInvoiceCreatedBeforeInvoiceCloseOut(int? salesInvoiceHeaderId, int menuCode, int parentMenuCode)
		{
			if (salesInvoiceHeaderId != null)
			{
				var hasReservationInvoiceCloseOut = await _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId && x.IsOnTheWay).AnyAsync();
				var fromSalesInvoiceOnTheWay = await _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.IsOnTheWay).FirstOrDefaultAsync();

				if (fromSalesInvoiceOnTheWay && !hasReservationInvoiceCloseOut)
				{
					return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, MenuCodeData.ReservationInvoiceCloseOut, SalesMessageData.StockOutReturnFromReservationInvoiceCreatedBeforeCloseOut) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutReturnQuantityForSaving(StockOutReturnDto stockOutReturn, int menuCode, int parentMenuCode, int? grandParentMenuCode)
		{
			ResponseDto result;
			if (stockOutReturn.StockOutReturnHeader!.StockOutHeaderId != null)
			{
				result = await CheckStockOutReturnFromStockOutQuantityExceeding(stockOutReturn, menuCode, parentMenuCode, (int)grandParentMenuCode!);
			}
			else
			{
				result = await CheckStockOutReturnFromSalesInvoiceQuantityExceeding(stockOutReturn, menuCode, parentMenuCode);
			}

			return result;
		}

		private async Task<ResponseDto> CheckStockOutReturnZeroStock(int stockOutReturnHeaderId, int storeId, List<StockOutReturnDetailDto> returnDetails, int menuCode, int parentMenuCode, int? grandParentMenuCode, bool isSave)
		{
			var oldReturnDetails = await _stockOutReturnDetailService.GetStockOutReturnDetailsAsQueryable(stockOutReturnHeaderId).ToListAsync();

			var storeIdToCheck = menuCode == MenuCodeData.StockOutReturnFromReservation ? await _storeService.GetReservedStoreByParentStoreId(storeId) : storeId;
			var settingMenuCode = menuCode == MenuCodeData.StockOutReturnFromReservation ? (int)grandParentMenuCode! : parentMenuCode;

			return await _zeroStockValidationService.ValidateZeroStockReturn(
				storeId: storeIdToCheck,
				newDetails: returnDetails,
				oldDetails: oldReturnDetails,
				detailKeySelector: x => (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber),
				itemIdSelector: x => x.ItemId,
				quantitySelector: x => x.Quantity + x.BonusQuantity,
				availableBalanceKeySelector: x => (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber),
				isGrouped: false,
				menuCode: menuCode,
				settingMenuCode: settingMenuCode,
				isSave: isSave);
		}

		private async Task<ResponseDto> CheckSalesInvoiceCanReturn(int? salesInvoiceHeaderId, int parentMenuCode, bool isCreate)
		{
			if (isCreate && salesInvoiceHeaderId != null)
			{
				var canReturnUntil = (await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById((int)salesInvoiceHeaderId))!.CanReturnUntilDate;

				if (canReturnUntil.Date < DateHelper.GetDateTimeNow().Date)
				{
					return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(parentMenuCode, SalesMessageData.NoLongerCanReturn) };
				}
			}
			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutReturnFromStockOutQuantityExceeding(StockOutReturnDto stockOutReturn, int menuCode, int parentMenuCode, int grandParentMenuCode)
		{
			if (stockOutReturn.StockOutReturnHeader == null || !(stockOutReturn.StockOutReturnHeader.StockOutHeaderId > 0)) return new ResponseDto { Message = "StockType mismatches parent foreign keys" };

			var stockOutReturnDetailsGrouped = _stockOutReturnDetailService.GroupStockOutReturnDetailsWithAllKeys(stockOutReturn.StockOutReturnDetails);
			var itemIds = stockOutReturn.StockOutReturnDetails.Select(x => x.ItemId).ToList();

			var result = await CheckStockOutReturnExceedingStockOutItselfForSave(stockOutReturn.StockOutReturnHeader.StockOutReturnHeaderId, stockOutReturnDetailsGrouped, (int)stockOutReturn.StockOutReturnHeader.StockOutHeaderId, itemIds, menuCode, parentMenuCode);
			if (result.Success == false) return result;

			if (stockOutReturn.StockOutReturnHeader.StockOutReturnHeaderId != 0)
			{
				var stockOutReturnHeader = await _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutReturn.StockOutReturnHeader.StockOutHeaderId).Select(x => new { x.SalesInvoiceHeaderId, x.ProformaInvoiceHeaderId }).FirstOrDefaultAsync();
				var salesInvoiceHeaderId = stockOutReturnHeader?.SalesInvoiceHeaderId;
				var proformaInvoiceHeaderId = stockOutReturnHeader?.ProformaInvoiceHeaderId;

				if (proformaInvoiceHeaderId != null)
				{
					result = await CheckStockOutReturnFromStockOutFromProformaInvoiceQuantityForSave(stockOutReturn.StockOutReturnHeader.StockOutReturnHeaderId, stockOutReturnDetailsGrouped, itemIds, (int)proformaInvoiceHeaderId, menuCode, grandParentMenuCode);
				}
				else if (salesInvoiceHeaderId != null)
				{
					result = await CheckStockOutReturnFromStockOutFromSalesInvoiceQuantityForSave(stockOutReturn.StockOutReturnHeader.StockOutReturnHeaderId, stockOutReturnDetailsGrouped, itemIds, (int)salesInvoiceHeaderId, menuCode, grandParentMenuCode);
				}
			}

			return result;
		}

		private async Task<ResponseDto> CheckStockOutReturnExceedingStockOutItselfForSave(int stockOutReturnHeaderId, List<StockOutReturnDetailDto> stockOutReturnDetailsGrouped, int stockOutHeaderId, List<int> itemIds, int menuCode, int parentMenuCode)
		{
			List<StockDisbursedReturnedDto>? stocksReturned;
			if (stockOutReturnHeaderId == 0)
			{
				stocksReturned = await _stockOutQuantityService.GetStocksDisbursedReturnedFromStockOut(stockOutHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			}
			else
			{
				stocksReturned = await _stockOutQuantityService.GetStocksDisbursedReturnedFromStockOutExceptStockOutReturnHeaderId(stockOutHeaderId, stockOutReturnHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			}

			var stockOutDetailsGrouped = await _stockOutDetailService.GetStockOutDetailsGroupedWithAllKeys(stockOutHeaderId);

			var AvailableReturnQuantities = (
				from stockOutReturnDetailGroup in stockOutReturnDetailsGrouped
				from stockReturned               in stocksReturned.Where(x => x.ItemId == stockOutReturnDetailGroup.ItemId && x.ItemPackageId == stockOutReturnDetailGroup.ItemPackageId && x.BatchNumber == stockOutReturnDetailGroup.BatchNumber && x.ExpireDate == stockOutReturnDetailGroup.ExpireDate && x.CostCenterId == stockOutReturnDetailGroup.CostCenterId && x.BarCode == stockOutReturnDetailGroup.BarCode && x.SellingPrice == stockOutReturnDetailGroup.SellingPrice && x.ItemDiscountPercent == stockOutReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
				from stockOutDetailGroup in stockOutDetailsGrouped.Where(x => x.ItemId == stockOutReturnDetailGroup.ItemId && x.ItemPackageId == stockOutReturnDetailGroup.ItemPackageId && x.BatchNumber == stockOutReturnDetailGroup.BatchNumber && x.ExpireDate == stockOutReturnDetailGroup.ExpireDate && x.CostCenterId == stockOutReturnDetailGroup.CostCenterId && x.BarCode == stockOutReturnDetailGroup.BarCode && x.SellingPrice == stockOutReturnDetailGroup.SellingPrice && x.ItemDiscountPercent == stockOutReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
				select new
				{
					stockOutReturnDetailGroup.ItemId,
					stockOutReturnDetailGroup.ItemPackageId,
					StockOutQuantity = stockOutDetailGroup != null ? stockOutDetailGroup.Quantity : 0,
					StockOutBonusQuantity = stockOutDetailGroup != null ? stockOutDetailGroup.BonusQuantity : 0,
					QuantityReturned = (stockReturned != null ? stockReturned.QuantityReturned : 0) + stockOutReturnDetailGroup.Quantity,
					BonusQuantityReturned = (stockReturned != null ? stockReturned.BonusQuantityReturned : 0) + stockOutReturnDetailGroup.BonusQuantity,
					QuantityReturnAvailable = (stockOutDetailGroup != null ? stockOutDetailGroup.Quantity : 0) - (stockReturned != null ? stockReturned.QuantityReturned : 0) - stockOutReturnDetailGroup.Quantity,
					BonusQuantityReturnAvailable = (stockOutDetailGroup != null ? stockOutDetailGroup.BonusQuantity : 0) - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0) - stockOutReturnDetailGroup.BonusQuantity
				});

			var exceedingItem = AvailableReturnQuantities.FirstOrDefault(x => x.QuantityReturnAvailable < 0 || x.BonusQuantityReturnAvailable < 0);
			if (exceedingItem != null)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();

				var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
				var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

				if (exceedingItem.QuantityReturnAvailable < 0)
				{
					return new ResponseDto { Id = stockOutReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.QuantityExceeding, itemName!, itemPackageName!, exceedingItem.QuantityReturned.ToNormalizedString(), exceedingItem.StockOutQuantity.ToNormalizedString()) };
				}
				else
				{
					return new ResponseDto { Id = stockOutReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.BonusQuantityExceeding, itemName!, itemPackageName!, exceedingItem.BonusQuantityReturned.ToNormalizedString(), exceedingItem.StockOutBonusQuantity.ToNormalizedString()) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutReturnFromStockOutFromProformaInvoiceQuantityForSave(int stockOutReturnHeaderId, List<StockOutReturnDetailDto> stockOutReturnDetailsGrouped, List<int> itemIds, int proformaInvoiceHeaderId, int menuCode, int grandParentMenuCode)
		{
			var finalStocksDisbursed = await _stockOutQuantityService.GetFinalStocksDisbursedFromProformaInvoiceExceptStockOutReturnHeaderId(proformaInvoiceHeaderId, stockOutReturnHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

			var proformaInvoiceDetailsGrouped = await _proformaInvoiceDetailService.GetProformaInvoiceDetailsGrouped(proformaInvoiceHeaderId);

			var stockOutReturnDetailsGroupedWithLessKeys = _stockOutReturnDetailService.GroupStockOutReturnDetails(stockOutReturnDetailsGrouped);

			var availableReturnQuantities = (
				from stockOutReturnDetailGroup in stockOutReturnDetailsGroupedWithLessKeys
				from finalstockDisbursed       in           finalStocksDisbursed.Where(x => x.ItemId == stockOutReturnDetailGroup.ItemId && x.ItemPackageId == stockOutReturnDetailGroup.ItemPackageId && x.BarCode == stockOutReturnDetailGroup.BarCode && x.SellingPrice == stockOutReturnDetailGroup.SellingPrice && x.ItemDiscountPercent == stockOutReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
				from proformaInvoiceDetailGroup in proformaInvoiceDetailsGrouped.Where(x => x.ItemId == stockOutReturnDetailGroup.ItemId && x.ItemPackageId == stockOutReturnDetailGroup.ItemPackageId && x.BarCode == stockOutReturnDetailGroup.BarCode && x.SellingPrice == stockOutReturnDetailGroup.SellingPrice && x.ItemDiscountPercent == stockOutReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
				select new
				{
					stockOutReturnDetailGroup.ItemId,
					stockOutReturnDetailGroup.ItemPackageId,
					ProformaInvoiceQuantity = proformaInvoiceDetailGroup.Quantity,
					ProformaInvoiceBonusQuantity = proformaInvoiceDetailGroup.BonusQuantity,
					QuantityReturnAvailable = (finalstockDisbursed != null ? finalstockDisbursed.QuantityDisbursed : 0) - stockOutReturnDetailGroup.Quantity,
					BonusQuantityReturnAvailable = (finalstockDisbursed != null ? finalstockDisbursed.BonusQuantityDisbursed : 0) - stockOutReturnDetailGroup.BonusQuantity
				});

			var exceedingItem = availableReturnQuantities.FirstOrDefault(x => x.QuantityReturnAvailable > x.ProformaInvoiceQuantity || x.BonusQuantityReturnAvailable > x.ProformaInvoiceBonusQuantity);
			if (exceedingItem != null)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();

				var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
				var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

				if (exceedingItem.QuantityReturnAvailable > exceedingItem.ProformaInvoiceQuantity)
				{
					return new ResponseDto { Id = stockOutReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, grandParentMenuCode, SalesMessageData.SaveCauseQuantityExceed, itemName!, itemPackageName!, exceedingItem.QuantityReturnAvailable.ToNormalizedString(), exceedingItem.ProformaInvoiceQuantity.ToNormalizedString()) };
				}
				else
				{
					return new ResponseDto { Id = stockOutReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, grandParentMenuCode, SalesMessageData.SaveCauseBonusQuantityExceed, itemName!, itemPackageName!, exceedingItem.BonusQuantityReturnAvailable.ToNormalizedString(), exceedingItem.ProformaInvoiceBonusQuantity.ToNormalizedString()) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutReturnFromStockOutFromSalesInvoiceQuantityForSave(int stockOutReturnHeaderId, List<StockOutReturnDetailDto> stockOutReturnDetailsGrouped, List<int> itemIds, int salesInvoiceHeaderId, int menuCode, int grandParentMenuCode)
		{
			var finalStocksDisbursed = await _stockOutQuantityService.GetFinalStocksDisbursedFromSalesInvoiceExceptStockOutReturnHeaderId(salesInvoiceHeaderId, stockOutReturnHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

			var salesInvoiceDetailsGrouped = await _salesInvoiceDetailService.GetSalesInvoiceDetailsGrouped(salesInvoiceHeaderId);

			var availableReturnQuantities = (
				from stockOutReturnDetailGroup in stockOutReturnDetailsGrouped
				from finalstockDisbursed in           finalStocksDisbursed.Where(x => x.ItemId == stockOutReturnDetailGroup.ItemId && x.ItemPackageId == stockOutReturnDetailGroup.ItemPackageId && x.CostCenterId == stockOutReturnDetailGroup.CostCenterId && x.BarCode == stockOutReturnDetailGroup.BarCode && x.SellingPrice == stockOutReturnDetailGroup.SellingPrice && x.ExpireDate == stockOutReturnDetailGroup.ExpireDate && x.BatchNumber == stockOutReturnDetailGroup.BatchNumber && x.ItemDiscountPercent == stockOutReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
				from salesInvoiceDetailGroup in salesInvoiceDetailsGrouped.Where(x => x.ItemId == stockOutReturnDetailGroup.ItemId && x.ItemPackageId == stockOutReturnDetailGroup.ItemPackageId && x.CostCenterId == stockOutReturnDetailGroup.CostCenterId && x.BarCode == stockOutReturnDetailGroup.BarCode && x.SellingPrice == stockOutReturnDetailGroup.SellingPrice && x.ExpireDate == stockOutReturnDetailGroup.ExpireDate && x.BatchNumber == stockOutReturnDetailGroup.BatchNumber && x.ItemDiscountPercent == stockOutReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
				select new
				{
					stockOutReturnDetailGroup.ItemId,
					stockOutReturnDetailGroup.ItemPackageId,
					SalesInvoiceQuantity = salesInvoiceDetailGroup.Quantity,
					SalesInvoiceBonusQuantity = salesInvoiceDetailGroup.BonusQuantity,
					QuantityReturnAvailable = (finalstockDisbursed != null ? finalstockDisbursed.QuantityDisbursed : 0) - stockOutReturnDetailGroup.Quantity,
					BonusQuantityReturnAvailable = (finalstockDisbursed != null ? finalstockDisbursed.BonusQuantityDisbursed : 0) - stockOutReturnDetailGroup.BonusQuantity
				});

			var exceedingItem = availableReturnQuantities.FirstOrDefault(x => x.QuantityReturnAvailable > x.SalesInvoiceQuantity || x.BonusQuantityReturnAvailable > x.SalesInvoiceBonusQuantity);
			if (exceedingItem != null)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();

				var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
				var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

				if (exceedingItem.QuantityReturnAvailable > exceedingItem.SalesInvoiceQuantity)
				{
					return new ResponseDto { Id = stockOutReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, grandParentMenuCode, SalesMessageData.SaveCauseQuantityExceed, itemName!, itemPackageName!, exceedingItem.QuantityReturnAvailable.ToNormalizedString(), exceedingItem.SalesInvoiceQuantity.ToNormalizedString()) };
				}
				else
				{
					return new ResponseDto { Id = stockOutReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, grandParentMenuCode, SalesMessageData.SaveCauseBonusQuantityExceed, itemName!, itemPackageName!, exceedingItem.BonusQuantityReturnAvailable.ToNormalizedString(), exceedingItem.SalesInvoiceBonusQuantity.ToNormalizedString()) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutReturnFromSalesInvoiceQuantityExceeding(StockOutReturnDto stockOutReturn, int menuCode, int parentMenuCode)
		{
			if (!(stockOutReturn.StockOutReturnHeader?.SalesInvoiceHeaderId > 0)) return new ResponseDto { Message = "StockType mismatches parent foreign keys" };
			var stockOutReturnDetailsGrouped = _stockOutReturnDetailService.GroupStockOutReturnDetailsWithAllKeys(stockOutReturn.StockOutReturnDetails);
			var itemIds = stockOutReturn.StockOutReturnDetails.Select(x => x.ItemId).ToList();

			List<StockDisbursedReturnedDto>? stocksReturned;
			if (stockOutReturn.StockOutReturnHeader.StockOutReturnHeaderId == 0)
			{
				stocksReturned = await _stockOutQuantityService.GetStocksDisbursedReturnedFromSalesInvoice((int)stockOutReturn.StockOutReturnHeader!.SalesInvoiceHeaderId!).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			}
			else
			{
				stocksReturned = await _stockOutQuantityService.GetStocksDisbursedReturnedFromSalesInvoiceExceptStockOutReturnHeaderId((int)stockOutReturn.StockOutReturnHeader.SalesInvoiceHeaderId, stockOutReturn.StockOutReturnHeader.StockOutReturnHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			}

			var salesInvoiceDetailsGrouped = await _salesInvoiceDetailService.GetSalesInvoiceDetailsGrouped((int)stockOutReturn.StockOutReturnHeader!.SalesInvoiceHeaderId!);
			var reservationInvoiceCloseOutDetailsGrouped = await GetReservationInvoiceCloseOutQuantities((int)stockOutReturn.StockOutReturnHeader.SalesInvoiceHeaderId);

			var availableReturnQuantities = (
				from stockOutReturnDetailGroup in stockOutReturnDetailsGrouped
				from stockReturned in                                           stocksReturned.Where(x => x.ItemId == stockOutReturnDetailGroup.ItemId && x.ItemPackageId == stockOutReturnDetailGroup.ItemPackageId && x.BatchNumber == stockOutReturnDetailGroup.BatchNumber && x.ExpireDate == stockOutReturnDetailGroup.ExpireDate && x.CostCenterId == stockOutReturnDetailGroup.CostCenterId && x.BarCode == stockOutReturnDetailGroup.BarCode && x.SellingPrice == stockOutReturnDetailGroup.SellingPrice && x.ItemDiscountPercent == stockOutReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
				from salesInvoiceDetailGroup in                     salesInvoiceDetailsGrouped.Where(x => x.ItemId == stockOutReturnDetailGroup.ItemId && x.ItemPackageId == stockOutReturnDetailGroup.ItemPackageId && x.BatchNumber == stockOutReturnDetailGroup.BatchNumber && x.ExpireDate == stockOutReturnDetailGroup.ExpireDate && x.CostCenterId == stockOutReturnDetailGroup.CostCenterId && x.BarCode == stockOutReturnDetailGroup.BarCode && x.SellingPrice == stockOutReturnDetailGroup.SellingPrice && x.ItemDiscountPercent == stockOutReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
				from salesInvoiceReturnDetailGroup in reservationInvoiceCloseOutDetailsGrouped.Where(x => x.ItemId == stockOutReturnDetailGroup.ItemId && x.ItemPackageId == stockOutReturnDetailGroup.ItemPackageId && x.BatchNumber == stockOutReturnDetailGroup.BatchNumber && x.ExpireDate == stockOutReturnDetailGroup.ExpireDate && x.CostCenterId == stockOutReturnDetailGroup.CostCenterId && x.BarCode == stockOutReturnDetailGroup.BarCode && x.SellingPrice == stockOutReturnDetailGroup.SellingPrice && x.ItemDiscountPercent == stockOutReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
				select new
				{
					stockOutReturnDetailGroup.ItemId,
					stockOutReturnDetailGroup.ItemPackageId,
					SalesInvoiceQuantity = salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.Quantity - (salesInvoiceReturnDetailGroup != null ? salesInvoiceReturnDetailGroup.Quantity : 0) : 0,
					SalesInvoiceBonusQuantity = salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.BonusQuantity - (salesInvoiceReturnDetailGroup != null ? salesInvoiceReturnDetailGroup.BonusQuantity : 0) : 0,
					QuantityReturned = (stockReturned != null ? stockReturned.QuantityReturned : 0) + stockOutReturnDetailGroup.Quantity,
					BonusQuantityReturned = (stockReturned != null ? stockReturned.BonusQuantityReturned : 0) + stockOutReturnDetailGroup.BonusQuantity,
					QuantityReturnAvailable = (salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.Quantity : 0) - (salesInvoiceReturnDetailGroup != null ? salesInvoiceReturnDetailGroup.Quantity : 0) - (stockReturned != null ? stockReturned.QuantityReturned : 0) - stockOutReturnDetailGroup.Quantity,
					BonusQuantityReturnAvailable = (salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.BonusQuantity : 0) - (salesInvoiceReturnDetailGroup != null ? salesInvoiceReturnDetailGroup.BonusQuantity : 0) - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0) - stockOutReturnDetailGroup.BonusQuantity
				});

			var exceedingItem = availableReturnQuantities.FirstOrDefault(x => x.QuantityReturnAvailable < 0 || x.BonusQuantityReturnAvailable < 0);
			if (exceedingItem != null)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();

				var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
				var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

				if (exceedingItem.QuantityReturnAvailable < 0)
				{
					return new ResponseDto { Id = stockOutReturn.StockOutReturnHeader.StockOutReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.QuantityExceeding, itemName!, itemPackageName!, exceedingItem.QuantityReturned.ToNormalizedString(), exceedingItem.SalesInvoiceQuantity.ToNormalizedString()) };
				}
				else
				{
					return new ResponseDto { Id = stockOutReturn.StockOutReturnHeader.StockOutReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.BonusQuantityExceeding, itemName!, itemPackageName!, exceedingItem.BonusQuantityReturned.ToNormalizedString(), exceedingItem.SalesInvoiceBonusQuantity.ToNormalizedString()) };
				}
			}
			return new ResponseDto { Success = true };
		}

		private async Task<List<SalesInvoiceReturnDetailDto>> GetReservationInvoiceCloseOutQuantities(int salesInvoiceHeaderId)
		{
			return await (from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId)
						  from salesInvoiceReturnDetails in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId)
						  select salesInvoiceReturnDetails).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber, x.CostCenterId, x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(x =>
						  new SalesInvoiceReturnDetailDto
						  {
							  ItemId = x.Key.ItemId,
							  ItemPackageId = x.Key.ItemPackageId,
							  ExpireDate = x.Key.ExpireDate,
							  BatchNumber = x.Key.BatchNumber,
							  CostCenterId = x.Key.CostCenterId,
							  BarCode = x.Key.BarCode,
							  SellingPrice = x.Key.SellingPrice,
							  ItemDiscountPercent = x.Key.ItemDiscountPercent,
							  Quantity = x.Sum(x => x.Quantity),
							  BonusQuantity = x.Sum(x => x.BonusQuantity)
						  }).ToListAsync();
		}

		private void TrimDetailStrings(List<StockOutReturnDetailDto> stockOutReturnDetails)
        {
            foreach (var stockOutReturnDetail in stockOutReturnDetails)
            {
                stockOutReturnDetail.BatchNumber = string.IsNullOrWhiteSpace(stockOutReturnDetail.BatchNumber) ? null : stockOutReturnDetail.BatchNumber.Trim();
            }
        }

        private async Task UpdateModelPrices(StockOutReturnDto stockOutReturn)
        {
            await UpdateDetailPrices(stockOutReturn.StockOutReturnDetails, stockOutReturn.StockOutReturnHeader!.StoreId);
            stockOutReturn.StockOutReturnHeader!.TotalCostValue = stockOutReturn.StockOutReturnDetails.Sum(x => x.CostValue);
        }

        private async Task UpdateDetailPrices(List<StockOutReturnDetailDto> stockOutReturnDetails, int storeId)
        {
            var itemIds = stockOutReturnDetails.Select(x => x.ItemId).ToList();

            var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId) && x.StoreId == storeId).Select(x => new { x.StoreId, x.ItemId, x.CostPrice }).ToListAsync();
            var consumerPrices = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).Select(x => new { x.ItemId, x.ConsumerPrice }).ToListAsync();
            var lastSellingPrices = await _salesInvoiceService.GetMultipleLastSalesPrices(itemIds);

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

            foreach (var stockOutReturnDetail in stockOutReturnDetails)
            {
                var packing = packings.Where(x => x.ItemId == stockOutReturnDetail.ItemId && x.FromPackageId == stockOutReturnDetail.ItemPackageId).Select(x => x.Packing).FirstOrDefault(1);

                stockOutReturnDetail.ConsumerPrice = consumerPrices.Where(x => x.ItemId == stockOutReturnDetail.ItemId).Select(x => x.ConsumerPrice).FirstOrDefault(0);
                stockOutReturnDetail.CostPrice = itemCosts.Where(x => x.ItemId == stockOutReturnDetail.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefault(0);
                stockOutReturnDetail.CostPackage = stockOutReturnDetail.CostPrice * packing;
                stockOutReturnDetail.CostValue = stockOutReturnDetail.CostPackage * (stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity);
                stockOutReturnDetail.LastSalesPrice = lastSellingPrices.Where(x => x.ItemId == stockOutReturnDetail.ItemId && x.ItemPackageId == stockOutReturnDetail.ItemPackageId).Select(x => x.SellingPrice).FirstOrDefault(0);
            }
        }

        public async Task<ResponseDto> DeleteStockOutReturn(int stockOutReturnHeaderId, int menuCode, bool affectBalances = true)
        {
			var stockOutReturnHeader = await _stockOutReturnHeaderService.GetStockOutReturnHeaderById(stockOutReturnHeaderId);
			if (stockOutReturnHeader == null) return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };

			var grandParentMenuCode = await GetGrandParentMenuCode(stockOutReturnHeader.StockOutHeaderId);
			var parentMenuCode = await GetParentMenuCode(stockOutReturnHeader.StockOutHeaderId, stockOutReturnHeader.SalesInvoiceHeaderId);
			ResponseDto validationResult = await CheckStockOutReturnIsValidForDelete(stockOutReturnHeader.StockOutReturnHeaderId, stockOutReturnHeader.StoreId, stockOutReturnHeader.StockOutHeaderId, stockOutReturnHeader.IsEnded, stockOutReturnHeader.IsBlocked, menuCode, parentMenuCode, grandParentMenuCode);
			if (validationResult.Success == false) return validationResult;


            var result = affectBalances ? 
				await _stockOutReturnService.DeleteStockOutReturn(stockOutReturnHeaderId, menuCode) :
				await _stockOutReturnService.DeleteStockOutReturnWithoutUpdatingBalances(stockOutReturnHeaderId, menuCode);
			if (result.Success == true)
			{
				await ReopenStockOutReturnParent(stockOutReturnHeader.StockOutHeaderId, stockOutReturnHeader.SalesInvoiceHeaderId);
				await UpdateProformaInvoiceStatusFromStockOutReturn(stockOutReturnHeader.StockOutHeaderId, stockOutReturnHeader.SalesInvoiceHeaderId);
				await UpdateReservationInvoiceCloseOutEnded(stockOutReturnHeader.SalesInvoiceHeaderId, false);
			}
			return result;
        }

		public async Task<ResponseDto> CheckStockOutReturnIsValidForDelete(int stockOutReturnHeaderId, int storeId, int? stockOutHeaderId, bool isEnded, bool isBlocked, int menuCode, int parentMenuCode, int? grandParentMenuCode)
		{
			if (isBlocked) return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };

			if (isEnded) return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseInvoiced) };

			var zeroStockResult = await CheckStockOutReturnZeroStock(stockOutReturnHeaderId, storeId, [], menuCode, parentMenuCode, grandParentMenuCode, false);
			if (zeroStockResult.Success == false) return zeroStockResult;

			if (stockOutHeaderId != null)
			{
				ResponseDto quantityResult = await CheckForDisbursedQuantityExceededAfterDeleting(stockOutReturnHeaderId, (int)stockOutHeaderId, menuCode, (int)grandParentMenuCode!);
				if (quantityResult.Success == false) return quantityResult;
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckForDisbursedQuantityExceededAfterDeleting(int stockOutReturnHeaderId, int stockOutHeaderId, int menuCode, int grandParentMenuCode)
		{
			var stockOutHeader = await _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId).Select(x => new { x.ProformaInvoiceHeaderId, x.SalesInvoiceHeaderId }).FirstOrDefaultAsync();
			ResponseDto? quantityResult = null;

			if (stockOutHeader!.ProformaInvoiceHeaderId != null)
			{
				quantityResult = await CheckStockOutReturnFromStockOutFromProformaInvoiceQuantityForDelete(stockOutReturnHeaderId, (int)stockOutHeader.ProformaInvoiceHeaderId, menuCode, grandParentMenuCode);
			}
			else
			{
				quantityResult = await CheckStockOutReturnFromStockOutFromSalesInvoiceQuantityForDelete(stockOutReturnHeaderId, (int)stockOutHeader.SalesInvoiceHeaderId!, menuCode, grandParentMenuCode);
			}

			if (quantityResult!.Success == false) return quantityResult;

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutReturnFromStockOutFromProformaInvoiceQuantityForDelete(int stockOutReturnHeaderId, int proformaInvoiceHeaderId, int menuCode, int grandParentMenuCode)
		{
			var finalstocksDisbursed = _stockOutQuantityService.GetFinalStocksDisbursedFromProformaInvoiceExceptStockOutReturnHeaderId(proformaInvoiceHeaderId, stockOutReturnHeaderId);
			var purchaseInvoiceDetailsGrouped = _proformaInvoiceDetailService.GetProformaInvoiceDetailsGroupedQueryable(proformaInvoiceHeaderId);

			var exceedingItem = await (
					from finalstockDisbursed in finalstocksDisbursed
					from proformaInvoiceDetailGroup in purchaseInvoiceDetailsGrouped.Where(x => x.ItemId == finalstockDisbursed.ItemId && x.ItemPackageId == finalstockDisbursed.ItemPackageId && x.BarCode == finalstockDisbursed.BarCode && x.SellingPrice == finalstockDisbursed.SellingPrice && x.ItemDiscountPercent == finalstockDisbursed.ItemDiscountPercent)
					select new
					{
						finalstockDisbursed.ItemId,
						finalstockDisbursed.ItemPackageId,
						ProformaInvoiceQuantity = proformaInvoiceDetailGroup.Quantity,
						ProformaInvoiceBonusQuantity = proformaInvoiceDetailGroup.BonusQuantity,
						QuantityReturnAvailable = finalstockDisbursed.QuantityDisbursed,
						BonusQuantityReturnAvailable = finalstockDisbursed.BonusQuantityDisbursed
					}
				).FirstOrDefaultAsync(x => x.QuantityReturnAvailable > x.ProformaInvoiceQuantity || x.BonusQuantityReturnAvailable > x.ProformaInvoiceBonusQuantity);

			if (exceedingItem != null)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();

				var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
				var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

				if (exceedingItem.QuantityReturnAvailable > exceedingItem.ProformaInvoiceQuantity)
				{
					return new ResponseDto { Id = stockOutReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, grandParentMenuCode, SalesMessageData.DeleteCauseQuantityExceed, itemName!, itemPackageName!, exceedingItem.QuantityReturnAvailable.ToNormalizedString(), exceedingItem.ProformaInvoiceQuantity.ToNormalizedString()) };
				}
				else
				{
					return new ResponseDto { Id = stockOutReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, grandParentMenuCode, SalesMessageData.DeleteCauseBonusQuantityExceed, itemName!, itemPackageName!, exceedingItem.BonusQuantityReturnAvailable.ToNormalizedString(), exceedingItem.ProformaInvoiceBonusQuantity.ToNormalizedString()) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutReturnFromStockOutFromSalesInvoiceQuantityForDelete(int stockOutReturnHeaderId, int salesInvoiceHeaderId, int menuCode, int grandParentMenuCode)
		{
			var finalstocksDisbursed = _stockOutQuantityService.GetFinalStocksDisbursedFromSalesInvoiceExceptStockOutReturnHeaderId(salesInvoiceHeaderId, stockOutReturnHeaderId);
			var salesInvoiceDetailGrouped = _salesInvoiceDetailService.GetSalesInvoiceDetailsGroupedQueryable(salesInvoiceHeaderId);

			var exceedingItem = await (
					from finalstockDisbursed in finalstocksDisbursed
					from salesInvoiceDetailGroup in salesInvoiceDetailGrouped.Where(x => x.ItemId == finalstockDisbursed.ItemId && x.ItemPackageId == finalstockDisbursed.ItemPackageId && x.CostCenterId == finalstockDisbursed.CostCenterId && x.BarCode == finalstockDisbursed.BarCode && x.SellingPrice == finalstockDisbursed.SellingPrice && x.ExpireDate == finalstockDisbursed.ExpireDate && x.BatchNumber == finalstockDisbursed.BatchNumber && x.ItemDiscountPercent == finalstockDisbursed.ItemDiscountPercent)
					select new
					{
						finalstockDisbursed.ItemId,
						finalstockDisbursed.ItemPackageId,
						SalesInvoiceQuantity = salesInvoiceDetailGroup.Quantity,
						SalesInvoiceBonusQuantity = salesInvoiceDetailGroup.BonusQuantity,
						QuantityReturnAvailable = finalstockDisbursed.QuantityDisbursed,
						BonusQuantityReturnAvailable = finalstockDisbursed.BonusQuantityDisbursed
					}
				).FirstOrDefaultAsync(x => x.QuantityReturnAvailable > x.SalesInvoiceQuantity || x.BonusQuantityReturnAvailable > x.SalesInvoiceBonusQuantity);

			if (exceedingItem != null)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();

				var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
				var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

				if (exceedingItem.QuantityReturnAvailable > exceedingItem.SalesInvoiceQuantity)
				{
					return new ResponseDto { Id = stockOutReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, grandParentMenuCode, SalesMessageData.DeleteCauseQuantityExceed, itemName!, itemPackageName!, exceedingItem.QuantityReturnAvailable.ToNormalizedString(), exceedingItem.SalesInvoiceQuantity.ToNormalizedString()) };
				}
				else
				{
					return new ResponseDto { Id = stockOutReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, grandParentMenuCode, SalesMessageData.DeleteCauseBonusQuantityExceed, itemName!, itemPackageName!, exceedingItem.BonusQuantityReturnAvailable.ToNormalizedString(), exceedingItem.SalesInvoiceBonusQuantity.ToNormalizedString()) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task ReopenStockOutReturnParent(int? stockOutHeaderId, int? salesInvoiceHeaderId)
		{
			if (stockOutHeaderId != null)
			{
				var isStocksRemaining = await _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId).AnyAsync();
				if (!isStocksRemaining)
				{
					await _stockOutHeaderService.UpdateClosed(stockOutHeaderId, false);
				}
			}
			else if (salesInvoiceHeaderId != null)
			{
				//if there is any stock Out, stock out return or sales invoice return related to this sales invoice header, then do not open it
				var isStocksRemaining = await _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.StockOutReturnHeaderId)
											   	.Concat(_stockOutHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.StockOutHeaderId))
											 	.Concat(_salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.SalesInvoiceReturnHeaderId)).AnyAsync();
				if (!isStocksRemaining)
				{
					await _salesInvoiceHeaderService.UpdateClosed(salesInvoiceHeaderId, false);
				}
			}
		}
	}
}
