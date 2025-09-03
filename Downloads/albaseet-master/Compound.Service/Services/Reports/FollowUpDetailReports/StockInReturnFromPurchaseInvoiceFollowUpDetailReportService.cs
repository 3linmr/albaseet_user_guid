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
	public class StockInReturnFromPurchaseInvoiceFollowUpDetailReportService : IStockInReturnFromPurchaseInvoiceFollowUpDetailReportService
	{
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
		private readonly IInvoiceStockInReturnService _invoiceStockInReturnService;

		public StockInReturnFromPurchaseInvoiceFollowUpDetailReportService(IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IPurchaseInvoiceDetailService purchaseInvoiceDetailService, IStockInReturnHeaderService stockInReturnHeaderService, IStockInReturnDetailService stockInReturnDetailService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, IPurchaseInvoiceReturnHeaderService purchaseInvoiceReturnHeaderService, IPurchaseInvoiceReturnDetailService purchaseInvoiceReturnDetailService, IMenuService menuService, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService, IInvoiceStockInReturnService invoiceStockInReturnService)
		{
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
			_invoiceStockInReturnService = invoiceStockInReturnService;
		}

		public async Task<IQueryable<StockInReturnFromPurchaseInvoiceFollowUpDetailReportDto>> GetStockInReturnFromPurchaseInvoiceFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var purchaseInvoiceReturns = await (from stockInReturnFromPurchaseInvoice in _stockInReturnHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
												from invoiceStockInReturn in _invoiceStockInReturnService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnFromPurchaseInvoice.StockInReturnHeaderId)
												from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == invoiceStockInReturn.PurchaseInvoiceReturnHeaderId)
												from purchaseInvoiceReturnDetail in _purchaseInvoiceReturnDetailService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeader.PurchaseInvoiceReturnHeaderId)
												where !purchaseInvoiceReturnHeader.IsDirectInvoice
												group new { purchaseInvoiceReturnHeader, purchaseInvoiceReturnDetail } by new { stockInReturnFromPurchaseInvoice.StockInReturnHeaderId, purchaseInvoiceReturnDetail.ItemId, purchaseInvoiceReturnDetail.ItemPackageId } into g
												select new { g.Key.StockInReturnHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.purchaseInvoiceReturnDetail.Quantity + x.purchaseInvoiceReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.purchaseInvoiceReturnHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												.ToDictionaryAsync(x => new { x.StockInReturnHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var anonymousDefault = new { Quantity = 0.0M, DocumentFullCodes = string.Empty };

			var query = from stockInReturnHeader in _stockInReturnHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from invoiceStockInReturn in _invoiceStockInReturnService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeader.StockInReturnHeaderId).DefaultIfEmpty()
						from purchaseInvoiceReturnHeader in _purchaseInvoiceReturnHeaderService.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == invoiceStockInReturn.PurchaseInvoiceReturnHeaderId).DefaultIfEmpty()
						from store in _storeService.GetAll().Where(x => x.StoreId == stockInReturnHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == stockInReturnHeader.MenuCode)

						from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeader.StockInReturnHeaderId)

						from item in _itemService.GetAll().Where(x => x.ItemId == stockInReturnDetail.ItemId)
						from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == stockInReturnDetail.ItemPackageId)
						from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == stockInReturnDetail.CostCenterId).DefaultIfEmpty()

						where stockInReturnHeader.PurchaseInvoiceHeaderId != null && (purchaseInvoiceReturnHeader == null || !purchaseInvoiceReturnHeader.IsDirectInvoice)
						orderby stockInReturnHeader.DocumentDate, stockInReturnHeader.StockInReturnHeaderId
						select new StockInReturnFromPurchaseInvoiceFollowUpDetailReportDto
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