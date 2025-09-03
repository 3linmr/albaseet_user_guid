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
	public class SalesInvoiceInterimFollowUpDetailReportService : ISalesInvoiceInterimFollowUpDetailReportService
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

		public SalesInvoiceInterimFollowUpDetailReportService(IProformaInvoiceHeaderService proformaInvoiceHeaderService, IProformaInvoiceDetailService proformaInvoiceDetailService, IStockOutHeaderService stockOutHeaderService, IStockOutDetailService stockOutDetailService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceDetailService salesInvoiceDetailService, IStockOutReturnHeaderService stockOutReturnHeaderService, IStockOutReturnDetailService stockOutReturnDetailService, IStoreService storeService, IHttpContextAccessor httpContextAccessor, IStringLocalizer<ProductRequestFollowUpReportService> localizer, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService, IMenuService menuService, IItemService itemService, ICostCenterService costCenterService, IItemPackageService itemPackageService, IInvoiceStockOutService invoiceStockOutService)
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

		public async Task<IQueryable<SalesInvoiceInterimFollowUpDetailReportDto>> GetSalesInvoiceInterimFollowUpDetailReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();


			//since it is impossible for a direct sales invoice return to made on a sales Invoice Interim, no need to check for it here
			var stockOutReturnFromSalesInvoices = await (from salesInvoiceInterimHeader in _salesInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && !x.IsDirectInvoice)
														 from stockOutReturnFromSalesInvoiceHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceInterimHeader.SalesInvoiceHeaderId)
														 from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnFromSalesInvoiceHeader.StockOutReturnHeaderId)
														 group new { stockOutReturnFromSalesInvoiceHeader, stockOutReturnDetail } by new { salesInvoiceInterimHeader.SalesInvoiceHeaderId, stockOutReturnDetail.ItemId, stockOutReturnDetail.ItemPackageId } into g
														 select new { g.Key.SalesInvoiceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.stockOutReturnDetail.Quantity + x.stockOutReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.stockOutReturnFromSalesInvoiceHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
														 .ToDictionaryAsync(x => new { x.SalesInvoiceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var salesInvoiceReturns = await (from salesInvoiceInterimHeader in _salesInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) && !x.IsDirectInvoice)
											 from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceInterimHeader.SalesInvoiceHeaderId)
											 from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId)
											 group new { salesInvoiceReturnHeader, salesInvoiceReturnDetail } by new { salesInvoiceInterimHeader.SalesInvoiceHeaderId, salesInvoiceReturnDetail.ItemId, salesInvoiceReturnDetail.ItemPackageId } into g
											 select new { g.Key.SalesInvoiceHeaderId, g.Key.ItemId, g.Key.ItemPackageId, Quantity = g.Sum(x => x.salesInvoiceReturnDetail.Quantity + x.salesInvoiceReturnDetail.BonusQuantity), DocumentFullCodes = string.Join(", ", g.Select(x => x.salesInvoiceReturnHeader).Distinct().Select(x => x.Prefix + x.DocumentCode + x.Suffix)) })
										     .ToDictionaryAsync(x => new { x.SalesInvoiceHeaderId, x.ItemId, x.ItemPackageId }, x => new { x.Quantity, x.DocumentFullCodes });

			var anonymousDefault = new { Quantity = 0.0M, DocumentFullCodes = string.Empty };

			var query = from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => storeIds.Contains(x.StoreId) && (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate))
						from store in _storeService.GetAll().Where(x => x.StoreId == salesInvoiceHeader.StoreId)
						from menu in _menuService.GetAll().Where(x => x.MenuCode == salesInvoiceHeader.MenuCode)

						from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)

						from item in _itemService.GetAll().Where(x => x.ItemId == salesInvoiceDetail.ItemId)
						from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == salesInvoiceDetail.ItemPackageId)
						from costCenter in _costCenterService.GetAll().Where(x => x.CostCenterId == salesInvoiceDetail.CostCenterId).DefaultIfEmpty()

						where !salesInvoiceHeader.IsDirectInvoice
						orderby salesInvoiceHeader.DocumentDate, salesInvoiceHeader.SalesInvoiceHeaderId
						select new SalesInvoiceInterimFollowUpDetailReportDto
						{
							SalesInvoiceHeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
							MenuCode = menu.MenuCode,
							MenuName = language == LanguageCode.Arabic ? menu.MenuNameAr : menu.MenuNameEn,
							DocumentFullCode = salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix,
							DocumentDate = salesInvoiceHeader.DocumentDate,
							StoreId = salesInvoiceHeader.StoreId,
							StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

							BarCode = salesInvoiceDetail.BarCode,
							ItemId = salesInvoiceDetail.ItemId,
							ItemCode = item.ItemCode,
							ItemName = salesInvoiceDetail.ItemNote != null ? salesInvoiceDetail.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
							ItemPackageId = itemPackage.ItemPackageId,
							ItemPackageCode = itemPackage.ItemPackageCode,
							ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
							Packing = salesInvoiceDetail.Packing,
							Quantity = salesInvoiceDetail.Quantity,
							CostCenterId = costCenter.CostCenterId,
							CostCenterCode = costCenter.CostCenterCode,
							CostCenterName = language == LanguageCode.Arabic ? costCenter.CostCenterNameAr : costCenter.CostCenterNameEn,
							PackagePrice = salesInvoiceDetail.SellingPrice,
							NetValue = salesInvoiceDetail.NetValue,
							CostPrice = salesInvoiceDetail.CostPrice,
							CostPackage = salesInvoiceDetail.CostPackage,
							CostValue = salesInvoiceDetail.CostValue,

							StockOutReturnFromSalesInvoiceQuantity = stockOutReturnFromSalesInvoices.GetValueOrDefault(new { salesInvoiceHeader.SalesInvoiceHeaderId, salesInvoiceDetail.ItemId, salesInvoiceDetail.ItemPackageId }, anonymousDefault).Quantity,
							StockOutReturnFromSalesInvoiceDocumentFullCodes = stockOutReturnFromSalesInvoices.GetValueOrDefault(new { salesInvoiceHeader.SalesInvoiceHeaderId, salesInvoiceDetail.ItemId, salesInvoiceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,
							SalesInvoiceReturnQuantity = salesInvoiceReturns.GetValueOrDefault(new { salesInvoiceHeader.SalesInvoiceHeaderId, salesInvoiceDetail.ItemId, salesInvoiceDetail.ItemPackageId }, anonymousDefault).Quantity,
							SalesInvoiceReturnDocumentFullCodes = salesInvoiceReturns.GetValueOrDefault(new { salesInvoiceHeader.SalesInvoiceHeaderId, salesInvoiceDetail.ItemId, salesInvoiceDetail.ItemPackageId }, anonymousDefault).DocumentFullCodes,

							Reference = salesInvoiceHeader.Reference,
							RemarksAr = salesInvoiceHeader.RemarksAr,
							RemarksEn = salesInvoiceHeader.RemarksEn,

							CreatedAt = salesInvoiceHeader.CreatedAt,
							UserNameCreated = salesInvoiceHeader.UserNameCreated,
							ModifiedAt = salesInvoiceHeader.ModifiedAt,
							UserNameModified = salesInvoiceHeader.UserNameModified
						};

			return query;
		}
	}
}