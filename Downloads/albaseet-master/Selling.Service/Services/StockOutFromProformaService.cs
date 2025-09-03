using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Purchases.Service.Services;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Calculation;
using Shared.Service.Services.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Sales.Service.Services
{
	public  class StockOutFromProformaService: IStockOutFromProformaService
	{
		private readonly IProformaInvoiceDetailService _proformaInvoiceDetailService;
		private readonly IItemService _itemService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IItemPackingService _itemPackingService;
		private readonly IItemCostService _itemCostService;
		private readonly ISalesInvoiceService _salesInvoiceService;
		private readonly IStockOutQuantityService _stockOutQuantityService;
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
		private readonly IZeroStockSettingService _zeroStockSettingService;
		private readonly ISalesMessageService _salesMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public StockOutFromProformaService(IProformaInvoiceDetailService proformaInvoiceDetailService, IItemService itemService, IItemPackingService itemPackingService, IItemCostService itemCostService, ISalesInvoiceService salesInvoiceService, IStockOutQuantityService stockOutQuantityService, IItemCurrentBalanceService itemCurrentBalanceService, IZeroStockSettingService zeroStockSettingService, ISalesMessageService salesMessageService, IHttpContextAccessor httpContextAccessor, IItemPackageService itemPackageService) 
		{
			_proformaInvoiceDetailService = proformaInvoiceDetailService;
			_itemService = itemService;
			_itemPackingService = itemPackingService;
			_itemCostService = itemCostService;
			_salesInvoiceService = salesInvoiceService;
			_stockOutQuantityService = stockOutQuantityService;
			_itemCurrentBalanceService = itemCurrentBalanceService;
			_zeroStockSettingService = zeroStockSettingService;
			_salesMessageService = salesMessageService;
			_httpContextAccessor = httpContextAccessor;
			_itemPackageService = itemPackageService;
		}

		public async Task<StockOutDetailsWithResponseDto> GetStockOutDetailsFromProformaInvoice(int proformaInvoiceHeaderId, int storeId, decimal headerDiscountPercent)
		{
			var proformaInvoiceDetails = await _proformaInvoiceDetailService.GetProformaInvoiceDetailsAsQueryable(proformaInvoiceHeaderId).ToListAsync();
			var groupedProformaInvoiceDetails = _proformaInvoiceDetailService.GroupProformaInvoiceDetails(proformaInvoiceDetails);

			var itemIds = proformaInvoiceDetails.Select(x => x.ItemId).ToList();
			var items = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var itemPackings = await _itemPackingService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var lastSalesPrices = await _salesInvoiceService.GetMultipleLastSalesPrices(itemIds);

			var finalStocksDisbursed = await _stockOutQuantityService.GetFinalStocksDisbursedFromProformaInvoice(proformaInvoiceHeaderId).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

			var stockOutDetails = (from proformaInvoiceDetail in proformaInvoiceDetails
								   from finalStockDisbursed in finalStocksDisbursed.Where(x => x.ItemId == proformaInvoiceDetail.ItemId && x.ItemPackageId == proformaInvoiceDetail.ItemPackageId && x.BarCode == proformaInvoiceDetail.BarCode && x.SellingPrice == proformaInvoiceDetail.SellingPrice && x.ItemDiscountPercent == proformaInvoiceDetail.ItemDiscountPercent).DefaultIfEmpty()
								   from proformaInvoiceGrouped in groupedProformaInvoiceDetails.Where(x => x.ItemId == proformaInvoiceDetail.ItemId && x.ItemPackageId == proformaInvoiceDetail.ItemPackageId && x.BarCode == proformaInvoiceDetail.BarCode && x.SellingPrice == proformaInvoiceDetail.SellingPrice && x.ItemDiscountPercent == proformaInvoiceDetail.ItemDiscountPercent)
								   from item in items.Where(x => x.ItemId == proformaInvoiceDetail.ItemId)
								   from itemPacking in itemPackings.Where(x => x.ItemId == proformaInvoiceDetail.ItemId && x.FromPackageId == proformaInvoiceDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
								   from itemCost in itemCosts.Where(x => x.ItemId == proformaInvoiceDetail.ItemId && x.ItemPackageId == proformaInvoiceDetail.ItemPackageId).DefaultIfEmpty()
								   from lastSalesPrice in lastSalesPrices.Where(x => x.ItemId == proformaInvoiceDetail.ItemId && x.ItemPackageId == proformaInvoiceDetail.ItemPackageId).DefaultIfEmpty()
								   select new StockOutDetailDto
								   {
									   StockOutDetailId = proformaInvoiceDetail.ProformaInvoiceDetailId, // <-- This is used to get the related detail taxes
									   CostCenterId = proformaInvoiceDetail.CostCenterId,
									   CostCenterName = proformaInvoiceDetail.CostCenterName,
									   ItemId = proformaInvoiceDetail.ItemId,
									   ItemCode = proformaInvoiceDetail.ItemCode,
									   ItemName = proformaInvoiceDetail.ItemName,
									   TaxTypeId = proformaInvoiceDetail.TaxTypeId,
									   ItemTypeId = proformaInvoiceDetail.ItemTypeId,
									   ItemPackageId = proformaInvoiceDetail.ItemPackageId,
									   ItemPackageName = proformaInvoiceDetail.ItemPackageName,
									   IsItemVatInclusive = proformaInvoiceDetail.IsItemVatInclusive,
									   BarCode = proformaInvoiceDetail.BarCode,
									   Packing = proformaInvoiceDetail.Packing,
									   Quantity = proformaInvoiceDetail.Quantity,
									   BonusQuantity = proformaInvoiceDetail.BonusQuantity,
									   AvailableQuantity = proformaInvoiceGrouped.Quantity - (finalStockDisbursed != null ? finalStockDisbursed.QuantityDisbursed : 0),
									   AvailableBonusQuantity = proformaInvoiceGrouped.BonusQuantity - (finalStockDisbursed != null ? finalStockDisbursed.BonusQuantityDisbursed : 0),
									   ProformaInvoiceQuantity = proformaInvoiceGrouped.Quantity,
									   ProformaInvoiceBonusQuantity = proformaInvoiceGrouped.BonusQuantity,
									   SellingPrice = proformaInvoiceDetail.SellingPrice,
									   ItemDiscountPercent = proformaInvoiceDetail.ItemDiscountPercent,
									   VatPercent = proformaInvoiceDetail.VatPercent,
									   Notes = proformaInvoiceDetail.Notes,
									   ItemNote = proformaInvoiceDetail.ItemNote,
									   ConsumerPrice = item.ConsumerPrice,
									   CostPrice = itemCost != null ? itemCost.CostPrice : 0,
									   CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
									   LastSalesPrice = lastSalesPrice != null ? lastSalesPrice.SellingPrice : 0
								   }).ToList();

			DistributeQuantityOnStockOutDetails(stockOutDetails, false);

			var stockOutDetailsWithResponseDto = new StockOutDetailsWithResponseDto { StockOutDetails = stockOutDetails, Result = new ResponseDto { Success = true } };
			
			var sellWithZeroStockFlag = await _zeroStockSettingService.GetZeroStockSettingByMenuCode(MenuCodeData.StockOutFromProformaInvoice, storeId);
			if (!sellWithZeroStockFlag)
			{
				stockOutDetailsWithResponseDto = await DistributeQuantityOnAvailableBatches(stockOutDetails, storeId);
			}

			stockOutDetailsWithResponseDto.StockOutDetails = stockOutDetailsWithResponseDto.StockOutDetails.Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();
			RecalculateStockOutDetailValues(stockOutDetailsWithResponseDto.StockOutDetails, headerDiscountPercent);
			return stockOutDetailsWithResponseDto;
		}

		public async Task<StockOutDetailsWithResponseDto> DistributeQuantityOnAvailableBatches(List<StockOutDetailDto> details, int storeId)
		{
			var itemIds = details.Select(x => x.ItemId).ToList();
			var availableBalances = await _itemCurrentBalanceService.GetItemCurrentBalanceInfo(storeId, false, false).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

			var result = DistributeQuantityOnAvailableBatchesInternal(details, availableBalances);

			var response = GenerateResponseMessage(result.incompleteItemIds);

			return new StockOutDetailsWithResponseDto { StockOutDetails = result.StockOutDetails, Result = response};
		}

		public ResponseDto GenerateResponseMessage(List<IncompleteItemDto> incompleteItems)
		{
			var response = new ResponseDto();

			response.Success = incompleteItems.Count == 0;
			if (response.Success == false)
			{
				StringBuilder messages = new StringBuilder();
				var language = _httpContextAccessor.GetProgramCurrentLanguage();

				foreach (var item in incompleteItems)
				{
					if (item.Partial)
					{
						messages.AppendLine(
								_salesMessageService.GetMessage(SalesMessageData.CannotPartiallyFulfillItemFromStore, item.ItemName ?? "", item.ItemPackageName ?? "")
							);
					}
					else
					{
						messages.AppendLine(
								_salesMessageService.GetMessage(SalesMessageData.CannotCompletelyFulfillItemFromStore, item.ItemName ?? "", item.ItemPackageName ?? "")
							);
					}
				}

				response.Message = messages.ToString();
			}

			return response;
		}

		public static StockOutDetailsAndListofIncompleteItems DistributeQuantityOnAvailableBatchesInternal(List<StockOutDetailDto> details, List<ItemCurrentBalanceDto> availableBalances)
		{
			var itemIds = details.Select(x => x.ItemId).ToList();

			var groupedAvailableBalances = availableBalances.GroupBy(x => (x.ItemId, x.ItemPackageId)).ToDictionary(
				keySelector: x => x.Key,
				elementSelector: x => x.OrderByDescending(y => y.ExpireDate.HasValue).ThenBy(y => y.ExpireDate).ToList() //nulls should come last
			);

			var modifiedDetails = new List<StockOutDetailDto>();

			var incompleteItemIds = new List<IncompleteItemDto>();
			var visitedItemIds = new List<(int,int)>();

			foreach (var detail in details)
			{
				var balanceGroup = groupedAvailableBalances.GetValueOrDefault((detail.ItemId, detail.ItemPackageId), []);
				decimal remainingProformaQuantity = detail.Quantity;
				decimal remainingProformaBonusQuantity = detail.BonusQuantity;
				int index = 0;

				//distribute quantity
				while (index < balanceGroup.Count && remainingProformaQuantity - balanceGroup[index].AvailableBalance > 0)
				{
					if (balanceGroup[index].AvailableBalance > 0)
					{
						var newDetail = CopyStockOutDetail(detail);
						newDetail.ExpireDate = balanceGroup[index].ExpireDate;
						newDetail.BatchNumber = balanceGroup[index].BatchNumber;

						newDetail.Quantity = balanceGroup[index].AvailableBalance;
						newDetail.BonusQuantity = 0;

						remainingProformaQuantity -= balanceGroup[index].AvailableBalance;
						modifiedDetails.Add(newDetail);

						balanceGroup[index].AvailableBalance -= newDetail.Quantity;
					}
					index++;
				}

				//mix quantity and bonus quantity
				if (index < balanceGroup.Count && remainingProformaQuantity > 0)
				{
					if (balanceGroup[index].AvailableBalance > 0)
					{
						var newDetail = CopyStockOutDetail(detail);
						newDetail.ExpireDate = balanceGroup[index].ExpireDate;
						newDetail.BatchNumber = balanceGroup[index].BatchNumber;

						newDetail.Quantity = remainingProformaQuantity;
						newDetail.BonusQuantity = Math.Min(remainingProformaBonusQuantity, balanceGroup[index].AvailableBalance - newDetail.Quantity);

						remainingProformaQuantity = 0;
						remainingProformaBonusQuantity -= Math.Min(remainingProformaBonusQuantity, balanceGroup[index].AvailableBalance - newDetail.Quantity);
						modifiedDetails.Add(newDetail);

						balanceGroup[index].AvailableBalance -= newDetail.Quantity + newDetail.BonusQuantity;
					}
					index++;

				}

				//distribute remaining bonus quantity
				while (index < balanceGroup.Count && remainingProformaBonusQuantity - balanceGroup[index].AvailableBalance > 0)
				{
					if (balanceGroup[index].AvailableBalance > 0)
					{
						var newDetail = CopyStockOutDetail(detail);
						newDetail.ExpireDate = balanceGroup[index].ExpireDate;
						newDetail.BatchNumber = balanceGroup[index].BatchNumber;

						newDetail.Quantity = 0;
						newDetail.BonusQuantity = balanceGroup[index].AvailableBalance;

						remainingProformaBonusQuantity -= balanceGroup[index].AvailableBalance;
						modifiedDetails.Add(newDetail);

						balanceGroup[index].AvailableBalance -= newDetail.BonusQuantity;
					}
					index++;
				}

				//excess bonus quantity
				if (index < balanceGroup.Count && remainingProformaBonusQuantity > 0)
				{
					if (balanceGroup[index].AvailableBalance > 0)
					{
						var newDetail = CopyStockOutDetail(detail);
						newDetail.ExpireDate = balanceGroup[index].ExpireDate;
						newDetail.BatchNumber = balanceGroup[index].BatchNumber;

						newDetail.Quantity = 0;
						newDetail.BonusQuantity = remainingProformaBonusQuantity;

						remainingProformaBonusQuantity -= Math.Min(remainingProformaBonusQuantity, balanceGroup[index].AvailableBalance - remainingProformaQuantity);
						modifiedDetails.Add(newDetail);

						balanceGroup[index].AvailableBalance -= newDetail.BonusQuantity;
					}
					index++;
				}

				if ((remainingProformaQuantity > 0 || remainingProformaBonusQuantity > 0) && !incompleteItemIds.Select(x => (x.ItemId, x.ItemPackageId)).Contains((detail.ItemId, detail.ItemPackageId)))
				{
					var wasVisitedBefore = visitedItemIds.Contains((detail.ItemId, detail.ItemPackageId));
					var isPartial = wasVisitedBefore ? true : remainingProformaQuantity < detail.Quantity || remainingProformaBonusQuantity < detail.BonusQuantity;
					incompleteItemIds.Add(new IncompleteItemDto { ItemId = detail.ItemId, ItemName = detail.ItemName, ItemPackageId = detail.ItemPackageId, ItemPackageName = detail.ItemPackageName, Partial = isPartial});
				}

				visitedItemIds.Add((detail.ItemId, detail.ItemPackageId));
			}

			return new StockOutDetailsAndListofIncompleteItems { StockOutDetails = modifiedDetails, incompleteItemIds = incompleteItemIds };
		}

		private static StockOutDetailDto CopyStockOutDetail(StockOutDetailDto stockOutDetail)
		{
			var newStockOutDetail = new StockOutDetailDto
			{
				StockOutDetailId = stockOutDetail.StockOutDetailId,
				StoreId = stockOutDetail.StoreId,
				AvailableBalance = stockOutDetail.AvailableBalance,
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
				ProformaInvoiceQuantity = stockOutDetail.ProformaInvoiceQuantity,
				ProformaInvoiceBonusQuantity = stockOutDetail.ProformaInvoiceBonusQuantity,
				SalesInvoiceQuantity = stockOutDetail.SalesInvoiceQuantity,
				SalesInvoiceBonusQuantity = stockOutDetail.SalesInvoiceBonusQuantity,
				AvailableQuantity = stockOutDetail.AvailableQuantity,
				AvailableBonusQuantity = stockOutDetail.AvailableBonusQuantity,
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

				CreatedAt = stockOutDetail.CreatedAt,
				UserNameCreated = stockOutDetail.UserNameCreated,
				IpAddressCreated = stockOutDetail.IpAddressCreated,
				Packages = stockOutDetail.Packages,
				Taxes = stockOutDetail.Taxes,
				ItemTaxData = stockOutDetail.ItemTaxData,
				StockOutDetailTaxes = CopyStockOutDetailTaxes(stockOutDetail.StockOutDetailTaxes),
			};

			return newStockOutDetail;
		}

		static List<StockOutDetailTaxDto> CopyStockOutDetailTaxes(List<StockOutDetailTaxDto> detailTaxes)
		{
			return detailTaxes.Select(x => new StockOutDetailTaxDto
			{
				StockOutDetailTaxId = x.StockOutDetailTaxId,
				StockOutDetailId = x.StockOutDetailId,
				TaxId = x.TaxId,
				TaxAfterVatInclusive = x.TaxAfterVatInclusive,
				CreditAccountId = x.CreditAccountId,
				TaxPercent = x.TaxPercent,
				TaxValue = x.TaxValue,
			}).ToList();
		}

		public List<StockOutDetailDto> RecalculateStockOutDetailValues(List<StockOutDetailDto> stockOutDetails, decimal headerDiscountPercent)
		{
			RecalculateDetailValue.RecalculateDetailValues(
				details: stockOutDetails,
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

			return stockOutDetails;
		}

		public List<StockOutDetailDto> DistributeQuantityOnStockOutDetails(List<StockOutDetailDto> stockOutDetails, bool useAllKeys)
		{
			QuantityDistributionLogic.DistributeQuantitiesOnDetails(
				details: stockOutDetails,
				keySelector: x => (x.ItemId, x.ItemPackageId, x.BarCode, (useAllKeys? x.CostCenterId : null), x.SellingPrice, x.ItemDiscountPercent, (useAllKeys ? x.ExpireDate : null), (useAllKeys ? x.BatchNumber : null)),
				availableQuantitySelector: x => x.AvailableQuantity,
				availableBonusQuantitySelector: x => x.AvailableBonusQuantity,
				quantitySelector: x => x.Quantity,
				bonusQuantitySelector: x => x.BonusQuantity,
				quantityAssigner: (x, value) => x.Quantity = value,
				bonusQuantityAssigner: (x, value) => x.BonusQuantity = value
			);

			return stockOutDetails;
		}
	}
}
