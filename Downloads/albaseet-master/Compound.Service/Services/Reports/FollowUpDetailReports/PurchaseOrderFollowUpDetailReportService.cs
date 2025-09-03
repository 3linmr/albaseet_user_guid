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
	public class PurchaseOrderFollowUpDetailReportService : IPurchaseOrderFollowUpDetailReportService
	{
		private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
		private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;
		private readonly IStockInHeaderService _stockInHeaderService;
		private readonly IStockInDetailService _stockInDetailService;
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;
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

		public PurchaseOrderFollowUpDetailReportService(IPurchaseOrderHeaderService purchaseOrderHeaderService, IPurchaseOrderDetailService purchaseOrderDetailService, IStockInHeaderService stockInHeaderService, IStockInDetailService stockInDetailService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IStockInReturnHeaderService stockInReturnHeaderService, IStockInReturnDetailService stockInReturnDetailService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, IMenuService menuService, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService)
		{
			_purchaseOrderHeaderService = purchaseOrderHeaderService;
			_purchaseOrderDetailService = purchaseOrderDetailService;
			_stockInHeaderService = stockInHeaderService;
			_stockInDetailService = stockInDetailService;
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_purchaseInvoiceDetailService = purchaseInvoiceDetailService;
			_stockInReturnHeaderService = stockInReturnHeaderService;
			_stockInReturnDetailService = stockInReturnDetailService;
			_storeService = storeService;
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
			_purchaseInvoiceReturnDetailService = purchaseInvoiceReturnDetailService;
			_menuService = menuService;
			_itemService = itemService;
			_costCenterService = costCenterService;
			_itemPackageService = itemPackageService;
		}

		public async Task<IQueryable<PurchaseOrderFollowUpDetailReportDto>> GetPurchaseOrderFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var stockInFromPurchaseOrders = await (from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
												   from stockInFromPurchaseOrderHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId)
												   from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
												   from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseOrderHeader.StockInHeaderId)
												   where directPurchaseInvoiceHeader == null
												   group new { stockInFromPurchaseOrderHeader, stockInDetail } by new { purchaseOrderHeader.PurchaseOrderHeaderId, stockInDetail.ItemId, stockInDetail.ItemPackageId } into g
												   select new { g.Key.PurchaseOrderHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInDetail.Quantity + x.stockInDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInFromPurchaseOrderHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												   .ToDictionaryAsync(x => new { x.PurchaseOrderHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockInReturnFromStockIns = await (from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
												   from stockInFromPurchaseOrderHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId)
												   from stockInReturnFromStockInHeader in _stockInReturnHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseOrderHeader.StockInHeaderId)
												   from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
												   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnFromStockInHeader.StockInReturnHeaderId)
												   where directPurchaseInvoiceHeader == null
												   group new { stockInReturnFromStockInHeader, stockInReturnDetail } by new { purchaseOrderHeader.PurchaseOrderHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId } into g
												   select new { g.Key.PurchaseOrderHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInReturnDetail.Quantity + x.stockInReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInReturnFromStockInHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												   .ToDictionaryAsync(x => new { x.PurchaseOrderHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var purchaseInvoiceInterims = await (from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
												 from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && !x.IsDirectInvoice)
												 from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
												 group new { purchaseInvoiceInterimHeader, purchaseInvoiceDetail } by new { purchaseOrderHeader.PurchaseOrderHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId } into g
												 select new { g.Key.PurchaseOrderHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceDetail.Quantity + x.purchaseInvoiceDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceInterimHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												 .ToDictionaryAsync(x => new { x.PurchaseOrderHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockInReturnFromPurchaseInvoices = await (from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
														   from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && !x.IsDirectInvoice)
														   from stockInReturnFromPurchaseInvoiceHeader in _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
														   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnFromPurchaseInvoiceHeader.StockInReturnHeaderId)
														   group new { stockInReturnFromPurchaseInvoiceHeader, stockInReturnDetail } by new { purchaseOrderHeader.PurchaseOrderHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId } into g
														   select new { g.Key.PurchaseOrderHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInReturnDetail.Quantity + x.stockInReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInReturnFromPurchaseInvoiceHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												           .ToDictionaryAsync(x => new { x.PurchaseOrderHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var purchaseInvoiceReturns = await (from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
												from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && !x.IsDirectInvoice)
												from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
												from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId)
												group new { purchaseInvoiceReturnHeader, purchaseInvoiceReturnDetail } by new { purchaseOrderHeader.PurchaseOrderHeaderId, purchaseInvoiceReturnDetail.ItemId, purchaseInvoiceReturnDetail.ItemPackageId } into g
												select new { g.Key.PurchaseOrderHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceReturnDetail.Quantity + x.purchaseInvoiceReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceReturnHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												.ToDictionaryAsync(x => new { x.PurchaseOrderHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var anonymousDefault = new { Quantity = 0.0M, DocumentFullCodes = string.Empty };

			var query = from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
			            from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && x.IsDirectInvoice).DefaultIfEmpty()
						from store in _storeService.GetAll().Where(x => x.StoreId == purchaseOrderHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == MenuCodeData.PurchaseOrder)

						from purchaseOrderDetail in _purchaseOrderDetailService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId)

						from item in _itemService.GetAll().Where(x => x.ItemId == purchaseOrderDetail.ItemId)
						from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == purchaseOrderDetail.ItemPackageId)
						from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == purchaseOrderDetail.CostCenterId).DefaultIfEmpty()

						where directPurchaseInvoiceHeader == null
						orderby purchaseOrderHeader.DocumentDate, purchaseOrderHeader.PurchaseOrderHeaderId
						select new PurchaseOrderFollowUpDetailReportDto
						{
							PurchaseOrderHeaderId = purchaseOrderHeader.PurchaseOrderHeaderId,
							MenuCode = menu.MenuCode,
							MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
							DocumentFullCode = purchaseOrderHeader.Prefix + purchaseOrderHeader.DocumentCode + purchaseOrderHeader.Suffix,
							DocumentDate = purchaseOrderHeader.DocumentDate,
							StoreId = purchaseOrderHeader.StoreId,
							StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

							BarCode = purchaseOrderDetail.BarCode,
							ItemId = purchaseOrderDetail.ItemId,
							ItemCode = item.ItemCode,
							ItemName = purchaseOrderDetail.ItemNote != null ? purchaseOrderDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
							ItemPackageId = itemPackage.ItemPackageId,
							ItemPackageCode = itemPackage.ItemPackageCode,
							ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
							Packing = purchaseOrderDetail.Packing,
							Quantity = purchaseOrderDetail.Quantity,
							CostCenterId = costCenter.CostCenterId,
							CostCenterCode = costCenter.CostCenterCode,
							CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
							PackagePrice = purchaseOrderDetail.PurchasePrice,
							NetValue = purchaseOrderDetail.NetValue,
							CostPrice = purchaseOrderDetail.CostPrice,
							CostPackage = purchaseOrderDetail.CostPackage,
							CostValue = purchaseOrderDetail.CostValue,

							StockInFromPurchaseOrderQuantity = stockInFromPurchaseOrders.GetValueOrDefault(new { purchaseOrderHeader.PurchaseOrderHeaderId, purchaseOrderDetail.ItemId, purchaseOrderDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInFromPurchaseOrderDocumentFullCodes = stockInFromPurchaseOrders.GetValueOrDefault(new { purchaseOrderHeader.PurchaseOrderHeaderId, purchaseOrderDetail.ItemId, purchaseOrderDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInReturnFromStockInQuantity = stockInReturnFromStockIns.GetValueOrDefault(new { purchaseOrderHeader.PurchaseOrderHeaderId, purchaseOrderDetail.ItemId, purchaseOrderDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInReturnFromStockInDocumentFullCodes = stockInReturnFromStockIns.GetValueOrDefault(new { purchaseOrderHeader.PurchaseOrderHeaderId, purchaseOrderDetail.ItemId, purchaseOrderDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseInvoiceInterimQuantity = purchaseInvoiceInterims.GetValueOrDefault(new { purchaseOrderHeader.PurchaseOrderHeaderId, purchaseOrderDetail.ItemId, purchaseOrderDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseInvoiceInterimDocumentFullCodes = purchaseInvoiceInterims.GetValueOrDefault(new { purchaseOrderHeader.PurchaseOrderHeaderId, purchaseOrderDetail.ItemId, purchaseOrderDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInReturnFromPurchaseInvoiceQuantity = stockInReturnFromPurchaseInvoices.GetValueOrDefault(new { purchaseOrderHeader.PurchaseOrderHeaderId, purchaseOrderDetail.ItemId, purchaseOrderDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInReturnFromPurchaseInvoiceDocumentFullCodes = stockInReturnFromPurchaseInvoices.GetValueOrDefault(new { purchaseOrderHeader.PurchaseOrderHeaderId, purchaseOrderDetail.ItemId, purchaseOrderDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseInvoiceReturnQuantity = purchaseInvoiceReturns.GetValueOrDefault(new { purchaseOrderHeader.PurchaseOrderHeaderId, purchaseOrderDetail.ItemId, purchaseOrderDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseInvoiceReturnDocumentFullCodes = purchaseInvoiceReturns.GetValueOrDefault(new { purchaseOrderHeader.PurchaseOrderHeaderId, purchaseOrderDetail.ItemId, purchaseOrderDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,

							Reference = purchaseOrderHeader.Reference,
							RemarksAr = purchaseOrderHeader.RemarksAr,
							RemarksEn = purchaseOrderHeader.RemarksEn,

							CreatedAt = purchaseOrderHeader.CreatedAt,
							UserNameCreated = purchaseOrderHeader.UserNameCreated,
							ModifiedAt = purchaseOrderHeader.ModifiedAt,
							UserNameModified = purchaseOrderHeader.UserNameModified
						};

			return query;
		}
	}
}