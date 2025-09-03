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
	public class StockInFromPurchaseOrderFollowUpDetailReportService : IStockInFromPurchaseOrderFollowUpDetailReportService
	{
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
		private readonly IInvoiceStockInService _invoiceStockInService;

		public StockInFromPurchaseOrderFollowUpDetailReportService(IStockInHeaderService stockInHeaderService, IStockInDetailService stockInDetailService, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IStockInReturnHeaderService stockInReturnHeaderService, IStockInReturnDetailService stockInReturnDetailService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, IMenuService menuService, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService, IInvoiceStockInService invoiceStockInService)
		{
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
			_invoiceStockInService = invoiceStockInService;
		}

		public async Task<IQueryable<StockInFromPurchaseOrderFollowUpDetailReportDto>> GetStockInFromPurchaseOrderFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var stockInReturnFromStockIns = await (from stockInFromPurchaseOrderHeader in _stockInHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.PurchaseOrderHeaderId != null)
												   from stockInReturnFromStockInHeader in _stockInReturnHeaderService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseOrderHeader.StockInHeaderId)
												   from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseOrderHeaderId == stockInFromPurchaseOrderHeader.PurchaseOrderHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
												   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnFromStockInHeader.StockInReturnHeaderId)
												   where directPurchaseInvoiceHeader == null
												   group new { stockInReturnFromStockInHeader, stockInReturnDetail } by new { stockInFromPurchaseOrderHeader.StockInHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId } into g
												   select new { g.Key.StockInHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInReturnDetail.Quantity + x.stockInReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInReturnFromStockInHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												   .ToDictionaryAsync(x => new { x.StockInHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var purchaseInvoiceInterims = await (from stockInFromPurchaseOrderHeader in _stockInHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.PurchaseOrderHeaderId != null)
												 from invoiceStockIn in _invoiceStockInService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseOrderHeader.StockInHeaderId)
												 from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == invoiceStockIn.PurchaseInvoiceHeaderId && !x.IsDirectInvoice)
												 from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
												 group new { purchaseInvoiceInterimHeader, purchaseInvoiceDetail } by new { stockInFromPurchaseOrderHeader.StockInHeaderId, purchaseInvoiceDetail.ItemId, purchaseInvoiceDetail.ItemPackageId } into g
												 select new { g.Key.StockInHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceDetail.Quantity + x.purchaseInvoiceDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceInterimHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												 .ToDictionaryAsync(x => new { x.StockInHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockInReturnFromPurchaseInvoices = await (from stockInFromPurchaseOrderHeader in _stockInHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.PurchaseOrderHeaderId != null)
												           from invoiceStockIn in _invoiceStockInService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseOrderHeader.StockInHeaderId)
												           from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == invoiceStockIn.PurchaseInvoiceHeaderId && !x.IsDirectInvoice)
														   from stockInReturnFromPurchaseInvoiceHeader in _stockInReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
														   from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnFromPurchaseInvoiceHeader.StockInReturnHeaderId)
														   group new { stockInReturnFromPurchaseInvoiceHeader, stockInReturnDetail } by new { stockInFromPurchaseOrderHeader.StockInHeaderId, stockInReturnDetail.ItemId, stockInReturnDetail.ItemPackageId } into g
														   select new { g.Key.StockInHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockInReturnDetail.Quantity + x.stockInReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockInReturnFromPurchaseInvoiceHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												           .ToDictionaryAsync(x => new { x.StockInHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var purchaseInvoiceReturns = await (from stockInFromPurchaseOrderHeader in _stockInHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.PurchaseOrderHeaderId != null)
												from invoiceStockIn in _invoiceStockInService.GetAll().Where(x => x.StockInHeaderId == stockInFromPurchaseOrderHeader.StockInHeaderId)
												from purchaseInvoiceInterimHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == invoiceStockIn.PurchaseInvoiceHeaderId && !x.IsDirectInvoice)
												from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceInterimHeader.PurchaseInvoiceHeaderId)
												from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId)
												group new { purchaseInvoiceReturnHeader, purchaseInvoiceReturnDetail } by new { stockInFromPurchaseOrderHeader.StockInHeaderId, purchaseInvoiceReturnDetail.ItemId, purchaseInvoiceReturnDetail.ItemPackageId } into g
												select new { g.Key.StockInHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceReturnDetail.Quantity + x.purchaseInvoiceReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceReturnHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												.ToDictionaryAsync(x => new { x.StockInHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var anonymousDefault = new { Quantity = 0.0M, DocumentFullCodes = string.Empty };

			var query = from stockInHeader in _stockInHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
			            from directPurchaseInvoiceHeader in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == stockInHeader.PurchaseInvoiceHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
						from store in _storeService.GetAll().Where(x => x.StoreId == stockInHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == stockInHeader.MenuCode)

						from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId == stockInHeader.StockInHeaderId)

						from item in _itemService.GetAll().Where(x => x.ItemId == stockInDetail.ItemId)
						from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == stockInDetail.ItemPackageId)
						from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == stockInDetail.CostCenterId).DefaultIfEmpty()

						where stockInHeader.PurchaseOrderHeaderId != null && directPurchaseInvoiceHeader == null
						orderby stockInHeader.DocumentDate, stockInHeader.StockInHeaderId
						select new StockInFromPurchaseOrderFollowUpDetailReportDto
						{
							StockInHeaderId = stockInHeader.StockInHeaderId,
							MenuCode = menu.MenuCode,
							MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
							DocumentFullCode = stockInHeader.Prefix + stockInHeader.DocumentCode + stockInHeader.Suffix,
							DocumentDate = stockInHeader.DocumentDate,
							StoreId = stockInHeader.StoreId,
							StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

							BarCode = stockInDetail.BarCode,
							ItemId = stockInDetail.ItemId,
							ItemCode = item.ItemCode,
							ItemName = stockInDetail.ItemNote != null ? stockInDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
							ItemPackageId = itemPackage.ItemPackageId,
							ItemPackageCode = itemPackage.ItemPackageCode,
							ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
							Packing = stockInDetail.Packing,
							Quantity = stockInDetail.Quantity,
							CostCenterId = costCenter.CostCenterId,
							CostCenterCode = costCenter.CostCenterCode,
							CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
							PackagePrice = stockInDetail.PurchasePrice,
							NetValue = stockInDetail.NetValue,
							CostPrice = stockInDetail.CostPrice,
							CostPackage = stockInDetail.CostPackage,
							CostValue = stockInDetail.CostValue,

							StockInReturnFromStockInQuantity = stockInReturnFromStockIns.GetValueOrDefault(new { stockInHeader.StockInHeaderId, stockInDetail.ItemId, stockInDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInReturnFromStockInDocumentFullCodes = stockInReturnFromStockIns.GetValueOrDefault(new { stockInHeader.StockInHeaderId, stockInDetail.ItemId, stockInDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseInvoiceInterimQuantity = purchaseInvoiceInterims.GetValueOrDefault(new { stockInHeader.StockInHeaderId, stockInDetail.ItemId, stockInDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseInvoiceInterimDocumentFullCodes = purchaseInvoiceInterims.GetValueOrDefault(new { stockInHeader.StockInHeaderId, stockInDetail.ItemId, stockInDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockInReturnFromPurchaseInvoiceQuantity = stockInReturnFromPurchaseInvoices.GetValueOrDefault(new { stockInHeader.StockInHeaderId, stockInDetail.ItemId, stockInDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockInReturnFromPurchaseInvoiceDocumentFullCodes = stockInReturnFromPurchaseInvoices.GetValueOrDefault(new { stockInHeader.StockInHeaderId, stockInDetail.ItemId, stockInDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							PurchaseInvoiceReturnQuantity = purchaseInvoiceReturns.GetValueOrDefault(new { stockInHeader.StockInHeaderId, stockInDetail.ItemId, stockInDetail.ItemPackageId }, anonymousDefault).Quantity,
							PurchaseInvoiceReturnDocumentFullCodes = purchaseInvoiceReturns.GetValueOrDefault(new { stockInHeader.StockInHeaderId, stockInDetail.ItemId, stockInDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,

							Reference = stockInHeader.Reference,
							RemarksAr = stockInHeader.RemarksAr,
							RemarksEn = stockInHeader.RemarksEn,

							CreatedAt = stockInHeader.CreatedAt,
							UserNameCreated = stockInHeader.UserNameCreated,
							ModifiedAt = stockInHeader.ModifiedAt,
							UserNameModified = stockInHeader.UserNameModified
						};

			return query;
		}
	}
}