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
using Shared.Helper.Logic;
using Shared.CoreOne.Contracts.Accounts;
using Sales.CoreOne.Contracts;

namespace Compound.Service.Services.Reports.Inventory
{
	public class StockTakingsReportService: IStockTakingsReportService
	{
		private readonly IItemService _itemService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IGeneralInventoryDocumentService _generalInventoryDocumentService;
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
		private readonly IItemPackageService _itemPackageService;
		private readonly IStoreService _storeService;
		private readonly IItemCategoryService _itemCategoryService;
		private readonly IItemSectionService _itemSectionService;
		private readonly IItemSubSectionService _itemSubSectionService;
		private readonly IItemSubCategoryService _itemSubCategoryService;
		private readonly IMainItemService _mainItemService;
		private readonly IItemTypeService _itemTypeService;
		private readonly IBranchService _branchService;
		private readonly IItemBarCodeService _itemBarCodeService;
		private readonly IItemBarCodeDetailService _itemBarCodeDetailService;
		private readonly IVendorService _vendorService;
		private readonly ITaxPercentService _taxPercentService;
		private readonly ITaxService _taxService;
		private readonly IItemTaxService _itemTaxService;
		private readonly ITaxTypeService _taxTypeService;
		private readonly IAccountService _accountService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
		private readonly IClientService _clientService;
		private readonly IItemAttributeService _itemAttributeService;
		private readonly IItemCostService _itemCostService;
		private readonly IItemPackingService _itemPackingService;
		private readonly IItemReportDataService _itemReportDataService;

		public StockTakingsReportService(IItemService itemService, IHttpContextAccessor httpContextAccessor, IGeneralInventoryDocumentService generalInventoryDocumentService, IItemCurrentBalanceService itemCurrentBalanceService, IItemPackageService itemPackageService, IStoreService storeService, IItemCategoryService itemCategoryService, IItemSectionService itemSectionService, IItemSubSectionService itemSubSectionService, IItemSubCategoryService itemSubCategoryService, IMainItemService mainItemService, IItemTypeService itemTypeService, IBranchService branchService, IItemBarCodeService itemBarCodeService, IItemBarCodeDetailService itemBarCodeDetailService, IVendorService vendorService, ITaxPercentService taxPercentService, ITaxService taxService, IItemTaxService itemTaxService, ITaxTypeService taxTypeService, IAccountService accountService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceDetailService salesInvoiceDetailService, IClientService clientService, IItemAttributeService itemAttributeService, IItemCostService itemCostService, IItemPackingService itemPackingService, IItemReportDataService itemReportDataService)
		{
			_itemService = itemService;
			_httpContextAccessor = httpContextAccessor;
			_generalInventoryDocumentService = generalInventoryDocumentService;
			_itemCurrentBalanceService = itemCurrentBalanceService;
			_itemPackageService = itemPackageService;
			_storeService = storeService;
			_itemCategoryService = itemCategoryService;
			_itemSectionService = itemSectionService;
			_itemSubSectionService = itemSubSectionService;
			_itemSubCategoryService = itemSubCategoryService;
			_mainItemService = mainItemService;
			_itemTypeService = itemTypeService;
			_branchService = branchService;
			_itemBarCodeService = itemBarCodeService;
			_itemBarCodeDetailService = itemBarCodeDetailService;
			_vendorService = vendorService;
			_taxPercentService = taxPercentService;
			_taxService = taxService;
			_itemTaxService = itemTaxService;
			_taxTypeService = taxTypeService;
			_accountService = accountService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_salesInvoiceDetailService = salesInvoiceDetailService;
			_clientService = clientService;
			_itemAttributeService = itemAttributeService;
			_itemCostService = itemCostService;
			_itemPackingService = itemPackingService;
			_itemReportDataService = itemReportDataService;
		}


		public async Task<IQueryable<StockTakingsReportDto>> GetStockTakingsReport(List<int> storeIds, DateTime? fromDate, DateTime? toDate, int? itemCategoryId, int? itemSubCategoryId, int? itemSectionId, int? itemSubSectionId, int? mainItemId, DateTime? expireBefore)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyIds = await _storeService.GetAllStores().Where(x => storeIds.Contains(x.StoreId)).Select(x => x.CompanyId).ToListAsync();

			var today = DateHelper.GetDateTimeNow();

			var salesInvoicesWithDateAndItemNote = from salesInvoiceDetail in _salesInvoiceDetailService.GetAll()
												   from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceDetail.SalesInvoiceHeaderId)
												   select new
												   {
													   SalesInvoiceHeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
													   DocumentFullCode = salesInvoiceHeader.Prefix + salesInvoiceHeader.DocumentCode + salesInvoiceHeader.Suffix,
													   DocumentDate = salesInvoiceHeader.DocumentDate,
													   StoreId = salesInvoiceHeader.StoreId,
													   ClientId = salesInvoiceHeader.ClientId,
													   ItemId = salesInvoiceDetail.ItemId,
													   ItemPackageId = salesInvoiceDetail.ItemPackageId,
													   ItemNote = salesInvoiceDetail.ItemNote,
													   BarCode = salesInvoiceDetail.BarCode,
													   ExpireDate = salesInvoiceDetail.ExpireDate,
													   BatchNumber = salesInvoiceDetail.BatchNumber,
												   };

			var perItemOtherTaxes = await _itemReportDataService.GetItemOtherTaxes(companyIds);
			var perItemAttributes = await _itemReportDataService.GetItemAttributes(companyIds);
			var perItemMenuNotes = await _itemReportDataService.GetItemMenuNotes(companyIds);

			//items and packages in the storeIds that don't have any movements
			//should still show in the report but with quantities zero

			var report = from item in _itemService.GetAll().Where(x => companyIds.Contains(x.CompanyId) && x.ItemTypeId != ItemTypeData.Service &&
						     (itemCategoryId == null || x.ItemCategoryId == itemCategoryId) &&
						     (itemSubCategoryId == null || x.ItemSubCategoryId == itemSubCategoryId) &&
						     (itemSectionId == null || x.ItemSectionId == itemSectionId) &&
						     (itemSubSectionId == null || x.ItemSubSectionId == itemSubSectionId) &&
						     (mainItemId == null || x.MainItemId == mainItemId)
				         )
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

				         from itemBarCode in _itemBarCodeService.GetAll().Where(x => x.ItemId == item.ItemId)
						 from itemPackage in _itemPackageService.GetAll().Where(x => x.ItemPackageId == itemBarCode.FromPackageId)
				         from itemPacking in _itemPackingService.GetAll().Where(x => x.ItemId == item.ItemId && x.FromPackageId == itemPackage.ItemPackageId && x.ToPackageId == item.SingularPackageId).DefaultIfEmpty()
				         from itemCost in _itemCostService.GetAll().Where(x => x.ItemId == item.ItemId && x.StoreId == store.StoreId).DefaultIfEmpty()

						 from quantity in GetReportQuantitiesCalculated(fromDate, toDate).Where(x => x.ItemId == item.ItemId && x.StoreId == store.StoreId && x.ItemPackageId == itemPackage.ItemPackageId).DefaultIfEmpty()
						 from itemBarCodeDetail in _itemBarCodeDetailService.GetAll().Where(x => x.ItemBarCodeId == itemBarCode.ItemBarCodeId && x.BarCode == quantity.BarCode).DefaultIfEmpty()

						 from lastSalesInvoice in salesInvoicesWithDateAndItemNote.Where(x => x.StoreId == store.StoreId && x.ItemId == item.ItemId && x.ItemPackageId == itemPackage.ItemPackageId && x.ItemNote == quantity.ItemNote && x.ExpireDate == quantity.ExpireDate && x.BatchNumber == quantity.BatchNumber && x.BarCode == quantity.BarCode).OrderByDescending(x => x.DocumentDate).Take(1).DefaultIfEmpty()
						 from lastSalesInvoiceClient in _clientService.GetAll().Where(x => x.ClientId == lastSalesInvoice.ClientId).DefaultIfEmpty()

						 where expireBefore == null || (quantity.ExpireDate != null && quantity.ExpireDate <= expireBefore)
						 select new StockTakingsReportDto
						 {
							 ItemId = item.ItemId,
							 ItemCode = item.ItemCode,
							 ItemName = item.ItemTypeId == ItemTypeData.Note ? quantity.ItemNote : (language == LanguageCode.Arabic ? item.ItemNameAr : item.ItemNameEn),
							 ItemNameAr = item.ItemNameAr,
							 ItemNameEn = item.ItemNameEn,
							 ItemTypeId = itemType.ItemTypeId,
							 ItemTypeName = language == LanguageCode.Arabic ? itemType.ItemTypeNameAr : itemType.ItemTypeNameEn,

							 BeginCurrentBalance = quantity.BeginCurrentBalance != null ? quantity.BeginCurrentBalance : 0,
							 BeginAvailableBalance = quantity.BeginAvailableBalance != null ? quantity.BeginAvailableBalance : 0,
							 InQuantity = quantity.InQuantity != null ? quantity.InQuantity : 0,
							 OutQuantity = quantity.OutQuantity != null ? quantity.OutQuantity : 0,
							 PendingOutQuantity = quantity.PendingOutQuantity != null ? quantity.PendingOutQuantity : 0,
							 EndCurrentBalance = quantity.EndCurrentBalance != null ? quantity.EndCurrentBalance : 0,
							 EndAvailableBalance = quantity.EndAvailableBalance != null ? quantity.EndAvailableBalance : 0,
							 ReservedQuantity = quantity.ReservedQuantity != null ? quantity.ReservedQuantity : 0,

					         CostPrice = itemCost != null ? itemCost.CostPrice : 0,
					         Packing = itemPacking != null ? itemPacking.Packing : 1,
					         CostPackage = itemCost != null ? itemCost.CostPrice * (itemPacking != null ? itemPacking.Packing : 1) : 0,
					         CostValue = itemCost != null && quantity.EndCurrentBalance != null ? itemCost.CostPrice * (itemPacking != null ? itemPacking.Packing : 1) * quantity.EndCurrentBalance : 0,
							 SellingPackage = ((decimal?)itemBarCodeDetail.ConsumerPrice ?? item.ConsumerPrice),
							 SellingValue = ((decimal?)itemBarCodeDetail.ConsumerPrice ?? item.ConsumerPrice) * (quantity.EndCurrentBalance != null ? quantity.EndCurrentBalance : 0),

							 ReorderPointQuantity = item.ReorderPointQuantity,

							 VendorId = vendor.VendorId,
							 VendorCode = vendor.VendorCode,
							 VendorName = language == LanguageCode.Arabic ? vendor.VendorNameAr : vendor.VendorNameEn,

							 TaxTypeName = language == LanguageCode.Arabic ? taxType.TaxTypeNameAr : taxType.TaxTypeNameEn,
							 TaxValue = item.TaxTypeId == TaxTypeData.Taxable ? (decimal?)vatPercent.VatPercent ?? 0 : 0,
							 OtherTaxes = perItemOtherTaxes.GetValueOrDefault(item.ItemId, ""),

							 StoreId = store.StoreId,
							 StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
							 ItemPackageId = itemPackage.ItemPackageId,
							 ItemPackageCode = itemPackage.ItemPackageCode,
							 ItemPackageName = language == LanguageCode.Arabic ? itemPackage.PackageNameAr : itemPackage.PackageNameEn,
							 ExpireDate = quantity.ExpireDate != null ? quantity.ExpireDate : null,
							 BatchNumber = quantity.BatchNumber != null ? quantity.BatchNumber : null,
							 BarCode = quantity.BarCode != null ? quantity.BarCode : null,

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

							 ItemAttributes = perItemAttributes.GetValueOrDefault(item.ItemId, ""),
							 ItemMenuNotes = perItemMenuNotes.GetValueOrDefault(item.ItemId, ""),

							 LastSalesInvoiceDate = lastSalesInvoice.DocumentDate,
							 LastSalesInvoiceFullCode = lastSalesInvoice.DocumentFullCode,
							 LastSalesInvoiceClientName = language == LanguageCode.Arabic ? lastSalesInvoiceClient.ClientNameAr : lastSalesInvoiceClient.ClientNameEn,

							 CreatedAt = item.CreatedAt,
							 UserNameCreated = item.UserNameCreated,
							 ModifiedAt = item.ModifiedAt,
							 UserNameModified = item.UserNameModified,
						 };

			return report.AsNoTracking();
		}

		private IQueryable<StockTakingsReportQuantityDto> GetReportQuantitiesCalculated(DateTime? fromDate, DateTime? toDate)
		{
			return from generalInventoryDocument in _generalInventoryDocumentService.GetGeneralInventoryDocuments()
				   group generalInventoryDocument by new { generalInventoryDocument.StoreId, generalInventoryDocument.ItemId, generalInventoryDocument.ItemNote, generalInventoryDocument.ItemPackageId, generalInventoryDocument.ExpireDate, generalInventoryDocument.BatchNumber, generalInventoryDocument.BarCode } into g
				   select new StockTakingsReportQuantityDto
				   {
					   StoreId = g.Key.StoreId,
					   ItemId = g.Key.ItemId,
					   ItemNote = g.Key.ItemNote,
					   ItemPackageId = g.Key.ItemPackageId,
					   ExpireDate = g.Key.ExpireDate,
					   BatchNumber = g.Key.BatchNumber,
					   BarCode = g.Key.BarCode,
					   BeginCurrentBalance = g.Sum(
						   x => (fromDate != null && x.DocumentDate < fromDate) ?
							   x.InQuantity - x.OutQuantity + x.OpenQuantity
							   : 0
						  ),
					   BeginAvailableBalance = g.Sum(
						   x => (fromDate != null && x.DocumentDate < fromDate) ?
							   x.InQuantity - x.OutQuantity + x.PendingInQuantity - x.PendingOutQuantity + x.OpenQuantity
							   : 0
						  ),
					   InQuantity = g.Sum(
						   x => (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) ?
							   x.InQuantity + x.PendingInQuantity + x.OpenQuantity
							   : 0
						  ),
					   OutQuantity = g.Sum(
						   x => (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) ?
							   x.OutQuantity
							   : 0
						  ),
					   PendingOutQuantity = g.Sum(
						   x => (fromDate == null || x.DocumentDate >= fromDate) && (toDate == null || x.DocumentDate <= toDate) ?
							   x.PendingOutQuantity
							   : 0
						  ),
					   EndCurrentBalance = g.Sum(
						   x => (toDate == null || x.DocumentDate <= toDate) ?
							   x.InQuantity - x.OutQuantity + x.OpenQuantity :
							   0
						  ),
					   EndAvailableBalance = g.Sum(
						   x => (toDate == null || x.DocumentDate <= toDate) ?
							   x.InQuantity - x.OutQuantity + x.PendingInQuantity - x.PendingOutQuantity + x.OpenQuantity :
							   0
						  ),
					   ReservedQuantity = g.Sum(
						   x => (toDate == null || x.DocumentDate <= toDate) ?
							   x.ReservedQuantity
							   : 0
						  ),
				   };
		}
	}
}
