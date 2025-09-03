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
using Shared.CoreOne.Contracts.Basics;

namespace Compound.Service.Services.Reports.FollowUpReports
{
    public class StockInReturnFromStockInFollowUpReportService : IStockInReturnFromStockInFollowUpReportService
    {
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
        private readonly IInvoiceStockInService _invoiceStockInService;

        public StockInReturnFromStockInFollowUpReportService(IStockInHeaderService stockInHeaderService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IStockInReturnHeaderService stockInReturnHeaderService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, ISupplierCreditMemoService supplierCreditMemoService, ISupplierDebitMemoService supplierDebitMemoService, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IMenuService menuService, IInvoiceStockInService invoiceStockInService)
        {
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
            _invoiceStockInService = invoiceStockInService;
        }

        public IQueryable<StockInReturnFromStockInFollowUpReportDto> GetStockInReturnFromStockInFollowUpReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var query = from stockInReturnFromStockIn in _stockInReturnHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
                        from store in _storeService.GetAll().Where(x => x.StoreId == stockInReturnFromStockIn.StoreId)
                        from menu in _menuService.GetAll().Where(x => x.MenuCode == stockInReturnFromStockIn.MenuCode)
                        from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInReturnFromStockIn.StockInHeaderId).DefaultIfEmpty()
                        from invoiceStock in _invoiceStockInService.GetAll().Where(x => x.StockInHeaderId == stockInHeader.StockInHeaderId).DefaultIfEmpty()
                        from purchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == invoiceStock.PurchaseInvoiceHeaderId).DefaultIfEmpty()
                        from stockInReturnFromPurchaseInvoice in _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
                        from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
                        from supplierDebitMemo in _supplierDebitMemoService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
                        from supplierCreditMemo in _supplierCreditMemoService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
                        where stockInHeader.PurchaseOrderHeaderId != null && (purchaseInvoiceHeader == null || !purchaseInvoiceHeader.IsDirectInvoice)
                        group new { stockInReturnFromStockIn, purchaseInvoiceHeader, stockInReturnFromPurchaseInvoice, purchaseInvoiceReturnHeader, supplierDebitMemo, supplierCreditMemo, store, menu }
                        by new { stockInReturnFromStockIn.StockInReturnHeaderId, stockInReturnFromStockIn.MenuCode, menu.MenuNameAr, menu.MenuNameEn, stockInReturnFromStockIn.Prefix, stockInReturnFromStockIn.DocumentCode, stockInReturnFromStockIn.Suffix, stockInReturnFromStockIn.DocumentDate, stockInReturnFromStockIn.NetValue, stockInReturnFromStockIn.Reference, stockInReturnFromStockIn.RemarksAr, stockInReturnFromStockIn.RemarksEn, store.StoreId, store.StoreNameAr, store.StoreNameEn, stockInReturnFromStockIn.CreatedAt, stockInReturnFromStockIn.UserNameCreated, stockInReturnFromStockIn.ModifiedAt, stockInReturnFromStockIn.UserNameModified }
                        into g
                        orderby g.Key.DocumentDate, g.Key.StockInReturnHeaderId
						select new StockInReturnFromStockInFollowUpReportDto
						{
							StockInReturnHeaderId = g.Key.StockInReturnHeaderId,
							MenuCode = g.Key.MenuCode,
							MenuName = language == LanguageCode.Arabic ? g.Key.MenuNameAr : g.Key.MenuNameEn,
							DocumentFullCode = g.Key.Prefix + g.Key.DocumentCode + g.Key.Suffix,
							DocumentDate = g.Key.DocumentDate,
							Price = g.Key.NetValue,
							StoreId = g.Key.StoreId,
							StoreName = language == LanguageCode.Arabic ? g.Key.StoreNameAr : g.Key.StoreNameEn,
							PurchaseInvoiceInterim = g.Count(x => x.purchaseInvoiceHeader != null) > 0 ? _localizer["Done"].ToString() : null,
							PaymentVoucher = g.Count(x => x.purchaseInvoiceHeader != null && x.purchaseInvoiceHeader.IsSettlementCompleted) > 0 ? _localizer["Done"].ToString() : null,
							StockInReturnFromPurchaseInvoice = g.Count(x => x.stockInReturnFromPurchaseInvoice != null) > 0 ? _localizer["Done"].ToString() : null,
							PurchaseInvoiceReturn = g.Count(x => x.purchaseInvoiceReturnHeader != null) > 0 ? _localizer["Done"].ToString() : null,
							SupplierDebitMemo = g.Count(x => x.supplierDebitMemo != null) > 0 ? _localizer["Done"].ToString() : null,
							SupplierCreditMemo = g.Count(x => x.supplierCreditMemo != null) > 0 ? _localizer["Done"].ToString() : null,
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
