using Sales.CoreOne.Contracts;
using Compound.CoreOne.Contracts.Reports.FollowUpDetailReports;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Modules;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Models.StaticData;
using Shared.CoreOne.Contracts.Menus;
using Sales.CoreOne.Models.Domain;
using Compound.Service.Services.Reports.FollowUpReports;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.CostCenters;
using Microsoft.EntityFrameworkCore;
using Compound.CoreOne.Models.Dtos.Reports.FollowUpReports;

namespace Compound.Service.Services.Reports.FollowUpDetailReports
{
	public class StockOutReturnFromSalesInvoiceFollowUpDetailReportService : IStockOutReturnFromSalesInvoiceFollowUpDetailReportService
	{
		private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
		private readonly IProformaInvoiceDetailService _proformaInvoiceDetailService;
		private readonly IStockOutHeaderService _stockOutHeaderService;
		private readonly IStockOutDetailService _stockOutDetailService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
		private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
		private readonly IStockOutReturnDetailService _stockOutReturnDetailService;
		private readonly IStoreService _storeService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<ProductRequestFollowUpReportService> _localizer;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly ISalesInvoiceReturnDetailService _salesInvoiceReturnDetailService;
		private readonly IMenuService _menuService;
		private readonly IItemService _itemService;
		private readonly ICostCenterService _costCenterService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IInvoiceStockOutReturnService _invoiceStockOutReturnService;

		public StockOutReturnFromSalesInvoiceFollowUpDetailReportService(IProformaInvoiceHeaderService proformaInvoiceHeaderService, IProformaInvoiceDetailService proformaInvoiceDetailService, IStockOutHeaderService stockOutHeaderService, IStockOutDetailService stockOutDetailService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceDetailService salesInvoiceDetailService, IStockOutReturnHeaderService stockOutReturnHeaderService, IStockOutReturnDetailService stockOutReturnDetailService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService, IMenuService menuService, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService, IInvoiceStockOutReturnService invoiceStockOutReturnService)
		{
			_proformaInvoiceHeaderService = proformaInvoiceHeaderService;
			_proformaInvoiceDetailService = proformaInvoiceDetailService;
			_stockOutHeaderService = stockOutHeaderService;
			_stockOutDetailService = stockOutDetailService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_salesInvoiceDetailService = salesInvoiceDetailService;
			_stockOutReturnHeaderService = stockOutReturnHeaderService;
			_stockOutReturnDetailService = stockOutReturnDetailService;
			_storeService = storeService;
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_salesInvoiceReturnDetailService = salesInvoiceReturnDetailService;
			_menuService = menuService;
			_itemService = itemService;
			_costCenterService = costCenterService;
			_itemPackageService = itemPackageService;
			_invoiceStockOutReturnService = invoiceStockOutReturnService;
		}

		public async Task<IQueryable<StockOutReturnFromSalesInvoiceFollowUpDetailReportDto>> GetStockOutReturnFromSalesInvoiceFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();


			var salesInvoiceReturns = await (from stockOutReturnFromSalesInvoice in _stockOutReturnHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
											 from invoiceStockOutReturn in _invoiceStockOutReturnService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnFromSalesInvoice.StockOutReturnHeaderId)
											 from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == invoiceStockOutReturn.SalesInvoiceReturnHeaderId)
											 from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId)
											 where !salesInvoiceReturnHeader.IsDirectInvoice
											 group new { salesInvoiceReturnHeader, salesInvoiceReturnDetail } by new { stockOutReturnFromSalesInvoice.StockOutReturnHeaderId, salesInvoiceReturnDetail.ItemId, salesInvoiceReturnDetail.ItemPackageId } into g
											 select new { g.Key.StockOutReturnHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.salesInvoiceReturnDetail.Quantity + x.salesInvoiceReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.salesInvoiceReturnHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
										     .ToDictionaryAsync(x => new { x.StockOutReturnHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var anonymousDefault = new { Quantity = 0.0M, DocumentFullCodes = string.Empty };

			var query = from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from invoiceStockOutReturn in _invoiceStockOutReturnService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId).DefaultIfEmpty()
						from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == invoiceStockOutReturn.SalesInvoiceReturnHeaderId).DefaultIfEmpty()
						from store in _storeService.GetAll().Where(x => x.StoreId == stockOutReturnHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == stockOutReturnHeader.MenuCode)

						from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId)

						from item in _itemService.GetAll().Where(x => x.ItemId == stockOutReturnDetail.ItemId)
						from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == stockOutReturnDetail.ItemPackageId)
						from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == stockOutReturnDetail.CostCenterId).DefaultIfEmpty()

						where stockOutReturnHeader.SalesInvoiceHeaderId != null && (salesInvoiceReturnHeader == null || !salesInvoiceReturnHeader.IsDirectInvoice)
						orderby stockOutReturnHeader.DocumentDate, stockOutReturnHeader.StockOutReturnHeaderId
						select new StockOutReturnFromSalesInvoiceFollowUpDetailReportDto
						{
							StockOutReturnHeaderId = stockOutReturnHeader.StockOutReturnHeaderId,
							MenuCode = menu.MenuCode,
							MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
							DocumentFullCode = stockOutReturnHeader.Prefix + stockOutReturnHeader.DocumentCode + stockOutReturnHeader.Suffix,
							DocumentDate = stockOutReturnHeader.DocumentDate,
							StoreId = stockOutReturnHeader.StoreId,
							StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

							BarCode = stockOutReturnDetail.BarCode,
							ItemId = stockOutReturnDetail.ItemId,
							ItemCode = item.ItemCode,
							ItemName = stockOutReturnDetail.ItemNote != null ? stockOutReturnDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
							ItemPackageId = itemPackage.ItemPackageId,
							ItemPackageCode = itemPackage.ItemPackageCode,
							ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
							Packing = stockOutReturnDetail.Packing,
							Quantity = stockOutReturnDetail.Quantity,
							CostCenterId = costCenter.CostCenterId,
							CostCenterCode = costCenter.CostCenterCode,
							CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
							PackagePrice = stockOutReturnDetail.SellingPrice,
							NetValue = stockOutReturnDetail.NetValue,
							CostPrice = stockOutReturnDetail.CostPrice,
							CostPackage = stockOutReturnDetail.CostPackage,
							CostValue = stockOutReturnDetail.CostValue,

							SalesInvoiceReturnQuantity = salesInvoiceReturns.GetValueOrDefault(new { stockOutReturnHeader.StockOutReturnHeaderId, stockOutReturnDetail.ItemId, stockOutReturnDetail.ItemPackageId }, anonymousDefault).Quantity,
							SalesInvoiceReturnDocumentFullCodes = salesInvoiceReturns.GetValueOrDefault(new { stockOutReturnHeader.StockOutReturnHeaderId, stockOutReturnDetail.ItemId, stockOutReturnDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,

							Reference = stockOutReturnHeader.Reference,
							RemarksAr = stockOutReturnHeader.RemarksAr,
							RemarksEn = stockOutReturnHeader.RemarksEn,

							CreatedAt = stockOutReturnHeader.CreatedAt,
							UserNameCreated = stockOutReturnHeader.UserNameCreated,
							ModifiedAt = stockOutReturnHeader.ModifiedAt,
							UserNameModified = stockOutReturnHeader.UserNameModified
						};

			return query;
		}
	}
}