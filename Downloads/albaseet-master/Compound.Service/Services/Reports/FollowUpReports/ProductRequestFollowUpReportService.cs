using Purchases.CoreOne.Contracts;
using Compound.CoreOne.Contracts.Reports.FollowUpReports;
using Microsoft.Extensions.Localization;
using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;
using Shared.CoreOne.Contracts.Modules;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Menus;
using Purchases.CoreOne.Models.Domain;

namespace Compound.Service.Services.Reports.FollowUpReports
{
	public class ProductRequestFollowUpReportService : IProductRequestFollowUpReportService
	{
		private readonly IProductRequestHeaderService _productRequestHeaderService;
		private readonly IProductRequestPriceHeaderService _productRequestPriceHeaderService;
		private readonly ISupplierQuotationHeaderService _supplierQuotationHeaderService;
		private readonly IPurchaseOrderHeaderService _purchaseOrderHeaderService;
		private readonly IStockInHeaderService _stockInHeaderService;
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IStockInReturnHeaderService _stockInReturnHeaderService;
		private readonly IStoreService _storeService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<ProductRequestFollowUpReportService> _localizer;
		private readonly ISupplierCreditMemoService _supplierCreditMemoService;
		private readonly ISupplierDebitMemoService _supplierDebitMemoService;
		private readonly IPurchaseInvoiceReturnHeaderService _purchaseInvoiceReturnHeaderService;
		private readonly IMenuService _menuService;

		public ProductRequestFollowUpReportService(IProductRequestHeaderService productRequestHeaderService, IProductRequestPriceHeaderService productRequestPriceHeaderService, ISupplierQuotationHeaderService supplierQuotationHeaderService, IPurchaseOrderHeaderService purchaseOrderHeaderService, IStockInHeaderService stockInHeaderService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IStockInReturnHeaderService stockInReturnHeaderService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, ISupplierCreditMemoService supplierCreditMemoService, ISupplierDebitMemoService supplierDebitMemoService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IMenuService menuService)
		{
			_productRequestHeaderService = productRequestHeaderService;
			_productRequestPriceHeaderService = productRequestPriceHeaderService;
			_supplierQuotationHeaderService = supplierQuotationHeaderService;
			_purchaseOrderHeaderService = purchaseOrderHeaderService;
			_stockInHeaderService = stockInHeaderService;
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_stockInReturnHeaderService = stockInReturnHeaderService;
			_storeService = storeService;
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_supplierCreditMemoService = supplierCreditMemoService;
			_supplierDebitMemoService = supplierDebitMemoService;
			_purchaseInvoiceReturnHeaderService = purchaseInvoiceReturnHeaderService;
			_menuService = menuService;
		}

		public IQueryable<ProductRequestFollowUpReportDto> GetProductRequestFollowUpReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var query = from productRequestHeader in _productRequestHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from store in _storeService.GetAll().Where(x => x.StoreId == productRequestHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == MenuCodeData.ProductRequest)
						from productRequestPriceHeader in _productRequestPriceHeaderService.GetAll().Where(x => x.ProductRequestHeaderId == productRequestHeader.ProductRequestHeaderId).DefaultIfEmpty()
						from supplierQuotationHeader in _supplierQuotationHeaderService.GetAll().Where(x => x.ProductRequestPriceHeaderId == productRequestPriceHeader.ProductRequestPriceHeaderId).DefaultIfEmpty()
						from purchaseOrderHeader in _purchaseOrderHeaderService.GetAll().Where(x => x.SupplierQuotationHeaderId == supplierQuotationHeader.SupplierQuotationHeaderId).DefaultIfEmpty()
						from stockInFromPurchaseOrder in _stockInHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId).DefaultIfEmpty()
						from stockInReturnFromStockIn in _stockInReturnHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseOrder.StockInHeaderId).DefaultIfEmpty()
						from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == purchaseOrderHeader.PurchaseOrderHeaderId).DefaultIfEmpty()
						from stockInReturnFromPurchaseInvoice in _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
						from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
						from supplierDebitMemo in _supplierDebitMemoService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
						from supplierCreditMemo in _supplierCreditMemoService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
						group new { productRequestHeader, productRequestPriceHeader, supplierQuotationHeader, purchaseOrderHeader, stockInFromPurchaseOrder, stockInReturnFromStockIn, purchaseInvoiceHeader, stockInReturnFromPurchaseInvoice, purchaseInvoiceReturnHeader, supplierDebitMemo, supplierCreditMemo, store, menu }
						by new { productRequestHeader.ProductRequestHeaderId, menu.MenuCode, menu.MenuNameAr, menu.MenuNameEn, productRequestHeader.Prefix, productRequestHeader.DocumentCode, productRequestHeader.Suffix, productRequestHeader.DocumentDate, productRequestHeader.ConsumerValue, productRequestHeader.Reference, productRequestHeader.RemarksAr, productRequestHeader.RemarksEn, store.StoreId, store.StoreNameAr, store.StoreNameEn, productRequestHeader.CreatedAt, productRequestHeader.UserNameCreated, productRequestHeader.ModifiedAt, productRequestHeader.UserNameModified }
						into g
						orderby g.Key.DocumentDate, g.Key.ProductRequestHeaderId
						select new ProductRequestFollowUpReportDto
						{
							ProductRequestHeaderId = g.Key.ProductRequestHeaderId,
							MenuCode = g.Key.MenuCode,
							MenuName = language == LanguageCode.Arabic ? g.Key.MenuNameAr : g.Key.MenuNameEn,
							DocumentFullCode = g.Key.Prefix + g.Key.DocumentCode + g.Key.Suffix,
							DocumentDate = g.Key.DocumentDate,
							Price = g.Key.ConsumerValue,
							StoreId = g.Key.StoreId,
							StoreName = language == LanguageCode.Arabic ? g.Key.StoreNameAr : g.Key.StoreNameEn,
							ProductRequestPrice = g.Count(x => x.productRequestPriceHeader != null) > 0 ? _localizer["Done"].ToString() : null,
							SupplierQuotation = g.Count(x => x.supplierQuotationHeader != null) > 0 ? _localizer["Done"].ToString() : null,
							PurchaseOrder = g.Count(x => x.purchaseOrderHeader != null && (x.purchaseInvoiceHeader == null || !x.purchaseInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							StockInFromPurchaseOrder = g.Count(x => x.stockInFromPurchaseOrder != null && (x.purchaseInvoiceHeader == null || !x.purchaseInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							StockInReturnFromStockIn = g.Count(x => x.stockInReturnFromStockIn != null && (x.purchaseInvoiceHeader == null || !x.purchaseInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							PurchaseInvoiceInterim = g.Count(x => x.purchaseInvoiceHeader != null && (!x.purchaseInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							PaymentVoucher = g.Count(x => x.purchaseInvoiceHeader != null && (!x.purchaseInvoiceHeader.IsDirectInvoice) && x.purchaseInvoiceHeader.IsSettlementCompleted) > 0 ? _localizer["Done"].ToString() : null,
							StockInReturnFromPurchaseInvoice = g.Count(x => x.stockInReturnFromPurchaseInvoice != null && (!x.purchaseInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							PurchaseInvoiceReturn = g.Count(x => x.purchaseInvoiceReturnHeader != null && (!x.purchaseInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							SupplierDebitMemo = g.Count(x => x.supplierDebitMemo != null && (!x.purchaseInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							SupplierCreditMemo = g.Count(x => x.supplierCreditMemo != null && (!x.purchaseInvoiceHeader.IsDirectInvoice)) > 0 ? _localizer["Done"].ToString() : null,
							Reference = g.Key.Reference,
							RemarksAr = g.Key.RemarksAr,
							RemarksEn = g.Key.RemarksEn,

							CreatedAt = g.Key.CreatedAt,
							UserNameCreated = g.Key.UserNameCreated,
							ModifiedAt = g.Key.ModifiedAt,
							UserNameModified = g.Key.UserNameModified
						};

			return query;
		}
	}
}