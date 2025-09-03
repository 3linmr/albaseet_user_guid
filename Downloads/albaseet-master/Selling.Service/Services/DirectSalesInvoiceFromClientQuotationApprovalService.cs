using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Purchases.Service.Services;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.Domain.Modules;
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
	public  class DirectSalesInvoiceFromClientQuotationApprovalService : IDirectSalesInvoiceFromClientQuotationApprovalService
	{
		private readonly IClientQuotationApprovalDetailService _clientQuotationApprovalDetailService;
		private readonly IItemService _itemService;
		private readonly IItemPackingService _itemPackingService;
		private readonly IItemCostService _itemCostService;
		private readonly ISalesInvoiceService _salesInvoiceService;
		private readonly IStockOutQuantityService _salesInvoiceQuantityService;
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
		private readonly IZeroStockSettingService _zeroStockSettingService;
		private readonly ISalesMessageService _salesMessageService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStockOutFromProformaService _stockOutFromProformaService;
		private readonly ITaxPercentService _taxPercentService;

		public DirectSalesInvoiceFromClientQuotationApprovalService(IClientQuotationApprovalDetailService clientQuotationApprovalDetailService, IItemService itemService, IItemPackingService itemPackingService, IItemCostService itemCostService, ISalesInvoiceService salesInvoiceService, IStockOutQuantityService salesInvoiceQuantityService, IItemCurrentBalanceService itemCurrentBalanceService, IZeroStockSettingService zeroStockSettingService, ISalesMessageService salesMessageService, IHttpContextAccessor httpContextAccessor, IStockOutFromProformaService stockOutFromProformaService, ITaxPercentService taxPercentService)
		{
			_clientQuotationApprovalDetailService = clientQuotationApprovalDetailService;
			_itemService = itemService;
			_itemPackingService = itemPackingService;
			_itemCostService = itemCostService;
			_salesInvoiceService = salesInvoiceService;
			_salesInvoiceQuantityService = salesInvoiceQuantityService;
			_itemCurrentBalanceService = itemCurrentBalanceService;
			_zeroStockSettingService = zeroStockSettingService;
			_salesMessageService = salesMessageService;
			_httpContextAccessor = httpContextAccessor;
			_stockOutFromProformaService = stockOutFromProformaService;
			_taxPercentService = taxPercentService;
		}

		public async Task<SalesInvoiceDetailsWithResponseDto> GetSalesInvoiceDetailsFromClientQuotationApproval(int clientQuotationApprovalHeaderId, int storeId, decimal headerDiscountPercent, bool isOnTheWay, bool isCreditPayment)
		{
			var clientQuotationApprovalDetails = await _clientQuotationApprovalDetailService.GetClientQuotationApprovalDetailsAsQueryable(clientQuotationApprovalHeaderId).ToListAsync();

			var itemIds = clientQuotationApprovalDetails.Select(x => x.ItemId).ToList();
			var items = await _itemService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var itemPackings = await _itemPackingService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var itemCosts = await _itemCostService.GetAll().Where(x => itemIds.Contains(x.ItemId)).ToListAsync();
			var lastSalesPrices = await _salesInvoiceService.GetMultipleLastSalesPrices(itemIds);

            var vatTaxId = (await _taxPercentService.GetVatByStoreId(storeId, DateTime.Now)).TaxId; //The date doesn't matter, I only want the taxId

			var salesInvoiceDetails = (from clientQuotationApprovalDetail in clientQuotationApprovalDetails
									   from item in items.Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId)
									   from itemPacking in itemPackings.Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId && x.FromPackageId == clientQuotationApprovalDetail.ItemPackageId && x.ToPackageId == item.SingularPackageId)
									   from itemCost in itemCosts.Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId && x.ItemPackageId == clientQuotationApprovalDetail.ItemPackageId).DefaultIfEmpty()
									   from lastSalesPrice in lastSalesPrices.Where(x => x.ItemId == clientQuotationApprovalDetail.ItemId && x.ItemPackageId == clientQuotationApprovalDetail.ItemPackageId).DefaultIfEmpty()
									   select new SalesInvoiceDetailDto
									   {
										   SalesInvoiceDetailId = clientQuotationApprovalDetail.ClientQuotationApprovalDetailId, // <-- used for detail taxes
										   ItemId = clientQuotationApprovalDetail.ItemId,
										   ItemCode = clientQuotationApprovalDetail.ItemCode,
										   ItemName = clientQuotationApprovalDetail.ItemName,
										   ItemTypeId = item.ItemTypeId,
										   TaxTypeId = clientQuotationApprovalDetail.TaxTypeId,
										   ItemPackageId = clientQuotationApprovalDetail.ItemPackageId,
										   ItemPackageName = clientQuotationApprovalDetail.ItemPackageName,
										   IsItemVatInclusive = clientQuotationApprovalDetail.IsItemVatInclusive,
										   CostCenterId = clientQuotationApprovalDetail.CostCenterId,
										   CostCenterName = clientQuotationApprovalDetail.CostCenterName,
										   BarCode = clientQuotationApprovalDetail.BarCode,
										   Packing = clientQuotationApprovalDetail.Packing,
										   Quantity = clientQuotationApprovalDetail.Quantity,
										   BonusQuantity = 0,
										   SellingPrice = clientQuotationApprovalDetail.SellingPrice,
										   ItemDiscountPercent = clientQuotationApprovalDetail.ItemDiscountPercent,
										   VatPercent = clientQuotationApprovalDetail.VatPercent,
										   Notes = clientQuotationApprovalDetail.Notes,
										   ItemNote = clientQuotationApprovalDetail.ItemNote,
										   VatTaxId = vatTaxId,
										   VatTaxTypeId = item.TaxTypeId,
										   ConsumerPrice = item.ConsumerPrice,
										   CostPrice = itemCost != null ? itemCost.CostPrice : 0,
										   CostPackage = itemCost != null ? itemCost.CostPrice * itemPacking.Packing : 0,
										   LastSalesPrice = lastSalesPrice != null ? lastSalesPrice.SellingPrice : 0
									   }).ToList();

			var salesInvoiceDetailsWithResponseDto = new SalesInvoiceDetailsWithResponseDto { SalesInvoiceDetails = salesInvoiceDetails, Result = new ResponseDto { Success = true } };

			var sellWithZeroStockFlag = await _zeroStockSettingService.GetZeroStockSettingByMenuCode(SalesInvoiceMenuCodeHelper.GetMenuCode(isOnTheWay, true, isCreditPayment), storeId);
			if (!sellWithZeroStockFlag)
			{
				salesInvoiceDetailsWithResponseDto = await DistributeQuantityOnAvailabileBatches(salesInvoiceDetails, storeId);
			}

			salesInvoiceDetailsWithResponseDto.SalesInvoiceDetails = salesInvoiceDetailsWithResponseDto.SalesInvoiceDetails.Where(x => x.Quantity > 0 || x.BonusQuantity > 0).ToList();
			RecalculateSalesInvoiceDetailValues(salesInvoiceDetailsWithResponseDto.SalesInvoiceDetails, headerDiscountPercent);
			return salesInvoiceDetailsWithResponseDto;
		}

		public async Task<SalesInvoiceDetailsWithResponseDto> DistributeQuantityOnAvailabileBatches(List<SalesInvoiceDetailDto> details, int storeId)
		{
			var itemIds = details.Select(x => x.ItemId).ToList();
			var availableBalances = await _itemCurrentBalanceService.GetItemCurrentBalanceInfo(storeId, false, false).Where(x => itemIds.Contains(x.ItemId)).ToListAsync();

			var result = DistributeQuantityOnAvailableBatchesInternal(details, availableBalances);

			var response = _stockOutFromProformaService.GenerateResponseMessage(result.incompleteItemIds);

			return new SalesInvoiceDetailsWithResponseDto { SalesInvoiceDetails = result.SalesInvoiceDetails, Result = response};
		}

		public static SalesInvoiceDetailsAndListofIncompleteItems DistributeQuantityOnAvailableBatchesInternal(List<SalesInvoiceDetailDto> details, List<ItemCurrentBalanceDto> availableBalances)
		{
			var groupedAvailableBalances = availableBalances.GroupBy(x => (x.ItemId, x.ItemPackageId)).ToDictionary(
				keySelector: x => x.Key,
				elementSelector: x => x.OrderByDescending(y => y.ExpireDate.HasValue).ThenBy(y => y.ExpireDate).ToList() //nulls should come last
			);

			var modifiedDetails = new List<SalesInvoiceDetailDto>();

			var incompleteItemIds = new List<IncompleteItemDto>();
			var visitedItemIds = new List<(int, int)>();

			foreach (var detail in details)
			{
				var balanceGroup = groupedAvailableBalances.GetValueOrDefault((detail.ItemId, detail.ItemPackageId), []);
				decimal remainingClientQuotationApprovalQuantity = detail.Quantity;
				int index = 0;

				//distribute quantity
				while (index < balanceGroup.Count && remainingClientQuotationApprovalQuantity - balanceGroup[index].AvailableBalance > 0)
				{
					if (balanceGroup[index].AvailableBalance > 0)
					{
						var newDetail = CopySalesInvoiceDetail(detail);
						newDetail.ExpireDate = balanceGroup[index].ExpireDate;
						newDetail.BatchNumber = balanceGroup[index].BatchNumber;

						newDetail.Quantity = balanceGroup[index].AvailableBalance;
						newDetail.BonusQuantity = 0;

						remainingClientQuotationApprovalQuantity -= balanceGroup[index].AvailableBalance;
						modifiedDetails.Add(newDetail);

						balanceGroup[index].AvailableBalance -= newDetail.Quantity;
					}
					index++;
				}

				//excess quantity
				if (index < balanceGroup.Count && remainingClientQuotationApprovalQuantity > 0)
				{
					if (balanceGroup[index].AvailableBalance > 0)
					{
						var newDetail = CopySalesInvoiceDetail(detail);
						newDetail.ExpireDate = balanceGroup[index].ExpireDate;
						newDetail.BatchNumber = balanceGroup[index].BatchNumber;

						newDetail.Quantity = remainingClientQuotationApprovalQuantity;
						newDetail.BonusQuantity = 0;

						remainingClientQuotationApprovalQuantity = 0;
						modifiedDetails.Add(newDetail);

						balanceGroup[index].AvailableBalance -= newDetail.Quantity;
					}
					index++;
				}

				if (remainingClientQuotationApprovalQuantity > 0 && !incompleteItemIds.Select(x => (x.ItemId, x.ItemPackageId)).Contains((detail.ItemId, detail.ItemPackageId)))
				{
					var wasVisitedBefore = visitedItemIds.Contains((detail.ItemId, detail.ItemPackageId));
					var isPartial = wasVisitedBefore ? true : remainingClientQuotationApprovalQuantity < detail.Quantity;
					incompleteItemIds.Add(new IncompleteItemDto { ItemId = detail.ItemId, ItemName = detail.ItemName, ItemPackageId = detail.ItemPackageId, ItemPackageName = detail.ItemPackageName, Partial = isPartial });
				}

				visitedItemIds.Add((detail.ItemId, detail.ItemPackageId));
			}

			return new SalesInvoiceDetailsAndListofIncompleteItems { SalesInvoiceDetails = modifiedDetails, incompleteItemIds = incompleteItemIds };
		}

		private static SalesInvoiceDetailDto CopySalesInvoiceDetail(SalesInvoiceDetailDto salesInvoiceDetail)
		{
			var newSalesInvoiceDetail = new SalesInvoiceDetailDto
			{
				SalesInvoiceDetailId = salesInvoiceDetail.SalesInvoiceDetailId,
				SalesInvoiceHeaderId = salesInvoiceDetail.SalesInvoiceHeaderId,
				StoreId = salesInvoiceDetail.StoreId,
				AvailableBalance = salesInvoiceDetail.AvailableBalance,
				ItemId = salesInvoiceDetail.ItemId,
				ItemCode = salesInvoiceDetail.ItemCode,
				ItemName = salesInvoiceDetail.ItemName,
				ItemTypeId = salesInvoiceDetail.ItemTypeId,
				TaxTypeId = salesInvoiceDetail.TaxTypeId,
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
				VatTaxId = salesInvoiceDetail.VatTaxId,
				VatTaxTypeId = salesInvoiceDetail.VatTaxTypeId,
				ConsumerPrice = salesInvoiceDetail.ConsumerPrice,
				CostPrice = salesInvoiceDetail.CostPrice,
				CostPackage = salesInvoiceDetail.CostPackage,
				CostValue = salesInvoiceDetail.CostValue,
				LastSalesPrice = salesInvoiceDetail.LastSalesPrice,

				CreatedAt = salesInvoiceDetail.CreatedAt,
				UserNameCreated = salesInvoiceDetail.UserNameCreated,
				IpAddressCreated = salesInvoiceDetail.IpAddressCreated,
				Packages = salesInvoiceDetail.Packages,
				Taxes = salesInvoiceDetail.Taxes,
				ItemTaxData = salesInvoiceDetail.ItemTaxData,
				SalesInvoiceDetailTaxes = CopySalesInvoiceDetailTaxes(salesInvoiceDetail.SalesInvoiceDetailTaxes)
			};

			return newSalesInvoiceDetail;
		}

		private static List<SalesInvoiceDetailTaxDto> CopySalesInvoiceDetailTaxes(List<SalesInvoiceDetailTaxDto> detailTaxes)
		{
			return detailTaxes.Select(x => new SalesInvoiceDetailTaxDto
			{
				SalesInvoiceDetailTaxId = x.SalesInvoiceDetailTaxId,
				SalesInvoiceDetailId = x.SalesInvoiceDetailId,
				TaxId = x.TaxId,
				TaxTypeId = x.TaxTypeId,
				TaxAfterVatInclusive = x.TaxAfterVatInclusive,
				CreditAccountId = x.CreditAccountId,
				TaxPercent = x.TaxPercent,
				TaxValue = x.TaxValue,
			}).ToList();
		}

		public List<SalesInvoiceDetailDto> RecalculateSalesInvoiceDetailValues(List<SalesInvoiceDetailDto> salesInvoiceDetails, decimal headerDiscountPercent)
		{
			RecalculateDetailValue.RecalculateDetailValues(
				details: salesInvoiceDetails,
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

			return salesInvoiceDetails;
		}
	}
}
