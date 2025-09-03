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
	public class ProformaInvoiceFollowUpDetailReportService : IProformaInvoiceFollowUpDetailReportService
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

		public ProformaInvoiceFollowUpDetailReportService(IProformaInvoiceHeaderService proformaInvoiceHeaderService, IProformaInvoiceDetailService proformaInvoiceDetailService, IStockOutHeaderService stockOutHeaderService, IStockOutDetailService stockOutDetailService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceDetailService salesInvoiceDetailService, IStockOutReturnHeaderService stockOutReturnHeaderService, IStockOutReturnDetailService stockOutReturnDetailService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService, IMenuService menuService, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService)
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
		}

		public async Task<IQueryable<ProformaInvoiceFollowUpDetailReportDto>> GetProformaInvoiceFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();


			var stockOutFromProformaInvoices = await (from proformaInvoiceHeader in _proformaInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
													  from stockOutFromProformaInvoiceHeader in _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeader.ProformaInvoiceHeaderId)
													  from directSalesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeader.ProformaInvoiceHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
													  from stockOutDetail in _stockOutDetailService.GetAll().Where(x => x.StockOutHeaderId == stockOutFromProformaInvoiceHeader.StockOutHeaderId)
													  where directSalesInvoiceHeader == null
													  group new { stockOutFromProformaInvoiceHeader, stockOutDetail } by new { proformaInvoiceHeader.ProformaInvoiceHeaderId, stockOutDetail.ItemId, stockOutDetail.ItemPackageId } into g
													  select new { g.Key.ProformaInvoiceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockOutDetail.Quantity + x.stockOutDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockOutFromProformaInvoiceHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												      .ToDictionaryAsync(x => new { x.ProformaInvoiceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockOutReturnFromStockOuts = await (from proformaInvoiceHeader in _proformaInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
													 from stockOutFromProformaInvoiceHeader in _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeader.ProformaInvoiceHeaderId)
													 from stockOutReturnFromStockOutHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutFromProformaInvoiceHeader.StockOutHeaderId)
													 from directSalesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeader.ProformaInvoiceHeaderId && x.IsDirectInvoice).Take(1).DefaultIfEmpty()
													 from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnFromStockOutHeader.StockOutReturnHeaderId)
													 where directSalesInvoiceHeader == null
													 group new { stockOutReturnFromStockOutHeader, stockOutReturnDetail } by new { proformaInvoiceHeader.ProformaInvoiceHeaderId, stockOutReturnDetail.ItemId, stockOutReturnDetail.ItemPackageId } into g
													 select new { g.Key.ProformaInvoiceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockOutReturnDetail.Quantity + x.stockOutReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockOutReturnFromStockOutHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
												     .ToDictionaryAsync(x => new { x.ProformaInvoiceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var salesInvoiceInterims = await (from proformaInvoiceHeader in _proformaInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
											  from salesInvoiceInterimHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeader.ProformaInvoiceHeaderId && !x.IsDirectInvoice)
											  from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceInterimHeader.SalesInvoiceHeaderId)
											  group new { salesInvoiceInterimHeader, salesInvoiceDetail } by new { proformaInvoiceHeader.ProformaInvoiceHeaderId, salesInvoiceDetail.ItemId, salesInvoiceDetail.ItemPackageId } into g
											  select new { g.Key.ProformaInvoiceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.salesInvoiceDetail.Quantity + x.salesInvoiceDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.salesInvoiceInterimHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
										      .ToDictionaryAsync(x => new { x.ProformaInvoiceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var stockOutReturnFromSalesInvoices = await (from proformaInvoiceHeader in _proformaInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
														 from salesInvoiceInterimHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeader.ProformaInvoiceHeaderId && !x.IsDirectInvoice)
														 from stockOutReturnFromSalesInvoiceHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceInterimHeader.SalesInvoiceHeaderId)
														 from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnFromSalesInvoiceHeader.StockOutReturnHeaderId)
														 group new { stockOutReturnFromSalesInvoiceHeader, stockOutReturnDetail } by new { proformaInvoiceHeader.ProformaInvoiceHeaderId, stockOutReturnDetail.ItemId, stockOutReturnDetail.ItemPackageId } into g
														 select new { g.Key.ProformaInvoiceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockOutReturnDetail.Quantity + x.stockOutReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockOutReturnFromSalesInvoiceHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
														 .ToDictionaryAsync(x => new { x.ProformaInvoiceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var salesInvoiceReturns = await (from proformaInvoiceHeader in _proformaInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
											 from salesInvoiceInterimHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeader.ProformaInvoiceHeaderId && !x.IsDirectInvoice)
											 from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceInterimHeader.SalesInvoiceHeaderId)
											 from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId)
											 group new { salesInvoiceReturnHeader, salesInvoiceReturnDetail } by new { proformaInvoiceHeader.ProformaInvoiceHeaderId, salesInvoiceReturnDetail.ItemId, salesInvoiceReturnDetail.ItemPackageId } into g
											 select new { g.Key.ProformaInvoiceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.salesInvoiceReturnDetail.Quantity + x.salesInvoiceReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.salesInvoiceReturnHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
										     .ToDictionaryAsync(x => new { x.ProformaInvoiceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var anonymousDefault = new { Quantity = 0.0M, DocumentFullCodes = string.Empty };

			var query = from proformaInvoiceHeader in _proformaInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from store in _storeService.GetAll().Where(x => x.StoreId == proformaInvoiceHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == MenuCodeData.ProformaInvoice)

						from proformaInvoiceDetail in _proformaInvoiceDetailService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeader.ProformaInvoiceHeaderId)

						from item in _itemService.GetAll().Where(x => x.ItemId == proformaInvoiceDetail.ItemId)
						from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == proformaInvoiceDetail.ItemPackageId)
						from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == proformaInvoiceDetail.CostCenterId).DefaultIfEmpty()

						orderby proformaInvoiceHeader.DocumentDate, proformaInvoiceHeader.ProformaInvoiceHeaderId
						select new ProformaInvoiceFollowUpDetailReportDto
						{
							ProformaInvoiceHeaderId = proformaInvoiceHeader.ProformaInvoiceHeaderId,
							MenuCode = menu.MenuCode,
							MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
							DocumentFullCode = proformaInvoiceHeader.Prefix + proformaInvoiceHeader.DocumentCode + proformaInvoiceHeader.Suffix,
							DocumentDate = proformaInvoiceHeader.DocumentDate,
							StoreId = proformaInvoiceHeader.StoreId,
							StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

							BarCode = proformaInvoiceDetail.BarCode,
							ItemId = proformaInvoiceDetail.ItemId,
							ItemCode = item.ItemCode,
							ItemName = proformaInvoiceDetail.ItemNote != null ? proformaInvoiceDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
							ItemPackageId = itemPackage.ItemPackageId,
							ItemPackageCode = itemPackage.ItemPackageCode,
							ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
							Packing = proformaInvoiceDetail.Packing,
							Quantity = proformaInvoiceDetail.Quantity,
							CostCenterId = costCenter.CostCenterId,
							CostCenterCode = costCenter.CostCenterCode,
							CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
							PackagePrice = proformaInvoiceDetail.SellingPrice,
							NetValue = proformaInvoiceDetail.NetValue,
							CostPrice = proformaInvoiceDetail.CostPrice,
							CostPackage = proformaInvoiceDetail.CostPackage,
							CostValue = proformaInvoiceDetail.CostValue,

							StockOutFromProformaInvoiceQuantity = stockOutFromProformaInvoices.GetValueOrDefault(new { proformaInvoiceHeader.ProformaInvoiceHeaderId, proformaInvoiceDetail.ItemId, proformaInvoiceDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockOutFromProformaInvoiceDocumentFullCodes = stockOutFromProformaInvoices.GetValueOrDefault(new { proformaInvoiceHeader.ProformaInvoiceHeaderId, proformaInvoiceDetail.ItemId, proformaInvoiceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockOutReturnFromStockOutQuantity = stockOutReturnFromStockOuts.GetValueOrDefault(new { proformaInvoiceHeader.ProformaInvoiceHeaderId, proformaInvoiceDetail.ItemId, proformaInvoiceDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockOutReturnFromStockOutDocumentFullCodes = stockOutReturnFromStockOuts.GetValueOrDefault(new { proformaInvoiceHeader.ProformaInvoiceHeaderId, proformaInvoiceDetail.ItemId, proformaInvoiceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							SalesInvoiceInterimQuantity = salesInvoiceInterims.GetValueOrDefault(new { proformaInvoiceHeader.ProformaInvoiceHeaderId, proformaInvoiceDetail.ItemId, proformaInvoiceDetail.ItemPackageId }, anonymousDefault).Quantity,
							SalesInvoiceInterimDocumentFullCodes = salesInvoiceInterims.GetValueOrDefault(new { proformaInvoiceHeader.ProformaInvoiceHeaderId, proformaInvoiceDetail.ItemId, proformaInvoiceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							StockOutReturnFromSalesInvoiceQuantity = stockOutReturnFromSalesInvoices.GetValueOrDefault(new { proformaInvoiceHeader.ProformaInvoiceHeaderId, proformaInvoiceDetail.ItemId, proformaInvoiceDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockOutReturnFromSalesInvoiceDocumentFullCodes = stockOutReturnFromSalesInvoices.GetValueOrDefault(new { proformaInvoiceHeader.ProformaInvoiceHeaderId, proformaInvoiceDetail.ItemId, proformaInvoiceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							SalesInvoiceReturnQuantity = salesInvoiceReturns.GetValueOrDefault(new { proformaInvoiceHeader.ProformaInvoiceHeaderId, proformaInvoiceDetail.ItemId, proformaInvoiceDetail.ItemPackageId }, anonymousDefault).Quantity,
							SalesInvoiceReturnDocumentFullCodes = salesInvoiceReturns.GetValueOrDefault(new { proformaInvoiceHeader.ProformaInvoiceHeaderId, proformaInvoiceDetail.ItemId, proformaInvoiceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,

							Reference = proformaInvoiceHeader.Reference,
							RemarksAr = proformaInvoiceHeader.RemarksAr,
							RemarksEn = proformaInvoiceHeader.RemarksEn,

							CreatedAt = proformaInvoiceHeader.CreatedAt,
							UserNameCreated = proformaInvoiceHeader.UserNameCreated,
							ModifiedAt = proformaInvoiceHeader.ModifiedAt,
							UserNameModified = proformaInvoiceHeader.UserNameModified
						};

			return query;
		}
	}
}