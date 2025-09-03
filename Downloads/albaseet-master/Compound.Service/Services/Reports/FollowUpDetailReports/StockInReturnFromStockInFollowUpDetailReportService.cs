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
	public class StockInReturnFromStockInFollowUpDetailReportService : IStockInReturnFromStockInFollowUpDetailReportService
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

		public StockInReturnFromStockInFollowUpDetailReportService(IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IStockInHeaderService stockInHeaderService, IStockInDetailService stockInDetailService, IStockInReturnHeaderService stockInReturnHeaderService, IStockInReturnDetailService stockInReturnDetailService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, IMenuService menuService, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService, IInvoiceStockInService invoiceStockInService)
		{
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_purchaseInvoiceDetailService = purchaseInvoiceDetailService;
			_stockInHeaderService = stockInHeaderService;
			_stockInDetailService = stockInDetailService;
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
			_invoiceStockInService = invoiceStockInService;
		}

		public async Task<IQueryable<StockInReturnFromStockInFollowUpDetailReportDto>> GetStockInReturnFromStockInFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var purchaseInvoiceInterims = await (from stockInReturnFromStockInHeader in _stockInReturnHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.StockInHeaderId != null)
												 from invoiceStockIn in _invoiceStockInService.GetAll().Where(x => x.StockInHeaderId == stockInReturnFromStockInHeader.StockInHeaderId)
												 from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == invoiceStockIn.PurchaseInvoiceHeaderId && !x.IsDirectInvoice)
												 from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
												 group new { purchaseInvoiceInterimHeader, purchaseInvoiceDetail } by new { stockInReturnFromStockInHeader.StockInReturnHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId } into g
												 select new { g.Key.StockInReturnHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceDetail.Quantity + x.purchaseInvoiceDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceInterimHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												 .ToDictionaryAsync(x => new { x.StockInReturnHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockInReturnFromPurchaseInvoices = await (from stockInReturnFromStockInHeader in _stockInReturnHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.StockInHeaderId != null)
												           from invoiceStockIn in _invoiceStockInService.GetAll().Where(x => x.StockInHeaderId == stockInReturnFromStockInHeader.StockInHeaderId)
												           from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == invoiceStockIn.PurchaseInvoiceHeaderId && !x.IsDirectInvoice)
														   from stockInReturnFromPurchaseInvoiceHeader in _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
														   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnFromPurchaseInvoiceHeader.StockInReturnHeaderId)
														   group new { stockInReturnFromPurchaseInvoiceHeader, stockInReturnDetail } by new { stockInReturnFromStockInHeader.StockInReturnHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId } into g
														   select new { g.Key.StockInReturnHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInReturnDetail.Quantity + x.stockInReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInReturnFromPurchaseInvoiceHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												           .ToDictionaryAsync(x => new { x.StockInReturnHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var purchaseInvoiceReturns = await (from stockInReturnFromStockInHeader in _stockInReturnHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.StockInHeaderId != null)
												from invoiceStockIn in _invoiceStockInService.GetAll().Where(x => x.StockInHeaderId == stockInReturnFromStockInHeader.StockInHeaderId)
												from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == invoiceStockIn.PurchaseInvoiceHeaderId && !x.IsDirectInvoice)
												from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
												from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId)
												group new { purchaseInvoiceReturnHeader, purchaseInvoiceReturnDetail } by new { stockInReturnFromStockInHeader.StockInReturnHeaderId, purchaseInvoiceReturnDetail.ItemId, purchaseInvoiceReturnDetail.ItemPackageId } into g
												select new { g.Key.StockInReturnHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceReturnDetail.Quantity + x.purchaseInvoiceReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceReturnHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												.ToDictionaryAsync(x => new { x.StockInReturnHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var anonymousDefault = new { Quantity = 0.0M, DocumentFullCodes = string.Empty };

			var query = from stockInReturnHeader in _stockInReturnHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from stockInHeader in _stockInHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInReturnHeader.StockInHeaderId)
			            from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == stockInHeader.PurchaseInvoiceHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
						from store in _storeService.GetAll().Where(x => x.StoreId == stockInReturnHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == stockInReturnHeader.MenuCode)

						from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeader.StockInReturnHeaderId)

						from item in _itemService.GetAll().Where(x => x.ItemId == stockInReturnDetail.ItemId)
						from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == stockInReturnDetail.ItemPackageId)
						from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == stockInReturnDetail.CostCenterId).DefaultIfEmpty()

						where stockInHeader.PurchaseOrderHeaderId != null && directPurchaseInvoiceHeader == null
						orderby stockInReturnHeader.DocumentDate, stockInReturnHeader.StockInReturnHeaderId
						select new StockInReturnFromStockInFollowUpDetailReportDto
						{
							StockInReturnHeaderId = stockInReturnHeader.StockInReturnHeaderId,
							MenuCode = menu.MenuCode,
							MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
							DocumentFullCode = stockInReturnHeader.Prefix + stockInReturnHeader.DocumentCode + stockInReturnHeader.Suffix,
							DocumentDate = stockInReturnHeader.DocumentDate,
							StoreId = stockInReturnHeader.StoreId,
							StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

							BarCode = stockInReturnDetail.BarCode,
							ItemId = stockInReturnDetail.ItemId,
							ItemCode = item.ItemCode,
							ItemName = stockInReturnDetail.ItemNote != null ? stockInReturnDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
							ItemPackageId = itemPackage.ItemPackageId,
							ItemPackageCode = itemPackage.ItemPackageCode,
							ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
							Packing = stockInReturnDetail.Packing,
							Quantity = stockInReturnDetail.Quantity,
							CostCenterId = costCenter.CostCenterId,
							CostCenterCode = costCenter.CostCenterCode,
							CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
							PackagePrice = stockInReturnDetail.PurchasePrice,
							NetValue = stockInReturnDetail.NetValue,
							CostPrice = stockInReturnDetail.CostPrice,
							CostPackage = stockInReturnDetail.CostPackage,
							CostValue = stockInReturnDetail.CostValue,

							PurchaseInvoiceInterimQuantity = purchaseInvoiceInterims.GetValueOrDefault(new { stockInReturnHeader.StockInReturnHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseInvoiceInterimDocumentFullCodes = purchaseInvoiceInterims.GetValueOrDefault(new { stockInReturnHeader.StockInReturnHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInReturnFromPurchaseInvoiceQuantity = stockInReturnFromPurchaseInvoices.GetValueOrDefault(new { stockInReturnHeader.StockInReturnHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInReturnFromPurchaseInvoiceDocumentFullCodes = stockInReturnFromPurchaseInvoices.GetValueOrDefault(new { stockInReturnHeader.StockInReturnHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseInvoiceReturnQuantity = purchaseInvoiceReturns.GetValueOrDefault(new { stockInReturnHeader.StockInReturnHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseInvoiceReturnDocumentFullCodes = purchaseInvoiceReturns.GetValueOrDefault(new { stockInReturnHeader.StockInReturnHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,

							Reference = stockInReturnHeader.Reference,
							RemarksAr = stockInReturnHeader.RemarksAr,
							RemarksEn = stockInReturnHeader.RemarksEn,

							CreatedAt = stockInReturnHeader.CreatedAt,
							UserNameCreated = stockInReturnHeader.UserNameCreated,
							ModifiedAt = stockInReturnHeader.ModifiedAt,
							UserNameModified = stockInReturnHeader.UserNameModified
						};

			return query;
		}
	}
}