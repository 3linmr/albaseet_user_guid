using static Shared.CoreOne.Models.StaticData.DocumentStatusData;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Identity;
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
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Contracts.Modules;
using Microsoft.VisualBasic;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Contracts.Menus;
using Sales.CoreOne.Models.StaticData;
using Shared.CoreOne.Models.Domain.Modules;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Purchases.Service.Services;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Contracts.Accounts;

namespace Sales.Service.Services
{
    public class SalesInvoiceHandlingService: ISalesInvoiceHandlingService
    {
        private readonly IClientCreditMemoService _clientCreditMemoService;
        private readonly IClientDebitMemoService _clientDebitMemoService;
        private readonly ISalesValueService _salesValueService;
        private readonly IClientQuotationApprovalHeaderService _clientQuotationApprovalHeaderService;
        private readonly IClientQuotationApprovalDetailService _clientQuotationApprovalDetailService;
        private readonly IClientQuotationApprovalDetailTaxService _clientQuotationApprovalDetailTaxService;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
        private readonly IStockOutService _stockOutService;
        private readonly IProformaInvoiceService _proformaInvoiceService;
        private readonly IStoreService _storeService;
        private readonly IProformaInvoiceStatusService _proformaInvoiceStatusService;
        private readonly IInvoiceStockOutService _invoiceStockOutService;
        private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItemPackageService _itemPackageService;
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly IProformaInvoiceDetailTaxService _proformaInvoiceDetailTaxService;
        private readonly IItemTaxService _itemTaxService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IStockOutHeaderService _stockOutHeaderService;
        private readonly IProformaInvoiceDetailService _proformaInvoiceDetailService;
        private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
        private readonly ISalesInvoiceService _salesInvoiceService;
        private readonly IItemCostService _itemCostService;
        private readonly IItemService _itemService;
        private readonly IItemPackingService _itemPackingService;
        private readonly IStockOutQuantityService _stockOutQuantityService;
        private readonly IGenericMessageService _genericMessageService;
        private readonly ISalesMessageService _salesMessageService;
        private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
        private readonly IZeroStockValidationService _zeroStockValidationService;
        private readonly IDirectSalesInvoiceFromClientQuotationApprovalService _directSalesInvoiceFromClientQuotationApprovalService;
        private readonly IItemNoteValidationService _itemNoteValidationService;
		private readonly IDocumentExceedValueSettingService _documentExceedValueSettingService;
        private readonly ICostCenterService _costCenterService;
		private readonly ITaxPercentService _taxPercentService;
		private readonly IAccountService _accountService;

        public SalesInvoiceHandlingService(IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService, ISalesValueService salesValueService, IClientQuotationApprovalHeaderService clientQuotationApprovalHeaderService, IClientQuotationApprovalDetailService clientQuotationApprovalDetailService, IClientQuotationApprovalDetailTaxService clientQuotationApprovalDetailTaxService, IApplicationSettingService applicationSettingService, ISalesInvoiceDetailService salesInvoiceDetailService, IStockOutService stockOutService, IProformaInvoiceService proformaInvoiceService, IStoreService storeService, IProformaInvoiceStatusService proformaInvoiceStatusService, IInvoiceStockOutService invoiceStockOutService, IStockOutReturnHeaderService stockOutReturnHeaderService, IHttpContextAccessor httpContextAccessor, IItemPackageService itemPackageService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IProformaInvoiceDetailTaxService proformaInvoiceDetailTaxService, IItemTaxService itemTaxService, IItemBarCodeService itemBarcodeService, IStockOutHeaderService stockOutHeaderService, IProformaInvoiceDetailService proformaInvoiceDetailService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, ISalesInvoiceService salesInvoiceService, IItemCostService itemCostService, IItemService itemService, IItemPackingService itemPackingService, IStockOutQuantityService stockOutQuantityService, IGenericMessageService genericMessageService, ISalesMessageService salesMessageService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IZeroStockValidationService zeroStockValidationService, IDirectSalesInvoiceFromClientQuotationApprovalService directSalesInvoiceFromClientQuotationApprovalService, IItemNoteValidationService itemNoteValidationService, IDocumentExceedValueSettingService documentExceedValueSettingService, ICostCenterService costCenterService, ITaxPercentService taxPercentService, IAccountService accountService)
        {
            _clientCreditMemoService = clientCreditMemoService;
            _clientDebitMemoService = clientDebitMemoService;
            _salesValueService = salesValueService;
            _clientQuotationApprovalHeaderService = clientQuotationApprovalHeaderService;
            _clientQuotationApprovalDetailService = clientQuotationApprovalDetailService;
            _clientQuotationApprovalDetailTaxService = clientQuotationApprovalDetailTaxService;
            _applicationSettingService = applicationSettingService;
            _salesInvoiceDetailService = salesInvoiceDetailService;
            _stockOutService = stockOutService;
            _proformaInvoiceService = proformaInvoiceService;
            _storeService = storeService;
            _proformaInvoiceStatusService = proformaInvoiceStatusService;
			_invoiceStockOutService = invoiceStockOutService;
            _stockOutReturnHeaderService = stockOutReturnHeaderService;
            _httpContextAccessor = httpContextAccessor;
            _itemPackageService = itemPackageService;
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _proformaInvoiceDetailTaxService = proformaInvoiceDetailTaxService;
            _itemTaxService = itemTaxService;
            _itemBarCodeService = itemBarcodeService;
            _stockOutHeaderService = stockOutHeaderService;
            _proformaInvoiceDetailService = proformaInvoiceDetailService;
            _proformaInvoiceHeaderService = proformaInvoiceHeaderService;
            _salesInvoiceService = salesInvoiceService;
            _itemCostService = itemCostService;
            _itemService = itemService;
            _itemPackingService = itemPackingService;
            _stockOutQuantityService = stockOutQuantityService;
            _genericMessageService = genericMessageService;
            _salesMessageService = salesMessageService;
            _salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
            _zeroStockValidationService = zeroStockValidationService;
            _directSalesInvoiceFromClientQuotationApprovalService = directSalesInvoiceFromClientQuotationApprovalService;
            _itemNoteValidationService = itemNoteValidationService;
			_documentExceedValueSettingService = documentExceedValueSettingService;
            _costCenterService = costCenterService;
			_taxPercentService = taxPercentService;
			_accountService = accountService;
        }

        public async Task<SalesInvoiceDto> GetSalesInvoiceFromProformaInvoice(int proformaInvoiceHeaderId)
        {
            var header = await _proformaInvoiceHeaderService.GetProformaInvoiceHeaderById(proformaInvoiceHeaderId);
            if (header == null) return new SalesInvoiceDto();

			var salesInvoiceDetails = await GetSalesInvoiceDetailsFromProformaInvoice(proformaInvoiceHeaderId, header.StoreId, header.DiscountPercent);
			await _salesInvoiceService.ModifySalesInvoiceDetailsWithStoreIdAndAvaialbleBalance(0, header.StoreId, salesInvoiceDetails, false, true);

			var salesInvoiceHeader = GetSalesInvoiceHeaderFromParent(salesInvoiceDetails, new SalesInvoiceFromParentDto { 
                ProformaInvoiceHeaderId = header.ProformaInvoiceHeaderId, 
                ProformaInvoiceFullCode = header.DocumentFullCode, 
                ProformaInvoiceDocumentReference = header.DocumentReference, 
                ClientId = header.ClientId, 
                ClientCode = header.ClientCode,
                ClientName = header.ClientName, 
                SellerId = header.SellerId,
                SellerCode = header.SellerCode,
                SellerName = header.SellerName,
                StoreId = header.StoreId, 
                StoreName = header.StoreName, 
                DocumentDate = header.DocumentDate, 
                Reference = header.Reference, 
                IsDirectInvoice = false, 
                CreditPayment = true, 
                TaxTypeId = header.TaxTypeId, 
                ShippingDate = header.ShippingDate, 
                DeliveryDate = header.DeliveryDate, 
                DueDate = header.DueDate, 
                ShipmentTypeId = header.ShipmentTypeId,
                ShipmentTypeName = header.ShipmentTypeName,
                CashClientName = header.CashClientName, 
                ClientPhone = header.ClientPhone, 
                ClientAddress = header.ClientAddress, 
                ClientTaxCode = header.ClientTaxCode, 
                DriverName = header.DriverName, 
                DriverPhone = header.DriverPhone, 
                ClientResponsibleName = header.ClientResponsibleName, 
                ClientResponsiblePhone = header.ClientResponsiblePhone, 
                ShipTo = header.ShipTo, 
                BillTo = header.BillTo, 
                ShippingRemarks = header.ShippingRemarks, 
                DiscountPercent = header.DiscountPercent,
                AdditionalDiscountValue = header.AdditionalDiscountValue,
                RemarksAr = header.RemarksAr, 
                RemarksEn = header.RemarksEn, 
                IsOnTheWay = false, 
                ArchiveHeaderId = header.ArchiveHeaderId });

            return new SalesInvoiceDto { SalesInvoiceHeader = salesInvoiceHeader, SalesInvoiceDetails = salesInvoiceDetails };
        }

		public async Task<SalesInvoiceWithResponseDto> GetSalesInvoiceFromClientQuotationApproval(int clientQuotationApprovalHeaderId, bool isOnTheWay, bool isCreditPayment)
		{
			var header = await _clientQuotationApprovalHeaderService.GetClientQuotationApprovalHeaderById(clientQuotationApprovalHeaderId);
			if (header == null) return new SalesInvoiceWithResponseDto { Result = new ResponseDto(), SalesInvoice = new SalesInvoiceDto()};

			var salesInvoiceDetailsResult = await GetSalesInvoiceDetailsFromClientQuotationApproval(clientQuotationApprovalHeaderId, header.StoreId, header.DiscountPercent, isOnTheWay, isCreditPayment);
            await _salesInvoiceService.ModifySalesInvoiceDetailsWithStoreIdAndAvaialbleBalance(0, header.StoreId, salesInvoiceDetailsResult.SalesInvoiceDetails, false, true);

            var salesInvoiceHeader = GetSalesInvoiceHeaderFromParent(salesInvoiceDetailsResult.SalesInvoiceDetails, new SalesInvoiceFromParentDto {
                ClientQuotationApprovalHeaderId = header.ClientQuotationApprovalHeaderId, 
                ClientId = header.ClientId, 
                ClientCode = header.ClientCode,
                ClientName = header.ClientName, 
                SellerId = header.SellerId,
                SellerCode = header.SellerCode,
                SellerName = header.SellerName,
                StoreId = header.StoreId, 
                StoreName = header.StoreName, 
                DocumentDate = header.DocumentDate, 
                Reference = header.Reference, 
                IsDirectInvoice = true, 
                CreditPayment = isCreditPayment, 
                TaxTypeId = TaxTypeData.Taxable, 
                DiscountPercent = header.DiscountPercent, 
                AdditionalDiscountValue = header.AdditionalDiscountValue,
                RemarksAr = header.RemarksAr, 
                RemarksEn = header.RemarksEn, 
                IsOnTheWay = isOnTheWay, 
                ArchiveHeaderId = header.ArchiveHeaderId });

			return new SalesInvoiceWithResponseDto { SalesInvoice = new SalesInvoiceDto { SalesInvoiceHeader = salesInvoiceHeader, SalesInvoiceDetails = salesInvoiceDetailsResult.SalesInvoiceDetails }, Result = salesInvoiceDetailsResult.Result };
		}

		private async Task<List<SalesInvoiceDetailDto>> GetSalesInvoiceDetailsFromProformaInvoice(int proformaInvoiceHeaderId, int storeId, decimal headerDiscountPercent)
        {
			var proformaInvoiceDetails = await _proformaInvoiceDetailService.GetProformaInvoiceDetailsFullyGrouped(proformaInvoiceHeaderId);

			var itemIds = proformaInvoiceDetails.Select(x => x.ItemId).ToList();
			var items = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var itemPackings = await _itemPackingService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var lastSalesPrices = await _salesInvoiceService.GetMultipleLastSalesPrices(itemIds);

            var unInvoicedStocks = await _stockOutQuantityService.GetUnInvoicedDisbursedQuantityFromProformaInvoiceWithAllKeys(proformaInvoiceHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

            var costCenterIds = unInvoicedStocks.Select(x => x.CostCenterId).ToList();
            var costCenters = await _costCenterService.GetAll().Where(x => costCenterIds.Contains(x.CostCenterId)).ToListAsync();

            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var vatTaxId = (await _taxPercentService.GetVatByStoreId(storeId, DateTime.Now)).TaxId; //The date doesn't matter, I only want the taxId

            var salesInvoiceDetails = (from proformaInvoiceDetail in proformaInvoiceDetails
                                       from unInvoicedStock in unInvoicedStocks.Where(x => x.ItemId == proformaInvoiceDetail.ItemId && x.ItemPackageId == proformaInvoiceDetail.ItemPackageId && x.BarCode == proformaInvoiceDetail.BarCode && x.SellingPrice == proformaInvoiceDetail.SellingPrice && x.ItemDiscountPercent == proformaInvoiceDetail.ItemDiscountPercent)
                                       from costCenter in costCenters.Where(x => x.CostCenterId == unInvoicedStock.CostCenterId).DefaultIfEmpty()
                                       from item in items.Where(x => x.ItemId == proformaInvoiceDetail.ItemId)
                                       from itemPacking in itemPackings.Where(x => x.ItemId == proformaInvoiceDetail.ItemId && x.FromPackageId == proformaInvoiceDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
                                       from itemCost in itemCosts.Where(x => x.ItemId == proformaInvoiceDetail.ItemId && x.ItemPackageId == proformaInvoiceDetail.ItemPackageId).DefaultIfEmpty()
                                       from lastSalesPrice in lastSalesPrices.Where(x => x.ItemId == proformaInvoiceDetail.ItemId && x.ItemPackageId == proformaInvoiceDetail.ItemPackageId).DefaultIfEmpty()
                                       select new SalesInvoiceDetailDto
                                       {
                                           SalesInvoiceDetailId = proformaInvoiceDetail.ProformaInvoiceDetailId, // <-- used for detail taxes
                                           ItemId = proformaInvoiceDetail.ItemId,
                                           ItemCode = proformaInvoiceDetail.ItemCode,
                                           ItemName = proformaInvoiceDetail.ItemName,
                                           ItemTypeId = proformaInvoiceDetail.ItemTypeId,
                                           TaxTypeId = proformaInvoiceDetail.TaxTypeId,
                                           ItemPackageId = proformaInvoiceDetail.ItemPackageId,
                                           ItemPackageName = proformaInvoiceDetail.ItemPackageName,
                                           IsItemVatInclusive = proformaInvoiceDetail.IsItemVatInclusive,
                                           CostCenterId = unInvoicedStock.CostCenterId,
										   CostCenterName = costCenter != null ? (language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn) : null,
                                           BarCode = proformaInvoiceDetail.BarCode,
                                           Packing = proformaInvoiceDetail.Packing,
                                           ExpireDate = unInvoicedStock.ExpireDate,
                                           BatchNumber = unInvoicedStock.BatchNumber,
                                           Quantity = unInvoicedStock.QuantityDisbursed,
                                           BonusQuantity = unInvoicedStock.BonusQuantityDisbursed,
                                           SellingPrice = proformaInvoiceDetail.SellingPrice,
                                           ItemDiscountPercent = proformaInvoiceDetail.ItemDiscountPercent,
                                           VatPercent = proformaInvoiceDetail.VatPercent,
										   Notes = proformaInvoiceDetail.Notes,
                                           ItemNote = proformaInvoiceDetail.ItemNote,
                                           VatTaxId = vatTaxId,
                                           VatTaxTypeId = item.TaxTypeId,
										   ConsumerPrice = item.ConsumerPrice,
										   CostPrice = itemCost != null ? itemCost.CostPrice : 0,
										   CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
										   LastSalesPrice = lastSalesPrice != null ? lastSalesPrice.SellingPrice : 0
									   }).ToList();

			salesInvoiceDetails = CalculateValues(salesInvoiceDetails, headerDiscountPercent);
			salesInvoiceDetails = salesInvoiceDetails.Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();

			await GetAuxiliaryData(salesInvoiceDetails);
            await CalculateOtherTaxesFromProformaInvoice(salesInvoiceDetails, headerDiscountPercent, proformaInvoiceHeaderId);
            SerializeSalesInvoiceDetails(salesInvoiceDetails);

            return salesInvoiceDetails;
		}

		private async Task<SalesInvoiceDetailsWithResponseDto> GetSalesInvoiceDetailsFromClientQuotationApproval(int clientQuotationApprovalHeaderId, int storeId, decimal headerDiscountPercent, bool isOnTheWay, bool isCreditPayment)
		{
            var result = await _directSalesInvoiceFromClientQuotationApprovalService.GetSalesInvoiceDetailsFromClientQuotationApproval(clientQuotationApprovalHeaderId, storeId, headerDiscountPercent, isOnTheWay, isCreditPayment);

			await GetAuxiliaryData(result.SalesInvoiceDetails);
			await CalculateOtherTaxesFromClientQuotationApproval(result.SalesInvoiceDetails, headerDiscountPercent, clientQuotationApprovalHeaderId);
			SerializeSalesInvoiceDetails(result.SalesInvoiceDetails);

			return result;
		}

		private SalesInvoiceHeaderDto GetSalesInvoiceHeaderFromParent(List<SalesInvoiceDetailDto> salesInvoiceDetails, SalesInvoiceFromParentDto salesInvoiceFromParentDto)
        {
			var totalValueFromDetail = salesInvoiceDetails.Sum(x => x.TotalValue);
			var totalValueAfterDiscountFromDetail = salesInvoiceDetails.Sum(x => x.TotalValueAfterDiscount);
			var totalItemDiscount = salesInvoiceDetails.Sum(x => x.ItemDiscountValue);
			var grossValueFromDetail = salesInvoiceDetails.Sum(x => x.GrossValue);
			var vatValueFromDetail = salesInvoiceDetails.Sum(x => x.VatValue);
			var subNetValueFromDetail = salesInvoiceDetails.Sum(x => x.SubNetValue);
			var otherTaxValueFromDetail = salesInvoiceDetails.Sum(x => x.OtherTaxValue);
			var netValueFromDetail = salesInvoiceDetails.Sum(x => x.NetValue);
			var totalCostValueFromDetail = salesInvoiceDetails.Sum(x => x.CostValue);

			return new SalesInvoiceHeaderDto
            {
                SalesInvoiceHeaderId = 0,
                ProformaInvoiceHeaderId = salesInvoiceFromParentDto.ProformaInvoiceHeaderId,
                ProformaInvoiceFullCode = salesInvoiceFromParentDto.ProformaInvoiceFullCode,
                ProformaInvoiceDocumentReference = salesInvoiceFromParentDto.ProformaInvoiceDocumentReference,
                ClientQuotationApprovalHeaderId = salesInvoiceFromParentDto.ClientQuotationApprovalHeaderId,
                ClientId = salesInvoiceFromParentDto.ClientId,
                ClientCode = salesInvoiceFromParentDto.ClientCode,
                ClientName = salesInvoiceFromParentDto.ClientName,
                SellerId = salesInvoiceFromParentDto.SellerId,
                SellerCode = salesInvoiceFromParentDto.SellerCode,
                SellerName = salesInvoiceFromParentDto.SellerName,
                StoreId = salesInvoiceFromParentDto.StoreId,
                StoreName = salesInvoiceFromParentDto.StoreName,
                DocumentDate = salesInvoiceFromParentDto.DocumentDate,
                Reference = salesInvoiceFromParentDto.Reference,
                IsDirectInvoice = salesInvoiceFromParentDto.IsDirectInvoice,
                CreditPayment = salesInvoiceFromParentDto.CreditPayment,
                TaxTypeId = salesInvoiceFromParentDto.TaxTypeId,
                ShippingDate = salesInvoiceFromParentDto.ShippingDate,
                DeliveryDate = salesInvoiceFromParentDto.DeliveryDate,
                DueDate = salesInvoiceFromParentDto.DueDate,
                ShipmentTypeId = salesInvoiceFromParentDto.ShipmentTypeId,
                ShipmentTypeName = salesInvoiceFromParentDto.ShipmentTypeName,
                CashClientName = salesInvoiceFromParentDto.CashClientName,
                ClientPhone = salesInvoiceFromParentDto.ClientPhone,
                ClientAddress = salesInvoiceFromParentDto.ClientAddress,
                ClientTaxCode = salesInvoiceFromParentDto.ClientTaxCode,
                DriverName = salesInvoiceFromParentDto.DriverName,
                DriverPhone = salesInvoiceFromParentDto.DriverPhone,
                ClientResponsibleName = salesInvoiceFromParentDto.ClientResponsibleName,
                ClientResponsiblePhone = salesInvoiceFromParentDto.ClientResponsiblePhone,
                ShipTo = salesInvoiceFromParentDto.ShipTo,
                BillTo = salesInvoiceFromParentDto.BillTo,
                ShippingRemarks = salesInvoiceFromParentDto.ShippingRemarks,
                TotalValue = totalValueFromDetail,
                DiscountPercent = salesInvoiceFromParentDto.DiscountPercent,
                DiscountValue = CalculateHeaderValue.DiscountValue(totalValueAfterDiscountFromDetail, salesInvoiceFromParentDto.DiscountPercent),
                TotalItemDiscount = totalItemDiscount,
                GrossValue = grossValueFromDetail,
                VatValue = vatValueFromDetail,
                SubNetValue = subNetValueFromDetail,
                OtherTaxValue = otherTaxValueFromDetail,
                NetValueBeforeAdditionalDiscount = netValueFromDetail,
                AdditionalDiscountValue = salesInvoiceFromParentDto.AdditionalDiscountValue,
                NetValue = CalculateHeaderValue.NetValue(netValueFromDetail, salesInvoiceFromParentDto.AdditionalDiscountValue),
                TotalCostValue = totalCostValueFromDetail,
                DebitAccountId = 0,
                CreditAccountId = 0,
                JournalHeaderId = 0,
                RemarksAr = salesInvoiceFromParentDto.RemarksAr,
                RemarksEn = salesInvoiceFromParentDto.RemarksEn,
                CanReturnInDays = 0,
                CanReturnUntilDate = DateHelper.GetDateTimeNow(),
                IsOnTheWay = salesInvoiceFromParentDto.IsOnTheWay,
                IsClosed = false,
                IsBlocked = false,
                IsEnded = false,
                ArchiveHeaderId = salesInvoiceFromParentDto.ArchiveHeaderId,
            };
        }

		private async Task GetAuxiliaryData(List<SalesInvoiceDetailDto> salesInvoiceDetails)
		{
			var itemIds = salesInvoiceDetails.Select(x => x.ItemId).ToList();
			var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
			var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

			foreach (var salesInvoiceDetail in salesInvoiceDetails)
			{
				salesInvoiceDetail.Packages = packages.Where(x => x.ItemId == salesInvoiceDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
				salesInvoiceDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == salesInvoiceDetail.ItemId).ToList();
				salesInvoiceDetail.Taxes = salesInvoiceDetail.ItemTaxData.ToJson();
			}
		}

        private async Task CalculateOtherTaxesFromProformaInvoice(List<SalesInvoiceDetailDto> salesInvoiceDetails, decimal headerDiscountPercent, int proformaInvoiceHeaderId)
        {
			var proformaInvoiceDetailTaxes = await _proformaInvoiceDetailTaxService.GetProformaInvoiceDetailTaxes(proformaInvoiceHeaderId).ToListAsync();
			foreach (var salesInvoiceDetail in salesInvoiceDetails)
			{
				salesInvoiceDetail.SalesInvoiceDetailTaxes = (
					from itemTax in proformaInvoiceDetailTaxes.Where(x =>
						x.ProformaInvoiceDetailId == salesInvoiceDetail.SalesInvoiceDetailId)
					select new SalesInvoiceDetailTaxDto
					{
						TaxId = itemTax.TaxId,
                        TaxTypeId = salesInvoiceDetail.VatTaxTypeId, //the VatTaxTypeId should contain the taxTypeId from item
						CreditAccountId = itemTax.CreditAccountId,
						TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
						TaxPercent = itemTax.TaxPercent,
						TaxValue = CalculateDetailValue.TaxValue(salesInvoiceDetail.Quantity, salesInvoiceDetail.SellingPrice,
							salesInvoiceDetail.ItemDiscountPercent, salesInvoiceDetail.VatPercent, itemTax.TaxPercent,
							itemTax.TaxAfterVatInclusive, headerDiscountPercent, salesInvoiceDetail.IsItemVatInclusive)
					}
				).ToList();

				salesInvoiceDetail.OtherTaxValue = salesInvoiceDetail.SalesInvoiceDetailTaxes.Sum(x => x.TaxValue);
				salesInvoiceDetail.NetValue = CalculateDetailValue.NetValue(salesInvoiceDetail.Quantity,
					salesInvoiceDetail.SellingPrice, salesInvoiceDetail.ItemDiscountPercent, salesInvoiceDetail.VatPercent,
					salesInvoiceDetail.OtherTaxValue, headerDiscountPercent, salesInvoiceDetail.IsItemVatInclusive);
			}
		}

		private async Task CalculateOtherTaxesFromClientQuotationApproval(List<SalesInvoiceDetailDto> salesInvoiceDetails, decimal headerDiscountPercent, int clientQuotationApprovalHeaderId)
		{
			var clientQuotationApprovalDetailTaxes = await _clientQuotationApprovalDetailTaxService.GetClientQuotationApprovalDetailTaxes(clientQuotationApprovalHeaderId).ToListAsync();
			foreach (var salesInvoiceDetail in salesInvoiceDetails)
			{
				salesInvoiceDetail.SalesInvoiceDetailTaxes = (
					from itemTax in clientQuotationApprovalDetailTaxes.Where(x =>
						x.ClientQuotationApprovalDetailId == salesInvoiceDetail.SalesInvoiceDetailId)
					select new SalesInvoiceDetailTaxDto
					{
						TaxId = itemTax.TaxId,
                        TaxTypeId = salesInvoiceDetail.VatTaxTypeId, //the VatTaxTypeId should contain the taxTypeId from item
						CreditAccountId = itemTax.CreditAccountId,
						TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
						TaxPercent = itemTax.TaxPercent,
						TaxValue = CalculateDetailValue.TaxValue(salesInvoiceDetail.Quantity, salesInvoiceDetail.SellingPrice,
							salesInvoiceDetail.ItemDiscountPercent, salesInvoiceDetail.VatPercent, itemTax.TaxPercent,
							itemTax.TaxAfterVatInclusive, headerDiscountPercent, salesInvoiceDetail.IsItemVatInclusive)
					}
				).ToList();

				salesInvoiceDetail.OtherTaxValue = salesInvoiceDetail.SalesInvoiceDetailTaxes.Sum(x => x.TaxValue);
				salesInvoiceDetail.NetValue = CalculateDetailValue.NetValue(salesInvoiceDetail.Quantity,
					salesInvoiceDetail.SellingPrice, salesInvoiceDetail.ItemDiscountPercent, salesInvoiceDetail.VatPercent,
					salesInvoiceDetail.OtherTaxValue, headerDiscountPercent, salesInvoiceDetail.IsItemVatInclusive);
			}
		}

		private void SerializeSalesInvoiceDetails(List<SalesInvoiceDetailDto> salesInvoiceDetails)
		{
			int newId = -1;
			int newSubId = -1;
			foreach (var salesInvoiceDetail in salesInvoiceDetails)
			{
				salesInvoiceDetail.SalesInvoiceDetailId = newId;

				salesInvoiceDetail.SalesInvoiceDetailTaxes.ForEach(y =>
				{
					y.SalesInvoiceDetailId = newId;
					y.SalesInvoiceDetailTaxId = newSubId--;
				});

				newId--;
			}
		}

		private List<SalesInvoiceDetailDto> CalculateValues(List<SalesInvoiceDetailDto> purchaseInvoiceDetails, decimal headerDiscountPercent)
		{
			RecalculateDetailValue.RecalculateDetailValues(
				details: purchaseInvoiceDetails,
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

			return purchaseInvoiceDetails;
		}


		public IQueryable<SalesInvoiceHeaderDto> GetSalesInvoiceHeadersByStoreIdAndMenuCode(int storeId, int? clientId, int menuCode, int salesInvoiceHeaderId)
        {
            clientId ??= 0;
            if (salesInvoiceHeaderId == 0) 
            {
                return menuCode switch
                {
                    MenuCodeData.StockOutFromReservation => GetSalesInvoiceHeadersForStockOut(storeId, (int)clientId),
                    MenuCodeData.StockOutReturnFromInvoice => GetSalesInvoiceHeadersForStockOutReturn(storeId, (int)clientId),
                    MenuCodeData.SalesInvoiceReturn => GetSalesInvoiceHeadersForSalesInvoiceReturn(storeId, (int)clientId),
                    MenuCodeData.CashSalesInvoiceReturn => GetSalesInvoiceHeadersForDirectSalesInvoiceReturn(storeId, (int)clientId, false),
                    MenuCodeData.CreditSalesInvoiceReturn => GetSalesInvoiceHeadersForDirectSalesInvoiceReturn(storeId, (int)clientId, true),
                    MenuCodeData.ReservationInvoiceCloseOut => GetSalesInvoiceHeadersForReservationInvoiceCloseOut(storeId, (int)clientId),
                    MenuCodeData.ClientDebitMemo => GetSalesInvoiceHeadersForClientDebitMemo(storeId, (int)clientId),
                    MenuCodeData.ClientCreditMemo => GetSalesInvoiceHeadersForClientCreditMemo(storeId, (int)clientId),
                    _ => Enumerable.Empty<SalesInvoiceHeaderDto>().AsQueryable()
                };
            }
            else
            {
                return _salesInvoiceHeaderService.GetSalesInvoiceHeaders().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId);
            };

		}

		private IQueryable<SalesInvoiceHeaderDto> GetSalesInvoiceHeadersForStockOut(int storeId, int clientId)
		{
			var salesInvoiceHeaders = _salesInvoiceHeaderService.GetSalesInvoiceHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsEnded == false && x.IsBlocked == false && x.IsOnTheWay == true);

			return from salesInvoiceHeader in salesInvoiceHeaders
				   from overallQuantityAvailable in _stockOutQuantityService.GetOverallQuantityAvailableFromSalesInvoices().Where(x => x.ParentId == salesInvoiceHeader.SalesInvoiceHeaderId && x.Quantity > 0)
				   select salesInvoiceHeader;
		}

		private IQueryable<SalesInvoiceHeaderDto> GetSalesInvoiceHeadersForStockOutReturn(int storeId, int clientId)
		{
			//StockOutReturn cannot be made on direct invoices other than reservation
			var salesInvoiceHeaders = _salesInvoiceHeaderService.GetSalesInvoiceHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsBlocked == false && (x.IsOnTheWay || !x.IsDirectInvoice) && x.CanReturnUntilDate.Date >= DateHelper.GetDateTimeNow().Date && !x.IsSettlementCompleted);

			var excludeSalesInvoiceIds = _clientCreditMemoService.GetAll().Select(x => x.SalesInvoiceHeaderId)
				.Concat(_clientDebitMemoService.GetAll().Select(x => x.SalesInvoiceHeaderId));

			//StockOutReturn cannot be made on reservation invoice if it does not have a closeout
			var hasReservationCloseOut = _salesInvoiceReturnHeaderService.GetAll().Where(x => x.IsOnTheWay).Select(x => x.SalesInvoiceHeaderId);

			return from salesInvoiceHeader in salesInvoiceHeaders.Where(x => !excludeSalesInvoiceIds.Contains(x.SalesInvoiceHeaderId) && (!x.IsOnTheWay || hasReservationCloseOut.Contains(x.SalesInvoiceHeaderId)))
				   from overallQuantityReturnAvailable in _stockOutQuantityService.GetOverallQuantityAvailableReturnFromSalesInvoices().Where(x => x.ParentId == salesInvoiceHeader.SalesInvoiceHeaderId && x.Quantity > 0)
				   select salesInvoiceHeader;
		}

		private IQueryable<SalesInvoiceHeaderDto> GetSalesInvoiceHeadersForSalesInvoiceReturn(int storeId, int clientId)
		{
			//SalesInvoiceReturn cannot be made on direct invoices that are not on the way (cash sales invoice, credit sales invoice)
			var salesInvoiceHeaders = _salesInvoiceHeaderService.GetSalesInvoiceHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsBlocked == false && (x.IsOnTheWay || !x.IsDirectInvoice) && !x.IsSettlementCompleted);

			var invoiceIdsAfterStock = _stockOutQuantityService.GetOverallUnInvoicedQuantityReturnedFromSalesInvoices().Where(x => x.Quantity > 0).Select(x => x.ParentId);

            var excludeSalesInvoiceIds = _clientCreditMemoService.GetAll().Select(x => x.SalesInvoiceHeaderId)
                .Concat(_clientDebitMemoService.GetAll().Select(x => x.SalesInvoiceHeaderId));

            return salesInvoiceHeaders.Where(x => invoiceIdsAfterStock.Contains(x.SalesInvoiceHeaderId) && !excludeSalesInvoiceIds.Contains(x.SalesInvoiceHeaderId));
		}

		private IQueryable<SalesInvoiceHeaderDto> GetSalesInvoiceHeadersForDirectSalesInvoiceReturn(int storeId, int clientId, bool isCreditPayment)
		{
            return from salesInvoiceHeader in _salesInvoiceHeaderService.GetSalesInvoiceHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsEnded == false && x.IsBlocked == false && x.CreditPayment == isCreditPayment && (x.IsDirectInvoice && !x.IsOnTheWay) && x.CanReturnUntilDate.Date >= DateHelper.GetDateTimeNow().Date && !x.IsSettlementCompleted)
				   from overallQuantityReturnAvailable in _stockOutQuantityService.GetOverallQuantityAvailableReturnFromSalesInvoices().Where(x => x.ParentId == salesInvoiceHeader.SalesInvoiceHeaderId && x.Quantity > 0)
                   select salesInvoiceHeader;
		}

		private IQueryable<SalesInvoiceHeaderDto> GetSalesInvoiceHeadersForReservationInvoiceCloseOut(int storeId, int clientId)
		{
			//isEnded flag checks that no invoice returns are already created on the sales Invoice
			var salesInvoiceHeaders = _salesInvoiceHeaderService.GetSalesInvoiceHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsBlocked == false && x.IsEnded == false && x.IsOnTheWay && !x.IsSettlementCompleted);

			var invoiceIdsOnTheWay = _stockOutQuantityService.GetOverallQuantityAvailableFromSalesInvoices().Where(x => x.Quantity > 0).Select(x => x.ParentId);

			return salesInvoiceHeaders.Where(x => invoiceIdsOnTheWay.Contains(x.SalesInvoiceHeaderId));
		}

		private IQueryable<SalesInvoiceHeaderDto> GetSalesInvoiceHeadersForClientDebitMemo(int storeId, int clientId)
		{
			var salesInvoiceHeaders = _salesInvoiceHeaderService.GetSalesInvoiceHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsBlocked == false && x.CreditPayment);

			var hasReservationCloseOut = _salesInvoiceReturnHeaderService.GetAll().Where(x => x.IsOnTheWay).Select(x => x.SalesInvoiceHeaderId);

			return salesInvoiceHeaders.Where(x => !x.IsOnTheWay || hasReservationCloseOut.Contains(x.SalesInvoiceHeaderId));
		}

		private IQueryable<SalesInvoiceHeaderDto> GetSalesInvoiceHeadersForClientCreditMemo(int storeId, int clientId)
		{
			var salesInvoiceHeaders = _salesInvoiceHeaderService.GetSalesInvoiceHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.IsBlocked == false && x.CreditPayment && !x.IsSettlementCompleted);

			var excludeSalesInvoiceHeaderIds = _salesValueService.GetOverallValueOfSalesInvoices().Where(x => x.OverallNetValue <= 0).Select(x => x.SalesInvoiceHeaderId);

			var hasReservationCloseOut = _salesInvoiceReturnHeaderService.GetAll().Where(x => x.IsOnTheWay).Select(x => x.SalesInvoiceHeaderId);

			return salesInvoiceHeaders.Where(x => !excludeSalesInvoiceHeaderIds.Contains(x.SalesInvoiceHeaderId) && (!x.IsOnTheWay || hasReservationCloseOut.Contains(x.SalesInvoiceHeaderId)));
		}

		public async Task<ExpirationDaysAndDateDto> GetDefaultValidReturnDate(int storeId)
        {
			var validDurationSetting = await _applicationSettingService.GetApplicationSettingValueByStoreId(storeId, ApplicationSettingDetailData.DaysToReturnSalesInvoice);

			var inDays = int.Parse(validDurationSetting!);
			var inDate = inDays != 0 ? DateHelper.GetDateTimeNow().AddDays(inDays) : DateHelper.GetDateTimeNow().AddYears(1);

			return new ExpirationDaysAndDateDto
			{
				ValidInDays = inDays,
				ValidUntil = inDate
			};
		}

		public async Task<ResponseDto> SaveSalesInvoice(SalesInvoiceDto salesInvoice, bool hasApprove, bool approved, int? requestId, string? documentReference = null)
        {
			TrimDetailStrings(salesInvoice.SalesInvoiceDetails);
            var salesInvoiceHeader = salesInvoice.SalesInvoiceHeader!;

            var menuCode = SalesInvoiceMenuCodeHelper.GetMenuCode(salesInvoiceHeader);
            var validationResult = await CheckSalesInvoiceIsValidForSave(salesInvoice, menuCode);
            if (validationResult.Success == false) return validationResult;

			await UpdateModelData(salesInvoice);

            var allResults = await SaveSalesInvoiceItself(salesInvoice, hasApprove, approved, requestId, documentReference);

            if (allResults.Result.Success)
            {
                await ApplySalesInvoiceSideEffectsOnSave(allResults.Result.Id, salesInvoiceHeader.ProformaInvoiceHeaderId, salesInvoiceHeader.IsDirectInvoice, salesInvoiceHeader.SalesInvoiceHeaderId == 0, allResults.StockOutHeaderId);
            }

            return allResults.Result;
        }

        private async Task<SalesInvoiceSaveResult> SaveSalesInvoiceItself(SalesInvoiceDto salesInvoice, bool hasApprove, bool approved, int? requestId, string? documentReference)
        {
            if (salesInvoice.SalesInvoiceHeader!.IsDirectInvoice)
            {
                return await SaveDirectSalesInvoice(salesInvoice, hasApprove, approved, requestId, documentReference);
            }
            else
            {
                return new SalesInvoiceSaveResult { Result = await _salesInvoiceService.SaveSalesInvoice(salesInvoice, hasApprove, approved, requestId, documentReference) };
            }
        }

		private async Task<SalesInvoiceSaveResult> SaveDirectSalesInvoice(SalesInvoiceDto salesInvoice, bool hasApprove, bool approved, int? requestId, string? documentReference)
		{
            documentReference ??= await GetDirectSalesInvoiceDocumentReference(salesInvoice.SalesInvoiceHeader!.SalesInvoiceHeaderId, hasApprove, requestId, salesInvoice.SalesInvoiceHeader!.IsOnTheWay, salesInvoice.SalesInvoiceHeader!.CreditPayment);

            var proformaResult = await SaveRelatedProformaInvoice(salesInvoice, hasApprove, approved, requestId, documentReference, salesInvoice.SalesInvoiceHeader!.ClientQuotationApprovalHeaderId);
            if (proformaResult.Success == false) return new SalesInvoiceSaveResult { Result = proformaResult };
            
            salesInvoice.SalesInvoiceHeader!.ProformaInvoiceHeaderId = proformaResult.Id;

            ResponseDto? stockOutResult = null;
			if (!salesInvoice.SalesInvoiceHeader.IsOnTheWay)
            {
                stockOutResult = await SaveRelatedStockOut(salesInvoice, hasApprove, approved, requestId, documentReference);
                if (stockOutResult.Success == false) return new SalesInvoiceSaveResult { Result = stockOutResult };
            }

            var salesInvoiceResult = await _salesInvoiceService.SaveSalesInvoice(salesInvoice, hasApprove, approved, requestId, documentReference);
			return new SalesInvoiceSaveResult { 
                Result = salesInvoiceResult,
                SalesInvoiceHeaderId = salesInvoiceResult.Id,
                ProformaInvoiceHeaderId = proformaResult.Id,
                StockOutHeaderId = stockOutResult?.Id,
			};
		}

        private async Task<string?> GetDirectSalesInvoiceDocumentReference(int salesInvoiceHeaderId, bool hasApprove, int? requestId, bool isOnTheWay, bool isCreditPayment)
        {
            if (salesInvoiceHeaderId != 0)
            {
                return null;
            }
            else if (hasApprove)
            {
                return $"{DocumentReferenceData.Approval}{requestId}";
            }
            else
            {
                return $"{SalesInvoiceMenuCodeHelper.GetDocumentReference(isOnTheWay, true, isCreditPayment)}{await _salesInvoiceHeaderService.GetNextId()}";
            }
        }

        private async Task<ResponseDto> SaveRelatedProformaInvoice(SalesInvoiceDto salesInvoiceDto, bool hasApprove, bool approved, int? requestId, string? documentReference, int? clientQuotationApprovalHeaderId)
		{
			SalesInvoiceHeaderDto salesInvoiceHeader = salesInvoiceDto.SalesInvoiceHeader!;

			var proformaInvoice = new ProformaInvoiceDto { ProformaInvoiceHeader = new ProformaInvoiceHeaderDto
			{
				ProformaInvoiceHeaderId = salesInvoiceHeader.SalesInvoiceHeaderId == 0 ? 0 : salesInvoiceHeader.ProformaInvoiceHeaderId,
                ClientQuotationApprovalHeaderId = clientQuotationApprovalHeaderId,
				DocumentDate = salesInvoiceHeader.DocumentDate,
				ClientId = salesInvoiceHeader.ClientId,
				StoreId = salesInvoiceHeader.StoreId,
				Reference = salesInvoiceHeader.Reference,
				CreditPayment = salesInvoiceHeader.CreditPayment,
				TaxTypeId = salesInvoiceHeader.TaxTypeId,
				ShippingDate = salesInvoiceHeader.ShippingDate,
				DeliveryDate = salesInvoiceHeader.DeliveryDate,
				DueDate = salesInvoiceHeader.DueDate,
				ShipmentTypeId = salesInvoiceHeader.ShipmentTypeId,
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
				TotalValue = salesInvoiceHeader.TotalValue,
				DiscountPercent = salesInvoiceHeader.DiscountPercent,
				DiscountValue = salesInvoiceHeader.DiscountValue,
				TotalItemDiscount = salesInvoiceHeader.TotalItemDiscount,
				GrossValue = salesInvoiceHeader.GrossValue,
				VatValue = salesInvoiceHeader.VatValue,
				SubNetValue = salesInvoiceHeader.SubNetValue,
				OtherTaxValue = salesInvoiceHeader.OtherTaxValue,
                NetValueBeforeAdditionalDiscount = salesInvoiceHeader.NetValueBeforeAdditionalDiscount,
				AdditionalDiscountValue = salesInvoiceHeader.AdditionalDiscountValue,
				NetValue = salesInvoiceHeader.NetValue,
				TotalCostValue = salesInvoiceHeader.TotalCostValue,
				RemarksAr = salesInvoiceHeader.RemarksAr,
				RemarksEn = salesInvoiceHeader.RemarksEn,
				IsClosed = true,
				IsCancelled = false,
				IsEnded = true,
				ArchiveHeaderId = salesInvoiceHeader.ArchiveHeaderId
			}, ProformaInvoiceDetails = (
				from salesInvoiceDetail in salesInvoiceDto.SalesInvoiceDetails
				select new ProformaInvoiceDetailDto
				{
					ProformaInvoiceDetailId = 0,
					ProformaInvoiceHeaderId = 0,
					CostCenterId = salesInvoiceDetail.CostCenterId,
					ItemId = salesInvoiceDetail.ItemId,
					ItemPackageId = salesInvoiceDetail.ItemPackageId,
					IsItemVatInclusive = salesInvoiceDetail.IsItemVatInclusive,
					BarCode = salesInvoiceDetail.BarCode,
					Packing = salesInvoiceDetail.Packing,
					Quantity = salesInvoiceDetail.Quantity,
					BonusQuantity = salesInvoiceDetail.BonusQuantity,
					SellingPrice = salesInvoiceDetail.SellingPrice,
					TotalValue = salesInvoiceDetail.TotalValue,
					ItemDiscountPercent = salesInvoiceDetail.ItemDiscountPercent,
					ItemDiscountValue = salesInvoiceDetail.ItemDiscountValue,
					TotalValueAfterDiscount = salesInvoiceDetail.TotalValueAfterDiscount,
					HeaderDiscountValue = salesInvoiceDetail.HeaderDiscountValue,
					GrossValue = salesInvoiceDetail.GrossValue,
					VatPercent = salesInvoiceDetail.VatPercent,
					VatValue = salesInvoiceDetail.VatValue,
					SubNetValue = salesInvoiceDetail.SubNetValue,
					OtherTaxValue = salesInvoiceDetail.OtherTaxValue,
					NetValue = salesInvoiceDetail.NetValue,
					Notes = salesInvoiceDetail.Notes,
                    ItemNote = salesInvoiceDetail.ItemNote,
					ConsumerPrice = salesInvoiceDetail.ConsumerPrice,
					CostPrice = salesInvoiceDetail.CostPrice,
					CostPackage = salesInvoiceDetail.CostPackage,
					CostValue = salesInvoiceDetail.CostValue,
					LastSalesPrice = salesInvoiceDetail.LastSalesPrice,
					ProformaInvoiceDetailTaxes = (
						from salesInvoiceDetailTax in salesInvoiceDetail.SalesInvoiceDetailTaxes
						select new ProformaInvoiceDetailTaxDto
						{
							TaxId = salesInvoiceDetailTax.TaxId,
							TaxAfterVatInclusive = salesInvoiceDetailTax.TaxAfterVatInclusive,
							CreditAccountId = salesInvoiceDetailTax.CreditAccountId,
							TaxPercent = salesInvoiceDetailTax.TaxPercent,
							TaxValue = salesInvoiceDetailTax.TaxValue,
						}).ToList()
				}).ToList()
			};

            return await _proformaInvoiceService.SaveProformaInvoice(proformaInvoice, hasApprove, approved, requestId, false, documentReference, DocumentStatusData.SalesInvoiceCreated, true);
		}

        private async Task<ResponseDto> SaveRelatedStockOut(SalesInvoiceDto salesInvoice, bool hasApprove, bool approved, int? requestId, string? documentReference)
        {
            SalesInvoiceHeaderDto salesInvoiceHeader = salesInvoice.SalesInvoiceHeader!;

			var stockOutHeaderId = await GetStockOutHeaderIdLinkedToSalesInvoice(salesInvoiceHeader.SalesInvoiceHeaderId);
			if (stockOutHeaderId == null) return new ResponseDto { Success = false, Message = "Stock Out missing from direct sales invoice" };

			var stockOut = new StockOutDto
            {
                StockOutHeader = new StockOutHeaderDto
                {
                    StockOutHeaderId = (int)stockOutHeaderId,
                    StockTypeId = StockTypeData.StockOutFromProformaInvoice,
                    ProformaInvoiceHeaderId = salesInvoiceHeader.ProformaInvoiceHeaderId,
                    ClientId = salesInvoiceHeader.ClientId,
                    StoreId = salesInvoiceHeader.StoreId,
                    DocumentDate = salesInvoiceHeader.DocumentDate,
                    Reference = salesInvoiceHeader.Reference,
                    TotalValue = salesInvoiceHeader.TotalValue,
                    DiscountPercent = salesInvoiceHeader.DiscountPercent,
                    DiscountValue = salesInvoiceHeader.DiscountValue,
                    TotalItemDiscount = salesInvoiceHeader.TotalItemDiscount,
                    GrossValue = salesInvoiceHeader.GrossValue,
                    VatValue = salesInvoiceHeader.VatValue,
                    SubNetValue = salesInvoiceHeader.SubNetValue,
                    OtherTaxValue = salesInvoiceHeader.OtherTaxValue,
                    NetValueBeforeAdditionalDiscount = salesInvoiceHeader.NetValueBeforeAdditionalDiscount,
                    AdditionalDiscountValue = salesInvoiceHeader.AdditionalDiscountValue,
                    NetValue = salesInvoiceHeader.NetValue,
                    TotalCostValue = salesInvoiceHeader.TotalCostValue,
                    RemarksAr = salesInvoiceHeader.RemarksAr,
                    RemarksEn = salesInvoiceHeader.RemarksEn,
                    IsClosed = false,
                    IsEnded = true,
                    ArchiveHeaderId = salesInvoiceHeader.ArchiveHeaderId,
                },
                StockOutDetails = (
					from salesInvoiceDetail in salesInvoice.SalesInvoiceDetails
					select new StockOutDetailDto
					{
                        StockOutDetailId = 0,
                        StockOutHeaderId = 0,
                        ItemId = salesInvoiceDetail.ItemId,
                        ItemPackageId = salesInvoiceDetail.ItemPackageId,
                        IsItemVatInclusive = salesInvoiceDetail.IsItemVatInclusive,
                        CostCenterId = salesInvoiceDetail.CostCenterId,
                        BarCode = salesInvoiceDetail.BarCode,
                        Packing = salesInvoiceDetail.Packing,
                        ExpireDate = salesInvoiceDetail.ExpireDate,
                        BatchNumber = salesInvoiceDetail.BatchNumber,
                        Quantity = salesInvoiceDetail.Quantity,
                        BonusQuantity = salesInvoiceDetail.BonusQuantity,
                        SellingPrice = salesInvoiceDetail.SellingPrice,
                        TotalValue = salesInvoiceDetail.TotalValue,
                        ItemDiscountPercent = salesInvoiceDetail.ItemDiscountPercent,
                        ItemDiscountValue = salesInvoiceDetail.ItemDiscountValue,
                        TotalValueAfterDiscount = salesInvoiceDetail.TotalValueAfterDiscount,
                        HeaderDiscountValue = salesInvoiceDetail.HeaderDiscountValue,
                        GrossValue = salesInvoiceDetail.GrossValue,
                        VatPercent = salesInvoiceDetail.VatPercent,
                        VatValue = salesInvoiceDetail.VatValue,
                        SubNetValue = salesInvoiceDetail.SubNetValue,
                        OtherTaxValue = salesInvoiceDetail.OtherTaxValue,
                        NetValue = salesInvoiceDetail.NetValue,
                        Notes = salesInvoiceDetail.Notes,
                        ItemNote = salesInvoiceDetail.ItemNote,
                        ConsumerPrice = salesInvoiceDetail.ConsumerPrice,
                        CostPrice = salesInvoiceDetail.CostPrice,
                        CostPackage = salesInvoiceDetail.CostPackage,
                        CostValue = salesInvoiceDetail.CostValue,
                        LastSalesPrice = salesInvoiceDetail.LastSalesPrice,
                        StockOutDetailTaxes = (
                            from salesInvoiceDetailTax in salesInvoiceDetail.SalesInvoiceDetailTaxes
                            select new StockOutDetailTaxDto
                            {
                                TaxId = salesInvoiceDetailTax.TaxId,
                                CreditAccountId = salesInvoiceDetailTax.CreditAccountId,
                                TaxAfterVatInclusive = salesInvoiceDetailTax.TaxAfterVatInclusive,
                                TaxPercent = salesInvoiceDetailTax.TaxPercent,
                                TaxValue = salesInvoiceDetailTax.TaxValue
                            }
                        ).ToList()
					}).ToList()
			};

            return await _stockOutService.SaveStockOut(stockOut, hasApprove, approved, requestId, documentReference, true);
		}

        private async Task<int?> GetStockOutHeaderIdLinkedToProformaInvoice(int proformaInvoiceHeaderId, int salesInvoiceHeaderId)
        {
            if (salesInvoiceHeaderId == 0)
            {
                return 0;
            }
            else {
                return await _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).Select(x => (int?)x.StockOutHeaderId).FirstOrDefaultAsync();
            }
        }

		private async Task<int?> GetStockOutHeaderIdLinkedToSalesInvoice(int salesInvoiceHeaderId)
		{
			if (salesInvoiceHeaderId == 0)
			{
				return 0;
			}
			else
			{
				return await _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId)
                    .Join(_proformaInvoiceHeaderService.GetAll(), x => x.ProformaInvoiceHeaderId, x => x.ProformaInvoiceHeaderId, (salesInvoice,proforma) => proforma)
                    .Join(_stockOutHeaderService.GetAll(), x => x.ProformaInvoiceHeaderId, x => x.ProformaInvoiceHeaderId, (proforma, stockOut) => (int?)stockOut.StockOutHeaderId)
                    .FirstOrDefaultAsync();
			}
		}

        //we need isCreate because the salesInvoiceHeaderId parameter has the Id of the new invoice, so it will never be zero
		private async Task ApplySalesInvoiceSideEffectsOnSave(int salesInvoiceHeaderId, int proformaInvoiceHeaderId, bool isDirectInvoice, bool isCreate, int? createdStockOutHeaderId)
        {
            if (isCreate)
            {
                if (!isDirectInvoice)
                {
                    await _proformaInvoiceHeaderService.UpdateClosed(proformaInvoiceHeaderId, true);
                    await _proformaInvoiceStatusService.UpdateProformaInvoiceStatus(proformaInvoiceHeaderId, DocumentStatusData.SalesInvoiceCreated); 
                }
                await MarkStockOutsAndStockOutReturnsAsInvoiced(salesInvoiceHeaderId, proformaInvoiceHeaderId, isDirectInvoice, createdStockOutHeaderId);
            }
        }

        private async Task MarkStockOutsAndStockOutReturnsAsInvoiced(int salesInvoiceHeaderId, int proformaInvoiceHeaderId, bool isDirectInvoice, int? createdStockOutHeaderId)
		{
            if (!isDirectInvoice)
            {
                var stockOutsToBeInvoiced = await GetUninvoicedStockOutsFromProformaInvoice(proformaInvoiceHeaderId);
                var stockOutReturnsToBeInvoiced = await GetUninvoicedStockOutReturnsFromProformaInvoice(proformaInvoiceHeaderId);

                await _invoiceStockOutService.LinkStockOutsToSalesInvoice(salesInvoiceHeaderId, stockOutsToBeInvoiced);
                await _stockOutHeaderService.UpdateStockOutsEnded(stockOutsToBeInvoiced, true);
                await _stockOutReturnHeaderService.UpdateStockOutReturnsEnded(stockOutReturnsToBeInvoiced, true);
            }
            else
            {
                //createStockOutHeaderId will be null when creating a reservation invoice and not null when creating a direct invoice
                if (createdStockOutHeaderId != null)
                {
                    await _invoiceStockOutService.LinkStockOutsToSalesInvoice(salesInvoiceHeaderId, (List<int>)([(int)createdStockOutHeaderId!]));
                }
            }
		}

		private async Task<List<int>> GetUninvoicedStockOutsFromProformaInvoice(int proformaInvoiceHeaderId)
		{
			return await _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId && !x.IsEnded).Select(x => x.StockOutHeaderId).ToListAsync();
		}

		private async Task<List<int>> GetUninvoicedStockOutReturnsFromProformaInvoice(int proformaInvoiceHeaderId)
		{
			return await (from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId && !x.IsEnded)
						  from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)
						  select stockOutReturnHeader.StockOutReturnHeaderId).ToListAsync();
		}

        public async Task<ResponseDto> CheckSalesInvoiceIsValidForSave(SalesInvoiceDto salesInvoice, int menuCode)
        {
            ResponseDto result;
            var salesInvoiceHeader = salesInvoice.SalesInvoiceHeader!;

            result = await CheckCollectedValueTotalEqualNetValue(salesInvoiceHeader.CreditPayment, salesInvoiceHeader.NetValue, salesInvoiceHeader.StoreId, salesInvoice.SalesInvoiceCollections, menuCode);
			if (result.Success == false) return result;

			result = await CheckSalesInvoiceJournalMismatch(salesInvoiceHeader, salesInvoice.SalesInvoiceCollections, salesInvoice.Journal?.JournalHeader, menuCode, salesInvoiceHeader.CreditPayment);
            if (result.Success == false) return result;

			if (salesInvoiceHeader.SalesInvoiceHeaderId != 0)
            {
                result = await CheckSalesInvoiceClosed(salesInvoiceHeader.SalesInvoiceHeaderId, menuCode);
                if (result.Success == false) return result;

				result = await CheckSalesInvoiceHasSettlement(salesInvoiceHeader.SalesInvoiceHeaderId, menuCode);
				if (result.Success == false) return result;
			}

            if (!salesInvoiceHeader.IsDirectInvoice || salesInvoiceHeader.SalesInvoiceHeaderId != 0)
            {
                result = await CheckProformaInvoiceBlocked(salesInvoiceHeader.ProformaInvoiceHeaderId, menuCode);
                if (result.Success == false) return result;
			}

			result = await _itemNoteValidationService.CheckItemNoteWithItemType(salesInvoice.SalesInvoiceDetails, x => x.ItemId, x => x.ItemNote);
			if (result.Success == false) return result;

			if (!salesInvoiceHeader.IsDirectInvoice)
            {
			    result = await CheckProformaInvoiceHasDirectInvoice(salesInvoiceHeader.ProformaInvoiceHeaderId, menuCode);
			    if (result.Success == false) return result;

                result = await CheckSalesInvoiceQuantity(salesInvoice, menuCode);
                if (result.Success == false) return result;
            }
            else
            {
				result = await CheckSalesInvoiceZeroStock(salesInvoiceHeader.SalesInvoiceHeaderId, salesInvoiceHeader.StoreId, salesInvoice.SalesInvoiceDetails, menuCode);
				if (result.Success == false) return result;
			}

			return new ResponseDto { Success = true };
        }

		private async Task<ResponseDto> CheckSalesInvoiceJournalMismatch(SalesInvoiceHeaderDto salesInvoiceHeader, List<SalesInvoiceCollectionDto> collections, JournalHeaderDto? journalHeader, int menuCode, bool isCredit)
		{
			var exceedValueSetting = await _documentExceedValueSettingService.GetSettingByMenuCode(menuCode, salesInvoiceHeader.StoreId);
			if (exceedValueSetting) return new ResponseDto { Success = true };

			//GetStoreRounding instead of header rounding to handle collection methods
			int rounding = await _storeService.GetStoreRounding(salesInvoiceHeader.StoreId);

			decimal journalCreditValue = NumberHelper.RoundNumber(journalHeader?.TotalCreditValue ?? 0, rounding);
			decimal invoiceValue = NumberHelper.RoundNumber((isCredit ? salesInvoiceHeader.NetValue : collections.Sum(x => x.CollectedValue)) + salesInvoiceHeader.AdditionalDiscountValue, rounding);

			if (journalCreditValue != invoiceValue)
			{
				return new ResponseDto { Success = false, Id = salesInvoiceHeader.SalesInvoiceHeaderId, Message = await _salesMessageService.GetMessage(menuCode, SalesMessageData.ValueNotMatchingJournalCredit) };
			}
			else
			{
				return new ResponseDto { Success = true };
			}
		}

		private async Task<ResponseDto> CheckCollectedValueTotalEqualNetValue(bool isCreditPayment, decimal netValue, int storeId, List<SalesInvoiceCollectionDto> collections, int menuCode)
        {
            var collectedTotal = collections.Sum(x => x.CollectedValue);
            var rounding = await _storeService.GetStoreRounding(storeId);

            var roundedNetValue = NumberHelper.RoundNumber(netValue, rounding);
            var roundedCollectedValue = NumberHelper.RoundNumber(collectedTotal, rounding);

			if (!isCreditPayment)
            {
                if (roundedNetValue != roundedCollectedValue)
                {
                    return new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(menuCode, SalesMessageData.CollectedValueNotMatchTotalValue, roundedCollectedValue.ToNormalizedString(), roundedNetValue.ToNormalizedString()) };
                }
            }
            else if (roundedCollectedValue != 0)
            {
				return new ResponseDto { Success = false, Message = _salesMessageService.GetMessage(SalesMessageData.NoCollectionMethodWithCreditInvoices) };
			}

            return new ResponseDto { Success = true };
        }

		private async Task<ResponseDto> CheckSalesInvoiceClosed(int salesInvoiceHeaderId, int menuCode)
		{
			var isClosed = await _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => (bool?)x.IsClosed).FirstOrDefaultAsync();
			if (isClosed == null)
			{
				return new ResponseDto { Id = salesInvoiceHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };
			}

			if (isClosed == true)
			{
				return new ResponseDto { Id = salesInvoiceHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotUpdateBecauseClosed) };
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckSalesInvoiceHasSettlement(int salesInvoiceHeaderId, int menuCode)
		{
			var hasSettlement = await _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.HasSettlement).FirstOrDefaultAsync();

			if (hasSettlement == true)
			{
				return new ResponseDto { Id = salesInvoiceHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotUpdateBecauseSettlementStarted) };
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckProformaInvoiceBlocked(int proformaInvoiceHeaderId, int menuCode)
		{
            var isBlocked = await _proformaInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).Select(x => x.IsBlocked).FirstOrDefaultAsync();

			if (isBlocked)
			{
				return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckProformaInvoiceHasDirectInvoice(int proformaInvoiceHeaderId, int menuCode)
		{
            var directInvoice = await _salesInvoiceHeaderService.GetSalesInvoiceHeaders().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId && x.IsDirectInvoice).FirstOrDefaultAsync();

            if (directInvoice != null)
            {
                return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, SalesInvoiceMenuCodeHelper.GetMenuCode(directInvoice), MenuCodeData.ProformaInvoice, GenericMessageData.HasDocument) };
            }

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckSalesInvoiceQuantity(SalesInvoiceDto salesInvoice, int menuCode)
		{
            if (salesInvoice.SalesInvoiceHeader!.SalesInvoiceHeaderId == 0)
            {
                return await CheckSalesInvoiceAfterStockQuantity(salesInvoice, menuCode);
            }
            else
            {
                return await CheckQuantityDidnotChange(salesInvoice, menuCode);
            }
		}

        private async Task<ResponseDto> CheckSalesInvoiceZeroStock(int salesInvoiceHeaderId, int storeId, List<SalesInvoiceDetailDto> salesInvoiceDetails, int menuCode)
        {
            List<SalesInvoiceDetailDto> oldSalesInvoiceDetails = [];
            if (salesInvoiceHeaderId != 0) oldSalesInvoiceDetails = await _salesInvoiceDetailService.GetSalesInvoiceDetailsAsQueryable(salesInvoiceHeaderId).ToListAsync();

            return await _zeroStockValidationService.ValidateZeroStock(
                storeId: storeId,
                newDetails: salesInvoiceDetails,
                oldDetails: oldSalesInvoiceDetails,
                detailKeySelector: x => (x.ItemId, x.ItemPackageId, x.BatchNumber, x.ExpireDate),
                itemIdSelector: x => x.ItemId,
                quantitySelector: x => x.Quantity + x.BonusQuantity,
                availableBalanceKeySelector: x => (x.ItemId, x.ItemPackageId, x.BatchNumber, x.ExpireDate),
                isGrouped: false,
                settingMenuCode: menuCode,
                menuCode: menuCode,
                isSave: true);
        }

        private async Task<ResponseDto> CheckSalesInvoiceAfterStockQuantity(SalesInvoiceDto salesInvoice, int menuCode)
        {
			var unInvoicedStocks = await _stockOutQuantityService.GetUnInvoicedDisbursedQuantityFromProformaInvoiceWithAllKeys(salesInvoice.SalesInvoiceHeader!.ProformaInvoiceHeaderId).ToListAsync();
            var salesInvoiceDetailsGrouped = _salesInvoiceDetailService.GroupSalesInvoiceDetailsWithoutExpireAndBatch(salesInvoice.SalesInvoiceDetails);

			var unmatchedQuantity = (from salesInvoiceDetailGroup in salesInvoiceDetailsGrouped
									 from unInvoicedStock in unInvoicedStocks.Where(x => x.ItemId == salesInvoiceDetailGroup.ItemId && x.ItemPackageId == salesInvoiceDetailGroup.ItemPackageId && x.CostCenterId == salesInvoiceDetailGroup.CostCenterId && x.BarCode == salesInvoiceDetailGroup.BarCode && x.SellingPrice == salesInvoiceDetailGroup.SellingPrice && x.ItemDiscountPercent == salesInvoiceDetailGroup.ItemDiscountPercent).DefaultIfEmpty()
									 select new
									 {
										 salesInvoiceDetailGroup.ItemId,
										 salesInvoiceDetailGroup.ItemPackageId,
										 SalesInvoiceQuantity = salesInvoiceDetailGroup.Quantity,
										 SalesInvoiceBonusQuantity = salesInvoiceDetailGroup.BonusQuantity,
										 QuantityDisbursed = unInvoicedStock != null ? unInvoicedStock.QuantityDisbursed : 0,
										 BonusQuantityDisbursed = unInvoicedStock != null ? unInvoicedStock.BonusQuantityDisbursed : 0
									 }).FirstOrDefault(x => x.QuantityDisbursed != x.SalesInvoiceQuantity || x.BonusQuantityDisbursed != x.SalesInvoiceBonusQuantity);

			if (unmatchedQuantity != null)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();

				var itemName = await _itemService.GetAll().Where(x => x.ItemId == unmatchedQuantity.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
				var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == unmatchedQuantity.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

				if (unmatchedQuantity.QuantityDisbursed != unmatchedQuantity.SalesInvoiceQuantity)
				{
					return new ResponseDto { Id = salesInvoice.SalesInvoiceHeader.SalesInvoiceHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, MenuCodeData.ProformaInvoice, SalesMessageData.QuantityNotMatchingUnInvoiced, itemName!, itemPackageName!, unmatchedQuantity.SalesInvoiceQuantity.ToNormalizedString(), unmatchedQuantity.QuantityDisbursed.ToNormalizedString()) };
				}
				else
				{
					return new ResponseDto { Id = salesInvoice.SalesInvoiceHeader.SalesInvoiceHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, MenuCodeData.ProformaInvoice, SalesMessageData.BonusQuantityNotMatchingUnInvoiced, itemName!, itemPackageName!, unmatchedQuantity.SalesInvoiceBonusQuantity.ToNormalizedString(), unmatchedQuantity.BonusQuantityDisbursed.ToNormalizedString()) };
				}
			}

			return new ResponseDto { Success = true };
        }

        private async Task<ResponseDto> CheckQuantityDidnotChange(SalesInvoiceDto salesInvoice, int menuCode)
        {
            List<SalesInvoiceDetailDto> salesInvoiceDetails = await _salesInvoiceDetailService.GetSalesInvoiceDetails(salesInvoice.SalesInvoiceHeader!.SalesInvoiceHeaderId);
            var quantityChanged = (from newSalesInvoiceDetail in salesInvoice.SalesInvoiceDetails
                                  from oldSalesInvoiceDetail in salesInvoiceDetails.Where(x => x.SalesInvoiceDetailId == newSalesInvoiceDetail.SalesInvoiceDetailId && (x.Quantity != newSalesInvoiceDetail.Quantity || x.BonusQuantity != newSalesInvoiceDetail.BonusQuantity))
                                  select oldSalesInvoiceDetail).Any();

            return quantityChanged ? 
                new ResponseDto { Success = false, Message = await _salesMessageService.GetMessage(menuCode, SalesMessageData.QuantityCannotBeChanged) } :
                new ResponseDto { Success = true };
        }

		private void TrimDetailStrings(List<SalesInvoiceDetailDto> salesInvoiceDetails)
        {
            foreach (var salesInvoiceDetail in salesInvoiceDetails)
            {
                salesInvoiceDetail.BatchNumber = string.IsNullOrWhiteSpace(salesInvoiceDetail.BatchNumber) ? null : salesInvoiceDetail.BatchNumber.Trim();
            }
        }

        private async Task UpdateModelData(SalesInvoiceDto salesInvoice)
        {
            await UpdateDetailPrices(salesInvoice.SalesInvoiceDetails, salesInvoice.SalesInvoiceHeader!.StoreId, salesInvoice.SalesInvoiceHeader!.SalesInvoiceHeaderId == 0);
            salesInvoice.SalesInvoiceHeader!.TotalCostValue = salesInvoice.SalesInvoiceDetails.Sum(x => x.CostValue);
            await _salesInvoiceService.ModifySalesInvoiceCreditLimits(salesInvoice.SalesInvoiceHeader);
            await AddCostValueJournalsToSalesInvoice(salesInvoice);
        }

        private async Task AddCostValueJournalsToSalesInvoice(SalesInvoiceDto salesInvoice)
        {
            var companyId = await _storeService.GetCompanyIdByStoreId(salesInvoice.SalesInvoiceHeader!.StoreId);
			var inventoryAccountId = await _accountService.GetAll().Where(x => x.CompanyId == companyId && x.AccountTypeId == AccountTypeData.InventoryAccount).Select(x => x.AccountId).FirstOrDefaultAsync();
			var revenueCostAccountId = await _accountService.GetAll().Where(x => x.CompanyId == companyId && x.AccountTypeId == AccountTypeData.RevenuesCostAccount).Select(x => x.AccountId).FirstOrDefaultAsync();

            if (salesInvoice.SalesInvoiceHeader?.SalesInvoiceHeaderId == 0)
            {
                salesInvoice.Journal?.JournalDetails?.Add(new JournalDetailDto
                {
                    JournalDetailId = 0,
                    JournalHeaderId = 0,
                    Serial = 0,
                    AccountId = inventoryAccountId,
                    CurrencyId = salesInvoice.Journal.JournalDetails[0].CurrencyId, //Assuming all details have the same currency, currency rate etc.
                    CurrencyRate = salesInvoice.Journal.JournalDetails[0].CurrencyRate,
                    DebitValue = 0,
                    DebitValueAccount = 0,
                    CreditValue = salesInvoice.SalesInvoiceHeader!.TotalCostValue,
                    CreditValueAccount = salesInvoice.SalesInvoiceHeader!.TotalCostValue * salesInvoice.Journal.JournalDetails[0].CurrencyRate,
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

                salesInvoice.Journal?.JournalDetails?.Add(new JournalDetailDto
                {
                    JournalDetailId = 0,
                    JournalHeaderId = 0,
                    Serial = 0,
                    AccountId = revenueCostAccountId,
                    CurrencyId = salesInvoice.Journal.JournalDetails[0].CurrencyId,
                    CurrencyRate = salesInvoice.Journal.JournalDetails[0].CurrencyRate,
                    DebitValue = salesInvoice.SalesInvoiceHeader!.TotalCostValue,
                    DebitValueAccount = salesInvoice.SalesInvoiceHeader!.TotalCostValue * salesInvoice.Journal.JournalDetails[0].CurrencyRate,
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
                foreach (var journalDetail in salesInvoice.Journal?.JournalDetails!)
                {
                    if (journalDetail.IsCostRelated)
                    {
                        journalDetail.CreditValue = journalDetail.AccountId == inventoryAccountId ? salesInvoice.SalesInvoiceHeader!.TotalCostValue : 0;
                        journalDetail.DebitValue = journalDetail.AccountId == revenueCostAccountId ? salesInvoice.SalesInvoiceHeader!.TotalCostValue : 0;
                        journalDetail.CreditValueAccount = journalDetail.CreditValue * journalDetail.CurrencyRate;
                        journalDetail.DebitValueAccount = journalDetail.DebitValue * journalDetail.CurrencyRate;
                    }
                }
            }
        }

        private async Task UpdateDetailPrices(List<SalesInvoiceDetailDto> salesInvoiceDetails, int storeId, bool isNew) //Todo: use isNew like this for the rest of the documents
        {
            var itemIds = salesInvoiceDetails.Select(x => x.ItemId).ToList();

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

            foreach (var salesInvoiceDetail in salesInvoiceDetails)
            {
                var packing = packings.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.FromPackageId == salesInvoiceDetail.ItemPackageId).Select(x => x.Packing).FirstOrDefault(1);

                if (isNew)
                {
                    salesInvoiceDetail.ConsumerPrice = consumerPrices.Where(x => x.ItemId == salesInvoiceDetail.ItemId).Select(x => x.ConsumerPrice).FirstOrDefault(0);
                    salesInvoiceDetail.CostPrice = itemCosts.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefault(0);
                    salesInvoiceDetail.CostPackage = salesInvoiceDetail.CostPrice * packing;
                    salesInvoiceDetail.LastSalesPrice = lastSellingPrices.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId).Select(x => x.SellingPrice).FirstOrDefault(0);
                }
                salesInvoiceDetail.CostValue = salesInvoiceDetail.CostPackage * (salesInvoiceDetail.Quantity + salesInvoiceDetail.BonusQuantity); //handle quantity changes
            }
        }

        public async Task<ResponseDto> DeleteSalesInvoice(int salesInvoiceHeaderId, int menuCode)
        {
            var salesInvoiceHeader = await _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => new {x.ProformaInvoiceHeaderId, x.IsEnded, x.IsBlocked, x.IsDirectInvoice, x.IsOnTheWay, x.HasSettlement}).FirstOrDefaultAsync();
            if (salesInvoiceHeader == null) return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound)};

            var validationResult = await CheckSalesInvoiceIsValidForDelete(salesInvoiceHeader.IsEnded, salesInvoiceHeader.IsBlocked, salesInvoiceHeader.HasSettlement, menuCode);
            if (validationResult.Success == false) return validationResult;

            //The stockouts must be unlinked from the salesInvoice before deleting so
            //we don't get foreign key violation
            await ApplySalesInvoiceSideEffectsOnDelete(salesInvoiceHeaderId, salesInvoiceHeader.ProformaInvoiceHeaderId, salesInvoiceHeader.IsDirectInvoice);

            var result = await DeleteSalesInvoiceItself(salesInvoiceHeaderId, salesInvoiceHeader.ProformaInvoiceHeaderId, salesInvoiceHeader.IsDirectInvoice, salesInvoiceHeader.IsOnTheWay, menuCode);
            if (result.Success && !salesInvoiceHeader.IsDirectInvoice)
            {
				await _proformaInvoiceStatusService.UpdateProformaInvoiceStatus(salesInvoiceHeader.ProformaInvoiceHeaderId, -1);
			}
            return result;
        }

        private async Task<ResponseDto> DeleteSalesInvoiceItself(int salesInvoiceHeaderId, int proformaInvoiceHeaderId, bool isDirectInvoice, bool isOnTheWay, int menuCode)
        {
            if (isDirectInvoice)
            {
                return await DeleteDirectSalesInvoice(salesInvoiceHeaderId, proformaInvoiceHeaderId, isOnTheWay, menuCode);
            }
            else
            {
                return await _salesInvoiceService.DeleteSalesInvoice(salesInvoiceHeaderId, menuCode);
            }
        }

        private async Task<ResponseDto> DeleteDirectSalesInvoice(int salesInvoiceHeaderId, int proformaInvoiceHeaderId, bool isOnTheWay, int menuCode)
        {
            if (!isOnTheWay)
            {
                var stockOutHeaderId = await GetStockOutHeaderIdLinkedToSalesInvoice(salesInvoiceHeaderId);
                var stockOutResult = await _stockOutService.DeleteStockOut(stockOutHeaderId ?? 0, MenuCodeData.StockOutFromReservation);
                if (stockOutResult.Success == false) return stockOutResult;
            }

            var salesInvoiceResult = await _salesInvoiceService.DeleteSalesInvoice(salesInvoiceHeaderId, menuCode);
            if (salesInvoiceResult.Success == false) return salesInvoiceResult;

            var proformaResult = await _proformaInvoiceService.DeleteProformaInvoice(proformaInvoiceHeaderId);
            if (proformaResult.Success == false) return proformaResult;

            return salesInvoiceResult;
        }

        private async Task ApplySalesInvoiceSideEffectsOnDelete(int salesInvoiceHeaderId, int proformaInvoiceHeaderId, bool isDirectInvoice)
        {
            if (!isDirectInvoice)
            {
                await ReopenProformaInvoice(proformaInvoiceHeaderId);
            }
            await MarkStockOutAndStockOutReturnsAsUnInvoiced(salesInvoiceHeaderId, isDirectInvoice);
		}

        private async Task MarkStockOutAndStockOutReturnsAsUnInvoiced(int salesInvoiceHeaderId, bool isDirectInvoice)
        {
            if (!isDirectInvoice)
            {
                var stockOutHeaderIds = await _invoiceStockOutService.GetStockOutsLinkedToSalesInvoice(salesInvoiceHeaderId).ToListAsync();
                var stockOutReturnHeaderIds = await _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutHeaderId != null && stockOutHeaderIds.Contains((int)x.StockOutHeaderId)).Select(x => x.StockOutReturnHeaderId).ToListAsync();

                await _stockOutHeaderService.UpdateStockOutsEnded(stockOutHeaderIds, false);
                await _stockOutReturnHeaderService.UpdateStockOutReturnsEnded(stockOutReturnHeaderIds, false); 
            }

            await _invoiceStockOutService.UnlinkStockOutsFromSalesInvoice(salesInvoiceHeaderId);
        }

        private async Task ReopenProformaInvoice(int proformaInvoiceHeaderId)
        {
            var hasStockOuts = await _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).AnyAsync();
            var hasSalesInvoices = await _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == proformaInvoiceHeaderId).AnyAsync();

            await _proformaInvoiceHeaderService.UpdateClosed(proformaInvoiceHeaderId, hasStockOuts || hasSalesInvoices);
        }

        public async Task<ResponseDto> CheckSalesInvoiceIsValidForDelete(bool isEnded, bool isBlocked, bool hasSettlement, int menuCode)
        {
			if (isEnded) 
            {
                return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseInvoiced) };
            }

            if (isBlocked)
            {
                return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
            }

            if (hasSettlement)
            {
                return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotDeleteBecauseSettlementStarted) };
            }

            //No need to check for IsClosed when deleting :because database foreign key constraint
			return new ResponseDto { Success = true };
		}
    }
}
