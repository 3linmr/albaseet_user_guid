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
    public class ProductRequestPriceFollowUpDetailReportService : IProductRequestPriceFollowUpDetailReportService
    {
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

        public ProductRequestPriceFollowUpDetailReportService(IProductRequestPriceDetailService productRequestPriceDetailService, IProductRequestPriceHeaderService productRequestPriceHeaderService, ISupplierQuotationHeaderService supplierQuotationHeaderService, ISupplierQuotationDetailService supplierQuotationDetailService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IPurchaseOrderDetailService purchaseOrderDetailService, IStockInHeaderService stockInHeaderService, IStockInDetailService stockInDetailService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IStockInReturnHeaderService stockInReturnHeaderService, IStockInReturnDetailService stockInReturnDetailService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, IMenuService menuService, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService)
        {
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

        public async Task<IQueryable<ProductRequestPriceFollowUpDetailReportDto>> GetProductRequestPriceFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var supplierQuotations = await (from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
                                            from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
                                            from supplierQuotationDetail in _supplierQuotationDetailService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
                                            group new { supplierQuotationHeader, supplierQuotationDetail } by new { productRequestPriceHeader.ProductRequestPriceHeaderId, supplierQuotationDetail.ItemId, supplierQuotationDetail.ItemPackageId } into g
                                            select new { g.Key.ProductRequestPriceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.supplierQuotationDetail.Quantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.supplierQuotationHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
                                            .ToDictionaryAsync(x => new { x.ProductRequestPriceHeaderId, x.ItemId, x.ItemPackageId}, x => new { x.Quantity, x.DocumentFullCodes });

            var purchaseOrders = await (from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
                                        from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
                                        from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
                                        from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
                                        from purchaseOrderDetail in _purchaseOrderDetailService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId)
                                        where directPurchaseInvoiceHeader == null
                                        group new { purchaseOrderHeader, purchaseOrderDetail } by new { productRequestPriceHeader.ProductRequestPriceHeaderId, purchaseOrderDetail.ItemId, purchaseOrderDetail.ItemPackageId } into g
                                        select new { g.Key.ProductRequestPriceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseOrderDetail.Quantity + x.purchaseOrderDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseOrderHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
                                        .ToDictionaryAsync(x => new { x.ProductRequestPriceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

            var stockInFromPurchaseOrders = await (from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
                                                   from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
                                                   from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
                                                   from stockInFromPurchaseOrderHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId)
                                                   from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
                                                   from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseOrderHeader.StockInHeaderId)
                                                   where directPurchaseInvoiceHeader == null
                                                   group new { stockInFromPurchaseOrderHeader, stockInDetail } by new { productRequestPriceHeader.ProductRequestPriceHeaderId, stockInDetail.ItemId, stockInDetail.ItemPackageId } into g
                                                   select new { g.Key.ProductRequestPriceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInDetail.Quantity + x.stockInDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInFromPurchaseOrderHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
                                                   .ToDictionaryAsync(x => new { x.ProductRequestPriceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

            var stockInReturnFromStockIns = await (from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
                                                   from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
                                                   from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
                                                   from stockInFromPurchaseOrderHeader in _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId)
                                                   from stockInReturnFromStockInHeader in _stockInReturnHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseOrderHeader.StockInHeaderId)
                                                   from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
                                                   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnFromStockInHeader.StockInReturnHeaderId)
                                                   where directPurchaseInvoiceHeader == null
                                                   group new { stockInReturnFromStockInHeader, stockInReturnDetail } by new { productRequestPriceHeader.ProductRequestPriceHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId } into g
                                                   select new { g.Key.ProductRequestPriceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInReturnDetail.Quantity + x.stockInReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInReturnFromStockInHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
                                                   .ToDictionaryAsync(x => new { x.ProductRequestPriceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

            var purchaseInvoiceInterims = await (from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
                                                 from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
                                                 from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
                                                 from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && !x.IsDirectInvoice)
                                                 from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
                                                 group new { purchaseInvoiceInterimHeader, purchaseInvoiceDetail } by new { productRequestPriceHeader.ProductRequestPriceHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId } into g
                                                 select new { g.Key.ProductRequestPriceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceDetail.Quantity + x.purchaseInvoiceDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceInterimHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
                                                 .ToDictionaryAsync(x => new { x.ProductRequestPriceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

            var stockInReturnFromPurchaseInvoices = await (from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
                                                           from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
                                                           from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
                                                           from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && !x.IsDirectInvoice)
                                                           from stockInReturnFromPurchaseInvoiceHeader in _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
                                                           from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnFromPurchaseInvoiceHeader.StockInReturnHeaderId)
                                                           group new { stockInReturnFromPurchaseInvoiceHeader, stockInReturnDetail } by new { productRequestPriceHeader.ProductRequestPriceHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId } into g
                                                           select new { g.Key.ProductRequestPriceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInReturnDetail.Quantity + x.stockInReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInReturnFromPurchaseInvoiceHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
                                                           .ToDictionaryAsync(x => new { x.ProductRequestPriceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var purchaseInvoiceReturns = await (from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
												from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
												from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId)
												from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId && !x.IsDirectInvoice)
												from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
												from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId)
												group new { purchaseInvoiceReturnHeader, purchaseInvoiceReturnDetail } by new { productRequestPriceHeader.ProductRequestPriceHeaderId, purchaseInvoiceReturnDetail.ItemId, purchaseInvoiceReturnDetail.ItemPackageId } into g
												select new { g.Key.ProductRequestPriceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceReturnDetail.Quantity + x.purchaseInvoiceReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceReturnHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												.ToDictionaryAsync(x => new { x.ProductRequestPriceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var anonymousDefault = new { Quantity = 0.0M, DocumentFullCodes = string.Empty };

			var query = from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from store in _storeService.GetAll().Where(x => x.StoreId == productRequestPriceHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == MenuCodeData.ProductRequestPrice)
						from productRequestPriceDetail in _productRequestPriceDetailService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId)
						from item in _itemService.GetAll().Where(x => x.ItemId == productRequestPriceDetail.ItemId)
						from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == productRequestPriceDetail.ItemPackageId)
						from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == productRequestPriceDetail.CostCenterId).DefaultIfEmpty()
						orderby productRequestPriceHeader.DocumentDate, productRequestPriceHeader.ProductRequestPriceHeaderId
						select new ProductRequestPriceFollowUpDetailReportDto
						{
							ProductRequestPriceHeaderId = productRequestPriceHeader.ProductRequestPriceHeaderId,
							MenuCode = menu.MenuCode,
							MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
							DocumentFullCode = productRequestPriceHeader.Prefix + productRequestPriceHeader.DocumentCode + productRequestPriceHeader.Suffix,
							DocumentDate = productRequestPriceHeader.DocumentDate,
							StoreId = productRequestPriceHeader.StoreId,
							StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

							BarCode = productRequestPriceDetail.BarCode,
							ItemId = productRequestPriceDetail.ItemId,
							ItemCode = item.ItemCode,
							ItemName = productRequestPriceDetail.ItemNote != null ? productRequestPriceDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
							ItemPackageId = itemPackage.ItemPackageId,
							ItemPackageCode = itemPackage.ItemPackageCode,
							ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
							Packing = productRequestPriceDetail.Packing,
							Quantity = productRequestPriceDetail.Quantity,
							CostCenterId = costCenter.CostCenterId,
							CostCenterCode = costCenter.CostCenterCode,
							CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
							PackagePrice = productRequestPriceDetail.RequestedPrice,
							NetValue = productRequestPriceDetail.NetValue,
							CostPrice = productRequestPriceDetail.CostPrice,
							CostPackage = productRequestPriceDetail.CostPackage,
							CostValue = productRequestPriceDetail.CostValue,

							SupplierQuotationQuantity = supplierQuotations.GetValueOrDefault(new { productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId }, anonymousDefault).Quantity,
							SupplierQuotationDocumentFullCodes = supplierQuotations.GetValueOrDefault(new { productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseOrderQuantity = purchaseOrders.GetValueOrDefault(new { productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseOrderDocumentFullCodes = purchaseOrders.GetValueOrDefault(new { productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInFromPurchaseOrderQuantity = stockInFromPurchaseOrders.GetValueOrDefault(new { productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInFromPurchaseOrderDocumentFullCodes = stockInFromPurchaseOrders.GetValueOrDefault(new { productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInReturnFromStockInQuantity = stockInReturnFromStockIns.GetValueOrDefault(new { productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInReturnFromStockInDocumentFullCodes = stockInReturnFromStockIns.GetValueOrDefault(new { productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseInvoiceInterimQuantity = purchaseInvoiceInterims.GetValueOrDefault(new { productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseInvoiceInterimDocumentFullCodes = purchaseInvoiceInterims.GetValueOrDefault(new { productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInReturnFromPurchaseInvoiceQuantity = stockInReturnFromPurchaseInvoices.GetValueOrDefault(new { productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInReturnFromPurchaseInvoiceDocumentFullCodes = stockInReturnFromPurchaseInvoices.GetValueOrDefault(new { productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseInvoiceReturnQuantity = purchaseInvoiceReturns.GetValueOrDefault(new { productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseInvoiceReturnDocumentFullCodes = purchaseInvoiceReturns.GetValueOrDefault(new { productRequestPriceHeader.ProductRequestPriceHeaderId, productRequestPriceDetail.ItemId, productRequestPriceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,

							Reference = productRequestPriceHeader.Reference,
							RemarksAr = productRequestPriceHeader.RemarksAr,
							RemarksEn = productRequestPriceHeader.RemarksEn,

							CreatedAt = productRequestPriceHeader.CreatedAt,
							UserNameCreated = productRequestPriceHeader.UserNameCreated,
							ModifiedAt = productRequestPriceHeader.ModifiedAt,
							UserNameModified = productRequestPriceHeader.UserNameModified
						};

			return query;
		}
	}
}