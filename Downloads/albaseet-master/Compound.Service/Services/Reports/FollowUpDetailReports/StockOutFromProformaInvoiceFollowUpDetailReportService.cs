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
	public class StockOutFromProformaInvoiceFollowUpDetailReportService : IStockOutFromProformaInvoiceFollowUpDetailReportService
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
		private readonly IInvoiceStockOutService _invoiceStockOutService;

		public StockOutFromProformaInvoiceFollowUpDetailReportService(IProformaInvoiceHeaderService proformaInvoiceHeaderService, IProformaInvoiceDetailService proformaInvoiceDetailService, IStockOutHeaderService stockOutHeaderService, IStockOutDetailService stockOutDetailService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceDetailService salesInvoiceDetailService, IStockOutReturnHeaderService stockOutReturnHeaderService, IStockOutReturnDetailService stockOutReturnDetailService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService, IMenuService menuService, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService, IInvoiceStockOutService invoiceStockOutService)
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
			_invoiceStockOutService = invoiceStockOutService;
		}

		public async Task<IQueryable<StockOutFromProformaInvoiceFollowUpDetailReportDto>> GetStockOutFromProformaInvoiceFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();


			var stockOutReturnFromStockOuts = await (from stockOutFromProformaInvoiceHeader in _stockOutHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.ProformaInvoiceHeaderId != null)
													 from stockOutReturnFromStockOutHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutFromProformaInvoiceHeader.StockOutHeaderId)
													 from directSalesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == stockOutFromProformaInvoiceHeader.ProformaInvoiceHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
													 from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnFromStockOutHeader.StockOutReturnHeaderId)
													 where directSalesInvoiceHeader == null
													 group new { stockOutReturnFromStockOutHeader, stockOutReturnDetail } by new { stockOutFromProformaInvoiceHeader.StockOutHeaderId, stockOutReturnDetail.ItemId, stockOutReturnDetail.ItemPackageId } into g
													 select new { g.Key.StockOutHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockOutReturnDetail.Quantity + x.stockOutReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockOutReturnFromStockOutHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												     .ToDictionaryAsync(x => new { x.StockOutHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var salesInvoiceInterims = await (from stockOutFromProformaInvoiceHeader in _stockOutHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.ProformaInvoiceHeaderId != null)
											  from invoiceStockOut in _invoiceStockOutService.GetAll().Where(x => x.StockOutHeaderId == stockOutFromProformaInvoiceHeader.StockOutHeaderId)
											  from salesInvoiceInterimHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == invoiceStockOut.SalesInvoiceHeaderId && !x.IsDirectInvoice)
											  from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceInterimHeader.SalesInvoiceHeaderId)
											  group new { salesInvoiceInterimHeader, salesInvoiceDetail } by new { stockOutFromProformaInvoiceHeader.StockOutHeaderId, salesInvoiceDetail.ItemId, salesInvoiceDetail.ItemPackageId } into g
											  select new { g.Key.StockOutHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.salesInvoiceDetail.Quantity + x.salesInvoiceDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.salesInvoiceInterimHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
										      .ToDictionaryAsync(x => new { x.StockOutHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockOutReturnFromSalesInvoices = await (from stockOutFromProformaInvoiceHeader in _stockOutHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.ProformaInvoiceHeaderId != null)
											             from invoiceStockOut in _invoiceStockOutService.GetAll().Where(x => x.StockOutHeaderId == stockOutFromProformaInvoiceHeader.StockOutHeaderId)
														 from salesInvoiceInterimHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == invoiceStockOut.SalesInvoiceHeaderId && !x.IsDirectInvoice)
														 from stockOutReturnFromSalesInvoiceHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceInterimHeader.SalesInvoiceHeaderId)
														 from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnFromSalesInvoiceHeader.StockOutReturnHeaderId)
														 group new { stockOutReturnFromSalesInvoiceHeader, stockOutReturnDetail } by new { stockOutFromProformaInvoiceHeader.StockOutHeaderId, stockOutReturnDetail.ItemId, stockOutReturnDetail.ItemPackageId } into g
														 select new { g.Key.StockOutHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockOutReturnDetail.Quantity + x.stockOutReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockOutReturnFromSalesInvoiceHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
														 .ToDictionaryAsync(x => new { x.StockOutHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var salesInvoiceReturns = await (from stockOutFromProformaInvoiceHeader in _stockOutHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && x.ProformaInvoiceHeaderId != null)
											 from invoiceStockOut in _invoiceStockOutService.GetAll().Where(x => x.StockOutHeaderId == stockOutFromProformaInvoiceHeader.StockOutHeaderId)
											 from salesInvoiceInterimHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == invoiceStockOut.SalesInvoiceHeaderId && !x.IsDirectInvoice)
											 from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceInterimHeader.SalesInvoiceHeaderId)
											 from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId)
											 group new { salesInvoiceReturnHeader, salesInvoiceReturnDetail } by new { stockOutFromProformaInvoiceHeader.StockOutHeaderId, salesInvoiceReturnDetail.ItemId, salesInvoiceReturnDetail.ItemPackageId } into g
											 select new { g.Key.StockOutHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.salesInvoiceReturnDetail.Quantity + x.salesInvoiceReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.salesInvoiceReturnHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
										     .ToDictionaryAsync(x => new { x.StockOutHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var anonymousDefault = new { Quantity = 0.0M, DocumentFullCodes = string.Empty };

			var query = from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from directSalesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == stockOutHeader.ProformaInvoiceHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
						from store in _storeService.GetAll().Where(x => x.StoreId == stockOutHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == stockOutHeader.MenuCode)

						from stockOutDetail in _stockOutDetailService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)

						from item in _itemService.GetAll().Where(x => x.ItemId == stockOutDetail.ItemId)
						from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == stockOutDetail.ItemPackageId)
						from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == stockOutDetail.CostCenterId).DefaultIfEmpty()

						where stockOutHeader.ProformaInvoiceHeaderId != null && directSalesInvoiceHeader == null
						orderby stockOutHeader.DocumentDate, stockOutHeader.StockOutHeaderId
						select new StockOutFromProformaInvoiceFollowUpDetailReportDto
						{
							StockOutHeaderId = stockOutHeader.StockOutHeaderId,
							MenuCode = menu.MenuCode,
							MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
							DocumentFullCode = stockOutHeader.Prefix + stockOutHeader.DocumentCode + stockOutHeader.Suffix,
							DocumentDate = stockOutHeader.DocumentDate,
							StoreId = stockOutHeader.StoreId,
							StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

							BarCode = stockOutDetail.BarCode,
							ItemId = stockOutDetail.ItemId,
							ItemCode = item.ItemCode,
							ItemName = stockOutDetail.ItemNote != null ? stockOutDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
							ItemPackageId = itemPackage.ItemPackageId,
							ItemPackageCode = itemPackage.ItemPackageCode,
							ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
							Packing = stockOutDetail.Packing,
							Quantity = stockOutDetail.Quantity,
							CostCenterId = costCenter.CostCenterId,
							CostCenterCode = costCenter.CostCenterCode,
							CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
							PackagePrice = stockOutDetail.SellingPrice,
							NetValue = stockOutDetail.NetValue,
							CostPrice = stockOutDetail.CostPrice,
							CostPackage = stockOutDetail.CostPackage,
							CostValue = stockOutDetail.CostValue,

							StockOutReturnFromStockOutQuantity = stockOutReturnFromStockOuts.GetValueOrDefault(new { stockOutHeader.StockOutHeaderId, stockOutDetail.ItemId, stockOutDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockOutReturnFromStockOutDocumentFullCodes = stockOutReturnFromStockOuts.GetValueOrDefault(new { stockOutHeader.StockOutHeaderId, stockOutDetail.ItemId, stockOutDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							SalesInvoiceInterimQuantity = salesInvoiceInterims.GetValueOrDefault(new { stockOutHeader.StockOutHeaderId, stockOutDetail.ItemId, stockOutDetail.ItemPackageId }, anonymousDefault).Quantity,
							SalesInvoiceInterimDocumentFullCodes = salesInvoiceInterims.GetValueOrDefault(new { stockOutHeader.StockOutHeaderId, stockOutDetail.ItemId, stockOutDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockOutReturnFromSalesInvoiceQuantity = stockOutReturnFromSalesInvoices.GetValueOrDefault(new { stockOutHeader.StockOutHeaderId, stockOutDetail.ItemId, stockOutDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockOutReturnFromSalesInvoiceDocumentFullCodes = stockOutReturnFromSalesInvoices.GetValueOrDefault(new { stockOutHeader.StockOutHeaderId, stockOutDetail.ItemId, stockOutDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							SalesInvoiceReturnQuantity = salesInvoiceReturns.GetValueOrDefault(new { stockOutHeader.StockOutHeaderId, stockOutDetail.ItemId, stockOutDetail.ItemPackageId }, anonymousDefault).Quantity,
							SalesInvoiceReturnDocumentFullCodes = salesInvoiceReturns.GetValueOrDefault(new { stockOutHeader.StockOutHeaderId, stockOutDetail.ItemId, stockOutDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,

							Reference = stockOutHeader.Reference,
							RemarksAr = stockOutHeader.RemarksAr,
							RemarksEn = stockOutHeader.RemarksEn,

							CreatedAt = stockOutHeader.CreatedAt,
							UserNameCreated = stockOutHeader.UserNameCreated,
							ModifiedAt = stockOutHeader.ModifiedAt,
							UserNameModified = stockOutHeader.UserNameModified
						};

			return query;
		}
	}
}