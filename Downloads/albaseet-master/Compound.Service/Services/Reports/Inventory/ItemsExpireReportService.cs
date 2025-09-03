using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Contracts.Reports.Inventory;
using Compound.CoreOne.Contracts.Reports.Shared;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Identity;
using Microsoft.AspNetCore.Http;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Compound.CoreOne.Models.Dtos.Reports.Shared;
using Compound.CoreOne.Models.Dtos.Reports.Inventory;
using Microsoft.EntityFrameworkCore;
using Shared.Helper.Logic;

namespace Compound.Service.Services.Reports.Inventory
{
	public class ItemsExpireReportService: IItemsExpireReportService
	{
		private readonly IStockTakingsReportService _stockTakingsReportService;

		public ItemsExpireReportService(IStockTakingsReportService stockTakingsReportService)
		{
			_stockTakingsReportService = stockTakingsReportService;
		}

		public async Task<IQueryable<ItemsExpireReportDto>> GetItemsExpireReport(List<int> storeIds, DateTime expireBefore)
		{
			var today = DateHelper.GetDateTimeNow().Date; 

			return from stock in await _stockTakingsReportService.GetStockTakingsReport(storeIds, null, null, null, null, null, null, null, expireBefore)
				   where stock.EndCurrentBalance > 0 || stock.EndAvailableBalance > 0 || stock.ReservedQuantity > 0
				   select new ItemsExpireReportDto
                   {
                       ItemId = stock.ItemId,
                       ItemCode = stock.ItemCode,
                       ItemName = stock.ItemName,
                       ItemNameAr = stock.ItemNameAr,
                       ItemNameEn = stock.ItemNameEn,
                       ItemTypeId = stock.ItemTypeId,
                       ItemTypeName = stock.ItemTypeName,
                   
                       BeginCurrentBalance = stock.BeginCurrentBalance,
                       BeginAvailableBalance = stock.BeginAvailableBalance,
                       InQuantity = stock.InQuantity,
                       OutQuantity = stock.OutQuantity,
                       PendingOutQuantity = stock.PendingOutQuantity,
                       EndCurrentBalance = stock.EndCurrentBalance,
                       EndAvailableBalance = stock.EndAvailableBalance,
                       ReservedQuantity = stock.ReservedQuantity,
                   
                       CostPrice = stock.CostPrice,
                       Packing = stock.Packing,
                       CostPackage = stock.CostPackage,
                       CostValue = stock.CostValue,
                       SellingPackage = stock.SellingPackage,
                       SellingValue = stock.SellingValue,
                   
                       ReorderPointQuantity = stock.ReorderPointQuantity,
                   
                       VendorId = stock.VendorId,
                       VendorCode = stock.VendorCode,
                       VendorName = stock.VendorName,
                   
                       TaxTypeName = stock.TaxTypeName,
                       TaxValue = stock.TaxValue,
                   
                       StoreId = stock.StoreId,
                       StoreName = stock.StoreName,
                       ItemPackageId = stock.ItemPackageId,
                       ItemPackageCode = stock.ItemPackageCode,
                       ItemPackageName = stock.ItemPackageName,
                   
                       ExpireDate = stock.ExpireDate,
                       RemainingDays = Math.Max(EF.Functions.DateDiffDay(today, stock.ExpireDate) ?? 0, 0),
                       ExpiredSince = Math.Max(EF.Functions.DateDiffDay(stock.ExpireDate, today) ?? 0, 0),
                   
                       BatchNumber = stock.BatchNumber,
                       BarCode = stock.BarCode,
                   
                       PurchasingPrice = stock.PurchasingPrice,
                       ConsumerPrice = stock.ConsumerPrice,
                       InternalPrice = stock.InternalPrice,
                       MaxDiscountPercent = stock.MaxDiscountPercent,
                       SalesAccountId = stock.SalesAccountId,
                       SalesAccountName = stock.SalesAccountName,
                       PurchaseAccountId = stock.PurchaseAccountId,
                       PurchaseAccountName = stock.PurchaseAccountName,
                       MinBuyQuantity = stock.MinBuyQuantity,
                       MinSellQuantity = stock.MinSellQuantity,
                       MaxBuyQuantity = stock.MaxBuyQuantity,
                       MaxSellQuantity = stock.MaxSellQuantity,
                       CoverageQuantity = stock.CoverageQuantity,
                       IsActive = stock.IsActive,
                       InActiveReasons = stock.InActiveReasons,
                       NoReplenishment = stock.NoReplenishment,
                       IsUnderSelling = stock.IsUnderSelling,
                       IsNoStock = stock.IsNoStock,
                       IsUntradeable = stock.IsUntradeable,
                       IsDeficit = stock.IsDeficit,
                       IsPos = stock.IsPos,
                       IsOnline = stock.IsOnline,
                       IsPoints = stock.IsPoints,
                       IsPromoted = stock.IsPromoted,
                       IsExpired = stock.IsExpired,
                       IsBatched = stock.IsBatched,
                       ItemLocation = stock.ItemLocation,
                   
                       ItemCategoryId = stock.ItemCategoryId,
                       ItemCategoryName = stock.ItemCategoryName,
                       ItemSubCategoryId = stock.ItemSubCategoryId,
                       ItemSubCategoryName = stock.ItemSubCategoryName,
                       ItemSectionId = stock.ItemSectionId,
                       ItemSectionName = stock.ItemSectionName,
                       ItemSubSectionId = stock.ItemSubSectionId,
                       ItemSubSectionName = stock.ItemSubSectionName,
                       MainItemId = stock.MainItemId,
                       MainItemName = stock.MainItemName,

					   OtherTaxes = stock.OtherTaxes,
					   ItemAttributes = stock.ItemAttributes,
					   ItemMenuNotes = stock.ItemMenuNotes,
                   
                       LastSalesInvoiceDate = stock.LastSalesInvoiceDate,
                       LastSalesInvoiceFullCode = stock.LastSalesInvoiceFullCode,
                       LastSalesInvoiceClientName = stock.LastSalesInvoiceClientName,
                   
                       CreatedAt = stock.CreatedAt,
                       UserNameCreated = stock.UserNameCreated,
                       ModifiedAt = stock.ModifiedAt,
                       UserNameModified = stock.UserNameModified
                   };
		}
	}
}
