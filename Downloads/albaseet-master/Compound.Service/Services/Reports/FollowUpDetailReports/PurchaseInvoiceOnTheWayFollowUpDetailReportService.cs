using Purchases.CoreOne.Contracts;
using Compound.CoreOne.Contracts.Reports.FollowUpDetailReports;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Modules;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Menus;
using Purchases.CoreOne.Models.Domain;
using Compound.Service.Services.Reports.FollowUpReports;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.CostCenters;
using Microsoft.EntityFrameworkCore;
using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.Service.Services.Reports.FollowUpDetailReports
{
	public class PurchaseInvoiceOnTheWayFollowUpDetailReportService : IPurchaseInvoiceOnTheWayFollowUpDetailReportService
	{
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
		private readonly IStockInHeaderService _stockInHeaderService;
		private readonly IStockInDetailService _stockInDetailService;
		private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
		private readonly IStockInReturnDetailService _stockInReturnDetailService;
		private readonly IStoreService _storeService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<ProductRequestFollowUpReportService> _localizer;
		private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
		private readonly IPurchaseInvoiceReturnDetailService _purchaseInvoiceReturnDetailService;
		private readonly IMenuService _menuService;
		private readonly IItemService _itemService;
		private readonly ICostCenterService _costCenterService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IInvoiceStockInService _invoiceStockInService;

		public PurchaseInvoiceOnTheWayFollowUpDetailReportService(IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IStockInReturnHeaderService stockInReturnHeaderService, IStockInReturnDetailService stockInReturnDetailService, IStockInHeaderService stockInHeaderService, IStockInDetailService stockInDetailService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, IMenuService menuService, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService, IInvoiceStockInService invoiceStockInService)
		{
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_purchaseInvoiceDetailService = purchaseInvoiceDetailService;
			_stockInReturnHeaderService = stockInReturnHeaderService;
			_stockInReturnDetailService = stockInReturnDetailService;
			_stockInHeaderService = stockInHeaderService;
			_stockInDetailService = stockInDetailService;
			_storeService = storeService;
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
			_purchaseInvoiceReturnDetailService = purchaseInvoiceReturnDetailService;
			_menuService = menuService;
			_itemService = itemService;
			_costCenterService = costCenterService;
			_itemPackageService = itemPackageService;
			_invoiceStockInService = invoiceStockInService;
		}

		public async Task<IQueryable<PurchaseInvoiceOnTheWayFollowUpDetailReportDto>> GetPurchaseInvoiceOnTheWayFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();


			var stockInFromPurchaseInvoices = await (from purchaseInvoiceOnTheWayHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.IsOnTheWay)
												     from stockInFromPurchaseInvoiceHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceOnTheWayHeader.PurchaseInvoiceHeaderId)
												     from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseInvoiceHeader.StockInHeaderId)
												     group new { stockInFromPurchaseInvoiceHeader, stockInDetail } by new { purchaseInvoiceOnTheWayHeader.PurchaseInvoiceHeaderId, stockInDetail.ItemId, stockInDetail.ItemPackageId } into g
												     select new { g.Key.PurchaseInvoiceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInDetail.Quantity + x.stockInDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInFromPurchaseInvoiceHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												     .ToDictionaryAsync(x => new { x.PurchaseInvoiceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockInReturnFromInvoicedStockIns = await (from purchaseInvoiceOnTheWayHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.IsOnTheWay)
												           from stockInFromPurchaseInvoiceHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceOnTheWayHeader.PurchaseInvoiceHeaderId)
														   from stockInReturnFromInvoicedStockInHeader in _stockInReturnHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseInvoiceHeader.StockInHeaderId)
														   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnFromInvoicedStockInHeader.StockInReturnHeaderId)
												           group new { stockInReturnFromInvoicedStockInHeader, stockInReturnDetail } by new { purchaseInvoiceOnTheWayHeader.PurchaseInvoiceHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId } into g
												           select new { g.Key.PurchaseInvoiceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInReturnDetail.Quantity + x.stockInReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInReturnFromInvoicedStockInHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												           .ToDictionaryAsync(x => new { x.PurchaseInvoiceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var purchaseInvoiceReturnOnTheWay = await (from purchaseInvoiceOnTheWayHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.IsOnTheWay)
													   from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceOnTheWayHeader.PurchaseInvoiceHeaderId)
													   from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId)
													   where purchaseInvoiceReturnHeader.IsOnTheWay
													   group new { purchaseInvoiceReturnHeader, purchaseInvoiceReturnDetail } by new { purchaseInvoiceOnTheWayHeader.PurchaseInvoiceHeaderId, purchaseInvoiceReturnDetail.ItemId, purchaseInvoiceReturnDetail.ItemPackageId } into g
													   select new { g.Key.PurchaseInvoiceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceReturnDetail.Quantity + x.purchaseInvoiceReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceReturnHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
													   .ToDictionaryAsync(x => new { x.PurchaseInvoiceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockInReturnFromPurchaseInvoices = await (from purchaseInvoiceOnTheWayHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.IsOnTheWay)
														   from stockInReturnFromPurchaseInvoiceHeader in _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceOnTheWayHeader.PurchaseInvoiceHeaderId)
														   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnFromPurchaseInvoiceHeader.StockInReturnHeaderId)
														   group new { stockInReturnFromPurchaseInvoiceHeader, stockInReturnDetail } by new { purchaseInvoiceOnTheWayHeader.PurchaseInvoiceHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId } into g
														   select new { g.Key.PurchaseInvoiceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInReturnDetail.Quantity + x.stockInReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInReturnFromPurchaseInvoiceHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												           .ToDictionaryAsync(x => new { x.PurchaseInvoiceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var purchaseInvoiceReturnsAfterPurchase = await (from purchaseInvoiceOnTheWayHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.IsOnTheWay)
															 from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceOnTheWayHeader.PurchaseInvoiceHeaderId)
															 from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId)
															 where !purchaseInvoiceReturnHeader.IsOnTheWay
															 group new { purchaseInvoiceReturnHeader, purchaseInvoiceReturnDetail } by new { purchaseInvoiceOnTheWayHeader.PurchaseInvoiceHeaderId, purchaseInvoiceReturnDetail.ItemId, purchaseInvoiceReturnDetail.ItemPackageId } into g
															 select new { g.Key.PurchaseInvoiceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceReturnDetail.Quantity + x.purchaseInvoiceReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceReturnHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												             .ToDictionaryAsync(x => new { x.PurchaseInvoiceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var anonymousDefault = new { Quantity = 0.0M, DocumentFullCodes = string.Empty };

			var query = from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from store in _storeService.GetAll().Where(x => x.StoreId == purchaseInvoiceHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == purchaseInvoiceHeader.MenuCode)

						from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId)

						from item in _itemService.GetAll().Where(x => x.ItemId == purchaseInvoiceDetail.ItemId)
						from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == purchaseInvoiceDetail.ItemPackageId)
						from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == purchaseInvoiceDetail.CostCenterId).DefaultIfEmpty()

						where purchaseInvoiceHeader.IsOnTheWay
						orderby purchaseInvoiceHeader.DocumentDate, purchaseInvoiceHeader.PurchaseInvoiceHeaderId
						select new PurchaseInvoiceOnTheWayFollowUpDetailReportDto
						{
							PurchaseInvoiceHeaderId = purchaseInvoiceHeader.PurchaseInvoiceHeaderId,
							MenuCode = menu.MenuCode,
							MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
							DocumentFullCode = purchaseInvoiceHeader.Prefix + purchaseInvoiceHeader.DocumentCode + purchaseInvoiceHeader.Suffix,
							DocumentDate = purchaseInvoiceHeader.DocumentDate,
							StoreId = purchaseInvoiceHeader.StoreId,
							StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

							BarCode = purchaseInvoiceDetail.BarCode,
							ItemId = purchaseInvoiceDetail.ItemId,
							ItemCode = item.ItemCode,
							ItemName = purchaseInvoiceDetail.ItemNote != null ? purchaseInvoiceDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
							ItemPackageId = itemPackage.ItemPackageId,
							ItemPackageCode = itemPackage.ItemPackageCode,
							ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
							Packing = purchaseInvoiceDetail.Packing,
							Quantity = purchaseInvoiceDetail.Quantity,
							CostCenterId = costCenter.CostCenterId,
							CostCenterCode = costCenter.CostCenterCode,
							CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
							PackagePrice = purchaseInvoiceDetail.PurchasePrice,
							NetValue = purchaseInvoiceDetail.NetValue,
							CostPrice = purchaseInvoiceDetail.CostPrice,
							CostPackage = purchaseInvoiceDetail.CostPackage,
							CostValue = purchaseInvoiceDetail.CostValue,

							StockInFromPurchaseInvoiceQuantity = stockInFromPurchaseInvoices.GetValueOrDefault(new { purchaseInvoiceHeader.PurchaseInvoiceHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInFromPurchaseInvoiceDocumentFullCodes = stockInFromPurchaseInvoices.GetValueOrDefault(new { purchaseInvoiceHeader.PurchaseInvoiceHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInReturnFromInvoicedStockInQuantity = stockInReturnFromInvoicedStockIns.GetValueOrDefault(new { purchaseInvoiceHeader.PurchaseInvoiceHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInReturnFromInvoicedStockInDocumentFullCodes = stockInReturnFromInvoicedStockIns.GetValueOrDefault(new { purchaseInvoiceHeader.PurchaseInvoiceHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseInvoiceReturnOnTheWayQuantity = purchaseInvoiceReturnOnTheWay.GetValueOrDefault(new { purchaseInvoiceHeader.PurchaseInvoiceHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseInvoiceReturnOnTheWayDocumentFullCodes = purchaseInvoiceReturnOnTheWay.GetValueOrDefault(new { purchaseInvoiceHeader.PurchaseInvoiceHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInReturnFromPurchaseInvoiceQuantity = stockInReturnFromPurchaseInvoices.GetValueOrDefault(new { purchaseInvoiceHeader.PurchaseInvoiceHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInReturnFromPurchaseInvoiceDocumentFullCodes = stockInReturnFromPurchaseInvoices.GetValueOrDefault(new { purchaseInvoiceHeader.PurchaseInvoiceHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseInvoiceReturnQuantity = purchaseInvoiceReturnsAfterPurchase.GetValueOrDefault(new { purchaseInvoiceHeader.PurchaseInvoiceHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseInvoiceReturnDocumentFullCodes = purchaseInvoiceReturnsAfterPurchase.GetValueOrDefault(new { purchaseInvoiceHeader.PurchaseInvoiceHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,

							Reference = purchaseInvoiceHeader.Reference,
							RemarksAr = purchaseInvoiceHeader.RemarksAr,
							RemarksEn = purchaseInvoiceHeader.RemarksEn,

							CreatedAt = purchaseInvoiceHeader.CreatedAt,
							UserNameCreated = purchaseInvoiceHeader.UserNameCreated,
							ModifiedAt = purchaseInvoiceHeader.ModifiedAt,
							UserNameModified = purchaseInvoiceHeader.UserNameModified
						};

			return query;
		}
	}
}