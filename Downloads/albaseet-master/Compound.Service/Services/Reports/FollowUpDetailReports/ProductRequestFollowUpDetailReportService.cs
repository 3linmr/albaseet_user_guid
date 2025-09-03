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
	public class ProductRequestFollowUpDetailReportService : IProductRequestFollowUpDetailReportService
	{
		private readonly IProductRequestHeaderService _productRequestHeaderService;
		private readonly IProductRequestDetailService _productRequestDetailService;
		private readonly IProductRequestPriceHeaderService _productRequestPriceHeaderService;
		private readonly IProductRequestPriceDetailService _productRequestPriceDetailService;
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

		public ProductRequestFollowUpDetailReportService(IProductRequestHeaderService productRequestHeaderService, IProductRequestDetailService productRequestDetailService, IProductRequestPriceDetailService productRequestPriceDetailService, IProductRequestPriceHeaderService productRequestPriceHeaderService, ISupplierQuotationHeaderService supplierQuotationHeaderService, ISupplierQuotationDetailService supplierQuotationDetailService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IPurchaseOrderDetailService purchaseOrderDetailService, IStockInHeaderService stockInHeaderService, IStockInDetailService stockInDetailService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IStockInReturnHeaderService stockInReturnHeaderService, IStockInReturnDetailService stockInReturnDetailService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, IMenuService menuService, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService)
		{
			_productRequestHeaderService = productRequestHeaderService;
			_productRequestDetailService = productRequestDetailService;
			_productRequestPriceHeaderService = productRequestPriceHeaderService;
			_productRequestPriceDetailService = productRequestPriceDetailService;
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

		public async Task<IQueryable<ProductRequestFollowUpDetailReportDto>> GetProductRequestFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var productRequestPrices = await (from productRequestHeader in _productRequestHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
											  from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => x.ProductRequestHeaderId == productRequestHeader.ProductRequestHeaderId)
											  from productRequestPriceDetail in _productRequestPriceDetailService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
											  group new { productRequestPriceHeader, productRequestPriceDetail } by new { productRequestHeader.ProductRequestHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId } into g
											  select new { g.Key.ProductRequestHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.productRequestPriceDetail.Quantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.productRequestPriceHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
											  .ToDictionaryAsync(x => new { x.ProductRequestHeaderId, x.ItemId, x.ItemPackageId}, x => new { x.Quantity, x.DocumentFullCodes });

			var supplierQuotations = await (from productRequestHeader in _productRequestHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
											from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => x.ProductRequestHeaderId == productRequestHeader.ProductRequestHeaderId)
											from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
											from supplierQuotationDetail in _supplierQuotationDetailService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
											group new { supplierQuotationHeader, supplierQuotationDetail } by new { productRequestHeader.ProductRequestHeaderId, supplierQuotationDetail.ItemId, supplierQuotationDetail.ItemPackageId } into g
											select new { g.Key.ProductRequestHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.supplierQuotationDetail.Quantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.supplierQuotationHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
											.ToDictionaryAsync(x => new { x.ProductRequestHeaderId, x.ItemId, x.ItemPackageId}, x => new { x.Quantity, x.DocumentFullCodes });

			var purchaseOrders = await (from productRequestHeader in _productRequestHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
										from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => x.ProductRequestHeaderId == productRequestHeader.ProductRequestHeaderId)
										from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
										from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
										from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
										from purchaseOrderDetail in _purchaseOrderDetailService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId)
										where directPurchaseInvoiceHeader == null
										group new { purchaseOrderHeader, purchaseOrderDetail } by new { productRequestHeader.ProductRequestHeaderId, purchaseOrderDetail.ItemId, purchaseOrderDetail.ItemPackageId } into g
										select new { g.Key.ProductRequestHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseOrderDetail.Quantity + x.purchaseOrderDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseOrderHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
										.ToDictionaryAsync(x => new { x.ProductRequestHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockInFromPurchaseOrders = await (from productRequestHeader in _productRequestHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
												   from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => x.ProductRequestHeaderId == productRequestHeader.ProductRequestHeaderId)
												   from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
												   from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
												   from stockInFromPurchaseOrderHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId)
												   from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
												   from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseOrderHeader.StockInHeaderId)
												   where directPurchaseInvoiceHeader == null
												   group new { stockInFromPurchaseOrderHeader, stockInDetail } by new { productRequestHeader.ProductRequestHeaderId, stockInDetail.ItemId, stockInDetail.ItemPackageId } into g
												   select new { g.Key.ProductRequestHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInDetail.Quantity + x.stockInDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInFromPurchaseOrderHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												   .ToDictionaryAsync(x => new { x.ProductRequestHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockInReturnFromStockIns = await (from productRequestHeader in _productRequestHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
												   from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => x.ProductRequestHeaderId == productRequestHeader.ProductRequestHeaderId)
												   from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
												   from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
												   from stockInFromPurchaseOrderHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId)
												   from stockInReturnFromStockInHeader in _stockInReturnHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseOrderHeader.StockInHeaderId)
												   from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
												   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnFromStockInHeader.StockInReturnHeaderId)
												   where directPurchaseInvoiceHeader == null
												   group new { stockInReturnFromStockInHeader, stockInReturnDetail } by new { productRequestHeader.ProductRequestHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId } into g
												   select new { g.Key.ProductRequestHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInReturnDetail.Quantity + x.stockInReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInReturnFromStockInHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												   .ToDictionaryAsync(x => new { x.ProductRequestHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var purchaseInvoiceInterims = await (from productRequestHeader in _productRequestHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
												 from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => x.ProductRequestHeaderId == productRequestHeader.ProductRequestHeaderId)
												 from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
												 from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
												 from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && !x.IsDirectInvoice)
												 from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
												 group new { purchaseInvoiceInterimHeader, purchaseInvoiceDetail } by new { productRequestHeader.ProductRequestHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId } into g
												 select new { g.Key.ProductRequestHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceDetail.Quantity + x.purchaseInvoiceDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceInterimHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												 .ToDictionaryAsync(x => new { x.ProductRequestHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockInReturnFromPurchaseInvoices = await (from productRequestHeader in _productRequestHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
														   from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => x.ProductRequestHeaderId == productRequestHeader.ProductRequestHeaderId)
														   from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
														   from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
														   from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && !x.IsDirectInvoice)
														   from stockInReturnFromPurchaseInvoiceHeader in _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
														   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnFromPurchaseInvoiceHeader.StockInReturnHeaderId)
														   group new { stockInReturnFromPurchaseInvoiceHeader, stockInReturnDetail } by new { productRequestHeader.ProductRequestHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId } into g
														   select new { g.Key.ProductRequestHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInReturnDetail.Quantity + x.stockInReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInReturnFromPurchaseInvoiceHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												           .ToDictionaryAsync(x => new { x.ProductRequestHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var purchaseInvoiceReturns = await (from productRequestHeader in _productRequestHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
												from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => x.ProductRequestHeaderId == productRequestHeader.ProductRequestHeaderId)
												from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
												from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
												from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && !x.IsDirectInvoice)
												from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
												from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId)
												group new { purchaseInvoiceReturnHeader, purchaseInvoiceReturnDetail } by new { productRequestHeader.ProductRequestHeaderId, purchaseInvoiceReturnDetail.ItemId, purchaseInvoiceReturnDetail.ItemPackageId } into g
												select new { g.Key.ProductRequestHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceReturnDetail.Quantity + x.purchaseInvoiceReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceReturnHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												.ToDictionaryAsync(x => new { x.ProductRequestHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var anonymousDefault = new { Quantity = 0.0M, DocumentFullCodes = string.Empty };

			var query = from productRequestHeader in _productRequestHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from store in _storeService.GetAll().Where(x => x.StoreId == productRequestHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == MenuCodeData.ProductRequest)

						from productRequestDetail in _productRequestDetailService.GetAll().Where(x => x.ProductRequestHeaderId == productRequestHeader.ProductRequestHeaderId)

						from item in _itemService.GetAll().Where(x => x.ItemId == productRequestDetail.ItemId)
						from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == productRequestDetail.ItemPackageId)
						from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == productRequestDetail.CostCenterId).DefaultIfEmpty()

						orderby productRequestHeader.DocumentDate, productRequestHeader.ProductRequestHeaderId
						select new ProductRequestFollowUpDetailReportDto
						{
							ProductRequestHeaderId = productRequestHeader.ProductRequestHeaderId,
							MenuCode = menu.MenuCode,
							MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
							DocumentFullCode = productRequestHeader.Prefix + productRequestHeader.DocumentCode + productRequestHeader.Suffix,
							DocumentDate = productRequestHeader.DocumentDate,
							StoreId = productRequestHeader.StoreId,
							StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

							BarCode = productRequestDetail.BarCode,
							ItemId = productRequestDetail.ItemId,
							ItemCode = item.ItemCode,
							ItemName = productRequestDetail.ItemNote != null ? productRequestDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
							ItemPackageId = itemPackage.ItemPackageId,
							ItemPackageCode = itemPackage.ItemPackageCode,
							ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
							Packing = productRequestDetail.Packing,
							Quantity = productRequestDetail.Quantity,
							CostCenterId = costCenter.CostCenterId,
							CostCenterCode = costCenter.CostCenterCode,
							CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
							PackagePrice = productRequestDetail.ConsumerPrice,
							NetValue = productRequestDetail.ConsumerValue,
							CostPrice = productRequestDetail.CostPrice,
							CostPackage = productRequestDetail.CostPackage,
							CostValue = productRequestDetail.CostValue,

							ProductRequestPriceQuantity = productRequestPrices.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).Quantity,
							ProductRequestPriceDocumentFullCodes = productRequestPrices.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							SupplierQuotationQuantity = supplierQuotations.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).Quantity,
							SupplierQuotationDocumentFullCodes = supplierQuotations.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseOrderQuantity = purchaseOrders.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseOrderDocumentFullCodes = purchaseOrders.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInFromPurchaseOrderQuantity = stockInFromPurchaseOrders.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInFromPurchaseOrderDocumentFullCodes = stockInFromPurchaseOrders.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInReturnFromStockInQuantity = stockInReturnFromStockIns.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInReturnFromStockInDocumentFullCodes = stockInReturnFromStockIns.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseInvoiceInterimQuantity = purchaseInvoiceInterims.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseInvoiceInterimDocumentFullCodes = purchaseInvoiceInterims.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInReturnFromPurchaseInvoiceQuantity = stockInReturnFromPurchaseInvoices.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInReturnFromPurchaseInvoiceDocumentFullCodes = stockInReturnFromPurchaseInvoices.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseInvoiceReturnQuantity = purchaseInvoiceReturns.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseInvoiceReturnDocumentFullCodes = purchaseInvoiceReturns.GetValueOrDefault(new { productRequestHeader.ProductRequestHeaderId, productRequestDetail.ItemId, productRequestDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,

							Reference = productRequestHeader.Reference,
							RemarksAr = productRequestHeader.RemarksAr,
							RemarksEn = productRequestHeader.RemarksEn,

							CreatedAt = productRequestHeader.CreatedAt,
							UserNameCreated = productRequestHeader.UserNameCreated,
							ModifiedAt = productRequestHeader.ModifiedAt,
							UserNameModified = productRequestHeader.UserNameModified
						};

			return query;
		}
	}
}