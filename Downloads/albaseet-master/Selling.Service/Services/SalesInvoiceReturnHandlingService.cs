using Sales.CoreOne.Models.Dtos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Models.Dtos;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Items;
using Microsoft.EntityFrameworkCore;
using Sales.CoreOne.Models.Domain;
using Shared.Service.Logic.Calculation;
using Shared.Helper.Extensions;
using Shared.Service.Services.Items;
using Shared.CoreOne.Models.Domain.Items;
using Shared.Helper.Logic;
using Purchases.CoreOne.Models.StaticData;
using Sales.CoreOne.Models.StaticData;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Modules;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Purchases.Service.Services;
using Shared.CoreOne.Contracts.Accounts;

namespace Sales.Service.Services
{
    public class SalesInvoiceReturnHandlingService: ISalesInvoiceReturnHandlingService
    {
		private readonly IStockOutReturnService _stockOutReturnService;
		private readonly IInvoiceStockOutReturnService _invoiceStockOutReturnService;
		private readonly IStoreService _storeService;
		private readonly IItemTaxService _itemTaxService;
		private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IStockOutQuantityService _stockOutQuantityService;
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
        private readonly ISalesInvoiceDetailTaxService _salesInvoiceDetailTaxService;
        private readonly ISalesInvoiceReturnService _salesInvoiceReturnService;
        private readonly ISalesInvoiceService _salesInvoiceService;
        private readonly IItemCostService _itemCostService;
        private readonly IItemService _itemService;
        private readonly IItemPackingService _itemPackingService;
		private readonly IStockOutHeaderService _stockOutHeaderService;
		private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
		private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
		private readonly IProformaInvoiceStatusService _proformaInvoiceStatusService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly ISalesInvoiceReturnDetailService _salesInvoiceReturnDetailService;
		private readonly IGenericMessageService _genericMessageService;
		private readonly ISalesMessageService _salesMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemPackageService _itemPackageService;
		private readonly IClientDebitMemoService _clientDebitMemoService;
		private readonly IClientCreditMemoService _clientCreditMemoService;
		private readonly IGetSalesInvoiceSettleValueService _getSalesInvoiceSettleValueService;
		private readonly IZeroStockValidationService _zeroStockValidationService;
		private readonly IItemNoteValidationService _itemNoteValidationService;
		private readonly IAccountService _accountService;

        public SalesInvoiceReturnHandlingService(IStockOutReturnService stockOutReturnService, IInvoiceStockOutReturnService invoiceStockOutReturnService, IStoreService storeService, IItemTaxService itemTaxService, IItemBarCodeService itemBarCodeService, IStockOutQuantityService stockOutQuantityService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceDetailService salesInvoiceDetailService, ISalesInvoiceDetailTaxService salesInvoiceDetailTaxService, ISalesInvoiceReturnService salesInvoiceReturnService, ISalesInvoiceService salesInvoiceService, IItemCostService itemCostService, IItemService itemService, IItemPackingService itemPackingService, IStockOutHeaderService stockOutHeaderService, IStockOutReturnHeaderService stockOutReturnHeaderService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, IProformaInvoiceStatusService proformaInvoiceStatusService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService, IGenericMessageService genericMessageService, ISalesMessageService salesMessageService, IHttpContextAccessor httpContextAccessor, IItemPackageService itemPackageService, IClientDebitMemoService clientDebitMemoService, IClientCreditMemoService clientCreditMemoService, IGetSalesInvoiceSettleValueService getSalesInvoiceSettleValueService, IZeroStockValidationService zeroStockValidationService, IItemNoteValidationService itemNoteValidationService, IAccountService accountService)
        {
			_stockOutReturnService = stockOutReturnService;
			_invoiceStockOutReturnService = invoiceStockOutReturnService;
			_storeService = storeService;
			_itemTaxService = itemTaxService;
			_itemBarCodeService = itemBarCodeService;
            _stockOutQuantityService = stockOutQuantityService;
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _salesInvoiceDetailService = salesInvoiceDetailService;
            _salesInvoiceDetailTaxService = salesInvoiceDetailTaxService;
            _salesInvoiceReturnService = salesInvoiceReturnService;
            _salesInvoiceService = salesInvoiceService;
            _itemCostService = itemCostService;
            _itemService = itemService;
            _itemPackingService = itemPackingService;
			_stockOutHeaderService = stockOutHeaderService;
			_stockOutReturnHeaderService = stockOutReturnHeaderService;
			_proformaInvoiceHeaderService = proformaInvoiceHeaderService;
			_proformaInvoiceStatusService = proformaInvoiceStatusService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_salesInvoiceReturnDetailService = salesInvoiceReturnDetailService;
			_genericMessageService = genericMessageService;
			_salesMessageService = salesMessageService;
			_httpContextAccessor = httpContextAccessor;
			_itemPackageService = itemPackageService;
			_clientDebitMemoService = clientDebitMemoService;
			_clientCreditMemoService = clientCreditMemoService;
			_getSalesInvoiceSettleValueService = getSalesInvoiceSettleValueService;
			_zeroStockValidationService = zeroStockValidationService;
			_itemNoteValidationService = itemNoteValidationService;
			_accountService = accountService;
        }

        public async Task<SalesInvoiceReturnDto> GetSalesInvoiceReturnFromSalesInvoice(int salesInvoiceHeaderId, bool isDirectInvoice, bool isOnTheWay)
        {
			var header = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById(salesInvoiceHeaderId);
			if (header == null) return new SalesInvoiceReturnDto();

			var salesInvoiceReturnDetails = await GetSalesInvoiceReturnDetailsFromSalesInvoice(salesInvoiceHeaderId, header.DiscountPercent, isDirectInvoice, isOnTheWay);
			var salesInvoiceReturnHeader = GetSalesInvoiceReturnHeaderFromSalesInvoice(salesInvoiceReturnDetails, header, isOnTheWay, isDirectInvoice);

			return new SalesInvoiceReturnDto { SalesInvoiceReturnHeader = salesInvoiceReturnHeader, SalesInvoiceReturnDetails = salesInvoiceReturnDetails };
		}

		private async Task<List<SalesInvoiceReturnDetailDto>> GetSalesInvoiceReturnDetailsFromSalesInvoice(int salesInvoiceHeaderId, decimal headerDiscountPercent, bool isDirectInvoice, bool isOnTheWay)
		{
			var salesInvoiceDetails = await _salesInvoiceDetailService.GetSalesInvoiceDetailsAsQueryable(salesInvoiceHeaderId).ToListAsync();

			var itemIds = salesInvoiceDetails.Select(x => x.ItemId).ToList();

			var salesInvoiceDetailData = new SalesInvoiceDetailDataDto
			{
				SalesInvoiceHeaderId = salesInvoiceHeaderId,
				HeaderDiscountPercent = headerDiscountPercent,
				SalesInvoiceDetails = salesInvoiceDetails,
				ItemIds = itemIds,
				Items = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync(),
				ItemPackings = await _itemPackingService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync(),
				ItemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync(),
				LastSalesPrices = await _salesInvoiceService.GetMultipleLastSalesPrices(itemIds)
			};

			var salesInvoiceReturnDetails = (isDirectInvoice, isOnTheWay) switch
			{
				(false, true) => await GetReservationInvoiceCloseoutDetailsFromReservationInvoice(salesInvoiceDetailData),
				(false, false) => await GetSalesInvoiceReturnDetailsFromSalesInvoice(salesInvoiceDetailData),
				(true, false) => await GetDirectSalesInvoiceReturnDetailsFromSalesInvoice(salesInvoiceDetailData),
				(true, true) => throw new InvalidOperationException("IsDirectInvoice and IsOnTheWay cannot both be true")
			};

			salesInvoiceReturnDetails = DistributeQuantityAndCalculateValues(salesInvoiceReturnDetails, headerDiscountPercent);
			salesInvoiceReturnDetails = salesInvoiceReturnDetails.Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();

			await GetAuxiliaryData(salesInvoiceReturnDetails);
			await CalculateOtherTaxes(salesInvoiceReturnDetails, headerDiscountPercent, salesInvoiceHeaderId);
			SerializeSalesInvoiceReturnDetails(salesInvoiceReturnDetails);

			return salesInvoiceReturnDetails;
		}

		private async Task<List<SalesInvoiceReturnDetailDto>> GetReservationInvoiceCloseoutDetailsFromReservationInvoice(SalesInvoiceDetailDataDto salesInvoiceDetailData)
		{
			var finalStocksDisbursed = await _stockOutQuantityService.GetFinalStocksDisbursedFromSalesInvoice(salesInvoiceDetailData.SalesInvoiceHeaderId).Where(x => salesInvoiceDetailData.ItemIds.Contains(x.ItemId)).ToListAsync();
			var salesInvoiceDetailsGrouped = _salesInvoiceDetailService.GroupSalesInvoiceDetails(salesInvoiceDetailData.SalesInvoiceDetails);

			var salesInvoiceReturnDetails = (from salesInvoiceDetail in salesInvoiceDetailData.SalesInvoiceDetails
											 from finalStockDisbursed in finalStocksDisbursed.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId && x.BarCode == salesInvoiceDetail.BarCode && x.SellingPrice == salesInvoiceDetail.SellingPrice && x.CostCenterId == salesInvoiceDetail.CostCenterId && x.ExpireDate == salesInvoiceDetail.ExpireDate && x.BatchNumber == salesInvoiceDetail.BatchNumber && x.ItemDiscountPercent == salesInvoiceDetail.ItemDiscountPercent).DefaultIfEmpty()
											 from salesInvoiceDetailGroup in salesInvoiceDetailsGrouped.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId && x.BarCode == salesInvoiceDetail.BarCode && x.SellingPrice == salesInvoiceDetail.SellingPrice && x.CostCenterId == salesInvoiceDetail.CostCenterId && x.ExpireDate == salesInvoiceDetail.ExpireDate && x.BatchNumber == salesInvoiceDetail.BatchNumber && x.ItemDiscountPercent == salesInvoiceDetail.ItemDiscountPercent)
											 from item in salesInvoiceDetailData.Items.Where(x => x.ItemId == salesInvoiceDetail.ItemId)
											 from itemPacking in salesInvoiceDetailData.ItemPackings.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.FromPackageId == salesInvoiceDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
											 from itemCost in salesInvoiceDetailData.ItemCosts.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId).DefaultIfEmpty()
											 from lastSalesPrice in salesInvoiceDetailData.LastSalesPrices.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId).DefaultIfEmpty()
											 select new SalesInvoiceReturnDetailDto
											 {
												 SalesInvoiceReturnDetailId = salesInvoiceDetail.SalesInvoiceDetailId, // <-- used for detail taxes
												 ItemId = salesInvoiceDetail.ItemId,
												 ItemCode = salesInvoiceDetail.ItemCode,
												 ItemName = salesInvoiceDetail.ItemName,
												 TaxTypeId = salesInvoiceDetail.TaxTypeId,
												 ItemTypeId = salesInvoiceDetail.ItemTypeId,
												 ItemPackageId = salesInvoiceDetail.ItemPackageId,
												 ItemPackageName = salesInvoiceDetail.ItemPackageName,
												 IsItemVatInclusive = salesInvoiceDetail.IsItemVatInclusive,
												 CostCenterId = salesInvoiceDetail.CostCenterId,
												 CostCenterName = salesInvoiceDetail.CostCenterName,
												 BarCode = salesInvoiceDetail.BarCode,
												 Packing = salesInvoiceDetail.Packing,
												 ExpireDate = salesInvoiceDetail.ExpireDate,
												 BatchNumber = salesInvoiceDetail.BatchNumber,
												 Quantity = salesInvoiceDetail.Quantity,
												 BonusQuantity = salesInvoiceDetail.BonusQuantity,
												 //key level quantities are internal to the backend and marked with JsonIgnore, I just need it 
												 //for the quantity distribution algorithm
												 KeyLevelQuantity = salesInvoiceDetailGroup.Quantity - (finalStockDisbursed != null ? finalStockDisbursed.QuantityDisbursed : 0),
												 KeyLevelBonusQuantity = salesInvoiceDetailGroup.BonusQuantity - (finalStockDisbursed != null ? finalStockDisbursed.BonusQuantityDisbursed : 0),
												 SalesInvoiceQuantity = salesInvoiceDetailGroup.Quantity,
												 SalesInvoiceBonusQuantity = salesInvoiceDetailGroup.BonusQuantity,
												 AvailableQuantity = 0,
												 AvailableBonusQuantity = 0,
												 SellingPrice = salesInvoiceDetail.SellingPrice,
												 ItemDiscountPercent = salesInvoiceDetail.ItemDiscountPercent,
												 VatPercent = salesInvoiceDetail.VatPercent,
												 Notes = salesInvoiceDetail.Notes,
												 ItemNote = salesInvoiceDetail.ItemNote,
										         VatTaxId = salesInvoiceDetail.VatTaxId,
										         VatTaxTypeId = salesInvoiceDetail.VatTaxTypeId,
												 ConsumerPrice = item.ConsumerPrice,
												 CostPrice = itemCost != null ? itemCost.CostPrice : 0,
												 CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
												 LastSalesPrice = lastSalesPrice != null ? lastSalesPrice.SellingPrice : 0
											 }).ToList();

			return salesInvoiceReturnDetails;
		}

		private async Task<List<SalesInvoiceReturnDetailDto>> GetSalesInvoiceReturnDetailsFromSalesInvoice(SalesInvoiceDetailDataDto salesInvoiceDetailData)
		{
			var uninvoicedStocksReturned = await _stockOutQuantityService.GetUnInvoicedStocksDisbursedReturnedFromSalesInvoice(salesInvoiceDetailData.SalesInvoiceHeaderId).Where(x => salesInvoiceDetailData.ItemIds.Contains(x.ItemId)).ToListAsync();
			var salesInvoiceDetailsGrouped = _salesInvoiceDetailService.GroupSalesInvoiceDetails(salesInvoiceDetailData.SalesInvoiceDetails);

			var salesInvoiceReturnDetails = (from salesInvoiceDetail in salesInvoiceDetailData.SalesInvoiceDetails
											 from uninvoicedStockReturned in uninvoicedStocksReturned.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId && x.BarCode == salesInvoiceDetail.BarCode && x.SellingPrice == salesInvoiceDetail.SellingPrice && x.CostCenterId == salesInvoiceDetail.CostCenterId && x.ExpireDate == salesInvoiceDetail.ExpireDate && x.BatchNumber == salesInvoiceDetail.BatchNumber && x.ItemDiscountPercent == salesInvoiceDetail.ItemDiscountPercent).DefaultIfEmpty()
											 from salesInvoiceDetailGroup in salesInvoiceDetailsGrouped.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId && x.BarCode == salesInvoiceDetail.BarCode && x.SellingPrice == salesInvoiceDetail.SellingPrice && x.CostCenterId == salesInvoiceDetail.CostCenterId && x.ExpireDate == salesInvoiceDetail.ExpireDate && x.BatchNumber == salesInvoiceDetail.BatchNumber && x.ItemDiscountPercent == salesInvoiceDetail.ItemDiscountPercent)
											 from item in salesInvoiceDetailData.Items.Where(x => x.ItemId == salesInvoiceDetail.ItemId)
											 from itemPacking in salesInvoiceDetailData.ItemPackings.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.FromPackageId == salesInvoiceDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
											 from itemCost in salesInvoiceDetailData.ItemCosts.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId).DefaultIfEmpty()
											 from lastSalesPrice in salesInvoiceDetailData.LastSalesPrices.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId).DefaultIfEmpty()
											 select new SalesInvoiceReturnDetailDto
											 {
												 SalesInvoiceReturnDetailId = salesInvoiceDetail.SalesInvoiceDetailId, // <-- used for detail taxes
												 ItemId = salesInvoiceDetail.ItemId,
												 ItemCode = salesInvoiceDetail.ItemCode,
												 ItemName = salesInvoiceDetail.ItemName,
												 TaxTypeId = salesInvoiceDetail.TaxTypeId,
												 ItemTypeId = salesInvoiceDetail.ItemTypeId,
												 ItemPackageId = salesInvoiceDetail.ItemPackageId,
												 ItemPackageName = salesInvoiceDetail.ItemPackageName,
												 IsItemVatInclusive = salesInvoiceDetail.IsItemVatInclusive,
												 CostCenterId = salesInvoiceDetail.CostCenterId,
												 CostCenterName = salesInvoiceDetail.CostCenterName,
												 BarCode = salesInvoiceDetail.BarCode,
												 Packing = salesInvoiceDetail.Packing,
												 ExpireDate = salesInvoiceDetail.ExpireDate,
												 BatchNumber = salesInvoiceDetail.BatchNumber,
												 Quantity = salesInvoiceDetail.Quantity,
												 BonusQuantity = salesInvoiceDetail.BonusQuantity,
												 KeyLevelQuantity = uninvoicedStockReturned != null ? uninvoicedStockReturned.QuantityReturned : 0,
												 KeyLevelBonusQuantity = uninvoicedStockReturned != null ? uninvoicedStockReturned.BonusQuantityReturned : 0,
												 //key level quantities are internal to the backend and marked with JsonIgnore, I just need it 
												 //for the quantity distribution algorithm
												 SalesInvoiceQuantity = salesInvoiceDetailGroup.Quantity,
												 SalesInvoiceBonusQuantity = salesInvoiceDetailGroup.BonusQuantity,
												 AvailableQuantity = 0,
												 AvailableBonusQuantity = 0,
												 SellingPrice = salesInvoiceDetail.SellingPrice,
												 ItemDiscountPercent = salesInvoiceDetail.ItemDiscountPercent,
												 VatPercent = salesInvoiceDetail.VatPercent,
												 Notes = salesInvoiceDetail.Notes,
												 ItemNote = salesInvoiceDetail.ItemNote,
										         VatTaxId = salesInvoiceDetail.VatTaxId,
										         VatTaxTypeId = salesInvoiceDetail.VatTaxTypeId,
												 ConsumerPrice = item.ConsumerPrice,
												 CostPrice = itemCost != null ? itemCost.CostPrice : 0,
												 CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
												 LastSalesPrice = lastSalesPrice != null ? lastSalesPrice.SellingPrice : 0
											 }).ToList();

			return salesInvoiceReturnDetails;
		}

		private async Task<List<SalesInvoiceReturnDetailDto>> GetDirectSalesInvoiceReturnDetailsFromSalesInvoice(SalesInvoiceDetailDataDto salesInvoiceDetailData)
		{
			var stocksReturned = await _stockOutQuantityService.GetStocksDisbursedReturnedFromSalesInvoice(salesInvoiceDetailData.SalesInvoiceHeaderId).Where(x => salesInvoiceDetailData.ItemIds.Contains(x.ItemId)).ToListAsync();
			var salesInvoiceDetailsGrouped = _salesInvoiceDetailService.GroupSalesInvoiceDetails(salesInvoiceDetailData.SalesInvoiceDetails);

			var salesInvoiceReturnDetails = (from salesInvoiceDetail in salesInvoiceDetailData.SalesInvoiceDetails
											 from stockReturned in stocksReturned.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId && x.BarCode == salesInvoiceDetail.BarCode && x.SellingPrice == salesInvoiceDetail.SellingPrice && x.CostCenterId == salesInvoiceDetail.CostCenterId && x.ExpireDate == salesInvoiceDetail.ExpireDate && x.BatchNumber == salesInvoiceDetail.BatchNumber && x.ItemDiscountPercent == salesInvoiceDetail.ItemDiscountPercent).DefaultIfEmpty()
											 from salesInvoiceDetailGroup in salesInvoiceDetailsGrouped.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId && x.BarCode == salesInvoiceDetail.BarCode && x.SellingPrice == salesInvoiceDetail.SellingPrice && x.CostCenterId == salesInvoiceDetail.CostCenterId && x.ExpireDate == salesInvoiceDetail.ExpireDate && x.BatchNumber == salesInvoiceDetail.BatchNumber && x.ItemDiscountPercent == salesInvoiceDetail.ItemDiscountPercent)
											 from item in salesInvoiceDetailData.Items.Where(x => x.ItemId == salesInvoiceDetail.ItemId)
											 from itemPacking in salesInvoiceDetailData.ItemPackings.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.FromPackageId == salesInvoiceDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
											 from itemCost in salesInvoiceDetailData.ItemCosts.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId).DefaultIfEmpty()
											 from lastSalesPrice in salesInvoiceDetailData.LastSalesPrices.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId).DefaultIfEmpty()
											 select new SalesInvoiceReturnDetailDto
											 {
												 SalesInvoiceReturnDetailId = salesInvoiceDetail.SalesInvoiceDetailId, // <-- used for detail taxes
												 ItemId = salesInvoiceDetail.ItemId,
												 ItemCode = salesInvoiceDetail.ItemCode,
												 ItemName = salesInvoiceDetail.ItemName,
												 TaxTypeId = salesInvoiceDetail.TaxTypeId,
												 ItemTypeId = salesInvoiceDetail.ItemTypeId,
												 ItemPackageId = salesInvoiceDetail.ItemPackageId,
												 ItemPackageName = salesInvoiceDetail.ItemPackageName,
												 IsItemVatInclusive = salesInvoiceDetail.IsItemVatInclusive,
												 CostCenterId = salesInvoiceDetail.CostCenterId,
												 CostCenterName = salesInvoiceDetail.CostCenterName,
												 BarCode = salesInvoiceDetail.BarCode,
												 Packing = salesInvoiceDetail.Packing,
												 ExpireDate = salesInvoiceDetail.ExpireDate,
												 BatchNumber = salesInvoiceDetail.BatchNumber,
												 Quantity = salesInvoiceDetail.Quantity,
												 BonusQuantity = salesInvoiceDetail.BonusQuantity,
												 KeyLevelQuantity = salesInvoiceDetailGroup.Quantity - (stockReturned != null ? stockReturned.QuantityReturned : 0),
												 KeyLevelBonusQuantity = salesInvoiceDetailGroup.BonusQuantity - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0),
												 //key level quantities are internal to the backend and marked with JsonIgnore, I just need it 
												 //for the quantity distribution algorithm
												 SalesInvoiceQuantity = salesInvoiceDetailGroup.Quantity,
												 SalesInvoiceBonusQuantity = salesInvoiceDetailGroup.BonusQuantity,
												 AvailableQuantity = salesInvoiceDetailGroup.Quantity - (stockReturned != null ? stockReturned.QuantityReturned : 0),
												 AvailableBonusQuantity = salesInvoiceDetailGroup.BonusQuantity - (stockReturned != null ? stockReturned.BonusQuantityReturned : 0),
												 SellingPrice = salesInvoiceDetail.SellingPrice,
												 ItemDiscountPercent = salesInvoiceDetail.ItemDiscountPercent,
												 VatPercent = salesInvoiceDetail.VatPercent,
												 Notes = salesInvoiceDetail.Notes,
												 ItemNote = salesInvoiceDetail.ItemNote,
										         VatTaxId = salesInvoiceDetail.VatTaxId,
										         VatTaxTypeId = salesInvoiceDetail.VatTaxTypeId,
												 ConsumerPrice = item.ConsumerPrice,
												 CostPrice = itemCost != null ? itemCost.CostPrice : 0,
												 CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
												 LastSalesPrice = lastSalesPrice != null ? lastSalesPrice.SellingPrice : 0
											 }).ToList();

			return salesInvoiceReturnDetails;
		}

		private SalesInvoiceReturnHeaderDto GetSalesInvoiceReturnHeaderFromSalesInvoice(List<SalesInvoiceReturnDetailDto> salesInvoiceReturnDetails, SalesInvoiceHeaderDto salesInvoiceHeader, bool isOnTheWay, bool isDirectInvoice)
		{
			var totalValueFromDetail = salesInvoiceReturnDetails.Sum(x => x.TotalValue);
			var totalValueAfterDiscountFromDetail = salesInvoiceReturnDetails.Sum(x => x.TotalValueAfterDiscount);
			var totalItemDiscount = salesInvoiceReturnDetails.Sum(x => x.ItemDiscountValue);
			var grossValueFromDetail = salesInvoiceReturnDetails.Sum(x => x.GrossValue);
			var vatValueFromDetail = salesInvoiceReturnDetails.Sum(x => x.VatValue);
			var subNetValueFromDetail = salesInvoiceReturnDetails.Sum(x => x.SubNetValue);
			var otherTaxValueFromDetail = salesInvoiceReturnDetails.Sum(x => x.OtherTaxValue);
			var netValueFromDetail = salesInvoiceReturnDetails.Sum(x => x.NetValue);
			var totalCostValueFromDetail = salesInvoiceReturnDetails.Sum(x => x.CostValue);

			return new SalesInvoiceReturnHeaderDto
			{
				SalesInvoiceReturnHeaderId = 0,
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
				IsDirectInvoice = isDirectInvoice,
				CreditPayment = salesInvoiceHeader.CreditPayment,
				TaxTypeId = salesInvoiceHeader.TaxTypeId,
				ShippingDate = salesInvoiceHeader.ShippingDate,
				DeliveryDate = salesInvoiceHeader.DeliveryDate,
				DueDate = salesInvoiceHeader.DueDate,
				ShipmentTypeId = salesInvoiceHeader.ShipmentTypeId,
				ShipmentTypeName = salesInvoiceHeader.ShipmentTypeName,
				CashClientName = salesInvoiceHeader.CashClientName,
				ClientPhone = salesInvoiceHeader.ClientPhone,
				ClientAddress = salesInvoiceHeader.ClientAddress,
				ClientTaxCode = salesInvoiceHeader.ClientTaxCode,
				DriverName = salesInvoiceHeader.DriverName,
				DriverPhone = salesInvoiceHeader.DriverPhone,
				ClientResponsibleName = salesInvoiceHeader.ClientResponsibleName,
				ClientResponsiblePhone = salesInvoiceHeader.ClientResponsiblePhone,
				ShipTo = salesInvoiceHeader.ShipTo,
				BillTo = salesInvoiceHeader.BillTo,
				ShippingRemarks = salesInvoiceHeader.ShippingRemarks,
				TotalValue = totalValueFromDetail,
				DiscountPercent = salesInvoiceHeader.DiscountPercent,
				DiscountValue = CalculateHeaderValue.DiscountValue(totalValueAfterDiscountFromDetail, salesInvoiceHeader.DiscountPercent),
				TotalItemDiscount = totalItemDiscount,
				GrossValue = grossValueFromDetail,
				VatValue = vatValueFromDetail,
				SubNetValue = subNetValueFromDetail,
				OtherTaxValue = otherTaxValueFromDetail,
				NetValueBeforeAdditionalDiscount = netValueFromDetail,
				AdditionalDiscountValue = salesInvoiceHeader.AdditionalDiscountValue,
				NetValue = CalculateHeaderValue.NetValue(netValueFromDetail, salesInvoiceHeader.AdditionalDiscountValue),
				TotalCostValue = totalCostValueFromDetail,
				DebitAccountId = salesInvoiceHeader.CreditAccountId,
				CreditAccountId = salesInvoiceHeader.DebitAccountId,
				JournalHeaderId = 0,
				RemarksAr = salesInvoiceHeader.RemarksAr,
				RemarksEn = salesInvoiceHeader.RemarksEn,
				IsOnTheWay = isOnTheWay,
				IsClosed = false,
				IsBlocked = false,
				IsEnded = false,
				ArchiveHeaderId = salesInvoiceHeader.ArchiveHeaderId,
			};
		}

		private List<SalesInvoiceReturnDetailDto> DistributeQuantityAndCalculateValues(List<SalesInvoiceReturnDetailDto> salesInvoiceReturnDetails, decimal headerDiscountPercent)
		{
			QuantityDistributionLogic.DistributeQuantitiesOnDetails(
				details: salesInvoiceReturnDetails,
				keySelector: x => (x.ItemId, x.ItemPackageId, x.BarCode, x.CostCenterId, x.SellingPrice, x.ItemDiscountPercent, x.ExpireDate, x.BatchNumber),
				availableQuantitySelector: x => x.KeyLevelQuantity,
				availableBonusQuantitySelector: x => x.KeyLevelBonusQuantity,
				quantitySelector: x => x.Quantity,
				bonusQuantitySelector: x => x.BonusQuantity,
				quantityAssigner: (x, value) => x.Quantity = value,
				bonusQuantityAssigner: (x, value) => x.BonusQuantity = value
			);

			RecalculateDetailValue.RecalculateDetailValues(
				details: salesInvoiceReturnDetails,
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

			return salesInvoiceReturnDetails;
		}

		private async Task GetAuxiliaryData(List<SalesInvoiceReturnDetailDto> salesInvoiceReturnDetails)
		{
			var itemIds = salesInvoiceReturnDetails.Select(x => x.ItemId).ToList();
			var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
			var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

			foreach (var salesInvoiceReturnDetail in salesInvoiceReturnDetails)
			{
				salesInvoiceReturnDetail.Packages = packages.Where(x => x.ItemId == salesInvoiceReturnDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
				salesInvoiceReturnDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == salesInvoiceReturnDetail.ItemId).ToList();
				salesInvoiceReturnDetail.Taxes = salesInvoiceReturnDetail.ItemTaxData.ToJson();
			}
		}

		private async Task CalculateOtherTaxes(List<SalesInvoiceReturnDetailDto> salesInvoiceReturnDetails, decimal headerDiscountPercent, int salesInvoiceHeaderId)
		{
			var salesInvoiceDetailTaxes = await _salesInvoiceDetailTaxService.GetSalesInvoiceDetailTaxes(salesInvoiceHeaderId).ToListAsync();
			foreach (var salesInvoiceReturnDetail in salesInvoiceReturnDetails)
			{
				salesInvoiceReturnDetail.SalesInvoiceReturnDetailTaxes = (
					from itemTax in salesInvoiceDetailTaxes.Where(x =>
						x.SalesInvoiceDetailId == salesInvoiceReturnDetail.SalesInvoiceReturnDetailId)
					select new SalesInvoiceReturnDetailTaxDto
					{
						TaxId = itemTax.TaxId,
						TaxTypeId = itemTax.TaxTypeId,
						DebitAccountId = itemTax.CreditAccountId,
						TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
						TaxPercent = itemTax.TaxPercent,
						TaxValue = CalculateDetailValue.TaxValue(salesInvoiceReturnDetail.Quantity, salesInvoiceReturnDetail.SellingPrice,
							salesInvoiceReturnDetail.ItemDiscountPercent, salesInvoiceReturnDetail.VatPercent, itemTax.TaxPercent,
							itemTax.TaxAfterVatInclusive, headerDiscountPercent, salesInvoiceReturnDetail.IsItemVatInclusive)
					}
				).ToList();

				salesInvoiceReturnDetail.OtherTaxValue = salesInvoiceReturnDetail.SalesInvoiceReturnDetailTaxes.Sum(x => x.TaxValue);
				salesInvoiceReturnDetail.NetValue = CalculateDetailValue.NetValue(salesInvoiceReturnDetail.Quantity,
					salesInvoiceReturnDetail.SellingPrice, salesInvoiceReturnDetail.ItemDiscountPercent, salesInvoiceReturnDetail.VatPercent,
					salesInvoiceReturnDetail.OtherTaxValue, headerDiscountPercent, salesInvoiceReturnDetail.IsItemVatInclusive);
			}
		}

		public async Task<int> GetParentMenuCode(int salesInvoiceHeaderId)
		{
			var salesInvoiceHeader = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById(salesInvoiceHeaderId);
			return SalesInvoiceMenuCodeHelper.GetMenuCode(salesInvoiceHeader!);
		}

		private void SerializeSalesInvoiceReturnDetails(List<SalesInvoiceReturnDetailDto> salesInvoiceReturnDetails)
		{
			int newId = -1;
			int newSubId = -1;
			foreach (var salesInvoiceReturnDetail in salesInvoiceReturnDetails)
			{
				salesInvoiceReturnDetail.SalesInvoiceReturnDetailId = newId;

				salesInvoiceReturnDetail.SalesInvoiceReturnDetailTaxes.ForEach(y =>
				{
					y.SalesInvoiceReturnDetailId = newId;
					y.SalesInvoiceReturnDetailTaxId = newSubId--;
				});

				newId--;
			}
		}

		public async Task<ResponseDto> SaveSalesInvoiceReturn(SalesInvoiceReturnDto salesInvoiceReturn, bool hasApprove, bool approved, int? requestId, string? documentReference = null)
		{
			TrimDetailStrings(salesInvoiceReturn.SalesInvoiceReturnDetails);

			var menuCode = SalesInvoiceReturnMenuCodeHelper.GetMenuCode(salesInvoiceReturn.SalesInvoiceReturnHeader!);
			var parentMenuCode = await GetParentMenuCode(salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceHeaderId);
			var validationResult = await CheckSalesInvoiceReturnIsValidForSave(salesInvoiceReturn, menuCode, parentMenuCode);
			if (validationResult.Success == false)
			{
				validationResult.Id = salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId;
				return validationResult;
			}

			await UpdateModelData(salesInvoiceReturn);

			var saveResult = await SaveSalesInvoiceReturnItself(salesInvoiceReturn, hasApprove, approved, requestId, documentReference, validationResult);
			if (saveResult.Result.Success)
			{
				await ApplySalesInvoiceReturnSideEffectsOnSave(saveResult.Result.Id, salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceHeaderId, salesInvoiceReturn.SalesInvoiceReturnHeader!.IsOnTheWay, salesInvoiceReturn.SalesInvoiceReturnHeader!.IsDirectInvoice, salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId == 0, saveResult.StockOutReturnHeaderId);

				var settlementExccedResult = await _getSalesInvoiceSettleValueService.CheckSettlementExceedingAndUpdateSettlementCompletedFlag(salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceHeaderId, menuCode, parentMenuCode, true);
				if (settlementExccedResult.Success == false) return settlementExccedResult;
			}

			return saveResult.Result;
		}

		private async Task<SalesInvoiceReturnSaveResult> SaveSalesInvoiceReturnItself(SalesInvoiceReturnDto salesInvoiceReturn, bool hasApprove, bool approved, int? requestId, string? documentReference, ResponseDto result)
		{
			if (salesInvoiceReturn.SalesInvoiceReturnHeader!.IsDirectInvoice)
			{
				return await SaveDirectSalesInvoiceReturn(salesInvoiceReturn, hasApprove, approved, requestId);
			}
			else
			{
				return new SalesInvoiceReturnSaveResult { Result = await _salesInvoiceReturnService.SaveSalesInvoiceReturn(salesInvoiceReturn, hasApprove, approved, requestId, documentReference) };
			}
		}

		public async Task<ResponseDto> CheckSalesInvoiceReturnIsValidForSave(SalesInvoiceReturnDto salesInvoiceReturn, int menuCode, int parentMenuCode)
		{
			ResponseDto result;
			var salesInvoiceReturnHeader = salesInvoiceReturn.SalesInvoiceReturnHeader!;
			
			result = await CheckTaxTypeMisMatchWithSalesInvoice(salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, salesInvoiceReturnHeader.SalesInvoiceHeaderId, salesInvoiceReturnHeader.TaxTypeId);
			if (result.Success == false) return result;

			result = await CheckPaidValueTotalEqualNetValue(salesInvoiceReturnHeader.CreditPayment, salesInvoiceReturnHeader.NetValue, salesInvoiceReturnHeader.StoreId, salesInvoiceReturn.SalesInvoiceReturnPayments, menuCode);
            if (result.Success == false)  return result;

            result = await CheckSalesInvoiceReturnFlagsAndConsistencyWithParent(salesInvoiceReturnHeader);
			if (result.Success == false) return result;

			result = await CheckSalesInvoiceReturnClosed(salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, menuCode);
			if (result.Success == false) return result;

			result = await CheckSalesInvoiceReturnJournalMismatch(salesInvoiceReturnHeader, salesInvoiceReturn.SalesInvoiceReturnPayments, salesInvoiceReturn.Journal?.JournalHeader, menuCode, salesInvoiceReturnHeader.CreditPayment);
			if (result.Success == false) return result;

			result = await CheckSalesInvoiceBlocked(salesInvoiceReturnHeader.SalesInvoiceHeaderId, menuCode);
			if (result.Success == false) return result;

			result = await CheckSalesInvoiceCanReturn(salesInvoiceReturnHeader.SalesInvoiceHeaderId, parentMenuCode, salesInvoiceReturnHeader.IsDirectInvoice, salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId == 0);
			if (result.Success == false) return result;

			if (salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId == 0)
			{
				if (salesInvoiceReturnHeader.IsOnTheWay)
				{
					result = await CheckReservationInvoiceAlreadyHasReservationInvoiceCloseOut(salesInvoiceReturnHeader.SalesInvoiceHeaderId, menuCode, parentMenuCode);
					if (result.Success == false) return result;
				}
			}

			result = await CheckSalesInvoiceHasClientCreditOrDebitMemo(salesInvoiceReturnHeader.SalesInvoiceHeaderId, menuCode, parentMenuCode);
			if (result.Success == false) return result;

			result = await _itemNoteValidationService.CheckItemNoteWithItemType(salesInvoiceReturn.SalesInvoiceReturnDetails, x => x.ItemId, x => x.ItemNote);
			if (result.Success == false) return result;

			result = await CheckSalesInvoiceReturnQuantity(salesInvoiceReturn, menuCode, parentMenuCode);
			if (result.Success == false) return result;

			result = await CheckSalesInvoiceReturnZeroStock(salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, salesInvoiceReturnHeader.StoreId, salesInvoiceReturn.SalesInvoiceReturnDetails, menuCode, parentMenuCode, true);
			if (result.Success == false) return result;

			return result;
		}

		private async Task<ResponseDto> CheckPaidValueTotalEqualNetValue(bool isCreditPayment, decimal netValue, int storeId, List<SalesInvoiceReturnPaymentDto> payments, int menuCode)
		{
			var paidTotal = payments.Sum(x => x.PaidValue);
			var rounding = await _storeService.GetStoreRounding(storeId);

			var roundedNetValue = NumberHelper.RoundNumber(netValue, rounding);
			var roundedPaidValue = NumberHelper.RoundNumber(paidTotal, rounding);

			if (!isCreditPayment)
			{
				if (roundedNetValue != roundedPaidValue)
				{
					return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(menuCode, SalesMessageData.PaidValueNotMatchTotalValue, roundedPaidValue.ToNormalizedString(), roundedNetValue.ToNormalizedString()) };
				}
			}
			else if (roundedPaidValue != 0)
			{
				return new ResponseDto { Success = false, Message = _salesMessageService.GetMessage(SalesMessageData.NoPaymentMethodWithCreditInvoices) };
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckSalesInvoiceReturnFlagsAndConsistencyWithParent(SalesInvoiceReturnHeaderDto salesInvoiceReturnHeader)
		{
			if (salesInvoiceReturnHeader.IsDirectInvoice && salesInvoiceReturnHeader.IsOnTheWay)
			{
				return new ResponseDto { Success = false, Message = "IsDirectInvoice and IsOnTheWay cannot both be set to true at the same time" };
			}

			var salesInvoiceHeader = await _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceReturnHeader.SalesInvoiceHeaderId).Select(x => new { x.IsOnTheWay, x.IsDirectInvoice }).FirstOrDefaultAsync();

			if ((!salesInvoiceHeader!.IsDirectInvoice || salesInvoiceHeader!.IsOnTheWay) && salesInvoiceReturnHeader.IsDirectInvoice)
			{
				return new ResponseDto { Success = false, Message = "Direct invoice return can only be created on direct invoice" };
			}

			if (!salesInvoiceHeader!.IsOnTheWay && salesInvoiceReturnHeader.IsOnTheWay)
			{
				return new ResponseDto { Success = false, Message = "You can create reservation invoice close out only from reservation invoice" };
			}

			if (salesInvoiceHeader!.IsDirectInvoice && !salesInvoiceHeader.IsOnTheWay && !salesInvoiceReturnHeader.IsDirectInvoice)
			{
				return new ResponseDto { Success = false, Message = "SalesInvoiceReturn from direct invoice must also be direct" };
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckSalesInvoiceReturnClosed(int salesInvoiceReturnHeaderId, int menuCode)
		{
			if (salesInvoiceReturnHeaderId != 0)
			{
				var isClosed = await _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeaderId).Select(x => (bool?)x.IsClosed).FirstOrDefaultAsync();
				if (isClosed == null)
				{
					return new ResponseDto { Id = salesInvoiceReturnHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };
				}

				if (isClosed == true)
				{
					return new ResponseDto { Id = salesInvoiceReturnHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotUpdateBecauseClosed) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckSalesInvoiceReturnJournalMismatch(SalesInvoiceReturnHeaderDto salesInvoiceReturnHeader, List<SalesInvoiceReturnPaymentDto> payments, JournalHeaderDto? journalHeader, int menuCode, bool isCredit)
		{
			//GetStoreRounding instead of header rounding to handle payment methods
			int rounding = await _storeService.GetStoreRounding(salesInvoiceReturnHeader.StoreId);

			decimal journalCreditValue = NumberHelper.RoundNumber(journalHeader?.TotalCreditValue ?? 0, rounding);
			decimal invoiceValue = NumberHelper.RoundNumber((isCredit ? salesInvoiceReturnHeader.NetValue : payments.Sum(x => x.PaidValue)) + salesInvoiceReturnHeader.AdditionalDiscountValue, rounding);

			if (journalCreditValue != invoiceValue)
			{
				return new ResponseDto { Success = false, Id = salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, Message = await _salesMessageService.GetMessage(menuCode, SalesMessageData.ValueNotMatchingJournalCredit) };
			}
			else
			{
				return new ResponseDto { Success = true };
			}
		}

		private async Task<ResponseDto> CheckSalesInvoiceBlocked(int salesInvoiceHeaderId, int menuCode)
		{
			var salesInvoiceHeader = await _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => new { x.IsBlocked }).FirstOrDefaultAsync();

			if (salesInvoiceHeader?.IsBlocked == true)
			{
				return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckSalesInvoiceCanReturn(int salesInvoiceHeaderId, int parentMenuCode, bool isDirectInvoice, bool isCreate)
		{
			if (isCreate && isDirectInvoice)
			{
				var canReturnUntil = (await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById((int)salesInvoiceHeaderId))!.CanReturnUntilDate;

				if (canReturnUntil.Date < DateHelper.GetDateTimeNow().Date)
				{
					return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(parentMenuCode, SalesMessageData.NoLongerCanReturn) };
				}
			}
			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckReservationInvoiceAlreadyHasReservationInvoiceCloseOut(int reservationInvoiceHeaderId, int menuCode, int parentMenuCode)
		{
			var alreadyHasSalesInvoiceReturn = await _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == reservationInvoiceHeaderId && x.IsOnTheWay).AnyAsync();

			if (alreadyHasSalesInvoiceReturn)
			{
				return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.AlreadyHasDocument) };
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

		private async Task<ResponseDto> CheckTaxTypeMisMatchWithSalesInvoice(int salesInvoiceReturnHeaderId, int salesInvoiceHeaderId, int taxTypeId)
		{
			var salesInvoiceTaxType = await _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.TaxTypeId).FirstOrDefaultAsync();

			if (salesInvoiceTaxType != taxTypeId)
			{
				return new ResponseDto { Id = salesInvoiceReturnHeaderId, Success = false, Message = "TaxType of Purchase Invoice Return must match that of the Purchase Invoice" };
			}

			return new ResponseDto { Success = true };
		}

		private async Task<SalesInvoiceReturnSaveResult> SaveDirectSalesInvoiceReturn(SalesInvoiceReturnDto salesInvoiceReturn, bool hasApprove, bool isApproved, int? requestId)
		{
			var documentReference = await GetDirectSalesInvoiceReturnDocumentReference(salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId, hasApprove, requestId, salesInvoiceReturn.SalesInvoiceReturnHeader!.CreditPayment);

			var stockOutReturnResult = await SaveRelatedStockOutReturn(salesInvoiceReturn, hasApprove, isApproved, requestId, documentReference);
			if (stockOutReturnResult.Success == false) return new SalesInvoiceReturnSaveResult { Result = stockOutReturnResult };

			var salesInvoiceReturnResult = await _salesInvoiceReturnService.SaveSalesInvoiceReturn(salesInvoiceReturn, hasApprove, isApproved, requestId, documentReference);
			return new SalesInvoiceReturnSaveResult
			{
				Result = salesInvoiceReturnResult,
				SalesInvoiceReturnHeaderId = salesInvoiceReturnResult.Id,
				StockOutReturnHeaderId = stockOutReturnResult.Id
			};
		}

		private async Task<string?> GetDirectSalesInvoiceReturnDocumentReference(int salesInvoiceReturnHeaderId, bool hasApprove, int? requestId, bool isCreditPayment)
		{
			if (salesInvoiceReturnHeaderId != 0)
			{
				return null;
			}
			else if (hasApprove)
			{
				return $"{DocumentReferenceData.Approval}{requestId}";
			}
			else
			{
				return $"{SalesInvoiceReturnMenuCodeHelper.GetDocumentReference(false, true, isCreditPayment)}{await _salesInvoiceReturnHeaderService.GetNextId()}";
			}
		}

		private async Task<ResponseDto> SaveRelatedStockOutReturn(SalesInvoiceReturnDto salesInvoiceReturn, bool hasApprove, bool approved, int? requestId, string? documentReference)
		{
			var salesInvoiceReturnHeader = salesInvoiceReturn.SalesInvoiceReturnHeader!;
			var stockOutReturnHeaderId = await GetStockOutReturnHeaderIdLinkedToSalesInvoiceReturn(salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId);

			var stockOutReturn = new StockOutReturnDto
			{
				StockOutReturnHeader = new StockOutReturnHeaderDto
				{
					StockOutReturnHeaderId = stockOutReturnHeaderId,
					StockTypeId = StockTypeData.StockOutReturnFromSalesInvoice,
					SalesInvoiceHeaderId = salesInvoiceReturnHeader.SalesInvoiceHeaderId,
					ClientId = salesInvoiceReturnHeader.ClientId,
					StoreId = salesInvoiceReturnHeader.StoreId,
					DocumentDate = salesInvoiceReturnHeader.DocumentDate,
					Reference = salesInvoiceReturnHeader.Reference,
					TotalValue = salesInvoiceReturnHeader.TotalValue,
					DiscountPercent = salesInvoiceReturnHeader.DiscountPercent,
					DiscountValue = salesInvoiceReturnHeader.DiscountValue,
					TotalItemDiscount = salesInvoiceReturnHeader.TotalItemDiscount,
					GrossValue = salesInvoiceReturnHeader.GrossValue,
					VatValue = salesInvoiceReturnHeader.VatValue,
					SubNetValue = salesInvoiceReturnHeader.SubNetValue,
					OtherTaxValue = salesInvoiceReturnHeader.OtherTaxValue,
					NetValueBeforeAdditionalDiscount = salesInvoiceReturnHeader.NetValueBeforeAdditionalDiscount,
					AdditionalDiscountValue = salesInvoiceReturnHeader.AdditionalDiscountValue,
					NetValue = salesInvoiceReturnHeader.NetValue,
					TotalCostValue = salesInvoiceReturnHeader.TotalCostValue,
					RemarksAr = salesInvoiceReturnHeader.RemarksAr,
					RemarksEn = salesInvoiceReturnHeader.RemarksEn,
					IsClosed = false,
					IsEnded = true,
					ArchiveHeaderId = salesInvoiceReturnHeader.ArchiveHeaderId,
				},
				StockOutReturnDetails = (
					from salesInvoiceReturnDetail in salesInvoiceReturn.SalesInvoiceReturnDetails
					select new StockOutReturnDetailDto
					{
						StockOutReturnDetailId = 0,
						StockOutReturnHeaderId = 0,
						ItemId = salesInvoiceReturnDetail.ItemId,
						ItemPackageId = salesInvoiceReturnDetail.ItemPackageId,
						IsItemVatInclusive = salesInvoiceReturnDetail.IsItemVatInclusive,
						CostCenterId = salesInvoiceReturnDetail.CostCenterId,
						BarCode = salesInvoiceReturnDetail.BarCode,
						Packing = salesInvoiceReturnDetail.Packing,
						ExpireDate = salesInvoiceReturnDetail.ExpireDate,
						BatchNumber = salesInvoiceReturnDetail.BatchNumber,
						Quantity = salesInvoiceReturnDetail.Quantity,
						BonusQuantity = salesInvoiceReturnDetail.BonusQuantity,
						SellingPrice = salesInvoiceReturnDetail.SellingPrice,
						TotalValue = salesInvoiceReturnDetail.TotalValue,
						ItemDiscountPercent = salesInvoiceReturnDetail.ItemDiscountPercent,
						ItemDiscountValue = salesInvoiceReturnDetail.ItemDiscountValue,
						TotalValueAfterDiscount = salesInvoiceReturnDetail.TotalValueAfterDiscount,
						HeaderDiscountValue = salesInvoiceReturnDetail.HeaderDiscountValue,
						GrossValue = salesInvoiceReturnDetail.GrossValue,
						VatPercent = salesInvoiceReturnDetail.VatPercent,
						VatValue = salesInvoiceReturnDetail.VatValue,
						SubNetValue = salesInvoiceReturnDetail.SubNetValue,
						OtherTaxValue = salesInvoiceReturnDetail.OtherTaxValue,
						NetValue = salesInvoiceReturnDetail.NetValue,
						Notes = salesInvoiceReturnDetail.Notes,
						ItemNote = salesInvoiceReturnDetail.ItemNote,
						ConsumerPrice = salesInvoiceReturnDetail.ConsumerPrice,
						CostPrice = salesInvoiceReturnDetail.CostPrice,
						CostPackage = salesInvoiceReturnDetail.CostPackage,
						CostValue = salesInvoiceReturnDetail.CostValue,
						LastSalesPrice = salesInvoiceReturnDetail.LastSalesPrice,
						StockOutReturnDetailTaxes = (
							from salesInvoiceReturnDetailTax in salesInvoiceReturnDetail.SalesInvoiceReturnDetailTaxes
							select new StockOutReturnDetailTaxDto
							{
								TaxId = salesInvoiceReturnDetailTax.TaxId,
								DebitAccountId = salesInvoiceReturnDetailTax.DebitAccountId,
								TaxAfterVatInclusive = salesInvoiceReturnDetailTax.TaxAfterVatInclusive,
								TaxPercent = salesInvoiceReturnDetailTax.TaxPercent,
								TaxValue = salesInvoiceReturnDetailTax.TaxValue
							}
						).ToList()
					}).ToList()
			};

			return await _stockOutReturnService.SaveStockOutReturn(stockOutReturn, hasApprove, approved, requestId, documentReference, true);
		}

		private async Task<int> GetStockOutReturnHeaderIdLinkedToSalesInvoiceReturn(int salesInvoiceReturnHeaderId)
		{
			if (salesInvoiceReturnHeaderId == 0)
			{
				return 0;
			}
			else
			{
				return await _invoiceStockOutReturnService.GetStockOutReturnsLinkedToSalesInvoiceReturn(salesInvoiceReturnHeaderId).FirstAsync();
			}
		}

		private async Task ApplySalesInvoiceReturnSideEffectsOnSave(int salesInvoiceReturnHeaderId, int salesInvoiceHeaderId, bool isOnTheWay, bool isDirectInvoice, bool isCreate, int? createdStockOutReturnHeaderId)
		{
			if (isCreate)
			{
				await _salesInvoiceHeaderService.UpdateEndedAndClosed(salesInvoiceHeaderId, isOnTheWay, true);

				await UpdateProformaInvoiceStatus(salesInvoiceHeaderId, true);
				await MarkStockOutAndStockOutReturnsAsInvoiced(salesInvoiceReturnHeaderId, salesInvoiceHeaderId, isOnTheWay, isDirectInvoice, createdStockOutReturnHeaderId);
			}
		}

		private async Task MarkStockOutAndStockOutReturnsAsInvoiced(int salesInvoiceReturnHeaderId, int salesInvoiceHeaderId, bool isOnTheWay, bool isDirectInvoice, int? createdStockOutReturnHeaderId)
		{
			if (isOnTheWay)
			{
				var stockOuts = await GetStockOutsLinkedToSalesInvoice(salesInvoiceHeaderId);
				var stockOutReturns = await GetStockOutReturnLinkedToSalesInvoiceThroughStockOuts(salesInvoiceHeaderId);

				await _stockOutHeaderService.UpdateStockOutsEnded(stockOuts, true);
				await _stockOutReturnHeaderService.UpdateStockOutReturnsEnded(stockOutReturns, true);
			}
			else if(isDirectInvoice)
			{
				await _invoiceStockOutReturnService.LinkStockOutReturnsToSalesInvoiceReturn(salesInvoiceReturnHeaderId, (List<int>)[(int)createdStockOutReturnHeaderId!]);
			}
			else
			{
				var stockOutReturnHeaderIds = await GetUninvoicedStockOutReturnsFromSalesInvoice(salesInvoiceHeaderId);
				await _stockOutReturnHeaderService.UpdateStockOutReturnsEnded(stockOutReturnHeaderIds, true);
				await _invoiceStockOutReturnService.LinkStockOutReturnsToSalesInvoiceReturn(salesInvoiceReturnHeaderId, stockOutReturnHeaderIds);
			}
		}

		private async Task MarkStockOutsAndStockOutReturnsAsUnInvoiced(int salesInvoiceReturnHeaderId, int salesInvoiceHeaderId, bool isOnTheWay, bool isDirectInvoice)
		{
			if (isOnTheWay)
			{
				var stockOuts = await GetStockOutsLinkedToSalesInvoice(salesInvoiceHeaderId);
				var stockOutReturns = await GetStockOutReturnLinkedToSalesInvoiceThroughStockOuts(salesInvoiceHeaderId);

				await _stockOutHeaderService.UpdateStockOutsEnded(stockOuts, false);
				await _stockOutReturnHeaderService.UpdateStockOutReturnsEnded(stockOutReturns, false);
			}
			else
			{
				if (!isDirectInvoice)
				{
					var stockOutReturnHeaderIds = await _invoiceStockOutReturnService.GetStockOutReturnsLinkedToSalesInvoiceReturn(salesInvoiceReturnHeaderId).ToListAsync();
					await _stockOutReturnHeaderService.UpdateStockOutReturnsEnded(stockOutReturnHeaderIds, false);
				}
				await _invoiceStockOutReturnService.UnlinkStockOutReturnsFromSalesInvoiceReturn(salesInvoiceReturnHeaderId);
			}
		}

		private async Task UpdateProformaInvoiceStatus(int salesInvoiceHeaderId, bool isSave)
		{
			var proformaInvoiceHeaderId = await GetRelatedProformaInvoice(salesInvoiceHeaderId);
			await _proformaInvoiceStatusService.UpdateProformaInvoiceStatus(proformaInvoiceHeaderId, isSave ? DocumentStatusData.SalesReturnInvoiceCreated : -1);
		}

		private async Task<int> GetRelatedProformaInvoice(int salesInvoiceHeaderId)
		{
			return (await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById(salesInvoiceHeaderId))!.ProformaInvoiceHeaderId;
		}

		private async Task<List<int>> GetStockOutsLinkedToSalesInvoice(int salesInvoiceHeaderId)
		{
			return await _stockOutHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.StockOutHeaderId).ToListAsync();
		}

		private async Task<List<int>> GetStockOutReturnLinkedToSalesInvoiceThroughStockOuts(int salesInvoiceHeaderId)
		{
			return await (from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId)
						  from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)
						  select stockOutReturnHeader.StockOutReturnHeaderId).ToListAsync();
		}

		private async Task<List<int>> GetUninvoicedStockOutReturnsFromSalesInvoice(int salesInvoiceHeaderId)
		{
			return await _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId && !x.IsEnded).Select(x => x.StockOutReturnHeaderId).ToListAsync();
		}

		private void TrimDetailStrings(List<SalesInvoiceReturnDetailDto> salesInvoiceReturnDetails)
        {
            foreach (var salesInvoiceReturnDetail in salesInvoiceReturnDetails)
            {
                salesInvoiceReturnDetail.BatchNumber = string.IsNullOrWhiteSpace(salesInvoiceReturnDetail.BatchNumber) ? null : salesInvoiceReturnDetail.BatchNumber.Trim();
            }
        }

        private async Task UpdateModelData(SalesInvoiceReturnDto salesInvoiceReturn)
        {
            await UpdateDetailPrices(salesInvoiceReturn.SalesInvoiceReturnDetails, salesInvoiceReturn.SalesInvoiceReturnHeader!.StoreId, salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId == 0);
            salesInvoiceReturn.SalesInvoiceReturnHeader!.TotalCostValue = salesInvoiceReturn.SalesInvoiceReturnDetails.Sum(x => x.CostValue);
			await AddCostValueJournalsToSalesInvoiceReturn(salesInvoiceReturn);
        }

        private async Task AddCostValueJournalsToSalesInvoiceReturn(SalesInvoiceReturnDto salesInvoiceReturn)
        {
            var companyId = await _storeService.GetCompanyIdByStoreId(salesInvoiceReturn.SalesInvoiceReturnHeader!.StoreId);
			var inventoryAccountId = await _accountService.GetAll().Where(x => x.CompanyId == companyId && x.AccountTypeId == AccountTypeData.InventoryAccount).Select(x => x.AccountId).FirstOrDefaultAsync();
			var revenueCostAccountId = await _accountService.GetAll().Where(x => x.CompanyId == companyId && x.AccountTypeId == AccountTypeData.RevenuesCostAccount).Select(x => x.AccountId).FirstOrDefaultAsync();

            if (salesInvoiceReturn.SalesInvoiceReturnHeader?.SalesInvoiceReturnHeaderId == 0)
            {
                salesInvoiceReturn.Journal?.JournalDetails?.Add(new JournalDetailDto
                {
                    JournalDetailId = 0,
                    JournalHeaderId = 0,
                    Serial = 0,
                    AccountId = revenueCostAccountId,
                    CurrencyId = salesInvoiceReturn.Journal.JournalDetails[0].CurrencyId, //Assuming all details have the same currency, currency rate etc.
                    CurrencyRate = salesInvoiceReturn.Journal.JournalDetails[0].CurrencyRate,
                    DebitValue = 0,
                    DebitValueAccount = 0,
                    CreditValue = salesInvoiceReturn.SalesInvoiceReturnHeader!.TotalCostValue,
                    CreditValueAccount = salesInvoiceReturn.SalesInvoiceReturnHeader!.TotalCostValue * salesInvoiceReturn.Journal.JournalDetails[0].CurrencyRate,
                    IsTax = false,
                    TaxId = null,
                    TaxTypeId = null,
                    TaxParentId = null,
                    TaxPercent = 0,
                    RemarksAr = null,
                    RemarksEn = null,
                    AutomaticRemarks = "CostValueCredit",
                    EntityTypeId = null,
                    EntityTypeName = null,
                    EntityId = null,
                    EntityNameAr = null,
                    EntityNameEn = null,
                    TaxCode = null,
                    EntityEmail = null,
                    TaxReference = null,
                    TaxDate = null,
                    IsLinkedToCostCenters = false,
                    IsCostCenterDistributed = false,
                    IsSystematic = true,
                    IsCostRelated = true,
                });

                salesInvoiceReturn.Journal?.JournalDetails?.Add(new JournalDetailDto
                {
                    JournalDetailId = 0,
                    JournalHeaderId = 0,
                    Serial = 0,
                    AccountId = inventoryAccountId,
                    CurrencyId = salesInvoiceReturn.Journal.JournalDetails[0].CurrencyId,
                    CurrencyRate = salesInvoiceReturn.Journal.JournalDetails[0].CurrencyRate,
                    DebitValue = salesInvoiceReturn.SalesInvoiceReturnHeader!.TotalCostValue,
                    DebitValueAccount = salesInvoiceReturn.SalesInvoiceReturnHeader!.TotalCostValue * salesInvoiceReturn.Journal.JournalDetails[0].CurrencyRate,
                    CreditValue = 0,
                    CreditValueAccount = 0,
                    IsTax = false,
                    TaxId = null,
                    TaxTypeId = null,
                    TaxParentId = null,
                    TaxPercent = 0,
                    RemarksAr = null,
                    RemarksEn = null,
                    AutomaticRemarks = "CostValueDebit",
                    EntityTypeId = null,
                    EntityTypeName = null,
                    EntityId = null,
                    EntityNameAr = null,
                    EntityNameEn = null,
                    TaxCode = null,
                    EntityEmail = null,
                    TaxReference = null,
                    TaxDate = null,
                    IsLinkedToCostCenters = false,
                    IsCostCenterDistributed = false,
                    IsSystematic = true,
                    IsCostRelated = true,
                });
            }
            else
            {
                foreach (var journalDetail in salesInvoiceReturn.Journal?.JournalDetails!)
                {
                    if (journalDetail.IsCostRelated)
                    {
                        journalDetail.CreditValue = journalDetail.AccountId == inventoryAccountId ? salesInvoiceReturn.SalesInvoiceReturnHeader!.TotalCostValue : 0;
                        journalDetail.DebitValue = journalDetail.AccountId == revenueCostAccountId ? salesInvoiceReturn.SalesInvoiceReturnHeader!.TotalCostValue : 0;
                        journalDetail.CreditValueAccount = journalDetail.CreditValue * journalDetail.CurrencyRate;
                        journalDetail.DebitValueAccount = journalDetail.DebitValue * journalDetail.CurrencyRate;
                    }
                }
            }
        }

        private async Task UpdateDetailPrices(List<SalesInvoiceReturnDetailDto> salesInvoiceReturnDetails, int storeId, bool isNew)
        {
            var itemIds = salesInvoiceReturnDetails.Select(x => x.ItemId).ToList();

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

            foreach (var salesInvoiceReturnDetail in salesInvoiceReturnDetails)
            {
                var packing = packings.Where(x => x.ItemId == salesInvoiceReturnDetail.ItemId && x.FromPackageId == salesInvoiceReturnDetail.ItemPackageId).Select(x => x.Packing).FirstOrDefault(1);

                if (isNew)
                {
                    salesInvoiceReturnDetail.ConsumerPrice = consumerPrices.Where(x => x.ItemId == salesInvoiceReturnDetail.ItemId).Select(x => x.ConsumerPrice).FirstOrDefault(0);
                    salesInvoiceReturnDetail.CostPrice = itemCosts.Where(x => x.ItemId == salesInvoiceReturnDetail.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefault(0);
                    salesInvoiceReturnDetail.CostPackage = salesInvoiceReturnDetail.CostPrice * packing;
                    salesInvoiceReturnDetail.LastSalesPrice = lastSellingPrices.Where(x => x.ItemId == salesInvoiceReturnDetail.ItemId && x.ItemPackageId == salesInvoiceReturnDetail.ItemPackageId).Select(x => x.SellingPrice).FirstOrDefault(0);
                }
                salesInvoiceReturnDetail.CostValue = salesInvoiceReturnDetail.CostPackage * (salesInvoiceReturnDetail.Quantity + salesInvoiceReturnDetail.BonusQuantity); //handle quantity changes
            }
        }

		public async Task<ResponseDto> CheckSalesInvoiceReturnQuantity(SalesInvoiceReturnDto salesInvoiceReturn, int menuCode, int parentMenuCode)
		{
			return (salesInvoiceReturn.SalesInvoiceReturnHeader!.IsDirectInvoice, salesInvoiceReturn.SalesInvoiceReturnHeader!.IsOnTheWay) switch {
					(false, true) => await CheckReservationInvoiceCloseOutQuantity(salesInvoiceReturn, menuCode, parentMenuCode),
					(false, false) => await CheckRegularSalesInvoiceReturnQuantity(salesInvoiceReturn, menuCode, parentMenuCode),
					(true, false) => await CheckDirectSalesInvoiceReturnQuantity(salesInvoiceReturn, menuCode, parentMenuCode),
					_ => new ResponseDto { Success = true }
				};
		}

		private async Task<ResponseDto> CheckSalesInvoiceReturnZeroStock(int salesInvoiceReturnHeaderId, int storeId, List<SalesInvoiceReturnDetailDto> returnDetails, int menuCode, int parentMenuCode, bool isSave)
		{
			//validation is only for direct sales invoice return and reservation close out
			if (salesInvoiceReturnHeaderId == 0 || menuCode == MenuCodeData.SalesInvoiceReturn) return new ResponseDto { Success = true };

			var oldReturnDetails = await _salesInvoiceReturnDetailService.GetSalesInvoiceReturnDetailsAsQueryable(salesInvoiceReturnHeaderId).ToListAsync();

			return await _zeroStockValidationService.ValidateZeroStockReturn(
					storeId: storeId,
					newDetails: returnDetails,
					oldDetails: oldReturnDetails,
					detailKeySelector: x => (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber),
					itemIdSelector: x => x.ItemId,
					quantitySelector: x => x.Quantity + x.BonusQuantity,
					availableBalanceKeySelector: x => (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber),
					isGrouped: false,
					menuCode: menuCode,
					settingMenuCode: parentMenuCode,
					isSave: isSave
				);
		}

		public async Task<ResponseDto> CheckReservationInvoiceCloseOutQuantity(SalesInvoiceReturnDto salesInvoiceReturn, int menuCode, int parentMenuCode)
		{
			var salesInvoiceReturnDetailsGrouped = _salesInvoiceReturnDetailService.GroupSalesInvoiceReturnDetails(salesInvoiceReturn.SalesInvoiceReturnDetails);
			var finalStocksDisbursed = await _stockOutQuantityService.GetFinalStocksDisbursedFromSalesInvoice(salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceHeaderId).ToListAsync();
			var salesInvoiceDetailsGrouped = await _salesInvoiceDetailService.GetSalesInvoiceDetailsGrouped(salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceHeaderId);

			var unmatchedQuantity = (from salesInvoiceReturnDetailGroup in salesInvoiceReturnDetailsGrouped
									 from salesInvoiceDetailGroup in salesInvoiceDetailsGrouped.Where(x => x.ItemId == salesInvoiceReturnDetailGroup.ItemId && x.ItemPackageId == salesInvoiceReturnDetailGroup.ItemPackageId && x.BatchNumber == salesInvoiceReturnDetailGroup.BatchNumber && x.ExpireDate == salesInvoiceReturnDetailGroup.ExpireDate && x.CostCenterId == salesInvoiceReturnDetailGroup.CostCenterId && x.BarCode == salesInvoiceReturnDetailGroup.BarCode && x.SellingPrice == salesInvoiceReturnDetailGroup.SellingPrice && x.ItemDiscountPercent == salesInvoiceReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
									 from finalStockDisbursed in finalStocksDisbursed.Where(x => x.ItemId == salesInvoiceReturnDetailGroup.ItemId && x.ItemPackageId == salesInvoiceReturnDetailGroup.ItemPackageId && x.BatchNumber == salesInvoiceReturnDetailGroup.BatchNumber && x.ExpireDate == salesInvoiceReturnDetailGroup.ExpireDate && x.CostCenterId == salesInvoiceReturnDetailGroup.CostCenterId && x.BarCode == salesInvoiceReturnDetailGroup.BarCode && x.SellingPrice == salesInvoiceReturnDetailGroup.SellingPrice && x.ItemDiscountPercent == salesInvoiceReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
									 select new
									 {
										 salesInvoiceReturnDetailGroup.ItemId,
										 salesInvoiceReturnDetailGroup.ItemPackageId,
										 SalesInvoiceReturnQuantity = salesInvoiceReturnDetailGroup.Quantity,
										 SalesInvoiceReturnBonusQuantity = salesInvoiceReturnDetailGroup.BonusQuantity,
										 RemainingQuantity = (salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.Quantity : 0) - (finalStockDisbursed != null ? finalStockDisbursed.QuantityDisbursed : 0),
										 RemainingBonusQuantity = (salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.BonusQuantity : 0) - (finalStockDisbursed != null ? finalStockDisbursed.BonusQuantityDisbursed : 0),
									 }).FirstOrDefault(x => x.RemainingQuantity != x.SalesInvoiceReturnQuantity || x.RemainingBonusQuantity != x.SalesInvoiceReturnBonusQuantity);

			if (unmatchedQuantity != null)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();

				var itemName = await _itemService.GetAll().Where(x => x.ItemId == unmatchedQuantity.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
				var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == unmatchedQuantity.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

				if (unmatchedQuantity.RemainingQuantity != unmatchedQuantity.SalesInvoiceReturnQuantity)
				{
					return new ResponseDto { Id = salesInvoiceReturn.SalesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.QuantityNotMatchingRemaining, itemName!, itemPackageName!, unmatchedQuantity.SalesInvoiceReturnQuantity.ToNormalizedString(), unmatchedQuantity.RemainingQuantity.ToNormalizedString()) };
				}
				else
				{
					return new ResponseDto { Id = salesInvoiceReturn.SalesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.BonusQuantityNotMatchingRemaining, itemName!, itemPackageName!, unmatchedQuantity.SalesInvoiceReturnBonusQuantity.ToNormalizedString(), unmatchedQuantity.RemainingBonusQuantity.ToNormalizedString()) };
				}
			}

			return new ResponseDto { Success = true };
		}

		public async Task<ResponseDto> CheckRegularSalesInvoiceReturnQuantity(SalesInvoiceReturnDto salesInvoiceReturn, int menuCode, int parentMenuCode)
		{
			return salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId == 0 ?
				await CheckNewlyCreatedRegularSalesInvoiceReturnQuantity(salesInvoiceReturn, menuCode, parentMenuCode) :
				await CheckQuantityDidnotChange(salesInvoiceReturn, menuCode);
		}

		private async Task<ResponseDto> CheckNewlyCreatedRegularSalesInvoiceReturnQuantity(SalesInvoiceReturnDto salesInvoiceReturn, int menuCode, int parentMenuCode)
		{
			var salesInvoiceReturnDetailsGrouped = _salesInvoiceReturnDetailService.GroupSalesInvoiceReturnDetails(salesInvoiceReturn.SalesInvoiceReturnDetails);
			var uninvoicedStocksReturned = await _stockOutQuantityService.GetUnInvoicedStocksDisbursedReturnedFromSalesInvoice(salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceHeaderId).ToListAsync();

			var unmatchedQuantity = (from salesInvoiceReturnDetailGroup in salesInvoiceReturnDetailsGrouped
									 from uninvoicedStockDisbursed in uninvoicedStocksReturned.Where(x => x.ItemId == salesInvoiceReturnDetailGroup.ItemId && x.ItemPackageId == salesInvoiceReturnDetailGroup.ItemPackageId && x.BatchNumber == salesInvoiceReturnDetailGroup.BatchNumber && x.ExpireDate == salesInvoiceReturnDetailGroup.ExpireDate && x.CostCenterId == salesInvoiceReturnDetailGroup.CostCenterId && x.BarCode == salesInvoiceReturnDetailGroup.BarCode && x.SellingPrice == salesInvoiceReturnDetailGroup.SellingPrice && x.ItemDiscountPercent == salesInvoiceReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
									 select new
									 {
										 salesInvoiceReturnDetailGroup.ItemId,
										 salesInvoiceReturnDetailGroup.ItemPackageId,
										 SalesInvoiceReturnQuantity = salesInvoiceReturnDetailGroup.Quantity,
										 SalesInvoiceReturnBonusQuantity = salesInvoiceReturnDetailGroup.BonusQuantity,
										 UnInvoicedQuantity = (uninvoicedStockDisbursed != null ? uninvoicedStockDisbursed.QuantityReturned : 0),
										 UnInvoicedBonusQuantity = (uninvoicedStockDisbursed != null ? uninvoicedStockDisbursed.BonusQuantityReturned : 0),
									 }).FirstOrDefault(x => x.UnInvoicedQuantity != x.SalesInvoiceReturnQuantity || x.UnInvoicedBonusQuantity != x.SalesInvoiceReturnBonusQuantity);

			if (unmatchedQuantity != null)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();

				var itemName = await _itemService.GetAll().Where(x => x.ItemId == unmatchedQuantity.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
				var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == unmatchedQuantity.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

				if (unmatchedQuantity.UnInvoicedQuantity != unmatchedQuantity.SalesInvoiceReturnQuantity)
				{
					return new ResponseDto { Id = salesInvoiceReturn.SalesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.QuantityNotMatchingUnInvoiced, itemName!, itemPackageName!, unmatchedQuantity.SalesInvoiceReturnQuantity.ToNormalizedString(), unmatchedQuantity.UnInvoicedQuantity.ToNormalizedString()) };
				}
				else
				{
					return new ResponseDto { Id = salesInvoiceReturn.SalesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.QuantityNotMatchingUnInvoiced, itemName!, itemPackageName!, unmatchedQuantity.SalesInvoiceReturnBonusQuantity.ToNormalizedString(), unmatchedQuantity.UnInvoicedBonusQuantity.ToNormalizedString()) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckQuantityDidnotChange(SalesInvoiceReturnDto salesInvoiceReturn, int menuCode)
		{
			List<SalesInvoiceReturnDetailDto> salesInvoiceReturnDetails = await _salesInvoiceReturnDetailService.GetSalesInvoiceReturnDetails(salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId);
			var quantityChanged = (from newSalesInvoiceReturnDetail in salesInvoiceReturn.SalesInvoiceReturnDetails
								   from oldSalesInvoiceReturnDetail in salesInvoiceReturnDetails.Where(x => x.SalesInvoiceReturnDetailId == newSalesInvoiceReturnDetail.SalesInvoiceReturnDetailId && (x.Quantity != newSalesInvoiceReturnDetail.Quantity || x.BonusQuantity != newSalesInvoiceReturnDetail.BonusQuantity))
								   select oldSalesInvoiceReturnDetail).Any();

			return quantityChanged ?
				new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(menuCode, SalesMessageData.QuantityCannotBeChanged) } :
				new ResponseDto { Success = true };
		}

		public async Task<ResponseDto> CheckDirectSalesInvoiceReturnQuantity(SalesInvoiceReturnDto salesInvoiceReturn, int menuCode, int parentMenuCode)
		{
			var salesInvoiceReturnDetailsGrouped = _salesInvoiceReturnDetailService.GroupSalesInvoiceReturnDetails(salesInvoiceReturn.SalesInvoiceReturnDetails);

			List<StockDisbursedReturnedDto> stocksReturned;
			if (salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId == 0)
			{
				stocksReturned = await _stockOutQuantityService.GetStocksDisbursedReturnedFromSalesInvoice(salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceHeaderId).ToListAsync();
			}
			else
			{
				var relatedStockOutReturnHeaderId = await GetStockOutReturnHeaderIdLinkedToSalesInvoiceReturn(salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId);
				stocksReturned = await _stockOutQuantityService.GetStocksDisbursedReturnedFromSalesInvoiceExceptStockOutReturnHeaderId(salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceHeaderId, relatedStockOutReturnHeaderId).ToListAsync();
			}

			var salesInvoiceDetailsGrouped = await _salesInvoiceDetailService.GetSalesInvoiceDetailsGrouped(salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceHeaderId);

			//Since direct invoice returns cannot be created on reservation invoice, no need to subtract close out quantities here

			var exceedingQuantity = (from salesInvoiceReturnDetailGroup in salesInvoiceReturnDetailsGrouped
									 from salesInvoiceDetailGroup in salesInvoiceDetailsGrouped.Where(x => x.ItemId == salesInvoiceReturnDetailGroup.ItemId && x.ItemPackageId == salesInvoiceReturnDetailGroup.ItemPackageId && x.BatchNumber == salesInvoiceReturnDetailGroup.BatchNumber && x.ExpireDate == salesInvoiceReturnDetailGroup.ExpireDate && x.CostCenterId == salesInvoiceReturnDetailGroup.CostCenterId && x.BarCode == salesInvoiceReturnDetailGroup.BarCode && x.SellingPrice == salesInvoiceReturnDetailGroup.SellingPrice && x.ItemDiscountPercent == salesInvoiceReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
									 from finalStockReturned in stocksReturned.Where(x => x.ItemId == salesInvoiceReturnDetailGroup.ItemId && x.ItemPackageId == salesInvoiceReturnDetailGroup.ItemPackageId && x.BatchNumber == salesInvoiceReturnDetailGroup.BatchNumber && x.ExpireDate == salesInvoiceReturnDetailGroup.ExpireDate && x.CostCenterId == salesInvoiceReturnDetailGroup.CostCenterId && x.BarCode == salesInvoiceReturnDetailGroup.BarCode && x.SellingPrice == salesInvoiceReturnDetailGroup.SellingPrice && x.ItemDiscountPercent == salesInvoiceReturnDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
									 select new
									 {
										 salesInvoiceReturnDetailGroup.ItemId,
										 salesInvoiceReturnDetailGroup.ItemPackageId,
										 SalesInvoiceQuantity = salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.Quantity : 0,
										 SalesInvoiceBonusQuantity = salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.BonusQuantity : 0,
										 QuantityReturned = (finalStockReturned != null ? finalStockReturned.QuantityReturned : 0) + salesInvoiceReturnDetailGroup.Quantity,
										 BonusQuantityReturned = (finalStockReturned != null ? finalStockReturned.BonusQuantityReturned : 0) + salesInvoiceReturnDetailGroup.BonusQuantity,
									 }).FirstOrDefault(x => x.QuantityReturned > x.SalesInvoiceQuantity || x.BonusQuantityReturned > x.SalesInvoiceBonusQuantity);

			if (exceedingQuantity != null)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();

				var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingQuantity.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
				var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingQuantity.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

				if (exceedingQuantity.QuantityReturned > exceedingQuantity.SalesInvoiceQuantity)
				{
					return new ResponseDto { Id = salesInvoiceReturn.SalesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.QuantityExceeding, itemName!, itemPackageName!, exceedingQuantity.QuantityReturned.ToNormalizedString(), exceedingQuantity.SalesInvoiceQuantity.ToNormalizedString()) };
				}
				else
				{
					return new ResponseDto { Id = salesInvoiceReturn.SalesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.BonusQuantityExceeding, itemName!, itemPackageName!, exceedingQuantity.BonusQuantityReturned.ToNormalizedString(), exceedingQuantity.SalesInvoiceBonusQuantity.ToNormalizedString()) };
				}
			}

			return new ResponseDto { Success = true };
		}


		public async Task<ResponseDto> DeleteSalesInvoiceReturn(int salesInvoiceReturnHeaderId, int menuCode)
        {
			var salesInvoiceReturnHeader = await _salesInvoiceReturnHeaderService.GetSalesInvoiceReturnHeaderById(salesInvoiceReturnHeaderId);
			if (salesInvoiceReturnHeader == null) return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };

			var parentMenuCode = await GetParentMenuCode(salesInvoiceReturnHeader.SalesInvoiceHeaderId);
			var validationResult = await CheckSalesInvoiceReturnIsValidForDelete(salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, salesInvoiceReturnHeader.SalesInvoiceHeaderId, salesInvoiceReturnHeader.StoreId, salesInvoiceReturnHeader.IsBlocked, salesInvoiceReturnHeader.IsOnTheWay, menuCode, parentMenuCode);
			if (validationResult.Success == false) return validationResult;

			var relatedStockOutReturnHeaderId = salesInvoiceReturnHeader.IsDirectInvoice ? (int?)await GetStockOutReturnHeaderIdLinkedToSalesInvoiceReturn(salesInvoiceReturnHeaderId) : null ;

			await ApplySalesInvoiceReturnSideEffectsOnDelete(salesInvoiceReturnHeaderId, salesInvoiceReturnHeader.SalesInvoiceHeaderId, relatedStockOutReturnHeaderId, salesInvoiceReturnHeader.IsOnTheWay, salesInvoiceReturnHeader.IsDirectInvoice);
            
			var result = await DeleteSalesInvoiceReturnItself(salesInvoiceReturnHeaderId, relatedStockOutReturnHeaderId, salesInvoiceReturnHeader.IsDirectInvoice, menuCode);
			if (result.Success)
			{
				await UpdateProformaInvoiceStatus(salesInvoiceReturnHeader.SalesInvoiceHeaderId, false);

				var settlementExccedResult = await _getSalesInvoiceSettleValueService.CheckSettlementExceedingAndUpdateSettlementCompletedFlag(salesInvoiceReturnHeader.SalesInvoiceHeaderId, menuCode, parentMenuCode, false);
				if (settlementExccedResult.Success == false) return settlementExccedResult;
			}
			return result;
        }

		public async Task<ResponseDto> CheckSalesInvoiceReturnIsValidForDelete(int salesInvoiceReturnHeaderId, int salesInvoiceHeaderId, int storeId, bool isBlocked, bool isOnTheWay, int menuCode, int parentMenuCode)
		{
			if (isBlocked)
			{
				return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
			}

			// SalesInvoiceReturn cannot be created without stockOutreturn so no need to check it seperately
			var salesInvoiceHasStockOutReturn = await CheckSalesInvoiceHasStockOutReturn(salesInvoiceHeaderId, isOnTheWay, menuCode, parentMenuCode);
			if (salesInvoiceHasStockOutReturn.Success == false) return salesInvoiceHasStockOutReturn;

			var hasClientDebitOrCreditMemo = await CheckSalesInvoiceHasClientCreditOrDebitMemo(salesInvoiceHeaderId, menuCode, parentMenuCode);
			if (hasClientDebitOrCreditMemo.Success == false) return hasClientDebitOrCreditMemo;

			var zeroStockResult = await CheckSalesInvoiceReturnZeroStock(salesInvoiceReturnHeaderId, storeId, [], menuCode, parentMenuCode, false);
			if (zeroStockResult.Success == false) return zeroStockResult;

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckSalesInvoiceHasStockOutReturn(int salesInvoiceHeaderId, bool isOnTheWay, int menuCode, int parentMenuCode)
		{
			if (isOnTheWay)
			{
				var isEnded = await _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).AnyAsync();
				if (isEnded)
				{
					return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.CannotDeleteBecauseReturnedAfterInvoice) };
				}
			}
			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> DeleteSalesInvoiceReturnItself(int salesInvoiceReturnHeaderId, int? relatedStockOutReturnHeaderId, bool isDirectInvoice, int menuCode)
		{
			if (isDirectInvoice)
			{
				await _stockOutReturnService.DeleteStockOutReturn((int)relatedStockOutReturnHeaderId!, MenuCodeData.StockOutReturnFromInvoice);
			}
			return await _salesInvoiceReturnService.DeleteSalesInvoiceReturn(salesInvoiceReturnHeaderId, menuCode);
		}

		private async Task ApplySalesInvoiceReturnSideEffectsOnDelete(int salesInvoiceReturnHeaderId, int salesInvoiceHeaderId, int? relatedStockOutReturnHeaderId, bool isOnTheWay, bool isDirectInvoice)
		{
			await ReopenSalesInvoice(salesInvoiceReturnHeaderId, salesInvoiceHeaderId, relatedStockOutReturnHeaderId, isOnTheWay);
			await MarkStockOutsAndStockOutReturnsAsUnInvoiced(salesInvoiceReturnHeaderId, salesInvoiceHeaderId, isOnTheWay, isDirectInvoice);
		}

		private async Task ReopenSalesInvoice(int salesInvoiceReturnHeaderId, int salesInvoiceHeaderId, int? relatedStockOutReturnHeaderId, bool isOnTheWay)
		{
			var hasStockOuts = await _stockOutHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).AnyAsync();
			var hasStockOutReturns = await _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId && x.StockOutReturnHeaderId != relatedStockOutReturnHeaderId).AnyAsync();
			var hasReservationInvoiceCloseOut = await _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId && x.IsOnTheWay && x.SalesInvoiceReturnHeaderId != salesInvoiceReturnHeaderId).AnyAsync();
			// No need to check client debit/credit memo

			await _salesInvoiceHeaderService.UpdateEndedAndClosed(salesInvoiceHeaderId, hasReservationInvoiceCloseOut, hasStockOuts || hasStockOutReturns || hasReservationInvoiceCloseOut);
		}
	}
}
