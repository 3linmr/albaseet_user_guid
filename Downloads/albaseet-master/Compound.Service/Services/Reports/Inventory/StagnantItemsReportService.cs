using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Contracts.Reports.Inventory;
using Compound.CoreOne.Contracts.Reports.Shared;
using Shared.CoreOne.Contracts.Menus;
using Shared.Helper.Identity;
using Microsoft.AspNetCore.Http;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Compound.CoreOne.Models.Dtos.Reports.Shared;
using Compound.CoreOne.Models.Dtos.Reports.Inventory;
using Microsoft.EntityFrameworkCore;
using Shared.Helper.Logic;
using Sales.CoreOne.Contracts;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Contracts.Accounts;

namespace Compound.Service.Services.Reports.Inventory
{
	public class StagnantItemsReportService : IStagnantItemsReportService
	{
		private readonly IStockTakingsReportService _stockTakingsReportService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
		private readonly IItemService _itemService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IGeneralInventoryDocumentService _generalInventoryDocumentService;
		private readonly IStoreService _storeService;
		private readonly IItemTypeService _itemTypeService;
		private readonly IStringLocalizer<StagnantItemsReportService> _localizer;
		private readonly IBranchService _branchService;
		private readonly IVendorService _vendorService;
		private readonly ITaxTypeService _taxTypeService;
		private readonly IItemCategoryService _itemCategoryService;
		private readonly IItemSubCategoryService _itemSubCategoryService;
		private readonly IItemSectionService _itemSectionService;
		private readonly IItemSubSectionService _itemSubSectionService;
		private readonly IMainItemService _mainItemService;
		private readonly IAccountService _accountService;
		private readonly ITaxPercentService _taxPercentService;
		private readonly IItemReportDataService _itemReportDataService;

		public StagnantItemsReportService(IStockTakingsReportService stockTakingsReportService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceDetailService salesInvoiceDetailService, IItemService itemService, IHttpContextAccessor httpContextAccessor, IGeneralInventoryDocumentService generalInventoryDocumentService, IStoreService storeService, IItemTypeService itemTypeService, IStringLocalizer<StagnantItemsReportService> localizer, IBranchService branchService, IVendorService vendorService, ITaxTypeService taxTypeService, IItemCategoryService itemCategoryService, IItemSubCategoryService itemSubCategoryService, IItemSectionService itemSectionService, IItemSubSectionService itemSubSectionService, IMainItemService mainItemService, IAccountService accountService, ITaxPercentService taxPercentService, IItemReportDataService itemReportDataService)
		{
			_stockTakingsReportService = stockTakingsReportService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_salesInvoiceDetailService = salesInvoiceDetailService;
			_itemService = itemService;
			_httpContextAccessor = httpContextAccessor;
			_generalInventoryDocumentService = generalInventoryDocumentService;
			_storeService = storeService;
			_itemTypeService = itemTypeService;
			_localizer = localizer;
			_branchService = branchService;
			_vendorService = vendorService;
			_taxTypeService = taxTypeService;
			_itemCategoryService = itemCategoryService;
			_itemSubCategoryService = itemSubCategoryService;
			_itemSectionService = itemSectionService;
			_itemSubSectionService = itemSubSectionService;
			_mainItemService = mainItemService;
			_accountService = accountService;
			_taxPercentService = taxPercentService;
			_itemReportDataService = itemReportDataService;
		}

		public async Task<IQueryable<StagnantItemsReportDto>> GetStagnantItemsReport(List<int> storeIds, int daysSinceLastSold)
		{
			var today = DateHelper.GetDateTimeNow().Date;

			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyIds = await _storeService.GetAllStores().Where(x => storeIds.Contains(x.StoreId)).Select(x => x.CompanyId).ToListAsync();

			var perItemOtherTaxes = await _itemReportDataService.GetItemOtherTaxes(companyIds);
			var perItemAttributes = await _itemReportDataService.GetItemAttributes(companyIds);
			var perItemMenuNotes = await _itemReportDataService.GetItemMenuNotes(companyIds);

			var stocks = from generalInventoryDocument in _generalInventoryDocumentService.GetGeneralInventoryDocuments()
						 group generalInventoryDocument by new { generalInventoryDocument.StoreId, generalInventoryDocument.ItemId, generalInventoryDocument.ItemNote } into g
						 select new
						 {
							 StoreId = g.Key.StoreId,
							 ItemId = g.Key.ItemId,
							 ItemNote = g.Key.ItemNote,
							 IsAvailable = g.Sum(x => x.InQuantity - x.OutQuantity + x.PendingInQuantity - x.PendingOutQuantity + x.OpenQuantity) > 0,
						 };

			var salesInvoicesWithDateAndItemNote = from salesInvoiceDetail in _salesInvoiceDetailService.GetAll()
												   from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceDetail.SalesInvoiceHeaderId)
												   select new
												   {
													   SalesInvoiceHeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
													   DocumentFullCode = salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix,
													   DocumentDate = salesInvoiceHeader.DocumentDate,
													   StoreId = salesInvoiceHeader.StoreId,
													   ItemId = salesInvoiceDetail.ItemId,
													   ItemNote = salesInvoiceDetail.ItemNote,
												   };


			var fromLastSoldDate = DateHelper.GetDateTimeNow().AddDays(-daysSinceLastSold);

			return from item in _itemService.GetAll().Where(x => companyIds.Contains(x.CompanyId))
				   from itemType in _itemTypeService.GetAll().Where(x => x.ItemTypeId == item.ItemTypeId)
				   from itemCategory in _itemCategoryService.GetAll().Where(x => x.ItemCategoryId == item.ItemCategoryId).DefaultIfEmpty()
				   from itemSubCategory in _itemSubCategoryService.GetAll().Where(x => x.ItemSubCategoryId == item.ItemSubCategoryId).DefaultIfEmpty()
				   from itemSection in _itemSectionService.GetAll().Where(x => x.ItemSectionId == item.ItemSectionId).DefaultIfEmpty()
				   from itemSubSection in _itemSubSectionService.GetAll().Where(x => x.ItemSubSectionId == item.ItemSubSectionId).DefaultIfEmpty()
				   from mainItem in _mainItemService.GetAll().Where(x => x.MainItemId == item.MainItemId).DefaultIfEmpty()

				   from vendor in _vendorService.GetAll().Where(x => x.VendorId == item.VendorId).DefaultIfEmpty()

				   from salesAccount in _accountService.GetAll().Where(x => x.AccountId == item.SalesAccountId).DefaultIfEmpty()
				   from purchaseAccount in _accountService.GetAll().Where(x => x.AccountId == item.PurchaseAccountId).DefaultIfEmpty()

				   from taxType in _taxTypeService.GetAll().Where(x => x.TaxTypeId == item.TaxTypeId)
				   from vatPercent in _taxPercentService.GetAllCompanyVatPercents(today).Where(x => x.CompanyId == item.CompanyId).DefaultIfEmpty()

				   from store in _storeService.GetAll().Where(x => storeIds.Contains(x.StoreId))
				   from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId && x.CompanyId == item.CompanyId) //store and item must be from same company
				   
				   from stock in stocks.Where(x => x.ItemId == item.ItemId && x.StoreId == store.StoreId).DefaultIfEmpty()
				   from lastSalesInvoice in salesInvoicesWithDateAndItemNote.Where(x => x.StoreId == store.StoreId && x.ItemId == item.ItemId && x.ItemNote == stock.ItemNote).OrderByDescending(x => x.DocumentDate).Take(1).DefaultIfEmpty()
				   where (lastSalesInvoice.DocumentDate == null || lastSalesInvoice.DocumentDate < fromLastSoldDate)
				   select new StagnantItemsReportDto
				   {
					   ItemId = item.ItemId,
					   ItemCode = item.ItemCode,
					   ItemName = item.ItemTypeId == ItemTypeData.Note ? stock.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
					   ItemTypeId = item.ItemTypeId,
					   ItemTypeName = language == LanguageCode.Arabic ? itemType.ItemTypeNameAr : itemType.ItemTypeNameEn,

					   StoreId = store.StoreId,
					   StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

					   LastSalesInvoiceHeaderId = lastSalesInvoice.SalesInvoiceHeaderId,
					   LastSalesInvoiceCode = lastSalesInvoice.DocumentFullCode,
					   LastSalesInvoiceDate = lastSalesInvoice.DocumentDate,
					   DaysSinceLastSold = Math.Max(EF.Functions.DateDiffDay(lastSalesInvoice.DocumentDate, today), 0),

					   IsAvailable = stock.IsAvailable ? (string)_localizer["HasAvailableBalance"] : (string)_localizer["NoAvailableBalance"],

					   ReorderPointQuantity = item.ReorderPointQuantity,

					   VendorId = vendor.VendorId,
					   VendorCode = vendor.VendorCode,
					   VendorName = language == LanguageCode.Arabic ? vendor.VendorNameAr : vendor.VendorNameEn,

					   TaxTypeName = language == LanguageCode.Arabic ? taxType.TaxTypeNameAr : taxType.TaxTypeNameEn,
					   TaxValue = item.TaxTypeId == TaxTypeData.Taxable ? (decimal?)vatPercent.VatPercent ?? 0 : 0,

					   PurchasingPrice = item.PurchasingPrice,
					   ConsumerPrice = item.ConsumerPrice,
					   InternalPrice = item.InternalPrice,
					   MaxDiscountPercent = item.MaxDiscountPercent,
					   SalesAccountId = item.SalesAccountId,
					   SalesAccountName = language == LanguageCode.Arabic ? salesAccount.AccountNameAr : salesAccount.AccountNameEn,
					   PurchaseAccountId = item.PurchaseAccountId,
					   PurchaseAccountName = language == LanguageCode.Arabic ? purchaseAccount.AccountNameAr : purchaseAccount.AccountNameEn,
					   MinBuyQuantity = item.MinBuyQuantity,
					   MinSellQuantity = item.MinSellQuantity,
					   MaxBuyQuantity = item.MaxBuyQuantity,
					   MaxSellQuantity = item.MaxSellQuantity,
					   CoverageQuantity = item.CoverageQuantity,
					   IsActive = item.IsActive,
					   InActiveReasons = item.InActiveReasons,
					   NoReplenishment = item.NoReplenishment,
					   IsUnderSelling = item.IsUnderSelling,
					   IsNoStock = item.IsNoStock,
					   IsUntradeable = item.IsUntradeable,
					   IsDeficit = item.IsDeficit,
					   IsPos = item.IsPos,
					   IsOnline = item.IsOnline,
					   IsPoints = item.IsPoints,
					   IsPromoted = item.IsPromoted,
					   IsExpired = item.IsExpired,
					   IsBatched = item.IsBatched,
					   ItemLocation = item.ItemLocation,

					   ItemCategoryId = item.ItemCategoryId,
					   ItemCategoryName = itemCategory != null ? language == LanguageCode.Arabic ? itemCategory.CategoryNameAr : itemCategory.CategoryNameEn : null,
					   ItemSubCategoryId = item.ItemSubCategoryId,
					   ItemSubCategoryName = itemSubCategory != null ? language == LanguageCode.Arabic ? itemSubCategory.SubCategoryNameAr : itemSubCategory.SubCategoryNameEn : null,
					   ItemSectionId = item.ItemSectionId,
					   ItemSectionName = itemSection != null ? language == LanguageCode.Arabic ? itemSection.SectionNameAr : itemSection.SectionNameEn : null,
					   ItemSubSectionId = item.ItemSubSectionId,
					   ItemSubSectionName = itemSubSection != null ? language == LanguageCode.Arabic ? itemSubSection.SubSectionNameAr : itemSubSection.SubSectionNameEn : null,
					   MainItemId = item.MainItemId,
					   MainItemName = mainItem != null ? language == LanguageCode.Arabic ? mainItem.MainItemNameAr : mainItem.MainItemNameEn : null,

					   OtherTaxes = perItemOtherTaxes.GetValueOrDefault(item.ItemId, string.Empty),
					   ItemAttributes = perItemAttributes.GetValueOrDefault(item.ItemId, string.Empty),
					   ItemMenuNotes = perItemMenuNotes.GetValueOrDefault(item.ItemId, string.Empty),

					   CreatedAt = item.CreatedAt,
					   UserNameCreated = item.UserNameCreated,
					   ModifiedAt = item.ModifiedAt,
					   UserNameModified = item.UserNameModified,
				   };
		}
	}
}
