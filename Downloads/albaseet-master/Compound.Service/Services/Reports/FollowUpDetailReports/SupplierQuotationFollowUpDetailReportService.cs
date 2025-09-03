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
	public class SupplierQuotationFollowUpDetailReportService : ISupplierQuotationFollowUpDetailReportService
	{
		private readonly ISupplierQuotationHeaderService _supplierQuotationHeaderService;
		private readonly ISupplierQuotationDetailService _supplierQuotationDetailService;
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

		public SupplierQuotationFollowUpDetailReportService(ISupplierQuotationHeaderService supplierQuotationHeaderService, ISupplierQuotationDetailService supplierQuotationDetailService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IPurchaseOrderDetailService purchaseOrderDetailService, IStockInHeaderService stockInHeaderService, IStockInDetailService stockInDetailService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IStockInReturnHeaderService stockInReturnHeaderService, IStockInReturnDetailService stockInReturnDetailService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, IMenuService menuService, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService)
		{
			_supplierQuotationHeaderService = supplierQuotationHeaderService;
			_supplierQuotationDetailService = supplierQuotationDetailService;
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

		public async Task<IQueryable<SupplierQuotationFollowUpDetailReportDto>> GetSupplierQuotationFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var purchaseOrders = await (from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
										from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
										from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
										from purchaseOrderDetail in _purchaseOrderDetailService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId)
										where directPurchaseInvoiceHeader == null
										group new { purchaseOrderHeader, purchaseOrderDetail } by new { supplierQuotationHeader.SupplierQuotationHeaderId, purchaseOrderDetail.ItemId, purchaseOrderDetail.ItemPackageId } into g
										select new { g.Key.SupplierQuotationHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseOrderDetail.Quantity + x.purchaseOrderDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseOrderHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
										.ToDictionaryAsync(x => new { x.SupplierQuotationHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockInFromPurchaseOrders = await (from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
												   from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
												   from stockInFromPurchaseOrderHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId)
												   from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
												   from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseOrderHeader.StockInHeaderId)
												   where directPurchaseInvoiceHeader == null
												   group new { stockInFromPurchaseOrderHeader, stockInDetail } by new { supplierQuotationHeader.SupplierQuotationHeaderId, stockInDetail.ItemId, stockInDetail.ItemPackageId } into g
												   select new { g.Key.SupplierQuotationHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInDetail.Quantity + x.stockInDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInFromPurchaseOrderHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												   .ToDictionaryAsync(x => new { x.SupplierQuotationHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockInReturnFromStockIns = await (from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
												   from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
												   from stockInFromPurchaseOrderHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId)
												   from stockInReturnFromStockInHeader in _stockInReturnHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseOrderHeader.StockInHeaderId)
												   from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
												   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnFromStockInHeader.StockInReturnHeaderId)
												   where directPurchaseInvoiceHeader == null
												   group new { stockInReturnFromStockInHeader, stockInReturnDetail } by new { supplierQuotationHeader.SupplierQuotationHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId } into g
												   select new { g.Key.SupplierQuotationHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInReturnDetail.Quantity + x.stockInReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInReturnFromStockInHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												   .ToDictionaryAsync(x => new { x.SupplierQuotationHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var purchaseInvoiceInterims = await (from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
												 from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
												 from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && !x.IsDirectInvoice)
												 from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
												 group new { purchaseInvoiceInterimHeader, purchaseInvoiceDetail } by new { supplierQuotationHeader.SupplierQuotationHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId } into g
												 select new { g.Key.SupplierQuotationHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceDetail.Quantity + x.purchaseInvoiceDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceInterimHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												 .ToDictionaryAsync(x => new { x.SupplierQuotationHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockInReturnFromPurchaseInvoices = await (from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
														   from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
														   from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && !x.IsDirectInvoice)
														   from stockInReturnFromPurchaseInvoiceHeader in _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
														   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnFromPurchaseInvoiceHeader.StockInReturnHeaderId)
														   group new { stockInReturnFromPurchaseInvoiceHeader, stockInReturnDetail } by new { supplierQuotationHeader.SupplierQuotationHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId } into g
														   select new { g.Key.SupplierQuotationHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInReturnDetail.Quantity + x.stockInReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInReturnFromPurchaseInvoiceHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												           .ToDictionaryAsync(x => new { x.SupplierQuotationHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var purchaseInvoiceReturns = await (from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
												from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
												from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && !x.IsDirectInvoice)
												from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
												from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId)
												group new { purchaseInvoiceReturnHeader, purchaseInvoiceReturnDetail } by new { supplierQuotationHeader.SupplierQuotationHeaderId, purchaseInvoiceReturnDetail.ItemId, purchaseInvoiceReturnDetail.ItemPackageId } into g
												select new { g.Key.SupplierQuotationHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceReturnDetail.Quantity + x.purchaseInvoiceReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceReturnHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												.ToDictionaryAsync(x => new { x.SupplierQuotationHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var anonymousDefault = new { Quantity = 0.0M, DocumentFullCodes = string.Empty };

			var query = from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from store in _storeService.GetAll().Where(x => x.StoreId == supplierQuotationHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == MenuCodeData.SupplierQuotation)

						from supplierQuotationDetail in _supplierQuotationDetailService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)

						from item in _itemService.GetAll().Where(x => x.ItemId == supplierQuotationDetail.ItemId)
						from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == supplierQuotationDetail.ItemPackageId)
						from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == supplierQuotationDetail.CostCenterId).DefaultIfEmpty()

						orderby supplierQuotationHeader.DocumentDate, supplierQuotationHeader.SupplierQuotationHeaderId
						select new SupplierQuotationFollowUpDetailReportDto
						{
							SupplierQuotationHeaderId = supplierQuotationHeader.SupplierQuotationHeaderId,
							MenuCode = menu.MenuCode,
							MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
							DocumentFullCode = supplierQuotationHeader.Prefix + supplierQuotationHeader.DocumentCode + supplierQuotationHeader.Suffix,
							DocumentDate = supplierQuotationHeader.DocumentDate,
							StoreId = supplierQuotationHeader.StoreId,
							StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

							BarCode = supplierQuotationDetail.BarCode,
							ItemId = supplierQuotationDetail.ItemId,
							ItemCode = item.ItemCode,
							ItemName = supplierQuotationDetail.ItemNote != null ? supplierQuotationDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
							ItemPackageId = itemPackage.ItemPackageId,
							ItemPackageCode = itemPackage.ItemPackageCode,
							ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
							Packing = supplierQuotationDetail.Packing,
							Quantity = supplierQuotationDetail.Quantity,
							CostCenterId = costCenter.CostCenterId,
							CostCenterCode = costCenter.CostCenterCode,
							CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
							PackagePrice = supplierQuotationDetail.ReceivedPrice,
							NetValue = supplierQuotationDetail.NetValue,
							CostPrice = supplierQuotationDetail.CostPrice,
							CostPackage = supplierQuotationDetail.CostPackage,
							CostValue = supplierQuotationDetail.CostValue,

							PurchaseOrderQuantity = purchaseOrders.GetValueOrDefault(new { supplierQuotationHeader.SupplierQuotationHeaderId, supplierQuotationDetail.ItemId, supplierQuotationDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseOrderDocumentFullCodes = purchaseOrders.GetValueOrDefault(new { supplierQuotationHeader.SupplierQuotationHeaderId, supplierQuotationDetail.ItemId, supplierQuotationDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInFromPurchaseOrderQuantity = stockInFromPurchaseOrders.GetValueOrDefault(new { supplierQuotationHeader.SupplierQuotationHeaderId, supplierQuotationDetail.ItemId, supplierQuotationDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInFromPurchaseOrderDocumentFullCodes = stockInFromPurchaseOrders.GetValueOrDefault(new { supplierQuotationHeader.SupplierQuotationHeaderId, supplierQuotationDetail.ItemId, supplierQuotationDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInReturnFromStockInQuantity = stockInReturnFromStockIns.GetValueOrDefault(new { supplierQuotationHeader.SupplierQuotationHeaderId, supplierQuotationDetail.ItemId, supplierQuotationDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInReturnFromStockInDocumentFullCodes = stockInReturnFromStockIns.GetValueOrDefault(new { supplierQuotationHeader.SupplierQuotationHeaderId, supplierQuotationDetail.ItemId, supplierQuotationDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseInvoiceInterimQuantity = purchaseInvoiceInterims.GetValueOrDefault(new { supplierQuotationHeader.SupplierQuotationHeaderId, supplierQuotationDetail.ItemId, supplierQuotationDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseInvoiceInterimDocumentFullCodes = purchaseInvoiceInterims.GetValueOrDefault(new { supplierQuotationHeader.SupplierQuotationHeaderId, supplierQuotationDetail.ItemId, supplierQuotationDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInReturnFromPurchaseInvoiceQuantity = stockInReturnFromPurchaseInvoices.GetValueOrDefault(new { supplierQuotationHeader.SupplierQuotationHeaderId, supplierQuotationDetail.ItemId, supplierQuotationDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInReturnFromPurchaseInvoiceDocumentFullCodes = stockInReturnFromPurchaseInvoices.GetValueOrDefault(new { supplierQuotationHeader.SupplierQuotationHeaderId, supplierQuotationDetail.ItemId, supplierQuotationDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseInvoiceReturnQuantity = purchaseInvoiceReturns.GetValueOrDefault(new { supplierQuotationHeader.SupplierQuotationHeaderId, supplierQuotationDetail.ItemId, supplierQuotationDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseInvoiceReturnDocumentFullCodes = purchaseInvoiceReturns.GetValueOrDefault(new { supplierQuotationHeader.SupplierQuotationHeaderId, supplierQuotationDetail.ItemId, supplierQuotationDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,

							Reference = supplierQuotationHeader.Reference,
							RemarksAr = supplierQuotationHeader.RemarksAr,
							RemarksEn = supplierQuotationHeader.RemarksEn,

							CreatedAt = supplierQuotationHeader.CreatedAt,
							UserNameCreated = supplierQuotationHeader.UserNameCreated,
							ModifiedAt = supplierQuotationHeader.ModifiedAt,
							UserNameModified = supplierQuotationHeader.UserNameModified
						};

			return query;
		}
	}
}