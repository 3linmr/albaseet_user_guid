using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Taxes;
using Compound.CoreOne.Contracts.Reports.Inventory;
using Shared.Helper.Identity;
using Microsoft.AspNetCore.Http;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Models.StaticData;
using Compound.CoreOne.Contracts.Reports.Shared;
using Microsoft.EntityFrameworkCore;
using Compound.CoreOne.Models.Dtos.Reports.Inventory;
using Purchases.CoreOne.Contracts;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Accounts;
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Journal;

namespace Compound.Service.Services.Reports.Inventory
{
	public class ItemsProfitReportService: IItemsProfitReportService
	{
		private readonly IItemService _itemService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly ISalesInvoiceReturnDetailService _salesInvoiceReturnDetailService;
		private readonly IStoreService _storeService;
		private readonly IItemTypeService _itemTypeService;
		private readonly IBranchService _branchService;
		private readonly IVendorService _vendorService;
		private readonly IItemCategoryService _itemCategoryService;
		private readonly IItemSubCategoryService _itemSubCategoryService;
		private readonly IItemSubSectionService _itemSubSectionService;
		private readonly IItemSectionService _itemSectionService;
		private readonly IMainItemService _mainItemService;
		private readonly IAccountService _accountService;
		private readonly ITaxTypeService _taxTypeService;
		private readonly ITaxPercentService _taxPercentService;
		private readonly IItemReportDataService _itemReportDataService;
		private readonly IClientCreditMemoService _clientCreditMemoService;
		private readonly IClientDebitMemoService _clientDebitMemoService;
		private readonly IJournalDetailService _journalDetailService;

		public ItemsProfitReportService(IItemService itemService, IHttpContextAccessor httpContextAccessor, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceDetailService salesInvoiceDetailService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService, IStoreService storeService, IItemTypeService itemTypeService, IBranchService branchService, IVendorService vendorService, IItemCategoryService itemCategoryService, IItemSubCategoryService itemSubCategoryService, IItemSubSectionService itemSubSectionService, IItemSectionService itemSectionService, IMainItemService mainItemService, IAccountService accountService, ITaxTypeService taxTypeService, ITaxPercentService taxPercentService, IItemReportDataService itemReportDataService, IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService, IJournalDetailService journalDetailService)
		{
			_itemService = itemService;
			_httpContextAccessor = httpContextAccessor;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_salesInvoiceDetailService = salesInvoiceDetailService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_salesInvoiceReturnDetailService = salesInvoiceReturnDetailService;
			_storeService = storeService;
			_itemTypeService = itemTypeService;
			_branchService = branchService;
			_vendorService = vendorService;
			_itemCategoryService = itemCategoryService;
			_itemSubCategoryService = itemSubCategoryService;
			_itemSubSectionService = itemSubSectionService;
			_itemSectionService = itemSectionService;
			_mainItemService = mainItemService;
			_accountService = accountService;
			_taxTypeService = taxTypeService;
			_taxPercentService = taxPercentService;
			_itemReportDataService = itemReportDataService;
			_clientCreditMemoService = clientCreditMemoService;
			_clientDebitMemoService = clientDebitMemoService;
			_journalDetailService = journalDetailService;
		}

		public async Task<IQueryable<ItemsProfitReportDto>> GetItemsProfitReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate)
		{
			var today = DateHelper.GetDateTimeNow().Date;
			var companyIds = await _storeService.GetAllStores().Where(x => storeIds.Contains(x.StoreId)).Select(x => x.CompanyId).ToListAsync();
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var perItemOtherTaxes = await _itemReportDataService.GetItemOtherTaxes(companyIds);
			var perItemAttributes = await _itemReportDataService.GetItemAttributes(companyIds);
			var perItemMenuNotes = await _itemReportDataService.GetItemMenuNotes(companyIds);

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

				   from sales in GetTotalItemSales(fromDate, toDate).Where(x => x.ItemId == item.ItemId && x.StoreId == store.StoreId).DefaultIfEmpty()
				   orderby ((sales.CostValue != 0 && sales.CostValue != null) ? (sales.GrossValue - sales.CostValue) / sales.CostValue * 100 : 0) descending
				   select new ItemsProfitReportDto
				   {
					   ItemId = item.ItemId,
					   ItemCode = item.ItemCode,
					   ItemName = item.ItemTypeId == ItemTypeData.Note ? sales.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
					   ItemNameAr = item.ItemNameAr,
					   ItemNameEn = item.ItemNameEn,
					   ItemTypeId = itemType.ItemTypeId,
					   ItemTypeName = language == LanguageCode.Arabic ? itemType.ItemTypeNameAr : itemType.ItemTypeNameEn,

					   StoreId = store.StoreId,
					   StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,

					   NumberOfSales = (decimal?)sales.NumberOfSales ?? 0,
					   QuantitySold = (decimal?)sales.Quantity ?? 0,

					   SellingValue = (decimal?)sales.GrossValue ?? 0,
					   CostValue = (decimal?)sales.CostValue ?? 0,
					   Profit = ((decimal?)sales.GrossValue ?? 0) - ((decimal?)sales.CostValue ?? 0),
					   ProfitPercent = (sales.CostValue != 0 && sales.CostValue != null) ? (sales.GrossValue - sales.CostValue) / sales.CostValue * 100 : 0,

					   AverageCostValue = (decimal?)sales.CostValue / (decimal?)sales.Quantity ?? 0,
					   AverageSellingValue = (decimal?)sales.GrossValue / (decimal?)sales.Quantity ?? 0,

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

		private IQueryable<ItemsProfitReportValuesDto> GetTotalItemSales(DateTime? fromDate, DateTime? toDate)
		{
			var salesInvoices = from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll()
								from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
								where (fromDate == null || salesInvoiceHeader.DocumentDate >= fromDate) &&
									  (toDate == null || salesInvoiceHeader.DocumentDate <= toDate)
								select new ItemsProfitReportValuesDto
								{
									SalesInvoiceHeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
									StoreId = salesInvoiceHeader.StoreId,
									ItemId = salesInvoiceDetail.ItemId,
									GrossValue = salesInvoiceDetail.GrossValue,
									ItemNote = salesInvoiceDetail.ItemNote,
									CostValue = salesInvoiceDetail.CostValue,
									Quantity = (salesInvoiceDetail.Quantity + salesInvoiceDetail.BonusQuantity) * salesInvoiceDetail.Packing,
								};

			var salesInvoiceReturns = from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll()
									  from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId)
									  where (fromDate == null || salesInvoiceReturnHeader.DocumentDate >= fromDate) &&
											(toDate == null || salesInvoiceReturnHeader.DocumentDate <= toDate)
									  select new ItemsProfitReportValuesDto
									  {
									      SalesInvoiceHeaderId = salesInvoiceReturnHeader.SalesInvoiceHeaderId,
										  StoreId = salesInvoiceReturnHeader.StoreId,
										  ItemId = salesInvoiceReturnDetail.ItemId,
										  GrossValue = -salesInvoiceReturnDetail.GrossValue,
										  ItemNote = salesInvoiceReturnDetail.ItemNote,
										  CostValue = -salesInvoiceReturnDetail.CostValue,
									      Quantity = -(salesInvoiceReturnDetail.Quantity + salesInvoiceReturnDetail.BonusQuantity) * salesInvoiceReturnDetail.Packing,
									  };

			var salesInvoiceHeaderTotals = _salesInvoiceHeaderService.GetAll().Select(x => new { x.SalesInvoiceHeaderId, x.GrossValue })
				.Concat(_salesInvoiceReturnHeaderService.GetAll().Select(x => new { x.SalesInvoiceHeaderId, GrossValue = -x.GrossValue }))
				.GroupBy(x => x.SalesInvoiceHeaderId).Select(g => new { SalesInvoiceHeaderId = g.Key, FinalGrossValue = g.Sum(x => x.GrossValue) });

			var salesInvoiceDetailTotals = _salesInvoiceDetailService.GetAll().Select(x => new { x.SalesInvoiceHeaderId, x.ItemId, x.ItemNote, x.GrossValue })
				.Concat(_salesInvoiceReturnDetailService.GetAll().Join(_salesInvoiceReturnHeaderService.GetAll(), x => x.SalesInvoiceReturnHeaderId, x => x.SalesInvoiceReturnHeaderId, (detail, header) => new { header.SalesInvoiceHeaderId, detail.ItemId, detail.ItemNote, GrossValue = -detail.GrossValue }))
				.GroupBy(x => new { x.SalesInvoiceHeaderId, x.ItemId, x.ItemNote }).Select(g => new { g.Key.SalesInvoiceHeaderId, g.Key.ItemId, g.Key.ItemNote, FinalGrossValue = g.Sum(x => x.GrossValue) });

			var clientCreditMemos = from clientCreditMemo in _clientCreditMemoService.GetAll()
								    from taxJournal in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == clientCreditMemo.JournalHeaderId && x.IsTax)
								    from memoValueBeforeTaxJournal in _journalDetailService.GetAll().Where(x => x.JournalDetailId == taxJournal.TaxParentId)
									from headerTotal in salesInvoiceHeaderTotals.Where(x => x.SalesInvoiceHeaderId == clientCreditMemo.SalesInvoiceHeaderId)
									from salesInvoiceDetail in salesInvoiceDetailTotals.Where(x => x.SalesInvoiceHeaderId == clientCreditMemo.SalesInvoiceHeaderId)
									where (fromDate == null || clientCreditMemo.DocumentDate >= fromDate) &&
										  (toDate == null || clientCreditMemo.DocumentDate <= toDate)
									select new ItemsProfitReportValuesDto
									{
										SalesInvoiceHeaderId = clientCreditMemo.SalesInvoiceHeaderId,
										StoreId = clientCreditMemo.StoreId,
										ItemId = salesInvoiceDetail.ItemId,
										GrossValue = -salesInvoiceDetail.FinalGrossValue * (memoValueBeforeTaxJournal.DebitValue / headerTotal.FinalGrossValue),
										ItemNote = salesInvoiceDetail.ItemNote,
										CostValue = 0,
										Quantity = 0,
									};

			var clientDebitMemos = from clientDebitMemo in _clientDebitMemoService.GetAll()
								   from taxJournal in _journalDetailService.GetAll().Where(x => x.JournalHeaderId == clientDebitMemo.JournalHeaderId && x.IsTax)
								   from memoValueBeforeTaxJournal in _journalDetailService.GetAll().Where(x => x.JournalDetailId == taxJournal.TaxParentId)
								   from headerTotal in salesInvoiceHeaderTotals.Where(x => x.SalesInvoiceHeaderId == clientDebitMemo.SalesInvoiceHeaderId)
								   from salesInvoiceDetail in salesInvoiceDetailTotals.Where(x => x.SalesInvoiceHeaderId == clientDebitMemo.SalesInvoiceHeaderId)
								   where (fromDate == null || clientDebitMemo.DocumentDate >= fromDate) &&
										 (toDate == null || clientDebitMemo.DocumentDate <= toDate)
								   select new ItemsProfitReportValuesDto
								   {
									   SalesInvoiceHeaderId = clientDebitMemo.SalesInvoiceHeaderId,
									   StoreId = clientDebitMemo.StoreId,
									   ItemId = salesInvoiceDetail.ItemId,
									   GrossValue = salesInvoiceDetail.FinalGrossValue * (memoValueBeforeTaxJournal.CreditValue / headerTotal.FinalGrossValue),
									   ItemNote = salesInvoiceDetail.ItemNote,
									   CostValue = 0,
									   Quantity = 0,
								   };


			return salesInvoices.Concat(salesInvoiceReturns).Concat(clientDebitMemos).Concat(clientCreditMemos).GroupBy(x => new { x.SalesInvoiceHeaderId, x.StoreId, x.ItemId, x.ItemNote }).Select(x => new
				{
					StoreId = x.Key.StoreId,
					ItemId = x.Key.ItemId,
					ItemNote = x.Key.ItemNote,
					Quantity = x.Sum(y => y.Quantity),
					GrossValue = x.Sum(y => y.GrossValue),
					CostValue = x.Sum(y => y.CostValue),
				})
				.Where(x => x.Quantity > 0 || x.GrossValue > 0 || x.CostValue > 0).GroupBy(x => new { x.StoreId, x.ItemId, x.ItemNote }) //if salesInvoice is returned completely, it does not count as sale
				.Select(x => new ItemsProfitReportValuesDto			
				{
					StoreId = x.Key.StoreId,
					ItemId = x.Key.ItemId,
					ItemNote = x.Key.ItemNote,
					NumberOfSales = x.Count(),
					GrossValue = x.Sum(y => y.GrossValue),
					CostValue = x.Sum(y => y.CostValue),
					Quantity = x.Sum(y => y.Quantity),
				});
		}
	}
}
