using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Models.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Modules;
using Shared.Helper.Models.Dtos;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Models.Domain.CostCenters;
using Shared.CoreOne.Models.Domain.Items;
using Shared.Helper.Extensions;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Sales.CoreOne.Models.Domain;
using Shared.Service.Logic.Calculation;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using Shared.CoreOne.Contracts.Menus;
using Sales.CoreOne.Models.StaticData;
using Shared.Service.Services.Inventory;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Logic;
using MoreLinq;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Purchases.Service.Services;

namespace Sales.Service.Services
{
    public class StockOutHandlingService: IStockOutHandlingService
    {
		private readonly ISalesMessageService _salesMessageService;
		private readonly IGenericMessageService _genericMessageService;
		private readonly IProformaInvoiceStatusService _proformaInvoiceStatusService;
		private readonly IItemPackageService _itemPackageService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
		private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
	    private readonly ISalesInvoiceDetailTaxService _salesInvoiceDetailTaxService;
	    private readonly IProformaInvoiceDetailTaxService _proformaInvoiceDetailTaxService;
	    private readonly IItemTaxService _itemTaxService;
	    private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IProformaInvoiceDetailService _proformaInvoiceDetailService;
        private readonly IStockOutQuantityService _stockOutQuantityService;
        private readonly IStockOutService _stockOutService;
        private readonly IStockOutHeaderService _stockOutHeaderService;
        private readonly IStockOutDetailService _stockOutDetailService;
        private readonly IStockOutDetailTaxService _stockOutDetailTaxService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IItemCostService _itemCostService;
        private readonly IItemService _itemService;
        private readonly ISalesInvoiceService _salesInvoiceService;
        private readonly IItemPackingService _itemPackingService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly IClientCreditMemoService _clientCreditMemoService;
		private readonly IClientDebitMemoService _clientDebitMemoService;
		private readonly IZeroStockValidationService _zeroStockValidationService;
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
		private readonly IStockOutFromProformaService _stockOutFromProformaService;
		private readonly IStoreService _storeService;
		private readonly IItemNoteValidationService _itemNoteValidationService;

        public StockOutHandlingService(ISalesMessageService salesMessageService, IGenericMessageService genericMessageService, IProformaInvoiceStatusService proformaInvoiceStatusService, IItemPackageService itemPackageService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceDetailService salesInvoiceDetailService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, ISalesInvoiceDetailTaxService salesInvoiceDetailTaxService, IProformaInvoiceDetailTaxService proformaInvoiceDetailTaxService, IItemTaxService itemTaxService, IItemBarCodeService itemBarCodeService, IProformaInvoiceDetailService proformaInvoiceDetailService, IStockOutQuantityService stockOutQuantityService, IStockOutService stockOutService, IStockOutHeaderService stockOutHeaderService, IStockOutDetailService stockOutDetailService, IStockOutDetailTaxService stockOutDetailTaxService, IMenuNoteService menuNoteService, IItemCostService itemCostService, IItemService itemService, ISalesInvoiceService salesInvoiceService, IItemPackingService itemPackingService, IHttpContextAccessor httpContextAccessor, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService, IZeroStockValidationService zeroStockValidationService, IItemCurrentBalanceService itemCurrentBalanceService, IStockOutFromProformaService stockOutFromProformaService, IStoreService storeService, IItemNoteValidationService itemNoteValidationService)
        {
			_salesMessageService = salesMessageService;
			_genericMessageService = genericMessageService;
			_proformaInvoiceStatusService = proformaInvoiceStatusService;
			_itemPackageService = itemPackageService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_salesInvoiceDetailService = salesInvoiceDetailService;
			_proformaInvoiceHeaderService = proformaInvoiceHeaderService;
	        _salesInvoiceDetailTaxService = salesInvoiceDetailTaxService;
			_proformaInvoiceDetailTaxService = proformaInvoiceDetailTaxService;
            _itemTaxService = itemTaxService;
	        _itemBarCodeService = itemBarCodeService;
            _proformaInvoiceDetailService = proformaInvoiceDetailService;
            _stockOutService = stockOutService;
            _stockOutHeaderService = stockOutHeaderService;
            _stockOutDetailService = stockOutDetailService;
            _stockOutDetailTaxService = stockOutDetailTaxService;
            _menuNoteService = menuNoteService;
            _itemCostService = itemCostService;
            _itemService = itemService;
            _salesInvoiceService = salesInvoiceService;
            _itemPackingService = itemPackingService;
            _stockOutQuantityService = stockOutQuantityService;
			_httpContextAccessor = httpContextAccessor;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_clientCreditMemoService = clientCreditMemoService;
			_clientDebitMemoService = clientDebitMemoService;
			_zeroStockValidationService = zeroStockValidationService;
			_itemCurrentBalanceService = itemCurrentBalanceService;
			_stockOutFromProformaService = stockOutFromProformaService;
			_storeService = storeService;
			_itemNoteValidationService = itemNoteValidationService;
        }

		public IQueryable<StockOutHeaderDto> GetStockOutHeadersByStoreId(int storeId, int? clientId, int stockTypeId, int stockOutHeaderId)
		{
			clientId ??= 0;
			if (stockOutHeaderId == 0)
			{
				return from stockOutHeader in _stockOutHeaderService.GetStockOutHeaders().Where(x => x.StoreId == storeId && (clientId == 0 || x.ClientId == clientId) && x.StockTypeId == stockTypeId && x.IsEnded == false && x.IsBlocked == false)
					   from overallQuantityReceived in _stockOutQuantityService.GetOverallQuantityAvailableReturnFromStockOuts().Where(x => x.ParentId == stockOutHeader.StockOutHeaderId && x.Quantity > 0)
					   select stockOutHeader;
			}
			else
			{
				return _stockOutHeaderService.GetStockOutHeaders().Where(x => x.StockOutHeaderId == stockOutHeaderId);
			}
		}

        public async Task<StockOutDto> GetStockOut(int stockOutHeaderId)
        {
            var header = await _stockOutHeaderService.GetStockOutHeaderById(stockOutHeaderId);
            if (header == null) { return new StockOutDto(); }

            var details = await GetStockOutDetailsCalculatedInternal(stockOutHeaderId, header.ProformaInvoiceHeaderId, header.SalesInvoiceHeaderId);
            var menuNotes = await _menuNoteService.GetMenuNotes(StockTypeData.ToMenuCode(header.StockTypeId), stockOutHeaderId).ToListAsync();
            var stockOutDetailTaxes = await _stockOutDetailTaxService.GetStockOutDetailTaxes(stockOutHeaderId).ToListAsync();

			await ModifyStockOutDetailsWithStoreIdAndAvailableBalance(header.StockOutHeaderId, header.StoreId, details, false, false);

            foreach (var detail in details)
            {
                detail.StockOutDetailTaxes = stockOutDetailTaxes.Where(x => x.StockOutDetailId == detail.StockOutDetailId).ToList();
            }

            return new StockOutDto() { StockOutHeader = header, StockOutDetails = details, MenuNotes = menuNotes };
        }

		public async Task ModifyStockOutDetailsWithStoreIdAndAvailableBalance(int stockOutHeaderId, int storeId, List<StockOutDetailDto> details, bool isRequestData, bool isCreate)
		{
			var itemIds = details.Select(x => x.ItemId).ToList();

			var availableBalances = await _itemCurrentBalanceService.GetItemCurrentBalanceInfo(storeId, false, false).Where(x => x.StoreId == storeId && itemIds.Contains(x.ItemId)).ToListAsync();
			var currentStockOutDetails = isCreate ? [] : (isRequestData ? await _stockOutDetailService.GetStockOutDetailsAsQueryable(stockOutHeaderId).ToListAsync() : details);

			var filteredAvailableBalances = availableBalances.Select(x => new { Key = (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber), Quantity = x.AvailableBalance });
			var filteredCurrentDetails = currentStockOutDetails.Select(x => new { Key = (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber), Quantity = x.Quantity + x.BonusQuantity });

			var finalAvailableBalances = filteredAvailableBalances.Concat(filteredCurrentDetails).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));

			foreach (var detail in details)
			{
				detail.StoreId = storeId;
				detail.AvailableBalance = finalAvailableBalances.GetValueOrDefault((detail.ItemId, detail.ItemPackageId, detail.ExpireDate, detail.BatchNumber), 0);
			}
		}

		private ResponseDto GetStockOutFromReservationResult(List<StockOutDetailDto> details)
		{
			var exceedingItemIds = details.GroupBy(x => (x.ItemId, x.ItemName, x.ItemPackageId, x.ItemPackageName, x.ExpireDate, x.BatchNumber)).Select(x => new
			{
				x.Key.ItemId,
				x.Key.ItemName,
				x.Key.ItemPackageId,
				x.Key.ItemPackageName,
				Quantity = x.Sum(y => y.Quantity + y.BonusQuantity),
				AvailableBalance = x.Select(y => y.AvailableBalance).First()
			}).Where(x => x.Quantity > x.AvailableBalance).Select(x => new IncompleteItemDto { ItemId = x.ItemId, ItemName = x.ItemName, ItemPackageId = x.ItemPackageId, ItemPackageName = x.ItemPackageName, Partial = x.Quantity < x.AvailableBalance}).ToList();

			return _stockOutFromProformaService.GenerateResponseMessage(exceedingItemIds);
		}

		public async Task<StockOutWithResponseDto> GetStockOutFromProformaInvoice(int proformaInvoiceHeaderId)
        {
			var proformaInvoiceHeader = await _proformaInvoiceHeaderService.GetProformaInvoiceHeaderById(proformaInvoiceHeaderId);
			if (proformaInvoiceHeader == null)
			{
				return new StockOutWithResponseDto { StockOut = new StockOutDto(), Result = new ResponseDto()};
			}

			var stockOutDetailsResult = await GetStockOutDetailsFromProformaInvoice(proformaInvoiceHeaderId, proformaInvoiceHeader.StoreId, proformaInvoiceHeader.DiscountPercent);
			await ModifyStockOutDetailsWithStoreIdAndAvailableBalance(0, proformaInvoiceHeader.StoreId, stockOutDetailsResult.StockOutDetails, false, true);

			var stockOutHeader = GetStockOutHeaderFromParent(stockOutDetailsResult.StockOutDetails,
				new StockOutParentDto
				{
					StockTypeId = StockTypeData.StockOutFromProformaInvoice,
					ParentHeaderId = proformaInvoiceHeader.ProformaInvoiceHeaderId,
					ParentFullCode = proformaInvoiceHeader.DocumentFullCode,
					ParentDocumentReference = proformaInvoiceHeader.DocumentReference,
					ClientId = proformaInvoiceHeader.ClientId,
					ClientCode = proformaInvoiceHeader.ClientCode,
					ClientName = proformaInvoiceHeader.ClientName,
					SellerId = proformaInvoiceHeader.SellerId,
					SellerCode = proformaInvoiceHeader.SellerCode,
					SellerName = proformaInvoiceHeader.SellerName,
					StoreId = proformaInvoiceHeader.StoreId,
					StoreName = proformaInvoiceHeader.StoreName,
					DocumentDate = proformaInvoiceHeader.DocumentDate,
					Reference = proformaInvoiceHeader.Reference,
					DiscountPercent = proformaInvoiceHeader.DiscountPercent,
					AdditionalDiscountValue = proformaInvoiceHeader.AdditionalDiscountValue,
					RemarksAr = proformaInvoiceHeader.RemarksAr,
					RemarksEn = proformaInvoiceHeader.RemarksEn,
					ArchiveHeaderId = proformaInvoiceHeader.ArchiveHeaderId
				});

			return new StockOutWithResponseDto
			{
				StockOut = new StockOutDto { StockOutHeader = stockOutHeader, StockOutDetails = stockOutDetailsResult.StockOutDetails },
				Result = stockOutDetailsResult.Result
			};
        }

        private StockOutHeaderDto GetStockOutHeaderFromParent(List<StockOutDetailDto> stockOutDetails, StockOutParentDto parent)
        {
	        var totalValueFromDetail = stockOutDetails.Sum(x => x.TotalValue);
	        var totalValueAfterDiscountFromDetail = stockOutDetails.Sum(x => x.TotalValueAfterDiscount);
	        var totalItemDiscount = stockOutDetails.Sum(x => x.ItemDiscountValue);
	        var grossValueFromDetail = stockOutDetails.Sum(x => x.GrossValue);
	        var vatValueFromDetail = stockOutDetails.Sum(x => x.VatValue);
	        var subNetValueFromDetail = stockOutDetails.Sum(x => x.SubNetValue);
	        var otherTaxValueFromDetail = stockOutDetails.Sum(x => x.OtherTaxValue);
	        var netValueFromDetail = stockOutDetails.Sum(x => x.NetValue);
	        var totalCostValueFromDetail = stockOutDetails.Sum(x => x.CostValue);

	        return new StockOutHeaderDto
	        {
		        StockOutHeaderId = 0,
		        StockTypeId = parent.StockTypeId,
		        ProformaInvoiceHeaderId = parent.StockTypeId == StockTypeData.StockOutFromProformaInvoice ? parent.ParentHeaderId : null,
		        ProformaInvoiceFullCode = parent.StockTypeId == StockTypeData.StockOutFromProformaInvoice ? parent.ParentFullCode : null,
		        ProformaInvoiceDocumentReference = parent.StockTypeId == StockTypeData.StockOutFromProformaInvoice ? parent.ParentDocumentReference : null,
		        SalesInvoiceHeaderId = parent.StockTypeId == StockTypeData.StockOutFromSalesInvoice ? parent.ParentHeaderId : null,
		        SalesInvoiceFullCode = parent.StockTypeId == StockTypeData.StockOutFromSalesInvoice ? parent.ParentFullCode : null,
		        SalesInvoiceDocumentReference = parent.StockTypeId == StockTypeData.StockOutFromSalesInvoice ? parent.ParentDocumentReference : null,
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

        public async Task<StockOutWithResponseDto> GetStockOutFromSalesInvoice(int salesInvoiceHeaderId)
        {
			var salesInvoiceHeader = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById(salesInvoiceHeaderId);
			if (salesInvoiceHeader == null)
			{
				return new StockOutWithResponseDto { StockOut = new StockOutDto(), Result = new ResponseDto() };
			}

			var stockOutDetails = await GetStockOutDetailsFromSalesInvoice(salesInvoiceHeaderId, salesInvoiceHeader.DiscountPercent);
			var reservationStoreId = await _storeService.GetReservedStoreByParentStoreId(salesInvoiceHeader.StoreId);
			await ModifyStockOutDetailsWithStoreIdAndAvailableBalance(0, reservationStoreId, stockOutDetails, false, true);

			var stockOutHeader = GetStockOutHeaderFromParent(stockOutDetails,
				new StockOutParentDto
				{
					StockTypeId = StockTypeData.StockOutFromSalesInvoice,
					ParentHeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
					ParentFullCode = salesInvoiceHeader.DocumentFullCode,
					ParentDocumentReference = salesInvoiceHeader.DocumentReference,
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

			var result = GetStockOutFromReservationResult(stockOutDetails);
			return new StockOutWithResponseDto { StockOut = new StockOutDto { StockOutHeader = stockOutHeader, StockOutDetails = stockOutDetails }, Result = result };
		}

		private async Task<StockOutDetailsWithResponseDto> GetStockOutDetailsFromProformaInvoice(int proformaInvoiceHeaderId, int storeId, decimal headerDiscountPercent)
		{
			var result = await _stockOutFromProformaService.GetStockOutDetailsFromProformaInvoice(proformaInvoiceHeaderId, storeId, headerDiscountPercent);

			await GetAuxiliaryData(result.StockOutDetails);
			await CalculateOtherTaxesFromProformaInvoice(result.StockOutDetails, headerDiscountPercent, proformaInvoiceHeaderId);
			SerializeStockOutDetails(result.StockOutDetails);
			return result;
		}

		private async Task<List<StockOutDetailDto>> GetStockOutDetailsFromSalesInvoice(int salesInvoiceHeaderId, decimal headerDiscountPercent)
		{
			var salesInvoiceDetails = await _salesInvoiceDetailService.GetSalesInvoiceDetailsAsQueryable(salesInvoiceHeaderId).ToListAsync();
			var salesInvoiceDetailsGrouped = _salesInvoiceDetailService.GroupSalesInvoiceDetails(salesInvoiceDetails);

			var itemIds = salesInvoiceDetails.Select(x => x.ItemId).ToList();
			var items = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var itemPackings = await _itemPackingService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var lastSalesPrices = await _salesInvoiceService.GetMultipleLastSalesPrices(itemIds);

			var finalStocksDisbursed = await _stockOutQuantityService.GetFinalStocksDisbursedFromSalesInvoice(salesInvoiceHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

			var stockOutDetails = (from salesInvoiceDetail in salesInvoiceDetails
								   from finalStockDisbursed in finalStocksDisbursed.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId && x.BarCode == salesInvoiceDetail.BarCode && x.SellingPrice == salesInvoiceDetail.SellingPrice && x.CostCenterId == salesInvoiceDetail.CostCenterId && x.ExpireDate == salesInvoiceDetail.ExpireDate && x.BatchNumber == salesInvoiceDetail.BatchNumber && x.ItemDiscountPercent == salesInvoiceDetail.ItemDiscountPercent).DefaultIfEmpty()
								   from salesInvoiceGroup in salesInvoiceDetailsGrouped.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId && x.BarCode == salesInvoiceDetail.BarCode && x.SellingPrice == salesInvoiceDetail.SellingPrice && x.CostCenterId == salesInvoiceDetail.CostCenterId && x.ExpireDate == salesInvoiceDetail.ExpireDate && x.BatchNumber == salesInvoiceDetail.BatchNumber && x.ItemDiscountPercent == salesInvoiceDetail.ItemDiscountPercent)
								   from item in items.Where(x => x.ItemId == salesInvoiceDetail.ItemId)
								   from itemPacking in itemPackings.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.FromPackageId == salesInvoiceDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
								   from itemCost in itemCosts.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId).DefaultIfEmpty()
								   from lastSalesPrice in lastSalesPrices.Where(x => x.ItemId == salesInvoiceDetail.ItemId && x.ItemPackageId == salesInvoiceDetail.ItemPackageId).DefaultIfEmpty()
								   select new StockOutDetailDto
								   {
									   StockOutDetailId = salesInvoiceDetail.SalesInvoiceDetailId, // <-- This is used to get the related detail taxes
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
									   AvailableQuantity = salesInvoiceGroup.Quantity - (finalStockDisbursed != null ? finalStockDisbursed.QuantityDisbursed : 0),
									   AvailableBonusQuantity = salesInvoiceGroup.BonusQuantity - (finalStockDisbursed != null ? finalStockDisbursed.BonusQuantityDisbursed : 0),
									   SalesInvoiceQuantity = salesInvoiceGroup.Quantity,
									   SalesInvoiceBonusQuantity = salesInvoiceGroup.BonusQuantity,
									   SellingPrice = salesInvoiceDetail.SellingPrice,
									   ItemDiscountPercent = salesInvoiceDetail.ItemDiscountPercent,
									   VatPercent = salesInvoiceDetail.VatPercent,
									   Notes = salesInvoiceDetail.Notes,
									   ItemNote = salesInvoiceDetail.ItemNote,
									   ConsumerPrice = item.ConsumerPrice,
									   CostPrice = itemCost != null ? itemCost.CostPrice : 0,
									   CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
									   LastSalesPrice = lastSalesPrice != null ? lastSalesPrice.SellingPrice : 0
								   }).ToList();

			stockOutDetails = _stockOutFromProformaService.DistributeQuantityOnStockOutDetails(stockOutDetails, true);
			stockOutDetails = stockOutDetails.Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();

			stockOutDetails = _stockOutFromProformaService.RecalculateStockOutDetailValues(stockOutDetails, headerDiscountPercent);

			await GetAuxiliaryData(stockOutDetails);
			await CalculateOtherTaxesFromSalesInvoice(stockOutDetails, headerDiscountPercent, salesInvoiceHeaderId);
			SerializeStockOutDetails(stockOutDetails);

			return stockOutDetails;
		}

		public async Task<List<StockOutDetailDto>> GetStockOutDetailsCalculated(int stockOutHeaderId)
        {
	        var stockOutHeader = await _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId).Select(x => new { x.ProformaInvoiceHeaderId, x.SalesInvoiceHeaderId }).FirstOrDefaultAsync();

	        return await GetStockOutDetailsCalculatedInternal(stockOutHeaderId, stockOutHeader?.ProformaInvoiceHeaderId, stockOutHeader?.SalesInvoiceHeaderId);
        }

		public async Task<List<StockOutDetailDto>> ModifyStockOutDetailsWithLiveAvailableQuantity(int stockOutHeaderId, int? proformaInvoiceHeaderId, int? salesInvoiceHeaderId, List<StockOutDetailDto> stockOutDetails)
		{
			return await GetStockOutDetailsCalculatedInternal(stockOutHeaderId, proformaInvoiceHeaderId, salesInvoiceHeaderId, stockOutDetails);
		}

        private async Task<List<StockOutDetailDto>> GetStockOutDetailsCalculatedInternal(int stockOutHeaderId, int? proformaInvoiceHeaderId, int? salesInvoiceHeaderId, List<StockOutDetailDto>? stockOutDetails = null)
        {
	        List<StockOutDetailDto> results = [];
            if (proformaInvoiceHeaderId != null)
            {
	            results = await GetStockOutFromProformaInvoiceDetailCalculated(stockOutHeaderId, (int)proformaInvoiceHeaderId, stockOutDetails);
            }
            else if (salesInvoiceHeaderId != null)
            {
				results = await GetStockOutFromSalesInvoiceDetailCalculated(stockOutHeaderId, (int)salesInvoiceHeaderId, stockOutDetails);
			}

            await GetAuxiliaryData(results);

			return results;
        }

        private async Task<List<StockOutDetailDto>> GetStockOutFromProformaInvoiceDetailCalculated(int stockOutHeaderId, int proformaInvoiceHeaderId, List<StockOutDetailDto>? stockOutDetails)
        {
            stockOutDetails ??= await _stockOutDetailService.GetStockOutDetailsAsQueryable(stockOutHeaderId).ToListAsync();
            var itemIds = stockOutDetails.Select(x => x.ItemId).ToList();

            var groupedProformaInvoiceDetails = await _proformaInvoiceDetailService.GetProformaInvoiceDetailsGrouped(proformaInvoiceHeaderId);
            var finalStocksDisbursed = await _stockOutQuantityService.GetFinalStocksDisbursedFromProformaInvoiceExceptStockOutHeaderId(proformaInvoiceHeaderId, stockOutHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

			return (from stockOutDetail in stockOutDetails
					from proformaInvoiceDetail in groupedProformaInvoiceDetails.Where(x => x.ItemId == stockOutDetail.ItemId && x.ItemPackageId == stockOutDetail.ItemPackageId && x.BarCode == stockOutDetail.BarCode && x.SellingPrice == stockOutDetail.SellingPrice && x.ItemDiscountPercent == stockOutDetail.ItemDiscountPercent).DefaultIfEmpty()
					from finalStockDisbursed     in finalStocksDisbursed.Where(x => x.ItemId == stockOutDetail.ItemId && x.ItemPackageId == stockOutDetail.ItemPackageId && x.BarCode == stockOutDetail.BarCode && x.SellingPrice == stockOutDetail.SellingPrice && x.ItemDiscountPercent == stockOutDetail.ItemDiscountPercent).DefaultIfEmpty()
					select new StockOutDetailDto
					{
						StockOutDetailId = stockOutDetail.StockOutDetailId,
						StockOutHeaderId = stockOutDetail.StockOutHeaderId,
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
						AvailableQuantity = (proformaInvoiceDetail != null ? proformaInvoiceDetail.Quantity : 0) - (finalStockDisbursed != null ? finalStockDisbursed.QuantityDisbursed : 0),
						AvailableBonusQuantity = (proformaInvoiceDetail != null ? proformaInvoiceDetail.BonusQuantity : 0) - (finalStockDisbursed != null ? finalStockDisbursed.BonusQuantityDisbursed : 0),
						ProformaInvoiceQuantity = (proformaInvoiceDetail != null ? proformaInvoiceDetail.Quantity : 0),
						ProformaInvoiceBonusQuantity = (proformaInvoiceDetail != null ? proformaInvoiceDetail.BonusQuantity : 0),
						SellingPrice = stockOutDetail.SellingPrice,
						TotalValue = stockOutDetail.TotalValue,
						ItemDiscountPercent = stockOutDetail.ItemDiscountPercent,
						ItemDiscountValue = stockOutDetail.ItemDiscountValue,
						TotalValueAfterDiscount = stockOutDetail.TotalValueAfterDiscount,
						HeaderDiscountValue = stockOutDetail.HeaderDiscountValue,
						GrossValue = stockOutDetail.GrossValue,
						VatPercent = stockOutDetail.VatPercent,
						VatValue = stockOutDetail.VatValue,
						SubNetValue = stockOutDetail.SubNetValue,
						OtherTaxValue = stockOutDetail.OtherTaxValue,
						NetValue = stockOutDetail.NetValue,
						Notes = stockOutDetail.Notes,
						ItemNote = stockOutDetail.ItemNote,
						ConsumerPrice = stockOutDetail.ConsumerPrice,
						CostPrice = stockOutDetail.CostPrice,
						CostPackage = stockOutDetail.CostPackage,
						CostValue = stockOutDetail.CostValue,
						LastSalesPrice = stockOutDetail.LastSalesPrice,

						StockOutDetailTaxes = stockOutDetail.StockOutDetailTaxes,

						CreatedAt = stockOutDetail.CreatedAt,
						IpAddressCreated = stockOutDetail.IpAddressCreated,
						UserNameCreated = stockOutDetail.UserNameCreated,
					}).ToList();
		}

        private async Task<List<StockOutDetailDto>> GetStockOutFromSalesInvoiceDetailCalculated(int stockOutHeaderId, int salesInvoiceHeaderId, List<StockOutDetailDto>? stockOutDetails)
        {
			stockOutDetails ??= await _stockOutDetailService.GetStockOutDetailsAsQueryable(stockOutHeaderId).ToListAsync();
			var itemIds = stockOutDetails.Select(x => x.ItemId).ToList();

			var salesInvoiceDetails = await _salesInvoiceDetailService.GetSalesInvoiceDetailsGrouped(salesInvoiceHeaderId);
			var finalStocksDisbursed = await _stockOutQuantityService.GetFinalStocksDisbursedFromSalesInvoiceExceptStockOutHeaderId(salesInvoiceHeaderId, stockOutHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

			return (from stockOutDetail in stockOutDetails
					from salesInvoiceDetail in salesInvoiceDetails.Where(x => x.ItemId == stockOutDetail.ItemId && x.ItemPackageId == stockOutDetail.ItemPackageId && x.CostCenterId == stockOutDetail.CostCenterId && x.BarCode == stockOutDetail.BarCode && x.SellingPrice == stockOutDetail.SellingPrice && x.BatchNumber == stockOutDetail.BatchNumber && x.ExpireDate == stockOutDetail.ExpireDate && x.ItemDiscountPercent == stockOutDetail.ItemDiscountPercent).DefaultIfEmpty()
					from finalStockDisbursed in finalStocksDisbursed.Where(x => x.ItemId == stockOutDetail.ItemId && x.ItemPackageId == stockOutDetail.ItemPackageId && x.CostCenterId == stockOutDetail.CostCenterId && x.BarCode == stockOutDetail.BarCode && x.SellingPrice == stockOutDetail.SellingPrice && x.BatchNumber == stockOutDetail.BatchNumber && x.ExpireDate == stockOutDetail.ExpireDate && x.ItemDiscountPercent == stockOutDetail.ItemDiscountPercent).DefaultIfEmpty()
					select new StockOutDetailDto
					{
						StockOutDetailId = stockOutDetail.StockOutDetailId,
						StockOutHeaderId = stockOutDetail.StockOutHeaderId,
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
						AvailableQuantity = (salesInvoiceDetail != null ? salesInvoiceDetail.Quantity : 0) - (finalStockDisbursed != null ? finalStockDisbursed.QuantityDisbursed : 0),
						AvailableBonusQuantity = (salesInvoiceDetail != null ? salesInvoiceDetail.BonusQuantity : 0) - (finalStockDisbursed != null ? finalStockDisbursed.BonusQuantityDisbursed : 0),
						SalesInvoiceQuantity = (salesInvoiceDetail != null ? salesInvoiceDetail.Quantity : 0),
						SalesInvoiceBonusQuantity = (salesInvoiceDetail != null ? salesInvoiceDetail.BonusQuantity : 0),
						SellingPrice = stockOutDetail.SellingPrice,
						TotalValue = stockOutDetail.TotalValue,
						ItemDiscountPercent = stockOutDetail.ItemDiscountPercent,
						ItemDiscountValue = stockOutDetail.ItemDiscountValue,
						TotalValueAfterDiscount = stockOutDetail.TotalValueAfterDiscount,
						HeaderDiscountValue = stockOutDetail.HeaderDiscountValue,
						GrossValue = stockOutDetail.GrossValue,
						VatPercent = stockOutDetail.VatPercent,
						VatValue = stockOutDetail.VatValue,
						SubNetValue = stockOutDetail.SubNetValue,
						OtherTaxValue = stockOutDetail.OtherTaxValue,
						NetValue = stockOutDetail.NetValue,
						Notes = stockOutDetail.Notes,
						ItemNote = stockOutDetail.ItemNote,
						ConsumerPrice = stockOutDetail.ConsumerPrice,
						CostPrice = stockOutDetail.CostPrice,
						CostPackage = stockOutDetail.CostPackage,
						CostValue = stockOutDetail.CostValue,
						LastSalesPrice = stockOutDetail.LastSalesPrice,

						StockOutDetailTaxes = stockOutDetail.StockOutDetailTaxes,

						CreatedAt = stockOutDetail.CreatedAt,
						IpAddressCreated = stockOutDetail.IpAddressCreated,
						UserNameCreated = stockOutDetail.UserNameCreated,
					}).ToList();
		}

        private async Task GetAuxiliaryData(List<StockOutDetailDto> stockOutDetails)
        {
	        var itemIds = stockOutDetails.Select(x => x.ItemId).ToList();
	        var packages = await _itemBarCodeService.GetItemsPackages(itemIds);
	        var itemTaxData = await _itemTaxService.GetItemTaxDataByItemIds(itemIds);

	        foreach (var stockOutDetail in stockOutDetails)
	        {
		        stockOutDetail.Packages = packages.Where(x => x.ItemId == stockOutDetail.ItemId).Select(s => s.Packages).FirstOrDefault().ToJson();
		        stockOutDetail.ItemTaxData = itemTaxData.Where(x => x.ItemId == stockOutDetail.ItemId).ToList();
		        stockOutDetail.Taxes = stockOutDetail.ItemTaxData.ToJson();
	        }
		}

        private async Task CalculateOtherTaxesFromProformaInvoice(List<StockOutDetailDto> stockOutDetails, decimal headerDiscountPercent, int proformaInvoiceHeaderId)
        {
	        var proformaInvoiceDetailTaxes = await _proformaInvoiceDetailTaxService.GetProformaInvoiceDetailTaxes(proformaInvoiceHeaderId).ToListAsync();
	        foreach (var stockOutDetail in stockOutDetails)
	        {
		        stockOutDetail.StockOutDetailTaxes = (
			        from itemTax in proformaInvoiceDetailTaxes.Where(x =>
				        x.ProformaInvoiceDetailId == stockOutDetail.StockOutDetailId)
			        select new StockOutDetailTaxDto
			        {
				        TaxId = itemTax.TaxId,
				        CreditAccountId = itemTax.CreditAccountId,
				        TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
				        TaxPercent = itemTax.TaxPercent,
				        TaxValue = CalculateDetailValue.TaxValue(stockOutDetail.Quantity, stockOutDetail.SellingPrice,
					        stockOutDetail.ItemDiscountPercent, stockOutDetail.VatPercent, itemTax.TaxPercent,
					        itemTax.TaxAfterVatInclusive, headerDiscountPercent, stockOutDetail.IsItemVatInclusive)
			        }
		        ).ToList();

		        stockOutDetail.OtherTaxValue = stockOutDetail.StockOutDetailTaxes.Sum(x => x.TaxValue);
		        stockOutDetail.NetValue = CalculateDetailValue.NetValue(stockOutDetail.Quantity,
			        stockOutDetail.SellingPrice, stockOutDetail.ItemDiscountPercent, stockOutDetail.VatPercent,
			        stockOutDetail.OtherTaxValue, headerDiscountPercent, stockOutDetail.IsItemVatInclusive);
	        }
		}

        private async Task CalculateOtherTaxesFromSalesInvoice(List<StockOutDetailDto> stockOutDetails, decimal headerDiscountPercent, int salesInvoiceHeaderId)
        {
	        var salesInvoiceDetailTaxes = await _salesInvoiceDetailTaxService.GetSalesInvoiceDetailTaxes(salesInvoiceHeaderId).ToListAsync();
	        foreach (var stockOutDetail in stockOutDetails)
	        {
		        stockOutDetail.StockOutDetailTaxes = (
			        from itemTax in salesInvoiceDetailTaxes.Where(x => x.SalesInvoiceDetailId == stockOutDetail.StockOutDetailId)
			        select new StockOutDetailTaxDto
			        {
				        TaxId = itemTax.TaxId,
				        CreditAccountId = itemTax.CreditAccountId,
				        TaxAfterVatInclusive = itemTax.TaxAfterVatInclusive,
				        TaxPercent = itemTax.TaxPercent,
				        TaxValue = CalculateDetailValue.TaxValue(stockOutDetail.Quantity, stockOutDetail.SellingPrice,
					        stockOutDetail.ItemDiscountPercent, stockOutDetail.VatPercent, itemTax.TaxPercent,
					        itemTax.TaxAfterVatInclusive, headerDiscountPercent, stockOutDetail.IsItemVatInclusive)
			        }
		        ).ToList();

		        stockOutDetail.OtherTaxValue = stockOutDetail.StockOutDetailTaxes.Sum(x => x.TaxValue);
		        stockOutDetail.NetValue = CalculateDetailValue.NetValue(stockOutDetail.Quantity, stockOutDetail.SellingPrice, stockOutDetail.ItemDiscountPercent, stockOutDetail.VatPercent, stockOutDetail.OtherTaxValue, headerDiscountPercent, stockOutDetail.IsItemVatInclusive);
	        }
		}

        private void SerializeStockOutDetails(List<StockOutDetailDto> stockOutDetails)
        {
	        int newId = -1;
	        int newSubId = -1;
	        foreach (var stockOutDetail in stockOutDetails)
	        {
		        stockOutDetail.StockOutDetailId = newId;

		        stockOutDetail.StockOutDetailTaxes.ForEach(y =>
		        {
			        y.StockOutDetailId = newId;
			        y.StockOutDetailTaxId = newSubId--;
		        });

		        newId--;
	        }
        }

		public async Task<int> GetParentMenuCode(int? proformaInvoiceHeaderId, int? salesInvoiceHeaderId)
		{
			if (proformaInvoiceHeaderId != null)
			{
				return MenuCodeData.ProformaInvoice;
			}
			else
			{
				var salesInvoiceHeader = await _salesInvoiceHeaderService.GetSalesInvoiceHeaderById((int)salesInvoiceHeaderId!);
				return SalesInvoiceMenuCodeHelper.GetMenuCode(salesInvoiceHeader!);
			}
		}

		public async Task<ResponseDto> SaveStockOut(StockOutDto stockOut, bool hasApprove, bool approved, int? requestId, bool affectBalances = true, string? documentReference = null, bool shouldInitializeFlags = false)
		{
			TrimDetailStrings(stockOut.StockOutDetails);

			var menuCode = StockTypeData.ToMenuCode(stockOut.StockOutHeader!.StockTypeId);
			var parentMenuCode = await GetParentMenuCode(stockOut.StockOutHeader!.ProformaInvoiceHeaderId, stockOut.StockOutHeader!.SalesInvoiceHeaderId);
			var validationResult = await CheckStockOutIsValidForSave(stockOut, menuCode, parentMenuCode);
			if (validationResult.Success == false) return validationResult;

			if (stockOut.StockOutHeader!.StockOutHeaderId == 0)
			{
				await UpdateModelPrices(stockOut);
			}

			var result = affectBalances ?
						await _stockOutService.SaveStockOut(stockOut, hasApprove, approved, requestId, documentReference, shouldInitializeFlags) :
						await _stockOutService.SaveStockOutWithoutUpdatingBalances(stockOut, hasApprove, approved, requestId, documentReference, shouldInitializeFlags);

			if (result.Success == true)
			{
				await ApplyStockOutSideEffects(stockOut.StockOutHeader.StockTypeId, stockOut.StockOutHeader.ProformaInvoiceHeaderId, stockOut.StockOutHeader.SalesInvoiceHeaderId);
			}

			return result;
		}

		public async Task<ResponseDto> CheckStockOutIsValidForSave(StockOutDto stockOut, int menuCode, int parentMenuCode)
		{
			ResponseDto result;
			if (stockOut.StockOutHeader?.StockOutHeaderId != 0)
			{
				result = await CheckStockOutClosed(stockOut.StockOutHeader!.StockOutHeaderId, menuCode);
				if (result.Success == false) return result;

				result = await CheckStockOutEnded(stockOut.StockOutHeader!.StockOutHeaderId, menuCode);
				if (result.Success == false) return result;
			}
			else
			{
				result = await CheckSalesInvoiceHasReservationInvoiceCloseOut(stockOut.StockOutHeader!.SalesInvoiceHeaderId, menuCode, parentMenuCode);
				if (result.Success == false) return result;

				result = await CheckSalesInvoiceHasClientCreditOrDebitMemo(stockOut.StockOutHeader!.SalesInvoiceHeaderId, menuCode, parentMenuCode);
				if (result.Success == false) return result;

				result = await CheckProformaHasAnyDirectOrReservationInvoice(stockOut.StockOutHeader!.ProformaInvoiceHeaderId, menuCode, parentMenuCode);
				if (result.Success == false) return result;
			}

			result = await CheckProformaInvoiceOrSalesInvoiceBlocked(stockOut.StockOutHeader!.ProformaInvoiceHeaderId, stockOut.StockOutHeader!.SalesInvoiceHeaderId, menuCode);
			if (result.Success == false) return result;

			result = await _itemNoteValidationService.CheckItemNoteWithItemType(stockOut.StockOutDetails, x => x.ItemId, x => x.ItemNote);
			if (result.Success == false) return result;

			result = await CheckStockOutQuantityForSaving(stockOut, menuCode, parentMenuCode);
			if (result.Success == false) return result;

			result = await CheckStockOutZeroStock(stockOut.StockOutHeader!.StockOutHeaderId, stockOut.StockOutHeader!.StoreId, stockOut.StockOutDetails, menuCode, parentMenuCode);
			if (result.Success == false) return result;

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutClosed(int stockOutHeaderId, int menuCode)
		{
			var isClosed = await _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId).Select(x => (bool?)x.IsClosed).FirstOrDefaultAsync();
			if (isClosed == null)
			{
				return new ResponseDto { Id = stockOutHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };
			}

			if (isClosed == true)
			{
				return new ResponseDto { Id = stockOutHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotUpdateBecauseClosed) };
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckStockOutEnded(int stockOutHeaderId, int menuCode)
		{
			var isEnded = await _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId).Select(x => x.IsEnded).FirstOrDefaultAsync();

			if (isEnded)
			{
				return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseInvoiced) };
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckSalesInvoiceHasReservationInvoiceCloseOut(int? salesInvoiceHeaderId, int menuCode, int parentMenuCode)
		{
			if (salesInvoiceHeaderId != null)
			{
				var hasReservationInvoiceCloseOut = await _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId && x.IsOnTheWay).AnyAsync();

				if (hasReservationInvoiceCloseOut)
				{
					return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, MenuCodeData.ReservationInvoiceCloseOut, parentMenuCode, GenericMessageData.HasDocument) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckSalesInvoiceHasClientCreditOrDebitMemo(int? salesInvoiceHeaderId, int menuCode, int parentMenuCode)
		{
			if (salesInvoiceHeaderId != null)
			{
				var clientCredits = _clientCreditMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => MenuCodeData.ClientCreditMemo);
				var clientDebits = _clientDebitMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => MenuCodeData.SupplierDebitMemo);

				var creditOrDebitMenuCode = await clientCredits.Concat(clientDebits).FirstOrDefaultAsync();

				if (creditOrDebitMenuCode != 0)
				{
					return new ResponseDto { Success = false, Message = await _genericMessageService.GetMessage(menuCode, creditOrDebitMenuCode, parentMenuCode, GenericMessageData.HasDocument) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckProformaHasAnyDirectOrReservationInvoice(int? proformaInvoiceHeaderId, int menuCode, int parentMenuCode)
		{
			if (proformaInvoiceHeaderId != null)
			{
				//all reservation invoices are direct invoices
				var directInvoice = await _salesInvoiceHeaderService.GetSalesInvoiceHeaders().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId && x.IsDirectInvoice).FirstOrDefaultAsync();

				if (directInvoice != null)
				{
					return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, SalesInvoiceMenuCodeHelper.GetMenuCode(directInvoice), parentMenuCode, GenericMessageData.HasDocument) };
				}
			}

			return new ResponseDto { Success = true };
		}

		private async Task<ResponseDto> CheckProformaInvoiceOrSalesInvoiceBlocked(int? proformaInvoiceHeaderId, int? salesInvoiceHeaderId, int menuCode)
		{
			var isBlocked = await (
					from proformaInvoiceHeader in _proformaInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).DefaultIfEmpty()
					from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).DefaultIfEmpty()
					select (proformaInvoiceHeader != null && proformaInvoiceHeader.IsBlocked) || (salesInvoiceHeader != null && salesInvoiceHeader.IsBlocked)
				).FirstOrDefaultAsync();

			if (isBlocked)
			{
				return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
			}

			return new ResponseDto { Success = true };
		}

		private async Task ApplyStockOutSideEffects(int stockTypeId, int? proformaInvoiceHeaderId, int? salesInvoiceHeaderId)
		{
			if (stockTypeId == StockTypeData.StockOutFromProformaInvoice)
			{
				await _proformaInvoiceHeaderService.UpdateClosed(proformaInvoiceHeaderId, true);
			}
			else
			{
				await _salesInvoiceHeaderService.UpdateClosed(salesInvoiceHeaderId, true);
			}

			await UpdateProformaInvoiceStatusFromStockOut(proformaInvoiceHeaderId, salesInvoiceHeaderId);
		}

		private async Task UpdateProformaInvoiceStatusFromStockOut(int? proformaInvoiceHeaderId, int? salesInvoiceHeaderId)
		{
			var affectedProformaInvoiceHeaderId = await GetProformaInvoiceRelatedToStockOut(proformaInvoiceHeaderId, salesInvoiceHeaderId);
			await _proformaInvoiceStatusService.UpdateProformaInvoiceStatus(affectedProformaInvoiceHeaderId, -1);
		}

		private async Task<int> GetProformaInvoiceRelatedToStockOut(int? proformaInvoiceHeaderId, int? salesInvoiceHeaderId)
		{
			if (proformaInvoiceHeaderId != null) return (int)proformaInvoiceHeaderId;

			return await _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.ProformaInvoiceHeaderId).FirstOrDefaultAsync();
		}

		private void TrimDetailStrings(List<StockOutDetailDto> stockOutDetails)
        {
            foreach (var stockOutDetail in stockOutDetails)
            {
                stockOutDetail.BatchNumber = string.IsNullOrWhiteSpace(stockOutDetail.BatchNumber) ? null : stockOutDetail.BatchNumber.Trim();
            }
        }

        private async Task UpdateModelPrices(StockOutDto stockOut)
        {
            await UpdateDetailPrices(stockOut.StockOutDetails, stockOut.StockOutHeader!.StoreId);
            stockOut.StockOutHeader!.TotalCostValue = stockOut.StockOutDetails.Sum(x => x.CostValue);
        }

        private async Task UpdateDetailPrices(List<StockOutDetailDto> stockOutDetails, int storeId)
        {
            var itemIds = stockOutDetails.Select(x => x.ItemId).ToList();

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

            foreach (var stockOutDetail in stockOutDetails)
            {
                var packing = packings.Where(x => x.ItemId == stockOutDetail.ItemId && x.FromPackageId == stockOutDetail.ItemPackageId).Select(x => x.Packing).FirstOrDefault(1);

                stockOutDetail.ConsumerPrice = consumerPrices.Where(x => x.ItemId == stockOutDetail.ItemId).Select(x => x.ConsumerPrice).FirstOrDefault(0);
                stockOutDetail.CostPrice = itemCosts.Where(x => x.ItemId == stockOutDetail.ItemId && x.StoreId == storeId).Select(x => x.CostPrice).FirstOrDefault(0);
                stockOutDetail.CostPackage = stockOutDetail.CostPrice * packing;
                stockOutDetail.CostValue = stockOutDetail.CostPackage * (stockOutDetail.Quantity + stockOutDetail.BonusQuantity);
                stockOutDetail.LastSalesPrice = lastSellingPrices.Where(x => x.ItemId == stockOutDetail.ItemId && x.ItemPackageId == stockOutDetail.ItemPackageId).Select(x => x.SellingPrice).FirstOrDefault(0);
            }
        }

		private async Task<ResponseDto> CheckStockOutQuantityForSaving(StockOutDto stockOut, int menuCode, int parentMenuCode)
		{
			ResponseDto result;
			if (stockOut.StockOutHeader!.StockTypeId == StockTypeData.StockOutFromProformaInvoice)
			{
				result = await CheckStockOutFromProformaInvoiceQuantityExceeding(stockOut, menuCode, parentMenuCode);
			}
			else
			{
				result = await CheckStockOutFromSalesInvoiceQuantityExceeding(stockOut, menuCode, parentMenuCode);
			}

			return result;
		}

		//only check for zero stock when saving, because delete will always increase the available balance
		private async Task<ResponseDto> CheckStockOutZeroStock(int stockOutHeaderId, int storeId, List<StockOutDetailDto> stockDetails, int menuCode, int parentMenuCode)
		{
			var oldStockDetails = stockOutHeaderId != 0 ? 
				await _stockOutDetailService.GetStockOutDetailsAsQueryable(stockOutHeaderId).ToListAsync() : [];

			var storeIdToCheck = menuCode == MenuCodeData.StockOutFromProformaInvoice ? storeId : await _storeService.GetReservedStoreByParentStoreId(storeId);
			int settingMenuCode = menuCode == MenuCodeData.StockOutFromProformaInvoice ? menuCode : parentMenuCode;

			return await _zeroStockValidationService.ValidateZeroStock(
				storeId: storeIdToCheck,
				newDetails: stockDetails,
				oldDetails: oldStockDetails,
				detailKeySelector: x => (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber),
				itemIdSelector: x => x.ItemId,
				quantitySelector: x => x.Quantity + x.BonusQuantity,
				availableBalanceKeySelector: x => (x.ItemId, x.ItemPackageId, x.ExpireDate, x.BatchNumber),
				isGrouped: false,
				menuCode: menuCode,
				settingMenuCode: settingMenuCode,
				isSave: true);
		}

		private async Task<ResponseDto> CheckStockOutFromProformaInvoiceQuantityExceeding(StockOutDto stockOut, int menuCode, int parentMenuCode)
		{
			if (stockOut.StockOutHeader == null || !(stockOut.StockOutHeader.ProformaInvoiceHeaderId > 0)) return new ResponseDto { Message = "StockType mismatches parent foreign keys" };

			var stockOutDetailGrouped = _stockOutDetailService.GroupStockOutDetails(stockOut.StockOutDetails);
			var itemIds = stockOutDetailGrouped.Select(x => x.ItemId).ToList();

			List<StockDisbursedFromProformaInvoiceDto>? finalstocksDisbursed;
			if (stockOut.StockOutHeader.StockOutHeaderId == 0)
			{
				finalstocksDisbursed = await _stockOutQuantityService.GetFinalStocksDisbursedFromProformaInvoice((int)stockOut.StockOutHeader.ProformaInvoiceHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			}
			else
			{
				finalstocksDisbursed = await _stockOutQuantityService.GetFinalStocksDisbursedFromProformaInvoiceExceptStockOutHeaderId((int)stockOut.StockOutHeader.ProformaInvoiceHeaderId, stockOut.StockOutHeader.StockOutHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			}

			var proformaInvoiceDetailsGrouped = await _proformaInvoiceDetailService.GetProformaInvoiceDetailsGrouped((int)stockOut.StockOutHeader.ProformaInvoiceHeaderId);

			var availableQuantities = (from stockOutDetail in stockOutDetailGrouped
									   from finalstockDisbursed       in           finalstocksDisbursed.Where(x => x.ItemId == stockOutDetail.ItemId && x.ItemPackageId == stockOutDetail.ItemPackageId && x.BarCode == stockOutDetail.BarCode && x.SellingPrice == stockOutDetail.SellingPrice && x.ItemDiscountPercent == stockOutDetail.ItemDiscountPercent).DefaultIfEmpty()
									   from proformaInvoiceDetailGroup in proformaInvoiceDetailsGrouped.Where(x => x.ItemId == stockOutDetail.ItemId && x.ItemPackageId == stockOutDetail.ItemPackageId && x.BarCode == stockOutDetail.BarCode && x.SellingPrice == stockOutDetail.SellingPrice && x.ItemDiscountPercent == stockOutDetail.ItemDiscountPercent).DefaultIfEmpty()
									   select new
									   {
										   stockOutDetail.ItemId,
										   stockOutDetail.ItemPackageId,
										   ProformaInvoiceQuantity = proformaInvoiceDetailGroup != null ? proformaInvoiceDetailGroup.Quantity : 0,
										   ProformaInvoiceBonusQuantity = proformaInvoiceDetailGroup != null ? proformaInvoiceDetailGroup.BonusQuantity : 0,
										   QuantityDisbursed = (finalstockDisbursed != null ? finalstockDisbursed.QuantityDisbursed : 0) + stockOutDetail.Quantity,
										   BonusQuantityDisbursed = (finalstockDisbursed != null ? finalstockDisbursed.BonusQuantityDisbursed : 0) + stockOutDetail.BonusQuantity,
										   QuantityAvailable = (proformaInvoiceDetailGroup != null ? proformaInvoiceDetailGroup.Quantity : 0) - (finalstockDisbursed != null ? finalstockDisbursed.QuantityDisbursed : 0) - stockOutDetail.Quantity,
										   BonusQuantityAvailable = (proformaInvoiceDetailGroup != null ? proformaInvoiceDetailGroup.BonusQuantity : 0) - (finalstockDisbursed != null ? finalstockDisbursed.BonusQuantityDisbursed : 0) - stockOutDetail.BonusQuantity
									   });

			var exceedingItem = availableQuantities.FirstOrDefault(x => x.QuantityAvailable < 0 || x.BonusQuantityAvailable < 0);
			if (exceedingItem != null)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();

				var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
				var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

				if (exceedingItem.QuantityAvailable < 0)
				{
					return new ResponseDto { Id = stockOut.StockOutHeader.StockOutHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.QuantityExceeding, itemName!, itemPackageName!, exceedingItem.QuantityDisbursed.ToNormalizedString(), exceedingItem.ProformaInvoiceQuantity.ToNormalizedString()) };
				}
				else
				{
					return new ResponseDto { Id = stockOut.StockOutHeader.StockOutHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.BonusQuantityExceeding, itemName!, itemPackageName!, exceedingItem.BonusQuantityDisbursed.ToNormalizedString(), exceedingItem.ProformaInvoiceBonusQuantity.ToNormalizedString()) };
				}
			}
			else
			{
				return new ResponseDto { Id = stockOut.StockOutHeader.StockOutHeaderId, Success = true };
			}
		}

		private async Task<ResponseDto> CheckStockOutFromSalesInvoiceQuantityExceeding(StockOutDto stockOut, int menuCode, int parentMenuCode)
		{
			if (stockOut.StockOutHeader == null || !(stockOut.StockOutHeader.SalesInvoiceHeaderId > 0)) return new ResponseDto { Message = "StockType mismatches parent foreign keys" };
			
			var stockOutDetailGrouped = _stockOutDetailService.GroupStockOutDetailsWithAllKeys(stockOut.StockOutDetails);
			var itemIds = stockOut.StockOutDetails.Select(x => x.ItemId).ToList();

			List<StockDisbursedFromSalesInvoiceDto>? finalstocksDisbursed;
			if (stockOut.StockOutHeader.StockOutHeaderId == 0)
			{
				finalstocksDisbursed = await _stockOutQuantityService.GetFinalStocksDisbursedFromSalesInvoice((int)stockOut.StockOutHeader.SalesInvoiceHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			}
			else
			{
				finalstocksDisbursed = await _stockOutQuantityService.GetFinalStocksDisbursedFromSalesInvoiceExceptStockOutHeaderId((int)stockOut.StockOutHeader.SalesInvoiceHeaderId, stockOut.StockOutHeader.StockOutHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			}

			var salesInvoiceDetailsGrouped = await _salesInvoiceDetailService.GetSalesInvoiceDetailsGrouped((int)stockOut.StockOutHeader.SalesInvoiceHeaderId);

			var availableQuantities = (from stockOutDetail in stockOutDetailGrouped
									   from finalstockDisbursed      in      finalstocksDisbursed.Where(x => x.ItemId == stockOutDetail.ItemId && x.ItemPackageId == stockOutDetail.ItemPackageId && x.CostCenterId == stockOutDetail.CostCenterId && x.BarCode == stockOutDetail.BarCode && x.SellingPrice == stockOutDetail.SellingPrice && x.ExpireDate == stockOutDetail.ExpireDate && x.BatchNumber == stockOutDetail.BatchNumber && x.ItemDiscountPercent == stockOutDetail.ItemDiscountPercent).DefaultIfEmpty()
									   from salesInvoiceDetailGroup in salesInvoiceDetailsGrouped.Where(x => x.ItemId == stockOutDetail.ItemId && x.ItemPackageId == stockOutDetail.ItemPackageId && x.CostCenterId == stockOutDetail.CostCenterId && x.BarCode == stockOutDetail.BarCode && x.SellingPrice == stockOutDetail.SellingPrice && x.ExpireDate == stockOutDetail.ExpireDate && x.BatchNumber == stockOutDetail.BatchNumber && x.ItemDiscountPercent == stockOutDetail.ItemDiscountPercent).DefaultIfEmpty()
									   select new
									   {
										   stockOutDetail.ItemId,
										   stockOutDetail.ItemPackageId,
										   SalesInvoiceQuantity = salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.Quantity : 0,
										   SalesInvoiceBonusQuantity = salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.BonusQuantity : 0,
										   QuantityDisbursed = (finalstockDisbursed != null ? finalstockDisbursed.QuantityDisbursed : 0) + stockOutDetail.Quantity,
										   BonusQuantityDisbursed = (finalstockDisbursed != null ? finalstockDisbursed.BonusQuantityDisbursed : 0) + stockOutDetail.BonusQuantity,
										   QuantityAvailable = (salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.Quantity : 0) - (finalstockDisbursed != null ? finalstockDisbursed.QuantityDisbursed : 0) - stockOutDetail.Quantity,
										   BonusQuantityAvailable = (salesInvoiceDetailGroup != null ? salesInvoiceDetailGroup.BonusQuantity : 0) - (finalstockDisbursed != null ? finalstockDisbursed.BonusQuantityDisbursed : 0) - stockOutDetail.BonusQuantity
									   });

			var exceedingItem = availableQuantities.FirstOrDefault(x => x.QuantityAvailable < 0 || x.BonusQuantityAvailable < 0);
			if (exceedingItem != null)
			{
				var language = _httpContextAccessor.GetProgramCurrentLanguage();

				var itemName = await _itemService.GetAll().Where(x => x.ItemId == exceedingItem.ItemId).Select(x => language == LanguageCode.Arabic ? x.ItemNameAr : x.ItemNameEn).FirstOrDefaultAsync();
				var itemPackageName = await _itemPackageService.GetAll().Where(x => x.ItemPackageId == exceedingItem.ItemPackageId).Select(x => language == LanguageCode.Arabic ? x.PackageNameAr : x.PackageNameEn).FirstOrDefaultAsync();

				if (exceedingItem.QuantityAvailable < 0)
				{
					return new ResponseDto { Id = stockOut.StockOutHeader.StockOutHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.QuantityExceeding, itemName!, itemPackageName!, exceedingItem.QuantityDisbursed.ToNormalizedString(), exceedingItem.SalesInvoiceQuantity.ToNormalizedString()) };
				}
				else
				{
					return new ResponseDto { Id = stockOut.StockOutHeader.StockOutHeaderId, Success = false, Message = await _salesMessageService.GetMessage(menuCode, parentMenuCode, SalesMessageData.BonusQuantityExceeding, itemName!, itemPackageName!, exceedingItem.BonusQuantityDisbursed.ToNormalizedString(), exceedingItem.SalesInvoiceBonusQuantity.ToNormalizedString()) };
				}
			}
			else
			{
				return new ResponseDto { Id = stockOut.StockOutHeader.StockOutHeaderId, Success = true };
			}
		}

		public async Task<ResponseDto> DeleteStockOut(int stockOutHeaderId, int menuCode, bool affectBalances = true)
        {
			var stockOutHeader = await _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId).Select(x => new { x.StockOutHeaderId, x.ProformaInvoiceHeaderId, x.SalesInvoiceHeaderId, x.IsEnded, x.IsBlocked }).FirstOrDefaultAsync();
			if (stockOutHeader == null) return new ResponseDto { Id = stockOutHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };

			ResponseDto validationResult = await CheckStockOutIsValidForDelete(stockOutHeader.StockOutHeaderId, stockOutHeader.ProformaInvoiceHeaderId, stockOutHeader.SalesInvoiceHeaderId, stockOutHeader.IsBlocked, stockOutHeader.IsEnded, menuCode);
			if (validationResult.Success == false) return validationResult;

			var result = affectBalances ? 
				await _stockOutService.DeleteStockOut(stockOutHeaderId, menuCode) :
				await _stockOutService.DeleteStockOutWithoutUpdatingBalances(stockOutHeaderId, menuCode);

			if (result.Success == true)
			{
				await ReopenStockOutParent(stockOutHeader.ProformaInvoiceHeaderId, stockOutHeader.SalesInvoiceHeaderId);
				await UpdateProformaInvoiceStatusFromStockOut(stockOutHeader.ProformaInvoiceHeaderId, stockOutHeader.SalesInvoiceHeaderId);
			}

			return result;
		}

		public async Task<ResponseDto> CheckStockOutIsValidForDelete(int stockOutHeaderId, int? proformaInvoiceHeaderId, int? salesInvoiceHeaderId, bool isBlocked, bool isEnded, int menuCode)
		{
			if (isBlocked == true)
			{
				return new ResponseDto { Id = stockOutHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseStoppedDealingOn) };
			}

			if (isEnded == true)
			{
				return new ResponseDto { Id = stockOutHeaderId, Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.CannotPerformOperationBecauseInvoiced) };
			}
			
			return new ResponseDto { Success = true };
		}

		private async Task ReopenStockOutParent(int? proformaInvoiceHeaderId, int? salesInvoiceHeaderId)
		{
			if (proformaInvoiceHeaderId != null)
			{
				var isStocksRemaining = await _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).AnyAsync();
				if (!isStocksRemaining)
				{
					await _proformaInvoiceHeaderService.UpdateClosed(proformaInvoiceHeaderId, false);
				}
			}
			else if (salesInvoiceHeaderId != null)
			{
				var isStocksRemaining = await _stockOutHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).AnyAsync();
				if (!isStocksRemaining)
				{
					await _salesInvoiceHeaderService.UpdateClosed(salesInvoiceHeaderId, false);
				}
			}
		}
    }
}
